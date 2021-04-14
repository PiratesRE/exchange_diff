using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class RegistryCrashRepository : ICrashRepository
	{
		public RegistryCrashRepository(string poisonRegistryEntryLocation, IStoreDriverTracer storeDriverTracer)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("poisonRegistryEntryLocation", poisonRegistryEntryLocation);
			ArgumentValidator.ThrowIfNull("storeDriverTracer", storeDriverTracer);
			this.poisonRegistryEntryLocation = poisonRegistryEntryLocation;
			this.storeDriverTracer = storeDriverTracer;
		}

		protected SortedSet<KeyValuePair<SubmissionPoisonContext, DateTime>> PurgablePoisonDataSet
		{
			get
			{
				return this.purgablePoisonDataSet;
			}
			set
			{
				this.purgablePoisonDataSet = value;
			}
		}

		public List<Guid> GetAllResourceIDs()
		{
			List<Guid> list = new List<Guid>();
			List<Guid> result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(this.poisonRegistryEntryLocation, RegistryKeyPermissionCheck.ReadWriteSubTree))
				{
					if (registryKey != null)
					{
						foreach (string text in registryKey.GetSubKeyNames())
						{
							Guid item;
							if (!Guid.TryParse(text, out item))
							{
								this.storeDriverTracer.GeneralTracer.TraceFail<string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Invalid Resource Guid {0}. Deleting it.", text);
								registryKey.DeleteSubKeyTree(text);
							}
							else
							{
								list.Add(item);
							}
						}
					}
				}
				result = list;
			}
			catch (UnauthorizedAccessException ex)
			{
				StoreDriverSubmission.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_PoisonMessageLoadFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
				throw new CrashRepositoryAccessException(Strings.PoisonMessageRegistryAccessFailed, ex);
			}
			return result;
		}

		public bool GetQuarantineInfoContext(Guid resourceGuid, TimeSpan quarantineExpiryWindow, out QuarantineInfoContext quarantineInfoContext)
		{
			ArgumentValidator.ThrowIfEmpty("resourceGuid", resourceGuid);
			quarantineInfoContext = null;
			bool result;
			try
			{
				string text = resourceGuid.ToString();
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format(RegistryCrashRepository.resourceSubKeyFromRoot, this.poisonRegistryEntryLocation, text), RegistryKeyPermissionCheck.ReadWriteSubTree))
				{
					DateTime quarantineStartTime;
					if (registryKey != null && this.GetQuarantineStartTime(registryKey, text, quarantineExpiryWindow, out quarantineStartTime))
					{
						quarantineInfoContext = new QuarantineInfoContext(quarantineStartTime);
						return true;
					}
				}
				result = false;
			}
			catch (UnauthorizedAccessException ex)
			{
				StoreDriverSubmission.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_PoisonMessageLoadFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
				throw new CrashRepositoryAccessException(Strings.PoisonMessageRegistryAccessFailed, ex);
			}
			return result;
		}

		public bool GetResourceCrashInfoData(Guid resourceGuid, TimeSpan crashExpiryWindow, out Dictionary<long, ResourceEventCounterCrashInfo> resourceCrashData, out SortedSet<DateTime> allCrashTimes)
		{
			ArgumentValidator.ThrowIfEmpty("resourceGuid", resourceGuid);
			string text = resourceGuid.ToString();
			resourceCrashData = new Dictionary<long, ResourceEventCounterCrashInfo>();
			allCrashTimes = new SortedSet<DateTime>();
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format(RegistryCrashRepository.poisonInfoKeyLocationFromRoot, this.poisonRegistryEntryLocation, text), RegistryKeyPermissionCheck.ReadWriteSubTree))
				{
					if (registryKey != null)
					{
						foreach (string text2 in registryKey.GetValueNames())
						{
							if (string.IsNullOrEmpty(text2))
							{
								registryKey.DeleteValue(text2, false);
							}
							else
							{
								ResourceEventCounterCrashInfo resourceEventCounterCrashInfo = null;
								long num;
								if (this.ProcessEventCounterData(text2, registryKey, text, out num))
								{
									string[] eventCounterCrashInfo = null;
									if (this.GetEventCounterCrashInfoValue(text2, registryKey, text, out eventCounterCrashInfo) && this.ProcessSingleEventCounterCrashInfoValue(eventCounterCrashInfo, text2, registryKey, text, crashExpiryWindow, out resourceEventCounterCrashInfo))
									{
										resourceCrashData.Add(num, resourceEventCounterCrashInfo);
										allCrashTimes.UnionWith(resourceEventCounterCrashInfo.CrashTimes);
										this.AddDataToPurgeDictionary(resourceGuid, num, resourceEventCounterCrashInfo.CrashTimes.Max);
									}
								}
							}
						}
					}
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				StoreDriverSubmission.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_PoisonMessageLoadFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
				throw new CrashRepositoryAccessException(Strings.PoisonMessageRegistryAccessFailed, ex);
			}
			return resourceCrashData.Count != 0;
		}

		public void PersistCrashInfo(Guid resourceGuid, long eventCounter, ResourceEventCounterCrashInfo resourceEventCounterCrashInfo, int maxCrashEntries)
		{
			ArgumentValidator.ThrowIfEmpty("resourceGuid", resourceGuid);
			ArgumentValidator.ThrowIfNull("resourceEventCounterCrashInfo", resourceEventCounterCrashInfo);
			ArgumentValidator.ThrowIfZeroOrNegative("maxCrashEntries", maxCrashEntries);
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(string.Format(RegistryCrashRepository.poisonInfoKeyLocationFromRoot, this.poisonRegistryEntryLocation, resourceGuid)))
				{
					this.PersistCrashInfoToRegistry(registryKey, resourceGuid, eventCounter, resourceEventCounterCrashInfo, maxCrashEntries);
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				StoreDriverSubmission.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_PoisonMessageSaveFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
				throw new CrashRepositoryAccessException(Strings.PoisonMessageRegistryAccessFailed, ex);
			}
		}

		public bool PersistQuarantineInfo(Guid resourceGuid, QuarantineInfoContext quarantineInfoContext, bool overrideExisting = false)
		{
			ArgumentValidator.ThrowIfEmpty("resourceGuid", resourceGuid);
			ArgumentValidator.ThrowIfNull("quarantineInfoContext", quarantineInfoContext);
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(this.poisonRegistryEntryLocation + "\\" + resourceGuid.ToString()))
				{
					if (registryKey.GetValue("QuarantineStart") == null || overrideExisting)
					{
						registryKey.SetValue("QuarantineStart", quarantineInfoContext.QuarantineStartTime);
						return true;
					}
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				StoreDriverSubmission.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_PoisonMessageSaveFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
				throw new CrashRepositoryAccessException(Strings.PoisonMessageRegistryAccessFailed, ex);
			}
			return false;
		}

		public void PurgeResourceData(Guid resourceGuid)
		{
			ArgumentValidator.ThrowIfEmpty("resourceGuid", resourceGuid);
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(this.poisonRegistryEntryLocation, RegistryKeyPermissionCheck.ReadWriteSubTree))
				{
					if (registryKey != null)
					{
						using (RegistryKey registryKey2 = registryKey.OpenSubKey(resourceGuid.ToString()))
						{
							if (registryKey2 != null && registryKey2.GetValueNames().Length == 0)
							{
								registryKey.DeleteSubKeyTree(resourceGuid.ToString(), false);
							}
						}
					}
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				StoreDriverSubmission.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_PoisonMessageSaveFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
				throw new CrashRepositoryAccessException(Strings.PoisonMessageRegistryAccessFailed, ex);
			}
		}

		protected void PersistCrashInfoToRegistry(RegistryKey resourcePoisonKey, Guid resourceGuid, long eventCounter, ResourceEventCounterCrashInfo resourceEventCounterCrashInfo, int maxCrashEntries)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (DateTime dateTime in resourceEventCounterCrashInfo.CrashTimes)
			{
				stringBuilder.Append(dateTime + ";");
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			string text = eventCounter.ToString();
			if (this.PurgeRegistryEntriesIfNecessary(resourceGuid.ToString(), text, maxCrashEntries))
			{
				this.AddDataToPurgeDictionary(resourceGuid, eventCounter, resourceEventCounterCrashInfo.CrashTimes.Max);
			}
			resourcePoisonKey.SetValue(text, new string[]
			{
				stringBuilder.ToString(),
				resourceEventCounterCrashInfo.IsPoisonNdrSent.ToString()
			}, RegistryValueKind.MultiString);
		}

		protected bool PurgeRegistryEntriesIfNecessary(string resourceBeingAdded, string eventCounterString, int maxCrashEntries)
		{
			if (this.purgablePoisonDataSet.Count == 0 || this.purgablePoisonDataSet.Count < maxCrashEntries)
			{
				return false;
			}
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format(RegistryCrashRepository.poisonInfoKeyLocationFromRoot, this.poisonRegistryEntryLocation, resourceBeingAdded)))
			{
				if (registryKey != null && registryKey.GetValue(eventCounterString) != null)
				{
					return false;
				}
			}
			KeyValuePair<SubmissionPoisonContext, DateTime> min = this.purgablePoisonDataSet.Min;
			using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(string.Format(RegistryCrashRepository.poisonInfoKeyLocationFromRoot, this.poisonRegistryEntryLocation, min.Key.ResourceGuid), RegistryKeyPermissionCheck.ReadWriteSubTree))
			{
				if (registryKey2 != null)
				{
					registryKey2.DeleteValue(min.Key.MapiEventCounter.ToString(), false);
					this.purgablePoisonDataSet.Remove(min);
					return true;
				}
			}
			return false;
		}

		protected void AddDataToPurgeDictionary(Guid resourceGuid, long eventCounter, DateTime crashTime)
		{
			this.purgablePoisonDataSet.Add(new KeyValuePair<SubmissionPoisonContext, DateTime>(new SubmissionPoisonContext(resourceGuid, eventCounter), crashTime));
		}

		protected bool ProcessEventCounterData(string eventCounterString, RegistryKey poisonKey, string resourceKeyString, out long eventCounter)
		{
			if (!long.TryParse(eventCounterString, out eventCounter))
			{
				this.storeDriverTracer.GeneralTracer.TraceFail<long, string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Invalid event counter {0} in {1} registry key. Deleting it.", eventCounter, resourceKeyString);
				poisonKey.DeleteValue(eventCounterString, false);
				return false;
			}
			return true;
		}

		protected bool GetEventCounterCrashInfoValue(string eventCounterString, RegistryKey poisonKey, string resourceKeyString, out string[] eventCounterCrashInfo)
		{
			eventCounterCrashInfo = null;
			if (poisonKey.GetValueKind(eventCounterString) != RegistryValueKind.MultiString)
			{
				this.storeDriverTracer.GeneralTracer.TraceFail<string, string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Event Counter Value is not multi string for event counter {0} inside resource {1}", eventCounterString, resourceKeyString);
				poisonKey.DeleteValue(eventCounterString, false);
				return false;
			}
			eventCounterCrashInfo = (string[])poisonKey.GetValue(eventCounterString);
			if (eventCounterCrashInfo.Length != 2)
			{
				this.storeDriverTracer.GeneralTracer.TraceFail<string[], string, string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Invalid value {0} for event counter {1} in {2} registry key. Deleting it.", eventCounterCrashInfo, eventCounterString, resourceKeyString);
				poisonKey.DeleteValue(eventCounterString, false);
				eventCounterCrashInfo = null;
				return false;
			}
			return true;
		}

		protected bool ProcessSingleEventCounterCrashInfoValue(string[] eventCounterCrashInfo, string eventCounterString, RegistryKey poisonKey, string resourceKeyString, TimeSpan crashExpiryWindow, out ResourceEventCounterCrashInfo resourceEventCounterCrashInfo)
		{
			ArgumentValidator.ThrowIfNull("eventCounterCrashInfo", eventCounterCrashInfo);
			ArgumentValidator.ThrowIfInvalidValue<int>("eventCounterCrashInfo", eventCounterCrashInfo.Length, (int length) => length == 2);
			resourceEventCounterCrashInfo = null;
			bool isPoisonNdrSent;
			SortedSet<DateTime> crashTimes;
			if (this.ParseSingleEventCounterNdrSentData(eventCounterCrashInfo[1], eventCounterString, poisonKey, resourceKeyString, out isPoisonNdrSent) && this.ProcessSingleEventCounterCrashTimeData(eventCounterCrashInfo[0], eventCounterString, poisonKey, resourceKeyString, crashExpiryWindow, out crashTimes))
			{
				resourceEventCounterCrashInfo = new ResourceEventCounterCrashInfo(crashTimes, isPoisonNdrSent);
				return true;
			}
			return false;
		}

		protected bool ParseSingleEventCounterNdrSentData(string eventCounterCrashInfoNdrValue, string eventCounterString, RegistryKey poisonKey, string resourceKeyString, out bool isNdrSent)
		{
			if (!bool.TryParse(eventCounterCrashInfoNdrValue, out isNdrSent))
			{
				this.storeDriverTracer.GeneralTracer.TraceFail<string, string, string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Invalid bool value {0} for event counter {1} in {2} registry key. Deleting it.", eventCounterCrashInfoNdrValue, eventCounterString, resourceKeyString);
				poisonKey.DeleteValue(eventCounterString, false);
				return false;
			}
			return true;
		}

		protected bool ProcessSingleEventCounterCrashTimeData(string eventCounterCrashInfoCrashTimes, string eventCounterString, RegistryKey poisonKey, string resourceKeyString, TimeSpan crashExpiryWindow, out SortedSet<DateTime> eventCounterCrashTimeSet)
		{
			eventCounterCrashTimeSet = null;
			string[] array = eventCounterCrashInfoCrashTimes.Split(";".ToCharArray());
			eventCounterCrashTimeSet = new SortedSet<DateTime>();
			foreach (string text in array)
			{
				DateTime dateTime;
				if (!DateTime.TryParse(text, out dateTime))
				{
					this.storeDriverTracer.GeneralTracer.TraceFail<string, string, string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Invalid Crash Time value {0} for event counter {1} in {2} registry key. Deleting it.", text, eventCounterString, resourceKeyString);
					poisonKey.DeleteValue(eventCounterString, false);
					eventCounterCrashTimeSet = null;
					return false;
				}
				if (!this.IsCrashTimeExpired(dateTime, eventCounterString, resourceKeyString, crashExpiryWindow))
				{
					eventCounterCrashTimeSet.Add(dateTime);
				}
			}
			if (eventCounterCrashTimeSet.Count == 0)
			{
				eventCounterCrashTimeSet = null;
				poisonKey.DeleteValue(eventCounterString, false);
				return false;
			}
			return true;
		}

		protected bool IsCrashTimeExpired(DateTime crashTime, string eventCounterString, string resourceKeyString, TimeSpan crashExpiryWindow)
		{
			if (StoreDriverUtils.CheckIfDateTimeExceedsThreshold(crashTime, DateTime.UtcNow, crashExpiryWindow))
			{
				this.storeDriverTracer.GeneralTracer.TracePass<DateTime, string, string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Crash time {0} has expired for Event Counter {1} inside resource {2}.", crashTime, eventCounterString, resourceKeyString);
				return true;
			}
			return false;
		}

		protected bool GetQuarantineStartTime(RegistryKey resourceKey, string resource, TimeSpan quarantineExpiryWindow, out DateTime quarantineStartTime)
		{
			quarantineStartTime = DateTime.MinValue;
			object value = resourceKey.GetValue("QuarantineStart");
			if (value == null)
			{
				resourceKey.DeleteValue("QuarantineStart", false);
				return false;
			}
			if (!DateTime.TryParse(value.ToString(), out quarantineStartTime))
			{
				this.storeDriverTracer.GeneralTracer.TraceFail<object, string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Failed to parse start time of Quarantine specfied by string {0} for resource {1}. Will delete it", value, resource);
				resourceKey.DeleteValue("QuarantineStart", false);
				return false;
			}
			if (StoreDriverUtils.CheckIfDateTimeExceedsThreshold(quarantineStartTime, DateTime.UtcNow, quarantineExpiryWindow))
			{
				this.storeDriverTracer.GeneralTracer.TracePass<DateTime, TimeSpan, string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Date time {0} has expired when compared using TimeSpan {1} for resource {2}. Entry will be removed from registry", quarantineStartTime, quarantineExpiryWindow, resource);
				resourceKey.DeleteValue("QuarantineStart", false);
				return false;
			}
			return true;
		}

		private const string PoisonSubKeyName = "PoisonInfo";

		private const string CrashTimesSeparator = ";";

		private static string poisonInfoKeyLocationFromRoot = "{0}\\{1}\\PoisonInfo";

		private static string resourceSubKeyFromRoot = "{0}\\{1}";

		private readonly string poisonRegistryEntryLocation;

		private readonly IStoreDriverTracer storeDriverTracer;

		private SortedSet<KeyValuePair<SubmissionPoisonContext, DateTime>> purgablePoisonDataSet = new SortedSet<KeyValuePair<SubmissionPoisonContext, DateTime>>(new PoisonDataComparer());
	}
}
