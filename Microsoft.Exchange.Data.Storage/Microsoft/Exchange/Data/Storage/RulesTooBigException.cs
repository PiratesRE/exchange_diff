using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class RulesTooBigException : StoragePermanentException
	{
		public RulesTooBigException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected RulesTooBigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
