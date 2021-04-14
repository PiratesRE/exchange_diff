using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorFailedExtractTextForFingerprintException : LocalizedException
	{
		public ErrorFailedExtractTextForFingerprintException() : base(Strings.ErrorFailedExtractTextForFingerprint)
		{
		}

		public ErrorFailedExtractTextForFingerprintException(Exception innerException) : base(Strings.ErrorFailedExtractTextForFingerprint, innerException)
		{
		}

		protected ErrorFailedExtractTextForFingerprintException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
