using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Get", "RoleGroupMember")]
	public sealed class GetRoleGroupMember : GetRecipientObjectTask<RoleGroupMemberIdParameter, ReducedRecipient>
	{
		private ADObjectId RootOrgUSGContainerId
		{
			get
			{
				if (this.rootOrgUSGContainerId == null)
				{
					this.rootOrgUSGContainerId = RoleGroupCommon.GetRootOrgUsgContainerId(this.ConfigurationSession, base.ServerSettings, base.PartitionOrRootOrgGlobalCatalogSession, base.CurrentOrganizationId);
				}
				return this.rootOrgUSGContainerId;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true)]
		public override RoleGroupMemberIdParameter Identity
		{
			get
			{
				return (RoleGroupMemberIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override Unlimited<uint> DefaultResultSize
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		internal new PSCredential Credential
		{
			get
			{
				return null;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeSoftDeletedObjects
		{
			get
			{
				return (SwitchParameter)(base.Fields["SoftDeletedObject"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SoftDeletedObject"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.OptionalIdentityData.RootOrgDomainContainerId = this.RootOrgUSGContainerId;
			base.InternalValidate();
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.IncludeSoftDeletedObjects.IsPresent)
			{
				base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjects = true;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			LocalizedString? localizedString;
			IEnumerable<ReducedRecipient> dataObjects = base.GetDataObjects(this.Identity, base.OptionalIdentityData, out localizedString);
			this.WriteResult<ReducedRecipient>(dataObjects);
			if (!base.HasErrors && localizedString != null)
			{
				base.WriteError(new ManagementObjectNotFoundException(localizedString.Value), ErrorCategory.InvalidData, null);
			}
			TaskLogger.LogExit();
		}

		private ADObjectId rootOrgUSGContainerId;
	}
}
