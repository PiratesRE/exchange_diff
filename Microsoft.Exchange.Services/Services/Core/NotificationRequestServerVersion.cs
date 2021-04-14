using System;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[XmlRoot(IsNullable = false, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlType(TypeName = "RequestServerVersion", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class NotificationRequestServerVersion : SoapHeader
	{
		public NotificationRequestServerVersion()
		{
		}

		public NotificationRequestServerVersion(ExchangeVersionType version)
		{
			this.Version = version;
		}

		[XmlAttribute(AttributeName = "Version")]
		public ExchangeVersionType Version
		{
			get
			{
				return this.versionField;
			}
			set
			{
				this.versionField = value;
			}
		}

		private ExchangeVersionType versionField;
	}
}
