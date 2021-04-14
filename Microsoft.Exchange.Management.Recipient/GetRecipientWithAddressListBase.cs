using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class GetRecipientWithAddressListBase<TIdentity, TDataObject> : GetRecipientBase<TIdentity, TDataObject> where TIdentity : RecipientIdParameter, new() where TDataObject : ADObject, new()
	{
		protected virtual string SystemAddressListRdn
		{
			get
			{
				return null;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && !string.IsNullOrEmpty(this.SystemAddressListRdn) && this.ConfigurationSession.GetOrgContainer().IsAddressListPagingEnabled)
			{
				this.addressListId = base.CurrentOrgContainerId.GetDescendantId(SystemAddressList.RdnSystemAddressListContainerToOrganization).GetChildId(this.SystemAddressListRdn);
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = base.InternalFilter;
				if (this.addressListId != null && this.Identity == null && base.AccountPartition == null && !base.SessionSettings.IncludeSoftDeletedObjects && !base.SessionSettings.IncludeInactiveMailbox && (base.ParameterSetName == "Identity" || base.ParameterSetName == "CookieSet"))
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AddressListMembership, this.addressListId),
						new ExistsFilter(ADRecipientSchema.DisplayName),
						queryFilter
					});
					this.addressListUsed = true;
				}
				return queryFilter;
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (this.addressListUsed && base.WriteObjectCount == 0U && this.ConfigurationSession.Read<AddressBookBase>(this.addressListId) == null)
			{
				this.WriteWarning(Strings.WarningSystemAddressListNotFound(this.addressListId.Name));
				this.addressListId = null;
				base.InternalProcessRecord();
			}
		}

		private ADObjectId addressListId;

		private bool addressListUsed;
	}
}
