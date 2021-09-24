using System;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWithMarten
{
    public class IssueDataController : ControllerBase
    {
        [HttpGet("/issue/{issueId}")]
        public async Task GetIssue(Guid issueId, [FromServices] IQuerySession session)
        {
            await session.Json.StreamById<Issue>(issueId, Response.Body, HttpContext.RequestAborted);
        }
    }
}
