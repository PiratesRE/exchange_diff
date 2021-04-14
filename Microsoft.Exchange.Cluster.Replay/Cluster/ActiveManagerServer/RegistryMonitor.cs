using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.Cluster;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class RegistryMonitor
	{
		internal RegistryMonitor(IADConfig adConfig)
		{
			this.adConfig = adConfig;
			this.workQueue = new Queue<RegistryReplicator>();
			this.replicators = new Dictionary<Guid, RegistryReplicator>();
			this.stopEvent = new ManualResetEvent(false);
			this.registryMonitorThread = new Thread(new ThreadStart(this.MonitorRegistry));
			this.registryMonitorThread.Name = "Registry Monitor";
			this.registryCopierThread = new Thread(new ParameterizedThreadStart(this.BackgroundCopy));
			this.registryCopierThread.Name = "Registry Copier";
			this.dbMountedEvent = new AutoResetEvent(false);
		}

		internal void PreMountCopy(Guid mdbGuid, bool isPublicMdb)
		{
			RegistryReplicator registryReplicator = null;
			RealRegistry realRegistry = null;
			ClusterRegistry clusterRegistry = null;
			AmConfig config = AmSystemManager.Instance.Config;
			if (config.IsPamOrSam)
			{
				string text = isPublicMdb ? RegistryMonitor.publicString : RegistryMonitor.privateString;
				string text2 = string.Format(RegistryMonitor.localRegistryPathFormat, Environment.MachineName, mdbGuid, text);
				string root = string.Format(RegistryMonitor.clusterRegistryPathFormat, mdbGuid, text);
				try
				{
					if (this.replicators.ContainsKey(mdbGuid))
					{
						this.replicators[mdbGuid].DisableCopy();
					}
					SafeRegistryHandle handle;
					DiagnosticsNativeMethods.ErrorCode errorCode = DiagnosticsNativeMethods.RegOpenKeyEx(SafeRegistryHandle.LocalMachine, text2, 0, 131097, out handle);
					if (DiagnosticsNativeMethods.ErrorCode.FileNotFound == errorCode)
					{
						RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(text2);
						if (registryKey != null)
						{
							registryKey.Close();
							errorCode = DiagnosticsNativeMethods.RegOpenKeyEx(SafeRegistryHandle.LocalMachine, text2, 0, 131097, out handle);
						}
					}
					if (errorCode != DiagnosticsNativeMethods.ErrorCode.Success)
					{
						throw new AmRegistryException("RegOpenKeyEx", new Win32Exception((int)errorCode));
					}
					realRegistry = new RealRegistry(text2, handle);
					clusterRegistry = new ClusterRegistry(root, config.DagConfig.Cluster.Handle);
					registryReplicator = new RegistryReplicator(realRegistry, clusterRegistry, null);
					realRegistry = null;
					clusterRegistry = null;
					if (!registryReplicator.IsInitialReplication)
					{
						registryReplicator.InverseCopy();
					}
				}
				catch (ClusterException innerException)
				{
					throw new AmDbMountNotAllowedDueToRegistryConfigurationException(innerException);
				}
				catch (AmRegistryException innerException2)
				{
					throw new AmDbMountNotAllowedDueToRegistryConfigurationException(innerException2);
				}
				finally
				{
					if (registryReplicator != null)
					{
						registryReplicator.Dispose();
					}
					if (realRegistry != null)
					{
						realRegistry.Dispose();
					}
					if (clusterRegistry != null)
					{
						clusterRegistry.Dispose();
					}
					if (this.replicators.ContainsKey(mdbGuid))
					{
						this.replicators[mdbGuid].EnableCopy();
					}
				}
			}
		}

		internal void PostMountCopy()
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (config.IsPamOrSam)
			{
				this.dbMountedEvent.Set();
			}
		}

		internal void Start()
		{
			this.registryMonitorThread.Start();
			this.registryCopierThread.Start();
		}

		internal void Stop()
		{
			this.stopEvent.Set();
			this.registryMonitorThread.Join();
			this.registryCopierThread.Join();
			foreach (KeyValuePair<Guid, RegistryReplicator> keyValuePair in this.replicators)
			{
				keyValuePair.Value.Dispose();
			}
			this.replicators.Clear();
		}

		private IADDatabase[] GetDatabasesToMonitor()
		{
			List<IADDatabase> list = new List<IADDatabase>();
			try
			{
				IADServer localServer = this.adConfig.GetLocalServer();
				if (localServer != null)
				{
					IEnumerable<IADDatabase> databasesOnServer = this.adConfig.GetDatabasesOnServer(localServer);
					if (databasesOnServer == null)
					{
						goto IL_CF;
					}
					using (IEnumerator<IADDatabase> enumerator = databasesOnServer.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IADDatabase iaddatabase = enumerator.Current;
							try
							{
								string name;
								ActiveManagerCore.GetDatabaseMountStatus(iaddatabase.Guid, out name);
								if (string.IsNullOrEmpty(name))
								{
									name = iaddatabase.Server.Name;
								}
								if (Cluster.StringIEquals(name, localServer.Name))
								{
									list.Add(iaddatabase);
								}
							}
							catch (AmGetFqdnFailedNotFoundException ex)
							{
								AmTrace.Error("RegistryMonitor.GetDatabasesToMonitor ignoring db {0} due to {1}", new object[]
								{
									iaddatabase.Guid,
									ex
								});
							}
						}
						goto IL_CF;
					}
				}
				AmTrace.Warning("FindLocalServer() failed.", new object[0]);
				IL_CF:;
			}
			catch (AmCommonTransientException ex2)
			{
				AmTrace.Warning("GetDatabasesToMonitor() failed with {0}", new object[]
				{
					ex2
				});
			}
			catch (ADTransientException ex3)
			{
				AmTrace.Warning("GetDatabasesToMonitor() failed with {0}", new object[]
				{
					ex3
				});
			}
			catch (ADExternalException ex4)
			{
				AmTrace.Warning("GetDatabasesToMonitor() failed with {0}", new object[]
				{
					ex4
				});
			}
			catch (ADOperationException ex5)
			{
				AmTrace.Warning("GetDatabasesToMonitor() failed with {0}", new object[]
				{
					ex5
				});
			}
			catch (ClusterException ex6)
			{
				AmTrace.Warning("GetDatabasesToMonitor() failed with {0}", new object[]
				{
					ex6
				});
			}
			return list.ToArray();
		}

		private void MonitorRegistry()
		{
			int num = 0;
			int num2 = RegistryParameters.RegistryMonitorPollingIntervalInSec / 5;
			AmConfig amConfig = null;
			Dictionary<Guid, RegistryReplicator>.KeyCollection.Enumerator enumerator = this.replicators.Keys.GetEnumerator();
			try
			{
				bool flag = false;
				RegistryReplicator registryReplicator = null;
				RealRegistry realRegistry = null;
				ClusterRegistry clusterRegistry = null;
				List<Guid> list = new List<Guid>();
				List<WaitHandle> list2 = new List<WaitHandle>();
				while (!this.stopEvent.WaitOne(5000, false))
				{
					amConfig = AmSystemManager.Instance.Config;
					while (!this.stopEvent.WaitOne(0, false) && amConfig.IsPamOrSam)
					{
						AmClusterHandle handle = amConfig.DagConfig.Cluster.Handle;
						if (flag)
						{
							enumerator.Dispose();
							enumerator = this.replicators.Keys.GetEnumerator();
						}
						int num3 = 0;
						list2.Clear();
						list.Clear();
						while (num3 < 64 && enumerator.MoveNext())
						{
							if (this.replicators[enumerator.Current].IsValid)
							{
								list2.Add(this.replicators[enumerator.Current].KeyChanged);
								list.Add(enumerator.Current);
								num3++;
							}
						}
						flag = (num3 < 64);
						if (list2.Count <= 0)
						{
							goto IL_205;
						}
						int num4 = WaitHandle.WaitAny(list2.ToArray(), 5000, false);
						if (num4 >= 0 && num4 < list2.Count)
						{
							Guid key = list[num4];
							if (this.replicators[key].IsCopyEnabled)
							{
								lock (this.workQueue)
								{
									this.workQueue.Enqueue(this.replicators[key]);
									this.replicators[key].KeyChanged.Reset();
								}
								try
								{
									DiagnosticsNativeMethods.ErrorCode errorCode = DiagnosticsNativeMethods.RegNotifyChangeKeyValue((SafeRegistryHandle)this.replicators[key].Handle, true, DiagnosticsNativeMethods.RegistryNotifications.ChangeName | DiagnosticsNativeMethods.RegistryNotifications.ChangeAttributes | DiagnosticsNativeMethods.RegistryNotifications.LastSet, this.replicators[key].KeyChanged.SafeWaitHandle, true);
									if (errorCode != DiagnosticsNativeMethods.ErrorCode.Success)
									{
										throw new AmRegistryException("RegNotifyChangeKeyValue", new Win32Exception((int)errorCode));
									}
									goto IL_21B;
								}
								catch (AmRegistryException ex)
								{
									AmTrace.Warning("Registering for registry key change notifications failed. Inner Exception: {0}", new object[]
									{
										ex
									});
									goto IL_21B;
								}
								goto IL_205;
							}
						}
						IL_21B:
						if (num == 0 || this.dbMountedEvent.WaitOne(0, false))
						{
							num2 = RegistryParameters.RegistryMonitorPollingIntervalInSec / 5;
							foreach (KeyValuePair<Guid, RegistryReplicator> keyValuePair in this.replicators)
							{
								keyValuePair.Value.SetMarkedForRemoval();
							}
							IADDatabase[] databasesToMonitor = this.GetDatabasesToMonitor();
							if (databasesToMonitor != null)
							{
								foreach (IADDatabase iaddatabase in databasesToMonitor)
								{
									if (this.replicators.ContainsKey(iaddatabase.Guid))
									{
										this.replicators[iaddatabase.Guid].ResetMarkedForRemoval();
									}
									else
									{
										Exception ex2 = null;
										try
										{
											if (!flag)
											{
												enumerator.Dispose();
												flag = true;
											}
											string text = iaddatabase.IsPublicFolderDatabase ? RegistryMonitor.publicString : RegistryMonitor.privateString;
											string text2 = string.Format(RegistryMonitor.localRegistryPathFormat, Environment.MachineName, iaddatabase.Guid, text);
											string root = string.Format(RegistryMonitor.clusterRegistryPathFormat, iaddatabase.Guid, text);
											ManualResetEvent manualResetEvent = new ManualResetEvent(false);
											SafeRegistryHandle safeRegistryHandle;
											DiagnosticsNativeMethods.ErrorCode errorCode = DiagnosticsNativeMethods.RegOpenKeyEx(SafeRegistryHandle.LocalMachine, text2, 0, 131097, out safeRegistryHandle);
											if (DiagnosticsNativeMethods.ErrorCode.FileNotFound == errorCode)
											{
												RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(text2);
												if (registryKey != null)
												{
													registryKey.Close();
													errorCode = DiagnosticsNativeMethods.RegOpenKeyEx(SafeRegistryHandle.LocalMachine, text2, 0, 131097, out safeRegistryHandle);
												}
											}
											if (errorCode != DiagnosticsNativeMethods.ErrorCode.Success)
											{
												throw new AmRegistryException("RegOpenKeyEx", new Win32Exception((int)errorCode));
											}
											realRegistry = new RealRegistry(text2, safeRegistryHandle);
											clusterRegistry = new ClusterRegistry(root, amConfig.DagConfig.Cluster.Handle);
											registryReplicator = new RegistryReplicator(realRegistry, clusterRegistry, manualResetEvent);
											realRegistry = null;
											clusterRegistry = null;
											if (registryReplicator.IsInitialReplication)
											{
												registryReplicator.Copy(handle);
												registryReplicator.SetInitialReplication();
											}
											else
											{
												registryReplicator.InverseCopy();
											}
											errorCode = DiagnosticsNativeMethods.RegNotifyChangeKeyValue(safeRegistryHandle, true, DiagnosticsNativeMethods.RegistryNotifications.ChangeName | DiagnosticsNativeMethods.RegistryNotifications.ChangeAttributes | DiagnosticsNativeMethods.RegistryNotifications.LastSet, manualResetEvent.SafeWaitHandle, true);
											if (errorCode != DiagnosticsNativeMethods.ErrorCode.Success)
											{
												throw new AmRegistryException("RegNotifyChangeKeyValue", new Win32Exception((int)errorCode));
											}
											this.replicators.Add(iaddatabase.Guid, registryReplicator);
											registryReplicator = null;
										}
										catch (AmRegistryException ex3)
										{
											ex2 = ex3;
											AmTrace.Warning("Failed to add database to monitor: {0}. Exception: {1}", new object[]
											{
												iaddatabase.Name,
												ex3
											});
										}
										catch (ClusterException ex4)
										{
											ex2 = ex4;
											AmTrace.Warning("Failed to add database to monitor: {0}. Exception: {1}", new object[]
											{
												iaddatabase.Name,
												ex4
											});
										}
										finally
										{
											if (ex2 != null)
											{
												ReplayEventLogConstants.Tuple_RegistryReplicatorException.LogEvent(null, new object[]
												{
													ex2
												});
											}
											if (registryReplicator != null)
											{
												registryReplicator.Dispose();
												registryReplicator = null;
											}
											if (realRegistry != null)
											{
												realRegistry.Dispose();
												realRegistry = null;
											}
											if (clusterRegistry != null)
											{
												clusterRegistry.Dispose();
												clusterRegistry = null;
											}
										}
									}
								}
							}
							list.Clear();
							foreach (KeyValuePair<Guid, RegistryReplicator> keyValuePair2 in this.replicators)
							{
								if (keyValuePair2.Value.IsMarkedForRemoval)
								{
									list.Add(keyValuePair2.Key);
								}
							}
							lock (this.workQueue)
							{
								foreach (Guid key2 in list)
								{
									registryReplicator = this.replicators[key2];
									registryReplicator.SetInvalid();
									if (!registryReplicator.IsCopying)
									{
										if (!flag)
										{
											flag = true;
										}
										this.replicators.Remove(key2);
										registryReplicator.Dispose();
									}
								}
							}
							registryReplicator = null;
						}
						num = (num + 1) % num2;
						amConfig = AmSystemManager.Instance.Config;
						continue;
						IL_205:
						if (!this.stopEvent.WaitOne(5000, false))
						{
							goto IL_21B;
						}
						break;
					}
				}
				foreach (KeyValuePair<Guid, RegistryReplicator> keyValuePair3 in this.replicators)
				{
					if (!keyValuePair3.Value.IsCopying)
					{
						keyValuePair3.Value.SetInvalid();
					}
				}
			}
			finally
			{
				enumerator.Dispose();
			}
		}

		private void BackgroundCopy(object data)
		{
			int millisecondsTimeout = 5000;
			RegistryReplicator registryReplicator = null;
			bool flag = false;
			while (!this.stopEvent.WaitOne(millisecondsTimeout, false))
			{
				millisecondsTimeout = 5000;
				AmConfig config = AmSystemManager.Instance.Config;
				if (config.IsPamOrSam)
				{
					AmClusterHandle handle = null;
					if (flag)
					{
						handle = config.DagConfig.Cluster.Handle;
						flag = false;
					}
					while (this.workQueue.Count > 0 && !this.stopEvent.WaitOne(0, false))
					{
						lock (this.workQueue)
						{
							registryReplicator = this.workQueue.Dequeue();
						}
						Exception ex = null;
						try
						{
							registryReplicator.Copy(handle);
						}
						catch (AmRegistryException ex2)
						{
							ex = ex2;
							lock (this.workQueue)
							{
								this.workQueue.Enqueue(registryReplicator);
							}
						}
						catch (ClusterException ex3)
						{
							ex = ex3;
							AmTrace.Warning("Caught AmClusterApiException during registry replication: {0}", new object[]
							{
								ex3
							});
							lock (this.workQueue)
							{
								this.workQueue.Enqueue(registryReplicator);
								millisecondsTimeout = 30000;
								flag = true;
								break;
							}
						}
						finally
						{
							if (ex != null)
							{
								ReplayEventLogConstants.Tuple_RegistryReplicatorException.LogEvent(null, new object[]
								{
									ex
								});
							}
						}
					}
				}
			}
		}

		private static string clusterRegistryPathFormat = "MsExchangeIs\\{1}{0}";

		private static string localRegistryPathFormat = "SYSTEM\\CurrentControlSet\\Services\\MsExchangeIs\\{0}\\{2}{1}";

		private static string publicString = "Public-";

		private static string privateString = "Private-";

		private Dictionary<Guid, RegistryReplicator> replicators;

		private Queue<RegistryReplicator> workQueue;

		private ManualResetEvent stopEvent;

		private Thread registryMonitorThread;

		private Thread registryCopierThread;

		private IADConfig adConfig;

		private AutoResetEvent dbMountedEvent;
	}
}
