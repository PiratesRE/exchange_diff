using System;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class ConnectivityRec : XMLSerializableBase
	{
		public ConnectivityRec()
		{
		}

		internal ConnectivityRec(ServerKind serverKind, MailboxInformation mailboxInfo, MailboxServerInformation serverInfo)
		{
			this.ServerKind = serverKind;
			this.ServerName = serverInfo.MailboxServerName;
			this.ServerVersion = new ServerVersion(serverInfo.MailboxServerVersion);
			if (serverInfo.ProxyServerName != null)
			{
				this.ProxyName = serverInfo.ProxyServerName;
				this.ProxyVersion = serverInfo.ProxyServerVersion.ServerVersion;
			}
			if (mailboxInfo != null)
			{
				this.ProviderName = mailboxInfo.ProviderName;
			}
		}

		internal ConnectivityRec(ServerKind serverKind, VersionInformation versionInfo)
		{
			this.ServerKind = serverKind;
			this.ServerName = versionInfo.ComputerName;
			this.ServerVersion = versionInfo.ServerVersion;
		}

		[XmlIgnore]
		public ServerKind ServerKind { get; set; }

		[XmlElement(ElementName = "ServerKindInt")]
		public int ServerKindInt
		{
			get
			{
				return (int)this.ServerKind;
			}
			set
			{
				this.ServerKind = (ServerKind)value;
			}
		}

		[XmlElement(ElementName = "ServerName")]
		public string ServerName { get; set; }

		[XmlIgnore]
		public ServerVersion ServerVersion { get; set; }

		[XmlElement(ElementName = "ServerVersionStr")]
		public string ServerVersionStr
		{
			get
			{
				if (!(this.ServerVersion != null))
				{
					return null;
				}
				return this.ServerVersion.ToString();
			}
			set
			{
				this.ServerVersion = ConnectivityRec.ParseServerVersion(value);
			}
		}

		[XmlElement(ElementName = "ProxyName")]
		public string ProxyName { get; set; }

		[XmlElement(ElementName = "ProviderName")]
		public string ProviderName { get; set; }

		[XmlIgnore]
		public ServerVersion ProxyVersion { get; set; }

		[XmlElement(ElementName = "ProxyVersionStr")]
		public string ProxyVersionStr
		{
			get
			{
				if (!(this.ProxyVersion != null))
				{
					return null;
				}
				return this.ProxyVersion.ToString();
			}
			set
			{
				this.ProxyVersion = ConnectivityRec.ParseServerVersion(value);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			switch (this.ServerKind)
			{
			case ServerKind.MRS:
				stringBuilder.Append("MRS ");
				break;
			case ServerKind.Source:
				stringBuilder.Append("S   ");
				break;
			case ServerKind.Target:
				stringBuilder.Append("T   ");
				break;
			case ServerKind.SourceArchive:
				stringBuilder.Append("SA  ");
				break;
			case ServerKind.TargetArchive:
				stringBuilder.Append("TA  ");
				break;
			case ServerKind.Cmdlet:
				stringBuilder.Append("CMD ");
				break;
			}
			stringBuilder.AppendFormat("{0} ({1})", this.ServerName, this.ServerVersion.ToString());
			if (!string.IsNullOrEmpty(this.ProxyName))
			{
				stringBuilder.AppendFormat("; P {0} ({1})", this.ProxyName, this.ProxyVersion.ToString());
			}
			if (!string.IsNullOrEmpty(this.ProviderName))
			{
				stringBuilder.AppendFormat("; {0}", this.ProviderName);
			}
			return stringBuilder.ToString();
		}

		private static ServerVersion ParseServerVersion(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			ServerVersion result;
			try
			{
				Version version = new Version(value);
				result = version;
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}
	}
}
