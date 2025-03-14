using System;
using Moq;
using Moq.Protected;

namespace PokeApiApp.Tests
{
    [TestClass]
    public class ApiServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private ApiService _apiService;

        [TestInitialize]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _apiService = new ApiService(_httpClient);
        }

        [TestMethod]
        public async Task GetPokemonAsync_ShouldReturnPokemon_WhenValidNameIsGiven()
        {
            var mockResponse = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent("{ \"name\": \"pikachu\", \"height\": 4, \"weight\": 60 }")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<System.Threading.CancellationToken>())
                .ReturnsAsync(mockResponse);

            var pokemon = await _apiService.GetPokemonAsync("pikachu");

            Assert.IsNotNull(pokemon);
            Assert.AreEqual("pikachu", pokemon.Name);
            Assert.AreEqual(4, pokemon.Height);
            Assert.AreEqual(60, pokemon.Weight);
        }

        [TestMethod]
        public async Task GetPokemonAsync_ShouldReturnNull_WhenPokemonNotFound()
        {
            var mockResponse = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.NotFound
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<System.Threading.CancellationToken>())
                .ReturnsAsync(mockResponse);

            var pokemon = await _apiService.GetPokemonAsync("unknownpokemon");

            Assert.IsNull(pokemon);
        }

        [TestMethod]
        public async Task GetPokemonAsync_ShouldThrowException_WhenNetworkFails()
        {
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<System.Threading.CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => _apiService.GetPokemonAsync("pikachu"));
        }
    }
}
