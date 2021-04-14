using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	internal class OverrideThrottleEntry : ThrottleDescriptionEntry
	{
		internal OverrideThrottleEntry(ThrottleEntryType entryType, RecoveryActionId recoveryActionId, ResponderCategory responderCategory, string responderTypeName, string responderName, string resourceName, string propertyName, WorkDefinitionOverride rawOverride) : base(entryType, recoveryActionId, responderCategory, responderTypeName, responderName, resourceName)
		{
			this.PropertyBag = new Dictionary<string, string>
			{
				{
					propertyName,
					rawOverride.NewPropertyValue
				}
			};
			this.RawOverride = rawOverride;
			this.HealthSetName = rawOverride.ServiceName;
		}

		internal Dictionary<string, string> PropertyBag { get; set; }

		internal WorkDefinitionOverride RawOverride { get; set; }

		internal string HealthSetName { get; private set; }

		internal static OverrideThrottleEntry ParseRawOverride(ThrottleEntryType entryType, WorkDefinitionOverride rawOverride)
		{
			RecoveryActionId recoveryActionId = RecoveryActionId.None;
			ResponderCategory responderCategory = ResponderCategory.Default;
			string responderTypeName = "*";
			string responderName = "*";
			string resourceName = "*";
			string propertyName = string.Empty;
			string serviceName = rawOverride.ServiceName;
			string workDefinitionName = rawOverride.WorkDefinitionName;
			string propertyName2 = rawOverride.PropertyName;
			if (string.IsNullOrWhiteSpace(propertyName2))
			{
				return null;
			}
			if (string.Equals(serviceName, ExchangeComponent.RecoveryAction.Name))
			{
				if (!Enum.TryParse<RecoveryActionId>(workDefinitionName, true, out recoveryActionId))
				{
					return null;
				}
				string[] array = (from o in propertyName2.Split(new char[]
				{
					'/',
					':'
				})
				select o.Trim()).ToArray<string>();
				if (array.Length > 1)
				{
					if (!string.Equals(array[0], "*", StringComparison.OrdinalIgnoreCase) && !Enum.TryParse<ResponderCategory>(array[0], true, out responderCategory))
					{
						return null;
					}
					if (array.Length > 2)
					{
						responderTypeName = array[1];
					}
					if (array.Length > 3)
					{
						responderName = array[2];
					}
					if (array.Length > 4)
					{
						resourceName = array[3];
					}
				}
				propertyName = array[array.Length - 1];
			}
			else
			{
				string[] array2 = propertyName2.Split(new char[]
				{
					'.'
				});
				if (array2.Length < 2 || !string.Equals(array2[0], "ThrottleAttributes"))
				{
					return null;
				}
				recoveryActionId = RecoveryActionId.Any;
				responderName = workDefinitionName;
				propertyName = array2[1];
				string[] array3 = workDefinitionName.Split(new char[]
				{
					'/'
				});
				if (array3.Length > 1)
				{
					responderName = array3[0];
					resourceName = array3[1];
				}
			}
			return new OverrideThrottleEntry(entryType, recoveryActionId, responderCategory, responderTypeName, responderName, resourceName, propertyName, rawOverride);
		}

		internal bool IsHealthSetMatching(string healthSetName)
		{
			return !string.IsNullOrEmpty(this.HealthSetName) && !string.IsNullOrEmpty(healthSetName) && (string.Equals(this.HealthSetName, healthSetName, StringComparison.OrdinalIgnoreCase) || string.Equals(this.HealthSetName, ExchangeComponent.RecoveryAction.Name, StringComparison.OrdinalIgnoreCase));
		}

		internal override Dictionary<string, string> GetPropertyBag()
		{
			return this.PropertyBag;
		}
	}
}
