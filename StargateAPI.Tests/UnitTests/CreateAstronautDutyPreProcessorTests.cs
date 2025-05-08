using Moq;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data.Repositories;
using StargateAPI.Business.Data;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace StargateAPI.Tests.UnitTests;

[TestClass]
public class CreateAstronautDutyPreProcessorTests
{
	private CreateAstronautDutyPreProcessor _preProcessor;
	private Mock<IRepository<Person>> _personRepoMock;
	private Mock<IRepository<AstronautDuty>> _astronautDutyRepoMock;

	[TestInitialize]
	public void Initialize()
	{
		_personRepoMock = new Mock<IRepository<Person>>();
		_astronautDutyRepoMock = new Mock<IRepository<AstronautDuty>>();
		_preProcessor = new CreateAstronautDutyPreProcessor(_personRepoMock.Object, _astronautDutyRepoMock.Object);

		_personRepoMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Person, bool>>>()))
			.Returns((Expression<Func<Person, bool>> predicate) =>
			{
				return GetPeople().AsQueryable().Where(predicate.Compile()).AsQueryable();
			});

		_astronautDutyRepoMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<AstronautDuty, bool>>>()))
			.Returns((Expression<Func<AstronautDuty, bool>> predicate) =>
			{
				return GetAstronautDuties().AsQueryable().Where(predicate.Compile()).AsQueryable();
			});
	}

	[TestMethod]
	[ExpectedException(typeof(BadHttpRequestException))]
	public void PreProcess_InvalidPerson_ShouldThrowBadHttpRequestException()
	{
		// Arrange
		var request = new CreateAstronautDuty
		{
			Name = "James",
			Rank = "1LT",
			DutyTitle = "Commander",
			DutyStartDate = DateTime.Now.Date,
		};

		// Act
		_preProcessor.Process(request, CancellationToken.None);
	}

	[TestMethod]
	[ExpectedException(typeof(BadHttpRequestException))]
	public void PreProcess_ValidPersonWithPreviousDuty_ShouldThrowBadHttpRequestException()
	{
		// Arrange
		var request = new CreateAstronautDuty
		{
			Name = "James",
			Rank = "1LT",
			DutyTitle = "Commander",
			DutyStartDate = DateTime.Now.Date,
		};

		// Act
		_preProcessor.Process(request, CancellationToken.None);
	}

	private List<Person> GetPeople()
	{
		return new List<Person>
			{
				new Person
				{
					Id = 1,
					Name = "James"
				},
				new Person
				{
					Id = 2,
					Name = "Jake"
				}
			};
	}

	private List<AstronautDuty> GetAstronautDuties()
	{
		return new List<AstronautDuty>
			{
				new AstronautDuty {
					Id = 1,
					PersonId = 1,
					Rank = "1LT",
					DutyTitle = "Commander",
					DutyStartDate = DateTime.Now.Date
				}
			};
	}
}

