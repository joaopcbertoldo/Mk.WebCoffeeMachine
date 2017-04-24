namespace Mkfeina.Domain.ServerArduinoComm
{
	public enum ResponseCodeEnum
	{
		NullResponse = -1,
		OK = 200,
		Cancel = 400,
		InvalidRequest = 401,
		InternalServerError = 402
	}

	public class Response
	{
		public static Response NullResponse { get => new Response() { ResponseCode = (int)ResponseCodeEnum.NullResponse }; }

		public int ResponseCode { get; set; }
	}
}