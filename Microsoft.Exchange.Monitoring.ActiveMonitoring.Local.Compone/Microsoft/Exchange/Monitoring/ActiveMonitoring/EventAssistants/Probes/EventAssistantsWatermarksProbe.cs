using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.EventAssistants.Probes
{
	public class EventAssistantsWatermarksProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition definition, Dictionary<string, string> propertyBag)
		{
			EventAssistantsDiscovery.PopulateProbeDefinition(definition as ProbeDefinition, propertyBag["TargetResource"], base.GetType(), definition.Name, TimeSpan.MaxValue, TimeSpan.FromMinutes(5.0));
			ProbeDefinition probeDefinition = StoreMonitoringHelpers.GetProbeDefinition(Environment.MachineName, definition.Name, propertyBag["TargetResource"], ExchangeComponent.EventAssistants.Name);
			Dictionary<string, string> dictionary = DefinitionHelperBase.ConvertExtensionAttributesToDictionary(probeDefinition.ExtensionAttributes);
			definition.Attributes["WatermarksVariationThreshold"] = dictionary["WatermarksVariationThreshold"];
			definition.Attributes["WatermarksBehindWarningThreshold"] = dictionary["WatermarksBehindWarningThreshold"];
			definition.Attributes["IncludedAssistantType"] = dictionary["IncludedAssistantType"];
			if (dictionary.ContainsKey("ExcludedAssistantType"))
			{
				definition.Attributes["ExcludedAssistantType"] = dictionary["ExcludedAssistantType"];
			}
			MailboxDatabase mailboxDatabaseFromName = DirectoryAccessor.Instance.GetMailboxDatabaseFromName(propertyBag["TargetResource"]);
			definition.TargetExtension = mailboxDatabaseFromName.Guid.ToString();
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>
			{
				new PropertyInformation("Identity", Strings.EventAssistantsWatermarksHelpString, true)
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DateTime utcNow = DateTime.UtcNow;
			string targetResource = base.Definition.TargetResource;
			string targetExtension = base.Definition.TargetExtension;
			Guid guid = new Guid(targetExtension);
			List<string> excludedAssistants = new List<string>();
			try
			{
				base.Result.StateAttribute1 = targetResource;
				base.Result.StateAttribute2 = targetExtension;
				int num = int.Parse(base.Definition.Attributes["WatermarksVariationThreshold"]);
				TimeSpan watermarkBehindWarningThrehold = TimeSpan.Parse(base.Definition.Attributes["WatermarksBehindWarningThreshold"]);
				base.Result.StateAttribute3 = watermarkBehindWarningThrehold.ToString();
				base.Result.StateAttribute6 = (double)num;
				string text = base.Definition.Attributes["IncludedAssistantType"];
				Guid? assistantConsumerGuidFromName = AssistantsCollection.GetAssistantConsumerGuidFromName(text);
				if (assistantConsumerGuidFromName == null)
				{
					throw new InvalidIncludedAssistantTypeException();
				}
				if (base.Definition.Attributes.ContainsKey("ExcludedAssistantType") && !string.IsNullOrWhiteSpace(base.Definition.Attributes["ExcludedAssistantType"]))
				{
					excludedAssistants = base.Definition.Attributes["ExcludedAssistantType"].Split(new char[]
					{
						','
					}).ToList<string>();
				}
				if (DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(guid))
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.EventAssistantsTracer, base.TraceContext, "Starting database watermarks check against database {0}", targetResource, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\EventAssistants\\EventAssistantsWatermarksProbe.cs", 126);
					using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Monitoring", Environment.MachineName, null, null, null))
					{
						Guid empty = Guid.Empty;
						Watermark[] watermarksForMailbox = exRpcAdmin.GetWatermarksForMailbox(guid, ref empty, Guid.Empty);
						MapiEventManager mapiEventManager = MapiEventManager.Create(exRpcAdmin, assistantConsumerGuidFromName.Value, guid);
						long eventCounter = mapiEventManager.ReadLastEvent().EventCounter;
						foreach (Watermark watermark in watermarksForMailbox)
						{
							if (eventCounter - watermark.EventCounter > (long)num)
							{
								EventAssistantsWatermarksProbe.WatermarkWithCreateTime[] waterMarkWithCreateTimes = EventAssistantsWatermarksProbe.BuildWaterMarkWithCreateTimes(mapiEventManager, watermarksForMailbox);
								DateTime eventTime = EventAssistantsWatermarksProbe.GetEventTime(mapiEventManager, eventCounter);
								List<EventAssistantsWatermarksProbe.WatermarkWithCreateTime> list = EventAssistantsWatermarksProbe.PopulateProblematicAssistants(waterMarkWithCreateTimes, eventTime, watermarkBehindWarningThrehold, text, excludedAssistants);
								bool flag = EventAssistantsWatermarksProbe.MakeFailureDecision(text, list);
								if (flag)
								{
									base.Result.StateAttribute11 = EventAssistantsWatermarksProbe.FindProblematicAssistant(list);
									base.Result.StateAttribute4 = EventAssistantsWatermarksProbe.BuildFormattedEventCounter(eventCounter, eventTime);
									base.Result.StateAttribute5 = EventAssistantsWatermarksProbe.BuildFormattedWatermarks(list);
									WTFDiagnostics.TraceError<string, string, string>(ExTraceGlobals.EventAssistantsTracer, base.TraceContext, "Database {0} is behind on watermarks. Last event counter: {1} Watermarks: {2}", targetResource, base.Result.StateAttribute4, base.Result.StateAttribute5, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\EventAssistants\\EventAssistantsWatermarksProbe.cs", 189);
									throw new WatermarksBehindException(targetResource);
								}
							}
						}
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.EventAssistantsTracer, base.TraceContext, "Successfully finished database watermarks check against database {0}", targetResource, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\EventAssistants\\EventAssistantsWatermarksProbe.cs", 202);
						goto IL_2F5;
					}
				}
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Skipping database watermarks check against database {0} as it is not mounted locally", targetResource, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\EventAssistants\\EventAssistantsWatermarksProbe.cs", 211);
				IL_2F5:;
			}
			finally
			{
				base.Result.SampleValue = (double)((int)(DateTime.UtcNow - utcNow).TotalMilliseconds);
			}
		}

		internal static List<EventAssistantsWatermarksProbe.WatermarkWithCreateTime> PopulateProblematicAssistants(EventAssistantsWatermarksProbe.WatermarkWithCreateTime[] waterMarkWithCreateTimes, DateTime lastEventTime, TimeSpan watermarkBehindWarningThrehold, string includedAssistantType, List<string> excludedAssistants)
		{
			List<EventAssistantsWatermarksProbe.WatermarkWithCreateTime> list = new List<EventAssistantsWatermarksProbe.WatermarkWithCreateTime>();
			if (waterMarkWithCreateTimes != null)
			{
				foreach (EventAssistantsWatermarksProbe.WatermarkWithCreateTime watermarkWithCreateTime in waterMarkWithCreateTimes)
				{
					if (watermarkWithCreateTime != null && lastEventTime - watermarkWithCreateTime.CreateTime > watermarkBehindWarningThrehold && AssistantsCollection.Contains(watermarkWithCreateTime.ConsumerGuid) && excludedAssistants != null && !excludedAssistants.Contains(AssistantsCollection.GetAssistantName(watermarkWithCreateTime.ConsumerGuid), StringComparer.InvariantCultureIgnoreCase))
					{
						list.Add(watermarkWithCreateTime);
					}
				}
			}
			return list;
		}

		internal static bool MakeFailureDecision(string includedAssistantType, List<EventAssistantsWatermarksProbe.WatermarkWithCreateTime> lowWatermarks)
		{
			if (lowWatermarks != null && lowWatermarks.Count != 0)
			{
				if (lowWatermarks.Count == 1 && (string.Equals(AssistantsCollection.GetAssistantName(lowWatermarks[0].ConsumerGuid), includedAssistantType) || string.Equals(includedAssistantType, "MultipleAssistants")))
				{
					return true;
				}
				if (lowWatermarks.Count > 1 && string.Equals(includedAssistantType, "MultipleAssistants"))
				{
					return true;
				}
			}
			return false;
		}

		internal static string FindProblematicAssistant(List<EventAssistantsWatermarksProbe.WatermarkWithCreateTime> lowWatermarks)
		{
			if (lowWatermarks.Count == 1)
			{
				return AssistantsCollection.GetAssistantName(lowWatermarks[0].ConsumerGuid);
			}
			if (lowWatermarks.Count == AssistantsCollection.complianceAssistants.Count)
			{
				for (int i = 0; i < lowWatermarks.Count; i++)
				{
					if (!AssistantsCollection.complianceAssistants.Contains(AssistantsCollection.GetAssistantName(lowWatermarks[i].ConsumerGuid), StringComparer.InvariantCultureIgnoreCase))
					{
						return "MultipleAssistants";
					}
				}
				return AssistantsCollection.GetAssistantName(lowWatermarks[0].ConsumerGuid);
			}
			return "MultipleAssistants";
		}

		private static EventAssistantsWatermarksProbe.WatermarkWithCreateTime[] BuildWaterMarkWithCreateTimes(MapiEventManager eventManager, Watermark[] watermarks)
		{
			EventAssistantsWatermarksProbe.WatermarkWithCreateTime[] array = new EventAssistantsWatermarksProbe.WatermarkWithCreateTime[watermarks.Length];
			for (int i = 0; i < watermarks.Length; i++)
			{
				DateTime eventTime = EventAssistantsWatermarksProbe.GetEventTime(eventManager, watermarks[i].EventCounter);
				array[i] = new EventAssistantsWatermarksProbe.WatermarkWithCreateTime(watermarks[i].ConsumerGuid, watermarks[i].EventCounter, eventTime);
			}
			return array;
		}

		private static DateTime GetEventTime(MapiEventManager eventManager, long eventCounter)
		{
			MapiEvent[] array = eventManager.ReadEvents(eventCounter, 1);
			if (array != null && array.Length != 0)
			{
				return array[0].CreateTime;
			}
			return DateTime.UtcNow;
		}

		private static string BuildFormattedWatermarks(List<EventAssistantsWatermarksProbe.WatermarkWithCreateTime> watermarks)
		{
			StringBuilder stringBuilder = new StringBuilder(watermarks.Count * 70);
			for (int i = 0; i < watermarks.Count; i++)
			{
				stringBuilder.Append(AssistantsCollection.GetAssistantName(watermarks[i].ConsumerGuid));
				stringBuilder.Append(" : ");
				stringBuilder.Append(EventAssistantsWatermarksProbe.BuildFormattedEventCounter(watermarks[i].EventCounter, watermarks[i].CreateTime));
				if (i != watermarks.Count - 1)
				{
					stringBuilder.Append(", \r\n");
				}
			}
			return stringBuilder.ToString();
		}

		private static string BuildFormattedEventCounter(long eventCounter, DateTime eventTime)
		{
			return string.Format("{0} : {1}", eventCounter, eventTime);
		}

		public class WatermarkWithCreateTime
		{
			public WatermarkWithCreateTime(Guid consumerGuid, long eventCounter, DateTime createTime)
			{
				this.ConsumerGuid = consumerGuid;
				this.EventCounter = eventCounter;
				this.CreateTime = createTime;
			}

			public Guid ConsumerGuid { get; private set; }

			public long EventCounter { get; private set; }

			public DateTime CreateTime { get; private set; }
		}
	}
}
