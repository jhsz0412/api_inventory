using Approvals.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Approvals.Presentation.Controllers;

[ApiController]
[Route("api/approvals/requisitions/{requisitionId:guid}/audit")]
[Authorize]
public sealed class ApprovalAuditController(
    IApprovalAuditService auditService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "permission:requisitions.read")]
    public async Task<IActionResult> Get(
        Guid requisitionId, CancellationToken cancellationToken)
    {
        return Ok(await auditService.GetByRequisitionIdAsync(
            requisitionId, cancellationToken));
    }
}
