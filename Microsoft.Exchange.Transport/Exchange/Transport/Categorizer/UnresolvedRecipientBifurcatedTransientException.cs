using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Transport.Categorizer
{
	[Serializable]
	internal class UnresolvedRecipientBifurcatedTransientException : Exception
	{
		public UnresolvedRecipientBifurcatedTransientException()
		{
		}

		protected UnresolvedRecipientBifurcatedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
