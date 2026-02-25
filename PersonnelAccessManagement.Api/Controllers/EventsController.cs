using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonnelAccessManagement.Application.Features.Events.Dtos;
using PersonnelAccessManagement.Application.Features.Events.Queries;

namespace PersonnelAccessManagement.Api.Controllers;

[ApiController]
[Route("api/events")]
[Authorize(Roles = "Admin")]
public sealed class EventsController : ControllerBase
{
    private readonly IMediator _mediator;
    public EventsController(IMediator mediator) => _mediator = mediator;
    
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] EventFilter filter, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListEventsQuery(filter), ct);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}/logs")]
    public async Task<IActionResult> GetLogs([FromRoute] Guid id, CancellationToken ct)
    {
        var logs = await _mediator.Send(new GetEventLogsQuery(id), ct);
        return Ok(logs);
    }
    
    [HttpGet("{id:guid}/logs/export")]
    public async Task<IActionResult> ExportLogs([FromRoute] Guid id, CancellationToken ct)
    {
        var logs = await _mediator.Send(new GetEventLogsQuery(id), ct);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Event Logları");
        
        var headers = new[] { "#", "Sicil No", "Personel Adı", "Rol ID", "Rol Adı", "İşlem", "Durum", "Hata", "Tarih" };
        for (var i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }
        
        for (var r = 0; r < logs.Count; r++)
        {
            var log = logs[r];
            ws.Cell(r + 2, 1).Value = r + 1;
            ws.Cell(r + 2, 2).Value = log.EmployeeNo;
            ws.Cell(r + 2, 3).Value = log.PersonnelName;
            ws.Cell(r + 2, 4).Value = log.RoleId;
            ws.Cell(r + 2, 5).Value = log.RoleName;
            ws.Cell(r + 2, 6).Value = log.Action;
            ws.Cell(r + 2, 7).Value = log.Status == "Success" ? "Başarılı" : "Başarısız";
            ws.Cell(r + 2, 8).Value = log.Error ?? "";
            ws.Cell(r + 2, 9).Value = log.CreatedAt.ToString("dd.MM.yyyy HH:mm");
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        ms.Position = 0;

        return File(
            ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"event-logs-{id:N}.xlsx"
        );
    }
}