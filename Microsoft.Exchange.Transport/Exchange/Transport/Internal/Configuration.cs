using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Transport.Internal
{
	public static class Configuration
	{
		public static TransportConfigContainer TransportConfigObject
		{
			get
			{
				return Components.Configuration.TransportSettings.TransportSettings;
			}
		}

		public static Server TransportServer
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer;
			}
		}
	}
}
