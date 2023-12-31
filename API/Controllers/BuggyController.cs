using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{

    [HttpGet("bad-request")]
    public ActionResult GetBadRequestError()
    {
        return BadRequest(new ProblemDetails
        {
            Title = "this is a bad request"
        });
    }

    [HttpGet("unauthorized")]
    public ActionResult GetUnauthorizedError()
    {
        return Unauthorized();
    }

    [HttpGet("not-found")]
    public ActionResult GetNotFoundError()
    {
        return NotFound();
    }

    [HttpGet("validation-error")]
    public ActionResult GetValidationError()
    {
        ModelState.AddModelError("Problem1", "this is the first error");
        ModelState.AddModelError("Problem2", "this is the second error");
        return ValidationProblem();
    }


    [HttpGet("server-error")]
    public ActionResult GetServerError()
    {
        throw new Exception("this is a server error");
    }

}
