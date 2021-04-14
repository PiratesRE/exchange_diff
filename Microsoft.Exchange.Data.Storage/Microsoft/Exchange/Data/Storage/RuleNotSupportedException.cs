using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class RuleNotSupportedException : StoragePermanentException
	{
		public RuleNotSupportedException(LocalizedString message) : base(message)
		{
		}

		public RuleNotSupportedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected RuleNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
