using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class Rule
	{
		public Rule(string name) : this(name, null)
		{
		}

		public Rule(string name, Condition condition)
		{
			this.name = name;
			this.condition = condition;
			this.actions = new ShortList<Action>();
			this.Enabled = RuleState.Enabled;
			this.SubType = RuleSubType.None;
			this.isTooAdvancedToParse = false;
			this.expiryDate = null;
			this.activationDate = null;
			this.Mode = RuleMode.Enforce;
			this.ErrorAction = RuleErrorAction.Ignore;
			this.WhenChangedUTC = null;
		}

		public Condition Condition
		{
			get
			{
				return this.condition;
			}
			set
			{
				this.condition = value;
			}
		}

		public ShortList<Action> Actions
		{
			get
			{
				return this.actions;
			}
			set
			{
				this.actions = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public Guid ImmutableId
		{
			get
			{
				return this.immutableId;
			}
			set
			{
				this.immutableId = value;
			}
		}

		public DateTime? WhenChangedUTC
		{
			get
			{
				return this.whenChangedUTC;
			}
			set
			{
				this.whenChangedUTC = value;
			}
		}

		public string Comments
		{
			get
			{
				return this.comments;
			}
			set
			{
				this.comments = value;
			}
		}

		public RuleState Enabled { get; set; }

		public RuleSubType SubType { get; set; }

		public bool IsTooAdvancedToParse
		{
			get
			{
				return this.isTooAdvancedToParse;
			}
			set
			{
				this.isTooAdvancedToParse = value;
			}
		}

		public DateTime? ExpiryDate
		{
			get
			{
				return this.expiryDate;
			}
			set
			{
				this.expiryDate = value;
			}
		}

		public DateTime? ActivationDate
		{
			get
			{
				return this.activationDate;
			}
			set
			{
				this.activationDate = value;
			}
		}

		public virtual Version MinimumVersion
		{
			get
			{
				if (this.ActivationDate != null)
				{
					return Rule.ActivationAndExpiryDateVersion;
				}
				if (this.ExpiryDate != null)
				{
					return Rule.ActivationAndExpiryDateVersion;
				}
				if (this.tags.Any<KeyValuePair<string, List<RuleTag>>>())
				{
					return Rule.RuleTagsVersion;
				}
				return Rule.BaseVersion;
			}
		}

		public RuleMode Mode { get; set; }

		public RuleErrorAction ErrorAction { get; set; }

		public int ConditionAndActionSize { get; set; }

		public bool SupportGetEstimatedSize { get; set; }

		public virtual int GetEstimatedSize()
		{
			if (!this.SupportGetEstimatedSize)
			{
				throw new InvalidOperationException("GetEstimatedSize currently is only supported when the rule is parsed from XML data");
			}
			int num = 0;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			if (!string.IsNullOrEmpty(this.name))
			{
				num += this.name.Length * 2;
				num += 18;
			}
			if (!string.IsNullOrEmpty(this.comments))
			{
				num += this.comments.Length * 2;
				num += 18;
			}
			num += 16;
			num += 24;
			num += 2;
			if (this.tags != null)
			{
				num += 18;
				foreach (KeyValuePair<string, List<RuleTag>> keyValuePair in this.tags)
				{
					num += keyValuePair.Key.Length * 2;
					foreach (RuleTag ruleTag in keyValuePair.Value)
					{
						num += ruleTag.Size;
					}
				}
			}
			return num + 18 + this.ConditionAndActionSize;
		}

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
			if (this.condition != null)
			{
				this.condition.GetSupplementalData(data);
			}
		}

		private Condition condition;

		private ShortList<Action> actions;

		private string name;

		private Guid immutableId;

		private string comments;

		private DateTime? activationDate;

		private bool isTooAdvancedToParse;

		private DateTime? expiryDate;

		private DateTime? whenChangedUTC;

		private readonly Dictionary<string, List<RuleTag>> tags = new Dictionary<string, List<RuleTag>>();

		public static readonly Version BaseVersion = new Version("14.00.0000.000");

		public static readonly Version RuleTagsVersion = new Version("15.00.0001.000");

		public static readonly Version ActivationAndExpiryDateVersion = new Version("15.00.0002.000");

		public static readonly Version BaseVersion15 = new Version("15.00.0000.000");

		public static readonly Version HighestHonoredVersion = new Version("15.00.0015.00");
	}
}
