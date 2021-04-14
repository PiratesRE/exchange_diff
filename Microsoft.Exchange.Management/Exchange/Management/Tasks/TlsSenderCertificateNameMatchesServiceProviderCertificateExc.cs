using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TlsSenderCertificateNameMatchesServiceProviderCertificateException : LocalizedException
	{
		public TlsSenderCertificateNameMatchesServiceProviderCertificateException(string certificate) : base(Strings.TlsSenderCertificateNameMatchesServiceProviderCertificateId(certificate))
		{
			this.certificate = certificate;
		}

		public TlsSenderCertificateNameMatchesServiceProviderCertificateException(string certificate, Exception innerException) : base(Strings.TlsSenderCertificateNameMatchesServiceProviderCertificateId(certificate), innerException)
		{
			this.certificate = certificate;
		}

		protected TlsSenderCertificateNameMatchesServiceProviderCertificateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.certificate = (string)info.GetValue("certificate", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("certificate", this.certificate);
		}

		public string Certificate
		{
			get
			{
				return this.certificate;
			}
		}

		private readonly string certificate;
	}
}
