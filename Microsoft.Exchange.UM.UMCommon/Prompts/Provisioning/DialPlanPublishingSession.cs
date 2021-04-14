using System;
using System.Collections;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	internal class DialPlanPublishingSession : PublishingSessionBase
	{
		internal DialPlanPublishingSession(string userName, UMDialPlan dialPlan) : base(userName, dialPlan)
		{
		}

		protected override UMDialPlan DialPlan
		{
			get
			{
				return (UMDialPlan)base.ConfigurationObject;
			}
		}

		public override void Upload(string source, string destinationName)
		{
			try
			{
				base.Upload(source, destinationName);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DialPlanCustomPromptUploadSucceeded, null, new object[]
				{
					base.UserName,
					this.DialPlan.Id,
					destinationName
				});
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PromptProvisioningTracer, this, "DialPlanPublishingSession.Upload: {0}.", new object[]
				{
					ex
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DialPlanCustomPromptUploadFailed, null, new object[]
				{
					base.UserName,
					this.DialPlan.Id,
					ex.Message
				});
				throw;
			}
		}

		protected override void AddConfiguredFiles(IDictionary fileList)
		{
			PublishingSessionBase.AddIfNotEmpty(fileList, this.DialPlan.WelcomeGreetingFilename);
			PublishingSessionBase.AddIfNotEmpty(fileList, this.DialPlan.InfoAnnouncementFilename);
		}

		protected override void UpdatePromptChangeKey(Guid guid)
		{
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(this.DialPlan.OrganizationId, false);
			UMDialPlan dialPlanFromId = iadsystemConfigurationLookup.GetDialPlanFromId(this.DialPlan.Id);
			dialPlanFromId.PromptChangeKey = guid.ToString("N");
			dialPlanFromId.Session.Save(dialPlanFromId);
		}
	}
}
