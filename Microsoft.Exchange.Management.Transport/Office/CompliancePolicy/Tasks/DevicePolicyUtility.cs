using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal static class DevicePolicyUtility
	{
		static DevicePolicyUtility()
		{
			DevicePolicyUtility.WorkloadToConfigurationRuleGuidMap = new Dictionary<Workload, Guid>();
			DevicePolicyUtility.WorkloadToConfigurationRuleGuidMap.Add(Workload.Exchange, DevicePolicyUtility.ExchangeDeviceConfigurationRuleGuid);
			DevicePolicyUtility.WorkloadToConfigurationRuleGuidMap.Add(Workload.Intune, DevicePolicyUtility.IntuneDeviceConfigurationRuleGuid);
			DevicePolicyUtility.WorkloadToConditionalAccessRuleGuidMap = new Dictionary<Workload, Guid>();
			DevicePolicyUtility.WorkloadToConditionalAccessRuleGuidMap.Add(Workload.Exchange, DevicePolicyUtility.ExchangeDeviceConditionalAccessRuleGuid);
			DevicePolicyUtility.WorkloadToConditionalAccessRuleGuidMap.Add(Workload.Intune, DevicePolicyUtility.IntuneDeviceConditionalAccessRuleGuid);
			DevicePolicyUtility.WorkloadToTenantConditionalAccessRuleGuidMap = new Dictionary<Workload, Guid>();
			DevicePolicyUtility.WorkloadToTenantConditionalAccessRuleGuidMap.Add(Workload.Intune, DevicePolicyUtility.IntuneDeviceTenantConditionalAccessRuleGuid);
			DevicePolicyUtility.WorkloadToTenantConditionalAccessPolicyGuidMap = new Dictionary<Workload, Guid>();
			DevicePolicyUtility.WorkloadToTenantConditionalAccessPolicyGuidMap.Add(Workload.Intune, DevicePolicyUtility.IntuneDeviceTenantConditionalAccessPolicyGuid);
			DevicePolicyUtility.ConfigurationRuleGuidSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			DevicePolicyUtility.ConfigurationRuleGuidSet.Add(DevicePolicyUtility.ExchangeDeviceConfigurationRuleGuid.ToString());
			DevicePolicyUtility.ConfigurationRuleGuidSet.Add(DevicePolicyUtility.IntuneDeviceConfigurationRuleGuid.ToString());
			DevicePolicyUtility.ConditionalAccessRuleGuidSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			DevicePolicyUtility.ConditionalAccessRuleGuidSet.Add(DevicePolicyUtility.ExchangeDeviceConditionalAccessRuleGuid.ToString());
			DevicePolicyUtility.ConditionalAccessRuleGuidSet.Add(DevicePolicyUtility.IntuneDeviceConditionalAccessRuleGuid.ToString());
			DevicePolicyUtility.TenantConditionalAccessRuleGuidSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			DevicePolicyUtility.TenantConditionalAccessRuleGuidSet.Add(DevicePolicyUtility.IntuneDeviceTenantConditionalAccessRuleGuid.ToString());
			DevicePolicyUtility.TenantConditionalAccessPolicyGuidSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			DevicePolicyUtility.TenantConditionalAccessPolicyGuidSet.Add(DevicePolicyUtility.IntuneDeviceTenantConditionalAccessPolicyGuid.ToString());
		}

		internal static bool GetConfigurationRuleGuidFromWorkload(Workload workload, out Guid ruleGuid)
		{
			return DevicePolicyUtility.WorkloadToConfigurationRuleGuidMap.TryGetValue(workload, out ruleGuid);
		}

		internal static bool GetConditionalAccessRuleGuidFromWorkload(Workload workload, out Guid ruleGuid)
		{
			return DevicePolicyUtility.WorkloadToConditionalAccessRuleGuidMap.TryGetValue(workload, out ruleGuid);
		}

		internal static bool GetTenantRuleGuidFromWorkload(Workload workload, out Guid ruleGuid)
		{
			return DevicePolicyUtility.WorkloadToTenantConditionalAccessRuleGuidMap.TryGetValue(workload, out ruleGuid);
		}

		internal static bool GetTenantPolicyGuidFromWorkload(Workload workload, out Guid ruleGuid)
		{
			return DevicePolicyUtility.WorkloadToTenantConditionalAccessPolicyGuidMap.TryGetValue(workload, out ruleGuid);
		}

		internal static bool IsDeviceConditionalAccessRule(string identity)
		{
			Guid ruleGuid;
			return Guid.TryParse(identity.Substring((identity.LastIndexOf('{') == -1) ? 0 : identity.LastIndexOf('{')), out ruleGuid) && DevicePolicyUtility.IsDeviceConditionalAccessRule(ruleGuid);
		}

		internal static bool IsDeviceConfigurationRule(string identity)
		{
			Guid ruleGuid;
			return Guid.TryParse(identity.Substring((identity.LastIndexOf('{') == -1) ? 0 : identity.LastIndexOf('{')), out ruleGuid) && DevicePolicyUtility.IsDeviceConfigurationRule(ruleGuid);
		}

		internal static bool IsDeviceTenantRule(string identity)
		{
			Guid ruleGuid;
			return Guid.TryParse(identity.Substring((identity.LastIndexOf('{') == -1) ? 0 : identity.LastIndexOf('{')), out ruleGuid) && DevicePolicyUtility.IsDeviceTenantRule(ruleGuid);
		}

		internal static bool IsDeviceTenantRule(Guid ruleGuid)
		{
			return DevicePolicyUtility.TenantConditionalAccessRuleGuidSet.Contains(ruleGuid.ToString());
		}

		internal static bool IsDeviceConfigurationRule(Guid ruleGuid)
		{
			return DevicePolicyUtility.ConfigurationRuleGuidSet.Contains(ruleGuid.ToString());
		}

		internal static bool IsDeviceConditionalAccessRule(Guid ruleGuid)
		{
			return DevicePolicyUtility.ConditionalAccessRuleGuidSet.Contains(ruleGuid.ToString());
		}

		internal static bool IsPropertySpecified(ADPresentationObject presentationObject, ADPropertyDefinition property)
		{
			object obj;
			return presentationObject != null && property != null && presentationObject.TryGetValueWithoutDefault(property, out obj);
		}

		internal static void ValidateDeviceScenarioArgument(PolicyScenario scenario)
		{
			if (!DevicePolicyUtility.DevicePolicyScenarios.Contains(scenario))
			{
				throw new ArgumentException("Invalid Policy Scenario Argument");
			}
		}

		private static readonly Guid ExchangeDeviceConfigurationRuleGuid = new Guid("4CD01950-43F9-47A1-AF0C-EA4E1BE47CBB");

		private static readonly Guid IntuneDeviceConfigurationRuleGuid = new Guid("58B50D1C-2B18-461C-8893-3E20C648B136");

		private static readonly Guid ExchangeDeviceConditionalAccessRuleGuid = new Guid("3CB6EC45-68E8-4758-935B-FCEFD71E234C");

		private static readonly Guid IntuneDeviceConditionalAccessRuleGuid = new Guid("914F151C-394B-4DA9-9422-F5A2F65DEC30");

		private static readonly Guid IntuneDeviceTenantConditionalAccessRuleGuid = new Guid("7577C5F3-05A4-4F55-A0A3-82AAB5E98C84");

		private static readonly Guid IntuneDeviceTenantConditionalAccessPolicyGuid = new Guid("A6958701-C82C-4064-AC11-64E40E7F4032");

		private static readonly Dictionary<Workload, Guid> WorkloadToConfigurationRuleGuidMap = null;

		private static readonly Dictionary<Workload, Guid> WorkloadToConditionalAccessRuleGuidMap = null;

		private static readonly Dictionary<Workload, Guid> WorkloadToTenantConditionalAccessRuleGuidMap = null;

		private static readonly Dictionary<Workload, Guid> WorkloadToTenantConditionalAccessPolicyGuidMap = null;

		private static readonly HashSet<string> ConfigurationRuleGuidSet = null;

		private static readonly HashSet<string> ConditionalAccessRuleGuidSet = null;

		private static readonly HashSet<string> TenantConditionalAccessRuleGuidSet = null;

		private static readonly HashSet<string> TenantConditionalAccessPolicyGuidSet = null;

		private static readonly IEnumerable<PolicyScenario> DevicePolicyScenarios = new PolicyScenario[]
		{
			PolicyScenario.DeviceConditionalAccess,
			PolicyScenario.DeviceSettings,
			PolicyScenario.DeviceTenantConditionalAccess
		};
	}
}
