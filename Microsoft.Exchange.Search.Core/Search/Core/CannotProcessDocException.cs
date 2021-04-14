using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotProcessDocException : OperationFailedException
	{
		public CannotProcessDocException() : base(Strings.CannotProcessDoc)
		{
		}

		public CannotProcessDocException(Exception innerException) : base(Strings.CannotProcessDoc, innerException)
		{
		}

		protected CannotProcessDocException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
