using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CertificateExpiredException : LocalizedException
	{
		public CertificateExpiredException(string thumbprint) : base(Strings.CertificateExpired(thumbprint))
		{
			this.thumbprint = thumbprint;
		}

		public CertificateExpiredException(string thumbprint, Exception innerException) : base(Strings.CertificateExpired(thumbprint), innerException)
		{
			this.thumbprint = thumbprint;
		}

		protected CertificateExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.thumbprint = (string)info.GetValue("thumbprint", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("thumbprint", this.thumbprint);
		}

		public string Thumbprint
		{
			get
			{
				return this.thumbprint;
			}
		}

		private readonly string thumbprint;
	}
}
