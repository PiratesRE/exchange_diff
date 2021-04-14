using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MailboxServerInformation
	{
		[DataMember]
		public string MailboxServerName { get; set; }

		[DataMember]
		public int MailboxServerVersion { get; set; }

		[DataMember]
		public string ProxyServerName { get; set; }

		[DataMember]
		public VersionInformation ProxyServerVersion { get; set; }

		[DataMember]
		public Guid MailboxServerGuid { get; set; }

		[DataMember]
		public uint MailboxSignatureVersion { get; set; }

		[DataMember]
		public uint DeleteMailboxVersion { get; set; }

		[DataMember]
		public uint InTransitStatusVersion { get; set; }

		[DataMember]
		public uint MailboxShapeVersion { get; set; }

		public LocalizedString ServerInfoString
		{
			get
			{
				ServerVersion serverVersion = new ServerVersion(this.MailboxServerVersion);
				if (string.IsNullOrEmpty(this.ProxyServerName))
				{
					return MrsStrings.MailboxServerInformation(this.MailboxServerName, serverVersion.ToString());
				}
				return MrsStrings.RemoteMailboxServerInformation(this.MailboxServerName, serverVersion.ToString(), this.ProxyServerName, this.ProxyServerVersion.ToString());
			}
		}

		public override string ToString()
		{
			return this.ServerInfoString.ToString();
		}
	}
}
