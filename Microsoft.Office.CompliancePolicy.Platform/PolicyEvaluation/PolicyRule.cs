using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class PolicyRule
	{
		public PolicyRule()
		{
			this.InitializeProperties();
		}

		public Condition Condition { get; set; }

		public IList<Action> Actions { get; set; }

		public string Name { get; set; }

		public Guid ImmutableId { get; set; }

		public string Comments { get; set; }

		public RuleState Enabled { get; set; }

		public Version MinimumVersion
		{
			get
			{
				Version version = this.Condition.MinimumVersion;
				foreach (Action action in this.Actions)
				{
					Version minimumVersion = action.MinimumVersion;
					if (version < minimumVersion)
					{
						version = minimumVersion;
					}
				}
				return version;
			}
		}

		public DateTime? WhenChangedUtc { get; set; }

		public bool IsTooAdvancedToParse { get; set; }

		public DateTime? ExpiryDate { get; set; }

		public DateTime? ActivationDate { get; set; }

		public RuleMode Mode { get; set; }

		public IEnumerable<RuleTag> GetTags()
		{
			return this.tags.SelectMany((KeyValuePair<string, List<RuleTag>> tagItem) => tagItem.Value);
		}

		public IEnumerable<RuleTag> GetTags(string tagType)
		{
			List<RuleTag> result;
			if (this.tags.TryGetValue(tagType, out result))
			{
				return result;
			}
			return new List<RuleTag>();
		}

		public void AddTag(RuleTag tag)
		{
			List<RuleTag> list;
			if (this.tags.TryGetValue(tag.TagType, out list))
			{
				list.Add(tag);
				return;
			}
			this.tags.Add(tag.TagType, new List<RuleTag>
			{
				tag
			});
		}

		public void RemoveAllTags(string tagType)
		{
			this.tags.Remove(tagType);
		}

		public void GetSupplementalData(SupplementalData data)
		{
			if (this.Condition != null)
			{
				this.Condition.GetSupplementalData(data);
			}
		}

		private void InitializeProperties()
		{
			this.Condition = null;
			this.Actions = new List<Action>();
			this.Name = string.Empty;
			this.ImmutableId = Guid.NewGuid();
			this.Comments = string.Empty;
			this.Enabled = RuleState.Enabled;
			this.IsTooAdvancedToParse = false;
			this.Mode = RuleMode.Enforce;
		}

		public static readonly Version BaseVersion = new Version("1.00.0000.000");

		public static readonly Version HighestHonoredVersion = new Version("1.00.0002.000");

		private readonly Dictionary<string, List<RuleTag>> tags = new Dictionary<string, List<RuleTag>>();
	}
}
