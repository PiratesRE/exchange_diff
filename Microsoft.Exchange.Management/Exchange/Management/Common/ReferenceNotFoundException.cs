using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Common
{
	[Serializable]
	public class ReferenceNotFoundException : ReferenceException
	{
		public ReferenceNotFoundException(string referenceValue, LocalizedException innerException) : base(referenceValue, innerException)
		{
		}

		public ReferenceNotFoundException(string referenceValue, Exception innerException) : base(referenceValue, innerException)
		{
		}

		public ReferenceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
