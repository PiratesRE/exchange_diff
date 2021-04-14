using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnexpectedConditionOrActionDetectedException : LocalizedException
	{
		public UnexpectedConditionOrActionDetectedException() : base(Strings.UnexpectedConditionOrActionDetected)
		{
		}

		public UnexpectedConditionOrActionDetectedException(Exception innerException) : base(Strings.UnexpectedConditionOrActionDetected, innerException)
		{
		}

		protected UnexpectedConditionOrActionDetectedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
