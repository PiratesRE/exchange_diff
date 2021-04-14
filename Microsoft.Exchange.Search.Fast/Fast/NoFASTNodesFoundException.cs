using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Fast
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NoFASTNodesFoundException : ComponentFailedPermanentException
	{
		public NoFASTNodesFoundException() : base(Strings.NoFASTNodesFound)
		{
		}

		public NoFASTNodesFoundException(Exception innerException) : base(Strings.NoFASTNodesFound, innerException)
		{
		}

		protected NoFASTNodesFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
