using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("get", "RemoteMailbox", DefaultParameterSetName = "Identity")]
	public sealed class GetRemoteMailbox : GetMailUserBase<RemoteMailboxIdParameter>
	{
		[Parameter]
		public OrganizationalUnitIdParameter OnPremisesOrganizationalUnit
		{
			get
			{
				return this.OrganizationalUnit;
			}
			set
			{
				this.OrganizationalUnit = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<RemoteMailboxSchema>();
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.InternalFilter,
					RemoteMailboxIdParameter.GetRemoteMailboxFilter(),
					this.Archive.IsPresent ? GetMailboxOrSyncMailbox.RemoteArchiveFilter : null
				});
			}
		}

		private new OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return base.OrganizationalUnit;
			}
			set
			{
				base.OrganizationalUnit = value;
			}
		}

		private new OrganizationIdParameter Organization
		{
			get
			{
				return base.Organization;
			}
			set
			{
				base.Organization = value;
			}
		}

		private new AccountPartitionIdParameter AccountPartition
		{
			get
			{
				return base.AccountPartition;
			}
			set
			{
				base.AccountPartition = value;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return RemoteMailbox.FromDataObject((ADUser)dataObject);
		}

		protected override void InternalBeginProcessing()
		{
			base.OptionalIdentityData.AdditionalFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				base.OptionalIdentityData.AdditionalFilter,
				this.Archive.IsPresent ? GetMailboxOrSyncMailbox.RemoteArchiveFilter : null
			});
			base.InternalBeginProcessing();
		}
	}
}
