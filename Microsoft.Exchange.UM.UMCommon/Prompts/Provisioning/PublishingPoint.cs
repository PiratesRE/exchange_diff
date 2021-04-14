using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	internal static class PublishingPoint
	{
		internal static IPublishingSession GetPublishingSession(string userName, ADConfigurationObject config)
		{
			IPublishingSession result;
			try
			{
				if (config == null)
				{
					throw new ArgumentNullException();
				}
				UMDialPlan umdialPlan = config as UMDialPlan;
				if (umdialPlan != null)
				{
					result = new DialPlanPublishingSession(userName, umdialPlan);
				}
				else
				{
					UMAutoAttendant umautoAttendant = config as UMAutoAttendant;
					if (umautoAttendant == null)
					{
						throw new ArgumentException();
					}
					result = new AutoAttendantPublishingSession(userName, umautoAttendant);
				}
			}
			catch (Exception ex)
			{
				PIIMessage data = PIIMessage.Create(PIIType._User, userName);
				CallIdTracer.TraceError(ExTraceGlobals.PromptProvisioningTracer, null, data, "PublishingPoint.GetPublishingSession(_User,{0}): {1}.", new object[]
				{
					config,
					ex
				});
				throw;
			}
			return result;
		}
	}
}
