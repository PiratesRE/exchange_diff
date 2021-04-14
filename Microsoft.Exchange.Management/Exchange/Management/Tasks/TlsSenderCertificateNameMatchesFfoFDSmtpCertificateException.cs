using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TlsSenderCertificateNameMatchesFfoFDSmtpCertificateException : LocalizedException
	{
		public TlsSenderCertificateNameMatchesFfoFDSmtpCertificateException(string certificate) : base(Strings.TlsSenderCertificateNameMatchesFfoFDSmtpCertificateId(certificate))
		{
			this.certificate = certificate;
		}

		public TlsSenderCertificateNameMatchesFfoFDSmtpCertificateException(string certificate, Exception innerException) : base(Strings.TlsSenderCertificateNameMatchesFfoFDSmtpCertificateId(certificate), innerException)
		{
			this.certificate = certificate;
		}

		protected TlsSenderCertificateNameMatchesFfoFDSmtpCertificateException(SerializationInfo info, StreamingContext context) : base(info, context)
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
