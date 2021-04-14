using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class RuleParseException : StoragePermanentException
	{
		public RuleParseException(string message) : base(ServerStrings.RuleParseError(message))
		{
		}

		public RuleParseException(LocalizedString message, Exception e) : base(message, e)
		{
		}

		protected RuleParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
