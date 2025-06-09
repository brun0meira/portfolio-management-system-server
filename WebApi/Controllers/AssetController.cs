using Domain.Dto;
using Domain.Interfaces;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IAssetBusiness _assetBusiness;

        public AssetsController(IAssetRepository assetRepository, IQuoteRepository quoteRepository, IAssetBusiness assetBusiness)
        {
            _assetRepository = assetRepository;
            _quoteRepository = quoteRepository;
            _assetBusiness = assetBusiness;
        }

        // Listar ativos disponíveis
        [HttpGet]
        public async Task<IActionResult> GetAssets()
        {
            var assets = await _assetRepository.GetAllAssetsAsync();
            return Ok(assets);
        }

        // Obter última cotação de um ativo.
        [HttpGet("{assetSymbol}/quote")]
        public async Task<IActionResult> GetLatestQuote(string assetSymbol)
        {
            var quote = await _quoteRepository.GetLatestQuoteByAssetSymbolAsync(assetSymbol.ToUpper());

            if (quote == null)
                return NotFound($"No quote found for asset: {assetSymbol}");

            return Ok(quote);
        }

        // Retornar histórico de cotações de um ativo (períodos: 7d,30d,1y,5y)
        [HttpGet("{assetSymbol}/history")]
        public async Task<IActionResult> GetAssetHistory(string assetSymbol, [FromQuery] string period = "7d")
        {
            try
            {
                var history = await _assetBusiness.GetQuoteHistoryAsync(assetSymbol, period);
                return Ok(history);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                // Logar o erro em produção
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // Calcular volatilidade de um ativo para um período
        [HttpGet("{assetSymbol}/volatility")]
        public async Task<IActionResult> GetVolatility(string assetSymbol, [FromQuery] string period = "30d")
        {
            try
            {
                var volatility = await _assetBusiness.CalculateVolatilityAsync(assetSymbol, period);
                return Ok(new { AssetSymbol = assetSymbol, Period = period, Volatility = volatility });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // Registrar novo asset
        [HttpPost("asset/create")]
        public async Task<IActionResult> RegisterAsset([FromBody] CreateAssetDto dto)
        {
            await _assetRepository.AddAssetAsync(dto);

            return Ok("Ativo registrada com sucesso.");
        }

        // 4. e receba todas as compras de um ativo e calcule o preço médio ponderado de aquisição.
        [HttpGet("average-price/asset")]
        public async Task<ActionResult<AveragePriceResponseDto>> GetAveragePrice([FromQuery] Guid userId, [FromQuery] Guid assetId)
        {
            try
            {
                var averagePrice = await _assetBusiness.CalculateWeightedAveragePriceAsync(userId, assetId);

                return Ok(new AveragePriceResponseDto
                {
                    AssetId = assetId,
                    AveragePrice = Math.Round(averagePrice, 2)
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

}
