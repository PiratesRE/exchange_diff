using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[KnownType(typeof(CreateCalendarEventActionResult))]
	[DataContract]
	[KnownType(typeof(MoveActionResult))]
	internal abstract class ReplayActionResult
	{
		public ReplayActionResult()
		{
		}

		public override string ToString()
		{
			return base.GetType().Name;
		}

		internal const ReplayActionResult Void = null;
	}
}
