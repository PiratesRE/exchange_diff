using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public sealed class DeviceTenantRule : DeviceRuleBase
	{
		public MultiValuedProperty<Guid> ExclusionList { get; set; }

		private new MultiValuedProperty<Guid> TargetGroups { get; set; }

		public DeviceTenantRule()
		{
		}

		public DeviceTenantRule(RuleStorage ruleStorage) : base(ruleStorage)
		{
		}

		public PolicyResourceScope? ApplyPolicyTo { get; set; }

		public bool? BlockUnsupportedDevices { get; set; }

		protected override IEnumerable<Condition> GetTaskConditions()
		{
			List<Condition> list = new List<Condition>();
			if (this.ExclusionList != null)
			{
				List<string> list2 = new List<string>();
				foreach (Guid guid in this.ExclusionList)
				{
					list2.Add(guid.ToString());
				}
				list.Add(new IsPredicate(Property.CreateProperty("ExclusionList", typeof(Guid).ToString()), list2));
			}
			if (this.ApplyPolicyTo != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceTenantRule.AccessControl_ResourceScope, typeof(string).ToString()), new List<string>
				{
					((int)this.ApplyPolicyTo.Value).ToString()
				}));
			}
			if (this.BlockUnsupportedDevices != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceTenantRule.AccessControl_AllowActionOnUnsupportedPlatform, typeof(string).ToString()), new List<string>
				{
					this.BlockUnsupportedDevices.ToString()
				}));
			}
			return list;
		}

		protected override void SetTaskConditions(IEnumerable<Condition> conditions)
		{
			foreach (Condition condition in conditions)
			{
				if (condition.GetType() == typeof(NameValuesPairConfigurationPredicate) || condition.GetType() == typeof(IsPredicate))
				{
					IsPredicate isPredicate = condition as IsPredicate;
					if (isPredicate != null)
					{
						MultiValuedProperty<Guid> multiValuedProperty = new MultiValuedProperty<Guid>();
						if (isPredicate.Property.Name.Equals("ExclusionList"))
						{
							if (isPredicate.Value.ParsedValue is Guid)
							{
								multiValuedProperty.Add(isPredicate.Value.ParsedValue);
							}
							if (isPredicate.Value.ParsedValue is List<Guid>)
							{
								foreach (string item in ((List<string>)isPredicate.Value.ParsedValue))
								{
									multiValuedProperty.Add(item);
								}
							}
							this.ExclusionList = multiValuedProperty;
						}
					}
					else
					{
						NameValuesPairConfigurationPredicate nameValuesPairConfigurationPredicate = condition as NameValuesPairConfigurationPredicate;
						if (nameValuesPairConfigurationPredicate != null)
						{
							bool value2;
							if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceTenantRule.AccessControl_ResourceScope))
							{
								PolicyResourceScope value;
								if (Enum.TryParse<PolicyResourceScope>(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.ApplyPolicyTo = new PolicyResourceScope?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceTenantRule.AccessControl_AllowActionOnUnsupportedPlatform) && bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value2))
							{
								this.BlockUnsupportedDevices = new bool?(value2);
							}
						}
					}
				}
			}
		}

		public static string AccessControl_ResourceScope = "AccessControl_ResourceScope";

		public static string AccessControl_AllowActionOnUnsupportedPlatform = "AccessControl_AllowActionOnUnsupportedPlatform";
	}
}
