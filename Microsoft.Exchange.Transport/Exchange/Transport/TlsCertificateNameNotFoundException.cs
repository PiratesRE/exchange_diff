using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TlsCertificateNameNotFoundException : ExchangeConfigurationException
	{
		public TlsCertificateNameNotFoundException(string tlsCertificateName, string connectorName) : base(Strings.TlsCertificateNameNotFound(tlsCertificateName, connectorName))
		{
			this.tlsCertificateName = tlsCertificateName;
			this.connectorName = connectorName;
		}

		public TlsCertificateNameNotFoundException(string tlsCertificateName, string connectorName, Exception innerException) : base(Strings.TlsCertificateNameNotFound(tlsCertificateName, connectorName), innerException)
		{
			this.tlsCertificateName = tlsCertificateName;
			this.connectorName = connectorName;
		}

		protected TlsCertificateNameNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.tlsCertificateName = (string)info.GetValue("tlsCertificateName", typeof(string));
			this.connectorName = (string)info.GetValue("connectorName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("tlsCertificateName", this.tlsCertificateName);
			info.AddValue("connectorName", this.connectorName);
		}

		public string TlsCertificateName
		{
			get
			{
				return this.tlsCertificateName;
			}
		}

		public string ConnectorName
		{
			get
			{
				return this.connectorName;
			}
		}

		private readonly string tlsCertificateName;

		private readonly string connectorName;
	}
}
