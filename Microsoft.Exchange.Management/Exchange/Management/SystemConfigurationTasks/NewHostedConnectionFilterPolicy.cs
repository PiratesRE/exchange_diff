using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "HostedConnectionFilterPolicy", SupportsShouldProcess = true)]
	public sealed class NewHostedConnectionFilterPolicy : NewMultitenancySystemConfigurationObjectTask<HostedConnectionFilterPolicy>
	{
		[Parameter]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		[Parameter(Mandatory = true, Position = 0)]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Parameter]
		public string AdminDisplayName
		{
			get
			{
				return this.DataObject.AdminDisplayName;
			}
			set
			{
				this.DataObject.AdminDisplayName = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<IPRange> IPAllowList
		{
			get
			{
				return this.DataObject.IPAllowList;
			}
			set
			{
				this.DataObject.IPAllowList = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<IPRange> IPBlockList
		{
			get
			{
				return this.DataObject.IPBlockList;
			}
			set
			{
				this.DataObject.IPBlockList = value;
			}
		}

		[Parameter]
		public bool EnableSafeList
		{
			get
			{
				return this.DataObject.EnableSafeList;
			}
			set
			{
				this.DataObject.EnableSafeList = value;
			}
		}

		[Parameter]
		public DirectoryBasedEdgeBlockMode DirectoryBasedEdgeBlockMode
		{
			get
			{
				return this.DataObject.DirectoryBasedEdgeBlockMode;
			}
			set
			{
				this.DataObject.DirectoryBasedEdgeBlockMode = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewHostedConnectionFilterPolicy(this.Name);
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			HostedConnectionFilterPolicy hostedConnectionFilterPolicy = (HostedConnectionFilterPolicy)base.PrepareDataObject();
			hostedConnectionFilterPolicy.SetId((IConfigurationSession)base.DataSession, this.Name);
			if (!NewHostedConnectionFilterPolicy.HostedConnectionFilterPolicyExist((IConfigurationSession)base.DataSession, null))
			{
				this.DataObject.IsDefault = true;
			}
			TaskLogger.LogExit();
			return hostedConnectionFilterPolicy;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.CreateParentContainerIfNeeded(this.DataObject);
			base.InternalProcessRecord();
			FfoDualWriter.SaveToFfo<HostedConnectionFilterPolicy>(this, this.DataObject, null);
			TaskLogger.LogExit();
		}

		private static bool HostedConnectionFilterPolicyExist(IConfigurationSession session, QueryFilter filter)
		{
			HostedConnectionFilterPolicy[] array = session.Find<HostedConnectionFilterPolicy>(null, QueryScope.SubTree, filter, null, 1);
			return array.Length != 0;
		}
	}
}
