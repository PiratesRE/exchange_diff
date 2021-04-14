using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(SendConnectionSettingsInfo))]
	[XmlType(TypeName = "ConnectionSettingsInfo", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract]
	[KnownType(typeof(SendConnectionSettingsInfo))]
	[Serializable]
	public class ConnectionSettingsInfo
	{
		[XmlElement("SendConnectionSettings")]
		[DataMember]
		public SendConnectionSettingsInfo SendConnectionSettings { get; set; }

		[DataMember]
		[XmlElement("ConnectionType")]
		public ConnectionSettingsInfoType ConnectionType { get; set; }

		[XmlElement("ServerName")]
		[DataMember]
		public string ServerName { get; set; }

		[XmlElement("Port")]
		[DataMember]
		public int Port { get; set; }

		[DataMember]
		[XmlElement("Authentication")]
		public string Authentication { get; set; }

		[DataMember]
		[XmlElement("Office365UserFound")]
		public bool Office365UserFound { get; set; }

		[XmlElement("Security")]
		[DataMember]
		public string Security { get; set; }

		public string ToMultiLineString(string lineSeparator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Receive connection settings:{0}Type={1}", lineSeparator, this.ConnectionType);
			ConnectionSettingsInfoType connectionType = this.ConnectionType;
			if (connectionType == ConnectionSettingsInfoType.ExchangeActiveSync || connectionType == ConnectionSettingsInfoType.Imap || connectionType == ConnectionSettingsInfoType.Pop)
			{
				stringBuilder.AppendFormat(",{0}", lineSeparator);
				stringBuilder.AppendFormat("Server name={0},{1}", this.ServerName, lineSeparator);
				stringBuilder.AppendFormat("Port={0},{1}", this.Port, lineSeparator);
				stringBuilder.AppendFormat("Authentication={0},{1}", this.Authentication, lineSeparator);
				stringBuilder.AppendFormat("Security={0}", this.Security);
				if (this.SendConnectionSettings != null)
				{
					stringBuilder.AppendFormat(".{0}", lineSeparator);
					stringBuilder.Append(this.SendConnectionSettings.ToMultiLineString(lineSeparator));
				}
				return stringBuilder.ToString();
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Unexpected connection settings type encountered when converting to public representation: {0}", new object[]
			{
				this.ConnectionType
			}));
		}

		public override string ToString()
		{
			return this.ToMultiLineString(" ");
		}
	}
}
