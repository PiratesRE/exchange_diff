using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Autodiscover
{
	internal class RequestData
	{
		public RequestData(IPrincipal user, bool useClientCertificateAuthentication, CallerRequestedCapabilities optinCapabilities)
		{
			this.User = user;
			this.UseClientCertificateAuthentication = useClientCertificateAuthentication;
			this.Timestamp = ExDateTime.Now.TimeOfDay.ToString();
			this.RequestSchemas = new List<string>();
			this.ResponseSchemas = new List<string>();
			this.CallerCapabilities = optinCapabilities;
		}

		public IPrincipal User { get; set; }

		public string EMailAddress { get; set; }

		public string RedirectPod { get; set; }

		public string LegacyDN { get; set; }

		public List<string> RequestSchemas { get; private set; }

		public List<string> ResponseSchemas { get; private set; }

		public string Timestamp { get; private set; }

		public string ComputerNameHash
		{
			get
			{
				return RequestData.computerNameHash;
			}
		}

		public CallerRequestedCapabilities CallerCapabilities { get; private set; }

		public string UserAuthType { get; set; }

		public IBudget Budget { get; set; }

		public ProxyRequestData ProxyRequestData { get; set; }

		public bool UseClientCertificateAuthentication { get; private set; }

		public int MapiHttpVersion { get; set; }

		public string UserAgent { get; set; }

		public void Clear()
		{
			this.User = null;
			this.RequestSchemas.Clear();
			this.ResponseSchemas.Clear();
			this.EMailAddress = string.Empty;
			this.RedirectPod = string.Empty;
			this.LegacyDN = string.Empty;
			this.Timestamp = string.Empty;
			this.Budget = null;
			this.UserAuthType = string.Empty;
			this.ProxyRequestData = null;
			this.MapiHttpVersion = 0;
			this.CallerCapabilities = null;
			this.UseClientCertificateAuthentication = false;
			this.UserAgent = string.Empty;
		}

		private static readonly string computerNameHash = ((uint)Environment.MachineName.GetHashCode()).ToString();
	}
}
