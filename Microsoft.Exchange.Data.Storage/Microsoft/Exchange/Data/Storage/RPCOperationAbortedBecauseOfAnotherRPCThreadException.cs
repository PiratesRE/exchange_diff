using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RPCOperationAbortedBecauseOfAnotherRPCThreadException : LocalizedException
	{
		public RPCOperationAbortedBecauseOfAnotherRPCThreadException() : base(ServerStrings.RPCOperationAbortedBecauseOfAnotherRPCThread)
		{
		}

		public RPCOperationAbortedBecauseOfAnotherRPCThreadException(Exception innerException) : base(ServerStrings.RPCOperationAbortedBecauseOfAnotherRPCThread, innerException)
		{
		}

		protected RPCOperationAbortedBecauseOfAnotherRPCThreadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
