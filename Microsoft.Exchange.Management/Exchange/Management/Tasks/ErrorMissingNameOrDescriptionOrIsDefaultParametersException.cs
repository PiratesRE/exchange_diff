using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorMissingNameOrDescriptionOrIsDefaultParametersException : LocalizedException
	{
		public ErrorMissingNameOrDescriptionOrIsDefaultParametersException() : base(Strings.ErrorMissingNameOrDescriptionOrIsDefaultParameters)
		{
		}

		public ErrorMissingNameOrDescriptionOrIsDefaultParametersException(Exception innerException) : base(Strings.ErrorMissingNameOrDescriptionOrIsDefaultParameters, innerException)
		{
		}

		protected ErrorMissingNameOrDescriptionOrIsDefaultParametersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
