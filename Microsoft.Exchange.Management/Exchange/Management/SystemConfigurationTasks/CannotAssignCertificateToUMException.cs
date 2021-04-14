using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotAssignCertificateToUMException : LocalizedException
	{
		public CannotAssignCertificateToUMException(string thumbprint) : base(Strings.CannotAssignCertificateToUM(thumbprint))
		{
			this.thumbprint = thumbprint;
		}

		public CannotAssignCertificateToUMException(string thumbprint, Exception innerException) : base(Strings.CannotAssignCertificateToUM(thumbprint), innerException)
		{
			this.thumbprint = thumbprint;
		}

		protected CannotAssignCertificateToUMException(SerializationInfo info, StreamingContext context) : base(info, context)
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
