using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Aggregation
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ValidateSecretFailureException : LocalizedException
	{
		public ValidateSecretFailureException() : base(Strings.ValidateSecretFailure)
		{
		}

		public ValidateSecretFailureException(Exception innerException) : base(Strings.ValidateSecretFailure, innerException)
		{
		}

		protected ValidateSecretFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
