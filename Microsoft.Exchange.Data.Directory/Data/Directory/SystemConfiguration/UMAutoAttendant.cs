using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class UMAutoAttendant : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return UMAutoAttendant.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return UMAutoAttendant.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return UMAutoAttendant.parentPath;
			}
		}

		private AutoAttendantSettings BusinessHourSettings
		{
			get
			{
				if (string.IsNullOrEmpty(this.BusinessHourFeatures))
				{
					this.businessHourSettings = new AutoAttendantSettings();
				}
				else
				{
					this.businessHourSettings = AutoAttendantSettings.FromXml(this.BusinessHourFeatures);
				}
				this.businessHourSettings.Parent = this;
				return this.businessHourSettings;
			}
		}

		private AutoAttendantSettings AfterHourSettings
		{
			get
			{
				if (string.IsNullOrEmpty(this.AfterHourFeatures))
				{
					this.afterHourSettings = new AutoAttendantSettings();
				}
				else
				{
					this.afterHourSettings = AutoAttendantSettings.FromXml(this.AfterHourFeatures);
				}
				this.afterHourSettings.Parent = this;
				return this.afterHourSettings;
			}
		}

		private void FlushBusinessHourSettings()
		{
			if (this.businessHourSettings != null)
			{
				this.BusinessHourFeatures = AutoAttendantSettings.ToXml(this.businessHourSettings);
			}
		}

		private void FlushAfterHourSettings()
		{
			if (this.afterHourSettings != null)
			{
				this.AfterHourFeatures = AutoAttendantSettings.ToXml(this.afterHourSettings);
			}
		}

		internal void SetStatus(StatusEnum status)
		{
			this.Enabled = (status == StatusEnum.Enabled);
		}

		internal void SetDialPlan(ADObjectId dialPlanId)
		{
			this[UMAutoAttendantSchema.UMDialPlan] = dialPlanId;
		}

		internal string DefaultMailboxLegacyDN
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.DefaultMailboxLegacyDN];
			}
			set
			{
				this[UMAutoAttendantSchema.DefaultMailboxLegacyDN] = value;
			}
		}

		private string AfterHourFeatures
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.AfterHourFeatures];
			}
			set
			{
				this[UMAutoAttendantSchema.AfterHourFeatures] = value;
			}
		}

		private string BusinessHourFeatures
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.BusinessHourFeatures];
			}
			set
			{
				this[UMAutoAttendantSchema.BusinessHourFeatures] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SpeechEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.AutomaticSpeechRecognitionEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.AutomaticSpeechRecognitionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowDialPlanSubscribers
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.AllowDialPlanSubscribers];
			}
			set
			{
				this[UMAutoAttendantSchema.AllowDialPlanSubscribers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowExtensions
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.AllowExtensions];
			}
			set
			{
				this[UMAutoAttendantSchema.AllowExtensions] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AllowedInCountryOrRegionGroups
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMAutoAttendantSchema.AllowedInCountryOrRegionGroups];
			}
			set
			{
				this[UMAutoAttendantSchema.AllowedInCountryOrRegionGroups] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AllowedInternationalGroups
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMAutoAttendantSchema.AllowedInternationalGroups];
			}
			set
			{
				this[UMAutoAttendantSchema.AllowedInternationalGroups] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CallSomeoneEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.CallSomeoneEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.CallSomeoneEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DialScopeEnum ContactScope
		{
			get
			{
				return (DialScopeEnum)this[UMAutoAttendantSchema.ContactScope];
			}
			set
			{
				this[UMAutoAttendantSchema.ContactScope] = value;
			}
		}

		public ADObjectId ContactAddressList
		{
			get
			{
				return (ADObjectId)this[UMAutoAttendantSchema.ContactAddressList];
			}
			set
			{
				this[UMAutoAttendantSchema.ContactAddressList] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SendVoiceMsgEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.SendVoiceMsgEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.SendVoiceMsgEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ScheduleInterval[] BusinessHoursSchedule
		{
			get
			{
				return (ScheduleInterval[])this[UMAutoAttendantSchema.BusinessHoursSchedule];
			}
			set
			{
				this[UMAutoAttendantSchema.BusinessHoursSchedule] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> PilotIdentifierList
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMAutoAttendantSchema.PilotIdentifierList];
			}
			set
			{
				this[UMAutoAttendantSchema.PilotIdentifierList] = value;
			}
		}

		public ADObjectId UMDialPlan
		{
			get
			{
				return (ADObjectId)this[UMAutoAttendantSchema.UMDialPlan];
			}
		}

		public ADObjectId DTMFFallbackAutoAttendant
		{
			get
			{
				return (ADObjectId)this[UMAutoAttendantSchema.DTMFFallbackAutoAttendant];
			}
			set
			{
				this[UMAutoAttendantSchema.DTMFFallbackAutoAttendant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<HolidaySchedule> HolidaySchedule
		{
			get
			{
				return (MultiValuedProperty<HolidaySchedule>)this[UMAutoAttendantSchema.HolidaySchedule];
			}
			set
			{
				this[UMAutoAttendantSchema.HolidaySchedule] = value;
			}
		}

		public string TimeZone
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.TimeZone];
			}
			set
			{
				this[UMAutoAttendantSchema.TimeZone] = value;
			}
		}

		public UMTimeZone TimeZoneName
		{
			get
			{
				return (UMTimeZone)this[UMAutoAttendantSchema.TimeZoneName];
			}
			set
			{
				this[UMAutoAttendantSchema.TimeZoneName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AutoAttendantDisambiguationFieldEnum MatchedNameSelectionMethod
		{
			get
			{
				return (AutoAttendantDisambiguationFieldEnum)this[UMAutoAttendantSchema.MatchedNameSelectionMethod];
			}
			set
			{
				this[UMAutoAttendantSchema.MatchedNameSelectionMethod] = value;
			}
		}

		internal Guid PromptChangeKey
		{
			get
			{
				return (Guid)this[UMAutoAttendantSchema.PromptChangeKey];
			}
			set
			{
				this[UMAutoAttendantSchema.PromptChangeKey] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BusinessLocation
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.BusinessLocation];
			}
			set
			{
				this[UMAutoAttendantSchema.BusinessLocation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DayOfWeek WeekStartDay
		{
			get
			{
				return (DayOfWeek)this[UMAutoAttendantSchema.WeekStartDay];
			}
			set
			{
				this[UMAutoAttendantSchema.WeekStartDay] = value;
			}
		}

		private bool Enabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.Enabled];
			}
			set
			{
				this[UMAutoAttendantSchema.Enabled] = value;
			}
		}

		public StatusEnum Status
		{
			get
			{
				if (!this.Enabled)
				{
					return StatusEnum.Disabled;
				}
				return StatusEnum.Enabled;
			}
		}

		[Parameter(Mandatory = false)]
		public UMLanguage Language
		{
			get
			{
				return (UMLanguage)this[UMAutoAttendantSchema.Language];
			}
			set
			{
				this[UMAutoAttendantSchema.Language] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OperatorExtension
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.OperatorExtension];
			}
			set
			{
				this[UMAutoAttendantSchema.OperatorExtension] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string InfoAnnouncementFilename
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.InfoAnnouncementFilename];
			}
			set
			{
				this[UMAutoAttendantSchema.InfoAnnouncementFilename] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public InfoAnnouncementEnabledEnum InfoAnnouncementEnabled
		{
			get
			{
				return (InfoAnnouncementEnabledEnum)this[UMAutoAttendantSchema.InfoAnnouncementEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.InfoAnnouncementEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool NameLookupEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.NameLookupEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.NameLookupEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool StarOutToDialPlanEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.StarOutToDialPlanEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.StarOutToDialPlanEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ForwardCallsToDefaultMailbox
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.ForwardCallsToDefaultMailbox];
			}
			set
			{
				this[UMAutoAttendantSchema.ForwardCallsToDefaultMailbox] = value;
			}
		}

		public ADUser DefaultMailbox
		{
			get
			{
				return this.defaultMailbox;
			}
			internal set
			{
				this.defaultMailbox = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BusinessName
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.BusinessName];
			}
			set
			{
				this[UMAutoAttendantSchema.BusinessName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BusinessHoursWelcomeGreetingFilename
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.BusinessHoursWelcomeGreetingFilename];
			}
			set
			{
				this[UMAutoAttendantSchema.BusinessHoursWelcomeGreetingFilename] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BusinessHoursWelcomeGreetingEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.BusinessHoursWelcomeGreetingEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.BusinessHoursWelcomeGreetingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BusinessHoursMainMenuCustomPromptFilename
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.BusinessHoursMainMenuCustomPromptFilename];
			}
			set
			{
				this[UMAutoAttendantSchema.BusinessHoursMainMenuCustomPromptFilename] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BusinessHoursMainMenuCustomPromptEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.BusinessHoursMainMenuCustomPromptEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.BusinessHoursMainMenuCustomPromptEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BusinessHoursTransferToOperatorEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.BusinessHoursTransferToOperatorEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.BusinessHoursTransferToOperatorEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<CustomMenuKeyMapping> BusinessHoursKeyMapping
		{
			get
			{
				return (MultiValuedProperty<CustomMenuKeyMapping>)this[UMAutoAttendantSchema.BusinessHoursKeyMapping];
			}
			set
			{
				this[UMAutoAttendantSchema.BusinessHoursKeyMapping] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BusinessHoursKeyMappingEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.BusinessHoursKeyMappingEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.BusinessHoursKeyMappingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AfterHoursWelcomeGreetingFilename
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.AfterHoursWelcomeGreetingFilename];
			}
			set
			{
				this[UMAutoAttendantSchema.AfterHoursWelcomeGreetingFilename] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AfterHoursWelcomeGreetingEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.AfterHoursWelcomeGreetingEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.AfterHoursWelcomeGreetingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AfterHoursMainMenuCustomPromptFilename
		{
			get
			{
				return (string)this[UMAutoAttendantSchema.AfterHoursMainMenuCustomPromptFilename];
			}
			set
			{
				this[UMAutoAttendantSchema.AfterHoursMainMenuCustomPromptFilename] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AfterHoursMainMenuCustomPromptEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.AfterHoursMainMenuCustomPromptEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.AfterHoursMainMenuCustomPromptEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AfterHoursTransferToOperatorEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.AfterHoursTransferToOperatorEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.AfterHoursTransferToOperatorEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<CustomMenuKeyMapping> AfterHoursKeyMapping
		{
			get
			{
				return (MultiValuedProperty<CustomMenuKeyMapping>)this[UMAutoAttendantSchema.AfterHoursKeyMapping];
			}
			set
			{
				this[UMAutoAttendantSchema.AfterHoursKeyMapping] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AfterHoursKeyMappingEnabled
		{
			get
			{
				return (bool)this[UMAutoAttendantSchema.AfterHoursKeyMappingEnabled];
			}
			set
			{
				this[UMAutoAttendantSchema.AfterHoursKeyMappingEnabled] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		internal UMDialPlan GetDialPlan()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(this.UMDialPlan);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 2111, "GetDialPlan", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\umautoattendantconfig.cs");
			return tenantOrTopologyConfigurationSession.Read<UMDialPlan>(this.UMDialPlan);
		}

		internal static UMAutoAttendant FindAutoAttendantByPilotIdentifierAndDialPlan(string pilotIdentifier, ADObjectId dialPlanId)
		{
			if (pilotIdentifier == null)
			{
				throw new ArgumentNullException("pilotIdentifier");
			}
			if (dialPlanId == null)
			{
				throw new ArgumentNullException("dialPlanId");
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(dialPlanId);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 2144, "FindAutoAttendantByPilotIdentifierAndDialPlan", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\umautoattendantconfig.cs");
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, UMAutoAttendantSchema.PilotIdentifierList, pilotIdentifier),
				new ComparisonFilter(ComparisonOperator.Equal, UMAutoAttendantSchema.UMDialPlan, dialPlanId)
			});
			UMAutoAttendant[] array = tenantOrTopologyConfigurationSession.Find<UMAutoAttendant>(null, QueryScope.SubTree, filter, null, 0);
			switch (array.Length)
			{
			case 0:
				return null;
			case 1:
				return array[0];
			default:
				throw new NonUniquePilotIdentifierException(pilotIdentifier, dialPlanId.ToString());
			}
		}

		internal AutoAttendantSettings GetCurrentSettings(out HolidaySchedule holidaySettings, ref bool isBusinessHour)
		{
			ExTimeZone exTimeZone = null;
			string timeZone = this.TimeZone;
			if (string.IsNullOrEmpty(timeZone))
			{
				ExTraceGlobals.UMAutoAttendantTracer.TraceDebug<string>((long)this.GetHashCode(), "AA [Name = \"{0}\"] TZ Id = empty string, defaulting to using Current machine timezone", base.Name);
				exTimeZone = ExTimeZone.CurrentTimeZone;
			}
			else if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(this.TimeZone, out exTimeZone))
			{
				throw new InvalidOperationException(DirectoryStrings.InvalidTimeZoneId(this.TimeZone));
			}
			ExDateTime utcNow = ExDateTime.UtcNow;
			ExDateTime exDateTime = exTimeZone.ConvertDateTime(utcNow);
			ExTraceGlobals.UMAutoAttendantTracer.TraceDebug((long)this.GetHashCode(), "AA [Name = \"{0}\"] UTC = {1} LOCAL = {2} TZ Id = {3}", new object[]
			{
				base.Name,
				utcNow.ToString("R"),
				exDateTime.ToString("R"),
				this.TimeZone
			});
			AutoAttendantSettings autoAttendantSettings = null;
			HolidaySchedule holidaySchedule = null;
			MultiValuedProperty<HolidaySchedule> holidaySchedule2 = this.HolidaySchedule;
			if (holidaySchedule2 != null && holidaySchedule2.Count > 0)
			{
				using (MultiValuedProperty<HolidaySchedule>.Enumerator enumerator = holidaySchedule2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						HolidaySchedule holidaySchedule3 = enumerator.Current;
						if ((ExDateTime)holidaySchedule3.StartDate.Date <= exDateTime.Date && (ExDateTime)holidaySchedule3.EndDate.Date >= exDateTime.Date)
						{
							ExTraceGlobals.UMAutoAttendantTracer.TraceDebug((long)this.GetHashCode(), "AA: {0} Call Time: {1} Matched with holiday: {2} {3}-{4}", new object[]
							{
								base.Name,
								exDateTime.ToString("R"),
								holidaySchedule3.Name,
								holidaySchedule3.StartDate.ToString("R"),
								holidaySchedule3.EndDate.ToString("R")
							});
							autoAttendantSettings = this.AfterHourSettings;
							isBusinessHour = false;
							if (holidaySchedule == null)
							{
								holidaySchedule = holidaySchedule3;
							}
							else if (holidaySchedule3.StartDate.Date > holidaySchedule.StartDate.Date)
							{
								holidaySchedule = holidaySchedule3;
							}
							else if (holidaySchedule3.StartDate.Date == holidaySchedule.StartDate.Date)
							{
								int num = string.Compare(holidaySchedule.Name, holidaySchedule3.Name, StringComparison.OrdinalIgnoreCase);
								holidaySchedule = ((num > 0) ? holidaySchedule3 : holidaySchedule);
							}
						}
					}
					goto IL_290;
				}
			}
			ExTraceGlobals.UMAutoAttendantTracer.TraceDebug<string, string>((long)this.GetHashCode(), "AA: {0} Call Time: {1} No holiday schedule found", base.Name, exDateTime.ToString("R"));
			IL_290:
			holidaySettings = holidaySchedule;
			if (autoAttendantSettings != null)
			{
				return autoAttendantSettings;
			}
			autoAttendantSettings = this.AfterHourSettings;
			isBusinessHour = false;
			foreach (ScheduleInterval scheduleInterval in this.BusinessHoursSchedule)
			{
				ExTraceGlobals.UMAutoAttendantTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "AA: {0} Call Time: {1} BusinessHour: {2}", base.Name, exDateTime.ToString("R"), scheduleInterval.ToString());
				if (scheduleInterval.Contains(new WeekDayAndTime((DateTime)exDateTime)))
				{
					ExTraceGlobals.UMAutoAttendantTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "AA: {0} Call Time: {1} Matched with BusinessHour: {2}", base.Name, exDateTime.ToString("R"), scheduleInterval.ToString());
					isBusinessHour = true;
					autoAttendantSettings = this.BusinessHourSettings;
					break;
				}
			}
			if (!isBusinessHour)
			{
				ExTraceGlobals.UMAutoAttendantTracer.TraceDebug<string, string>((long)this.GetHashCode(), "AA: {0} Call Time: {1} Returning AfterHour settings", base.Name, exDateTime.ToString("R"));
			}
			return autoAttendantSettings;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (base.IsModified(ADObjectSchema.Name) && base.ObjectState != ObjectState.New)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.AACantChangeName, base.Id, string.Empty));
			}
			LocalizedString localizedString = this.ValidateSettings(this.BusinessHourSettings, AASettingsEnum.BusinessHourSettings, this.OperatorExtension);
			if (localizedString != LocalizedString.Empty)
			{
				errors.Add(new PropertyValidationError(localizedString, UMAutoAttendantSchema.BusinessHoursKeyMapping, this));
			}
			localizedString = this.ValidateSettings(this.AfterHourSettings, AASettingsEnum.AfterHourSettings, this.OperatorExtension);
			if (localizedString != LocalizedString.Empty)
			{
				errors.Add(new PropertyValidationError(localizedString, UMAutoAttendantSchema.AfterHoursKeyMapping, this));
			}
			if (this.InfoAnnouncementEnabled != InfoAnnouncementEnabledEnum.False && string.IsNullOrEmpty(this.InfoAnnouncementFilename))
			{
				localizedString = DirectoryStrings.InvalidAutoAttendantSetting("InfoAnnouncementEnabled", "InfoAnnouncementFilename");
				errors.Add(new ObjectValidationError(localizedString, base.Id, string.Empty));
			}
			if (!string.IsNullOrEmpty(this.InfoAnnouncementFilename) && !this.VerifyValidCustomGreetingFile(this.InfoAnnouncementFilename))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.InvalidCustomGreetingFilename("InfoAnnouncementFilename"), base.Id, string.Empty));
			}
			if (this.HolidaySchedule.Count > 0)
			{
				HolidaySchedule[] array = this.HolidaySchedule.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					HolidaySchedule holidaySchedule = array[i];
					if (!string.IsNullOrEmpty(holidaySchedule.Greeting) && !this.VerifyValidCustomGreetingFile(holidaySchedule.Greeting))
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.InvalidCustomGreetingFilename("HolidaySchedule"), base.Id, string.Empty));
					}
					for (int j = i + 1; j < array.Length; j++)
					{
						if (i != j)
						{
							HolidaySchedule holidaySchedule2 = array[j];
							if (string.Compare(holidaySchedule.Name, holidaySchedule2.Name, StringComparison.OrdinalIgnoreCase) == 0)
							{
								errors.Add(new ObjectValidationError(DirectoryStrings.DuplicateHolidaysError(holidaySchedule.Name), base.Id, string.Empty));
								break;
							}
						}
					}
				}
			}
			if ((this.CallSomeoneEnabled || this.SendVoiceMsgEnabled) && this.ContactScope == DialScopeEnum.AddressList && this.ContactAddressList == null)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.InvalidCallSomeoneScopeSettings("AddressList", "ContactAddressList"), base.Id, string.Empty));
			}
			if (base.IsModified(UMAutoAttendantSchema.DefaultMailboxLegacyDN))
			{
				ADUser aduser = this.DefaultMailbox;
				if (aduser != null && (!aduser.UMEnabled || aduser.UMRecipientDialPlanId == null || !aduser.UMRecipientDialPlanId.Equals(this.UMDialPlan) || string.IsNullOrEmpty(this.DefaultMailboxLegacyDN)))
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.InvalidDefaultMailbox, base.Id, string.Empty));
				}
			}
		}

		private bool VerifyValidCustomGreetingFile(string file)
		{
			if (string.IsNullOrEmpty(file))
			{
				return true;
			}
			string extension = Path.GetExtension(file);
			return string.Compare(extension, ".wav", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(extension, ".wma", StringComparison.OrdinalIgnoreCase) == 0;
		}

		private LocalizedString ValidateSettings(AutoAttendantSettings aa, AASettingsEnum settings, string operatorExtension)
		{
			string text = (settings == AASettingsEnum.BusinessHourSettings) ? "BusinessHours" : "AfterHours";
			if (aa.WelcomeGreetingEnabled && string.IsNullOrEmpty(aa.WelcomeGreetingFilename))
			{
				return DirectoryStrings.InvalidAutoAttendantSetting(text + "WelcomeGreetingEnabled", text + "WelcomeGreetingFilename");
			}
			if (!string.IsNullOrEmpty(aa.WelcomeGreetingFilename) && !this.VerifyValidCustomGreetingFile(aa.WelcomeGreetingFilename))
			{
				return DirectoryStrings.InvalidCustomGreetingFilename(text + "WelcomeGreetingFilename");
			}
			if (aa.MainMenuCustomPromptEnabled && string.IsNullOrEmpty(aa.MainMenuCustomPromptFilename))
			{
				return DirectoryStrings.InvalidAutoAttendantSetting(text + "MainMenuCustomPromptEnabled", text + "MainMenuCustomPromptFilename");
			}
			if (!string.IsNullOrEmpty(aa.MainMenuCustomPromptFilename) && !this.VerifyValidCustomGreetingFile(aa.MainMenuCustomPromptFilename))
			{
				return DirectoryStrings.InvalidCustomGreetingFilename(text + "MainMenuCustomPromptFilename");
			}
			if (aa.KeyMappingEnabled && (aa.KeyMapping == null || aa.KeyMapping.Length == 0))
			{
				return DirectoryStrings.InvalidAutoAttendantSetting(text + "KeyMappingEnabled", text + "KeyMapping");
			}
			if (aa.KeyMapping != null && aa.KeyMapping.Length > 10)
			{
				return DirectoryStrings.TooManyKeyMappings(text);
			}
			return LocalizedString.Empty;
		}

		private static UMAutoAttendantSchema schema = ObjectSchema.GetInstance<UMAutoAttendantSchema>();

		private static string mostDerivedClass = "msExchUMAutoAttendant";

		private static ADObjectId parentPath = new ADObjectId("CN=UM AutoAttendant Container");

		private AutoAttendantSettings businessHourSettings;

		private AutoAttendantSettings afterHourSettings;

		private ADUser defaultMailbox;
	}
}
