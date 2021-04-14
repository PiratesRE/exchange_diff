using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.PersonalAutoAttendant;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMPhoneSession;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Start", "UMPhoneSession", SupportsShouldProcess = true, DefaultParameterSetName = "DefaultVoicemailGreeting")]
	public sealed class StartUMPhoneSession : NewTenantADTaskBase<UMPhoneSession>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageStartUMPhoneSession;
			}
		}

		[ValidateNotNullOrEmpty]
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

		[Parameter(Mandatory = true, ParameterSetName = "AwayVoicemailGreeting")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "DefaultVoicemailGreeting")]
		public MailboxIdParameter UMMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["UMMailbox"];
			}
			set
			{
				base.Fields["UMMailbox"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "DefaultVoicemailGreeting")]
		[Parameter(Mandatory = true, ParameterSetName = "AwayVoicemailGreeting")]
		[Parameter(Mandatory = true, ParameterSetName = "PlayOnPhoneGreeting")]
		public string PhoneNumber
		{
			get
			{
				return (string)base.Fields["PhoneNumber"];
			}
			set
			{
				base.Fields["PhoneNumber"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DefaultVoicemailGreeting")]
		public SwitchParameter DefaultVoicemailGreeting
		{
			get
			{
				return (SwitchParameter)(base.Fields["DefaultVoicemailGreeting"] ?? false);
			}
			set
			{
				base.Fields["DefaultVoicemailGreeting"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AwayVoicemailGreeting")]
		public SwitchParameter AwayVoicemailGreeting
		{
			get
			{
				return (SwitchParameter)(base.Fields["AwayVoicemailGreeting"] ?? false);
			}
			set
			{
				base.Fields["AwayVoicemailGreeting"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "PlayOnPhoneGreeting")]
		[ValidateNotNullOrEmpty]
		public UMCallAnsweringRuleIdParameter CallAnsweringRuleId
		{
			get
			{
				return (UMCallAnsweringRuleIdParameter)base.Fields["CallAnsweringRuleId"];
			}
			set
			{
				base.Fields["CallAnsweringRuleId"] = value;
			}
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 133, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\StartUMPhoneSession.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ExchangeErrorCategory.Client);
				return adorganizationalUnit.OrganizationId;
			}
			return base.ResolveCurrentOrganization();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.ValidateParameters();
			if (!base.HasErrors)
			{
				this.DataObject.PhoneNumber = this.PhoneNumber;
			}
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (parameterSetName == "AwayVoicemailGreeting")
				{
					return this.CreateProviderObjectForMailbox(TypeOfPlayOnPhoneGreetingCall.AwayGreetingRecording);
				}
				if (parameterSetName == "DefaultVoicemailGreeting")
				{
					return this.CreateProviderObjectForMailbox(TypeOfPlayOnPhoneGreetingCall.VoicemailGreetingRecording);
				}
				if (parameterSetName == "PlayOnPhoneGreeting")
				{
					return this.CreateProviderObjectForPlayOnPhone();
				}
			}
			throw new InvalidOperationException();
		}

		private void ResolveADUser(MailboxIdParameter mailbox)
		{
			this.adUser = (ADUser)base.GetDataObject<ADUser>(mailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorUserNotFound(mailbox.ToString())), new LocalizedString?(Strings.ErrorUserNotUnique(mailbox.ToString())));
			base.VerifyIsWithinScopes(TaskHelper.UnderscopeSessionToOrganization(base.TenantGlobalCatalogSession, this.adUser.OrganizationId, true), this.adUser, true, new DataAccessTask<UMPhoneSession>.ADObjectOutOfScopeString(Strings.ErrorCannotChangeMailboxOutOfWriteScope));
		}

		private UMPlayOnPhoneDataProvider CreateProviderObjectForMailbox(TypeOfPlayOnPhoneGreetingCall callType)
		{
			this.ResolveADUser(this.UMMailbox);
			return new UMPlayOnPhoneDataProvider(this.adUser, callType);
		}

		private UMPlayOnPhoneDataProvider CreateProviderObjectForPlayOnPhone()
		{
			MailboxIdParameter mailboxIdParameter = this.CallAnsweringRuleId.RawMailbox;
			if (mailboxIdParameter == null)
			{
				ADObjectId adObjectId;
				if (!base.TryGetExecutingUserId(out adObjectId))
				{
					base.WriteError(new MailboxMustBeSpecifiedException("CallAnsweringRuleId"), ErrorCategory.InvalidArgument, null);
				}
				mailboxIdParameter = new MailboxIdParameter(adObjectId);
			}
			this.ResolveADUser(mailboxIdParameter);
			return new UMPlayOnPhoneDataProvider(this.adUser, new Guid?(this.CallAnsweringRuleId.RawRuleGuid.Value));
		}

		private void ValidateParameters()
		{
			if (this.CallAnsweringRuleId != null)
			{
				using (UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(this.adUser))
				{
					if (umsubscriber != null)
					{
						using (IPAAStore ipaastore = PAAStore.Create(umsubscriber))
						{
							if (ipaastore.GetAutoAttendant(this.CallAnsweringRuleId.RawRuleGuid.Value, PAAValidationMode.None) == null)
							{
								base.WriteError(new CallAnsweringRuleNotFoundException(this.CallAnsweringRuleId.RawRuleGuid.Value.ToString()), ErrorCategory.InvalidArgument, null);
							}
							goto IL_99;
						}
					}
					base.WriteError(new UserNotUmEnabledException(this.adUser.Id.ToString()), (ErrorCategory)1000, null);
					IL_99:;
				}
			}
		}

		private ADUser adUser;

		internal abstract class ParameterSet
		{
			internal const string DefaultVoicemailGreeting = "DefaultVoicemailGreeting";

			internal const string AwayVoicemailGreeting = "AwayVoicemailGreeting";

			internal const string PlayOnPhoneGreeting = "PlayOnPhoneGreeting";
		}
	}
}
