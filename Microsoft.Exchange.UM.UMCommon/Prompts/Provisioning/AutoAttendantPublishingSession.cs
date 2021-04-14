using System;
using System.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	internal class AutoAttendantPublishingSession : PublishingSessionBase
	{
		internal AutoAttendantPublishingSession(string userName, UMAutoAttendant autoAttendant) : base(userName, autoAttendant)
		{
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(autoAttendant.OrganizationId);
			this.dialPlan = iadsystemConfigurationLookup.GetDialPlanFromId(autoAttendant.UMDialPlan);
		}

		protected override UMDialPlan DialPlan
		{
			get
			{
				return this.dialPlan;
			}
		}

		private UMAutoAttendant AutoAttendant
		{
			get
			{
				return (UMAutoAttendant)base.ConfigurationObject;
			}
		}

		public override void Upload(string source, string destinationName)
		{
			try
			{
				base.Upload(source, destinationName);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_AutoAttendantCustomPromptUploadSucceeded, null, new object[]
				{
					base.UserName,
					this.AutoAttendant.Id,
					destinationName
				});
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PromptProvisioningTracer, this, "AutoAttendantPublishingSession.Upload: {0}.", new object[]
				{
					ex
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_AutoAttendantCustomPromptUploadFailed, null, new object[]
				{
					base.UserName,
					this.AutoAttendant.Id,
					ex.Message
				});
				throw;
			}
		}

		protected override void AddConfiguredFiles(IDictionary fileList)
		{
			PublishingSessionBase.AddIfNotEmpty(fileList, this.AutoAttendant.InfoAnnouncementFilename);
			PublishingSessionBase.AddIfNotEmpty(fileList, this.AutoAttendant.AfterHoursWelcomeGreetingFilename);
			PublishingSessionBase.AddIfNotEmpty(fileList, this.AutoAttendant.AfterHoursMainMenuCustomPromptFilename);
			if (this.AutoAttendant.AfterHoursKeyMapping != null)
			{
				foreach (CustomMenuKeyMapping customMenuKeyMapping in this.AutoAttendant.AfterHoursKeyMapping)
				{
					PublishingSessionBase.AddIfNotEmpty(fileList, customMenuKeyMapping.PromptFileName);
				}
			}
			PublishingSessionBase.AddIfNotEmpty(fileList, this.AutoAttendant.BusinessHoursWelcomeGreetingFilename);
			PublishingSessionBase.AddIfNotEmpty(fileList, this.AutoAttendant.BusinessHoursMainMenuCustomPromptFilename);
			if (this.AutoAttendant.BusinessHoursKeyMapping != null)
			{
				foreach (CustomMenuKeyMapping customMenuKeyMapping2 in this.AutoAttendant.BusinessHoursKeyMapping)
				{
					PublishingSessionBase.AddIfNotEmpty(fileList, customMenuKeyMapping2.PromptFileName);
				}
			}
			if (this.AutoAttendant.HolidaySchedule != null)
			{
				foreach (HolidaySchedule holidaySchedule in this.AutoAttendant.HolidaySchedule)
				{
					PublishingSessionBase.AddIfNotEmpty(fileList, holidaySchedule.Greeting);
				}
			}
		}

		protected override void UpdatePromptChangeKey(Guid guid)
		{
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(this.AutoAttendant.OrganizationId, false);
			UMAutoAttendant autoAttendantFromId = iadsystemConfigurationLookup.GetAutoAttendantFromId(this.AutoAttendant.Id);
			autoAttendantFromId.PromptChangeKey = guid;
			autoAttendantFromId.Session.Save(autoAttendantFromId);
		}

		private UMDialPlan dialPlan;
	}
}
