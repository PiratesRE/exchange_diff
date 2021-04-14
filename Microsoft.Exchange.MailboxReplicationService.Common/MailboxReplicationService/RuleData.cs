using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[KnownType(typeof(RuleActionForwardData))]
	[KnownType(typeof(RuleActionDelegateData))]
	[KnownType(typeof(RuleActionTagData))]
	[DataContract]
	[KnownType(typeof(RuleActionBounceData))]
	[KnownType(typeof(RuleActionCopyData))]
	[KnownType(typeof(RuleActionDeferData))]
	[KnownType(typeof(RuleActionDeleteData))]
	[KnownType(typeof(RuleActionExternalCopyData))]
	[KnownType(typeof(RuleActionExternalMoveData))]
	[KnownType(typeof(RuleActionInMailboxCopyData))]
	[KnownType(typeof(RuleActionInMailboxMoveData))]
	[KnownType(typeof(RuleActionMarkAsReadData))]
	[KnownType(typeof(RuleActionMoveData))]
	[KnownType(typeof(RuleActionOOFReplyData))]
	[KnownType(typeof(RuleActionReplyData))]
	internal sealed class RuleData
	{
		public RuleData()
		{
		}

		[DataMember]
		public int ExecutionSequence { get; set; }

		[DataMember]
		public int Level { get; set; }

		[DataMember]
		public uint StateFlags { get; set; }

		[DataMember]
		public uint UserFlags { get; set; }

		[DataMember]
		public RestrictionData Condition { get; set; }

		[DataMember]
		public RuleActionData[] Actions { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Provider { get; set; }

		[DataMember]
		public byte[] ProviderData { get; set; }

		[DataMember]
		public bool IsExtended { get; set; }

		[DataMember]
		public PropValueData[] ExtraProperties { get; set; }

		private RuleData(Rule rule)
		{
			this.ExecutionSequence = rule.ExecutionSequence;
			this.Level = rule.Level;
			this.StateFlags = (uint)rule.StateFlags;
			this.UserFlags = rule.UserFlags;
			this.Condition = DataConverter<RestrictionConverter, Restriction, RestrictionData>.GetData(rule.Condition);
			this.Actions = DataConverter<RuleActionConverter, RuleAction, RuleActionData>.GetData(rule.Actions);
			this.Name = rule.Name;
			this.Provider = rule.Provider;
			this.ProviderData = rule.ProviderData;
			this.IsExtended = rule.IsExtended;
			if (rule.IsExtended)
			{
				this.ExtraProperties = DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(rule.ExtraProperties);
				return;
			}
			if (rule.ExtraProperties != null && rule.ExtraProperties.Length > 0)
			{
				MrsTracer.Common.Error("Non-extended rule '{0}' has {1} extra properties", new object[]
				{
					rule.Name,
					rule.ExtraProperties.Length
				});
			}
		}

		public static RuleData Create(Rule rule)
		{
			if (rule == null)
			{
				return null;
			}
			return new RuleData(rule);
		}

		public static void ConvertRulesToUplevel(RuleData[] rules, Func<byte[], bool> isFolderLocal)
		{
			if (rules != null)
			{
				foreach (RuleData ruleData in rules)
				{
					if (ruleData.Actions != null)
					{
						for (int j = 0; j < ruleData.Actions.Length; j++)
						{
							RuleActionMoveCopyData ruleActionMoveCopyData = ruleData.Actions[j] as RuleActionMoveCopyData;
							if (ruleActionMoveCopyData != null)
							{
								bool folderIsLocal = isFolderLocal(ruleActionMoveCopyData.FolderEntryID);
								ruleData.Actions[j] = RuleActionMoveCopyData.ConvertToUplevel(ruleActionMoveCopyData, folderIsLocal);
							}
						}
					}
				}
			}
		}

		public static void ConvertRulesToDownlevel(RuleData[] rules)
		{
			if (rules != null)
			{
				foreach (RuleData ruleData in rules)
				{
					if (ruleData.Actions != null)
					{
						for (int j = 0; j < ruleData.Actions.Length; j++)
						{
							RuleActionMoveCopyData ruleActionMoveCopyData = ruleData.Actions[j] as RuleActionMoveCopyData;
							if (ruleActionMoveCopyData != null)
							{
								ruleData.Actions[j] = RuleActionMoveCopyData.ConvertToDownlevel(ruleActionMoveCopyData);
							}
						}
					}
				}
			}
		}

		public void Enumerate(CommonUtils.EnumPropTagDelegate propTagEnumerator, CommonUtils.EnumPropValueDelegate propValueEnumerator, CommonUtils.EnumAdrEntryDelegate adrEntryEnumerator)
		{
			if (this.Condition != null)
			{
				if (propTagEnumerator != null)
				{
					this.Condition.EnumeratePropTags(propTagEnumerator);
				}
				if (propValueEnumerator != null)
				{
					this.Condition.EnumeratePropValues(propValueEnumerator);
				}
			}
			if (this.Actions != null)
			{
				foreach (RuleActionData ruleActionData in this.Actions)
				{
					ruleActionData.Enumerate(propTagEnumerator, propValueEnumerator, adrEntryEnumerator);
				}
			}
			if (this.ExtraProperties != null)
			{
				foreach (PropValueData propValueData in this.ExtraProperties)
				{
					if (propTagEnumerator != null)
					{
						int propTag = propValueData.PropTag;
						propTagEnumerator(ref propTag);
						propValueData.PropTag = propTag;
					}
					if (propValueEnumerator != null)
					{
						propValueEnumerator(propValueData);
					}
				}
			}
		}

		public Rule GetRule()
		{
			Rule rule = new Rule();
			rule.ExecutionSequence = this.ExecutionSequence;
			rule.Level = this.Level;
			rule.StateFlags = (RuleStateFlags)this.StateFlags;
			rule.UserFlags = this.UserFlags;
			rule.Condition = DataConverter<RestrictionConverter, Restriction, RestrictionData>.GetNative(this.Condition);
			rule.Actions = DataConverter<RuleActionConverter, RuleAction, RuleActionData>.GetNative(this.Actions);
			rule.Name = this.Name;
			rule.Provider = this.Provider;
			rule.ProviderData = this.ProviderData;
			rule.IsExtended = this.IsExtended;
			if (this.IsExtended)
			{
				rule.ExtraProperties = DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(this.ExtraProperties);
			}
			else if (this.ExtraProperties != null && this.ExtraProperties.Length > 0)
			{
				MrsTracer.Common.Error("Non-extended rule '{0}' has {1} extra properties", new object[]
				{
					this.Name,
					this.ExtraProperties.Length
				});
			}
			return rule;
		}

		public override string ToString()
		{
			return string.Format("Rule: Condition: {0}; Actions: {1}; Name '{2}'; Provider: '{3}'; ProviderData: {4}; ExecutionSequence: {5}; Level: {6}; StateFlags: {7}; UserFlags: {8}; IsExtended: {9}", new object[]
			{
				(this.Condition == null) ? "none" : this.Condition.ToString(),
				CommonUtils.ConcatEntries<RuleActionData>(this.Actions, null),
				this.Name,
				this.Provider,
				TraceUtils.DumpBytes(this.ProviderData),
				this.ExecutionSequence,
				this.Level,
				this.StateFlags,
				this.UserFlags,
				this.IsExtended
			});
		}
	}
}
