using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ActiveSyncMiniRecipientSchema : StorageMiniRecipientSchema
	{
		public static readonly ADPropertyDefinition ActiveSyncEnabled = ADUserSchema.ActiveSyncEnabled;

		public static readonly ADPropertyDefinition ActiveSyncMailboxPolicy = ADUserSchema.ActiveSyncMailboxPolicy;

		public static readonly ADPropertyDefinition ActiveSyncAllowedDeviceIDs = ADUserSchema.ActiveSyncAllowedDeviceIDs;

		public static readonly ADPropertyDefinition ActiveSyncBlockedDeviceIDs = ADUserSchema.ActiveSyncBlockedDeviceIDs;
	}
}
