using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.AirSync
{
	internal class DeviceClassCache
	{
		protected DeviceClassCache()
		{
			this.cache = new Dictionary<OrganizationId, DeviceClassCache.DeviceClassDataSet>();
			this.timerStartTime = ExDateTime.MinValue;
			this.realTimeRefresh = false;
		}

		public static DeviceClassCache Instance
		{
			get
			{
				return DeviceClassCache.instance;
			}
		}

		public bool Started
		{
			get
			{
				return this.timerStartTime != ExDateTime.MinValue;
			}
		}

		public static string NormalizeDeviceClass(string deviceClass)
		{
			if (string.IsNullOrEmpty(deviceClass))
			{
				return deviceClass;
			}
			if (deviceClass.StartsWith("IMEI", StringComparison.OrdinalIgnoreCase))
			{
				return "IMEI";
			}
			return deviceClass;
		}

		public void Start()
		{
			if (this.Started)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "DeviceClassCache is already started.");
				return;
			}
			if (GlobalSettings.DeviceClassCachePerOrgRefreshInterval == 0)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "DeviceClassCache is turned off.");
				return;
			}
			TimeSpan startDelay = DeviceClassCache.GetStartDelay();
			this.timerStartTime = ExDateTime.UtcNow + startDelay;
			this.refreshTimer = new GuardedTimer(new TimerCallback(this.Refresh), null, startDelay, TimeSpan.FromSeconds((double)DeviceClassCache.TimerKickinInterval));
			AirSyncDiagnostics.TraceDebug<ExDateTime, ExDateTime>(ExTraceGlobals.RequestsTracer, this, "DeviceClassCache is started at '{0}-UTC'.  The internal timer will be started at '{1}-UTC'.", ExDateTime.UtcNow, this.timerStartTime);
		}

		public void Stop()
		{
			if (this.refreshTimer != null)
			{
				this.refreshTimer.Dispose(true);
				this.refreshTimer = null;
			}
			if (this.cache != null)
			{
				foreach (DeviceClassCache.DeviceClassDataSet deviceClassDataSet in this.cache.Values)
				{
					deviceClassDataSet.Dispose();
				}
				this.cache.Clear();
			}
			this.timerStartTime = ExDateTime.MinValue;
			AirSyncDiagnostics.TraceDebug<ExDateTime>(ExTraceGlobals.RequestsTracer, this, "DeviceClassCache is stoped at '{0}-UTC'.", ExDateTime.UtcNow);
		}

		public void Add(OrganizationId organizationId, string deviceType, string deviceModel)
		{
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Adding device class: orgId={0}, deviceType={1}, deviceModel={2}, started={3}.", new object[]
			{
				organizationId,
				deviceType,
				deviceModel,
				this.Started
			});
			if (!this.Started)
			{
				return;
			}
			if (string.IsNullOrEmpty(deviceType) || deviceType.Length > 32)
			{
				throw new ArgumentException("Invalid deviceType: " + deviceType);
			}
			if (string.IsNullOrEmpty(deviceModel))
			{
				throw new ArgumentNullException("deviceModel");
			}
			try
			{
				DeviceClassCache.DeviceClassData data = new DeviceClassCache.DeviceClassData(DeviceClassCache.EnforceLengthLimit(deviceType, DeviceClassCache.ADPropertyConstraintLength.MaxDeviceTypeLength, false), DeviceClassCache.EnforceLengthLimit(deviceModel, DeviceClassCache.ADPropertyConstraintLength.MaxDeviceModelLength, false));
				lock (this.thisLock)
				{
					DeviceClassCache.DeviceClassDataSet deviceClassDataSet;
					if (this.cache.TryGetValue(organizationId, out deviceClassDataSet))
					{
						if (!deviceClassDataSet.Contains(data))
						{
							if (deviceClassDataSet.Count >= GlobalSettings.DeviceClassPerOrgMaxADCount)
							{
								AirSyncDiagnostics.TraceDebug<OrganizationId, int>(ExTraceGlobals.RequestsTracer, this, "Device class will not be added to the cache since it already reaches the cap:orgId={0}, count={1}.", organizationId, deviceClassDataSet.Count);
							}
							else
							{
								deviceClassDataSet.Add(data);
								AirSyncDiagnostics.TraceDebug<OrganizationId>(ExTraceGlobals.RequestsTracer, this, "New device class is added to the existing org '{0}'.", organizationId);
							}
						}
					}
					else if (this.cache.Count >= GlobalSettings.ADCacheMaxOrgCount)
					{
						AirSyncDiagnostics.TraceDebug<OrganizationId, int>(ExTraceGlobals.RequestsTracer, this, "Device class set will not be added to the cache since it already reaches the cap:orgId={0}, count={1}.", organizationId, this.cache.Count);
					}
					else
					{
						deviceClassDataSet = new DeviceClassCache.DeviceClassDataSet(organizationId);
						deviceClassDataSet.Add(data);
						this.cache.Add(organizationId, deviceClassDataSet);
						AirSyncDiagnostics.TraceDebug<OrganizationId>(ExTraceGlobals.RequestsTracer, this, "New device class is added to the new org '{0}'.", organizationId);
					}
				}
			}
			finally
			{
				AirSyncDiagnostics.FaultInjectionTracer.TraceTest<bool>(2359700797U, ref this.realTimeRefresh);
				if (this.realTimeRefresh)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Calling Refresh real time.");
					this.Refresh(null);
					this.realTimeRefresh = false;
				}
			}
		}

		private static string EnforceLengthLimit(string deviceClassString, int constraintLength, bool generateUniqueName)
		{
			if (deviceClassString.Length <= constraintLength)
			{
				return deviceClassString;
			}
			if (generateUniqueName)
			{
				string text = Guid.NewGuid().ToString("N");
				if (constraintLength >= text.Length)
				{
					return deviceClassString.Substring(0, constraintLength - text.Length) + text;
				}
			}
			return deviceClassString.Substring(0, constraintLength);
		}

		private static TimeSpan GetStartDelay()
		{
			int seed;
			try
			{
				seed = Environment.MachineName.GetHashCode();
			}
			catch
			{
				seed = Environment.TickCount;
			}
			Random random = new Random(seed);
			return TimeSpan.FromSeconds((double)random.Next(0, (int)GlobalSettings.DeviceClassCacheMaxStartDelay.TotalSeconds));
		}

		private static void ProcessForADAdds(IConfigurationSession scopedSession, DeviceClassCache.DeviceClassDataSet localDataSet, DeviceClassCache.DeviceClassDataSet dataSetFromAD, ref int totalADWriteCount, int perOrgDeleteCount)
		{
			int num = 0;
			foreach (DeviceClassCache.DeviceClassData deviceClassData in localDataSet)
			{
				if (totalADWriteCount >= GlobalSettings.DeviceClassCacheMaxADUploadCount)
				{
					AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, null, "2. Stop updating AD because the cap is reached: adUpdateCount={0}", totalADWriteCount);
					break;
				}
				if (!dataSetFromAD.Contains(deviceClassData))
				{
					if (dataSetFromAD.Count + num - perOrgDeleteCount >= GlobalSettings.DeviceClassPerOrgMaxADCount)
					{
						AirSyncDiagnostics.TraceDebug<int, int>(ExTraceGlobals.RequestsTracer, null, "Stop adding new device class node to AD because the cap is reached: dataSetFromAD.Count={0}, DeviceClassPerOrgMaxADCount={1}", dataSetFromAD.Count, GlobalSettings.DeviceClassPerOrgMaxADCount);
						break;
					}
					if (dataSetFromAD.Count + num >= DeviceClassCache.ADCapWarningThreshold)
					{
						AirSyncDiagnostics.TraceDebug<int, int>(ExTraceGlobals.RequestsTracer, null, "Still adding new device class node to AD but the cap is close to be reached: dataSetFromAD.Count={0}, DeviceClassPerOrgMaxADCount={1}", dataSetFromAD.Count, DeviceClassCache.ADCapWarningThreshold);
						AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_TooManyDeviceClassNodes, localDataSet.OrganizationId.ToString(), new string[]
						{
							dataSetFromAD.Count.ToString(),
							localDataSet.OrganizationId.ToString(),
							GlobalSettings.DeviceClassPerOrgMaxADCount.ToString()
						});
					}
					DeviceClassCache.CreateOrUpdateActiveSyncDeviceClass(scopedSession, deviceClassData, localDataSet.OrganizationId);
					num++;
					totalADWriteCount++;
				}
			}
		}

		private static void ProcessForADCleanup(IConfigurationSession scopedSession, DeviceClassCache.DeviceClassDataSet localDataSet, DeviceClassCache.DeviceClassDataSet dataSetFromAD, ref int totalADWriteCount, ref int perOrgDeleteCount)
		{
			foreach (DeviceClassCache.DeviceClassData deviceClassData in dataSetFromAD)
			{
				if (totalADWriteCount >= GlobalSettings.DeviceClassCacheMaxADUploadCount)
				{
					AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, null, "3. Stop updating AD because the cap is reached: adUpdateCount={0}", totalADWriteCount);
					break;
				}
				if (localDataSet.Contains(deviceClassData))
				{
					if ((ExDateTime.UtcNow - deviceClassData.LastUpdateTime).Days >= TimeSpan.FromDays((double)GlobalSettings.DeviceClassCacheADCleanupInterval).Days)
					{
						AirSyncDiagnostics.TraceDebug<ExDateTime, int>(ExTraceGlobals.RequestsTracer, null, "Update or Create DeviceClass in AD. LastUpdateTime :{0}. adUpdateCount: {1}", deviceClassData.LastUpdateTime, totalADWriteCount);
						DeviceClassCache.CreateOrUpdateActiveSyncDeviceClass(scopedSession, deviceClassData, localDataSet.OrganizationId);
						totalADWriteCount++;
					}
				}
				else if ((ExDateTime.UtcNow - deviceClassData.LastUpdateTime).Days > TimeSpan.FromDays((double)GlobalSettings.DeviceClassCacheADCleanupInterval).Days * 2)
				{
					AirSyncDiagnostics.TraceDebug<ExDateTime, int>(ExTraceGlobals.RequestsTracer, null, "Delete DeviceClass in AD. LastUpdateTime :{0}, adUpdateCount: {1} ", deviceClassData.LastUpdateTime, totalADWriteCount);
					DeviceClassCache.DeleteActiveSyncDeviceClass(scopedSession, deviceClassData, localDataSet.OrganizationId);
					perOrgDeleteCount++;
					totalADWriteCount++;
				}
			}
		}

		private static ActiveSyncDeviceClasses GetActiveSyncDeviceClassContainer(IConfigurationSession scopedSession, OrganizationId orgId)
		{
			ActiveSyncDeviceClasses[] deviceClasses = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				deviceClasses = scopedSession.Find<ActiveSyncDeviceClasses>(orgId.ConfigurationUnit, QueryScope.SubTree, DeviceClassCache.deviceClassesFilter, DeviceClassCache.deviceClassesSortOrder, 0);
			});
			DeviceClassCache.UpdateProtocolLogLastUsedDC(scopedSession);
			if (deviceClasses == null)
			{
				AirSyncDiagnostics.TraceDebug<OrganizationId>(ExTraceGlobals.RequestsTracer, null, "Oragnization \"{0}\" has no DeviceClass container in AD.", orgId);
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_NoDeviceClassContainer, new string[]
				{
					orgId.ToString()
				});
				return null;
			}
			AirSyncDiagnostics.TraceDebug<OrganizationId, int>(ExTraceGlobals.RequestsTracer, null, "Oragnization '{0}' has '{1}' DeviceClass container in AD.", orgId, deviceClasses.Length);
			if (deviceClasses.Length == 1)
			{
				return deviceClasses[0];
			}
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "TooManyDeviceClassContainers");
			}
			return DeviceClassCache.CleanMangledObjects(scopedSession, deviceClasses, "ExchangeDeviceClasses") as ActiveSyncDeviceClasses;
		}

		private static void CreateOrUpdateActiveSyncDeviceClass(IConfigurationSession scopedSession, DeviceClassCache.DeviceClassData deviceClassData, OrganizationId orgId)
		{
			try
			{
				ActiveSyncDeviceClass deviceClass = deviceClassData.ToActiveSyncDeviceClass(scopedSession, orgId);
				deviceClass.LastUpdateTime = new DateTime?(DateTime.UtcNow);
				ADNotificationAdapter.RunADOperation(delegate()
				{
					scopedSession.Save(deviceClass);
				});
				DeviceClassCache.UpdateProtocolLogLastUsedDC(scopedSession);
				AirSyncDiagnostics.TraceDebug<OrganizationId, string, string>(ExTraceGlobals.RequestsTracer, null, "Created DeviceClassData in AD: orgId={0}, deviceType={1}, deviceModel={2}", orgId, deviceClassData.DeviceType, deviceClassData.DeviceModel);
			}
			catch (LocalizedException ex)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Failed to create DeviceClassData: orgId={0}, deviceType={1}, deviceModel={2}, exception=\n\r{3}", new object[]
				{
					orgId,
					deviceClassData.DeviceType,
					deviceClassData.DeviceModel,
					ex
				});
			}
		}

		private static void DeleteActiveSyncDeviceClass(IConfigurationSession scopedSession, DeviceClassCache.DeviceClassData deviceClassData, OrganizationId orgId)
		{
			ActiveSyncDeviceClass adObject = deviceClassData.ToActiveSyncDeviceClass(scopedSession, orgId);
			DeviceClassCache.DeleteObject(scopedSession, adObject);
		}

		private static void UpdateProtocolLogLastUsedDC(IConfigurationSession session)
		{
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, session.LastUsedDc);
			}
		}

		private static ADConfigurationObject CleanMangledObjects(IConfigurationSession scopedSession, ADConfigurationObject[] objects, string rdnShouldBe)
		{
			int num = -1;
			for (int i = 0; i < objects.Length; i++)
			{
				if (num == -1 && !DeviceClassCache.DnIsMangled(objects[i], rdnShouldBe))
				{
					num = i;
				}
				else
				{
					DeviceClassCache.DeleteObject(scopedSession, objects[i]);
				}
			}
			if (num != -1)
			{
				return objects[num];
			}
			return null;
		}

		private static void DeleteObject(IConfigurationSession scopedSession, ADConfigurationObject adObject)
		{
			try
			{
				ADNotificationAdapter.RunADOperation(delegate()
				{
					scopedSession.Delete(adObject);
				});
				DeviceClassCache.UpdateProtocolLogLastUsedDC(scopedSession);
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Deleted object: {0}", adObject.Id.DistinguishedName);
			}
			catch (LocalizedException ex)
			{
				AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.RequestsTracer, null, "Failed to delete object {0} because: {1}", adObject.Id.DistinguishedName, ex.Message);
			}
		}

		private static bool DnIsMangled(ADConfigurationObject adObject, string rdnShouldBe)
		{
			string escapedName = adObject.Id.Rdn.EscapedName;
			if (!escapedName.Equals(rdnShouldBe, StringComparison.Ordinal))
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Found mangled device object DN: {0}", adObject.Id.DistinguishedName);
				return true;
			}
			return false;
		}

		private void Refresh(object state)
		{
			try
			{
				AirSyncDiagnostics.TraceDebug<ExDateTime>(ExTraceGlobals.RequestsTracer, this, "Refresh is being call at '{0}-UTC'.", ExDateTime.UtcNow);
				AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "DeviceClassCache contains '{0}' elements.", this.cache.Values.Count);
				List<DeviceClassCache.DeviceClassDataSet> list;
				lock (this.thisLock)
				{
					if (this.cache.Values.Count < 1)
					{
						return;
					}
					list = new List<DeviceClassCache.DeviceClassDataSet>(this.cache.Values);
				}
				int num = 0;
				foreach (DeviceClassCache.DeviceClassDataSet deviceClassDataSet in list)
				{
					if (this.realTimeRefresh || !(ExDateTime.UtcNow - deviceClassDataSet.StartTime < TimeSpan.FromSeconds((double)GlobalSettings.DeviceClassCachePerOrgRefreshInterval)))
					{
						if (num >= GlobalSettings.DeviceClassCacheMaxADUploadCount)
						{
							AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "1. Stop updating AD because the cap is reached: adUpdateCount={0}", num);
							break;
						}
						lock (this.thisLock)
						{
							if (!this.cache.ContainsKey(deviceClassDataSet.OrganizationId))
							{
								AirSyncDiagnostics.TraceDebug<OrganizationId>(ExTraceGlobals.RequestsTracer, this, "Organization {0} is already removed from the cache by another thread", deviceClassDataSet.OrganizationId);
								continue;
							}
							this.cache.Remove(deviceClassDataSet.OrganizationId);
						}
						using (deviceClassDataSet)
						{
							IConfigurationSession scopedSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(deviceClassDataSet.OrganizationId), 930, "Refresh", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\DeviceClassCache.cs");
							ActiveSyncDeviceClasses deviceClassContainer = DeviceClassCache.GetActiveSyncDeviceClassContainer(scopedSession, deviceClassDataSet.OrganizationId);
							if (deviceClassContainer != null)
							{
								ADPagedReader<ActiveSyncDeviceClass> deviceClassReader = null;
								ADNotificationAdapter.RunADOperation(delegate()
								{
									deviceClassReader = scopedSession.FindPaged<ActiveSyncDeviceClass>(deviceClassContainer.Id, QueryScope.OneLevel, null, null, 0);
								});
								DeviceClassCache.UpdateProtocolLogLastUsedDC(scopedSession);
								if (deviceClassReader != null)
								{
									using (DeviceClassCache.DeviceClassDataSet deviceClassDataSet3 = new DeviceClassCache.DeviceClassDataSet(deviceClassDataSet.OrganizationId))
									{
										foreach (ActiveSyncDeviceClass activeSyncDeviceClass in deviceClassReader)
										{
											if (!string.IsNullOrEmpty(activeSyncDeviceClass.DeviceType) && !string.IsNullOrEmpty(activeSyncDeviceClass.DeviceModel) && activeSyncDeviceClass.LastUpdateTime != null)
											{
												string commonName = ActiveSyncDeviceClass.GetCommonName(activeSyncDeviceClass.DeviceType, activeSyncDeviceClass.DeviceModel);
												if (DeviceClassCache.DnIsMangled(activeSyncDeviceClass, commonName))
												{
													AirSyncDiagnostics.TraceDebug<ADObjectId>(ExTraceGlobals.RequestsTracer, this, "Delete the Mangled DeviceClassCache {0}.", activeSyncDeviceClass.Id);
													DeviceClassCache.DeleteObject(scopedSession, activeSyncDeviceClass);
													num++;
												}
												else
												{
													deviceClassDataSet3.Add(new DeviceClassCache.DeviceClassData(activeSyncDeviceClass));
												}
											}
											else
											{
												AirSyncDiagnostics.TraceDebug<string, string, DateTime?>(ExTraceGlobals.RequestsTracer, this, "Delete the DeviceClassCache. Either DeviceType, DeviceModel or LastupdatTime is null. DeviceType:{0}, DeviceModel:{1}, LastUpdateTime:{2}.", activeSyncDeviceClass.DeviceType, activeSyncDeviceClass.DeviceModel, activeSyncDeviceClass.LastUpdateTime);
												DeviceClassCache.DeleteObject(scopedSession, activeSyncDeviceClass);
												num++;
											}
										}
										DeviceClassCache.UpdateProtocolLogLastUsedDC(scopedSession);
										AirSyncDiagnostics.TraceDebug<int, OrganizationId>(ExTraceGlobals.RequestsTracer, this, "'{0}' device classes are loaded from AD for org '{1}'", deviceClassDataSet3.Count, deviceClassDataSet.OrganizationId);
										int perOrgDeleteCount = 0;
										DeviceClassCache.ProcessForADCleanup(scopedSession, deviceClassDataSet, deviceClassDataSet3, ref num, ref perOrgDeleteCount);
										DeviceClassCache.ProcessForADAdds(scopedSession, deviceClassDataSet, deviceClassDataSet3, ref num, perOrgDeleteCount);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				AirSyncUtility.ProcessException(ex);
			}
		}

		private static readonly int ADCapWarningThreshold = GlobalSettings.DeviceClassPerOrgMaxADCount * 9 / 10;

		private static readonly int TimerKickinInterval = (GlobalSettings.DeviceClassCachePerOrgRefreshInterval / 10 > 0) ? (GlobalSettings.DeviceClassCachePerOrgRefreshInterval / 10) : 1;

		private static readonly SortBy deviceClassesSortOrder = new SortBy(ADObjectSchema.WhenCreated, SortOrder.Ascending);

		private static readonly ComparisonFilter deviceClassesFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "ExchangeDeviceClasses");

		private static InternDataPool<string> internStringPool = new InternDataPool<string>(100);

		private static DeviceClassCache instance = new DeviceClassCache();

		private object thisLock = new object();

		private Dictionary<OrganizationId, DeviceClassCache.DeviceClassDataSet> cache;

		private ExDateTime timerStartTime;

		private GuardedTimer refreshTimer;

		private bool realTimeRefresh;

		private struct ADPropertyConstraintLength
		{
			public static int MaxDeviceModelLength
			{
				get
				{
					if (DeviceClassCache.ADPropertyConstraintLength.maxDeviceModelLength == -1)
					{
						DeviceClassCache.ADPropertyConstraintLength.maxDeviceModelLength = MobileDevice.GetStringConstraintLength(ActiveSyncDeviceClass.StaticSchema, ActiveSyncDeviceClassSchema.DeviceModel);
					}
					return DeviceClassCache.ADPropertyConstraintLength.maxDeviceModelLength;
				}
			}

			public static int MaxDeviceTypeLength
			{
				get
				{
					if (DeviceClassCache.ADPropertyConstraintLength.maxDeviceTypeLength == -1)
					{
						DeviceClassCache.ADPropertyConstraintLength.maxDeviceTypeLength = MobileDevice.GetStringConstraintLength(ActiveSyncDeviceClass.StaticSchema, ActiveSyncDeviceClassSchema.DeviceType);
					}
					return DeviceClassCache.ADPropertyConstraintLength.maxDeviceTypeLength;
				}
			}

			public static int MaxDeviceClassNameLength
			{
				get
				{
					if (DeviceClassCache.ADPropertyConstraintLength.maxDeviceClassNameLength == -1)
					{
						DeviceClassCache.ADPropertyConstraintLength.maxDeviceClassNameLength = MobileDevice.GetStringConstraintLength(ActiveSyncDeviceClass.StaticSchema, ADObjectSchema.Name);
					}
					return DeviceClassCache.ADPropertyConstraintLength.maxDeviceClassNameLength;
				}
			}

			private static int maxDeviceModelLength = -1;

			private static int maxDeviceTypeLength = -1;

			private static int maxDeviceClassNameLength = -1;
		}

		private struct DeviceClassData : IComparable<DeviceClassCache.DeviceClassData>
		{
			public DeviceClassData(string deviceType, string deviceModel)
			{
				this = new DeviceClassCache.DeviceClassData(deviceType, deviceModel, ExDateTime.UtcNow);
			}

			public DeviceClassData(ActiveSyncDeviceClass deviceClassFromAD)
			{
				this = new DeviceClassCache.DeviceClassData(deviceClassFromAD.DeviceType, deviceClassFromAD.DeviceModel, new ExDateTime(ExTimeZone.UtcTimeZone, deviceClassFromAD.LastUpdateTime.Value));
				this.deviceClassFromAD = deviceClassFromAD;
			}

			public DeviceClassData(string deviceType, string deviceModel, ExDateTime lastUpdateTime)
			{
				this.deviceType = DeviceClassCache.internStringPool.Intern(deviceType);
				this.deviceModel = DeviceClassCache.internStringPool.Intern(deviceModel);
				this.lastUpdateTime = lastUpdateTime;
				this.deviceClassFromAD = null;
			}

			public string DeviceType
			{
				get
				{
					return this.deviceType;
				}
			}

			public string DeviceModel
			{
				get
				{
					return this.deviceModel;
				}
			}

			public ExDateTime LastUpdateTime
			{
				get
				{
					return this.lastUpdateTime;
				}
				set
				{
					this.lastUpdateTime = value;
				}
			}

			public ActiveSyncDeviceClass DeviceClassFromAD
			{
				get
				{
					return this.deviceClassFromAD;
				}
			}

			public ActiveSyncDeviceClass ToActiveSyncDeviceClass(IConfigurationSession scopedSession, OrganizationId orgId)
			{
				if (this.DeviceClassFromAD != null)
				{
					this.DeviceClassFromAD.DeviceType = this.DeviceType;
					this.DeviceClassFromAD.DeviceModel = this.DeviceModel;
					this.DeviceClassFromAD.LastUpdateTime = new DateTime?((DateTime)this.LastUpdateTime);
					return this.DeviceClassFromAD;
				}
				ActiveSyncDeviceClass activeSyncDeviceClass = new ActiveSyncDeviceClass
				{
					DeviceType = this.DeviceType,
					DeviceModel = this.DeviceModel,
					LastUpdateTime = new DateTime?((DateTime)this.LastUpdateTime),
					OrganizationId = orgId
				};
				activeSyncDeviceClass.Name = DeviceClassCache.EnforceLengthLimit(activeSyncDeviceClass.GetCommonName(), DeviceClassCache.ADPropertyConstraintLength.MaxDeviceClassNameLength, true);
				activeSyncDeviceClass.SetId(scopedSession, activeSyncDeviceClass.Name);
				return activeSyncDeviceClass;
			}

			public override int GetHashCode()
			{
				int hashCode = this.deviceType.GetHashCode();
				int hashCode2 = this.deviceModel.GetHashCode();
				if (hashCode == hashCode2)
				{
					return hashCode;
				}
				return hashCode ^ hashCode2;
			}

			public override bool Equals(object obj)
			{
				DeviceClassCache.DeviceClassData deviceClassData = (DeviceClassCache.DeviceClassData)obj;
				return this.DeviceType == deviceClassData.DeviceType && this.DeviceModel == deviceClassData.DeviceModel;
			}

			public int CompareTo(DeviceClassCache.DeviceClassData other)
			{
				int num = this.DeviceType.CompareTo(other.DeviceType);
				if (num == 0)
				{
					num = this.DeviceModel.CompareTo(other.DeviceModel);
				}
				return num;
			}

			private string deviceType;

			private string deviceModel;

			private ExDateTime lastUpdateTime;

			private ActiveSyncDeviceClass deviceClassFromAD;
		}

		private class DeviceClassDataSet : DisposeTrackableBase, IEnumerable<DeviceClassCache.DeviceClassData>, IEnumerable
		{
			public DeviceClassDataSet(OrganizationId orgId)
			{
				this.OrganizationId = orgId;
				this.StartTime = ExDateTime.UtcNow;
				this.cache = new HashSet<DeviceClassCache.DeviceClassData>();
			}

			public ExDateTime StartTime { get; private set; }

			public OrganizationId OrganizationId { get; private set; }

			public int Count
			{
				get
				{
					return this.cache.Count;
				}
			}

			public bool Add(DeviceClassCache.DeviceClassData data)
			{
				return this.cache.Add(data);
			}

			public bool Contains(DeviceClassCache.DeviceClassData data)
			{
				return this.cache.Contains(data);
			}

			public IEnumerator<DeviceClassCache.DeviceClassData> GetEnumerator()
			{
				return this.cache.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			protected override void InternalDispose(bool isDisposing)
			{
				AirSyncDiagnostics.TraceDebug<OrganizationId>(ExTraceGlobals.RequestsTracer, this, "Disposing DeviceClassDataSet : orgId={0}", this.OrganizationId);
				if (isDisposing)
				{
					foreach (DeviceClassCache.DeviceClassData deviceClassData in this.cache)
					{
						DeviceClassCache.internStringPool.Release(deviceClassData.DeviceType);
						DeviceClassCache.internStringPool.Release(deviceClassData.DeviceModel);
					}
					this.cache.Clear();
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<DeviceClassCache.DeviceClassDataSet>(this);
			}

			private HashSet<DeviceClassCache.DeviceClassData> cache;
		}
	}
}
