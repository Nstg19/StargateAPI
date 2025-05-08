using Moq;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Data.Repositories;

namespace StargateAPI.Tests.UnitTests;

[TestClass]
public class CreatePersonHandlerTests
{
	private CreatePersonHandler _handler;
	private Mock<IRepository<Person>> _personRepoMock;

	[TestInitialize]
	public void Initialize()
	{
		_personRepoMock = new Mock<IRepository<Person>>();
		_handler = new CreatePersonHandler(_personRepoMock.Object);
	}

	[TestMethod]
	public async Task Handle_ValidRequest_ShouldAddPerson()
	{
		// Arrange
		var request = new CreatePerson
		{
			Name = "James"
		};

		// Act
		var result = await _handler.Handle(request, CancellationToken.None);

		// Assert
		_personRepoMock.Verify(x => x.AddAsync(It.Is<Person>(z => z.Name == request.Name)), Times.Once);
	}
}
