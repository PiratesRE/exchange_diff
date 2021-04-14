using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public abstract class ThrottleSettingsBase
	{
		public ThrottleSettingsBase()
		{
			this.fixedEntries = new FixedThrottleEntry[0];
			this.globalOverrides = new OverrideThrottleEntry[0];
			this.localOverrides = new OverrideThrottleEntry[0];
		}

		public void Initialize(IEnumerable<FixedThrottleEntry> fixedEntries, IEnumerable<WorkDefinitionOverride> globalOverrides, IEnumerable<WorkDefinitionOverride> localOverrides)
		{
			this.fixedEntries = this.OrderEntries<FixedThrottleEntry>(fixedEntries);
			if (globalOverrides != null)
			{
				this.globalOverrides = this.OrderEntries<OverrideThrottleEntry>(from rawOverride in globalOverrides
				select OverrideThrottleEntry.ParseRawOverride(ThrottleEntryType.GlobalOverride, rawOverride));
			}
			if (localOverrides != null)
			{
				this.localOverrides = this.OrderEntries<OverrideThrottleEntry>(from rawOverride in localOverrides
				select OverrideThrottleEntry.ParseRawOverride(ThrottleEntryType.LocalOverride, rawOverride));
			}
		}

		public abstract string[] GetServersInGroup(string categoryName);

		public abstract FixedThrottleEntry ConstructDefaultThrottlingSettings(RecoveryActionId recoveryActionId);

		public FixedThrottleEntry GetThrottleDefinition(RecoveryActionId recoveryActionId, string resourceName, ResponderDefinition responderDefinition)
		{
			return this.GetThrottleDefinition(recoveryActionId, responderDefinition.ResponderCategory, responderDefinition.TypeName, responderDefinition.Name, resourceName, responderDefinition.ServiceName);
		}

		public FixedThrottleEntry GetThrottleDefinition(RecoveryActionId recoveryActionId, string responderCategoryStr, string responderTypeName, string responderName, string resourceName, string healthSetName)
		{
			ResponderCategory responderCategory = ThrottleDescriptionEntry.ParseResponderCategory(responderCategoryStr);
			FixedThrottleEntry fixedMatchingEntry = this.GetFixedMatchingEntry(recoveryActionId, responderCategory, responderTypeName, responderName, resourceName);
			this.ApplyOverrides(healthSetName, fixedMatchingEntry, this.globalOverrides);
			this.ApplyOverrides(healthSetName, fixedMatchingEntry, this.localOverrides);
			return fixedMatchingEntry;
		}

		public string ConvertThrottleDefinitionsToCompactXml(IEnumerable<ThrottleDescriptionEntry> entries)
		{
			XElement xelement = new XElement("ThrottleEntries");
			foreach (ThrottleDescriptionEntry throttleDescriptionEntry in entries)
			{
				XElement xelement2 = new XElement(throttleDescriptionEntry.RecoveryActionId.ToString(), new XAttribute("ResourceName", throttleDescriptionEntry.ResourceName));
				xelement2.Add(throttleDescriptionEntry.GetThrottlePropertiesAsXml());
				xelement.Add(xelement2);
			}
			return xelement.ToString();
		}

		public string GetThrottleDefinitionsAsCompactXml(RecoveryActionId recoveryActionId, string resourceName, ResponderDefinition responderDefinition)
		{
			return this.ConvertThrottleDefinitionsToCompactXml(new List<ThrottleDescriptionEntry>
			{
				this.GetThrottleDefinition(recoveryActionId, resourceName, responderDefinition)
			});
		}

		internal T[] OrderEntries<T>(IEnumerable<T> entries) where T : ThrottleDescriptionEntry
		{
			IOrderedEnumerable<T> source = from entry in entries
			where entry != null
			orderby entry.RecoveryActionId, entry.ResponderCategory, entry.ResponderTypeName, entry.ResponderName, entry.ResourceName
			select entry;
			return source.ToArray<T>();
		}

		internal FixedThrottleEntry GetFixedMatchingEntry(RecoveryActionId recoveryActionId, ResponderCategory responderCategory, string responderTypeName, string responderName, string resourceName)
		{
			FixedThrottleEntry fixedThrottleEntry = this.fixedEntries.LastOrDefault((FixedThrottleEntry v) => v.IsConfigMatches(recoveryActionId, responderCategory, responderTypeName, responderName, resourceName));
			if (fixedThrottleEntry == null)
			{
				WTFDiagnostics.TraceDebug<RecoveryActionId>(ExTraceGlobals.RecoveryActionTracer, this.traceContext, "RecoveryActionId: {0} is not registered with throttling framework. Using the default throttle settings", recoveryActionId, null, "GetFixedMatchingEntry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\ThrottleSettingsBase.cs", 255);
				fixedThrottleEntry = this.ConstructDefaultThrottlingSettings(recoveryActionId);
			}
			return new FixedThrottleEntry(recoveryActionId, responderCategory, responderTypeName, responderName, resourceName, fixedThrottleEntry.ThrottleParameters);
		}

		internal void ApplyOverrides(string healthSetName, FixedThrottleEntry baseEntry, IEnumerable<OverrideThrottleEntry> overrideEntries)
		{
			foreach (OverrideThrottleEntry overrideThrottleEntry in overrideEntries)
			{
				if (overrideThrottleEntry.IsConfigMatches(baseEntry.RecoveryActionId, baseEntry.ResponderCategory, baseEntry.ResponderTypeName, baseEntry.ResponderName, baseEntry.ResourceName))
				{
					baseEntry.ThrottleParameters.ApplyPropertyOverrides(overrideThrottleEntry.PropertyBag);
				}
			}
		}

		protected void ReportAllThrottleEntriesToCrimson(bool isLogAsync)
		{
			Action action = delegate()
			{
				NativeMethods.EvtClearLog(IntPtr.Zero, "Microsoft-Exchange-ManagedAvailability/ThrottlingConfig", null, 0);
				this.ReportEntriesToCrimsonEvent(this.fixedEntries);
				this.ReportEntriesToCrimsonEvent(this.globalOverrides);
				this.ReportEntriesToCrimsonEvent(this.localOverrides);
			};
			if (isLogAsync)
			{
				ThreadPool.QueueUserWorkItem(delegate(object unused)
				{
					action();
				});
				return;
			}
			action();
		}

		private void ReportEntriesToCrimsonEvent(IEnumerable<ThrottleDescriptionEntry> entries)
		{
			foreach (ThrottleDescriptionEntry throttleDescriptionEntry in entries)
			{
				throttleDescriptionEntry.WriteToCrimsonLog();
			}
		}

		public const string ThrottlingConfigChannelName = "Microsoft-Exchange-ManagedAvailability/ThrottlingConfig";

		private FixedThrottleEntry[] fixedEntries;

		private OverrideThrottleEntry[] globalOverrides;

		private OverrideThrottleEntry[] localOverrides;

		private TracingContext traceContext = TracingContext.Default;
	}
}
