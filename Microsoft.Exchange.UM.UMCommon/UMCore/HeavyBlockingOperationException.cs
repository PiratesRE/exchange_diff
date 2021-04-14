using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class HeavyBlockingOperationException : LocalizedException
	{
		public HeavyBlockingOperationException() : base(Strings.HeavyBlockingOperationException)
		{
		}

		public HeavyBlockingOperationException(Exception innerException) : base(Strings.HeavyBlockingOperationException, innerException)
		{
		}

		protected HeavyBlockingOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
