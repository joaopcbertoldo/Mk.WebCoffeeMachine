using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using static Mkfeina.Domain.Extentions;

namespace Mkfeina.Server.Attributes
{
	public class DecryptionAttribute : AuthorizeAttribute
	{
		private byte[] KEY = new byte[] { 1, 2, 3 };
		//private byte[] KEY = new byte[] { 42, 33, 13 };

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var original = actionContext.Request.Content.ReadAsByteArrayAsync().Result;
			var originalStr = Encoding.ASCII.GetString(original);
			var decrypted = original.Decrypt(KEY);
			var decryptedStr = Encoding.ASCII.GetString(decrypted);
		}
	}
}