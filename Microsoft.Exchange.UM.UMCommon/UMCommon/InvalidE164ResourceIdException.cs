using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidE164ResourceIdException : LocalizedException
	{
		public InvalidE164ResourceIdException() : base(Strings.ExceptionInvalidE164ResourceId)
		{
		}

		public InvalidE164ResourceIdException(Exception innerException) : base(Strings.ExceptionInvalidE164ResourceId, innerException)
		{
		}

		protected InvalidE164ResourceIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
