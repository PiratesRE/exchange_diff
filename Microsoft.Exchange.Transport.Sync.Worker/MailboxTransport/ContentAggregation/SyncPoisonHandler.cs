using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Worker;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SyncPoisonHandler
	{
		internal static bool PoisonDetectionEnabled
		{
			get
			{
				return SyncPoisonHandler.poisonDetectionEnabled;
			}
			set
			{
				SyncPoisonHandler.poisonDetectionEnabled = value;
			}
		}

		internal static int PoisonItemThreshold
		{
			get
			{
				return SyncPoisonHandler.poisonItemThreshold;
			}
			set
			{
				SyncPoisonHandler.poisonItemThreshold = value;
			}
		}

		internal static int PoisonSubscriptionThreshold
		{
			get
			{
				return SyncPoisonHandler.poisonSubscriptionThreshold;
			}
			set
			{
				SyncPoisonHandler.poisonSubscriptionThreshold = value;
			}
		}

		internal static int MaxPoisonousItemsPerSubscriptionThreshold
		{
			get
			{
				return SyncPoisonHandler.maxPoisonousItemsPerSubscriptionThreshold;
			}
			set
			{
				SyncPoisonHandler.maxPoisonousItemsPerSubscriptionThreshold = value;
			}
		}

		internal static TimeSpan PoisonContextExpiry
		{
			get
			{
				return SyncPoisonHandler.poisonContextExpiry;
			}
			set
			{
				SyncPoisonHandler.poisonContextExpiry = value;
			}
		}

		internal static bool TransportSyncEnabled
		{
			get
			{
				return SyncPoisonHandler.transportSyncEnabled;
			}
			set
			{
				SyncPoisonHandler.transportSyncEnabled = value;
			}
		}

		internal static bool PoisonDetectionOperational
		{
			get
			{
				return SyncPoisonHandler.TransportSyncEnabled && SyncPoisonHandler.PoisonDetectionEnabled;
			}
		}

		public static void ClearPoisonContext(Guid subscriptionId, SyncPoisonStatus syncPoisonStatus, SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionId", subscriptionId);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			syncLogSession.LogDebugging((TSLID)542UL, SyncPoisonHandler.tracer, "Clear Poison Context for Subscription: {0}, with status: {1}", new object[]
			{
				subscriptionId,
				syncPoisonStatus
			});
			if (syncPoisonStatus == SyncPoisonStatus.CleanSubscription)
			{
				return;
			}
			string text = subscriptionId.ToString();
			Exception exception = null;
			try
			{
				lock (SyncPoisonHandler.baseRegistryKeySyncRoot)
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\SyncPoisonContext\\", true))
					{
						if (registryKey != null)
						{
							try
							{
								registryKey.DeleteSubKeyTree(text);
							}
							catch (ArgumentException ex)
							{
								syncLogSession.LogError((TSLID)543UL, SyncPoisonHandler.tracer, "Delete registry entry for subscription {0} failed with error: {1}.", new object[]
								{
									text,
									ex
								});
							}
						}
					}
				}
			}
			catch (SecurityException ex2)
			{
				exception = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				exception = ex3;
			}
			SyncPoisonHandler.HandleRegistryAccessError(exception, syncLogSession);
			lock (SyncPoisonHandler.suspectedSubscriptionListSyncRoot)
			{
				SyncPoisonHandler.suspectedSubscriptionList.Remove(text);
			}
			if (syncPoisonStatus == SyncPoisonStatus.PoisonousItems)
			{
				lock (SyncPoisonHandler.poisonItemListSyncRoot)
				{
					SyncPoisonHandler.poisonItemList.Remove(text);
				}
			}
			lock (SyncPoisonHandler.crashCallstackListSyncRoot)
			{
				SyncPoisonHandler.crashCallstackList.Remove(text);
			}
		}

		public static bool IsPoisonItem(SyncPoisonContext syncPoisonContext, SyncPoisonStatus syncPoisonStatus, SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("syncPoisonContext", syncPoisonContext);
			SyncUtilities.ThrowIfArgumentNull("syncPoisonContext.Item", syncPoisonContext.Item);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			syncLogSession.LogDebugging((TSLID)544UL, SyncPoisonHandler.tracer, "Checking IsPoisonItem for Item: {0}, with status: {1}", new object[]
			{
				syncPoisonContext,
				syncPoisonStatus
			});
			if (!SyncPoisonHandler.PoisonDetectionOperational)
			{
				syncLogSession.LogDebugging((TSLID)545UL, SyncPoisonHandler.tracer, "Poison Detection is not operational, return clean status for the item: {0}", new object[]
				{
					syncPoisonContext
				});
				return false;
			}
			if (syncPoisonStatus != SyncPoisonStatus.PoisonousItems)
			{
				syncLogSession.LogDebugging((TSLID)546UL, SyncPoisonHandler.tracer, "Taking item {0} as clean, since sync poison status ({1}) is not {2}", new object[]
				{
					syncPoisonContext,
					syncPoisonStatus,
					SyncPoisonStatus.PoisonousItems
				});
				return false;
			}
			List<string> list = null;
			lock (SyncPoisonHandler.poisonItemListSyncRoot)
			{
				if (!SyncPoisonHandler.poisonItemList.TryGetValue(syncPoisonContext.SubscriptionId.ToString(), out list))
				{
					syncLogSession.LogDebugging((TSLID)547UL, SyncPoisonHandler.tracer, "No entry found for the item {0} in the poisonous item list", new object[]
					{
						syncPoisonContext
					});
					return false;
				}
			}
			return list.Contains(syncPoisonContext.Item.Key);
		}

		internal static void SetSyncPoisonContextOnCurrentThread(object syncPoisonContext)
		{
			SyncPoisonHandler.syncPoisonContext = (syncPoisonContext as SyncPoisonContext);
			if (syncPoisonContext != null && SyncPoisonHandler.syncPoisonContext == null)
			{
				throw new ArgumentException("Invalid argument type, should be SyncPoisonContext", "syncPoisonContext");
			}
		}

		internal static void ClearSyncPoisonContextFromCurrentThread()
		{
			SyncPoisonHandler.syncPoisonContext = null;
		}

		internal static void SavePoisonContext(Exception e, SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("e", e);
			if (syncLogSession != null)
			{
				syncLogSession.LogDebugging((TSLID)548UL, SyncPoisonHandler.tracer, "Saving Poison Context.", new object[0]);
			}
			if (!SyncPoisonHandler.PoisonDetectionOperational)
			{
				if (syncLogSession != null)
				{
					syncLogSession.LogDebugging((TSLID)549UL, SyncPoisonHandler.tracer, "Poison Detection is not operational, no need to Save Poison Context.", new object[0]);
				}
				return;
			}
			if (SyncPoisonHandler.syncPoisonContext == null)
			{
				if (syncLogSession != null)
				{
					syncLogSession.LogDebugging((TSLID)550UL, SyncPoisonHandler.tracer, "No context information found on the crashing thread", new object[0]);
				}
				return;
			}
			AggregationComponent.EventLogger.LogEvent(TransportSyncWorkerEventLogConstants.Tuple_SubscriptionCausedCrash, null, new object[]
			{
				SyncPoisonHandler.syncPoisonContext.SubscriptionId,
				e
			});
			Exception exception = null;
			try
			{
				string subkey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\SyncPoisonContext\\" + SyncPoisonHandler.syncPoisonContext.SubscriptionId;
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3202755901U);
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(subkey))
				{
					if (SyncPoisonHandler.syncPoisonContext.HasSubscriptionContextOnly)
					{
						SyncPoisonHandler.IncrementCrashCountRegistryValue(registryKey);
					}
					else
					{
						string subkey2 = Guid.NewGuid().ToString();
						using (RegistryKey registryKey2 = registryKey.CreateSubKey(subkey2))
						{
							registryKey2.SetValue("SyncPoisonItem", SyncPoisonHandler.syncPoisonContext.Item.Key);
						}
					}
					registryKey.SetValue("Timestamp", ExDateTime.UtcNow.ToString());
					registryKey.SetValue("CrashCallStack", e.ToString());
				}
			}
			catch (SecurityException ex)
			{
				exception = ex;
			}
			catch (UnauthorizedAccessException ex2)
			{
				exception = ex2;
			}
			SyncPoisonHandler.HandleRegistryAccessError(exception, syncLogSession);
		}

		internal static void Load(SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			syncLogSession.LogDebugging((TSLID)551UL, SyncPoisonHandler.tracer, "Loading Poison Context from Registry ....", new object[0]);
			if (!SyncPoisonHandler.PoisonDetectionEnabled)
			{
				syncLogSession.LogDebugging((TSLID)552UL, SyncPoisonHandler.tracer, "Poison Detection is disabled, no need to Load Poison Context", new object[0]);
				return;
			}
			Exception exception = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\SyncPoisonContext\\", true))
				{
					if (registryKey == null)
					{
						syncLogSession.LogVerbose((TSLID)553UL, SyncPoisonHandler.tracer, "No registry entry found while Loading poison context.", new object[0]);
						return;
					}
					foreach (string text in registryKey.GetSubKeyNames())
					{
						using (RegistryKey registryKey2 = registryKey.OpenSubKey(text, true))
						{
							if (registryKey2 == null)
							{
								syncLogSession.LogError((TSLID)554UL, SyncPoisonHandler.tracer, "Registry entry for Subscription key ({0}) no longer exists, continue with the next one.", new object[]
								{
									text
								});
							}
							else
							{
								object value = registryKey2.GetValue("Timestamp");
								ExDateTime minValue = ExDateTime.MinValue;
								if (value is string && ExDateTime.TryParse((string)value, out minValue))
								{
									if (minValue.Add(SyncPoisonHandler.PoisonContextExpiry) >= ExDateTime.UtcNow)
									{
										string value2 = registryKey2.GetValue("CrashCallStack") as string;
										if (!string.IsNullOrEmpty(value2))
										{
											SyncPoisonHandler.crashCallstackList.Add(text, value2);
										}
										object value3 = registryKey2.GetValue("CrashCount");
										if (value3 is int && (int)value3 >= SyncPoisonHandler.PoisonSubscriptionThreshold)
										{
											SyncPoisonHandler.suspectedSubscriptionList.Add(text, true);
											goto IL_35D;
										}
										SyncPoisonHandler.suspectedSubscriptionList.Add(text, false);
										List<string> list = new List<string>(1);
										Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
										foreach (string text2 in registryKey2.GetSubKeyNames())
										{
											using (RegistryKey registryKey3 = registryKey2.OpenSubKey(text2))
											{
												if (registryKey3 == null)
												{
													syncLogSession.LogError((TSLID)556UL, SyncPoisonHandler.tracer, "Registry entry for item key ({0}) no longer exists, continue will the next one.", new object[]
													{
														text2
													});
												}
												else
												{
													string text3 = registryKey3.GetValue("SyncPoisonItem") as string;
													if (string.IsNullOrEmpty(text3))
													{
														syncLogSession.LogError((TSLID)557UL, SyncPoisonHandler.tracer, "Invalid Value found for SyncPoisonItem: {0}, deleting item Key: {1}", new object[]
														{
															text3,
															text2
														});
														registryKey2.DeleteSubKey(text2, false);
													}
													else
													{
														int num = 0;
														if (dictionary.TryGetValue(text3, out num))
														{
															num++;
														}
														else
														{
															num = 1;
														}
														dictionary[text3] = num;
														if (num >= SyncPoisonHandler.PoisonItemThreshold)
														{
															list.Add(text3);
														}
													}
												}
											}
										}
										if (list.Count >= SyncPoisonHandler.MaxPoisonousItemsPerSubscriptionThreshold)
										{
											syncLogSession.LogError((TSLID)558UL, SyncPoisonHandler.tracer, "Subscription {0} has more than {1} Poisonous items. Making the whole subscription poisonous.", new object[]
											{
												text,
												SyncPoisonHandler.MaxPoisonousItemsPerSubscriptionThreshold
											});
											SyncPoisonHandler.suspectedSubscriptionList[text] = true;
											goto IL_35D;
										}
										if (list.Count > 0)
										{
											SyncPoisonHandler.poisonItemList.Add(text, list);
										}
										goto IL_35D;
									}
								}
								try
								{
									registryKey.DeleteSubKeyTree(text);
								}
								catch (ArgumentException ex)
								{
									syncLogSession.LogError((TSLID)555UL, SyncPoisonHandler.tracer, "Delete registry entry for subscription key {0} failed with error: {1}, continue will the next one.", new object[]
									{
										text,
										ex
									});
								}
							}
						}
						IL_35D:;
					}
				}
			}
			catch (SecurityException ex2)
			{
				exception = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				exception = ex3;
			}
			SyncPoisonHandler.HandleRegistryAccessError(exception, syncLogSession);
		}

		internal static SyncPoisonStatus GetPoisonStatus(Guid subscriptionId, SyncLogSession syncLogSession, out string poisonCallstack)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionId", subscriptionId);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			poisonCallstack = null;
			syncLogSession.LogDebugging((TSLID)559UL, SyncPoisonHandler.tracer, "Get Poison Status for Subscription: {0}", new object[]
			{
				subscriptionId
			});
			if (!SyncPoisonHandler.PoisonDetectionOperational)
			{
				syncLogSession.LogDebugging((TSLID)560UL, SyncPoisonHandler.tracer, "Poison Detection is not operational, just return Clean Status.", new object[0]);
				return SyncPoisonStatus.CleanSubscription;
			}
			if (SyncPoisonHandler.suspectedSubscriptionList.Count == 0)
			{
				syncLogSession.LogDebugging((TSLID)561UL, SyncPoisonHandler.tracer, "No suspected subscriptions found, just return Clean Status.", new object[0]);
				return SyncPoisonStatus.CleanSubscription;
			}
			string key = subscriptionId.ToString();
			bool flag = false;
			lock (SyncPoisonHandler.suspectedSubscriptionListSyncRoot)
			{
				if (!SyncPoisonHandler.suspectedSubscriptionList.TryGetValue(key, out flag))
				{
					syncLogSession.LogDebugging((TSLID)562UL, SyncPoisonHandler.tracer, "Subscription {0} is not in suspected subscription list, return Clean Status.", new object[]
					{
						subscriptionId
					});
					return SyncPoisonStatus.CleanSubscription;
				}
			}
			if (flag)
			{
				syncLogSession.LogError((TSLID)563UL, SyncPoisonHandler.tracer, "Poisonous Subscription found: {0}", new object[]
				{
					subscriptionId
				});
				lock (SyncPoisonHandler.crashCallstackListSyncRoot)
				{
					if (!SyncPoisonHandler.crashCallstackList.TryGetValue(key, out poisonCallstack))
					{
						syncLogSession.LogDebugging((TSLID)564UL, SyncPoisonHandler.tracer, "Subscription {0} does not have a callstack information, assign to null.", new object[]
						{
							subscriptionId
						});
						poisonCallstack = null;
					}
				}
				return SyncPoisonStatus.PoisonousSubscription;
			}
			lock (SyncPoisonHandler.poisonItemListSyncRoot)
			{
				if (SyncPoisonHandler.poisonItemList.ContainsKey(key))
				{
					syncLogSession.LogError((TSLID)565UL, SyncPoisonHandler.tracer, "Subscription {0} has Poisonous items.", new object[]
					{
						subscriptionId
					});
					return SyncPoisonStatus.PoisonousItems;
				}
			}
			syncLogSession.LogVerbose((TSLID)566UL, SyncPoisonHandler.tracer, "Suspected Subscription {0} found.", new object[]
			{
				subscriptionId
			});
			return SyncPoisonStatus.SuspectedSubscription;
		}

		private static void IncrementCrashCountRegistryValue(RegistryKey registryKey)
		{
			int num = 0;
			object value = registryKey.GetValue("CrashCount");
			if (value is int)
			{
				num = (int)value;
			}
			registryKey.SetValue("CrashCount", num + 1);
		}

		private static void HandleRegistryAccessError(Exception exception, SyncLogSession syncLogSession)
		{
			if (exception != null)
			{
				if (syncLogSession != null)
				{
					syncLogSession.LogError((TSLID)567UL, SyncPoisonHandler.tracer, "Registry Access failed with error: {0}", new object[]
					{
						exception
					});
				}
				AggregationComponent.EventLogger.LogEvent(TransportSyncWorkerEventLogConstants.Tuple_RegistryAccessDenied, null, new object[]
				{
					exception
				});
				EventNotificationHelper.PublishTransportEventNotificationItem(TransportSyncNotificationEvent.RegistryAccessDenied.ToString(), exception);
			}
		}

		private const string BaseSyncPoisonLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\SyncPoisonContext\\";

		private const string CrashCountRegistryName = "CrashCount";

		private const string CrashCallstack = "CrashCallStack";

		private const string TimestampRegistryName = "Timestamp";

		private const string SyncPoisonItemRegistryName = "SyncPoisonItem";

		private const int ExpectedPoisonEntries = 0;

		private const int ExpectedPoisonItemsPerSubscription = 1;

		private static readonly Trace tracer = ExTraceGlobals.SyncPoisonHandlerTracer;

		[ThreadStatic]
		private static SyncPoisonContext syncPoisonContext;

		private static Dictionary<string, bool> suspectedSubscriptionList = new Dictionary<string, bool>(0);

		private static Dictionary<string, List<string>> poisonItemList = new Dictionary<string, List<string>>(0);

		private static Dictionary<string, string> crashCallstackList = new Dictionary<string, string>(0);

		private static object baseRegistryKeySyncRoot = new object();

		private static object suspectedSubscriptionListSyncRoot = new object();

		private static object poisonItemListSyncRoot = new object();

		private static object crashCallstackListSyncRoot = new object();

		private static bool poisonDetectionEnabled = false;

		private static bool transportSyncEnabled = false;

		private static int poisonItemThreshold = 2;

		private static int poisonSubscriptionThreshold = 2;

		private static int maxPoisonousItemsPerSubscriptionThreshold = 3;

		private static TimeSpan poisonContextExpiry = TimeSpan.FromDays(2.0);
	}
}
