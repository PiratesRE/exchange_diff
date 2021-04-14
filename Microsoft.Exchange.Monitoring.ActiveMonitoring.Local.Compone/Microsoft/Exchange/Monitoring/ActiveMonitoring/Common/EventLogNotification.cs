using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class EventLogNotification : IDisposable
	{
		private EventLogNotification()
		{
			TimerCallback callback = new TimerCallback(this.SendPeriodicGreenEvents);
			this.greenResetTimer = new Timer(callback, null, EventLogNotification.TimerInterval, EventLogNotification.TimerInterval);
			new EventLogNotification.EventNotificationMetadata
			{
				ServiceName = "EventLogNofiticationDiag",
				ComponentName = "InstanceStart",
				TagName = null,
				Message = string.Format("Instance started at {0}", DateTime.UtcNow.ToString("o"))
			}.GenerateEventNotificationItem().Publish(false);
		}

		static EventLogNotification()
		{
			EventLogNotification.LogDebug("Instance initialized", new object[0]);
		}

		private static TimeSpan TimerInterval
		{
			get
			{
				if (EventLogNotification.timerInterval == TimeSpan.Zero)
				{
					EventLogNotification.timerInterval = TimeSpan.FromSeconds((double)RegistryHelper.GetProperty<int>("ProcessorTimerIntervalInSecs", 300, "EventProcessor", null, false));
				}
				return EventLogNotification.timerInterval;
			}
		}

		public static EventLogNotification Instance
		{
			get
			{
				return EventLogNotification.instance;
			}
		}

		public static string ConstructResultMask(string subscriptionName, string resourceName = null)
		{
			return NotificationItem.GenerateResultName("EventLogNotification", subscriptionName, string.IsNullOrWhiteSpace(resourceName) ? resourceName : resourceName.ToLower());
		}

		private static void LogDebug(string pattern, params object[] arguments)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, TracingContext.Default, pattern, arguments, null, "LogDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\EventLogNotification\\EventLogNotification.cs", 214);
		}

		private static string ConstructXPathForEventMatches(IEnumerable<EventLogNotification.EventMatchInternal> matches)
		{
			SortedSet<int> sortedSet = new SortedSet<int>();
			if (matches != null)
			{
				foreach (EventLogNotification.EventMatchInternal eventMatchInternal in matches)
				{
					foreach (int item in eventMatchInternal.EventMatch.EventIds)
					{
						sortedSet.Add(item);
					}
				}
			}
			if (sortedSet.Count < 1)
			{
				return string.Empty;
			}
			List<Tuple<int, int>> sparseRanges = EventLogNotification.SparseRange.GetSparseRanges(sortedSet.ToList<int>(), 6);
			List<string> filterString = new List<string>();
			sparseRanges.ForEach(delegate(Tuple<int, int> r)
			{
				filterString.Add(string.Format("(EventID >= {0} and EventID <= {1})", r.Item1, r.Item2));
			});
			return string.Format("*[System[{0}]]", string.Join(" or ", filterString));
		}

		public void AddSubscription(EventLogSubscription subscription)
		{
			this.CheckDisposed(true);
			string contentHash = subscription.GetContentHash();
			bool flag = false;
			try
			{
				this.rwLockForSubscription.EnterUpgradeableReadLock();
				flag = true;
				bool flag2 = false;
				if (this.subscriptionHashcodes.ContainsKey(subscription.Name))
				{
					if (this.subscriptionHashcodes[subscription.Name].Equals(contentHash))
					{
						EventLogNotification.LogDebug("Subscription named '{0}' already exists. Hashcode={1}", new object[]
						{
							subscription.Name,
							contentHash
						});
						return;
					}
					EventLogNotification.LogDebug("Subscription of same name encountered, replacing existing one... Name={0}", new object[]
					{
						subscription.Name
					});
					flag2 = true;
				}
				HashSet<string> hashSet = new HashSet<string>();
				bool flag3 = false;
				try
				{
					this.rwLockForSubscription.EnterWriteLock();
					flag3 = true;
					this.subscriptionHashcodes[subscription.Name] = contentHash;
					this.allSubscriptionObjects.AddOrUpdate(subscription.Name, subscription, (string key, EventLogSubscription oldValue) => subscription);
					EventLogNotification.EventMatchInternal[] array = EventLogNotification.EventMatchInternal.ConstructFromSubscription(subscription);
					EventLogNotification.SubscriptionMetadata value = EventLogNotification.SubscriptionMetadata.ConstructFromSubscription(subscription);
					if (flag2)
					{
						foreach (KeyValuePair<string, LinkedList<EventLogNotification.EventMatchInternal>> keyValuePair in this.eventMatchByLogName)
						{
							LinkedList<EventLogNotification.EventMatchInternal> value2 = keyValuePair.Value;
							LinkedListNode<EventLogNotification.EventMatchInternal> next;
							for (LinkedListNode<EventLogNotification.EventMatchInternal> linkedListNode = value2.First; linkedListNode != null; linkedListNode = next)
							{
								next = linkedListNode.Next;
								if (linkedListNode.Value != null && linkedListNode.Value.SubscriptionName.Equals(subscription.Name))
								{
									value2.Remove(linkedListNode);
								}
							}
						}
					}
					this.metadataByName[subscription.Name] = value;
					foreach (EventLogNotification.EventMatchInternal eventMatchInternal in array)
					{
						if (eventMatchInternal != null)
						{
							hashSet.Add(eventMatchInternal.EventMatch.LogName);
							LinkedList<EventLogNotification.EventMatchInternal> linkedList = null;
							if (!this.eventMatchByLogName.TryGetValue(eventMatchInternal.EventMatch.LogName, out linkedList))
							{
								linkedList = new LinkedList<EventLogNotification.EventMatchInternal>();
								this.eventMatchByLogName.Add(eventMatchInternal.EventMatch.LogName, linkedList);
							}
							linkedList.AddLast(eventMatchInternal);
						}
					}
				}
				finally
				{
					if (flag3)
					{
						this.rwLockForSubscription.ExitWriteLock();
					}
				}
				this.RefreshWatchers(hashSet);
			}
			finally
			{
				if (flag)
				{
					this.rwLockForSubscription.ExitUpgradeableReadLock();
				}
			}
		}

		public void Dispose()
		{
			if (this.isDisposed)
			{
				return;
			}
			EventLogNotification.LogDebug("Dispose begins.", new object[0]);
			bool flag = false;
			try
			{
				this.rwLockForEventWatchers.EnterWriteLock();
				flag = true;
				if (this.eventWatchersByLogName != null)
				{
					foreach (KeyValuePair<string, EventLogWatcher> keyValuePair in this.eventWatchersByLogName)
					{
						if (keyValuePair.Value != null)
						{
							keyValuePair.Value.Dispose();
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					this.rwLockForEventWatchers.ExitWriteLock();
				}
			}
			if (this.greenResetTimer != null)
			{
				this.greenResetTimer.Dispose();
			}
			lock (this.disposeLock)
			{
				this.isDisposed = true;
			}
			EventLogNotification.LogDebug("Dispose finished.", new object[0]);
		}

		private void RefreshWatchers(IEnumerable<string> logNames)
		{
			if (logNames != null && logNames.Any<string>())
			{
				bool flag = false;
				try
				{
					this.rwLockForEventWatchers.EnterWriteLock();
					flag = true;
					foreach (string text in logNames)
					{
						EventLogWatcher eventLogWatcher = null;
						if (this.eventWatchersByLogName.TryGetValue(text, out eventLogWatcher) && eventLogWatcher != null)
						{
							eventLogWatcher.Enabled = false;
							eventLogWatcher.Dispose();
						}
						eventLogWatcher = this.CreateWatcher(text);
						if (eventLogWatcher != null)
						{
							eventLogWatcher.Enabled = true;
						}
						this.eventWatchersByLogName[text] = eventLogWatcher;
					}
				}
				finally
				{
					if (flag)
					{
						this.rwLockForEventWatchers.ExitWriteLock();
					}
				}
			}
		}

		private EventLogWatcher CreateWatcher(string logName)
		{
			LinkedList<EventLogNotification.EventMatchInternal> matches = null;
			if (!this.eventMatchByLogName.TryGetValue(logName, out matches))
			{
				EventLogNotification.LogDebug("CreateWatcher: Unable to get any matches by logname {0}", new object[]
				{
					logName
				});
				return null;
			}
			string text = EventLogNotification.ConstructXPathForEventMatches(matches);
			EventLogNotification.LogDebug("CreateWatcher: XPath for logname {0}={1}", new object[]
			{
				logName,
				string.IsNullOrWhiteSpace(text) ? "NULL" : text
			});
			if (!string.IsNullOrWhiteSpace(text))
			{
				EventLogWatcher eventLogWatcher = new EventLogWatcher(new EventLogQuery(logName, PathType.LogName, text));
				eventLogWatcher.EventRecordWritten += this.watcherEventRecordWritten;
				return eventLogWatcher;
			}
			EventLogNotification.LogDebug("CreateWatcher: XPath is empty for logName {0}", new object[]
			{
				logName
			});
			return null;
		}

		private void watcherEventRecordWritten(object sender, EventRecordWrittenEventArgs e)
		{
			this.CheckDisposed(true);
			if (e.EventRecord != null)
			{
				using (e.EventRecord)
				{
					EventLogNotification.EventRecordInternal eventRecordInternal = EventLogNotification.EventRecordInternal.ConstructFromEventRecord(e.EventRecord);
					List<EventLogNotification.EventMatchInternal> list = new List<EventLogNotification.EventMatchInternal>();
					bool flag = false;
					try
					{
						this.rwLockForSubscription.EnterReadLock();
						flag = true;
						LinkedList<EventLogNotification.EventMatchInternal> linkedList = null;
						if (!this.eventMatchByLogName.TryGetValue(eventRecordInternal.LogName, out linkedList))
						{
							EventLogNotification.LogDebug("EventRecordWritten: No matches for LogName={0}", new object[]
							{
								eventRecordInternal.LogName
							});
							return;
						}
						if (linkedList != null && linkedList.Count > 0)
						{
							list.AddRange(linkedList);
						}
					}
					finally
					{
						if (flag)
						{
							this.rwLockForSubscription.ExitReadLock();
						}
					}
					if (list != null && list.Count > 0)
					{
						foreach (EventLogNotification.EventMatchInternal eventMatchInternal in list)
						{
							if (eventMatchInternal.IsMatch(eventRecordInternal))
							{
								EventLogNotification.EventNotificationMetadata notification = eventMatchInternal.GetNotification(eventRecordInternal);
								this.SendAndRecordNotification(notification);
								Interlocked.Increment(ref this.eventNotificationSent);
								EventLogSubscription eventLogSubscription = null;
								if (this.allSubscriptionObjects.TryGetValue(eventMatchInternal.SubscriptionName, out eventLogSubscription) && eventLogSubscription != null)
								{
									if (notification.IsCritical && eventLogSubscription.OnRedEvents != null)
									{
										eventLogSubscription.OnRedEvents(eventRecordInternal, notification);
									}
									else if (!notification.IsCritical && eventLogSubscription.OnGreenEvents != null)
									{
										eventLogSubscription.OnGreenEvents(eventRecordInternal, notification);
									}
								}
							}
						}
					}
				}
			}
		}

		private void SendAndRecordNotification(EventLogNotification.EventNotificationMetadata enm)
		{
			string key = string.Format("{0}|{1}|{2}", enm.ServiceName, enm.ComponentName, enm.TagName);
			if (enm.IsCritical)
			{
				this.notificationRecord[key] = DateTime.UtcNow;
			}
			else
			{
				this.notificationRecord[key] = DateTime.MinValue;
			}
			enm.GenerateEventNotificationItem().Publish(false);
		}

		private bool CheckDisposed(bool doThrow = true)
		{
			lock (this.disposeLock)
			{
				if (this.isDisposed)
				{
					if (doThrow)
					{
						throw new ApplicationException("EventLogNotification is already disposed.");
					}
					return true;
				}
			}
			return false;
		}

		private void SendPeriodicGreenEvents(object stateInfo)
		{
			if (this.CheckDisposed(false))
			{
				EventLogNotification.LogDebug("SendPeriodicGreenEvents: Current instance is disposed.", new object[0]);
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (this.notificationRecord != null && this.notificationRecord.Count > 0)
			{
				foreach (KeyValuePair<string, DateTime> keyValuePair in this.notificationRecord)
				{
					string[] array = keyValuePair.Key.Split(EventLogNotification.keySplitter, StringSplitOptions.RemoveEmptyEntries);
					EventLogNotification.SubscriptionMetadata subscriptionMetadata = null;
					if (array != null && array.Length > 0 && this.metadataByName.TryGetValue(array[0], out subscriptionMetadata) && keyValuePair.Value != DateTime.MinValue && subscriptionMetadata.AutoResetInterval != EventLogSubscription.NoAutoReset && utcNow - keyValuePair.Value >= subscriptionMetadata.AutoResetInterval)
					{
						this.SendAndRecordNotification(new EventLogNotification.EventNotificationMetadata
						{
							ServiceName = array[0],
							ComponentName = ((array.Length > 1) ? array[1] : null),
							TagName = ((array.Length > 2) ? array[2] : null),
							IsCritical = false,
							Message = "Auto Reset Green Event."
						});
					}
				}
			}
			new EventLogNotification.EventNotificationMetadata
			{
				ServiceName = "EventLogNofiticationDiag",
				ComponentName = "Heartbeat",
				TagName = null,
				Message = string.Format("Subscription count = {0}, LogName count monitored = {1}, Events Sent={2}", this.subscriptionHashcodes.Count, this.eventWatchersByLogName.Count, this.eventNotificationSent),
				StateAttribute3 = this.eventNotificationSent.ToString()
			}.GenerateEventNotificationItem().Publish(false);
		}

		private const string EventNotificationServiceName = "EventLogNotification";

		private const string EventNotificationServiceNameForDiag = "EventLogNofiticationDiag";

		private const string EventNotificationInstanceStart = "InstanceStart";

		private const string EventNotificationHeartbeatCompName = "Heartbeat";

		private const string EventResetSubkey = "EventProcessor";

		private const string EventResetIntervalValueName = "ProcessorTimerIntervalInSecs";

		private static TimeSpan timerInterval = TimeSpan.Zero;

		private static char[] keySplitter = new char[]
		{
			'|'
		};

		private static EventLogNotification instance = new EventLogNotification();

		private readonly object disposeLock = new object();

		private int eventNotificationSent;

		private bool isDisposed;

		private ReaderWriterLockSlim rwLockForSubscription = new ReaderWriterLockSlim();

		private Dictionary<string, string> subscriptionHashcodes = new Dictionary<string, string>();

		private Dictionary<string, LinkedList<EventLogNotification.EventMatchInternal>> eventMatchByLogName = new Dictionary<string, LinkedList<EventLogNotification.EventMatchInternal>>();

		private Dictionary<string, EventLogNotification.SubscriptionMetadata> metadataByName = new Dictionary<string, EventLogNotification.SubscriptionMetadata>();

		private ReaderWriterLockSlim rwLockForEventWatchers = new ReaderWriterLockSlim();

		private Dictionary<string, EventLogWatcher> eventWatchersByLogName = new Dictionary<string, EventLogWatcher>();

		private ConcurrentDictionary<string, DateTime> notificationRecord = new ConcurrentDictionary<string, DateTime>();

		private ConcurrentDictionary<string, EventLogSubscription> allSubscriptionObjects = new ConcurrentDictionary<string, EventLogSubscription>();

		private Timer greenResetTimer;

		public static class SparseRange
		{
			public static List<Tuple<int, int>> GetSparseRanges(List<int> sortedSet, int maxRanges = 6)
			{
				List<EventLogNotification.SparseRange.Hole> list = new List<EventLogNotification.SparseRange.Hole>();
				List<Tuple<int, int>> list2 = new List<Tuple<int, int>>();
				if (sortedSet == null || sortedSet.Count < 1)
				{
					return new List<Tuple<int, int>>();
				}
				int num = sortedSet.FirstOrDefault<int>();
				int item = sortedSet.LastOrDefault<int>();
				if (sortedSet.Count <= 2)
				{
					list2.Add(new Tuple<int, int>(num, item));
					return list2;
				}
				for (int i = 0; i < sortedSet.Count - 1; i++)
				{
					if (sortedSet[i + 1] - sortedSet[i] > 1)
					{
						list.Add(new EventLogNotification.SparseRange.Hole
						{
							PreviousInteger = sortedSet[i],
							NextInteger = sortedSet[i + 1],
							HoleSize = sortedSet[i + 1] - sortedSet[i]
						});
					}
				}
				if (list.Count < 1)
				{
					list2.Add(new Tuple<int, int>(num, item));
					return list2;
				}
				list.Sort((EventLogNotification.SparseRange.Hole x, EventLogNotification.SparseRange.Hole y) => y.HoleSize.CompareTo(x.HoleSize));
				List<EventLogNotification.SparseRange.Hole> list3 = new List<EventLogNotification.SparseRange.Hole>();
				int num2 = 1;
				foreach (EventLogNotification.SparseRange.Hole item2 in list)
				{
					if (num2 > maxRanges - 1)
					{
						break;
					}
					list3.Add(item2);
					num2++;
				}
				list3.Sort((EventLogNotification.SparseRange.Hole x, EventLogNotification.SparseRange.Hole y) => x.PreviousInteger.CompareTo(y.PreviousInteger));
				int item3 = num;
				foreach (EventLogNotification.SparseRange.Hole hole in list3)
				{
					list2.Add(new Tuple<int, int>(item3, hole.PreviousInteger));
					item3 = hole.NextInteger;
				}
				list2.Add(new Tuple<int, int>(item3, item));
				return list2;
			}

			private class Hole
			{
				public int PreviousInteger;

				public int NextInteger;

				public int HoleSize;
			}
		}

		public class EventRecordInternal
		{
			public string LogName { get; private set; }

			public string ProviderName { get; private set; }

			public int Id { get; private set; }

			public EventRecord EventRecord { get; private set; }

			private EventRecordInternal()
			{
			}

			public static EventLogNotification.EventRecordInternal ConstructFromEventRecord(EventRecord record)
			{
				return new EventLogNotification.EventRecordInternal
				{
					EventRecord = record,
					LogName = record.LogName,
					ProviderName = record.ProviderName,
					Id = record.Id
				};
			}

			public string GeneratePropertyXml()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("<Properties>");
				IList<EventProperty> properties = this.EventRecord.Properties;
				if (properties != null)
				{
					for (int i = 0; i < properties.Count; i++)
					{
						stringBuilder.AppendFormat("<Property index={0}>{1}</Property>{2}", i, (properties[i].Value == null) ? "NULL" : properties[i].Value.ToString(), Environment.NewLine);
					}
				}
				stringBuilder.AppendLine("</Properties>");
				return stringBuilder.ToString();
			}
		}

		public class EventMatchInternal
		{
			public EventMatchingRule EventMatch { get; private set; }

			public bool IsRed { get; private set; }

			public string SubscriptionName { get; private set; }

			private EventMatchInternal()
			{
			}

			public static EventLogNotification.EventMatchInternal[] ConstructFromSubscription(EventLogSubscription subscription)
			{
				EventLogNotification.EventMatchInternal[] array = new EventLogNotification.EventMatchInternal[2];
				array[0] = new EventLogNotification.EventMatchInternal();
				array[0].IsRed = true;
				array[0].EventMatch = subscription.RedEvents;
				array[0].SubscriptionName = subscription.Name;
				if (subscription.GreenEvents != null)
				{
					array[1] = new EventLogNotification.EventMatchInternal();
					array[1].IsRed = false;
					array[1].EventMatch = subscription.GreenEvents;
					array[1].SubscriptionName = subscription.Name;
				}
				else
				{
					array[1] = null;
				}
				return array;
			}

			public bool IsMatch(EventLogNotification.EventRecordInternal record)
			{
				EventMatchingRule eventMatch = this.EventMatch;
				return eventMatch.EventIds.Contains(record.Id) && !string.IsNullOrWhiteSpace(record.LogName) && record.LogName.Equals(eventMatch.LogName, StringComparison.OrdinalIgnoreCase) && (eventMatch.ProviderName.Equals("*") || string.IsNullOrWhiteSpace(record.ProviderName) || record.ProviderName.Equals(eventMatch.ProviderName, StringComparison.OrdinalIgnoreCase)) && (eventMatch.OnMatching == null || eventMatch.OnMatching(record));
			}

			public EventLogNotification.EventNotificationMetadata GetNotification(EventLogNotification.EventRecordInternal record)
			{
				EventLogNotification.EventNotificationMetadata eventNotificationMetadata = new EventLogNotification.EventNotificationMetadata();
				eventNotificationMetadata.ServiceName = "EventLogNotification";
				eventNotificationMetadata.ComponentName = this.SubscriptionName;
				string tagName = "Normal";
				if (this.EventMatch.ResourceNameIndex != -1 && record.EventRecord.Properties != null && record.EventRecord.Properties.Count - 1 >= this.EventMatch.ResourceNameIndex && record.EventRecord.Properties[this.EventMatch.ResourceNameIndex].Value != null)
				{
					tagName = record.EventRecord.Properties[this.EventMatch.ResourceNameIndex].Value.ToString().ToLower();
				}
				eventNotificationMetadata.TagName = tagName;
				eventNotificationMetadata.EventMessage = (this.EventMatch.EvaluateMessage ? record.EventRecord.FormatDescription() : "NotEvaluated");
				eventNotificationMetadata.EventPropertiesXml = (this.EventMatch.PopulatePropertiesXml ? record.GeneratePropertyXml() : "NotEvaluated");
				eventNotificationMetadata.IsCritical = this.IsRed;
				eventNotificationMetadata.Message = string.Format("Event LogName={0}, Provider={1}, Id={2} caught. Message={3}, Properties={4}. IsCritical={5} for Subscription {6}", new object[]
				{
					record.LogName,
					record.ProviderName,
					record.Id,
					eventNotificationMetadata.EventMessage,
					eventNotificationMetadata.EventPropertiesXml,
					eventNotificationMetadata.IsCritical,
					this.SubscriptionName
				});
				if (this.EventMatch.OnNotify != null)
				{
					this.EventMatch.OnNotify(record, ref eventNotificationMetadata);
				}
				return eventNotificationMetadata;
			}
		}

		public class SubscriptionMetadata
		{
			public string Name { get; set; }

			public TimeSpan AutoResetInterval { get; set; }

			private SubscriptionMetadata()
			{
			}

			public static EventLogNotification.SubscriptionMetadata ConstructFromSubscription(EventLogSubscription subscription)
			{
				return new EventLogNotification.SubscriptionMetadata
				{
					Name = subscription.Name,
					AutoResetInterval = subscription.AutoResetInterval
				};
			}
		}

		public class EventNotificationMetadata
		{
			public string ServiceName { get; set; }

			public string ComponentName { get; set; }

			public string TagName { get; set; }

			public bool IsCritical { get; set; }

			public string Message { get; set; }

			public string EventMessage { get; set; }

			public string EventPropertiesXml { get; set; }

			public string StateAttribute3 { get; set; }

			public string StateAttribute4 { get; set; }

			public string StateAttribute5 { get; set; }

			private static string TruncateIfNeeded(string str)
			{
				if (!string.IsNullOrWhiteSpace(str) && str.Length >= 3072)
				{
					return string.Format("{0}...", str.Substring(0, 3069));
				}
				return str;
			}

			public EventNotificationItem GenerateEventNotificationItem()
			{
				string text = this.ComponentName;
				if (string.IsNullOrWhiteSpace(text))
				{
					text = "Normal";
				}
				EventNotificationItem eventNotificationItem = new EventNotificationItem(this.ServiceName, text, this.TagName, this.IsCritical ? ResultSeverityLevel.Critical : ResultSeverityLevel.Informational);
				if (!string.IsNullOrWhiteSpace(this.Message))
				{
					eventNotificationItem.Message = EventLogNotification.EventNotificationMetadata.TruncateIfNeeded(this.Message);
				}
				eventNotificationItem.StateAttribute1 = EventLogNotification.EventNotificationMetadata.TruncateIfNeeded(this.EventMessage);
				eventNotificationItem.StateAttribute2 = EventLogNotification.EventNotificationMetadata.TruncateIfNeeded(this.EventPropertiesXml);
				eventNotificationItem.StateAttribute3 = EventLogNotification.EventNotificationMetadata.TruncateIfNeeded(this.StateAttribute3);
				eventNotificationItem.StateAttribute4 = EventLogNotification.EventNotificationMetadata.TruncateIfNeeded(this.StateAttribute4);
				eventNotificationItem.StateAttribute5 = EventLogNotification.EventNotificationMetadata.TruncateIfNeeded(this.StateAttribute5);
				return eventNotificationItem;
			}

			public const string DefaultComponentName = "Normal";
		}
	}
}
