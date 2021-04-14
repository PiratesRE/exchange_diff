using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.Management.MapiTasks.Presentation
{
	[Serializable]
	public sealed class MailboxStatistics : MailboxStatistics
	{
		public List<MoveHistoryEntry> MoveHistory { get; internal set; }
	}
}
