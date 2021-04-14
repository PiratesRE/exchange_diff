using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.MdbCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MissingMailboxOwnerPropertyException : OperationFailedException
	{
		public MissingMailboxOwnerPropertyException() : base(Strings.MissingMailboxOwnerProperty)
		{
		}

		public MissingMailboxOwnerPropertyException(Exception innerException) : base(Strings.MissingMailboxOwnerProperty, innerException)
		{
		}

		protected MissingMailboxOwnerPropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
