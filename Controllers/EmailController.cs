using let_em_cook.Services;

namespace let_em_cook.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

/*
 * This controller is for testing purposes
 */

[Route("api/email")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly EmailService _emailService;

    public EmailController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        var success = await _emailService.SendEmailAsync(request.To, request.Subject, request.Body);

        if (success)
            return Ok(new { message = "Email sent successfully!" });

        return StatusCode(500, new { error = "Failed to send email." });
    }
}

public class EmailRequest
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
