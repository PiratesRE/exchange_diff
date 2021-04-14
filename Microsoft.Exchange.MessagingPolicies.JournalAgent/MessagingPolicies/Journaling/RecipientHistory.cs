using System;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal class RecipientHistory
	{
		internal IHistoryRecordFacade FirstRecord
		{
			get
			{
				return this.firstRecord;
			}
		}

		internal RecipientP2Type P2Type
		{
			get
			{
				return this.p2Type;
			}
		}

		internal RecipientHistory(IHistoryFacade globalHistory, IHistoryFacade perRecipientHistory)
		{
			if (globalHistory != null)
			{
				this.p2Type = globalHistory.RecipientType;
				if (globalHistory.Records != null && globalHistory.Records.Count > 0)
				{
					this.firstRecord = globalHistory.Records[0];
					return;
				}
			}
			if (perRecipientHistory != null)
			{
				if (this.p2Type == RecipientP2Type.Unknown)
				{
					this.p2Type = perRecipientHistory.RecipientType;
				}
				if (perRecipientHistory.Records != null && perRecipientHistory.Records.Count > 0)
				{
					this.firstRecord = perRecipientHistory.Records[0];
				}
			}
		}

		private IHistoryRecordFacade firstRecord;

		private RecipientP2Type p2Type;
	}
}
