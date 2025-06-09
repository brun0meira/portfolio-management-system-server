using Domain.Business;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Domain.Entities;
using Domain.Enum;
using System.Linq;

namespace Tests.Business
{
    public class AssetBusinessTests
    {
        private readonly Mock<IAssetRepository> _assetRepositoryMock;
        private readonly Mock<ITradeRepository> _tradeRepositoryMock;
        private readonly AssetBusiness _assetBusiness;

        public AssetBusinessTests()
        {
            _assetRepositoryMock = new Mock<IAssetRepository>();
            _tradeRepositoryMock = new Mock<ITradeRepository>();

            _assetBusiness = new AssetBusiness(_assetRepositoryMock.Object, _tradeRepositoryMock.Object);
        }

        [Fact]
        public async Task CalculateWeightedAveragePriceAsync_ReturnsCorrectAverage()
        {
            var userId = Guid.NewGuid();
            var assetId = Guid.NewGuid();

            var trades = new List<Trade>
            {
                new Trade { Quantity = 10, UnitPrice = 20, Fee = 1 },
                new Trade { Quantity = 5, UnitPrice = 30, Fee = 2 },
            };

            _tradeRepositoryMock
                .Setup(repo => repo.GetBuyTradesByAssetAsync(userId, assetId))
                .ReturnsAsync(trades);

            var result = await _assetBusiness.CalculateWeightedAveragePriceAsync(userId, assetId);

            // Assert
            // total cost: (10*20+1) + (5*30+2) = 201 + 152 = 353
            // total quantity = 15
            // expected average = 353 / 15 = 23.53
            Assert.Equal(23.53m, Math.Round(result, 2));
        }

        [Fact]
        public async Task CalculateWeightedAveragePriceAsync_Throws_WhenNoTrades()
        {
            var userId = Guid.NewGuid();
            var assetId = Guid.NewGuid();

            _tradeRepositoryMock
                .Setup(repo => repo.GetBuyTradesByAssetAsync(userId, assetId))
                .ReturnsAsync(new List<Trade>());

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _assetBusiness.CalculateWeightedAveragePriceAsync(userId, assetId));

            Assert.Equal("Nenhuma operação de compra encontrada para este ativo.", exception.Message);
        }

        [Fact]
        public async Task CalculateWeightedAveragePriceAsync_Throws_WhenInvalidTradeData()
        {
            var userId = Guid.NewGuid();
            var assetId = Guid.NewGuid();

            var trades = new List<Trade>
            {
                new Trade { Quantity = 0, UnitPrice = 10, Fee = 0 }
            };

            _tradeRepositoryMock
                .Setup(repo => repo.GetBuyTradesByAssetAsync(userId, assetId))
                .ReturnsAsync(trades);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _assetBusiness.CalculateWeightedAveragePriceAsync(userId, assetId));

            Assert.Equal("Operação com dados inválidos encontrada.", exception.Message);
        }

        [Fact]
        public async Task CalculateWeightedAveragePriceAsync_Throws_WhenTotalQuantityIsZero()
        {
            var userId = Guid.NewGuid();
            var assetId = Guid.NewGuid();

            var trades = new List<Trade>
            {
                new Trade { Quantity = 0, UnitPrice = 0, Fee = 0 }
            };

            _tradeRepositoryMock
                .Setup(repo => repo.GetBuyTradesByAssetAsync(userId, assetId))
                .ReturnsAsync(trades);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _assetBusiness.CalculateWeightedAveragePriceAsync(userId, assetId));

            Assert.Equal("Operação com dados inválidos encontrada.", exception.Message);
        }
    }
}
