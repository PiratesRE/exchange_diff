using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorCannotCreateFingerprintException : LocalizedException
	{
		public ErrorCannotCreateFingerprintException() : base(Strings.ErrorCannotCreateFingerprint)
		{
		}

		public ErrorCannotCreateFingerprintException(Exception innerException) : base(Strings.ErrorCannotCreateFingerprint, innerException)
		{
		}

		protected ErrorCannotCreateFingerprintException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
