using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	public class AuditUploaderConfigRules
	{
		public int Count
		{
			get
			{
				return this.auditUploaderRules.Count;
			}
		}

		public Actions GetOperation(string component, string tenant, string user, string operation, DateTime currentTime)
		{
			AuditUploaderDictionaryKey currentKey = new AuditUploaderDictionaryKey(component, tenant, user, operation);
			int num = 0;
			while ((double)num < Math.Pow((double)AuditUploaderDictionaryKey.NumberOfFields, 2.0))
			{
				AuditUploaderDictionaryKey nextKey = this.GetNextKey(currentKey, num);
				if (this.auditUploaderRules == null)
				{
					EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_InvalidRuleCollection, "Audit Uploader rules collection has not been generated yet.", new object[0]);
					return Actions.Skip;
				}
				if (this.auditUploaderRules.ContainsKey(nextKey))
				{
					TimeSpan? actionThrottlingInterval = this.auditUploaderRules[nextKey].ActionThrottlingInterval;
					if (actionThrottlingInterval == null || currentTime.Subtract(actionThrottlingInterval.Value).CompareTo(this.auditUploaderRules[nextKey].LastTriggerDate) > 0)
					{
						this.auditUploaderRules[nextKey].LastTriggerDate = currentTime;
						return this.auditUploaderRules[nextKey].ActionToPerform;
					}
					return Actions.LetThrough;
				}
				else
				{
					num++;
				}
			}
			EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_InvalidRuleCollection, "Audit Uploader rules collection does not contain a matching rule for the given entry.", new object[0]);
			return Actions.LetThrough;
		}

		public void Add(AuditUploaderDictionaryKey key, AuditUploaderAction action)
		{
			this.auditUploaderRules.Add(key, action);
		}

		public bool Contains(AuditUploaderDictionaryKey key)
		{
			return this.auditUploaderRules.ContainsKey(key);
		}

		private AuditUploaderDictionaryKey GetNextKey(AuditUploaderDictionaryKey currentKey, int index)
		{
			this.tempKey.CopyFrom(currentKey);
			for (int i = 0; i < AuditUploaderDictionaryKey.NumberOfFields; i++)
			{
				if ((index >> i & 1) == 1)
				{
					this.tempKey[i] = AuditUploaderDictionaryKey.WildCard;
				}
			}
			return this.tempKey;
		}

		private Dictionary<AuditUploaderDictionaryKey, AuditUploaderAction> auditUploaderRules = new Dictionary<AuditUploaderDictionaryKey, AuditUploaderAction>();

		private AuditUploaderDictionaryKey tempKey = new AuditUploaderDictionaryKey(AuditUploaderDictionaryKey.WildCard, AuditUploaderDictionaryKey.WildCard, AuditUploaderDictionaryKey.WildCard, AuditUploaderDictionaryKey.WildCard);
	}
}
