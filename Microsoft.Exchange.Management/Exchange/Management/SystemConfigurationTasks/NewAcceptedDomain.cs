using System;
using System.Management.Automation;
using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "AcceptedDomain", SupportsShouldProcess = true)]
	public sealed class NewAcceptedDomain : NewMultitenancySystemConfigurationObjectTask<AcceptedDomain>
	{
		[Parameter(Mandatory = true, Position = 0)]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = MailboxTaskHelper.GetNameOfAcceptableLengthForMultiTenantMode(value, out this.nameWarning);
			}
		}

		[Parameter(Mandatory = true)]
		public SmtpDomainWithSubdomains DomainName
		{
			get
			{
				return this.DataObject.DomainName;
			}
			set
			{
				this.DataObject.DomainName = value;
			}
		}

		[Parameter]
		public AcceptedDomainType DomainType
		{
			get
			{
				return this.DataObject.DomainType;
			}
			set
			{
				this.DataObject.DomainType = value;
			}
		}

		[Parameter]
		public AuthenticationType AuthenticationType
		{
			get
			{
				return this.DataObject.RawAuthenticationType;
			}
			set
			{
				this.DataObject.RawAuthenticationType = value;
			}
		}

		[Parameter]
		public LiveIdInstanceType LiveIdInstanceType
		{
			get
			{
				return this.DataObject.RawLiveIdInstanceType;
			}
			set
			{
				this.DataObject.RawLiveIdInstanceType = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter CatchAllRecipient
		{
			get
			{
				return (RecipientIdParameter)base.Fields[AcceptedDomainSchema.CatchAllRecipient];
			}
			set
			{
				base.Fields[AcceptedDomainSchema.CatchAllRecipient] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MatchSubDomains
		{
			get
			{
				return this.DataObject.MatchSubDomains;
			}
			set
			{
				this.DataObject.MatchSubDomains = value;
			}
		}

		[Parameter]
		public MailFlowPartnerIdParameter MailFlowPartner
		{
			get
			{
				return (MailFlowPartnerIdParameter)base.Fields[AcceptedDomainSchema.MailFlowPartner];
			}
			set
			{
				base.Fields[AcceptedDomainSchema.MailFlowPartner] = value;
			}
		}

		[Parameter]
		public bool OutboundOnly
		{
			get
			{
				return this.DataObject.OutboundOnly;
			}
			set
			{
				this.DataObject.OutboundOnly = value;
			}
		}

		[Parameter]
		public bool InitialDomain
		{
			get
			{
				return this.DataObject.InitialDomain;
			}
			set
			{
				this.DataObject.InitialDomain = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipDnsProvisioning
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipDnsProvisioning"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipDnsProvisioning"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipDomainNameValidation
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipDomainNameValidation"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipDomainNameValidation"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewAcceptedDomain(this.Name, this.DomainName.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			if (this.nameWarning != LocalizedString.Empty)
			{
				this.WriteWarning(this.nameWarning);
			}
			base.InternalBeginProcessing();
			MailFlowPartnerIdParameter mailFlowPartner = this.MailFlowPartner;
			if (mailFlowPartner != null)
			{
				MailFlowPartner mailFlowPartner2 = (MailFlowPartner)base.GetDataObject<MailFlowPartner>(mailFlowPartner, base.GlobalConfigSession, this.RootId, new LocalizedString?(Strings.MailFlowPartnerNotExists(mailFlowPartner)), new LocalizedString?(Strings.MailFlowPartnerNotUnique(mailFlowPartner)), ExchangeErrorCategory.Client);
				this.mailFlowPartnerId = (ADObjectId)mailFlowPartner2.Identity;
			}
		}

		internal static void ValidateDomainName(AcceptedDomain domain, Task.TaskErrorLoggingDelegate errorWriter)
		{
			string domain2 = domain.DomainName.Domain;
			DuplicateAcceptedDomainException ex = new DuplicateAcceptedDomainException(domain2);
			ConflictingAcceptedDomainException conflictingAcceptedDomainException = new ConflictingAcceptedDomainException(domain2);
			Exception ex2;
			if (!ADAccountPartitionLocator.ValidateDomainName(domain, ex, conflictingAcceptedDomainException, out ex2))
			{
				ErrorCategory category = ErrorCategory.InvalidOperation;
				if (ex2 == ex)
				{
					category = ErrorCategory.ResourceExists;
				}
				errorWriter(ex2, category, domain);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			AcceptedDomain acceptedDomain = (AcceptedDomain)base.PrepareDataObject();
			acceptedDomain.SetId(this.ConfigurationSession, this.Name);
			if (base.Fields.IsModified(AcceptedDomainSchema.MailFlowPartner))
			{
				acceptedDomain.MailFlowPartner = this.mailFlowPartnerId;
			}
			else
			{
				IConfigurable[] array = base.DataSession.Find<PerimeterConfig>(null, null, true, null);
				if (array != null && array.Length == 1 && ((PerimeterConfig)array[0]).MailFlowPartner != null)
				{
					acceptedDomain.MailFlowPartner = ((PerimeterConfig)array[0]).MailFlowPartner;
				}
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled && !this.DataObject.InitialDomain)
			{
				acceptedDomain.PendingCompletion = true;
			}
			return acceptedDomain;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (Server.IsSubscribedGateway(base.GlobalConfigSession))
			{
				base.WriteError(new CannotRunOnSubscribedEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			base.InternalValidate();
			if (this.SkipDomainNameValidation)
			{
				if (!TemplateTenantConfiguration.IsTemplateTenant(base.OrganizationId))
				{
					base.WriteError(new CannotBypassDomainNameValidationException(), ErrorCategory.InvalidOperation, null);
				}
			}
			else
			{
				NewAcceptedDomain.ValidateDomainName(this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.DataObject.DomainName.Equals(SmtpDomainWithSubdomains.StarDomain))
			{
				this.WriteWarning(Strings.WarnAboutStarAcceptedDomain);
			}
			NewAcceptedDomain.DomainAdditionValidator domainAdditionValidator = new NewAcceptedDomain.DomainAdditionValidator(this);
			domainAdditionValidator.ValidateAllPolicies();
			if (this.DataObject.InitialDomain)
			{
				NewAcceptedDomain.ValidateInitialDomain(this.DataObject, this.ConfigurationSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.DataObject.DomainType == AcceptedDomainType.ExternalRelay && (Datacenter.IsMicrosoftHostedOnly(true) || Datacenter.IsForefrontForOfficeDatacenter()))
			{
				base.WriteError(new ExternalRelayDomainsAreNotAllowedInDatacenterAndFfoException(), ErrorCategory.InvalidOperation, null);
			}
			if (base.Fields.IsModified(AcceptedDomainSchema.CatchAllRecipient) && this.CatchAllRecipient != null)
			{
				this.resolvedCatchAllRecipient = (ADRecipient)base.GetDataObject<ADRecipient>(this.CatchAllRecipient, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.CatchAllRecipientNotExists(this.CatchAllRecipient)), new LocalizedString?(Strings.CatchAllRecipientNotUnique(this.CatchAllRecipient)), ExchangeErrorCategory.Client);
			}
			AcceptedDomainUtility.ValidateCatchAllRecipient(this.resolvedCatchAllRecipient, this.DataObject, base.Fields.IsModified(AcceptedDomainSchema.CatchAllRecipient), new Task.TaskErrorLoggingDelegate(base.WriteError));
			AcceptedDomainUtility.ValidateIfOutboundConnectorToRouteDomainIsPresent(base.DataSession, this.DataObject, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			AcceptedDomainUtility.ValidateMatchSubDomains(this.DataObject.MatchSubDomains, this.DataObject.DomainType, new Task.TaskErrorLoggingDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			this.DataObject.Default = !NewAcceptedDomain.DomainsExist(this.ConfigurationSession, null);
			this.DataObject.AddressBookEnabled = (this.DataObject.DomainType == AcceptedDomainType.Authoritative);
			bool flag = AcceptedDomainUtility.IsCoexistenceDomain(this.DataObject.DomainName.Domain);
			if (flag && !this.SkipDnsProvisioning)
			{
				this.DataObject.IsCoexistenceDomain = true;
				try
				{
					AcceptedDomainUtility.RegisterCoexistenceDomain(this.DataObject.DomainName.Domain);
				}
				catch (TimeoutException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				}
				catch (InvalidOperationException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
				}
				catch (SecurityAccessDeniedException exception3)
				{
					base.WriteError(exception3, ErrorCategory.InvalidArgument, null);
				}
				catch (CommunicationException exception4)
				{
					base.WriteError(exception4, ErrorCategory.InvalidArgument, null);
				}
			}
			if (base.Fields.IsModified(AcceptedDomainSchema.CatchAllRecipient))
			{
				if (this.resolvedCatchAllRecipient != null)
				{
					this.DataObject.CatchAllRecipientID = this.resolvedCatchAllRecipient.OriginalId;
				}
				else
				{
					this.DataObject.CatchAllRecipientID = null;
				}
			}
			base.InternalProcessRecord();
		}

		private static bool DomainsExist(IConfigurationSession session, QueryFilter filter)
		{
			AcceptedDomain[] array = session.Find<AcceptedDomain>(null, QueryScope.SubTree, filter, null, 1);
			return array.Length != 0;
		}

		private static void ValidateInitialDomain(AcceptedDomain domain, IConfigurationSession session, Task.TaskErrorLoggingDelegate errorWriter)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.InitialDomain, true);
			AcceptedDomain[] array = session.Find<AcceptedDomain>(null, QueryScope.SubTree, filter, null, 0);
			if (array.Length != 0)
			{
				errorWriter(new DuplicateInitialDomainException(), ErrorCategory.ResourceExists, domain);
			}
		}

		private LocalizedString nameWarning = LocalizedString.Empty;

		private ADObjectId mailFlowPartnerId;

		private ADRecipient resolvedCatchAllRecipient;

		private class DomainAdditionValidator : SetAcceptedDomain.DomainEditValidator
		{
			public DomainAdditionValidator(NewAcceptedDomain task) : base(new Task.TaskErrorLoggingDelegate(task.WriteError), (IConfigurationSession)task.DataSession, null, task.DataObject)
			{
			}
		}
	}
}
