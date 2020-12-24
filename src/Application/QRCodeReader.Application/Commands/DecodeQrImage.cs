using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using static System.String;

namespace QRCodeReader.Application.Commands
{
    public class DecodeQrImage
    {
        public class Command : IRequest<string> // the response of request is a string - the encoded text
        {
            public string FilePath { get; }

            public Command(string filePath)
            {
                FilePath = filePath;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(a => a.FilePath)
                    .NotEmpty()
                    .Must(FileExists).WithMessage(command => $"file not found {command.FilePath}");
            }

            private bool FileExists(string filePath)
                => File.Exists(filePath);
        }

        internal class QrDataResponse
        {
            public QrDataResponse(Response[] responses)
            {
                Responses = responses;
            }

            public Response[] Responses { get; }
        }

        internal class Response
        {
            public Response(string type, Symbol[] symbol)
            {
                Type = type;
                Symbol = symbol;
            }

            public string Type { get; }
            public Symbol[] Symbol { get; }
        }

        internal class Symbol
        {
            public Symbol(int seq, string data, string error)
            {
                Seq = seq;
                Data = data;
                Error = error;
            }

            public int Seq { get; }
            public string Data { get; }
            public string Error { get; }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly IConfiguration _configuration;
            private readonly IHttpClientFactory _clientFactory;
            private readonly ILogger<Command> _logger;

            public Handler(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<Command> logger)
            {
                _configuration = configuration;
                _clientFactory = clientFactory;
                _logger = logger;
            }

            public async Task<string> Handle(Command request, CancellationToken cancellationToken)
            {
                var responseString = Empty;
                var client = _clientFactory.CreateClient();
                var qrApiUrl = _configuration.GetSection("QR")["Api"];
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                MultipartFormDataContent form = new MultipartFormDataContent();
                var image = await File.ReadAllBytesAsync(request.FilePath, cancellationToken);
                var httpContent = new ByteArrayContent(image);
                httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                form.Add(httpContent, "file", Path.GetFileName(request.FilePath));

                try
                {
                    var httpResponse =
                        await client.PostAsync($"{qrApiUrl}/read-qr-code/", form, cancellationToken);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        responseString = await httpResponse.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        _logger.LogError($"Unable to send http request to url {qrApiUrl}");
                        throw new Exception($"Unable to send http request to url {qrApiUrl}");
                    }
                }

                catch (Exception ex)
                {
                    _logger.LogError("Unable to send http request", ex.Message);
                    throw;
                }

                var data = JsonConvert.DeserializeObject<List<Response>>(responseString)[0].Symbol[0].Data;
                return data;
            }
        }
    }
}