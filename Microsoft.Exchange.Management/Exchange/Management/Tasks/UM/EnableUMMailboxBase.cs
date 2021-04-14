using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	public class EnableUMMailboxBase<TIdentity> : UMMailboxTask<TIdentity> where TIdentity : RecipientIdParameter, new()
	{
		public EnableUMMailboxBase()
		{
			this.PinExpired = true;
			this.ValidateOnly = false;
		}

		[Parameter]
		public MultiValuedProperty<string> Extensions
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["Extensions"];
			}
			set
			{
				base.Fields["Extensions"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public MailboxPolicyIdParameter UMMailboxPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields["UMMailboxPolicy"];
			}
			set
			{
				base.Fields["UMMailboxPolicy"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SIPResourceIdentifier
		{
			get
			{
				return (string)base.Fields["SIPResourceIdentifier"];
			}
			set
			{
				base.Fields["SIPResourceIdentifier"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Pin
		{
			get
			{
				return (string)base.Fields["Pin"];
			}
			set
			{
				base.Fields["Pin"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PinExpired
		{
			get
			{
				return (bool)(base.Fields["PinExpired"] ?? false);
			}
			set
			{
				base.Fields["PinExpired"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string NotifyEmail
		{
			get
			{
				return (string)base.Fields["NotifyEmail"];
			}
			set
			{
				base.Fields["NotifyEmail"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PilotNumber
		{
			get
			{
				return (string)base.Fields["PilotNumber"];
			}
			set
			{
				base.Fields["PilotNumber"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutomaticSpeechRecognitionEnabled
		{
			get
			{
				return (bool)(base.Fields["ASREnabled"] ?? false);
			}
			set
			{
				base.Fields["ASREnabled"] = value;
			}
		}

		[Parameter]
		public SwitchParameter ValidateOnly
		{
			get
			{
				return (SwitchParameter)base.Fields["ValidateOnly"];
			}
			set
			{
				base.Fields["ValidateOnly"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				TIdentity identity = this.Identity;
				return Strings.ConfirmationMessageEnableUMMailbox(identity.ToString(), this.UMMailboxPolicy.ToString());
			}
		}

		protected virtual bool ShouldSavePin
		{
			get
			{
				return true;
			}
		}

		protected virtual bool ShouldSubmitWelcomeMessage
		{
			get
			{
				return true;
			}
		}

		protected virtual bool ShouldInitUMMailbox
		{
			get
			{
				return true;
			}
		}

		protected UMDialPlan DialPlan
		{
			get
			{
				return this.dialPlan;
			}
		}

		private bool IsUserAllowedForUnifiedMessaging(ADUser user)
		{
			bool result = true;
			if (user != null && user.Database != null)
			{
				DatabaseLocationInfo serverForDatabase = ActiveManager.GetCachingActiveManagerInstance().GetServerForDatabase(user.Database.ObjectGuid);
				if (serverForDatabase != null && serverForDatabase.AdminDisplayVersion.Major > (int)ExchangeObjectVersion.Exchange2010.ExchangeBuild.Major && (serverForDatabase.AdminDisplayVersion.Major != 15 || serverForDatabase.AdminDisplayVersion.Minor > 1))
				{
					result = false;
				}
			}
			return result;
		}

		protected override void DoValidate()
		{
			LocalizedException ex = null;
			if (this.ShouldInitUMMailbox && UMSubscriber.IsValidSubscriber(this.DataObject))
			{
				ex = new UserAlreadyUmEnabledException(this.DataObject.Id.Name);
			}
			else if (!this.IsUserAllowedForUnifiedMessaging(this.DataObject))
			{
				ex = new UserNotAllowedForUMEnabledException();
			}
			else
			{
				MailboxPolicyIdParameter ummailboxPolicy = this.UMMailboxPolicy;
				this.mailboxPolicy = (UMMailboxPolicy)base.GetDataObject<UMMailboxPolicy>(ummailboxPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.UMMailboxPolicyIdNotFound(ummailboxPolicy.ToString())), new LocalizedString?(Strings.MultipleUMMailboxPolicyWithSameId(ummailboxPolicy.ToString())));
				this.dialPlan = this.mailboxPolicy.GetDialPlan();
				if (this.DialPlan.SubscriberType != UMSubscriberType.Consumer || this.DialPlan.URIType != UMUriType.E164 || !string.IsNullOrEmpty(this.SIPResourceIdentifier) || (this.Extensions != null && this.Extensions.Count != 0))
				{
					if (this.Extensions == null || this.Extensions.Count == 0)
					{
						string text = null;
						PhoneNumber phoneNumber;
						if (PhoneNumber.TryParse(this.DataObject.Phone, out phoneNumber))
						{
							text = this.DialPlan.GetDefaultExtension(phoneNumber.Number);
						}
						if (!string.IsNullOrEmpty(Utils.TrimSpaces(text)))
						{
							this.Extensions = new MultiValuedProperty<string>(text);
						}
					}
					if (this.DialPlan.URIType == UMUriType.SipName)
					{
						ProxyAddress proxyAddress = this.DataObject.EmailAddresses.Find((ProxyAddress p) => string.Equals(p.PrefixString, "sip", StringComparison.OrdinalIgnoreCase));
						string text2 = (proxyAddress != null) ? proxyAddress.AddressString : null;
						if (string.IsNullOrEmpty(this.SIPResourceIdentifier))
						{
							this.SIPResourceIdentifier = ((text2 != null) ? text2 : null);
						}
						else if (text2 != null && !string.Equals(this.SIPResourceIdentifier, text2, StringComparison.OrdinalIgnoreCase))
						{
							ex = new SIPResouceIdConflictWithExistingValue(this.SIPResourceIdentifier, text2);
							base.WriteError(ex, ErrorCategory.InvalidArgument, null);
						}
					}
					IRecipientSession tenantLocalRecipientSession = RecipientTaskHelper.GetTenantLocalRecipientSession(this.DataObject.OrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId);
					LocalizedException ex2 = null;
					TelephoneNumberProcessStatus telephoneNumberProcessStatus;
					Utils.ValidateExtensionsAndSipResourceIdentifier(tenantLocalRecipientSession, this.ConfigurationSession, CommonConstants.DataCenterADPresent, this.DataObject, this.DialPlan, (this.Extensions != null) ? this.Extensions.ToArray() : null, null, this.SIPResourceIdentifier, out ex2, out telephoneNumberProcessStatus);
					if (ex2 != null)
					{
						this.DataObject.EmailAddresses.Clear();
						ex = ex2;
					}
				}
			}
			if (ex == null || this.ValidateOnly)
			{
				this.DataObject.UMEnabledFlags |= UMEnabledFlags.UMEnabled;
				if (base.Fields.IsModified("ASREnabled"))
				{
					bool flag = (bool)base.Fields["ASREnabled"];
					if (flag)
					{
						this.DataObject.UMEnabledFlags |= UMEnabledFlags.ASREnabled;
					}
					else
					{
						this.DataObject.UMEnabledFlags = (this.DataObject.UMEnabledFlags & ~UMEnabledFlags.ASREnabled);
					}
				}
				this.DataObject.PopulateDtmfMap(true);
				if (!this.ValidateOnly)
				{
					Utils.UMPopulate(this.DataObject, this.SIPResourceIdentifier, this.Extensions, this.mailboxPolicy, this.DialPlan);
					if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
					{
						RecipientTaskHelper.ValidateSmtpAddress(this.ConfigurationSession, this.DataObject.EmailAddresses, this.DataObject, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
					}
					if (this.ShouldSavePin)
					{
						base.PinInfo = base.ValidateOrGeneratePIN(this.Pin, this.mailboxPolicy.Guid);
						base.PinInfo.PinExpired = this.PinExpired;
						base.PinInfo.LockedOut = false;
					}
				}
				else if (this.Extensions != null)
				{
					this.DataObject.AddEUMProxyAddress(this.Extensions, this.DialPlan);
				}
			}
			if (ex == null)
			{
				if (this.ValidateOnly)
				{
					this.WriteResult();
				}
				return;
			}
			if (this.ValidateOnly)
			{
				this.WriteResult();
				base.WriteError(ex, ErrorCategory.InvalidArgument, null, false, null);
				return;
			}
			base.WriteError(ex, ErrorCategory.InvalidArgument, null);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.ValidateOnly)
			{
				return;
			}
			if (this.ShouldSavePin)
			{
				base.SavePIN(this.mailboxPolicy.Guid);
			}
			if (this.ShouldInitUMMailbox)
			{
				base.InitUMMailbox();
			}
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				if (this.ShouldSubmitWelcomeMessage)
				{
					base.SubmitWelcomeMessage(this.NotifyEmail, this.PilotNumber, this.Extensions, this.DialPlan);
				}
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMUserEnabled, null, new object[]
				{
					this.DataObject.Id.Name
				});
				this.WriteResult();
			}
			TaskLogger.LogExit();
		}

		private void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Id
			});
			UMMailbox ummailbox = new UMMailbox(this.DataObject);
			if (this.ValidateOnly)
			{
				ummailbox.SIPResourceIdentifier = this.SIPResourceIdentifier;
			}
			base.WriteObject(ummailbox);
			TaskLogger.LogExit();
		}

		private UMDialPlan dialPlan;

		private UMMailboxPolicy mailboxPolicy;
	}
}
