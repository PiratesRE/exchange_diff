using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CertificateLoadErrorException : MailboxReplicationPermanentException
	{
		public CertificateLoadErrorException(string certificateName, string errorMessage) : base(MrsStrings.CertificateLoadError(certificateName, errorMessage))
		{
			this.certificateName = certificateName;
			this.errorMessage = errorMessage;
		}

		public CertificateLoadErrorException(string certificateName, string errorMessage, Exception innerException) : base(MrsStrings.CertificateLoadError(certificateName, errorMessage), innerException)
		{
			this.certificateName = certificateName;
			this.errorMessage = errorMessage;
		}

		protected CertificateLoadErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.certificateName = (string)info.GetValue("certificateName", typeof(string));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("certificateName", this.certificateName);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string CertificateName
		{
			get
			{
				return this.certificateName;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string certificateName;

		private readonly string errorMessage;
	}
}
