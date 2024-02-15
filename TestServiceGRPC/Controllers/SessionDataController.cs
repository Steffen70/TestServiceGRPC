using CommonLib.Model;
using Microsoft.AspNetCore.Mvc;

namespace TestServiceGRPC.Controllers;

[Route("api/session-data")]
public class SessionDataController : BaseController
{
    private readonly SessionData _sessionData;

    public SessionDataController(SessionData sessionData) => _sessionData = sessionData;

    public IActionResult Get() => Ok(_sessionData);
}