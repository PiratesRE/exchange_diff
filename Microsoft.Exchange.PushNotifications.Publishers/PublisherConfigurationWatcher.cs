using System;
using System.Timers;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PublisherConfigurationWatcher : IDisposable
	{
		public PublisherConfigurationWatcher(string service, int refreshRateInMinutes)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("service", service);
			ArgumentValidator.ThrowIfNegative("refreshRateInMinutes", refreshRateInMinutes);
			this.Service = service;
			this.refreshRateInMinutes = refreshRateInMinutes;
		}

		public event EventHandler<ConfigurationChangedEventArgs> OnChangeEvent;

		public event EventHandler<ConfigurationReadEventArgs> OnReadEvent;

		public string Service { get; private set; }

		public int ResfreshRateInMinutes
		{
			get
			{
				return this.refreshRateInMinutes;
			}
		}

		public PushNotificationPublisherConfiguration Start()
		{
			this.configuration = this.LoadConfiguration(true);
			if (this.ResfreshRateInMinutes > 0)
			{
				this.timer = new Timer((double)(this.refreshRateInMinutes * 60 * 1000));
				this.timer.Elapsed += this.OnTimedEvent;
				this.timer.Start();
			}
			return this.configuration;
		}

		public void Dispose()
		{
			if (this.timer != null)
			{
				this.timer.Stop();
				this.timer.Dispose();
				this.timer = null;
			}
			if (this.OnChangeEvent != null)
			{
				this.OnChangeEvent = null;
			}
		}

		internal void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			try
			{
				PushNotificationPublisherConfiguration pushNotificationPublisherConfiguration = this.LoadConfiguration(false);
				EventHandler<ConfigurationReadEventArgs> onReadEvent = this.OnReadEvent;
				if (onReadEvent != null)
				{
					onReadEvent(this, new ConfigurationReadEventArgs(pushNotificationPublisherConfiguration));
				}
				if (!this.configuration.Equals(pushNotificationPublisherConfiguration))
				{
					string arg = this.configuration.ToFullString();
					string arg2 = pushNotificationPublisherConfiguration.ToFullString();
					ExTraceGlobals.PushNotificationServiceTracer.TraceDebug<int, string, string>((long)this.GetHashCode(), "[PublisherConfigurationWatcher:OnTimedEvent] Worker Process to be recycled by the configuration watcher due to a change on the configuration. Interval={0} minutes | Current={1} | Updated={2}.", this.refreshRateInMinutes, arg, arg2);
					PushNotificationsCrimsonEvents.PublisherConfigurationChanged.Log<string, string>(string.Format("{{interval:{0}; current:{{{1}}}; updated:{{{2}}}}}", this.refreshRateInMinutes, arg, arg2), this.Service);
					this.configuration = pushNotificationPublisherConfiguration;
					EventHandler<ConfigurationChangedEventArgs> onChangeEvent = this.OnChangeEvent;
					if (onChangeEvent != null)
					{
						onChangeEvent(this, new ConfigurationChangedEventArgs(pushNotificationPublisherConfiguration));
					}
				}
			}
			catch (Exception ex)
			{
				this.ReportException(ex);
			}
		}

		protected virtual PushNotificationPublisherConfiguration LoadConfiguration(bool ignoreErrors = false)
		{
			return new PushNotificationPublisherConfiguration(ignoreErrors, null);
		}

		protected virtual void ReportException(Exception ex)
		{
			PushNotificationsCrimsonEvents.ErrorReadingConfiguration.Log<Exception>(ex);
			if (ExTraceGlobals.PushNotificationServiceTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ExTraceGlobals.PushNotificationServiceTracer.TraceError<string>((long)this.GetHashCode(), "[PublisherConfigurationWatcher:OnTimedEvent] An error occurred when attempting to read the server configuration {1}.", ex.ToTraceString());
			}
		}

		private readonly int refreshRateInMinutes;

		private Timer timer;

		private PushNotificationPublisherConfiguration configuration;
	}
}
