using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AsyncCopyGetException : TransientException
	{
		public AsyncCopyGetException() : base(CommonStrings.AsyncCopyGetException)
		{
		}

		public AsyncCopyGetException(Exception innerException) : base(CommonStrings.AsyncCopyGetException, innerException)
		{
		}

		protected AsyncCopyGetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
