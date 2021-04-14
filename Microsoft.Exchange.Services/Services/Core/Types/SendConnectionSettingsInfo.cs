using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract]
	[XmlType(TypeName = "SmtpConnectionSettings", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SendConnectionSettingsInfo
	{
		public ConnectionSettingsInfoType ConnectionType
		{
			get
			{
				return ConnectionSettingsInfoType.Smtp;
			}
		}

		[XmlElement("ServerName")]
		[DataMember]
		public string ServerName { get; set; }

		[XmlElement("Port")]
		[DataMember]
		public int Port { get; set; }

		public string ToMultiLineString(string lineSeparator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Send connection settings:{0}", lineSeparator);
			stringBuilder.AppendFormat("Server name={0},{1}", this.ServerName, lineSeparator);
			stringBuilder.AppendFormat("Port={0}.", this.Port);
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return this.ToMultiLineString(" ");
		}
	}
}
