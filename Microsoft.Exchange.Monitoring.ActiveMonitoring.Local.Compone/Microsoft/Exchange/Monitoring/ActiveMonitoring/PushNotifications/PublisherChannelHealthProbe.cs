using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PushNotifications
{
	public class PublisherChannelHealthProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			if (probeDefinition == null)
			{
				throw new ArgumentException("definition must be a ProbeDefinition");
			}
			if (!propertyBag.ContainsKey("TargetAppId"))
			{
				throw new ArgumentException("Please specify value for TargetAppIdMask");
			}
			probeDefinition.Attributes["TargetAppId"] = propertyBag["TargetAppId"].ToString();
			if (propertyBag.ContainsKey("TargetAppPublisher"))
			{
				probeDefinition.Attributes["TargetAppPublisher"] = propertyBag["TargetAppPublisher"].ToString();
				if (propertyBag.ContainsKey("SkipInstanceTag"))
				{
					probeDefinition.Attributes["SkipInstanceTag"] = propertyBag["SkipInstanceTag"].ToString();
				}
				return;
			}
			throw new ArgumentException("Please specify value for TargetAppPublisher");
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			bool flag = false;
			if (base.Definition.Attributes.ContainsKey("TargetAppId"))
			{
				this.appIdProbeMask = base.Definition.Attributes["TargetAppId"].ToString();
			}
			if (base.Definition.Attributes.ContainsKey("TargetAppPublisher"))
			{
				this.publisherType = base.Definition.Attributes["TargetAppPublisher"].ToString();
			}
			if (base.Definition.Attributes.ContainsKey("SkipInstanceTag"))
			{
				bool.TryParse(base.Definition.Attributes["SkipInstanceTag"], out this.skipInstance);
			}
			PublisherType publisherType = (PublisherType)Enum.Parse(typeof(PublisherType), this.publisherType);
			List<string> list = new List<string>();
			switch (publisherType)
			{
			case PublisherType.APNS:
				list.Add("ApnsChannelConnect");
				list.Add("ApnsChannelAuthenticate");
				list.Add("ApnsCertPresent");
				list.Add("ApnsCertValidation");
				list.Add("ApnsCertLoaded");
				list.Add("ApnsCertPrivateKey");
				break;
			case PublisherType.WNS:
				list.Add("WnsChannelBackOff");
				break;
			case PublisherType.GCM:
				list.Add("GcmChannelBackOff");
				break;
			case PublisherType.WebApp:
				list.Add("WebAppChannelBackOff");
				break;
			case PublisherType.Azure:
				list.Add("AzureChannelBackOff");
				break;
			case PublisherType.AzureHubCreation:
				list.Add("AzureHubCreationChannelBackOff");
				break;
			case PublisherType.AzureDeviceRegistration:
				list.Add("AzureDeviceRegistrationChannelBackOff");
				break;
			case PublisherType.AzureChallengeRequest:
				list.Add("AzureChallengeRequestChannelBackOff");
				break;
			default:
				throw new InvalidOperationException(string.Format("Monitoring for {0} publisher not implemented", publisherType.ToString()));
			}
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.PushNotificationTracer, base.TraceContext, "NotificationDeliveryProbe.DoWork: Checking presence of NotificationProcessed events for channel - {0}", this.appIdProbeMask, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PushNotifications\\Probes\\PublisherChannelHealthProbe.cs", 169);
			this.eventLookbackInterval = base.Definition.RecurrenceIntervalSeconds * 3;
			DateTime executionStartTime = base.Result.ExecutionStartTime;
			DateTime startTime = executionStartTime - TimeSpan.FromSeconds((double)this.eventLookbackInterval);
			DateTime endTime = executionStartTime;
			if (this.skipInstance && base.Definition.CreatedTime.AddSeconds((double)this.eventLookbackInterval) > executionStartTime)
			{
				ProbeResult result = base.Result;
				result.StateAttribute12 += string.Format("Skipping this probe instance: No event is expected yet as probe instance was created less than {0} minutes ago", this.eventLookbackInterval / 60);
				return;
			}
			foreach (string text in list)
			{
				WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.PushNotificationTracer, base.TraceContext, "ChannelHealthProbe.DoWork: Checking presence of {0} probe results for App - {1}", text, this.appIdProbeMask, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PushNotifications\\Probes\\PublisherChannelHealthProbe.cs", 185);
				string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.PushNotificationsProtocol.Name, text, this.appIdProbeMask);
				IDataAccessQuery<ProbeResult> probeResults = base.Broker.GetProbeResults(sampleMask, startTime, endTime);
				if (probeResults.Count<ProbeResult>() < 1)
				{
					ProbeResult result2 = base.Result;
					result2.StateAttribute12 += string.Format(this.resultStringFormat, text, this.appIdProbeMask, "No Results");
				}
				else
				{
					ProbeResult probeResult = probeResults.First<ProbeResult>();
					if (probeResult.ResultType != ResultType.Succeeded)
					{
						flag = true;
						ProbeResult result3 = base.Result;
						result3.StateAttribute12 += string.Format(this.resultStringFormat, text, this.appIdProbeMask, "Failed");
						ProbeResult result4 = base.Result;
						result4.StateAttribute13 += probeResult.StateAttribute2;
					}
					else
					{
						ProbeResult result5 = base.Result;
						result5.StateAttribute12 += string.Format(this.resultStringFormat, text, this.appIdProbeMask, "Passed");
					}
				}
			}
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Atleast one error has been detected in the {0} publisher channel for {1} app", publisherType.ToString(), this.appIdProbeMask));
			}
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>();
		}

		public const string TargetAppIdProperty = "TargetAppId";

		public const string PublisherForAppProperty = "TargetAppPublisher";

		protected const int NumberOfErrorsToRead = 3;

		protected string appIdProbeMask;

		protected string publisherType;

		protected bool skipInstance;

		protected int eventLookbackInterval;

		protected string resultStringFormat = "{0}/{1} : {2} | ";
	}
}
