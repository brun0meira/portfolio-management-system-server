using Domain.Dto;
using Domain.Interfaces;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportBusiness _reportBusiness;
    private readonly IReportRepository _reportRepository;

    public ReportsController(IReportBusiness reportBusiness, IReportRepository reportRepository)
    {
        _reportBusiness = reportBusiness;
        _reportRepository = reportRepository;
    }

    // Top N clientes por valor de posição
    [HttpGet("top-clients-by-position")]
    public async Task<ActionResult<List<TopClientDto>>> GetTopClientsByPosition([FromQuery] int limit = 5)
    {
        if (limit <= 0)
        {
            return BadRequest("Limit must be greater than 0.");
        }

        var result = await _reportBusiness.GetTopClientsByPositionAsync(limit);
        return Ok(result);
    }

    // Top N clientes por volume de corretagem
    [HttpGet("top-clients-by-brokerage")]
    public async Task<IActionResult> GetTopClientsByBrokerage([FromQuery] int limit = 10)
    {
        if (limit <= 0) limit = 10;

        var result = await _reportRepository.GetTopClientsByBrokerageAsync(limit);
        return Ok(result);
    }
}
