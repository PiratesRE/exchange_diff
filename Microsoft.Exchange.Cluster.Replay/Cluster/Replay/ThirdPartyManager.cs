using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.ThirdPartyReplication;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ThirdPartyManager : IServiceComponent
	{
		public static ThirdPartyManager Instance
		{
			get
			{
				return ThirdPartyManager.s_manager;
			}
		}

		public static bool IsInitialized
		{
			get
			{
				return ThirdPartyManager.Instance.m_initialized;
			}
		}

		public static bool IsThirdPartyReplicationEnabled
		{
			get
			{
				if (!ThirdPartyManager.Instance.m_initialized)
				{
					throw new TPRInitException(ThirdPartyManager.Instance.m_initFailMsg);
				}
				return ThirdPartyManager.Instance.m_tprEnabled;
			}
		}

		public bool IsAmeListening
		{
			get
			{
				return this.m_isAmeListening;
			}
		}

		public string Name
		{
			get
			{
				return "Third Party Replication Manager";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.ThirdPartyReplicationManager;
			}
		}

		public bool IsCritical
		{
			get
			{
				return false;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public bool IsRetriableOnError
		{
			get
			{
				return true;
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		public void Invoke(Action toInvoke)
		{
			toInvoke();
		}

		public static string GetPrimaryActiveManager()
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (config.IsPamOrSam)
			{
				return config.DagConfig.CurrentPAM.Fqdn;
			}
			throw new NoPAMDesignatedException();
		}

		public static void LogInvalidOperation(Exception e)
		{
			ReplayCrimsonEvents.TPREnabledInvalidOperation.Log<string, string>(e.Message, e.StackTrace);
		}

		public static void PreventOperationWhenTPREnabled(string operationName)
		{
			if (ThirdPartyManager.IsThirdPartyReplicationEnabled)
			{
				TPREnabledInvalidOperationException ex = new TPREnabledInvalidOperationException(operationName);
				try
				{
					throw ex;
				}
				finally
				{
					ThirdPartyManager.LogInvalidOperation(ex);
				}
			}
		}

		public bool Start()
		{
			if (!this.m_initialized)
			{
				this.Init();
			}
			return this.m_initialized;
		}

		public void Stop()
		{
			lock (this.m_serviceLock)
			{
				this.m_fShutdown = true;
			}
			this.StopServiceStarter();
			lock (this.m_serviceLock)
			{
				this.StopService();
			}
		}

		public void AmRoleNotify(AmConfig amConfig)
		{
			if (this.m_tprEnabled || (amConfig.DagConfig != null && amConfig.DagConfig.IsThirdPartyReplEnabled))
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.HandleAmRoleChange), amConfig);
			}
		}

		public Notify OpenNotifyChannel()
		{
			if (!ThirdPartyManager.IsThirdPartyReplicationEnabled)
			{
				throw new TPRNotEnabledException();
			}
			if (!this.m_isAmeListening)
			{
				throw new TPRProviderNotListeningException();
			}
			return Notify.Open(this.m_openTimeout, this.m_sendTimeout, this.m_receiveTimeout);
		}

		public void ImmediateDismountMailboxDatabase(Guid databaseId)
		{
			ExTraceGlobals.ThirdPartyManagerTracer.TraceDebug(0L, "ImmediateDismountMailboxDatabase called");
			this.CheckForPam("ImmediateDismountMailboxDatabase");
			Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				this.CheckForPam("ImmediateDismountMailboxDatabase");
				IADDatabase db = this.LookupDatabase(databaseId);
				AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Admin, AmDbActionReason.FailureItem, AmDbActionCategory.Dismount);
				AmConfig config = AmSystemManager.Instance.Config;
				new AmDbPamAction(config, db, actionCode, this.GenerateUniqueDbActionId())
				{
					LockTimeout = new TimeSpan?(this.m_openTimeout)
				}.Dismount(UnmountFlags.SkipCacheFlush);
			});
			if (ex == null)
			{
				return;
			}
			if (ex is ThirdPartyReplicationException)
			{
				throw ex;
			}
			throw new ImmediateDismountMailboxDatabaseException(databaseId, ex.Message);
		}

		public void ChangeActiveServer(Guid databaseId, string newActiveServerName)
		{
			ExTraceGlobals.ThirdPartyManagerTracer.TraceDebug(0L, "ChangeActiveServer called");
			Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				this.CheckForPam("ChangeActiveServer");
				IADDatabase db = this.LookupDatabase(databaseId);
				this.CheckServerForCopy(db, newActiveServerName);
				AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Admin, AmDbActionReason.FailureItem, AmDbActionCategory.Move);
				AmConfig config = AmSystemManager.Instance.Config;
				AmDbPamAction amDbPamAction = new AmDbPamAction(config, db, actionCode, this.GenerateUniqueDbActionId());
				amDbPamAction.ChangeActiveServerForThirdParty(newActiveServerName, this.m_openTimeout);
			});
			if (ex != null)
			{
				throw ex;
			}
		}

		public void AmeIsStarting(TimeSpan retryDelay, TimeSpan openTimeout, TimeSpan sendTimeout, TimeSpan receiveTimeout)
		{
			lock (this)
			{
				this.SetTimeouts(retryDelay, openTimeout, sendTimeout, receiveTimeout);
				this.m_isAmeListening = true;
				AmConfig config = AmSystemManager.Instance.Config;
				if (config.IsPAM)
				{
					if (this.BecomePame())
					{
					}
				}
				else
				{
					this.RevokePame();
				}
			}
		}

		public void AmeIsStopping()
		{
			lock (this)
			{
				this.m_isAmeListening = false;
			}
		}

		public bool CheckHealth(out string errMsg)
		{
			errMsg = string.Empty;
			lock (this.m_serviceLock)
			{
				if (this.m_service == null)
				{
					if (this.m_serviceStartFailure == null)
					{
						errMsg = ReplayStrings.TPRNotYetStarted;
						return false;
					}
					errMsg = ReplayStrings.TPRExchangeNotListening(this.m_serviceStartFailure.Message);
					return false;
				}
			}
			TimeSpan timeout = new TimeSpan(0, 0, 3);
			Exception ex = this.DoTPRCommunication(delegate(object param0, EventArgs param1)
			{
				using (Request request = Request.Open(timeout, timeout, timeout))
				{
					request.GetPrimaryActiveManager();
				}
			});
			if (ex != null)
			{
				errMsg = ReplayStrings.TPRExchangeListenerNotResponding(ex.Message);
				return false;
			}
			ex = this.DoTPRCommunication(delegate(object param0, EventArgs param1)
			{
				using (Notify notify = Notify.Open(timeout, timeout, timeout))
				{
					TimeSpan timeSpan = default(TimeSpan);
					notify.GetTimeouts(out timeSpan, out timeSpan, out timeSpan, out timeSpan);
				}
			});
			if (ex != null)
			{
				errMsg = ReplayStrings.TPRProviderNotResponding(ex.Message);
				return false;
			}
			return true;
		}

		internal static void TestSetTPRInitialized()
		{
			ThirdPartyManager.Instance.m_initialized = true;
		}

		internal NotificationResponse DatabaseMoveNeeded(Guid dbId, string currentActiveFqdn, bool mountDesired)
		{
			NotificationResponse response = NotificationResponse.Incomplete;
			Exception ex = this.DoTPRCommunication(delegate(object param0, EventArgs param1)
			{
				using (Notify notify = this.OpenNotifyChannel())
				{
					response = notify.DatabaseMoveNeeded(dbId, currentActiveFqdn, mountDesired);
					ReplayCrimsonEvents.TPRDatabaseMoveNeededResponse.Log<Guid, string, bool, NotificationResponse>(dbId, currentActiveFqdn, mountDesired, response);
				}
			});
			if (ex != null)
			{
				ReplayCrimsonEvents.TPRDatabaseMoveNeededCommunicationFailed.Log<Guid, string, bool, string>(dbId, currentActiveFqdn, mountDesired, ex.Message);
			}
			return response;
		}

		private void Init()
		{
			bool flag = false;
			try
			{
				object serviceLock;
				Monitor.Enter(serviceLock = this.m_serviceLock, ref flag);
				if (!this.m_fShutdown)
				{
					IADDatabaseAvailabilityGroup dag = null;
					Exception exception = null;
					TimeSpan invokeTimeout = TimeSpan.FromSeconds((double)RegistryParameters.MonitoringADGetConfigTimeoutInSec);
					try
					{
						InvokeWithTimeout.Invoke(delegate()
						{
							dag = DagHelper.GetLocalServerDatabaseAvailabilityGroup(null, out exception);
						}, invokeTimeout);
					}
					catch (TimeoutException exception)
					{
						TimeoutException exception2;
						exception = exception2;
					}
					if (exception != null)
					{
						ExTraceGlobals.ThirdPartyManagerTracer.TraceError<Exception>(0L, "TPR Init fails due to AD problems: {0}", exception);
						this.m_initFailMsg = exception.Message;
						ReplayEventLogConstants.Tuple_TPRManagerInitFailure.LogEvent(this.Name, new object[]
						{
							this.m_initFailMsg
						});
					}
					else
					{
						if (dag == null)
						{
							ExTraceGlobals.ThirdPartyManagerTracer.TraceDebug(0L, "TPR not enabled because we are not in a DAG");
						}
						else
						{
							if (dag.ThirdPartyReplication == ThirdPartyReplicationMode.Enabled)
							{
								this.m_tprEnabled = true;
							}
							ExTraceGlobals.ThirdPartyManagerTracer.TraceDebug<string>(0L, "TPR is {0}enabled", this.m_tprEnabled ? string.Empty : "not ");
						}
						this.m_initialized = true;
					}
				}
			}
			finally
			{
				if (flag)
				{
					object serviceLock;
					Monitor.Exit(serviceLock);
				}
			}
		}

		private void HandleAmRoleChange(object amConfigObj)
		{
			AmConfig amConfig = (AmConfig)amConfigObj;
			lock (this.m_serviceLock)
			{
				if (!this.m_fShutdown)
				{
					if (amConfig.IsPamOrSam)
					{
						this.m_tprEnabled = amConfig.DagConfig.IsThirdPartyReplEnabled;
						if (!this.m_initialized)
						{
							this.m_initialized = true;
						}
						if (!this.m_tprEnabled)
						{
							this.StopService();
						}
						else if (this.m_service == null)
						{
							this.TryToStartService();
						}
						else if (amConfig.IsPAM)
						{
							this.BecomePame();
						}
						else
						{
							this.RevokePame();
						}
					}
					else if (amConfig.IsStandalone)
					{
						this.m_tprEnabled = false;
						if (!this.m_initialized)
						{
							this.m_initialized = true;
						}
						this.StopService();
					}
					else if (this.IsAmeListening)
					{
						this.RevokePame();
					}
				}
			}
		}

		private void StopService()
		{
			this.m_isAmeListening = false;
			if (this.m_service != null)
			{
				this.m_service.StopListening();
				this.m_service = null;
			}
		}

		private void TryToStartService()
		{
			bool flag = false;
			lock (this.m_serviceLock)
			{
				if (this.m_fShutdown)
				{
					return;
				}
				if (this.m_tprEnabled)
				{
					if (this.m_service == null)
					{
						this.StartService();
						if (this.m_service == null)
						{
							if (this.m_serviceStarter == null)
							{
								this.ScheduleServiceStartupRetry();
							}
						}
						else
						{
							flag = true;
						}
					}
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.StopServiceStarter();
			}
		}

		private void StartService()
		{
			Exception ex = null;
			this.m_service = ThirdPartyService.StartListening(out ex);
			if (this.m_service == null)
			{
				this.m_serviceStartFailure = ex;
				return;
			}
			ex = this.DoTPRCommunication(delegate(object param0, EventArgs param1)
			{
				TimeSpan timeSpan = new TimeSpan(0, 0, 2);
				using (Notify notify = Notify.Open(timeSpan, timeSpan, timeSpan))
				{
					notify.GetTimeouts(out this.m_retryDelay, out this.m_openTimeout, out this.m_sendTimeout, out this.m_receiveTimeout);
					this.m_isAmeListening = true;
					AmConfig config = AmSystemManager.Instance.Config;
					if (config.IsPAM)
					{
						this.BecomePame();
					}
					else
					{
						this.RevokePame();
					}
				}
			});
			if (ex != null)
			{
				ExTraceGlobals.ThirdPartyManagerTracer.TraceError<Exception>(0L, "StartService fails to contact the AME: {0}", ex);
			}
		}

		private Exception DoADAction(EventHandler ev)
		{
			Exception ex = null;
			try
			{
				ev(null, null);
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (ADExternalException ex3)
			{
				ex = ex3;
			}
			catch (ADOperationException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				StackFrame stackFrame = new StackFrame(1);
				ExTraceGlobals.ThirdPartyServiceTracer.TraceError<string, string, Exception>(0L, "DoAction({0}) fails: {1}, {2}", stackFrame.GetMethod().Name, ex.Message, ex);
			}
			return ex;
		}

		private Exception DoTPRCommunication(EventHandler ev)
		{
			Exception ex = null;
			try
			{
				ev(null, null);
			}
			catch (ThirdPartyReplicationException ex2)
			{
				ex = ex2;
			}
			catch (TPRProviderNotListeningException ex3)
			{
				ex = ex3;
			}
			catch (TPRNotEnabledException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				StackFrame stackFrame = new StackFrame(1);
				ExTraceGlobals.ThirdPartyServiceTracer.TraceError<string, string, Exception>(0L, "DoAction({0}) fails: {1}, {2}", stackFrame.GetMethod().Name, ex.Message, ex);
			}
			return ex;
		}

		private bool BecomePame()
		{
			Exception ex = this.DoTPRCommunication(delegate(object param0, EventArgs param1)
			{
				using (Notify notify = this.OpenNotifyChannel())
				{
					notify.BecomePame();
				}
			});
			if (ex != null)
			{
				ExTraceGlobals.ThirdPartyManagerTracer.TraceError<string, Exception>(0L, "BecomePame notification failed: {0}, {1}", ex.Message, ex);
				return false;
			}
			return true;
		}

		private bool RevokePame()
		{
			Exception ex = this.DoTPRCommunication(delegate(object param0, EventArgs param1)
			{
				using (Notify notify = this.OpenNotifyChannel())
				{
					notify.RevokePame();
				}
			});
			if (ex != null)
			{
				ExTraceGlobals.ThirdPartyManagerTracer.TraceError<string, Exception>(0L, "RevokePame notification failed: {0}, {1}", ex.Message, ex);
				return false;
			}
			return true;
		}

		private string GenerateUniqueDbActionId()
		{
			return string.Format("{0}.TPR", ExDateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff"));
		}

		private void CheckForPam(string methodName)
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (!config.IsPAM)
			{
				throw new NotThePamException(methodName);
			}
		}

		private IADDatabase LookupDatabase(Guid databaseId)
		{
			IADDatabase db = null;
			Exception ex = this.DoADAction(delegate(object param0, EventArgs param1)
			{
				IADToplogyConfigurationSession iadtoplogyConfigurationSession = ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
				db = iadtoplogyConfigurationSession.FindDatabaseByGuid(databaseId);
			});
			if (db != null)
			{
				return db;
			}
			ExTraceGlobals.ThirdPartyManagerTracer.TraceDebug<Guid, Exception>(0L, "LookupDatabase({0}) failed. Ex={1}", databaseId, ex);
			if (ex == null)
			{
				throw new NoSuchDatabaseException(databaseId);
			}
			throw ex;
		}

		private void CheckServerForCopy(IADDatabase db, string serverName)
		{
			AmServerName amServerName = new AmServerName(serverName);
			IADDatabaseCopy dbCopy = null;
			Exception ex = this.DoADAction(delegate(object param0, EventArgs param1)
			{
				foreach (IADDatabaseCopy iaddatabaseCopy in db.DatabaseCopies)
				{
					if (MachineName.Comparer.Equals(iaddatabaseCopy.HostServerName, amServerName.NetbiosName))
					{
						dbCopy = iaddatabaseCopy;
						return;
					}
				}
			});
			if (dbCopy != null)
			{
				return;
			}
			ExTraceGlobals.ThirdPartyManagerTracer.TraceDebug(0L, "CheckServerForCopy:no copy of {0} guid={1} on {2}. Ex={3}", new object[]
			{
				db.Name,
				db.Guid,
				serverName,
				ex
			});
			if (ex == null)
			{
				throw new NoCopyOnServerException(db.Guid, db.Name, serverName);
			}
			throw ex;
		}

		private void SetTimeouts(TimeSpan retryDelay, TimeSpan openTimeout, TimeSpan sendTimeout, TimeSpan receiveTimeout)
		{
			this.m_retryDelay = retryDelay;
			this.m_openTimeout = openTimeout;
			this.m_sendTimeout = sendTimeout;
			this.m_receiveTimeout = receiveTimeout;
		}

		private void ScheduleServiceStartupRetry()
		{
			this.m_serviceStarter = new ThirdPartyManager.PeriodicTPRStarter(TimeSpan.FromMilliseconds((double)RegistryParameters.ConfigUpdaterTimerIntervalSlow));
			this.m_serviceStarter.Start();
		}

		private void StopServiceStarter()
		{
			ThirdPartyManager.PeriodicTPRStarter periodicTPRStarter = null;
			lock (this.m_starterLock)
			{
				if (this.m_serviceStarter != null)
				{
					ExTraceGlobals.ThirdPartyManagerTracer.TraceDebug((long)this.GetHashCode(), "PeriodicStarter is being stopped.");
					periodicTPRStarter = this.m_serviceStarter;
					this.m_serviceStarter = null;
				}
			}
			if (periodicTPRStarter != null)
			{
				periodicTPRStarter.Stop();
			}
		}

		private static ThirdPartyManager s_manager = new ThirdPartyManager();

		private ThirdPartyService m_service;

		private object m_serviceLock = new object();

		private Exception m_serviceStartFailure;

		private bool m_tprEnabled;

		private bool m_initialized;

		private string m_initFailMsg;

		private bool m_fShutdown;

		private bool m_isAmeListening;

		private TimeSpan m_retryDelay;

		private TimeSpan m_openTimeout;

		private TimeSpan m_sendTimeout;

		private TimeSpan m_receiveTimeout;

		private ThirdPartyManager.PeriodicTPRStarter m_serviceStarter;

		private object m_starterLock = new object();

		[ClassAccessLevel(AccessLevel.MSInternal)]
		internal class PeriodicTPRStarter : TimerComponent
		{
			public PeriodicTPRStarter(TimeSpan periodicStartInterval) : base(periodicStartInterval, periodicStartInterval, "PeriodicTPRStarter")
			{
			}

			protected override void TimerCallbackInternal()
			{
				ThirdPartyManager.Instance.TryToStartService();
			}
		}
	}
}
