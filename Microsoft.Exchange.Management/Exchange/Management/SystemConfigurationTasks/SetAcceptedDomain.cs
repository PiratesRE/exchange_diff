using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ManagementEndpoint;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.LoggingCommon;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "AcceptedDomain", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetAcceptedDomain : SetSystemConfigurationObjectTask<AcceptedDomainIdParameter, AcceptedDomain>
	{
		[Parameter]
		public bool MakeDefault
		{
			get
			{
				return this.makeDefault;
			}
			set
			{
				this.makeDefault = value;
			}
		}

		[Parameter]
		public bool IsCoexistenceDomain
		{
			get
			{
				return (bool)base.Fields[AcceptedDomainSchema.IsCoexistenceDomain];
			}
			set
			{
				base.Fields[AcceptedDomainSchema.IsCoexistenceDomain] = value;
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
				return (bool)base.Fields[AcceptedDomainSchema.MatchSubDomains];
			}
			set
			{
				base.Fields[AcceptedDomainSchema.MatchSubDomains] = value;
			}
		}

		[Parameter(Mandatory = false)]
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
		public AuthenticationType AuthenticationType
		{
			get
			{
				return (AuthenticationType)base.Fields[AcceptedDomainSchema.RawAuthenticationType];
			}
			set
			{
				base.Fields[AcceptedDomainSchema.RawAuthenticationType] = value;
			}
		}

		[Parameter]
		public bool InitialDomain
		{
			get
			{
				return (bool)base.Fields[AcceptedDomainSchema.InitialDomain];
			}
			set
			{
				base.Fields[AcceptedDomainSchema.InitialDomain] = value;
			}
		}

		[Parameter]
		public LiveIdInstanceType LiveIdInstanceType
		{
			get
			{
				return (LiveIdInstanceType)base.Fields[AcceptedDomainSchema.RawLiveIdInstanceType];
			}
			set
			{
				base.Fields[AcceptedDomainSchema.RawLiveIdInstanceType] = value;
			}
		}

		[Parameter]
		public bool EnableNego2Authentication
		{
			get
			{
				return (bool)(base.Fields[AcceptedDomainSchema.EnableNego2Authentication] ?? false);
			}
			set
			{
				base.Fields[AcceptedDomainSchema.EnableNego2Authentication] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAcceptedDomain(this.Identity.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			AcceptedDomain acceptedDomain = (AcceptedDomain)this.GetDynamicParameters();
			if (base.Fields.IsModified(AcceptedDomainSchema.MailFlowPartner))
			{
				MailFlowPartnerIdParameter mailFlowPartner = this.MailFlowPartner;
				if (mailFlowPartner != null)
				{
					MailFlowPartner mailFlowPartner2 = (MailFlowPartner)base.GetDataObject<MailFlowPartner>(mailFlowPartner, base.GlobalConfigSession, this.RootId, new LocalizedString?(Strings.MailFlowPartnerNotExists(mailFlowPartner)), new LocalizedString?(Strings.MailFlowPartnerNotUnique(mailFlowPartner)), ExchangeErrorCategory.Client);
					acceptedDomain.MailFlowPartner = (ADObjectId)mailFlowPartner2.Identity;
					return;
				}
				acceptedDomain.MailFlowPartner = null;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.makeDefault && !this.DataObject.Default)
			{
				this.DataObject.Default = true;
				ADPagedReader<AcceptedDomain> adpagedReader = ((IConfigurationSession)base.DataSession).FindPaged<AcceptedDomain>(this.DataObject.Id.Parent, QueryScope.OneLevel, null, null, 0);
				foreach (AcceptedDomain acceptedDomain in adpagedReader)
				{
					if (acceptedDomain.Default)
					{
						acceptedDomain.Default = false;
						base.DataSession.Save(acceptedDomain);
					}
				}
			}
			if (base.Fields.IsModified(AcceptedDomainSchema.IsCoexistenceDomain) && this.DataObject.IsCoexistenceDomain != this.IsCoexistenceDomain)
			{
				if (!this.IsCoexistenceDomain)
				{
					try
					{
						AcceptedDomainUtility.DeregisterCoexistenceDomain(this.DataObject.DomainName.Domain);
						goto IL_162;
					}
					catch (TimeoutException exception)
					{
						base.WriteError(exception, ErrorCategory.InvalidArgument, null);
						goto IL_162;
					}
					catch (InvalidOperationException exception2)
					{
						base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
						goto IL_162;
					}
					catch (SecurityAccessDeniedException exception3)
					{
						base.WriteError(exception3, ErrorCategory.InvalidArgument, null);
						goto IL_162;
					}
					catch (CommunicationException exception4)
					{
						base.WriteError(exception4, ErrorCategory.InvalidArgument, null);
						goto IL_162;
					}
				}
				try
				{
					AcceptedDomainUtility.RegisterCoexistenceDomain(this.DataObject.DomainName.Domain);
				}
				catch (TimeoutException exception5)
				{
					base.WriteError(exception5, ErrorCategory.InvalidArgument, null);
				}
				catch (InvalidOperationException exception6)
				{
					base.WriteError(exception6, ErrorCategory.InvalidArgument, null);
				}
				catch (SecurityAccessDeniedException exception7)
				{
					base.WriteError(exception7, ErrorCategory.InvalidArgument, null);
				}
				catch (CommunicationException exception8)
				{
					base.WriteError(exception8, ErrorCategory.InvalidArgument, null);
				}
				IL_162:
				this.DataObject.IsCoexistenceDomain = this.IsCoexistenceDomain;
			}
			if (base.Fields.IsModified(AcceptedDomainSchema.RawAuthenticationType))
			{
				this.DataObject.RawAuthenticationType = this.AuthenticationType;
			}
			if (base.Fields.IsModified(AcceptedDomainSchema.InitialDomain))
			{
				this.DataObject.InitialDomain = this.InitialDomain;
			}
			if (base.Fields.IsModified(AcceptedDomainSchema.RawLiveIdInstanceType))
			{
				this.DataObject.RawLiveIdInstanceType = this.LiveIdInstanceType;
			}
			if (base.Fields.IsModified(AcceptedDomainSchema.EnableNego2Authentication))
			{
				this.DataObject.EnableNego2Authentication = this.EnableNego2Authentication;
				if (ManagementEndpointBase.IsGlobalDirectoryConfigured())
				{
					IGlobalDirectorySession globalSession = DirectorySessionFactory.GetGlobalSession(null);
					globalSession.SetDomainFlag(this.DataObject.Name, GlsDomainFlags.Nego2Enabled, this.EnableNego2Authentication);
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
			if (base.Fields.IsModified(AcceptedDomainSchema.MatchSubDomains))
			{
				this.DataObject.MatchSubDomains = this.MatchSubDomains;
			}
			base.InternalProcessRecord();
			FfoDualWriter.SaveToFfo<AcceptedDomain>(this, this.DataObject, TenantSettingSyncLogType.SYNCACCEPTEDDOM, null);
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			if (Server.IsSubscribedGateway(base.GlobalConfigSession))
			{
				base.WriteError(new CannotRunOnSubscribedEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			base.InternalValidate();
			if (this.DataObject.PendingRemoval && !this.DataObject.IsChanged(AcceptedDomainSchema.PendingRemoval))
			{
				base.WriteError(new CannotOperateOnAcceptedDomainPendingRemovalException(this.DataObject.DomainName.ToString()), ErrorCategory.InvalidOperation, null);
			}
			if (this.DataObject.PendingRemoval && this.DataObject.IsChanged(AcceptedDomainSchema.PendingRemoval))
			{
				RemoveAcceptedDomain.CheckDomainForRemoval(this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.DataObject.IsChanged(ADObjectSchema.Name))
			{
				NewAcceptedDomain.ValidateDomainName(this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.DataObject.DomainName.Equals(SmtpDomainWithSubdomains.StarDomain))
			{
				this.WriteWarning(Strings.WarnAboutStarAcceptedDomain);
			}
			SetAcceptedDomain.DomainEditValidator domainEditValidator = new SetAcceptedDomain.DomainEditValidator(this);
			domainEditValidator.ValidateAllPolicies();
			if (this.DataObject.IsChanged(AcceptedDomainSchema.AcceptedDomainType) && this.DataObject.DomainType == AcceptedDomainType.ExternalRelay && (Datacenter.IsMicrosoftHostedOnly(true) || Datacenter.IsForefrontForOfficeDatacenter()))
			{
				base.WriteError(new ExternalRelayDomainsAreNotAllowedInDatacenterAndFfoException(), ErrorCategory.InvalidOperation, null);
			}
			if (base.Fields.IsModified(AcceptedDomainSchema.CatchAllRecipient) && this.CatchAllRecipient != null)
			{
				this.resolvedCatchAllRecipient = (ADRecipient)base.GetDataObject<ADRecipient>(this.CatchAllRecipient, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.CatchAllRecipientNotExists(this.CatchAllRecipient)), new LocalizedString?(Strings.CatchAllRecipientNotUnique(this.CatchAllRecipient)), ExchangeErrorCategory.Client);
			}
			AcceptedDomainUtility.ValidateCatchAllRecipient(this.resolvedCatchAllRecipient, this.DataObject, base.Fields.IsModified(AcceptedDomainSchema.CatchAllRecipient), new Task.TaskErrorLoggingDelegate(base.WriteError));
			AcceptedDomainUtility.ValidateIfOutboundConnectorToRouteDomainIsPresent(base.DataSession, this.DataObject, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			if (this.DataObject.IsChanged(AcceptedDomainSchema.AcceptedDomainType) || base.Fields.IsModified(AcceptedDomainSchema.MatchSubDomains))
			{
				bool matchSubDomains = base.Fields.IsModified(AcceptedDomainSchema.MatchSubDomains) ? this.MatchSubDomains : this.DataObject.MatchSubDomains;
				AcceptedDomainUtility.ValidateMatchSubDomains(matchSubDomains, this.DataObject.DomainType, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
		}

		private bool makeDefault;

		private ADRecipient resolvedCatchAllRecipient;

		internal class DomainEditValidator : UpdateEmailAddressPolicy.DomainValidator
		{
			protected Task.TaskErrorLoggingDelegate ErrorWriter
			{
				get
				{
					return this.errorWriter;
				}
			}

			protected AcceptedDomain OldDomain
			{
				get
				{
					return this.oldDomain;
				}
			}

			public DomainEditValidator(SetAcceptedDomain task) : this(new Task.TaskErrorLoggingDelegate(task.WriteError), (IConfigurationSession)task.DataSession, SetAcceptedDomain.DomainEditValidator.LoadOldVersion(task), task.DataObject)
			{
			}

			public DomainEditValidator(Task.TaskErrorLoggingDelegate errorWriter, IConfigurationSession session, AcceptedDomain oldDomain, AcceptedDomain newDomain) : this(session, oldDomain, newDomain)
			{
				this.errorWriter = errorWriter;
			}

			private DomainEditValidator(IConfigurationSession session, AcceptedDomain oldDomain, AcceptedDomain newDomain) : base(SetAcceptedDomain.DomainEditValidator.FindConflictingDomains(session, oldDomain, newDomain))
			{
				this.oldDomain = oldDomain;
				this.newDomain = newDomain;
				this.session = session;
			}

			public void ValidateAllPolicies()
			{
				if (this.oldDomain != null && this.oldDomain.FederatedOrganizationLink != null && (this.newDomain == null || this.oldDomain.DomainName.Domain != this.newDomain.DomainName.Domain) && !SetAcceptedDomain.DomainEditValidator.isMultiTenancyEnabled)
				{
					this.errorWriter(new CannotRemoveFederatedAcceptedDomainException(this.oldDomain.DomainName.Domain), ErrorCategory.InvalidOperation, this.oldDomain.Identity);
				}
				foreach (EmailAddressPolicy policy in this.session.FindAllPaged<EmailAddressPolicy>())
				{
					base.Validate(policy);
				}
			}

			protected override void WriteInvalidTemplate(SmtpProxyAddressTemplate template)
			{
				if (this.oldDomain == null)
				{
					return;
				}
				if (!this.newDomain.IsChanged(AcceptedDomainSchema.AcceptedDomainType) || this.newDomain.DomainType != AcceptedDomainType.ExternalRelay)
				{
					return;
				}
				if (this.IsUsedBy(template))
				{
					this.ErrorWriter(new LocalizedException(Strings.CannotMakeAcceptedDomainExternalRelaySinceItIsReferencedByAddressTemplate(this.OldDomain.DomainName, template)), ErrorCategory.InvalidOperation, this.OldDomain.Identity);
				}
			}

			protected override void HandleNonAuthoritativeDomains(EmailAddressPolicy policy, HashSet<SmtpDomain> domains)
			{
				ProxyAddressTemplateCollection proxyAddressTemplateCollection = new ProxyAddressTemplateCollection();
				foreach (ProxyAddressTemplate proxyAddressTemplate in policy.NonAuthoritativeDomains)
				{
					SmtpDomain template;
					if (!UpdateEmailAddressPolicy.DomainValidator.TryGetDomain(proxyAddressTemplate, out template) || (!SetAcceptedDomain.DomainEditValidator.Conflict(this.newDomain, template) && !SetAcceptedDomain.DomainEditValidator.Conflict(this.oldDomain, template)))
					{
						proxyAddressTemplateCollection.Add(proxyAddressTemplate);
					}
				}
				foreach (SmtpDomain smtpDomain in domains)
				{
					if (SetAcceptedDomain.DomainEditValidator.Conflict(this.newDomain, smtpDomain) || SetAcceptedDomain.DomainEditValidator.Conflict(this.oldDomain, smtpDomain))
					{
						SmtpProxyAddressTemplate item = new SmtpProxyAddressTemplate("@" + smtpDomain.Domain, false);
						proxyAddressTemplateCollection.Add(item);
					}
				}
				UpdateEmailAddressPolicy.CheckEapVersion(policy);
				policy.NonAuthoritativeDomains = proxyAddressTemplateCollection;
				this.session.Save(policy);
			}

			protected static bool Conflict(AcceptedDomain accepted, SmtpDomain template)
			{
				return accepted != null && accepted.DomainName != null && accepted.DomainName.Match(template.Domain) != -1;
			}

			protected bool IsUsedBy(SmtpProxyAddressTemplate template)
			{
				SmtpDomain template2;
				return UpdateEmailAddressPolicy.DomainValidator.TryGetDomain(template, out template2) && SetAcceptedDomain.DomainEditValidator.Conflict(this.OldDomain, template2);
			}

			private static AcceptedDomain LoadOldVersion(SetAcceptedDomain task)
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, task.DataObject.Guid);
				AcceptedDomain[] array = task.ConfigurationSession.Find<AcceptedDomain>(null, QueryScope.SubTree, filter, null, 1);
				if (array.Length != 0)
				{
					return array[0];
				}
				return null;
			}

			private static IEnumerable<AcceptedDomain> FindConflictingDomains(IConfigurationSession session, AcceptedDomain oldDomain, AcceptedDomain newDomain)
			{
				List<QueryFilter> filters = new List<QueryFilter>();
				filters.AddRange(AcceptedDomain.ConflictingDomainFilters(oldDomain, false));
				filters.AddRange(AcceptedDomain.ConflictingDomainFilters(newDomain, false));
				QueryFilter filter = new OrFilter(filters.ToArray());
				ADPagedReader<AcceptedDomain> results = session.FindPaged<AcceptedDomain>(null, QueryScope.SubTree, filter, null, 0);
				foreach (AcceptedDomain result in results)
				{
					yield return result;
				}
				yield return newDomain;
				yield break;
			}

			private static readonly bool isMultiTenancyEnabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;

			private AcceptedDomain newDomain;

			private AcceptedDomain oldDomain;

			private IConfigurationSession session;

			private Task.TaskErrorLoggingDelegate errorWriter;
		}
	}
}
