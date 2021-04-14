using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorCannotLoadFingerprintCreatorException : LocalizedException
	{
		public ErrorCannotLoadFingerprintCreatorException() : base(Strings.ErrorCannotLoadFingerprintCreator)
		{
		}

		public ErrorCannotLoadFingerprintCreatorException(Exception innerException) : base(Strings.ErrorCannotLoadFingerprintCreator, innerException)
		{
		}

		protected ErrorCannotLoadFingerprintCreatorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
