using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.DxStore.Server
{
	public class DxStoreInstanceChecker
	{
		public DxStoreInstanceChecker(DxStoreManager manager, InstanceGroupConfig config)
		{
			this.manager = manager;
			this.config = config;
		}

		public InstanceClientFactory InstanceClientFactory { get; set; }

		internal bool IsRestartRequested { get; set; }

		public void Start()
		{
			lock (this.locker)
			{
				if (this.timer == null)
				{
					this.InstanceClientFactory = new InstanceClientFactory(this.config, null);
					this.StartInstance();
					this.timer = new GuardedTimer(delegate(object o)
					{
						this.CheckHealth();
					}, null, TimeSpan.Zero, this.config.Settings.InstanceHealthCheckPeriodicInterval);
				}
			}
		}

		public bool ReconfigureBestEffort(string callerLabel, InstanceGroupMemberConfig[] members, InstanceStatusInfo si, out bool isReconfiguredAttempted, out bool isPaxosMembershipInSync)
		{
			bool isSuccess = false;
			isReconfiguredAttempted = false;
			isPaxosMembershipInSync = false;
			lock (this.locker)
			{
				if (si != null && si.IsLeader)
				{
					if (this.config.Settings.IsAppendOnlyMembership)
					{
						Dictionary<string, InstanceGroupMemberConfig> dictionary = members.ToDictionary((InstanceGroupMemberConfig m) => m.Name);
						foreach (string text in si.PaxosInfo.Members)
						{
							InstanceGroupMemberConfig value = null;
							if (!dictionary.TryGetValue(text, out value))
							{
								value = new InstanceGroupMemberConfig
								{
									Name = text
								};
								dictionary.Add(text, value);
							}
						}
						members = dictionary.Values.ToArray<InstanceGroupMemberConfig>();
					}
					isPaxosMembershipInSync = si.AreMembersSame(members);
					if (!isPaxosMembershipInSync)
					{
						isReconfiguredAttempted = true;
						string arg = string.Join(",", si.PaxosInfo.Members.ToArray<string>());
						string arg2 = string.Join(",", (from m in members
						select m.Name).ToArray<string>());
						string context = string.Format("Current Paxos Members: '{0}' - New Membership attempted: '{1}'", arg, arg2);
						Utils.RunOperation(this.config.Identity, callerLabel, delegate
						{
							this.InstanceClientFactory.LocalClient.Reconfigure(members, null);
							isSuccess = true;
						}, this.manager.EventLogger, LogOptions.LogAll | this.config.Settings.AdditionalLogOptions, true, new TimeSpan?(this.config.Settings.MemberReconfigureTimeout), null, null, null, context);
					}
				}
			}
			return isSuccess;
		}

		public void Stop(bool isBestEffort = false)
		{
			this.isStopRequested = true;
			lock (this.locker)
			{
				if (this.timer != null)
				{
					this.timer.Dispose(true);
					this.timer = null;
				}
				this.StopInstanceProcess(isBestEffort);
			}
		}

		private void CheckHealth()
		{
			lock (this.locker)
			{
				if (this.isStopRequested)
				{
					DxStoreManager.Tracer.TraceDebug<string>((long)this.config.Identity.GetHashCode(), "{0}: Skipping call to check health since stop requested", this.config.Identity);
				}
				else if (this.IsRestartRequested)
				{
					DxStoreManager.Tracer.TraceDebug<string>((long)this.config.Identity.GetHashCode(), "{0}: Skipping call to check health since restart requested", this.config.Identity);
				}
				else
				{
					this.CheckHealthInternal();
				}
			}
		}

		private InstanceStatusInfo GetInstanceStatus()
		{
			InstanceStatusInfo statusInfo = null;
			Exception ex = Utils.RunOperation(this.config.Identity, "GetInstanceStatus", delegate
			{
				statusInfo = this.InstanceClientFactory.LocalClient.GetStatus(null);
			}, this.manager.EventLogger, LogOptions.LogException | LogOptions.LogPeriodic | this.config.Settings.AdditionalLogOptions, true, null, null, null, null, null);
			if (ex == null)
			{
				if (DxStoreManager.Tracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					string arg = Utils.SerializeObjectToJsonString<InstanceStatusInfo>(statusInfo, false, true) ?? "<serialization error>";
					DxStoreManager.Tracer.TraceDebug<string, string>((long)this.config.Identity.GetHashCode(), "{0}: InstanceChecker.GetInstanceStatus() returned {1}", this.config.Identity, arg);
				}
			}
			else if (DxStoreManager.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				DxStoreManager.Tracer.TraceError<string, Exception>((long)this.config.Identity.GetHashCode(), "{0}: InstanceChecker.GetInstanceStatus() failed with {1}", this.config.Identity, ex);
			}
			return statusInfo;
		}

		private void CheckHealthInternal()
		{
			bool flag = true;
			bool flag2 = false;
			InstanceStatusInfo instanceStatus = this.GetInstanceStatus();
			if (instanceStatus != null)
			{
				if (instanceStatus.State == InstanceState.Running || instanceStatus.State == InstanceState.Starting)
				{
					this.firstObservedUnhealthyTime = DateTimeOffset.MinValue;
					flag = false;
				}
			}
			else if (this.processInfo == null)
			{
				flag2 = true;
			}
			if (flag)
			{
				if (this.firstObservedUnhealthyTime == DateTimeOffset.MinValue)
				{
					this.firstObservedUnhealthyTime = DateTimeOffset.Now;
				}
				if (DateTimeOffset.Now - this.firstObservedUnhealthyTime >= this.config.Settings.DurationToWaitBeforeRestart)
				{
					flag2 = true;
				}
				this.manager.EventLogger.Log(DxEventSeverity.Warning, 0, "{0}: Instance is unhealthy. (IsRestartRequired: {1}, FirstUnhealthyTime: {2})", new object[]
				{
					this.config.Identity,
					flag2,
					this.firstObservedUnhealthyTime
				});
			}
			else if (this.config.Settings.IsAllowDynamicReconfig && instanceStatus != null && instanceStatus.IsValidPaxosMembersExist() && instanceStatus.IsValidLeaderExist() && !instanceStatus.AreMembersSame(this.config.Members))
			{
				bool flag4;
				bool flag5;
				bool flag3 = this.ReconfigureBestEffort("Checker.ReconfigureBestEffort", this.config.Members, instanceStatus, out flag4, out flag5);
				if (flag4 && !flag3)
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				this.IsRestartRequested = true;
				Task.Factory.StartNew(delegate()
				{
					this.manager.RestartInstance(this.config.Name, false);
				});
			}
		}

		private void StopInstanceProcess(bool isBestEffort = false)
		{
			LogOptions options = LogOptions.LogAll | this.config.Settings.AdditionalLogOptions;
			Utils.RunOperation(this.config.Identity, "LocalClient.Stop", delegate
			{
				this.InstanceClientFactory.LocalClient.Stop(true, null);
			}, this.manager.EventLogger, options, true, new TimeSpan?(this.manager.Config.ManagerStopTimeout), null, null, null, null);
			Utils.RunOperation(this.config.Identity, "KillInstanceProcess", new Action(this.KillInstanceProcess), this.manager.EventLogger, options, isBestEffort, null, null, null, null, null);
		}

		private void KillInstanceProcess()
		{
			if (this.process != null)
			{
				if (!this.process.HasExited)
				{
					using (new Privilege("SeDebugPrivilege"))
					{
						try
						{
							DxStoreManager.Tracer.TraceWarning<string>((long)this.config.GetHashCode(), "{0}: Killing instance process", this.config.Identity);
							this.process.Kill();
						}
						catch (InvalidOperationException arg)
						{
							DxStoreManager.Tracer.TraceError<string, InvalidOperationException>((long)this.config.GetHashCode(), "{0}: Kill instance process failed since process might have already exited (error: {1})", this.config.Identity, arg);
						}
					}
				}
				this.process.Dispose();
			}
			if (this.jobObject != null)
			{
				this.jobObject.Dispose();
				this.jobObject = null;
			}
			this.processInfo = null;
		}

		private void StartInstance()
		{
			InstanceStatusInfo status = this.GetInstanceStatus();
			Exception ex = Utils.RunWithPrivilege("SeDebugPrivilege", delegate
			{
				this.StartProcess(status);
			});
			if (ex != null)
			{
				this.manager.EventLogger.Log(DxEventSeverity.Error, 0, "{0}: StartProcess failed with privilege exception {1}", new object[]
				{
					this.config.Identity,
					ex
				});
			}
		}

		private bool StartProcess(InstanceStatusInfo statusInfo)
		{
			DxStoreInstanceChecker.<>c__DisplayClass19 CS$<>8__locals1 = new DxStoreInstanceChecker.<>c__DisplayClass19();
			CS$<>8__locals1.statusInfo = statusInfo;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.cmdLineArguments = string.Format("-GroupName:{0} -Self:{1}", this.config.Name, this.config.Self);
			if (this.config.IsZeroboxMode)
			{
				DxStoreInstanceChecker.<>c__DisplayClass19 CS$<>8__locals2 = CS$<>8__locals1;
				CS$<>8__locals2.cmdLineArguments += " -ZeroboxMode";
			}
			Utils.RunOperation(this.config.Identity, string.Format("Starting Process : {0} {1}", this.config.Settings.InstanceProcessName, CS$<>8__locals1.cmdLineArguments), delegate
			{
				CS$<>8__locals1.<>4__this.StartProcessInternal(CS$<>8__locals1.statusInfo, CS$<>8__locals1.cmdLineArguments);
			}, this.manager.EventLogger, LogOptions.LogAll | this.config.Settings.AdditionalLogOptions, false, null, null, null, null, null);
			return this.processInfo != null;
		}

		private void StartProcessInternal(InstanceStatusInfo statusInfo, string cmdLineArguments)
		{
			Process process = null;
			if (statusInfo != null && statusInfo.HostProcessInfo != null && statusInfo.HostProcessInfo.Id > 0)
			{
				process = Utils.GetMatchingProcess(statusInfo.HostProcessInfo.Id, statusInfo.HostProcessInfo.StartTime, true);
			}
			if (process == null)
			{
				DxStoreManager.Tracer.TraceDebug<string, string, string>((long)this.config.Identity.GetHashCode(), "{0}: Starting process {1} {2}", this.config.Identity, this.config.Settings.InstanceProcessName, cmdLineArguments);
				this.process = Process.Start(this.config.Settings.InstanceProcessName, cmdLineArguments);
			}
			else
			{
				DxStoreManager.Tracer.TraceDebug<string, int>((long)this.config.Identity.GetHashCode(), "{0}: Found already running process id# {1}", this.config.Identity, process.Id);
				this.process = process;
			}
			if (this.process != null)
			{
				this.jobObject = new JobObject("Job." + this.config.Identity, (long)this.config.Settings.InstanceMemoryCommitSizeLimitInMb);
				this.processInfo = new ProcessBasicInfo(this.process);
				if (this.config.Settings.IsKillInstanceProcessWhenParentDies)
				{
					this.jobObject.Add(this.process);
				}
			}
		}

		private readonly object locker = new object();

		private readonly DxStoreManager manager;

		private readonly InstanceGroupConfig config;

		private GuardedTimer timer;

		private ProcessBasicInfo processInfo;

		private DateTimeOffset firstObservedUnhealthyTime = DateTimeOffset.MinValue;

		private bool isStopRequested;

		private JobObject jobObject;

		private Process process;
	}
}
