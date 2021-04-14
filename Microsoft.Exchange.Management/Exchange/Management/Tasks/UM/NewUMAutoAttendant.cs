using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("New", "UMAutoAttendant", SupportsShouldProcess = true)]
	public class NewUMAutoAttendant : NewMultitenancySystemConfigurationObjectTask<UMAutoAttendant>
	{
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> PilotIdentifierList
		{
			get
			{
				return this.DataObject.PilotIdentifierList;
			}
			set
			{
				this.DataObject.PilotIdentifierList = value;
			}
		}

		[Parameter(Mandatory = true)]
		public UMDialPlanIdParameter UMDialPlan
		{
			get
			{
				return (UMDialPlanIdParameter)base.Fields["UMDialPlan"];
			}
			set
			{
				base.Fields["UMDialPlan"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SharedUMDialPlan
		{
			get
			{
				return (SwitchParameter)(base.Fields["SharedUMDialPlan"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SharedUMDialPlan"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public StatusEnum Status
		{
			get
			{
				return (StatusEnum)base.Fields["Status"];
			}
			set
			{
				base.Fields["Status"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SpeechEnabled
		{
			get
			{
				return (bool)base.Fields["SpeechEnabled"];
			}
			set
			{
				base.Fields["SpeechEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMAutoAttendantIdParameter DTMFFallbackAutoAttendant
		{
			get
			{
				return (UMAutoAttendantIdParameter)base.Fields["DTMFFallbackAutoAttendant"];
			}
			set
			{
				base.Fields["DTMFFallbackAutoAttendant"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewUMAutoAttendant(base.Name.ToString(), base.FormatMultiValuedProperty(this.PilotIdentifierList), this.UMDialPlan.ToString());
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || ValidationHelper.IsKnownException(exception);
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			UMAutoAttendant umautoAttendant = (UMAutoAttendant)base.PrepareDataObject();
			umautoAttendant.SetId((IConfigurationSession)base.DataSession, base.Name);
			UMDialPlanIdParameter umdialPlan = this.UMDialPlan;
			UMDialPlan umdialPlan2 = (UMDialPlan)base.GetDataObject<UMDialPlan>(umdialPlan, this.GetDialPlanSession(), null, new LocalizedString?(Strings.NonExistantDialPlan(umdialPlan.ToString())), new LocalizedString?(Strings.MultipleDialplansWithSameId(umdialPlan.ToString())));
			umautoAttendant.SetDialPlan(umdialPlan2.Id);
			this.dialPlan = umdialPlan2;
			if (base.HasErrors)
			{
				return null;
			}
			if (base.Fields.IsModified("DTMFFallbackAutoAttendant"))
			{
				UMAutoAttendantIdParameter dtmffallbackAutoAttendant = this.DTMFFallbackAutoAttendant;
				if (dtmffallbackAutoAttendant != null)
				{
					this.fallbackAA = (UMAutoAttendant)base.GetDataObject<UMAutoAttendant>(dtmffallbackAutoAttendant, base.DataSession, null, new LocalizedString?(Strings.NonExistantAutoAttendant(dtmffallbackAutoAttendant.ToString())), new LocalizedString?(Strings.MultipleAutoAttendantsWithSameId(dtmffallbackAutoAttendant.ToString())));
					umautoAttendant.DTMFFallbackAutoAttendant = this.fallbackAA.Id;
				}
				else
				{
					umautoAttendant.DTMFFallbackAutoAttendant = null;
				}
			}
			TaskLogger.LogExit();
			return umautoAttendant;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.Name.Length > 64)
			{
				base.WriteError(new InvalidAutoAttendantException(Strings.AANameTooLong), ErrorCategory.NotSpecified, null);
			}
			LocalizedException ex = ValidationHelper.ValidateDialedNumbers(this.DataObject.PilotIdentifierList, this.dialPlan);
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.NotSpecified, this.DataObject);
			}
			foreach (string text in this.DataObject.PilotIdentifierList)
			{
				UMAutoAttendant umautoAttendant = UMAutoAttendant.FindAutoAttendantByPilotIdentifierAndDialPlan(text, this.DataObject.UMDialPlan);
				if (umautoAttendant != null)
				{
					base.WriteError(new AutoAttendantExistsException(text, this.DataObject.UMDialPlan.Name), ErrorCategory.NotSpecified, null);
				}
			}
			if (this.dialPlan.URIType == UMUriType.SipName && this.DataObject.PilotIdentifierList != null)
			{
				Utility.CheckForPilotIdentifierDuplicates(this.DataObject, this.ConfigurationSession, this.DataObject.PilotIdentifierList, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.DataObject.DTMFFallbackAutoAttendant != null)
			{
				ValidationHelper.ValidateDtmfFallbackAA(this.DataObject, this.dialPlan, this.fallbackAA);
			}
			StatusEnum status = StatusEnum.Disabled;
			if (base.Fields["Status"] != null)
			{
				status = (StatusEnum)base.Fields["Status"];
			}
			this.DataObject.SetStatus(status);
			if (base.Fields["SpeechEnabled"] != null && this.SpeechEnabled)
			{
				this.DataObject.SpeechEnabled = true;
			}
			else
			{
				this.DataObject.SpeechEnabled = false;
			}
			this.DataObject.NameLookupEnabled = true;
			this.DataObject.OperatorExtension = null;
			this.DataObject.InfoAnnouncementEnabled = InfoAnnouncementEnabledEnum.False;
			this.DataObject.InfoAnnouncementFilename = null;
			this.DataObject.CallSomeoneEnabled = true;
			this.DataObject.SendVoiceMsgEnabled = false;
			if (this.dialPlan.SubscriberType == UMSubscriberType.Consumer)
			{
				this.DataObject.ContactScope = DialScopeEnum.GlobalAddressList;
				this.DataObject.AllowDialPlanSubscribers = false;
				this.DataObject.AllowExtensions = false;
			}
			else
			{
				this.DataObject.ContactScope = DialScopeEnum.DialPlan;
				this.DataObject.AllowDialPlanSubscribers = true;
				this.DataObject.AllowExtensions = true;
			}
			this.DataObject.BusinessHoursWelcomeGreetingEnabled = false;
			this.DataObject.BusinessHoursWelcomeGreetingFilename = null;
			this.DataObject.BusinessHoursMainMenuCustomPromptEnabled = false;
			this.DataObject.BusinessHoursMainMenuCustomPromptFilename = null;
			this.DataObject.BusinessHoursTransferToOperatorEnabled = false;
			this.DataObject.AfterHoursWelcomeGreetingEnabled = false;
			this.DataObject.AfterHoursWelcomeGreetingFilename = null;
			this.DataObject.AfterHoursMainMenuCustomPromptEnabled = false;
			this.DataObject.AfterHoursMainMenuCustomPromptFilename = null;
			this.DataObject.AfterHoursTransferToOperatorEnabled = false;
			this.DataObject.TimeZone = ExTimeZone.CurrentTimeZone.Id;
			this.DataObject.Language = UMLanguage.DefaultLanguage;
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.CreateParentContainerIfNeeded(this.DataObject);
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_AutoAttendantCreated, null, new object[]
				{
					this.DataObject.Id.DistinguishedName,
					this.DataObject.UMDialPlan
				});
			}
			TaskLogger.LogExit();
		}

		private IConfigurationSession GetDialPlanSession()
		{
			IConfigurationSession result = (IConfigurationSession)base.DataSession;
			if (this.SharedUMDialPlan)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				result = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 369, "GetDialPlanSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\new_umautoattendant.cs");
			}
			return result;
		}

		private const string ParameterSharedUMDialPlan = "SharedUMDialPlan";

		private UMDialPlan dialPlan;

		private UMAutoAttendant fallbackAA;
	}
}
