using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Get", "Notification", DefaultParameterSetName = "Filter")]
	public sealed class GetNotification : GetTenantADObjectWithIdentityTaskBase<EwsStoreObjectIdParameter, AsyncOperationNotification>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Filter")]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter Summary
		{
			get
			{
				return (SwitchParameter)(base.Fields["Summary"] ?? false);
			}
			set
			{
				base.Fields["Summary"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Settings")]
		public SwitchParameter Settings
		{
			get
			{
				return (SwitchParameter)(base.Fields["Settings"] ?? false);
			}
			set
			{
				base.Fields["Settings"] = value;
			}
		}

		[Parameter]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filter")]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filter")]
		public ExDateTime? StartDate
		{
			get
			{
				return (ExDateTime?)base.Fields["StartDate"];
			}
			set
			{
				base.Fields["StartDate"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Settings")]
		[Parameter(Mandatory = false, ParameterSetName = "Filter")]
		public AsyncOperationType ProcessType
		{
			get
			{
				return (AsyncOperationType)(base.Fields["ProcessType"] ?? AsyncOperationType.Unknown);
			}
			set
			{
				base.Fields["ProcessType"] = value;
			}
		}

		protected override Unlimited<uint> DefaultResultSize
		{
			get
			{
				return new Unlimited<uint>(50U);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new AsyncOperationNotificationDataProvider(base.CurrentOrganizationId);
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 161, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\AsyncOperationNotification\\GetNotification.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				return adorganizationalUnit.OrganizationId;
			}
			return base.ResolveCurrentOrganization();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (base.ParameterSetName == "Settings")
			{
				string id;
				if (AsyncOperationNotificationDataProvider.SettingsObjectIdentityMap.TryGetValue(this.ProcessType, out id))
				{
					this.Identity = new EwsStoreObjectIdParameter(id);
				}
				else
				{
					base.WriteError(new ArgumentException(Strings.ErrorInvalidAsyncNotificationProcessType(this.ProcessType.ToString())), ErrorCategory.InvalidArgument, this.ProcessType);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			if (this.Identity != null)
			{
				this.WriteResult(base.GetDataObject(this.Identity));
				return;
			}
			ProviderPropertyDefinition[] properties = this.Summary ? new ProviderPropertyDefinition[]
			{
				EwsStoreObjectSchema.AlternativeId,
				AsyncOperationNotificationSchema.Type,
				AsyncOperationNotificationSchema.Status
			} : null;
			this.WriteResult<AsyncOperationNotification>(((AsyncOperationNotificationDataProvider)base.DataSession).GetNotificationDetails(base.Fields.IsModified("ProcessType") ? new AsyncOperationType?(this.ProcessType) : null, this.StartDate, this.ResultSize.IsUnlimited ? null : new int?((int)this.ResultSize.Value), properties));
		}

		private const string FilterParameterSet = "Filter";

		private const string SettingsParameterSet = "Settings";

		private const string SummaryParameter = "Summary";

		private const string SettingsParameter = "Settings";

		private const string ProcessTypeParameter = "ProcessType";
	}
}
