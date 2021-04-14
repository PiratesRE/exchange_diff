using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IRecipientChangeTracker
	{
		void AddRecipient(CoreRecipient coreRecipient, out bool considerRecipientModified);

		void RemoveAddedRecipient(CoreRecipient coreRecipient);

		void RemoveUnchangedRecipient(CoreRecipient coreRecipient);

		void RemoveModifiedRecipient(CoreRecipient coreRecipient);

		void OnModifyRecipient(CoreRecipient coreRecipient);
	}
}
