using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface ISchemaVersion
	{
		ComponentVersion GetCurrentSchemaVersion(Context context);

		void SetCurrentSchemaVersion(Context context, ComponentVersion version);

		string Identifier { get; }

		int MailboxNumber { get; }
	}
}
