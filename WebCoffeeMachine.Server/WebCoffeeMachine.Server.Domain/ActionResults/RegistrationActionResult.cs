using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using WebCoffeeMachine.Domain.ServerArduinoComm;

namespace WebCoffeeMachine.Server.Domain.ActionResults
{
    public class RegistrationActionResult : IHttpActionResult
    {
        private RegistrationResultStatusEnum _resultStatus;
        private HttpRequestMessage _request;
        private int? _communicationPin;

        public RegistrationActionResult(HttpRequestMessage request, RegistrationResultStatusEnum resultStatus, int? communicationPin = null)
        {
            _request = request;
            _resultStatus = resultStatus;
            _communicationPin = communicationPin;
        }

#warning otimizar as respostas para mandar só o código e não o "s"
#warning testar colocar tudo numa task sem inicializá-la

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response;
            switch (_resultStatus) {
                case RegistrationResultStatusEnum.Ok:
                    response = _request.CreateResponse(HttpStatusCode.OK, new RegistrationResponse() { p = _communicationPin.Value });
                    break;

                case RegistrationResultStatusEnum.UniqueNameAlreadyTaken:
                    response = _request.CreateResponse(HttpStatusCode.Conflict);
                    break;

                case RegistrationResultStatusEnum.InvalidForms:
                    response = _request.CreateResponse(HttpStatusCode.BadRequest);
                    break;

                default:
                    response = _request.CreateResponse(HttpStatusCode.InternalServerError);
                    break;
            }
            return Task.FromResult(response);
        }
    }
}