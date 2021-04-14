using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class JournalRuleCorruptException : LocalizedException
	{
		public JournalRuleCorruptException() : base(Strings.JournalRuleCorrupt)
		{
		}

		public JournalRuleCorruptException(Exception innerException) : base(Strings.JournalRuleCorrupt, innerException)
		{
		}

		protected JournalRuleCorruptException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
