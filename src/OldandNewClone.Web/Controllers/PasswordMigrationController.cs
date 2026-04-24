using Microsoft.AspNetCore.Mvc;
using OldandNewClone.Web.Services;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordMigrationController : ControllerBase
{
    private readonly PasswordFieldMigration _passwordFieldMigration;

    public PasswordMigrationController(PasswordFieldMigration passwordFieldMigration)
    {
        _passwordFieldMigration = passwordFieldMigration;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var status = await _passwordFieldMigration.GetStatusAsync();
        return Ok(status);
    }

    [HttpPost("migrate")]
    public async Task<IActionResult> Migrate()
    {
        var result = await _passwordFieldMigration.MigratePasswordFieldsAsync();
        return Ok(result);
    }
}