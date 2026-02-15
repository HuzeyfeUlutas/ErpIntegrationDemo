using Microsoft.AspNetCore.Mvc;
using PersonnelAccessManagement.Infrastructure.Seeders;

namespace PersonnelAccessManagement.Api.Controllers;


#if DEBUG
[ApiController]
[Route("api/seed")]
public sealed class SeedController : ControllerBase
{
    private readonly DataSeeder _seeder;
    public SeedController(DataSeeder seeder) => _seeder = seeder;

    // POST /api/seed
    [HttpPost]
    public async Task<IActionResult> Seed(CancellationToken ct)
    {
        await _seeder.SeedAsync(ct);
        return Ok(new { message = "Seed tamamlandı: 350 rol, 13.000 personel" });
    }
}
#endif