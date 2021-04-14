using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class UnifiedCustomSyncStateInfo : SyncStateInfo
	{
		public override string UniqueName
		{
			get
			{
				return "Microsoft.Exchange.MailboxUnified.ContentAggregation.UnifiedCustomSyncState";
			}
			set
			{
				throw new InvalidOperationException("This property is not settable.");
			}
		}

		public override int Version
		{
			get
			{
				return 1;
			}
		}

		private const string NameKey = "Microsoft.Exchange.MailboxUnified.ContentAggregation.UnifiedCustomSyncState";
	}
}
