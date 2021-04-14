using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CertificateNotValidForExchangeException : LocalizedException
	{
		public CertificateNotValidForExchangeException(string thumbprint, string reason) : base(Strings.CertificateNotValidForExchange(thumbprint, reason))
		{
			this.thumbprint = thumbprint;
			this.reason = reason;
		}

		public CertificateNotValidForExchangeException(string thumbprint, string reason, Exception innerException) : base(Strings.CertificateNotValidForExchange(thumbprint, reason), innerException)
		{
			this.thumbprint = thumbprint;
			this.reason = reason;
		}

		protected CertificateNotValidForExchangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.thumbprint = (string)info.GetValue("thumbprint", typeof(string));
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("thumbprint", this.thumbprint);
			info.AddValue("reason", this.reason);
		}

		public string Thumbprint
		{
			get
			{
				return this.thumbprint;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string thumbprint;

		private readonly string reason;
	}
}
