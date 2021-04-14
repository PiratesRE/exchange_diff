using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Common
{
	[Serializable]
	public class ReferenceInvalidException : ReferenceException
	{
		public ReferenceInvalidException(string referenceValue, LocalizedException innerException) : base(referenceValue, innerException)
		{
		}

		public ReferenceInvalidException(string referenceValue, Exception innerException) : base(referenceValue, innerException)
		{
		}

		public ReferenceInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
