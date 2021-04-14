using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public abstract class ThrottleDescriptionEntry
	{
		public ThrottleDescriptionEntry(ThrottleEntryType entryType, RecoveryActionId recoveryActionId, ResponderCategory responderCategory = ResponderCategory.Default, string responderTypeName = "*", string responderName = "*", string resourceName = "*")
		{
			this.ThrottleEntryType = entryType;
			this.RecoveryActionId = recoveryActionId;
			this.ResponderCategory = responderCategory;
			this.ResponderTypeName = (string.IsNullOrWhiteSpace(responderTypeName) ? "*" : responderTypeName);
			this.ResponderName = (string.IsNullOrWhiteSpace(responderName) ? "*" : responderName);
			this.ResourceName = (string.IsNullOrWhiteSpace(resourceName) ? "*" : resourceName);
			this.Identity = this.ConstructIdentity();
		}

		internal RecoveryActionId RecoveryActionId { get; private set; }

		internal ResponderCategory ResponderCategory { get; private set; }

		internal string ResponderTypeName { get; private set; }

		internal string ResponderName { get; private set; }

		internal string ResourceName { get; private set; }

		internal string Identity { get; private set; }

		internal ThrottleEntryType ThrottleEntryType { get; private set; }

		internal static bool IsMatchWildcard(string source, string target)
		{
			if (string.IsNullOrEmpty(source) || string.Equals(source, "*"))
			{
				return true;
			}
			if (string.IsNullOrEmpty(target))
			{
				return false;
			}
			string pattern = "^" + Regex.Escape(source).Replace("\\*", ".*").Replace("\\?", ".") + "$";
			return Regex.IsMatch(target, pattern, RegexOptions.IgnoreCase);
		}

		internal static ResponderCategory ParseResponderCategory(string categoryStr)
		{
			ResponderCategory result = ResponderCategory.Default;
			if (!string.IsNullOrWhiteSpace(categoryStr) && !Enum.TryParse<ResponderCategory>(categoryStr, true, out result))
			{
				result = ResponderCategory.Default;
			}
			return result;
		}

		internal string ConstructIdentity()
		{
			return string.Format("{0}/{1}/{2}/{3}/{4}", new object[]
			{
				this.RecoveryActionId,
				this.ResponderCategory,
				this.ResponderTypeName,
				this.ResponderName,
				this.ResourceName
			});
		}

		internal bool IsConfigMatches(RecoveryActionId recoveryActionId, ResponderCategory responderCategory, string responderTypeName, string responderName, string resourceName)
		{
			return (this.RecoveryActionId == RecoveryActionId.Any || this.RecoveryActionId == recoveryActionId) && (this.ResponderCategory == ResponderCategory.Default || this.ResponderCategory == responderCategory) && ThrottleDescriptionEntry.IsMatchWildcard(this.ResponderTypeName, responderTypeName) && ThrottleDescriptionEntry.IsMatchWildcard(this.ResponderName, responderName) && ThrottleDescriptionEntry.IsMatchWildcard(this.ResourceName, resourceName);
		}

		internal void WriteToCrimsonLog()
		{
			ManagedAvailabilityCrimsonEvent managedAvailabilityCrimsonEvent;
			switch (this.ThrottleEntryType)
			{
			case ThrottleEntryType.BaseConfig:
				managedAvailabilityCrimsonEvent = ManagedAvailabilityCrimsonEvents.ThrottleBaseConfig;
				break;
			case ThrottleEntryType.GlobalOverride:
				managedAvailabilityCrimsonEvent = ManagedAvailabilityCrimsonEvents.ThrottleGlobalOverride;
				break;
			case ThrottleEntryType.LocalOverride:
				managedAvailabilityCrimsonEvent = ManagedAvailabilityCrimsonEvents.ThrottleLocalOverride;
				break;
			default:
				managedAvailabilityCrimsonEvent = ManagedAvailabilityCrimsonEvents.ThrottleEffective;
				break;
			}
			XElement throttlePropertiesAsXml = this.GetThrottlePropertiesAsXml();
			managedAvailabilityCrimsonEvent.LogGeneric(new object[]
			{
				this.Identity,
				this.RecoveryActionId,
				this.ResponderCategory,
				this.ResponderTypeName,
				this.ResponderName,
				this.ResourceName,
				throttlePropertiesAsXml.ToString()
			});
		}

		internal abstract Dictionary<string, string> GetPropertyBag();

		internal XElement GetThrottlePropertiesAsXml()
		{
			Dictionary<string, string> propertyBag = this.GetPropertyBag();
			XAttribute[] content = (from kvp in propertyBag
			select new XAttribute(kvp.Key, kvp.Value)).ToArray<XAttribute>();
			return new XElement("ThrottleConfig", content);
		}

		internal const string Default = "*";
	}
}
