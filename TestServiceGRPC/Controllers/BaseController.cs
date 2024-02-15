using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestServiceGRPC.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public class BaseController : Controller
{

}
