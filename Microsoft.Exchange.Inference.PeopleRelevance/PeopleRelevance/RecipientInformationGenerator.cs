using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Inference;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.PeopleICommunicateWith;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RecipientInformationGenerator
	{
		public RecipientInformationGenerator()
		{
			this.DiagnosticsSession = Microsoft.Exchange.Search.Core.Diagnostics.DiagnosticsSession.CreateComponentDiagnosticsSession("RecipientInformationGenerator", ExTraceGlobals.MdbTrainingFeederTracer, (long)this.GetHashCode());
		}

		public IDiagnosticsSession DiagnosticsSession { get; set; }

		internal IEnumerable<IRecipientInfo> RunTrainingQuery(MailboxSession session)
		{
			this.DiagnosticsSession.TraceDebug("Querying Recipient information from mailbox", new object[0]);
			IPicwActions picwActions = PicwActions.Create(session);
			ICollection<IRecipientInfo> recipientInfoItems = picwActions.GetRecipientInfoItems(RecipientInfoSortType.LastSentTimeUtc, SortOrder.Ascending);
			return (from s in recipientInfoItems
			where s.SentCount > 0U
			select s).ToList<IRecipientInfo>();
		}

		private const string ComponentName = "RecipientInformationGenerator";
	}
}
