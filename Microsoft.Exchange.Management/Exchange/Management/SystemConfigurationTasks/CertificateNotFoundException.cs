using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CertificateNotFoundException : LocalizedException
	{
		public CertificateNotFoundException(string thumbprint) : base(Strings.CertificateNotFound(thumbprint))
		{
			this.thumbprint = thumbprint;
		}

		public CertificateNotFoundException(string thumbprint, Exception innerException) : base(Strings.CertificateNotFound(thumbprint), innerException)
		{
			this.thumbprint = thumbprint;
		}

		protected CertificateNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
