using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class DeviceBehavior
	{
		public DeviceBehavior() : this(false)
		{
		}

		public DeviceBehavior(bool initialize)
		{
			if (initialize)
			{
				this.userAgentTimes = new List<ExDateTime>();
				this.userAgentStrings = new List<string>();
				this.recentCommandTimes = new List<ExDateTime>();
				this.recentCommandHashCodes = new List<int>();
				this.watsons = new List<ExDateTime>();
				this.outOfBudgets = new List<ExDateTime>();
				this.syncTimes = new List<ExDateTime>();
				this.syncKeys = new List<int>();
			}
			this.WhenLoaded = ExDateTime.UtcNow;
		}

		internal DeviceAccessStateReason AutoBlockReason
		{
			get
			{
				return this.autoBlockReason;
			}
		}

		internal ExDateTime TimeToUpdateAD
		{
			get
			{
				return this.timeToUpdateAD;
			}
		}

		internal GlobalInfo Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				lock (this.instanceLock)
				{
					this.owner = value;
				}
			}
		}

		internal ExDateTime WhenLoaded { get; set; }

		internal ProtocolLogger ProtocolLogger { get; set; }

		internal string CacheToken { get; set; }

		private static TimeSpan MaxADUpdateDelay
		{
			get
			{
				if (DeviceBehavior.maxADUpdateDelay == null)
				{
					DeviceBehavior.maxADUpdateDelay = new TimeSpan?(TimeSpan.FromHours((double)GlobalSettings.AutoBlockADWriteDelay));
				}
				return DeviceBehavior.maxADUpdateDelay.Value;
			}
		}

		public static List<Exception> ResetAutoBlockedDevices(MailboxSession mailboxSession)
		{
			List<Exception> list = new List<Exception>();
			IEnumerator enumerator = SyncStateStorage.GetEnumerator(mailboxSession, null);
			using (enumerator as IDisposable)
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					SyncStateStorage syncStateStorage = (SyncStateStorage)obj;
					if (syncStateStorage.DeviceIdentity.IsProtocol("AirSync"))
					{
						try
						{
							using (GlobalInfo globalInfo = GlobalInfo.LoadFromMailbox(mailboxSession, syncStateStorage, null))
							{
								if (globalInfo.DeviceBehavior.AutoBlockReason != DeviceAccessStateReason.Unknown)
								{
									globalInfo.DeviceBehavior.UnblockDevice();
									globalInfo.IsDirty = true;
									globalInfo.SaveToMailbox();
								}
							}
						}
						catch (LocalizedException item)
						{
							list.Add(item);
						}
					}
				}
			}
			return list;
		}

		public static DeviceBehavior GetDeviceBehavior(Guid userGuid, DeviceIdentity deviceIdentity, GlobalInfo globalInfo, object traceObject, ProtocolLogger protocolLogger)
		{
			string token = DeviceBehaviorCache.GetToken(userGuid, deviceIdentity);
			globalInfo.DeviceBehavior.ProtocolLogger = protocolLogger;
			globalInfo.DeviceBehavior.CacheToken = token;
			DeviceBehavior deviceBehavior;
			if (globalInfo.DeviceADObjectId == null || !DeviceBehaviorCache.TryGetAndRemoveValue(userGuid, deviceIdentity, out deviceBehavior))
			{
				if (protocolLogger != null)
				{
					protocolLogger.SetValue(ProtocolLoggerData.DeviceBehaviorLoaded, 7);
				}
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, traceObject, "No device in cache, return GlobalInfo.DeviceBehavior");
				return globalInfo.DeviceBehavior;
			}
			if (deviceBehavior.AutoBlockReason != globalInfo.DeviceBehavior.AutoBlockReason)
			{
				if (protocolLogger != null)
				{
					protocolLogger.SetValue(ProtocolLoggerData.DeviceBehaviorLoaded, 1);
				}
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, traceObject, "AutoBlockReason changed, return GlobalInfo.DeviceBehavior");
				return globalInfo.DeviceBehavior;
			}
			int num = globalInfo.DeviceBehavior.IsNewerThan(deviceBehavior.WhenLoaded);
			if (num > -1)
			{
				string arg = DeviceBehavior.dateCollections[num];
				if (protocolLogger != null)
				{
					protocolLogger.SetValue(ProtocolLoggerData.DeviceBehaviorLoaded, num + 2);
				}
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, traceObject, "{0} is newer, return GlobalInfo.DeviceBehavior", arg);
				return globalInfo.DeviceBehavior;
			}
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, traceObject, "Return cached DeviceBehavior");
			deviceBehavior.Owner = globalInfo;
			deviceBehavior.ProtocolLogger = protocolLogger;
			return deviceBehavior;
		}

		public bool Validate()
		{
			lock (this.instanceLock)
			{
				if (!Enum.IsDefined(typeof(DeviceAccessStateReason), this.autoBlockReason))
				{
					AirSyncDiagnostics.TraceInfo<DeviceAccessStateReason>(ExTraceGlobals.RequestsTracer, this, "Invalid AutoBlockReason {0}", this.autoBlockReason);
					return false;
				}
				if (this.userAgentTimes == null)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "No UserAgentTimes list");
					return false;
				}
				if (this.userAgentStrings == null)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "No UserAgentStrings list");
					return false;
				}
				if (this.userAgentTimes.Count != this.userAgentStrings.Count)
				{
					AirSyncDiagnostics.TraceInfo<int, int>(ExTraceGlobals.RequestsTracer, this, "Lengths of UserAgentTimes ({0}) and UserAgentStrings ({1}) don't match", this.userAgentTimes.Count, this.userAgentStrings.Count);
					return false;
				}
				if (this.recentCommandTimes == null)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "No RecentCommandTimes list");
					return false;
				}
				if (this.recentCommandHashCodes == null)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "No RecentCommandHashcodes list");
					return false;
				}
				if (this.recentCommandTimes.Count != this.recentCommandHashCodes.Count)
				{
					AirSyncDiagnostics.TraceInfo<int, int>(ExTraceGlobals.RequestsTracer, this, "Lengths of RecentCommandTimes ({0}) and RecentCommandHashcodes ({1}) don't match", this.recentCommandTimes.Count, this.recentCommandHashCodes.Count);
					return false;
				}
				if (this.syncTimes == null)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "No SyncTimes list");
					return false;
				}
				if (this.syncKeys == null)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "No SyncKeys list");
					return false;
				}
				if (this.syncTimes.Count != this.syncKeys.Count)
				{
					AirSyncDiagnostics.TraceInfo<int, int>(ExTraceGlobals.RequestsTracer, this, "Lengths of SyncTimes ({0}) and SyncKeys ({1}) don't match", this.syncTimes.Count, this.syncKeys.Count);
					return false;
				}
				if (this.watsons == null)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "No Watsons list");
					return false;
				}
				if (this.outOfBudgets == null)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "No OutOfBudgets list");
					return false;
				}
			}
			return true;
		}

		public void RecordNewUserAgent(string newUserAgent)
		{
			EnhancedTimeSpan behaviorTypeIncidenceDuration = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.UserAgentsChanges).BehaviorTypeIncidenceDuration;
			lock (this.instanceLock)
			{
				if (newUserAgent != null && !this.userAgentStrings.Contains(newUserAgent))
				{
					ExDateTime utcNow = ExDateTime.UtcNow;
					ExDateTime windowStartTime = utcNow - behaviorTypeIncidenceDuration;
					DeviceBehavior.ClearOldRecords(windowStartTime, this.userAgentTimes, this.userAgentStrings);
					this.userAgentTimes.Add(utcNow);
					this.userAgentStrings.Add(newUserAgent);
					this.SaveDeviceBehavior(false);
					int behaviorTypeIncidenceLimit = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.UserAgentsChanges).BehaviorTypeIncidenceLimit;
					if (behaviorTypeIncidenceLimit > 0 && this.userAgentTimes.Count > behaviorTypeIncidenceLimit)
					{
						this.BlockDevice(AutoblockThresholdType.UserAgentsChanges);
					}
					else if (utcNow > this.blockTime && utcNow < this.nextUnblockTime)
					{
						this.UnblockDevice();
					}
				}
			}
		}

		public void RecordCommand(Command command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "RecordCommand {0}", command.GetType().Name);
			int num;
			if (command.Request.CommandXml == null)
			{
				num = command.Request.CommandType.ToString().GetHashCode();
				foreach (string text in command.Request.GetRawHttpRequest().QueryString.AllKeys)
				{
					num ^= text.GetHashCode();
					num ^= command.Request.GetRawHttpRequest().QueryString[text].GetHashCode();
				}
				if (command.InputStream != null)
				{
					int num2 = 0;
					uint num3 = 0U;
					command.InputStream.Seek(0L, SeekOrigin.Begin);
					int num4;
					while ((num4 = command.InputStream.ReadByte()) != -1)
					{
						if (num2 < 4)
						{
							num3 = (num3 << 8) + (uint)((byte)num4);
							num2++;
						}
						else
						{
							num ^= num3.GetHashCode();
							num3 = 0U;
							num2 = 0;
						}
					}
					if (num2 > 0)
					{
						num ^= num3.GetHashCode();
					}
					command.InputStream.Seek(0L, SeekOrigin.Begin);
				}
			}
			else
			{
				num = command.Request.CommandXml.OuterXml.ToString().GetHashCode();
			}
			this.RecordCommand(num);
		}

		public void RecordCommand(int commandHashcode)
		{
			AirSyncDiagnostics.TraceInfo<int>(ExTraceGlobals.RequestsTracer, this, "RecordCommand HC:{0}", commandHashcode);
			ExDateTime utcNow = ExDateTime.UtcNow;
			DeviceAutoBlockThreshold autoBlockThreshold = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.RecentCommands);
			EnhancedTimeSpan behaviorTypeIncidenceDuration = autoBlockThreshold.BehaviorTypeIncidenceDuration;
			if (behaviorTypeIncidenceDuration < ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.CommandFrequency).BehaviorTypeIncidenceDuration)
			{
				behaviorTypeIncidenceDuration = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.CommandFrequency).BehaviorTypeIncidenceDuration;
			}
			ExDateTime exDateTime = utcNow - behaviorTypeIncidenceDuration;
			lock (this.instanceLock)
			{
				DeviceBehavior.ClearOldRecords(exDateTime, this.recentCommandTimes, this.recentCommandHashCodes);
				this.recentCommandTimes.Add(utcNow);
				this.recentCommandHashCodes.Add(commandHashcode);
				this.SaveDeviceBehavior(false);
				if (this.ProtocolLogger != null)
				{
					this.ProtocolLogger.SetValue(ProtocolLoggerData.CommandHashCode, commandHashcode);
				}
				if (!this.BlockOnFrequency())
				{
					exDateTime = utcNow - autoBlockThreshold.BehaviorTypeIncidenceDuration;
					int num = 0;
					for (int i = 0; i < this.recentCommandTimes.Count; i++)
					{
						if (!(this.recentCommandTimes[i] < exDateTime) && this.recentCommandHashCodes[i] == commandHashcode)
						{
							num++;
						}
					}
					int behaviorTypeIncidenceLimit = autoBlockThreshold.BehaviorTypeIncidenceLimit;
					if (behaviorTypeIncidenceLimit > 0 && num > behaviorTypeIncidenceLimit)
					{
						this.BlockDevice(AutoblockThresholdType.RecentCommands);
					}
				}
			}
		}

		public void RecordWatson()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "RecordWatson");
			ExDateTime utcNow = ExDateTime.UtcNow;
			DeviceAutoBlockThreshold autoBlockThreshold = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.Watsons);
			ExDateTime windowStartTime = utcNow - autoBlockThreshold.BehaviorTypeIncidenceDuration;
			lock (this.instanceLock)
			{
				DeviceBehavior.ClearOldRecords(windowStartTime, this.watsons);
				this.watsons.Add(utcNow);
				this.SaveDeviceBehavior(false);
				int behaviorTypeIncidenceLimit = autoBlockThreshold.BehaviorTypeIncidenceLimit;
				if (behaviorTypeIncidenceLimit > 0 && this.watsons.Count > behaviorTypeIncidenceLimit)
				{
					this.BlockDevice(AutoblockThresholdType.Watsons);
				}
			}
		}

		public void RecordOutOfBudget()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "RecordOutOfBudget");
			ExDateTime utcNow = ExDateTime.UtcNow;
			DeviceAutoBlockThreshold autoBlockThreshold = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.OutOfBudgets);
			ExDateTime windowStartTime = utcNow - autoBlockThreshold.BehaviorTypeIncidenceDuration;
			lock (this.instanceLock)
			{
				DeviceBehavior.ClearOldRecords(windowStartTime, this.outOfBudgets);
				this.outOfBudgets.Add(utcNow);
				this.SaveDeviceBehavior(false);
				int behaviorTypeIncidenceLimit = autoBlockThreshold.BehaviorTypeIncidenceLimit;
				if (behaviorTypeIncidenceLimit > 0 && this.outOfBudgets.Count > behaviorTypeIncidenceLimit)
				{
					this.BlockDevice(AutoblockThresholdType.OutOfBudgets);
				}
			}
		}

		public void AddSyncKey(ExDateTime syncAttemptTime, string collectionId, uint syncKey)
		{
			AirSyncDiagnostics.TraceInfo<string, uint>(ExTraceGlobals.RequestsTracer, this, "AddSyncKey collectionId:{0} syncKey:{1}", collectionId, syncKey);
			DeviceAutoBlockThreshold autoBlockThreshold = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.SyncCommands);
			lock (this.instanceLock)
			{
				int num = this.syncTimes.Count - 1;
				while (num > -1 && !(this.syncTimes[num] == syncAttemptTime))
				{
					num--;
				}
				if (num == -1)
				{
					this.RecordSyncCommand(syncAttemptTime);
					num = this.syncTimes.Count - 1;
					if (this.syncTimes[num] != syncAttemptTime)
					{
						throw new InvalidOperationException("Recently added sync record is not the last one!");
					}
					if (this.BlockOnFrequency())
					{
						return;
					}
				}
				int num2 = this.syncKeys[num] ^ ((collectionId != null) ? collectionId.GetHashCode() : 0) ^ syncKey.GetHashCode();
				this.syncKeys[num] = num2;
				AirSyncDiagnostics.TraceInfo<int>(ExTraceGlobals.RequestsTracer, this, "AddSyncKey new hashcode {0}", num2);
				if (this.ProtocolLogger != null)
				{
					this.ProtocolLogger.SetValue(ProtocolLoggerData.SyncHashCode, num2);
				}
				int num3 = 0;
				ExDateTime t = syncAttemptTime - autoBlockThreshold.BehaviorTypeIncidenceDuration;
				for (int i = 0; i < this.syncTimes.Count; i++)
				{
					if (!(this.syncTimes[i] < t) && this.syncKeys[i] == num2)
					{
						num3++;
					}
				}
				int behaviorTypeIncidenceLimit = autoBlockThreshold.BehaviorTypeIncidenceLimit;
				if (behaviorTypeIncidenceLimit > 0 && num3 >= behaviorTypeIncidenceLimit)
				{
					this.BlockDevice(AutoblockThresholdType.SyncCommands);
				}
				else if (this.AutoBlockReason != DeviceAccessStateReason.Unknown)
				{
					this.UnblockDevice();
				}
			}
		}

		public DeviceAccessStateReason IsDeviceAutoBlocked(string userAgent)
		{
			TimeSpan timeSpan;
			return this.IsDeviceAutoBlocked(userAgent, out timeSpan);
		}

		public DeviceAccessStateReason IsDeviceAutoBlocked(string userAgent, out TimeSpan blockTime)
		{
			blockTime = TimeSpan.Zero;
			DeviceAccessStateReason result;
			lock (this.instanceLock)
			{
				if ((string.IsNullOrEmpty(userAgent) || this.userAgentStrings.Contains(userAgent)) && this.AutoBlockReason >= DeviceAccessStateReason.UserAgentsChanges && this.nextUnblockTime > ExDateTime.UtcNow && this.blockTime > ADNotificationManager.GetAutoBlockThreshold(this.AutoBlockReason).LastChangeTime)
				{
					blockTime = this.nextUnblockTime - ExDateTime.UtcNow;
					result = this.AutoBlockReason;
				}
				else
				{
					result = DeviceAccessStateReason.Unknown;
				}
			}
			return result;
		}

		public DeviceAccessStateReason IsDeviceAutoBlocked(ExDateTime requestTime, out TimeSpan blockTime)
		{
			blockTime = TimeSpan.Zero;
			ExDateTime t = (this.AutoBlockReason != DeviceAccessStateReason.Unknown) ? ADNotificationManager.GetAutoBlockThreshold(this.AutoBlockReason).LastChangeTime : ExDateTime.MinValue;
			DeviceAccessStateReason result;
			lock (this.instanceLock)
			{
				if (this.AutoBlockReason != DeviceAccessStateReason.Unknown)
				{
					if (this.blockTime < t)
					{
						this.UnblockDevice();
					}
					else if (ExDateTime.UtcNow >= this.nextUnblockTime)
					{
						this.UnblockDevice();
					}
					else
					{
						blockTime = this.nextUnblockTime - requestTime;
					}
				}
				result = this.AutoBlockReason;
			}
			return result;
		}

		public BackOffValue GetAutoBlockBackOffTime()
		{
			BackOffValue autoBlockBackOffTimeForSyncCommands = this.GetAutoBlockBackOffTimeForSyncCommands();
			BackOffValue autoBlockBackOffTimeForCommandsFrequency = this.GetAutoBlockBackOffTimeForCommandsFrequency();
			if (autoBlockBackOffTimeForSyncCommands.CompareTo(autoBlockBackOffTimeForCommandsFrequency) > 0)
			{
				AirSyncDiagnostics.TraceInfo<double, BackOffType, string>(ExTraceGlobals.RequestsTracer, this, "GetAutoblockBackOffTime: BackOffTime-{0} sec, BackOffType-{1}, BackOffReason:{2}", autoBlockBackOffTimeForSyncCommands.BackOffDuration, autoBlockBackOffTimeForSyncCommands.BackOffType, autoBlockBackOffTimeForSyncCommands.BackOffReason);
				return autoBlockBackOffTimeForSyncCommands;
			}
			AirSyncDiagnostics.TraceInfo<double, BackOffType, string>(ExTraceGlobals.RequestsTracer, this, "GetAutoblockBackOffTime: BackOffTime-{0} sec, BackOffType-{1}, BackOffReason:{2}", autoBlockBackOffTimeForCommandsFrequency.BackOffDuration, autoBlockBackOffTimeForCommandsFrequency.BackOffType, autoBlockBackOffTimeForCommandsFrequency.BackOffReason);
			return autoBlockBackOffTimeForCommandsFrequency;
		}

		private BackOffValue GetAutoBlockBackOffTimeForSyncCommands()
		{
			int behaviorTypeIncidenceLimit = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.SyncCommands).BehaviorTypeIncidenceLimit;
			EnhancedTimeSpan behaviorTypeIncidenceDuration = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.SyncCommands).BehaviorTypeIncidenceDuration;
			if (behaviorTypeIncidenceDuration.TotalSeconds == 0.0)
			{
				return BackOffValue.NoBackOffValue;
			}
			if (this.syncTimes == null || this.syncTimes.Count == 0 || this.syncTimes.Count <= behaviorTypeIncidenceLimit / 2)
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "GetAutoblockBackOffTime: Skip calculating backOff time.");
				return new BackOffValue
				{
					BackOffDuration = -1.0 * behaviorTypeIncidenceDuration.TotalSeconds,
					BackOffType = BackOffType.Low,
					BackOffReason = AutoblockThresholdType.SyncCommands.ToString()
				};
			}
			TimeSpan currentDuration = ExDateTime.UtcNow.Subtract(this.syncTimes[0]);
			BackOffValue backOffValue = this.CalculateAutoBlockBackOffTime(behaviorTypeIncidenceLimit, behaviorTypeIncidenceDuration, this.syncTimes.Count, currentDuration, AutoblockThresholdType.SyncCommands.ToString());
			AirSyncDiagnostics.TraceInfo<int, double>(ExTraceGlobals.RequestsTracer, this, "GetAutoblockBackOffTime: SyncCommandLimit:{0}, backOffDuration:{1}", behaviorTypeIncidenceLimit, backOffValue.BackOffDuration);
			return backOffValue;
		}

		private BackOffValue GetAutoBlockBackOffTimeForCommandsFrequency()
		{
			EnhancedTimeSpan behaviorTypeIncidenceDuration = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.CommandFrequency).BehaviorTypeIncidenceDuration;
			if (behaviorTypeIncidenceDuration.TotalSeconds == 0.0)
			{
				return BackOffValue.NoBackOffValue;
			}
			BackOffValue backOffValue;
			if ((this.recentCommandTimes != null && this.recentCommandTimes.Count >= 0) || (this.syncTimes != null && this.syncTimes.Count >= 0))
			{
				int behaviorTypeIncidenceLimit = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.CommandFrequency).BehaviorTypeIncidenceLimit;
				ExDateTime exDateTime = ExDateTime.MaxValue;
				ExDateTime exDateTime2 = ExDateTime.MaxValue;
				int num = 0;
				if (this.syncTimes != null && this.syncKeys.Count > 0)
				{
					num += this.syncTimes.Count;
					exDateTime = this.syncTimes[0];
				}
				if (this.recentCommandTimes != null && this.recentCommandTimes.Count > 0)
				{
					num += this.recentCommandTimes.Count;
					exDateTime2 = this.recentCommandTimes[0];
				}
				TimeSpan currentDuration = (exDateTime < exDateTime2) ? ExDateTime.UtcNow.Subtract(exDateTime) : ExDateTime.UtcNow.Subtract(exDateTime2);
				backOffValue = this.CalculateAutoBlockBackOffTime(behaviorTypeIncidenceLimit, behaviorTypeIncidenceDuration, num, currentDuration, AutoblockThresholdType.CommandFrequency.ToString());
				AirSyncDiagnostics.TraceInfo<int, double>(ExTraceGlobals.RequestsTracer, this, "GetAutoblockBackOffTime: SyncCommandLimit:{0}, backOffDuration:{1}", behaviorTypeIncidenceLimit, backOffValue.BackOffDuration);
			}
			else
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "GetAutoblockBackOffTime: Skip calculating backOff time.");
				backOffValue = new BackOffValue
				{
					BackOffType = BackOffType.Low,
					BackOffReason = AutoblockThresholdType.CommandFrequency.ToString(),
					BackOffDuration = -1.0 * behaviorTypeIncidenceDuration.TotalSeconds
				};
			}
			return backOffValue;
		}

		private BackOffValue CalculateAutoBlockBackOffTime(int abThresholdLimit, EnhancedTimeSpan abThresholdDuration, int currentCmdCount, TimeSpan currentDuration, string backOffReason)
		{
			double num = -1.0 * abThresholdDuration.TotalSeconds;
			BackOffType backOffType = BackOffType.Low;
			if (abThresholdLimit > 0 && (double)currentCmdCount > (double)abThresholdLimit * GlobalSettings.AutoblockBackOffMediumThreshold)
			{
				double num2 = (abThresholdDuration.TotalSeconds > 0.0) ? ((double)abThresholdLimit * currentDuration.TotalSeconds / abThresholdDuration.TotalSeconds) : 0.0;
				if ((double)currentCmdCount > num2)
				{
					num = (double)currentCmdCount * abThresholdDuration.TotalSeconds / (double)abThresholdLimit - currentDuration.TotalSeconds;
				}
				backOffType = (((double)currentCmdCount > (double)abThresholdLimit * GlobalSettings.AutoblockBackOffHighThreshold) ? BackOffType.High : BackOffType.Medium);
				AirSyncDiagnostics.TraceInfo<double>(ExTraceGlobals.RequestsTracer, this, "GetAutoblockBackOffTime: Command Frequency backOff time ", num);
			}
			return new BackOffValue
			{
				BackOffDuration = num,
				BackOffReason = backOffReason,
				BackOffType = backOffType
			};
		}

		private static void ClearOldRecords(ExDateTime windowStartTime, List<ExDateTime> times)
		{
			DeviceBehavior.ClearOldRecords(windowStartTime, times, null);
		}

		internal int IsNewerThan(ExDateTime timeToCheck)
		{
			lock (this.instanceLock)
			{
				if (this.userAgentTimes.Count > 0 && this.userAgentTimes[this.userAgentTimes.Count - 1] > timeToCheck)
				{
					return 0;
				}
				if (this.recentCommandTimes.Count > 0 && this.recentCommandTimes[this.recentCommandTimes.Count - 1] > timeToCheck)
				{
					return 1;
				}
				if (this.watsons.Count > 0 && this.watsons[this.watsons.Count - 1] > timeToCheck)
				{
					return 2;
				}
				if (this.outOfBudgets.Count > 0 && this.outOfBudgets[this.outOfBudgets.Count - 1] > timeToCheck)
				{
					return 3;
				}
				if (this.syncTimes.Count > 0 && this.syncTimes[this.syncTimes.Count - 1] > timeToCheck)
				{
					return 4;
				}
			}
			return -1;
		}

		private static void ClearOldRecords(ExDateTime windowStartTime, List<ExDateTime> times, object secondaryList)
		{
			if (times == null || times.Count == 0)
			{
				return;
			}
			int num = 0;
			while (num < times.Count && times[num] < windowStartTime)
			{
				num++;
			}
			if (num > 0)
			{
				times.RemoveRange(0, num);
				if (secondaryList != null)
				{
					List<int> list = secondaryList as List<int>;
					if (list != null)
					{
						list.RemoveRange(0, num);
						return;
					}
					List<string> list2 = secondaryList as List<string>;
					if (list2 != null)
					{
						list2.RemoveRange(0, num);
						return;
					}
					throw new InvalidOperationException(string.Format("Expected secondary list to be either List<int> or List<string> but encountered '{0}'", secondaryList.GetType().FullName));
				}
			}
		}

		private void RecordSyncCommand(ExDateTime syncAttemptTime)
		{
			AirSyncDiagnostics.TraceInfo<ExDateTime>(ExTraceGlobals.RequestsTracer, this, "RecordSyncCommand syncAttemptTime:{0:o}", syncAttemptTime);
			ExDateTime utcNow = ExDateTime.UtcNow;
			EnhancedTimeSpan behaviorTypeIncidenceDuration = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.SyncCommands).BehaviorTypeIncidenceDuration;
			if (behaviorTypeIncidenceDuration < ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.CommandFrequency).BehaviorTypeIncidenceDuration)
			{
				behaviorTypeIncidenceDuration = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.CommandFrequency).BehaviorTypeIncidenceDuration;
			}
			ExDateTime windowStartTime = utcNow - behaviorTypeIncidenceDuration;
			lock (this.instanceLock)
			{
				DeviceBehavior.ClearOldRecords(windowStartTime, this.syncTimes, this.syncKeys);
				this.syncTimes.Add(syncAttemptTime);
				this.syncKeys.Add(0);
				this.SaveDeviceBehavior(false);
			}
		}

		private bool BlockOnFrequency()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "BlockOnFrequency");
			int behaviorTypeIncidenceLimit = ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.CommandFrequency).BehaviorTypeIncidenceLimit;
			if (behaviorTypeIncidenceLimit == 0)
			{
				return false;
			}
			ExDateTime utcNow = ExDateTime.UtcNow;
			ExDateTime t = utcNow - ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.CommandFrequency).BehaviorTypeIncidenceDuration;
			int num = 0;
			lock (this.instanceLock)
			{
				foreach (ExDateTime t2 in this.recentCommandTimes)
				{
					if (t2 > t)
					{
						num++;
					}
				}
				if (num >= behaviorTypeIncidenceLimit)
				{
					this.BlockDevice(AutoblockThresholdType.CommandFrequency);
					return true;
				}
				foreach (ExDateTime t3 in this.syncTimes)
				{
					if (t3 > t)
					{
						num++;
					}
				}
				if (num >= behaviorTypeIncidenceLimit)
				{
					this.BlockDevice(AutoblockThresholdType.CommandFrequency);
					return true;
				}
			}
			return false;
		}

		private void BlockDevice(AutoblockThresholdType autoblockThresholdType)
		{
			AirSyncDiagnostics.TraceInfo<AutoblockThresholdType>(ExTraceGlobals.RequestsTracer, this, "BlockDevice {0}", autoblockThresholdType);
			EnhancedTimeSpan deviceBlockDuration = ADNotificationManager.GetAutoBlockThreshold(autoblockThresholdType).DeviceBlockDuration;
			ExDateTime utcNow = ExDateTime.UtcNow;
			lock (this.instanceLock)
			{
				if (this.blockTime > utcNow || this.nextUnblockTime < utcNow)
				{
					this.blockTime = utcNow;
					this.nextUnblockTime = utcNow + deviceBlockDuration;
					this.autoBlockReason = DeviceAccessStateReason.UserAgentsChanges + (int)autoblockThresholdType;
					this.timeToUpdateAD = utcNow;
					this.SaveDeviceBehavior(true);
					if (deviceBlockDuration != EnhancedTimeSpan.Zero)
					{
						AirSyncCounters.AutoBlockedDevices.Increment();
					}
					if (this.ProtocolLogger != null)
					{
						this.ProtocolLogger.SetValue(ProtocolLoggerData.AutoBlockEvent, autoblockThresholdType.ToString());
					}
				}
			}
		}

		private void UnblockDevice()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "UnblockDevice");
			TimeSpan timeSpan = ADNotificationManager.Started ? (ADNotificationManager.GetAutoBlockThreshold(this.AutoBlockReason).BehaviorTypeIncidenceDuration * (long)GlobalSettings.AutoBlockADWriteDelay) : TimeSpan.Zero;
			lock (this.instanceLock)
			{
				switch (this.autoBlockReason)
				{
				case DeviceAccessStateReason.UserAgentsChanges:
					this.userAgentTimes.Clear();
					this.userAgentStrings.Clear();
					break;
				case DeviceAccessStateReason.RecentCommands:
					this.recentCommandTimes.Clear();
					this.recentCommandHashCodes.Clear();
					break;
				case DeviceAccessStateReason.Watsons:
					this.watsons.Clear();
					break;
				case DeviceAccessStateReason.OutOfBudgets:
					this.outOfBudgets.Clear();
					break;
				case DeviceAccessStateReason.SyncCommands:
					this.syncTimes.Clear();
					this.syncKeys.Clear();
					break;
				case DeviceAccessStateReason.CommandFrequency:
					this.recentCommandTimes.Clear();
					this.recentCommandHashCodes.Clear();
					this.syncTimes.Clear();
					this.syncKeys.Clear();
					break;
				}
				if (timeSpan > DeviceBehavior.MaxADUpdateDelay)
				{
					timeSpan = DeviceBehavior.MaxADUpdateDelay;
				}
				this.timeToUpdateAD = ExDateTime.UtcNow + timeSpan;
				this.nextUnblockTime = ExDateTime.MinValue;
				this.autoBlockReason = DeviceAccessStateReason.Unknown;
				this.SaveDeviceBehavior(true);
			}
		}

		private void SaveDeviceBehavior(bool forceSave)
		{
			AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, this, "SaveDeviceBehavior forceSave:{0}", forceSave);
			if (this.Owner != null && !this.Owner.IsDisposed)
			{
				if (this.CacheToken == null)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "CacheToken is null, skip caching");
					forceSave = true;
				}
				else
				{
					forceSave = (forceSave || DeviceBehaviorCache.ContainsKey(this.CacheToken));
					DeviceBehaviorCache.AddOrReplace(this.CacheToken, this);
				}
				if (!forceSave)
				{
					return;
				}
				if (this.ProtocolLogger != null)
				{
					this.ProtocolLogger.SetValue(ProtocolLoggerData.DeviceBehaviorSaved, 1);
				}
				try
				{
					this.Owner.DeviceBehavior = this;
					this.Owner.IsDirty = true;
					return;
				}
				catch (ObjectDisposedException)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SaveDeviceBehavior owner is disposed");
					return;
				}
			}
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "SaveDeviceBehavior owner {0}", (this.Owner == null) ? "not set" : "disposed");
		}

		internal void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			lock (this.instanceLock)
			{
				if (!this.Validate())
				{
					throw new InvalidOperationException("DeviceBehavior data is invalid prior to serialization");
				}
				GenericListData<DateTimeData, ExDateTime> genericListData = new GenericListData<DateTimeData, ExDateTime>(this.userAgentTimes);
				genericListData.SerializeData(writer, componentDataPool);
				GenericListData<StringData, string> genericListData2 = new GenericListData<StringData, string>(this.userAgentStrings);
				genericListData2.SerializeData(writer, componentDataPool);
				GenericListData<DateTimeData, ExDateTime> genericListData3 = new GenericListData<DateTimeData, ExDateTime>(this.recentCommandTimes);
				genericListData3.SerializeData(writer, componentDataPool);
				GenericListData<Int32Data, int> genericListData4 = new GenericListData<Int32Data, int>(this.recentCommandHashCodes);
				genericListData4.SerializeData(writer, componentDataPool);
				GenericListData<DateTimeData, ExDateTime> genericListData5 = new GenericListData<DateTimeData, ExDateTime>(this.watsons);
				genericListData5.SerializeData(writer, componentDataPool);
				GenericListData<DateTimeData, ExDateTime> genericListData6 = new GenericListData<DateTimeData, ExDateTime>(this.outOfBudgets);
				genericListData6.SerializeData(writer, componentDataPool);
				GenericListData<DateTimeData, ExDateTime> genericListData7 = new GenericListData<DateTimeData, ExDateTime>(this.syncTimes);
				genericListData7.SerializeData(writer, componentDataPool);
				GenericListData<Int32Data, int> genericListData8 = new GenericListData<Int32Data, int>(this.syncKeys);
				genericListData8.SerializeData(writer, componentDataPool);
				componentDataPool.GetDateTimeDataInstance().Bind(this.blockTime).SerializeData(writer, componentDataPool);
				componentDataPool.GetDateTimeDataInstance().Bind(this.nextUnblockTime).SerializeData(writer, componentDataPool);
				componentDataPool.GetInt32DataInstance().Bind((int)this.autoBlockReason).SerializeData(writer, componentDataPool);
				componentDataPool.GetDateTimeDataInstance().Bind(this.timeToUpdateAD).SerializeData(writer, componentDataPool);
			}
		}

		internal void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			lock (this.instanceLock)
			{
				GenericListData<DateTimeData, ExDateTime> genericListData = new GenericListData<DateTimeData, ExDateTime>();
				genericListData.DeserializeData(reader, componentDataPool);
				this.userAgentTimes = genericListData.Data;
				GenericListData<StringData, string> genericListData2 = new GenericListData<StringData, string>();
				genericListData2.DeserializeData(reader, componentDataPool);
				this.userAgentStrings = genericListData2.Data;
				GenericListData<DateTimeData, ExDateTime> genericListData3 = new GenericListData<DateTimeData, ExDateTime>();
				genericListData3.DeserializeData(reader, componentDataPool);
				this.recentCommandTimes = genericListData3.Data;
				GenericListData<Int32Data, int> genericListData4 = new GenericListData<Int32Data, int>();
				genericListData4.DeserializeData(reader, componentDataPool);
				this.recentCommandHashCodes = genericListData4.Data;
				GenericListData<DateTimeData, ExDateTime> genericListData5 = new GenericListData<DateTimeData, ExDateTime>();
				genericListData5.DeserializeData(reader, componentDataPool);
				this.watsons = genericListData5.Data;
				GenericListData<DateTimeData, ExDateTime> genericListData6 = new GenericListData<DateTimeData, ExDateTime>();
				genericListData6.DeserializeData(reader, componentDataPool);
				this.outOfBudgets = genericListData6.Data;
				GenericListData<DateTimeData, ExDateTime> genericListData7 = new GenericListData<DateTimeData, ExDateTime>();
				genericListData7.DeserializeData(reader, componentDataPool);
				this.syncTimes = genericListData7.Data;
				GenericListData<Int32Data, int> genericListData8 = new GenericListData<Int32Data, int>();
				genericListData8.DeserializeData(reader, componentDataPool);
				this.syncKeys = genericListData8.Data;
				DateTimeData dateTimeDataInstance = componentDataPool.GetDateTimeDataInstance();
				dateTimeDataInstance.DeserializeData(reader, componentDataPool);
				this.blockTime = dateTimeDataInstance.Data;
				DateTimeData dateTimeDataInstance2 = componentDataPool.GetDateTimeDataInstance();
				dateTimeDataInstance2.DeserializeData(reader, componentDataPool);
				this.nextUnblockTime = dateTimeDataInstance2.Data;
				Int32Data int32DataInstance = componentDataPool.GetInt32DataInstance();
				int32DataInstance.DeserializeData(reader, componentDataPool);
				this.autoBlockReason = (DeviceAccessStateReason)int32DataInstance.Data;
				DateTimeData dateTimeDataInstance3 = componentDataPool.GetDateTimeDataInstance();
				dateTimeDataInstance3.DeserializeData(reader, componentDataPool);
				this.timeToUpdateAD = dateTimeDataInstance3.Data;
				if (!this.Validate())
				{
					throw new CorruptSyncStateException(new LocalizedString("DeviceBehavior.DeserializeData"), null);
				}
			}
		}

		private const int UserAgentTimesIndex = 0;

		private const int RecentCommandTimesIndex = 1;

		private const int WatsonsIndex = 2;

		private const int OutOfBudgetsIndex = 3;

		private const int SyncTimesIndex = 4;

		private static readonly string[] dateCollections = new string[]
		{
			"UserAgentTimes",
			"RecentCommandTimes",
			"Watsons",
			"OutOfBudgets",
			"SyncTimes"
		};

		private static TimeSpan? maxADUpdateDelay;

		private object instanceLock = new object();

		private List<ExDateTime> userAgentTimes;

		private List<string> userAgentStrings;

		private List<ExDateTime> recentCommandTimes;

		private List<int> recentCommandHashCodes;

		private List<ExDateTime> watsons;

		private List<ExDateTime> outOfBudgets;

		private List<ExDateTime> syncTimes;

		private List<int> syncKeys;

		private ExDateTime blockTime = ExDateTime.MinValue;

		private ExDateTime nextUnblockTime = ExDateTime.MinValue;

		private DeviceAccessStateReason autoBlockReason;

		private ExDateTime timeToUpdateAD = ExDateTime.MinValue;

		private GlobalInfo owner;
	}
}
