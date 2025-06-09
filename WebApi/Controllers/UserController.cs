using Domain.Business;
using Domain.Dto;
using Domain.Interfaces;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

public class UserController : ControllerBase
{
    private readonly ITradeRepository _tradeRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDividendRepository _dividendRepository;
    private readonly ITradeBusiness _tradeBusiness;
    private readonly IReportRepository _reportRepository;

    public UserController(ITradeRepository tradeRepository, IPositionRepository positionRepository, IUserRepository userRepository, IDividendRepository dividendRepository, ITradeBusiness tradeBusiness, IReportRepository reportRepository)
    {

        _tradeRepository = tradeRepository;
        _positionRepository = positionRepository;
        _userRepository = userRepository;
        _dividendRepository = dividendRepository;
        _tradeBusiness = tradeBusiness;
        _reportRepository = reportRepository;
    }

    // Listar as operações de um usuario
    [HttpGet("{userId}/operations")]
    public async Task<IActionResult> GetUserOperations(Guid userId)
    {
        var trades = await _tradeRepository.GetTradesByUserIdAsync(userId);

        if (trades == null || !trades.Any())
            return NotFound(new { message = "Nenhuma operação encontrada para o usuário informado." });

        return Ok(trades);
    }

    // Obter posição consolidada de todos os ativos de um usuario
    [HttpGet("{userId}/positions")]
    public async Task<IActionResult> GetUserPositions(Guid userId)
    {
        var positions = await _positionRepository.GetPositionsByUserIdAsync(userId);

        if (positions == null || !positions.Any())
            return NotFound(new { message = "Nenhuma posição encontrada para o usuário informado." });

        return Ok(positions);
    }

    // Obter posição de um ativo especifico de um user especifico
    [HttpGet("{userId}/positions/{assetCode}")]
    public async Task<IActionResult> GetUserPositionByAsset(Guid userId, string assetCode)
    {
        var position = await _positionRepository.GetUserAssetPositionAsync(userId, assetCode.ToUpper());

        if (position == null)
            return NotFound(new { message = "Posição não encontrada para o usuário e ativo especificados." });

        return Ok(position);
    }

    // Total de corretagem paga pelo usuário.
    [HttpGet("{userId}/brokerage-total")]
    public async Task<IActionResult> GetTotalBrokerage(Guid userId)
    {
        var result = await _userRepository.GetTotalBrokerageAsync(userId);

        if (result == null)
            return NotFound(new { message = "Usuário não encontrado ou sem operações." });

        return Ok(result);
    }

    // Listar dividendos/JCP registrados para o usuário.
    [HttpGet("{userId}/provisions")]
    public async Task<ActionResult<List<UserProvisionDto>>> GetUserProvisions(Guid userId)
    {
        var result = await _userRepository.GetUserProvisionsAsync(userId);

        if (result == null || !result.Any())
            return NotFound("Nenhuma provisão encontrada para este usuário.");

        return Ok(result);
    }

    // Registrar nova operação de compra/venda.
    [HttpPost("{userId}/operations")]
    public async Task<IActionResult> RegisterTrade(Guid userId, [FromBody] CreateTradeDto dto)
    {
        var success = await _tradeBusiness.CreateTradeAsync(userId, dto);

        if (!success)
            return BadRequest("Usuário, ativo ou tipo de operação inválido.");

        return Ok("Operação registrada com sucesso.");
    }

    // Registrar provento (dividendo/JCP) recebido pelo usuário.
    [HttpPost("{userId}/dividends")]
    public async Task<IActionResult> RegisterDividend(Guid userId, [FromBody] CreateDividendDto dto)
    {
        var success = await _dividendRepository.RegisterDividendAsync(userId, dto);

        if (!success)
            return BadRequest("Usuário, ativo ou operação não encontrados ou dados inválidos.");

        return Ok("Provento registrado com sucesso.");
    }

    // Histórico temporal de P&L do usuário
    [HttpGet("{userId}/pnl-history")]
    public async Task<IActionResult> GetPnLHistory(Guid userId)
    {
        var pnlHistory = await _userRepository.GetUserPnLHistoryAsync(userId);

        if (pnlHistory == null || !pnlHistory.Any())
            return NotFound("Usuário sem dados de PnL.");

        return Ok(pnlHistory);
    }

    // Simular operação para estimativa de P&L e impacto na posição.
    [HttpPost("{userId}/simulate-operation")]
    public async Task<IActionResult> SimulateOperation(Guid userId, [FromBody] SimulateOperationRequest request)
    {
        if (request.Quantity <= 0)
            return BadRequest("Quantidade deve ser maior que zero.");

        if (request.UnitPrice <= 0)
            return BadRequest("Preço unitário deve ser maior que zero.");

        var result = await _tradeBusiness.SimulateOperationAsync(userId, request);
        return Ok(result);
    }

    // Resumo mensal de P&L, corretagens, proventos por usuário.
    [HttpGet("summary")]
    public async Task<IActionResult> GetMonthlySummary([FromQuery] int year, [FromQuery] int month)
    {
        if (year <= 0 || month <= 0 || month > 12)
            return BadRequest("Year and month query parameters must be valid.");

        var summary = await _reportRepository.GetMonthlySummaryAsync(year, month);
        return Ok(summary);
    }

    // Registrar novo usuario
    [HttpPost("/create")]
    public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto dto)
    {
        await _userRepository.AddUserAsync(dto);

        return Ok("Usuario registrada com sucesso.");
    }
}

