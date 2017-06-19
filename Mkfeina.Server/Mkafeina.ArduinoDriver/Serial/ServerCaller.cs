using Mkafeina.Domain.ServerArduinoComm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net;

namespace Mkafeina.ArduinoDriver.Serial
{
	public class ServerCaller
	{
		private const string
			APPLICATION_JSON = "application/json",
			POST = "POST"
			;

		private const int
			STANDARD_TIMEOUT = 15000
			;

		private object _communicationSyncOnj = new object();

		public ServerCaller()
		{
		}

		private TResponse SendHttpRequest<TRequest, TResponse>(TRequest requestObj, string url)
			where TRequest : ArduinoRequest
			where TResponse : ArduinoResponse
		{
			lock (_communicationSyncOnj)
			{
				try
				{
					var jsonStr = JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings()
					{
						ContractResolver = new CamelCasePropertyNamesContractResolver(),
						NullValueHandling = NullValueHandling.Ignore
					});
					var request = (HttpWebRequest)WebRequest.Create(url);
					request.Method = POST;
					request.ContentType = APPLICATION_JSON;
					request.Timeout = STANDARD_TIMEOUT;

					StreamWriter streamWriter;
					using (streamWriter = new StreamWriter(request.GetRequestStream()))
					{
						try
						{
							streamWriter.Write(jsonStr);
							streamWriter.Flush();
							streamWriter.Close();
						}
						catch (Exception exception)
						{
							if (streamWriter != null)
							{
								streamWriter.Close();
								streamWriter.Dispose();
							}
							Console.WriteLine("Problem ocurred during communication (on write stream phase).");
							return null;
						}
					}
					TResponse responseObj;
					var response = (HttpWebResponse)request.GetResponse();
					using (var streamReader = new StreamReader(response.GetResponseStream()))
					{
						var responseStr = streamReader.ReadToEnd();
						responseObj = JObject.Parse(responseStr).ToObject<TResponse>();
					}
					return responseObj;
				}
				catch (WebException exception)
				{
					Console.WriteLine($"WEB Exception : {exception}");
					return null;
				}
				catch (Exception exception)
				{
					Console.WriteLine($"Exception : {exception}");
					return null;
				}
			}
		}

		public bool Send<TRequest, TResponse>(TRequest request, out TResponse response, string url)
			where TRequest : ArduinoRequest
			where TResponse : ArduinoResponse
		{
			try
			{
				response = SendHttpRequest<TRequest, TResponse>(request, url);

				if (response == null)
				{
					Console.WriteLine($"Null response");
					return false;
				}
				else
				{
					Console.WriteLine($"Got response");
					return true;
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine($"Exception : {exception}");
				response = null;
				return false;
			}
		}
	}
}