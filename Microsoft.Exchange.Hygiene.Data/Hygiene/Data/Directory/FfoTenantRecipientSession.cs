using System;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal class FfoTenantRecipientSession : FfoRecipientSession, ITenantRecipientSession, IRecipientSession, IDirectorySession, IConfigDataProvider
	{
		public FfoTenantRecipientSession(bool useConfigNC, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(useConfigNC, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
		}

		public FfoTenantRecipientSession(ADObjectId tenantId) : base(tenantId)
		{
		}

		ADRawEntry ITenantRecipientSession.ChooseBetweenAmbiguousUsers(ADRawEntry[] entries)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADObjectId ITenantRecipientSession.ChooseBetweenAmbiguousUsers(ADObjectId user1Id, ADObjectId user2Id)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		public DirectoryBackendType DirectoryBackendType
		{
			get
			{
				return DirectoryBackendType.SQL;
			}
		}

		ADRawEntry[] ITenantRecipientSession.FindByNetID(string netID, string organizationContext, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRawEntry[0];
		}

		ADRawEntry[] ITenantRecipientSession.FindByNetID(string netID, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRawEntry[0];
		}

		MiniRecipient ITenantRecipientSession.FindRecipientByNetID(NetID netId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADRawEntry ITenantRecipientSession.FindUniqueEntryByNetID(string netID, string organizationContext, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADRawEntry ITenantRecipientSession.FindUniqueEntryByNetID(string netID, params PropertyDefinition[] properties)
		{
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, IADSecurityPrincipalSchema.NetID, new NetID(netID));
			return ((IConfigDataProvider)this).Find<ADUser>(filter, null, false, null).Cast<ADUser>().FirstOrDefault<ADUser>();
		}

		ADRawEntry ITenantRecipientSession.FindByLiveIdMemberName(string liveIdMemberName, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		Result<ADRawEntry>[] ITenantRecipientSession.ReadMultipleByLinkedPartnerId(LinkedPartnerGroupInformation[] entryIds, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRawEntry>[0];
		}

		Result<ADRawEntry>[] ITenantRecipientSession.FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRawEntry>[0];
		}

		Result<ADRawEntry>[] ITenantRecipientSession.FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, bool includeDeletedObjects, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRawEntry>[0];
		}
	}
}
