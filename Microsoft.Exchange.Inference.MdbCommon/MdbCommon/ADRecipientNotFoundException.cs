using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.MdbCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ADRecipientNotFoundException : OperationFailedException
	{
		public ADRecipientNotFoundException() : base(Strings.AdRecipientNotFound)
		{
		}

		public ADRecipientNotFoundException(Exception innerException) : base(Strings.AdRecipientNotFound, innerException)
		{
		}

		protected ADRecipientNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
