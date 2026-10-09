using Backend.Rag;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AskController(RagService rag) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AskResponse>> Ask([FromBody] AskRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest("Question is required.");
        }
        return await rag.AskAsync(request.Question, ct);
    }
}
