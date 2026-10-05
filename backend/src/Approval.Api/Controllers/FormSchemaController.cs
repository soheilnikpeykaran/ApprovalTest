using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Approval.Api.Controllers;
[ApiController, Route("api/form-schema"), AllowAnonymous]
public sealed class FormSchemaController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { fields = new object[]
    {
        new { key = "title", label = "عنوان", type = "text", required = true },
        new { key = "amount", label = "مبلغ (تومان)", type = "number", required = true },
        new { key = "description", label = "توضیحات", type = "textarea", required = false },
        new { key = "urgency", label = "فوریت", type = "select", required = true, options = new[] { "کم", "متوسط", "زیاد" } }
    }});
}
