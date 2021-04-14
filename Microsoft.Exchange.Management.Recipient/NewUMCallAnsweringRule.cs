using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "UMCallAnsweringRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class NewUMCallAnsweringRule : NewTenantADTaskBase<UMCallAnsweringRule>
	{
		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter]
		[ValidateNotNull]
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

		[ValidateNotNullOrEmpty]
		[Parameter]
		public MultiValuedProperty<CallerIdItem> CallerIds
		{
			get
			{
				return this.DataObject.CallerIds;
			}
			set
			{
				this.DataObject.CallerIds = value;
			}
		}

		[Parameter]
		public bool CallersCanInterruptGreeting
		{
			get
			{
				return this.DataObject.CallersCanInterruptGreeting;
			}
			set
			{
				this.DataObject.CallersCanInterruptGreeting = value;
			}
		}

		[Parameter]
		public bool CheckAutomaticReplies
		{
			get
			{
				return this.DataObject.CheckAutomaticReplies;
			}
			set
			{
				this.DataObject.CheckAutomaticReplies = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.DataObject.Enabled;
			}
			internal set
			{
				this.DataObject.Enabled = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public MultiValuedProperty<string> ExtensionsDialed
		{
			get
			{
				return this.DataObject.ExtensionsDialed;
			}
			set
			{
				this.DataObject.ExtensionsDialed = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public MultiValuedProperty<KeyMapping> KeyMappings
		{
			get
			{
				return this.DataObject.KeyMappings;
			}
			set
			{
				this.DataObject.KeyMappings = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string Name
		{
			get
			{
				return this.DataObject.Name;
			}
			set
			{
				this.DataObject.Name = value;
			}
		}

		[Parameter]
		public int Priority
		{
			get
			{
				return this.DataObject.Priority;
			}
			set
			{
				this.DataObject.Priority = value;
			}
		}

		[Parameter]
		public int ScheduleStatus
		{
			get
			{
				return this.DataObject.ScheduleStatus;
			}
			set
			{
				this.DataObject.ScheduleStatus = value;
			}
		}

		[Parameter]
		[ValidateNotNull]
		public TimeOfDay TimeOfDay
		{
			get
			{
				return this.DataObject.TimeOfDay;
			}
			set
			{
				this.DataObject.TimeOfDay = value;
			}
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 173, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\um\\UMCallAnsweringRule\\NewUMCallAnsweringRule.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ExchangeErrorCategory.Client);
				return adorganizationalUnit.OrganizationId;
			}
			return base.ResolveCurrentOrganization();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewCallAnsweringRule(this.Name);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADObjectId executingUserId;
			base.TryGetExecutingUserId(out executingUserId);
			return UMCallAnsweringRuleUtils.GetDataProviderForCallAnsweringRuleTasks(null, this.Mailbox, base.SessionSettings, base.TenantGlobalCatalogSession, executingUserId, "new-callansweringrule", new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskErrorLoggingDelegate(base.WriteError));
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			UMCallAnsweringRuleDataProvider umcallAnsweringRuleDataProvider = (UMCallAnsweringRuleDataProvider)base.DataSession;
			umcallAnsweringRuleDataProvider.ValidateUMCallAnsweringRuleProperties(this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
		}

		protected override bool IsKnownException(Exception exception)
		{
			return UMCallAnsweringRuleUtils.IsKnownException(exception) || base.IsKnownException(exception);
		}

		protected override void InternalStateReset()
		{
			UMCallAnsweringRuleUtils.DisposeCallAnsweringRuleDataProvider(base.DataSession);
			base.InternalStateReset();
		}

		protected override void Dispose(bool disposing)
		{
			UMCallAnsweringRuleUtils.DisposeCallAnsweringRuleDataProvider(base.DataSession);
			base.Dispose(disposing);
		}
	}
}
