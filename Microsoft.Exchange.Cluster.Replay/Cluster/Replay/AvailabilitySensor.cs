using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class AvailabilitySensor
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.LogReplayerTracer;
			}
		}

		public bool FailedToRead { get; private set; }

		public IHealthValidationResult LastReading { get; private set; }

		public ExDateTime LastReadingTime { get; private set; }

		public int MinAvailableCopies { get; set; }

		public TimeSpan MaxProbeFreq { get; set; }

		public IMonitoringADConfigProvider ADConfigProvider { get; private set; }

		public ICopyStatusClientLookup CopyStatusLookup { get; private set; }

		public IADDatabase Database { get; private set; }

		public AvailabilitySensor(IADDatabase database, IMonitoringADConfigProvider adConfigProvider, ICopyStatusClientLookup statusLookup) : this(database, adConfigProvider, statusLookup, 2, AvailabilitySensor.DefaultMaxProbeFreq)
		{
		}

		public AvailabilitySensor(IADDatabase database, IMonitoringADConfigProvider adConfigProvider, ICopyStatusClientLookup statusLookup, int minCopies, TimeSpan maxProbeFreq)
		{
			this.Database = database;
			this.ADConfigProvider = adConfigProvider;
			this.CopyStatusLookup = statusLookup;
			this.MinAvailableCopies = minCopies;
			this.MaxProbeFreq = maxProbeFreq;
		}

		public bool DatabaseHasSufficientAvailability()
		{
			if (this.LastReading != null)
			{
				if (!(ExDateTime.UtcNow - this.LastReadingTime >= this.MaxProbeFreq))
				{
					goto IL_C6;
				}
			}
			try
			{
				IMonitoringADConfig configIgnoringStaleness = this.ADConfigProvider.GetConfigIgnoringStaleness(true);
				DatabaseAvailabilityValidator databaseAvailabilityValidator = new DatabaseAvailabilityValidator(this.Database, this.MinAvailableCopies, this.CopyStatusLookup, configIgnoringStaleness, null, true);
				this.LastReading = databaseAvailabilityValidator.Run();
				this.LastReadingTime = ExDateTime.Now;
			}
			catch (MonitoringADServiceShuttingDownException arg)
			{
				AvailabilitySensor.Tracer.TraceError<MonitoringADServiceShuttingDownException>((long)this.GetHashCode(), "AvailabilitySensor: Got service shutting down exception when retrieving AD config: {0}", arg);
			}
			catch (MonitoringADConfigException ex)
			{
				this.FailedToRead = true;
				AvailabilitySensor.Tracer.TraceError<MonitoringADConfigException>((long)this.GetHashCode(), "AvailabilitySensor: Got exception when retrieving AD config: {0}", ex);
				ReplayCrimsonEvents.AvailabilitySensorError.LogPeriodic<string, MonitoringADConfigException>(this.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, ex.Message, ex);
			}
			IL_C6:
			return this.LastReading != null && this.LastReading.HealthyCopiesCount >= this.MinAvailableCopies;
		}

		public const int DefaultMinAvailableCopies = 2;

		public static readonly TimeSpan DefaultMaxProbeFreq = TimeSpan.FromSeconds(60.0);
	}
}
