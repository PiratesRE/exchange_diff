using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ImportCertificateBase64DataInvalidException : LocalizedException
	{
		public ImportCertificateBase64DataInvalidException() : base(Strings.ImportCertificateBase64DataInvalid)
		{
		}

		public ImportCertificateBase64DataInvalidException(Exception innerException) : base(Strings.ImportCertificateBase64DataInvalid, innerException)
		{
		}

		protected ImportCertificateBase64DataInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
