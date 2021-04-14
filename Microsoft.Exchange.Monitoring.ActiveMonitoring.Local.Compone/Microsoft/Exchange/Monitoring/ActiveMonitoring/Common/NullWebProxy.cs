using System;
using System.Net;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class NullWebProxy : IWebProxy
	{
		public static NullWebProxy Instance
		{
			get
			{
				return NullWebProxy.instance;
			}
		}

		public ICredentials Credentials { get; set; }

		public Uri GetProxy(Uri destination)
		{
			throw new NotImplementedException();
		}

		public bool IsBypassed(Uri host)
		{
			return true;
		}

		private static NullWebProxy instance = new NullWebProxy();
	}
}
