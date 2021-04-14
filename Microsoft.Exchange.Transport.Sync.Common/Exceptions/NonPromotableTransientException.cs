using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Exceptions
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class NonPromotableTransientException : TransientException
	{
		public NonPromotableTransientException(LocalizedString localizedString) : base(localizedString)
		{
		}

		public NonPromotableTransientException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
		}

		protected NonPromotableTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
