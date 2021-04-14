using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorFileIsTooSmallForFingerprintException : LocalizedException
	{
		public ErrorFileIsTooSmallForFingerprintException() : base(Strings.ErrorFileIsTooSmallForFingerprint)
		{
		}

		public ErrorFileIsTooSmallForFingerprintException(Exception innerException) : base(Strings.ErrorFileIsTooSmallForFingerprint, innerException)
		{
		}

		protected ErrorFileIsTooSmallForFingerprintException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
