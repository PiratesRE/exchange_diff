using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogReplayScanControl
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.LogReplayerTracer;
			}
		}

		public LogReplayScanControl.ControlParameters Parameters { get; set; }

		public IADDatabase Database { get; private set; }

		public LogReplayScanControl(IADDatabase database, bool isLagCopy, IMonitoringADConfigProvider adConfigProvider, ICopyStatusClientLookup statusLookup, IPerfmonCounters keepingUpReader) : this(database, isLagCopy, adConfigProvider, statusLookup, keepingUpReader, new LogReplayScanControl.ControlParameters())
		{
		}

		public LogReplayScanControl(IADDatabase database, bool isLagCopy, IMonitoringADConfigProvider adConfigProvider, ICopyStatusClientLookup statusLookup, IPerfmonCounters keepingUpReader, LogReplayScanControl.ControlParameters parameters)
		{
			this.sensor = new AvailabilitySensor(database, adConfigProvider, statusLookup, parameters.MinAvailablePassiveCopies + 1, parameters.MaxProbeFreq);
			this.isLagCopy = isLagCopy;
			this.Database = database;
			this.Parameters = parameters;
			this.keepingUp = keepingUpReader;
		}

		public bool ShouldBeEnabled(bool isFastLagPlaydownDesired, long replayQ)
		{
			this.RunLogic(isFastLagPlaydownDesired, replayQ);
			return this.isEnabled;
		}

		private void RunLogic(bool isFastLagPlaydownDesired, long replayQ)
		{
			if (RegistryParameters.DisableDatabaseScan)
			{
				this.isEnabled = false;
				return;
			}
			if (isFastLagPlaydownDesired)
			{
				if (this.DisableScan())
				{
					this.LogDisable("FastLagPlaydownDesired");
				}
				return;
			}
			if (!this.isEnabled && ExDateTime.UtcNow - this.lastDisableTime < this.Parameters.MinDisabledWindow)
			{
				return;
			}
			if (this.isEnabled && replayQ < this.Parameters.MinReplayQOfInterest)
			{
				return;
			}
			if (this.isLagCopy)
			{
				if (replayQ > this.Parameters.MaxReplayQForBehindCopy)
				{
					if (this.DisableScan())
					{
						this.LogDisable(string.Format("Lag too behind. ReplayQ={0} MaxQ={1}", replayQ, this.Parameters.MaxReplayQForBehindCopy));
						return;
					}
				}
				else if (!this.isEnabled && replayQ < this.Parameters.MaxReplayQForAvailableCopy && this.EnableScan())
				{
					this.LogEnable(string.Format("Lag catching up. ReplayQ={0} MaxQ={1}", replayQ, this.Parameters.MaxReplayQForAvailableCopy));
				}
				return;
			}
			bool flag = this.sensor.DatabaseHasSufficientAvailability();
			if (this.sensor.FailedToRead)
			{
				if (replayQ > this.Parameters.MaxReplayQForAvailableCopy && this.DisableScan())
				{
					this.LogDisable("DatabaseHasSufficientAvailability failed");
				}
				return;
			}
			IHealthValidationResult lastReading = this.sensor.LastReading;
			if (!flag)
			{
				if (this.DisableScan())
				{
					string reason = string.Format("Database '{0}' has insufficient availability. MinAvail={1} CurAvail={2}", this.Database.Name, this.sensor.MinAvailableCopies, lastReading.HealthyCopiesCount);
					this.LogDisable(reason);
				}
				return;
			}
			if (this.isEnabled)
			{
				if (replayQ <= this.Parameters.MaxReplayQForAvailableCopy)
				{
					return;
				}
				if (lastReading.IsTargetCopyHealthy)
				{
					if (this.DisableScan())
					{
						this.LogDisable(string.Format("Availability at risk. ReplayQ={0}", replayQ));
					}
					return;
				}
				if (replayQ > this.Parameters.MaxReplayQForBehindCopy || (this.keepingUp != null && this.keepingUp.ReplayQueueNotKeepingUp > 0L))
				{
					if (this.DisableScan())
					{
						this.LogDisable(string.Format("Too far behind. ReplayQ={0}", replayQ));
					}
					return;
				}
			}
			else if (lastReading.IsTargetCopyHealthy)
			{
				if (replayQ > this.Parameters.MinReplayQOfInterest)
				{
					return;
				}
				if (this.EnableScan())
				{
					this.LogEnable(string.Format("Available copy recovered. ReplayQ={0}", replayQ));
				}
				return;
			}
			else if (this.EnableScan())
			{
				this.LogEnable(string.Format("Availability is ok due to other copies. This copy can resume scanning. ReplayQ={0}", replayQ));
			}
		}

		private bool EnableScan()
		{
			if (!this.isEnabled)
			{
				this.isEnabled = true;
				return true;
			}
			return false;
		}

		private bool DisableScan()
		{
			if (this.isEnabled)
			{
				this.isEnabled = false;
				this.lastDisableTime = ExDateTime.UtcNow;
				return true;
			}
			return false;
		}

		private void LogDisable(string reason)
		{
			LogReplayScanControl.Tracer.TraceError<string, string>((long)this.GetHashCode(), "LogReplayScanDisabled('{0}'): {1}", this.Database.Name, reason);
			ReplayCrimsonEvents.LogReplayScanDisabled.LogPeriodic<string, string, Guid, string>(this.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, this.Database.Name, Environment.MachineName, this.Database.Guid, reason);
		}

		private void LogEnable(string reason)
		{
			LogReplayScanControl.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "LogReplayScanEnabled('{0}'): {1}", this.Database.Name, reason);
			ReplayCrimsonEvents.LogReplayScanEnabled.LogPeriodic<string, string, Guid, string>(this.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, this.Database.Name, Environment.MachineName, this.Database.Guid, reason);
		}

		private readonly bool isLagCopy;

		private IPerfmonCounters keepingUp;

		private bool isEnabled = true;

		private ExDateTime lastDisableTime;

		private AvailabilitySensor sensor;

		public class ControlParameters
		{
			public int MinAvailablePassiveCopies { get; set; }

			public TimeSpan MinDisabledWindow { get; set; }

			public long MinReplayQOfInterest { get; set; }

			public long MaxReplayQForAvailableCopy { get; set; }

			public long MaxReplayQForBehindCopy { get; set; }

			public TimeSpan MaxProbeFreq { get; set; }

			public ControlParameters()
			{
				this.MinDisabledWindow = TimeSpan.FromSeconds(60.0);
				this.MinAvailablePassiveCopies = 1;
				this.MinReplayQOfInterest = 5L;
				this.MaxReplayQForAvailableCopy = 100L;
				this.MaxReplayQForBehindCopy = 10000L;
				this.MaxProbeFreq = AvailabilitySensor.DefaultMaxProbeFreq;
			}
		}
	}
}
