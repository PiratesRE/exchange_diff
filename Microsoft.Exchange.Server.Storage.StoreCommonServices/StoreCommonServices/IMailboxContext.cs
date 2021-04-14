using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IMailboxContext
	{
		int MailboxNumber { get; }

		bool IsUnifiedMailbox { get; }

		string GetDisplayName(Context context);

		bool GetCreatedByMove(Context context);

		bool GetPreservingMailboxSignature(Context context);

		bool GetMRSPreservingMailboxSignature(Context context);

		HashSet<ushort> GetDefaultPromotedMessagePropertyIds(Context context);

		DateTime GetCreationTime(Context context);
	}
}
