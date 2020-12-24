using MediatR;
using Microsoft.AspNetCore.Mvc;
using QRCodeReader.Application.Commands;
using System.Threading.Tasks;

namespace QRCodeReader.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReaderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReaderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> DecodeQRImage([FromBody] DecodeQrImage.Command command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}