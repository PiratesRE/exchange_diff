using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data.Storage.Clutter;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ActivityLogHelper
	{
		public static bool IsActivityLoggingEnabled(bool forceRefresh = false)
		{
			if (ActivityLogHelper.isActivityLoggingEnabled == null || forceRefresh)
			{
				bool? activityLoggingRegkeyOverride = ActivityLogHelper.GetActivityLoggingRegkeyOverride();
				if (activityLoggingRegkeyOverride != null)
				{
					ActivityLogHelper.isActivityLoggingEnabled = new bool?(activityLoggingRegkeyOverride.Value);
				}
				else
				{
					ActivityLogHelper.isActivityLoggingEnabled = new bool?(VariantConfiguration.InvariantNoFlightingSnapshot.Inference.ActivityLogging.Enabled);
				}
			}
			return ActivityLogHelper.isActivityLoggingEnabled.Value;
		}

		internal static bool? GetActivityLoggingRegkeyOverride()
		{
			bool? result = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference", RegistryKeyPermissionCheck.ReadSubTree))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue("ActivityLoggingEnabled");
						if (value != null && value is int)
						{
							result = new bool?((int)value == 1);
						}
					}
				}
			}
			catch (IOException arg)
			{
				ExTraceGlobals.SessionTracer.TraceDebug(0L, string.Format("Could not read registry. Exception: {0}", arg));
			}
			return result;
		}

		public static IEnumerable<Activity> FixupItemIds(IEnumerable<Activity> activities)
		{
			return ActivityLogHelper.FixupItemIds(activities, false);
		}

		public static IEnumerable<Activity> FixupItemIds(IEnumerable<Activity> activities, bool returnItemlessActivities)
		{
			Util.ThrowOnNullArgument(activities, "activities");
			IEnumerable<Activity> moveActivities = from activity in activities
			where activity.Id == ActivityId.Move && activity.ItemId != null && activity.PreviousItemId != null
			select activity;
			moveActivities = from activity in moveActivities
			orderby activity.TimeStamp descending
			select activity;
			Dictionary<StoreObjectId, StoreObjectId> fixupDictionary = new Dictionary<StoreObjectId, StoreObjectId>();
			foreach (Activity activity2 in moveActivities)
			{
				StoreObjectId value;
				if (fixupDictionary.TryGetValue(activity2.ItemId, out value))
				{
					fixupDictionary[activity2.PreviousItemId] = value;
				}
				else
				{
					fixupDictionary[activity2.PreviousItemId] = activity2.ItemId;
				}
			}
			foreach (Activity activity3 in activities)
			{
				if (activity3.ItemId != null)
				{
					StoreObjectId latestItemId;
					if (fixupDictionary.TryGetValue(activity3.ItemId, out latestItemId))
					{
						activity3.ItemId = latestItemId;
					}
					yield return activity3;
				}
				else if (returnItemlessActivities)
				{
					yield return activity3;
				}
			}
			yield break;
		}

		internal static bool CatchNonFatalExceptions(Action action, string diagnosticsDataToLog = null)
		{
			bool result = false;
			try
			{
				action();
			}
			catch (Exception ex)
			{
				if (ex is OutOfMemoryException || ex is StackOverflowException || ex is ThreadAbortException)
				{
					throw;
				}
				result = true;
				diagnosticsDataToLog = (diagnosticsDataToLog ?? string.Empty);
				if (!(ex is StoragePermanentException) && !(ex is StorageTransientException))
				{
					ReportOptions options = ActivityLoggingConfig.Instance.IsDumpCollectionEnabled ? ReportOptions.None : ReportOptions.DoNotCollectDumps;
					ExWatson.SendReport(ex, options, diagnosticsDataToLog);
				}
				InferenceDiagnosticsLog.Log("CatchNonFatalExceptions", new List<string>
				{
					diagnosticsDataToLog,
					ex.ToString()
				});
			}
			return result;
		}

		internal static IDictionary<string, string> ExtractDeliveryDetails(StoreSession session, Item item)
		{
			IDictionary<string, string> customProperties = null;
			ActivityLogHelper.CatchNonFatalExceptions(delegate
			{
				MessageItem messageItem = item as MessageItem;
				if (messageItem == null)
				{
					return;
				}
				bool flag = false;
				bool flag2 = false;
				string value = null;
				ConversationId conversationId = null;
				string value2 = null;
				bool? flag3 = null;
				if (((IDirectPropertyBag)messageItem.PropertyBag).IsLoaded(InternalSchema.InferenceClassificationResult))
				{
					InferenceClassificationResult? valueAsNullable = messageItem.GetValueAsNullable<InferenceClassificationResult>(InternalSchema.InferenceClassificationResult);
					flag3 = new bool?(valueAsNullable != null && valueAsNullable.Value.HasFlag(InferenceClassificationResult.IsClutterFinal));
				}
				string value3 = string.Empty;
				if (flag3 != null)
				{
					value3 = (flag3.Value ? bool.TrueString : bool.FalseString);
				}
				bool? flag4 = new bool?(messageItem.IsGroupEscalationMessage);
				string value4 = string.Empty;
				if (flag4 != null)
				{
					value4 = (flag4.Value ? bool.TrueString : bool.FalseString);
				}
				StoreObjectId parentId = messageItem.ParentId;
				DefaultFolderType defaultFolderType = DefaultFolderType.None;
				bool flag5 = false;
				if (parentId != null)
				{
					StoreObjectId defaultFolderId = session.GetDefaultFolderId(DefaultFolderType.Inbox);
					flag = parentId.Equals(defaultFolderId);
					StoreObjectId defaultFolderId2 = session.GetDefaultFolderId(DefaultFolderType.Clutter);
					flag2 = (defaultFolderId2 != null && parentId.Equals(defaultFolderId2));
					MailboxSession mailboxSession = session as MailboxSession;
					if (mailboxSession != null)
					{
						defaultFolderType = mailboxSession.IsDefaultFolderType(parentId);
						flag5 = true;
					}
				}
				value = messageItem.InternetMessageId;
				ConversationIndex index;
				if (ConversationIndex.TryCreate(messageItem.ConversationIndex, out index))
				{
					conversationId = ConversationId.Create(index);
				}
				string value5 = string.Empty;
				if (((IDirectPropertyBag)messageItem.PropertyBag).IsLoaded(InternalSchema.InferenceMessageIdentifier))
				{
					value5 = messageItem.GetValueAsNullable<Guid>(InternalSchema.InferenceMessageIdentifier).ToString();
				}
				if (messageItem.Sender != null)
				{
					value2 = messageItem.Sender.SmtpEmailAddress;
				}
				List<string> list = null;
				List<string> list2 = null;
				foreach (Recipient recipient in messageItem.Recipients)
				{
					if (recipient.Participant != null)
					{
						if (recipient.IsDistributionList() == true)
						{
							if (list == null)
							{
								list = new List<string>(2);
							}
							list.Add(recipient.SmtpAddress());
						}
						else if (recipient.IsGroupMailbox() == true)
						{
							if (list2 == null)
							{
								list2 = new List<string>(2);
							}
							list2.Add(recipient.SmtpAddress());
						}
					}
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{
						"IsClutter",
						value3
					},
					{
						"DeliveredToInbox",
						flag ? bool.TrueString : bool.FalseString
					},
					{
						"DeliveredToClutter",
						flag2 ? bool.TrueString : bool.FalseString
					},
					{
						"IsGroupEscalationMessage",
						value4
					},
					{
						"InternetMessageId",
						value
					},
					{
						"ConversationId",
						(conversationId == null) ? string.Empty : conversationId.ToString()
					},
					{
						"MessageGuid",
						value5
					},
					{
						"SenderSmtpAddress",
						value2
					}
				};
				if (list != null && list.Count > 0)
				{
					dictionary.Add("DLRecipients", string.Join(";", list));
				}
				if (list2 != null)
				{
					dictionary.Add("GMRecipients", string.Join(";", list2));
				}
				if (flag5)
				{
					Dictionary<string, string> dictionary2 = dictionary;
					string key = "DeliveredFolderType";
					int num = (int)defaultFolderType;
					dictionary2.Add(key, num.ToString());
					if (defaultFolderType == DefaultFolderType.None && parentId != null)
					{
						dictionary.Add("DeliveredFolderId", parentId.ToString());
					}
				}
				customProperties = dictionary;
			}, null);
			return customProperties;
		}

		internal const string ActivityLoggingRegkeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference";

		internal const string ActivityLoggingRegkeyValueName = "ActivityLoggingEnabled";

		private static bool? isActivityLoggingEnabled = null;
	}
}
