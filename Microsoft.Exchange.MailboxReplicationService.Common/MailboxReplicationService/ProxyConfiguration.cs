using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class ProxyConfiguration
	{
		[DataMember(Name = "ExportBufferSizeKB")]
		public int ExportBufferSizeKB { get; set; }

		public ProxyConfiguration()
		{
			this.ExportBufferSizeKB = ConfigBase<MRSConfigSchema>.GetConfig<int>("ExportBufferSizeKB");
		}
	}
}
