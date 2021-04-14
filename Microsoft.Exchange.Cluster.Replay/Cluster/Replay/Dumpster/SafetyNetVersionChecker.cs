using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay.Dumpster
{
	internal class SafetyNetVersionChecker : TimerComponent, ISafetyNetVersionCheck
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.DumpsterTracer;
			}
		}

		public SafetyNetVersionChecker() : base(TimeSpan.Zero, TimeSpan.FromMilliseconds((double)RegistryParameters.MonitoringADConfigManagerIntervalInMsec), "SafetyNetVersionChecker")
		{
		}

		public static bool IsSafetyNetRedeliveryRpcSupportedOnHubServer(ServerVersion hubServerVersion)
		{
			return ServerVersion.Compare(hubServerVersion, SafetyNetVersionChecker.SafetyNetRedeliveryRequestRpcSupportVersion) >= 0;
		}

		public bool IsVersionCheckSatisfied()
		{
			if (this.m_isVersionCheckSatisfied)
			{
				return true;
			}
			ManualOneShotEvent.Result result = this.m_firstPollCompleted.WaitOne();
			DiagCore.AssertOrWatson(result != ManualOneShotEvent.Result.WaitTimedOut, "waitResult cannot be WaitTimedOut!", new object[0]);
			if (result == ManualOneShotEvent.Result.ShuttingDown)
			{
				SafetyNetVersionChecker.Tracer.TraceError((long)this.GetHashCode(), "SafetyNetVersionChecker.IsVersionCheckSatisfied(): Service is shutting down.");
				throw new SafetyNetVersionCheckException(ReplayStrings.MonitoringADServiceShuttingDownException);
			}
			bool result2;
			lock (this.m_locker)
			{
				if (this.m_isVersionCheckSatisfied)
				{
					result2 = true;
				}
				else
				{
					if (this.m_lastException != null)
					{
						SafetyNetVersionChecker.Tracer.TraceError<Exception>((long)this.GetHashCode(), "SafetyNetVersionChecker.IsVersionCheckSatisfied(): Throwing last exception: {0}", this.m_lastException);
						throw this.m_lastException;
					}
					result2 = false;
				}
			}
			return result2;
		}

		protected override void TimerCallbackInternal()
		{
			Exception lastException = null;
			bool flag = false;
			try
			{
				flag = this.IsVersionSatisfiedImpl();
			}
			catch (MonitoringADConfigException ex)
			{
				lastException = new SafetyNetVersionCheckException(ex.Message, ex);
				SafetyNetVersionChecker.Tracer.TraceError<MonitoringADConfigException>((long)this.GetHashCode(), "SafetyNetVersionChecker: Got exception when retrieving AD config: {0}", ex);
				ReplayCrimsonEvents.SafetyNetVersionCheckerError.LogPeriodic<string, MonitoringADConfigException>(this.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, ex.Message, ex);
			}
			finally
			{
				lock (this.m_locker)
				{
					this.m_isVersionCheckSatisfied = flag;
					this.m_lastException = lastException;
				}
				this.m_firstPollCompleted.Set();
			}
			if (flag)
			{
				base.ChangeTimer(InvokeWithTimeout.InfiniteTimeSpan, InvokeWithTimeout.InfiniteTimeSpan);
			}
		}

		protected override void StopInternal()
		{
			base.StopInternal();
			this.m_firstPollCompleted.Close();
		}

		private bool IsVersionSatisfiedImpl()
		{
			if (RegistryTestHook.SafetyNetVersionCheckerOverride == SafetyNetVersionCheckerOverrideEnum.DumpsterSchema)
			{
				SafetyNetVersionChecker.Tracer.TraceDebug((long)this.GetHashCode(), "SafetyNetVersionChecker.IsVersionSatisfiedImpl(): TestHook specified to force using older Dumpster request schema. Returning 'false'.");
				return false;
			}
			if (RegistryTestHook.SafetyNetVersionCheckerOverride == SafetyNetVersionCheckerOverrideEnum.SafetyNetSchema)
			{
				SafetyNetVersionChecker.Tracer.TraceDebug((long)this.GetHashCode(), "SafetyNetVersionChecker.IsVersionSatisfiedImpl(): TestHook specified to force using newer SafetyNet request schema. Returning 'true'.");
				return true;
			}
			IMonitoringADConfig config = Dependencies.MonitoringADConfigProvider.GetConfig(true);
			foreach (IADServer iadserver in config.Servers)
			{
				if (ServerVersion.Compare(iadserver.AdminDisplayVersion, SafetyNetVersionChecker.SafetyNetRedeliverySchemaSupportVersion) < 0)
				{
					SafetyNetVersionChecker.Tracer.TraceDebug<string, ServerVersion, ServerVersion>((long)this.GetHashCode(), "SafetyNetVersionChecker.IsVersionSatisfiedImpl(): Server '{0}' does *NOT* meet minimum SafetyNetRedeliverySchemaSupportVersion of '{1}'. Actual: {2}", iadserver.Name, SafetyNetVersionChecker.SafetyNetRedeliverySchemaSupportVersion, iadserver.AdminDisplayVersion);
					return false;
				}
			}
			SafetyNetVersionChecker.Tracer.TraceDebug((long)this.GetHashCode(), "SafetyNetVersionChecker.IsVersionSatisfiedImpl(): All servers in local DAG meet minimum SafetyNetRedeliverySchemaSupportVersion.");
			return true;
		}

		private const string FirstSafetyNetVersionPollCompletedEventName = "FirstSafetyNetVersionPollCompletedEvent";

		private static readonly ServerVersion SafetyNetRedeliverySchemaSupportVersion = new ServerVersion(15, 0, 211, 0);

		private static readonly ServerVersion SafetyNetRedeliveryRequestRpcSupportVersion = new ServerVersion(15, 0, 330, 0);

		private ManualOneShotEvent m_firstPollCompleted = new ManualOneShotEvent("FirstSafetyNetVersionPollCompletedEvent");

		private Exception m_lastException;

		private bool m_isVersionCheckSatisfied;

		private object m_locker = new object();
	}
}
