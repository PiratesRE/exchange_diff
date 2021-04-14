using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class MonitoringADConfigManager : TimerComponent, IMonitoringADConfigProvider
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		public MonitoringADConfigManager(IReplayAdObjectLookup adObjectLookup, IReplayAdObjectLookup adObjectLookupPartiallyConsistent, IADToplogyConfigurationSession adSession, IADToplogyConfigurationSession adSessionPartiallyConsistent) : base(TimeSpan.Zero, TimeSpan.FromMilliseconds((double)RegistryParameters.MonitoringADConfigManagerIntervalInMsec), "MonitoringADConfigManager")
		{
			this.m_adObjectLookup = adObjectLookup;
			this.m_adObjectLookupPartiallyConsistent = adObjectLookupPartiallyConsistent;
			this.m_adSession = adSession;
			this.m_adSessionPartiallyConsistent = adSessionPartiallyConsistent;
		}

		public Exception LastException { get; private set; }

		public IMonitoringADConfig GetRecentConfig(bool waitForInit = true)
		{
			return this.GetConfig(waitForInit, MonitoringADConfigManager.CachedConfigShortTTL);
		}

		public IMonitoringADConfig GetConfigIgnoringStaleness(bool waitForInit = true)
		{
			return this.GetConfig(waitForInit, InvokeWithTimeout.InfiniteTimeSpan);
		}

		public IMonitoringADConfig GetConfig(bool waitForInit = true)
		{
			return this.GetConfig(waitForInit, MonitoringADConfigManager.CachedConfigLongTTL);
		}

		private IMonitoringADConfig GetConfig(bool waitForInit, TimeSpan allowedStaleness)
		{
			if (waitForInit)
			{
				TimeSpan getConfigWaitTimeout = MonitoringADConfigManager.GetConfigWaitTimeout;
				ManualOneShotEvent.Result result = this.m_firstLookupCompleted.WaitOne(getConfigWaitTimeout);
				if (result == ManualOneShotEvent.Result.WaitTimedOut)
				{
					MonitoringADConfigManager.Tracer.TraceError<TimeSpan>((long)this.GetHashCode(), "MonitoringADConfigManager.GetConfig(): AD initial lookup timed out after {0}.", getConfigWaitTimeout);
					throw new MonitoringADFirstLookupTimeoutException((int)getConfigWaitTimeout.TotalMilliseconds);
				}
				if (result == ManualOneShotEvent.Result.ShuttingDown)
				{
					MonitoringADConfigManager.Tracer.TraceError((long)this.GetHashCode(), "MonitoringADConfigManager.GetConfig(): m_firstLookupCompleted event is null, which means the service is shutting down!");
					throw new MonitoringADServiceShuttingDownException();
				}
			}
			Exception ex = null;
			IMonitoringADConfig monitoringADConfig = null;
			lock (this.m_locker)
			{
				monitoringADConfig = this.m_config;
				ex = this.LastException;
			}
			if (monitoringADConfig != null)
			{
				TimeSpan timeSpan = DateTime.UtcNow.Subtract(monitoringADConfig.CreateTimeUtc);
				if (allowedStaleness != InvokeWithTimeout.InfiniteTimeSpan)
				{
					if (timeSpan > allowedStaleness)
					{
						MonitoringADConfigManager.Tracer.TraceError<TimeSpan, TimeSpan>((long)this.GetHashCode(), "MonitoringADConfigManager.GetConfig(): Cached config is older ({0}) than max TTL ({1})", timeSpan, allowedStaleness);
						throw new MonitoringADConfigStaleException(timeSpan.ToString(), allowedStaleness.ToString(), AmExceptionHelper.GetExceptionMessageOrNoneString(ex), ex);
					}
				}
				else
				{
					MonitoringADConfigManager.Tracer.TraceDebug((long)this.GetHashCode(), "MonitoringADConfigManager.GetConfig(): Ignoring cached config staleness check.");
				}
				MonitoringADConfigManager.Tracer.TraceDebug<TimeSpan, DateTime>((long)this.GetHashCode(), "MonitoringADConfigManager.GetConfig(): Returning cached config of age ({0}), created at '{1} UTC'", timeSpan, monitoringADConfig.CreateTimeUtc);
				return monitoringADConfig;
			}
			if (ex == null)
			{
				MonitoringADConfigManager.Tracer.TraceError((long)this.GetHashCode(), "MonitoringADConfigManager.GetConfig(): AD initial lookup has not completed yet.");
				throw new MonitoringADInitNotCompleteException();
			}
			MonitoringADConfigManager.Tracer.TraceError<Exception>((long)this.GetHashCode(), "MonitoringADConfigManager.GetConfig(): Throwing last exception: {0}", ex);
			throw ex;
		}

		protected override void TimerCallbackInternal()
		{
			ExTraceGlobals.ADCacheTracer.TraceDebug((long)this.GetHashCode(), "Refresh: MonitoringADConfigManager.TimerCallbackInternal");
			this.RefreshInternal();
		}

		public void TryRefreshConfig()
		{
			try
			{
				InvokeWithTimeout.Invoke(delegate()
				{
					this.RefreshInternal();
				}, MonitoringADConfigManager.ConfigRefreshTimeout);
			}
			catch (TimeoutException ex)
			{
				MonitoringADConfigManager.Tracer.TraceError<TimeoutException>((long)this.GetHashCode(), "MonitoringADConfigManager.RunRefresh failed: {0}", ex);
				ReplayCrimsonEvents.MonitoringADLookupError.LogPeriodic<string, TimeoutException>(this.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, ex.Message, ex);
				this.healthReporter.RaiseRedEvent(ex.Message);
			}
		}

		private void RefreshInternal()
		{
			MonitoringADConfig config = null;
			Exception ex = null;
			try
			{
				Dependencies.ReplayAdObjectLookup.Clear();
				config = MonitoringADConfig.GetConfig(new AmServerName(Dependencies.ManagementClassHelper.LocalComputerFqdn), this.m_adObjectLookup, this.m_adObjectLookupPartiallyConsistent, this.m_adSession, this.m_adSessionPartiallyConsistent, () => base.PrepareToStopCalled);
				this.healthReporter.RaiseGreenEvent();
			}
			catch (MonitoringADConfigException ex2)
			{
				ex = ex2;
				ReplayCrimsonEvents.MonitoringADLookupError.LogPeriodic<string, MonitoringADConfigException>(this.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, ex2.ErrorMsg, ex2);
				this.healthReporter.RaiseRedEvent(ex2.ErrorMsg);
			}
			finally
			{
				lock (this.m_locker)
				{
					if (ex == null)
					{
						this.m_config = config;
					}
					this.LastException = ex;
				}
				this.m_firstLookupCompleted.Set();
			}
		}

		protected override void StopInternal()
		{
			base.StopInternal();
			this.m_firstLookupCompleted.Close();
		}

		private const string FirstMonitoringADLookupCompletedEventName = "FirstMonitoringADLookupCompletedEvent";

		private static readonly TimeSpan CachedConfigLongTTL = TimeSpan.FromSeconds((double)RegistryParameters.MonitoringADConfigStaleTimeoutLongInSec);

		private static readonly TimeSpan CachedConfigShortTTL = TimeSpan.FromSeconds((double)RegistryParameters.MonitoringADConfigStaleTimeoutShortInSec);

		private static readonly TimeSpan ConfigRefreshTimeout = TimeSpan.FromSeconds((double)RegistryParameters.MonitoringADGetConfigTimeoutInSec);

		private static readonly TimeSpan GetConfigWaitTimeout = TimeSpan.FromSeconds((double)RegistryParameters.MonitoringADGetConfigTimeoutInSec);

		private IMonitoringADConfig m_config;

		private IReplayAdObjectLookup m_adObjectLookup;

		private IReplayAdObjectLookup m_adObjectLookupPartiallyConsistent;

		private IADToplogyConfigurationSession m_adSession;

		private IADToplogyConfigurationSession m_adSessionPartiallyConsistent;

		private object m_locker = new object();

		private ADHealthReporter healthReporter = new ADHealthReporter();

		private ManualOneShotEvent m_firstLookupCompleted = new ManualOneShotEvent("FirstMonitoringADLookupCompletedEvent");
	}
}
