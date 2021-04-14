using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class E164ResourceIdNeededException : LocalizedException
	{
		public E164ResourceIdNeededException() : base(Strings.ExceptionE164ResourceIdNeeded)
		{
		}

		public E164ResourceIdNeededException(Exception innerException) : base(Strings.ExceptionE164ResourceIdNeeded, innerException)
		{
		}

		protected E164ResourceIdNeededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
