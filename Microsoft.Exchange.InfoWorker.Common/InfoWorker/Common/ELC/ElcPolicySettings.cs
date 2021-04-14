using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.ELC;
using Microsoft.Exchange.InfoWorker.EventLog;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal sealed class ElcPolicySettings
	{
		internal ElcPolicySettings(ContentSetting contentSettings, string messageClass)
		{
			this.contentSettings = contentSettings;
			this.messageClass = messageClass;
		}

		public EnhancedTimeSpan? AgeLimitForRetention
		{
			get
			{
				return this.ContentSettings.AgeLimitForRetention;
			}
		}

		public RetentionActionType RetentionAction
		{
			get
			{
				return this.ContentSettings.RetentionAction;
			}
		}

		public RetentionDateType TriggerForRetention
		{
			get
			{
				return this.ContentSettings.TriggerForRetention;
			}
		}

		internal ContentSetting ContentSettings
		{
			get
			{
				return this.contentSettings;
			}
			set
			{
				this.contentSettings = value;
			}
		}

		internal string MessageClass
		{
			get
			{
				return this.messageClass;
			}
			set
			{
				this.messageClass = value;
			}
		}

		internal bool JournalingEnabled
		{
			get
			{
				return this.ContentSettings.JournalingEnabled;
			}
		}

		internal bool RetentionEnabled
		{
			get
			{
				return this.ContentSettings.RetentionEnabled;
			}
		}

		internal string Name
		{
			get
			{
				return this.ContentSettings.Name;
			}
		}

		internal static void ParseContentSettings(List<ElcPolicySettings> policyList, ContentSetting elcContentSetting)
		{
			if (ElcMessageClass.IsMultiMessageClass(elcContentSetting.MessageClass))
			{
				string[] array = elcContentSetting.MessageClass.Split(ElcMessageClass.MessageClassDelims, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					policyList.Add(new ElcPolicySettings(elcContentSetting, text));
				}
				return;
			}
			policyList.Add(new ElcPolicySettings(elcContentSetting, elcContentSetting.MessageClass));
		}

		internal static ContentSetting GetApplyingPolicy(List<ElcPolicySettings> elcPolicies, string itemClass, Dictionary<string, ContentSetting> itemClassToPolicyMapping)
		{
			return ElcPolicySettings.GetApplyingPolicy(elcPolicies, itemClass, itemClassToPolicyMapping, itemClass);
		}

		internal static ContentSetting GetApplyingPolicy(IEnumerable<ElcPolicySettings> elcPolicies, string itemClass, Dictionary<string, ContentSetting> itemClassToPolicyMapping, string cacheKey)
		{
			if (itemClassToPolicyMapping.ContainsKey(cacheKey))
			{
				return itemClassToPolicyMapping[cacheKey];
			}
			if (elcPolicies == null)
			{
				return null;
			}
			string effectiveItemClass = ElcPolicySettings.GetEffectiveItemClass(itemClass);
			ContentSetting contentSetting = null;
			foreach (ElcPolicySettings elcPolicySettings in elcPolicies)
			{
				if (elcPolicySettings.MessageClass == "*")
				{
					contentSetting = elcPolicySettings.ContentSettings;
				}
				else if (!string.IsNullOrEmpty(effectiveItemClass))
				{
					if (string.Compare(itemClass, elcPolicySettings.MessageClass, StringComparison.OrdinalIgnoreCase) == 0)
					{
						contentSetting = elcPolicySettings.ContentSettings;
						break;
					}
					if (elcPolicySettings.MessageClass.EndsWith("*") && effectiveItemClass.StartsWith(elcPolicySettings.MessageClass.TrimEnd(new char[]
					{
						'*'
					}), StringComparison.OrdinalIgnoreCase))
					{
						contentSetting = elcPolicySettings.ContentSettings;
						break;
					}
				}
			}
			itemClassToPolicyMapping[cacheKey] = contentSetting;
			return contentSetting;
		}

		internal static string GetEffectiveItemClass(string itemClass)
		{
			string result = itemClass;
			if (string.Compare(itemClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA", StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = "IPM.Note.Microsoft.Voicemail.UM.CA";
			}
			else if (string.Compare(itemClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = "IPM.Note.Microsoft.Voicemail.UM";
			}
			return result;
		}

		internal static bool ArePolicyPropertiesValid(ContentSetting policy, object objectToTrace, string mailboxAddress)
		{
			if (policy == null)
			{
				ElcPolicySettings.Tracer.TraceDebug(0L, "{0}: Policy is null.", new object[]
				{
					objectToTrace
				});
				return false;
			}
			if (!policy.RetentionEnabled)
			{
				ElcPolicySettings.Tracer.TraceDebug<object, string>(0L, "{0}: Policy '{1}' is disabled.", objectToTrace, policy.Name);
				return false;
			}
			EnhancedTimeSpan? ageLimitForRetention = policy.AgeLimitForRetention;
			TimeSpan? timeSpan = (ageLimitForRetention != null) ? new TimeSpan?(ageLimitForRetention.GetValueOrDefault()) : null;
			if (timeSpan == null || timeSpan.Value.TotalDays <= 0.0)
			{
				ElcPolicySettings.Tracer.TraceDebug<object, string>(0L, "{0}: Age limit of Policy '{1}' is not greater than 0.", objectToTrace, policy.Name);
				Globals.ELCLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_InvalidRetentionAgeLimit, "Managed Folder Assistant Run", new object[]
				{
					policy.Name,
					mailboxAddress
				});
				return false;
			}
			return true;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ELCAssistantTracer;

		private static readonly Trace TracerPfd = ExTraceGlobals.PFDTracer;

		private ContentSetting contentSettings;

		private string messageClass;
	}
}
