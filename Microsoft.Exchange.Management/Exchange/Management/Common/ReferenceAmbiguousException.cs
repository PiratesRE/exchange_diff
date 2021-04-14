using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Common
{
	[Serializable]
	public class ReferenceAmbiguousException : ReferenceException
	{
		public ReferenceAmbiguousException(string referenceValue, LocalizedException innerException) : base(referenceValue, innerException)
		{
		}

		public ReferenceAmbiguousException(string referenceValue, Exception innerException) : base(referenceValue, innerException)
		{
		}

		public ReferenceAmbiguousException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
