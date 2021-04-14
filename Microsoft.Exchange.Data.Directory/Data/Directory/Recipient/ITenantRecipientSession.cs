using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface ITenantRecipientSession : IRecipientSession, IDirectorySession, IConfigDataProvider
	{
		ADRawEntry ChooseBetweenAmbiguousUsers(ADRawEntry[] entries);

		ADObjectId ChooseBetweenAmbiguousUsers(ADObjectId user1Id, ADObjectId user2Id);

		DirectoryBackendType DirectoryBackendType { get; }

		Result<ADRawEntry>[] FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, params PropertyDefinition[] properties);

		Result<ADRawEntry>[] FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, bool includeDeletedObjects, params PropertyDefinition[] properties);

		ADRawEntry[] FindByNetID(string netID, string organizationContext, params PropertyDefinition[] properties);

		ADRawEntry[] FindByNetID(string netID, params PropertyDefinition[] properties);

		MiniRecipient FindRecipientByNetID(NetID netId);

		ADRawEntry FindUniqueEntryByNetID(string netID, string organizationContext, params PropertyDefinition[] properties);

		ADRawEntry FindUniqueEntryByNetID(string netID, params PropertyDefinition[] properties);

		ADRawEntry FindByLiveIdMemberName(string liveIdMemberName, params PropertyDefinition[] properties);

		Result<ADRawEntry>[] ReadMultipleByLinkedPartnerId(LinkedPartnerGroupInformation[] entryIds, params PropertyDefinition[] properties);
	}
}
