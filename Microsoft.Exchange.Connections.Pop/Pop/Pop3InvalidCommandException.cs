using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class Pop3InvalidCommandException : Exception
	{
		public Pop3InvalidCommandException()
		{
		}

		public Pop3InvalidCommandException(string message) : base(message)
		{
		}

		public Pop3InvalidCommandException(string message, Exception innerException) : base(message, innerException)
		{
		}

		internal Pop3InvalidCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
