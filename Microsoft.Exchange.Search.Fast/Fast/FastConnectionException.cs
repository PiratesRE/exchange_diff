using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Fast
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FastConnectionException : ComponentFailedTransientException
	{
		public FastConnectionException() : base(Strings.ConnectionFailure)
		{
		}

		public FastConnectionException(Exception innerException) : base(Strings.ConnectionFailure, innerException)
		{
		}

		protected FastConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
