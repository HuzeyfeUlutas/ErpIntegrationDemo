using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonnelAccessManagement.Application.Features.Jobs.Dtos;
using PersonnelAccessManagement.Application.Features.Jobs.Queries;

namespace PersonnelAccessManagement.Api.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize(Roles = "Admin")]
public sealed class JobsController : ControllerBase
{
    private readonly IMediator _mediator;
    public JobsController(IMediator mediator) => _mediator = mediator;
    
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] JobFilter filter, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListJobsQuery(filter), ct);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}/logs")]
    public async Task<IActionResult> GetLogs([FromRoute] Guid id, CancellationToken ct)
    {
        var logs = await _mediator.Send(new GetJobLogsQuery(id), ct);
        return Ok(logs);
    }
    
    [HttpGet("{id:guid}/logs/export")]
    public async Task<IActionResult> ExportLogs([FromRoute] Guid id, CancellationToken ct)
    {
        var logs = await _mediator.Send(new GetJobLogsQuery(id), ct);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Job Logları");
        
        var headers = new[] { "#", "Mesaj", "Durum", "Tarih" };
        for (var i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }
        
        string TrStatus(string s) => s switch
        {
            "INFO" => "Bilgi",
            "FATAL" => "Kritik Hata",
            "WARNING" => "Uyarı",
            "ERROR" => "Hata",
            "SUCCESS" => "Başarılı",
            _ => s
        };

        for (var r = 0; r < logs.Count; r++)
        {
            var log = logs[r];
            ws.Cell(r + 2, 1).Value = r + 1;
            ws.Cell(r + 2, 2).Value = log.Message;
            ws.Cell(r + 2, 3).Value = TrStatus(log.Status);
            ws.Cell(r + 2, 4).Value = log.CreatedAt.ToString("dd.MM.yyyy HH:mm:ss");
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        ms.Position = 0;

        return File(
            ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"job-logs-{id:N}.xlsx"
        );
    }
}