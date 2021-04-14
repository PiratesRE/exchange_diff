using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Common;
using Microsoft.Exchange.Transport.Storage.Messaging;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport
{
	internal class PoisonMessage : ITransportComponent
	{
		public static PoisonContext Context
		{
			internal get
			{
				return PoisonMessage.context;
			}
			set
			{
				PoisonMessage.context = value;
			}
		}

		private static bool Enabled
		{
			get
			{
				return PoisonMessage.loaded && Components.Configuration.LocalServer.TransportServer.PoisonMessageDetectionEnabled;
			}
		}

		public static void AddAsyncMessage(string internetMessageId)
		{
			if (!PoisonMessage.Enabled)
			{
				return;
			}
			if (internetMessageId == null)
			{
				return;
			}
			lock (PoisonMessage.asyncMessageIdsSyncRoot)
			{
				PoisonMessage.asyncMessageIds.Add(internetMessageId);
			}
		}

		public static void RemoveAsyncMessage(string internetMessageId)
		{
			if (!PoisonMessage.Enabled)
			{
				return;
			}
			if (internetMessageId == null)
			{
				return;
			}
			lock (PoisonMessage.asyncMessageIdsSyncRoot)
			{
				PoisonMessage.asyncMessageIds.Remove(internetMessageId);
			}
		}

		public static void SavePoisonContext(Exception exception)
		{
			if (!PoisonMessage.Enabled)
			{
				return;
			}
			if (PoisonMessage.Context == null)
			{
				PoisonMessage.SaveAsyncPoisonInformation();
				return;
			}
			try
			{
				switch (PoisonMessage.Context.Source)
				{
				case MessageProcessingSource.Pickup:
					break;
				case MessageProcessingSource.StoreDriverSubmit:
					using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\StoreDriver"))
					{
						StoreDriverSubmitContext storeDriverSubmitContext = PoisonMessage.Context as StoreDriverSubmitContext;
						if (storeDriverSubmitContext != null)
						{
							string text = storeDriverSubmitContext.ToString();
							registryKey.SetValue(text, 1, RegistryValueKind.DWord);
							lock (PoisonMessage.syncRoot)
							{
								PoisonMessage.storeDriverPoisonIds.Add(text);
							}
						}
						goto IL_24D;
					}
					break;
				default:
					goto IL_17A;
				}
				using (RegistryKey registryKey2 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\Pickup"))
				{
					PickupContext pickupContext = PoisonMessage.Context as PickupContext;
					if (pickupContext != null)
					{
						try
						{
							int num = 1;
							if (registryKey2.GetValue(pickupContext.FileName) != null)
							{
								if (registryKey2.GetValueKind(pickupContext.FileName) != RegistryValueKind.DWord)
								{
									registryKey2.DeleteValue(pickupContext.FileName, false);
								}
								else
								{
									num = (int)registryKey2.GetValue(pickupContext.FileName);
									num++;
								}
							}
							registryKey2.SetValue(pickupContext.FileName, num, RegistryValueKind.DWord);
							lock (PoisonMessage.syncRoot)
							{
								TransportHelpers.AttemptAddToDictionary<string, int>(PoisonMessage.pickupFileList, pickupContext.FileName, num, null);
							}
						}
						catch (ArgumentException)
						{
							return;
						}
					}
					goto IL_24D;
				}
				IL_17A:
				MessageContext messageContext = PoisonMessage.Context as MessageContext;
				if (messageContext != null)
				{
					if (messageContext.MessageId > 0L)
					{
						using (RegistryKey registryKey3 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\BootProcess"))
						{
							registryKey3.SetValue(messageContext.MessageId.ToString(NumberFormatInfo.InvariantInfo), messageContext.Source.ToString(), RegistryValueKind.String);
							lock (PoisonMessage.syncRoot)
							{
								PoisonMessage.messageList.Add(messageContext.MessageId);
							}
						}
					}
					if (!string.IsNullOrEmpty(messageContext.InternetMessageId))
					{
						PoisonMessage.SavePoisonInformation(new string[]
						{
							messageContext.InternetMessageId
						}, 1.0, DateTime.UtcNow);
					}
				}
				IL_24D:;
			}
			catch (UnauthorizedAccessException ex)
			{
				PoisonMessage.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PoisonMessageSaveFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
			}
			if (exception != null)
			{
				PoisonMessage.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PoisonMessageCrash, null, new object[]
				{
					exception
				});
			}
		}

		public static bool VerifyStoreDriverSubmission(string poisonId)
		{
			return PoisonMessage.Enabled && PoisonMessage.storeDriverPoisonIds.Count != 0 && PoisonMessage.storeDriverPoisonIds.Contains(poisonId);
		}

		public static bool Verify(long msgId)
		{
			return PoisonMessage.Enabled && PoisonMessage.messageList.Count != 0 && PoisonMessage.messageList.Contains(msgId);
		}

		public static bool DidMessageCrashTransport(string internetMessageId, out double crashCount)
		{
			crashCount = 0.0;
			if (!PoisonMessage.Enabled || PoisonMessage.crashesByInternetMessageId.Count == 0 || string.IsNullOrEmpty(internetMessageId))
			{
				return false;
			}
			CrashProperties crashProperties;
			if (PoisonMessage.crashesByInternetMessageId.TryGetValue(internetMessageId, out crashProperties))
			{
				crashCount = crashProperties.CrashCount;
				return true;
			}
			return false;
		}

		public static bool IsMessagePoison(string internetMessageId)
		{
			double crashCount = 0.0;
			return PoisonMessage.DidMessageCrashTransport(internetMessageId, out crashCount) && PoisonMessage.MeetsPoisonThreshold(crashCount);
		}

		public static bool Verify(string fileName)
		{
			int num;
			return PoisonMessage.Enabled && PoisonMessage.pickupFileList.Count != 0 && PoisonMessage.pickupFileList.TryGetValue(fileName, out num) && num >= Components.Configuration.LocalServer.TransportServer.PoisonThreshold;
		}

		public static void MarkStoreDriverSubmissionHandled(string poisonId)
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\StoreDriver"))
				{
					registryKey.DeleteValue(poisonId, false);
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				PoisonMessage.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PoisonMessageMarkFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
			}
			PoisonMessage.storeDriverPoisonIds.Remove(poisonId);
		}

		public static void MarkMessageHandled(long msgId)
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\BootProcess"))
				{
					registryKey.DeleteValue(msgId.ToString(), false);
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				PoisonMessage.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PoisonMessageMarkFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
			}
			PoisonMessage.messageList.Remove(msgId);
		}

		public static void MarkFileHandled(string fileName)
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\Pickup"))
				{
					registryKey.DeleteValue(fileName, false);
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				PoisonMessage.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PoisonMessageMarkFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
			}
			PoisonMessage.pickupFileList.Remove(fileName);
		}

		public static void PruneExpiredInternetMessageIdEntries()
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\InternetMessageIds"))
				{
					List<string> list = new List<string>();
					foreach (KeyValuePair<string, CrashProperties> entry in PoisonMessage.crashesByInternetMessageId)
					{
						if (PoisonMessage.IsExpired(entry))
						{
							list.Add(entry.Key);
						}
					}
					if (list.Count != 0)
					{
						Dictionary<string, CrashProperties> dictionary = new Dictionary<string, CrashProperties>(PoisonMessage.crashesByInternetMessageId, StringComparer.InvariantCultureIgnoreCase);
						foreach (string text in list)
						{
							dictionary.Remove(text);
							registryKey.DeleteValue(text, false);
						}
						lock (PoisonMessage.syncRoot)
						{
							PoisonMessage.crashesByInternetMessageId = dictionary;
						}
					}
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				PoisonMessage.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PoisonMessagePruneFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
			}
			catch (SecurityException ex2)
			{
				PoisonMessage.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PoisonMessagePruneFailedRegistryAccessDenied, null, new object[]
				{
					ex2.Message
				});
			}
		}

		public void Load()
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\BootProcess"))
				{
					foreach (string text in registryKey.GetValueNames())
					{
						long item;
						if (!long.TryParse(text, out item))
						{
							ExTraceGlobals.GeneralTracer.TraceError<string, RegistryKey>(0L, "Invalid value {0} in the {1} regkey. Deleting it.", text, registryKey);
							registryKey.DeleteValue(text, false);
						}
						else
						{
							PoisonMessage.messageList.Add(item);
						}
					}
				}
				using (RegistryKey registryKey2 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\InternetMessageIds"))
				{
					foreach (string text2 in registryKey2.GetValueNames())
					{
						if (registryKey2.GetValueKind(text2) != RegistryValueKind.String)
						{
							ExTraceGlobals.GeneralTracer.TraceError<string, string>(0L, "Invalid value type '{0}' in registry key '{1}'. Deleting it.", registryKey2.GetValueKind(text2).ToString(), text2);
							registryKey2.DeleteValue(text2, false);
						}
						else
						{
							string text3 = registryKey2.GetValue(text2) as string;
							CrashProperties crashProperties = PoisonMessage.TryParseCrashEntry(text3);
							if (crashProperties != null)
							{
								lock (PoisonMessage.syncRoot)
								{
									TransportHelpers.AttemptAddToDictionary<string, CrashProperties>(PoisonMessage.crashesByInternetMessageId, text2, crashProperties, null);
									goto IL_13C;
								}
							}
							ExTraceGlobals.GeneralTracer.TraceError<string, string>(0L, "Invalid value '{0}' in registry key '{1}'. Deleting it.", text3, text2);
							registryKey2.DeleteValue(text2, false);
						}
						IL_13C:;
					}
				}
				using (RegistryKey registryKey3 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\StoreDriver"))
				{
					foreach (string item2 in registryKey3.GetValueNames())
					{
						PoisonMessage.storeDriverPoisonIds.Add(item2);
					}
				}
				using (RegistryKey registryKey4 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\Pickup"))
				{
					foreach (string text4 in registryKey4.GetValueNames())
					{
						if (registryKey4.GetValueKind(text4) != RegistryValueKind.DWord)
						{
							ExTraceGlobals.GeneralTracer.TraceError<string, RegistryKey>(0L, "Invalid value {0} in {1} registry key. Deleting it.", text4, registryKey4);
							registryKey4.DeleteValue(text4, false);
						}
						else
						{
							int valueToAdd = (int)registryKey4.GetValue(text4);
							TransportHelpers.AttemptAddToDictionary<string, int>(PoisonMessage.pickupFileList, text4, valueToAdd, null);
						}
					}
				}
				IStartableTransportComponent startableTransportComponent;
				if (Components.TryGetBootScanner(out startableTransportComponent))
				{
					((IBootLoader)startableTransportComponent).OnBootLoadCompleted += this.OnBootLoadCompleted;
				}
				PoisonMessage.loaded = true;
			}
			catch (UnauthorizedAccessException ex)
			{
				PoisonMessage.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PoisonMessageLoadFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
				throw new TransportComponentLoadFailedException(Strings.PoisonMessageRegistryAccessFailed, ex);
			}
			catch (SecurityException ex2)
			{
				PoisonMessage.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PoisonMessageLoadFailedRegistryAccessDenied, null, new object[]
				{
					ex2.Message
				});
				throw new TransportComponentLoadFailedException(Strings.PoisonMessageRegistryAccessFailed, ex2);
			}
		}

		public void Unload()
		{
			IStartableTransportComponent startableTransportComponent;
			if (Components.TryGetBootScanner(out startableTransportComponent))
			{
				((IBootLoader)startableTransportComponent).OnBootLoadCompleted -= this.OnBootLoadCompleted;
			}
			PoisonMessage.loaded = false;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public bool HandlePoison(TransportMailItem mailItem, out bool newPoisonMessage)
		{
			newPoisonMessage = false;
			if (!PoisonMessage.Enabled)
			{
				return false;
			}
			if (PoisonMessage.MeetsPoisonThreshold(Convert.ToDouble(mailItem.PoisonCount)))
			{
				return true;
			}
			long recordId = mailItem.RecordId;
			string internetMessageId = mailItem.InternetMessageId;
			double num = 0.0;
			if (PoisonMessage.DidMessageCrashTransport(internetMessageId, out num))
			{
				mailItem.PoisonCount = Convert.ToInt32(num);
				mailItem.BootloadingPriority = DeliveryPriority.None;
				PoisonMessage.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PoisonCountUpdated, null, new object[]
				{
					recordId,
					mailItem.PoisonCount
				});
				ExTraceGlobals.StorageTracer.TraceDebug(mailItem.MsgId, "Poison count updated on message.");
				mailItem.CommitImmediate();
				PoisonMessage.MarkMessageHandled(recordId);
			}
			if (PoisonMessage.MeetsPoisonThreshold(num))
			{
				newPoisonMessage = true;
				return true;
			}
			return false;
		}

		public void SetMessageContext(TransportMailItem mailItem, MessageProcessingSource messageProcessingSource)
		{
			PoisonMessage.Context = new MessageContext(mailItem.RecordId, mailItem.InternetMessageId, messageProcessingSource);
		}

		public void LogPoisonMessageCount(int poisonMessageCount)
		{
			if (poisonMessageCount > 0)
			{
				PoisonMessage.eventLogger.LogEvent(TransportEventLogConstants.Tuple_MessageCountEnqueuedToPoisonQueue, null, new object[]
				{
					poisonMessageCount,
					Components.Configuration.LocalServer.TransportServer.PoisonThreshold
				});
			}
		}

		internal static void SavePoisonInformation(IEnumerable<string> internetMessageIds, double crashCountIncrement, DateTime lastCrashTime)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\InternetMessageIds"))
			{
				foreach (string text in internetMessageIds)
				{
					KeyValuePair<string, double> keyValuePair = new KeyValuePair<string, double>("poisonCount", PoisonMessage.UpdateCrashCount(text, crashCountIncrement));
					KeyValuePair<string, string> keyValuePair2 = new KeyValuePair<string, string>("lastCrashTime", lastCrashTime.ToString("u"));
					StringBuilder stringBuilder = new StringBuilder(256);
					stringBuilder.AppendFormat("{0};{1}", keyValuePair, keyValuePair2);
					registryKey.SetValue(text, stringBuilder.ToString(), RegistryValueKind.String);
				}
			}
		}

		internal static void SaveAsyncPoisonInformation()
		{
			PoisonMessage.SavePoisonInformation(PoisonMessage.asyncMessageIds, Components.TransportAppConfig.PoisonMessage.AsyncMultiplier, DateTime.UtcNow);
		}

		internal static void ClearCrashInfo()
		{
			PoisonMessage.crashesByInternetMessageId.Clear();
		}

		internal static string PoisonInternetMessageIdLocation
		{
			get
			{
				return "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\InternetMessageIds";
			}
		}

		private static IEnumerable<KeyValuePair<string, string>> GetCrashes(string keyValuePairs)
		{
			string[] keyValuePairArray = keyValuePairs.Split(new char[]
			{
				';'
			});
			foreach (string keyValuePair in keyValuePairArray)
			{
				string trimmedKeyValuePair = keyValuePair.TrimStart(new char[]
				{
					'['
				}).TrimEnd(new char[]
				{
					']'
				});
				string[] elements = trimmedKeyValuePair.Split(new char[]
				{
					','
				});
				if (elements.Length != 2)
				{
					ExTraceGlobals.GeneralTracer.TraceError<string>(0L, "Unable to parse crash info property '{0}'; skipping this entry", keyValuePair);
				}
				else
				{
					string key = elements[0];
					string value = elements[1];
					yield return new KeyValuePair<string, string>(key, value);
				}
			}
			yield break;
		}

		private static CrashProperties TryParseCrashEntry(string keyValuePairs)
		{
			bool flag = false;
			bool flag2 = false;
			double crashCount = 0.0;
			DateTime minValue = DateTime.MinValue;
			foreach (KeyValuePair<string, string> keyValuePair in PoisonMessage.GetCrashes(keyValuePairs))
			{
				if (keyValuePair.Key == "poisonCount")
				{
					flag = double.TryParse(keyValuePair.Value, out crashCount);
					if (!flag)
					{
						ExTraceGlobals.GeneralTracer.TraceError<string>(0L, "Unable to convert string poison count '{0}' to number", keyValuePair.Value);
						return null;
					}
				}
				else if (keyValuePair.Key == "lastCrashTime")
				{
					DateTimeStyles style = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.RoundtripKind;
					flag2 = DateTime.TryParseExact(keyValuePair.Value, "u", DateTimeFormatInfo.InvariantInfo, style, out minValue);
					if (!flag2)
					{
						ExTraceGlobals.GeneralTracer.TraceError<string>(0L, "Invalid last crash time value in registry: '{0}'", keyValuePair.Value);
						return null;
					}
				}
			}
			if (flag && flag2)
			{
				return new CrashProperties(crashCount, minValue);
			}
			return null;
		}

		private static bool IsExpired(KeyValuePair<string, CrashProperties> entry)
		{
			bool result = true;
			try
			{
				result = (entry.Value.LastCrashTime + Components.TransportAppConfig.PoisonMessage.CrashDetectionWindow < DateTime.UtcNow);
			}
			catch (ArgumentOutOfRangeException)
			{
				ExTraceGlobals.GeneralTracer.TraceError<string, string>(0L, "Overflow detected calculating if a poison message entry should be expired.  Entry will be removed.  LastCrashTime = {0}, CrashDetectionWindow = {1}", entry.Value.LastCrashTime.ToString("u"), Components.TransportAppConfig.PoisonMessage.CrashDetectionWindow.ToString());
			}
			return result;
		}

		private static double UpdateCrashCount(string internetMessageId, double incrementAmount)
		{
			double num = incrementAmount;
			bool flag = false;
			CrashProperties crashProperties;
			lock (PoisonMessage.syncRoot)
			{
				flag = PoisonMessage.crashesByInternetMessageId.TryGetValue(internetMessageId, out crashProperties);
			}
			if (flag)
			{
				num += crashProperties.CrashCount;
			}
			return num;
		}

		private static bool MeetsPoisonThreshold(double crashCount)
		{
			return Components.Configuration.LocalServer.TransportServer.PoisonThreshold > 0 && crashCount >= Convert.ToDouble(Components.Configuration.LocalServer.TransportServer.PoisonThreshold);
		}

		private void OnBootLoadCompleted()
		{
			PoisonMessage.PruneExpiredInternetMessageIdEntries();
		}

		private const string BasePoisonMsgLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage";

		private const string PickupPoisonMsgLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\Pickup";

		private const string BootProcessPoisonMsgLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\BootProcess";

		private const string BootProcessPoisonInternetMessageIdLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\InternetMessageIds";

		private const string StoreDriverPoisonMsgLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\StoreDriver";

		private const string PoisonCountValueName = "poisonCount";

		private const string LastCrashTimeValueName = "lastCrashTime";

		private static bool loaded;

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.GeneralTracer.Category, TransportEventLog.GetEventSource());

		[ThreadStatic]
		private static PoisonContext context;

		private static HashSet<long> messageList = new HashSet<long>();

		private static Dictionary<string, CrashProperties> crashesByInternetMessageId = new Dictionary<string, CrashProperties>(StringComparer.InvariantCultureIgnoreCase);

		private static HashSet<string> storeDriverPoisonIds = new HashSet<string>();

		private static Dictionary<string, int> pickupFileList = new Dictionary<string, int>();

		private static object syncRoot = new object();

		private static RefCountedSet<string> asyncMessageIds = new RefCountedSet<string>();

		private static object asyncMessageIdsSyncRoot = new object();
	}
}
