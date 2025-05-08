using MediatR;
using MediatR.Pipeline;
using StargateAPI.Business.Data;
using StargateAPI.Business.Data.Repositories;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreatePerson : IRequest<CreatePersonResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson>
    {
        private readonly IRepository<Person> _personRepository;
        public CreatePersonPreProcessor(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }
        public Task Process(CreatePerson request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new BadHttpRequestException("Invalid person name.");

            var person =  _personRepository.GetAll(x => x.Name == request.Name).FirstOrDefault();

            if (person is not null) throw new BadHttpRequestException("Person with same name exists.");

            return Task.CompletedTask;
        }
    }

    public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult>
    {
		private readonly IRepository<Person> _personRepository;

		public CreatePersonHandler(IRepository<Person> personRepository)
        {
			_personRepository = personRepository;
		}
        public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            var newPerson = new Person()
            {
                Name = request.Name
            };

            await _personRepository.AddAsync(newPerson);

            return new CreatePersonResult()
            {
                Id = newPerson.Id
            };       
        }
    }

    public class CreatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
