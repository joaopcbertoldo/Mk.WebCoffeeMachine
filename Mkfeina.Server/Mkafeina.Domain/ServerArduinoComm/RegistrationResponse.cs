using System.Web.Http;

namespace Mkafeina.Domain.ServerArduinoComm
{
	public enum RegistrationResponseCodeEnum
	{
		RegisteredButNotAccepted = 57,
		AlreadyRegistered = 75
	}

	public class RegistrationResponse : Response, IHttpActionResult
	{
#warning improve hashing with the hashing that takes an extra str???
		public string TrueUniqueName { get; set; }
	}
}