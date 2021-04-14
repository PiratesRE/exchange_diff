using System;
using System.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal struct AuthenticationData
	{
		public bool UseDefaultCredentials { get; set; }

		public ICredentials Credentials { get; set; }
	}
}
