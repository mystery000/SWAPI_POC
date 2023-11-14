using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWAPI_POC.Controllers;
using Moq;
using MediatR;
using Microsoft.AspNetCore.Http;
using SWAPI_POC_Core.Models;
using SWAPI_POC.Resources.Swapi.Operations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI.Models;
using System.Diagnostics;
using Newtonsoft.Json;

namespace SWAPI_POC.Tests
{
    [TestClass]
    public class SwapiControllerTests
    {
        private SwapiController _swapiController;
        private Mock<IMediator> _mediatorMock;

        [TestInitialize]
        public void Initialize()
        {
            _mediatorMock = new Mock<IMediator>();
            _swapiController = new SwapiController(_mediatorMock.Object);
        }

        [TestMethod]
        public async Task GetAllStarshipsByPersonName_ReturnsOkObjectResult()
        {
            // Arrange
            var expectedStarships = new List<Starship>(); 

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStarships.GetStarshipsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetStarships.QueryResponse { Success = true, Message = "Success", Data = expectedStarships });

            // Act
            var result = await _swapiController.GetAllStarshipsByPersonName(new GetStarships.GetStarshipsCommand());    // Default PersonName is 'Luke Skywalker'

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetAllSpeciesClassification_ReturnsOkObjectResult()
        {
            // Arrange
            var expectedSpeciesClassifications = new List<string>();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllSpecies.GetAllSpeciesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetAllSpecies.QueryResponse { Success = true, Message = "Success", Data = expectedSpeciesClassifications });

            // Act
            var result = await _swapiController.GetAllSpeciesClassification();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetTotalPopulationOfAllPlanets_ReturnsOkObjectResult()
        {
            // Arrange
            var expectedTotalPopulation = String.Empty; 

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTotalPopulation.GetTotalPopulationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTotalPopulation.QueryResponse { Success = true, Message = "Success", Data = expectedTotalPopulation });

            // Act
            var result = await _swapiController.GetTotalPopulationOfAllPlanets();

            // Console.WriteLine(JsonConvert.SerializeObject(result));
            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }
       
    }
}
