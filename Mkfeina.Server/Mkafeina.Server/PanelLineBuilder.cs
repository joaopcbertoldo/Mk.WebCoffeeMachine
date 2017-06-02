using Mkafeina.Domain.Dashboard.Panels;
using System;
using static Mkafeina.Server.Constants;

namespace Mkafeina.Server
{
	public class PanelLineBuilder : AbstractPanelLineBuilder
	{
		public override string Build(string lineName)
		{
			switch (lineName)
			{
				#region Status Panel Lines

				case SERVER_ADDRESS:
					return $"Server address : {AppConfig.Sgt.ServerAddress}";

				case SERVER_NICE_ADDRESS:
					return $"Server nice address : {AppConfig.Sgt.ServerNiceAddress}";

				#endregion Status Panel Lines

				#region Commands Panel Lines

				case COMMAND_F5:
					return "F5 : reload configs, dashboard and recipes";

				case COMMAND_F4:
					return "F4 : reload recipes";

				#endregion Commands Panel Lines

				default:
					throw new NotImplementedException();
			}
		}

		public override string UpdateEventHandler(string lineName, object caller)
		{
			throw new NotImplementedException();
		}
	}
}