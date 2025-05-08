using Dapper;
using MediatR;
using MediatR.Pipeline;
using StargateAPI.Business.Data;
using StargateAPI.Business.Data.Repositories;
using StargateAPI.Controllers;
using System.ComponentModel.DataAnnotations;

namespace StargateAPI.Business.Commands
{
    public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
    {
        [Required]
        public required string Name { get; set; }

		[Required]
		public required string Rank { get; set; }

		[Required]
		public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class CreateAstronautDutyPreProcessor : IRequestPreProcessor<CreateAstronautDuty>
    {
		private readonly IRepository<Person> _personRepository;
		private readonly IRepository<AstronautDuty> _astronautDutyRepository;

		public CreateAstronautDutyPreProcessor(IRepository<Person> personRepository,
			IRepository<AstronautDuty> astronautDutyRepository)
        {
			_personRepository = personRepository;
            _astronautDutyRepository = astronautDutyRepository;
		}

        public Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = _personRepository.GetAll(x => x.Name == request.Name).FirstOrDefault();

			if (person is null) throw new BadHttpRequestException($"Person not found.");

            var verifyNoPreviousDuty = _astronautDutyRepository
				.GetAll(z => z.PersonId == person.Id && z.DutyTitle == request.DutyTitle &&
                z.DutyStartDate.Date == request.DutyStartDate.Date && z.Rank == request.Rank).FirstOrDefault();

            if (verifyNoPreviousDuty is not null) throw new BadHttpRequestException("Previous duty exists for the person.");

            return Task.CompletedTask;
        }
    }

    public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        private readonly StargateContext _context;

        public CreateAstronautDutyHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {

            var query = $"SELECT * FROM [Person] WHERE \'{request.Name}\' = Name";

            var person = await _context.Connection.QueryFirstOrDefaultAsync<Person>(query);

            query = $"SELECT * FROM [AstronautDetail] WHERE {person.Id} = PersonId";

            var astronautDetail = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDetail>(query);

            if (astronautDetail == null)
            {
                astronautDetail = new AstronautDetail();
                astronautDetail.PersonId = person.Id;
                astronautDetail.CurrentDutyTitle = request.DutyTitle;
                astronautDetail.CurrentRank = request.Rank;
                astronautDetail.CareerStartDate = request.DutyStartDate.Date;
                if (request.DutyTitle == "RETIRED")
                {
                    astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }

                await _context.AstronautDetails.AddAsync(astronautDetail);

            }
            else
            {
                astronautDetail.CurrentDutyTitle = request.DutyTitle;
                astronautDetail.CurrentRank = request.Rank;
                if (request.DutyTitle == "RETIRED")
                {
                    astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }
                _context.AstronautDetails.Update(astronautDetail);
            }

            query = $"SELECT * FROM [AstronautDuty] WHERE {person.Id} = PersonId Order By DutyStartDate Desc";

            var astronautDuty = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDuty>(query);

            if (astronautDuty != null)
            {
                astronautDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                _context.AstronautDuties.Update(astronautDuty);
            }

            var newAstronautDuty = new AstronautDuty()
            {
                PersonId = person.Id,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate.Date,
                DutyEndDate = null
            };

            await _context.AstronautDuties.AddAsync(newAstronautDuty);

            await _context.SaveChangesAsync();

            return new CreateAstronautDutyResult()
            {
                Id = newAstronautDuty.Id
            };
        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}
