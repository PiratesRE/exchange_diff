using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess
{
	internal class ProtocolDescriptor
	{
		public HttpProtocol HttpProtocol { get; set; }

		public string AppPool { get; set; }

		public string VirtualDirectory { get; set; }

		public bool IsRedirectOK { get; set; }

		public Component HealthSet { get; set; }

		public bool DeferAlertOnCafeWideFailure { get; set; }

		public override string ToString()
		{
			return this.HttpProtocol.ToString();
		}

		public AuthenticationMethod[] AuthPreferenceOrderDatacenter { get; set; }

		public AuthenticationMethod[] AuthPreferenceOrderEnterprise { get; set; }

		public int ProtocolPriority { get; set; }

		public string LogFolderName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.logFolderName))
				{
					return this.logFolderName;
				}
				return this.HttpProtocol.ToString();
			}
			set
			{
				this.logFolderName = value;
			}
		}

		private string logFolderName;
	}
}
