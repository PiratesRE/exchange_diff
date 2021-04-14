using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FrameworkAggregationConfiguration
	{
		private FrameworkAggregationConfiguration()
		{
			this.deltaSyncSettingsUpdateInterval = TimeSpan.FromDays(1.0);
			this.deltaSyncEndPointUnreachableThreshold = TimeSpan.FromHours(1.0);
			this.imapMaxFoldersSupported = 2000;
			this.ReadAppConfig();
		}

		internal static FrameworkAggregationConfiguration Instance
		{
			get
			{
				if (FrameworkAggregationConfiguration.instance == null)
				{
					lock (FrameworkAggregationConfiguration.instanceInitializationLock)
					{
						if (FrameworkAggregationConfiguration.instance == null)
						{
							FrameworkAggregationConfiguration.instance = new FrameworkAggregationConfiguration();
						}
					}
				}
				return FrameworkAggregationConfiguration.instance;
			}
		}

		internal static ExEventLog EventLogger
		{
			get
			{
				return FrameworkAggregationConfiguration.eventLogger;
			}
		}

		public TimeSpan DeltaSyncSettingsUpdateInterval
		{
			get
			{
				return this.deltaSyncSettingsUpdateInterval;
			}
		}

		public TimeSpan DeltaSyncEndPointUnreachableThreshold
		{
			get
			{
				return this.deltaSyncEndPointUnreachableThreshold;
			}
		}

		public int ImapMaxFoldersSupported
		{
			get
			{
				return this.imapMaxFoldersSupported;
			}
		}

		private void ReadAppConfig()
		{
			string[] array = new string[]
			{
				"DeltaSyncEndPointUnreachableThreshold",
				"DeltaSyncSettingsUpdateInterval",
				"ImapMaxFoldersSupported"
			};
			string exePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FrameworkAggregationConfiguration.TransportProcessName);
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(exePath);
			foreach (string text in array)
			{
				string text2 = null;
				try
				{
					if (configuration.AppSettings.Settings[text] != null)
					{
						text2 = configuration.AppSettings.Settings[text].Value;
					}
				}
				catch (ConfigurationErrorsException arg)
				{
					ExTraceGlobals.CommonTracer.TraceWarning<string, ConfigurationErrorsException>(0L, "failed to read config {0}: {1}", text, arg);
				}
				if (string.IsNullOrEmpty(text2))
				{
					ExTraceGlobals.CommonTracer.TraceDebug<string>(0L, "cannot apply null/empty config {0}", text);
				}
				else
				{
					bool flag = true;
					string a;
					if ((a = text) != null)
					{
						TimeSpan timeSpan;
						if (!(a == "DeltaSyncEndPointUnreachableThreshold"))
						{
							if (!(a == "DeltaSyncSettingsUpdateInterval"))
							{
								if (a == "ImapMaxFoldersSupported")
								{
									int num;
									if (int.TryParse(text2, out num))
									{
										this.imapMaxFoldersSupported = num;
									}
									else
									{
										flag = false;
									}
								}
							}
							else if (TimeSpan.TryParse(text2, out timeSpan))
							{
								this.deltaSyncSettingsUpdateInterval = timeSpan;
							}
							else
							{
								flag = false;
							}
						}
						else if (TimeSpan.TryParse(text2, out timeSpan))
						{
							this.deltaSyncEndPointUnreachableThreshold = timeSpan;
						}
						else
						{
							flag = false;
						}
					}
					if (!flag)
					{
						ExTraceGlobals.CommonTracer.TraceWarning<string, string>(0L, "cannot apply config {0} with invalid value: {1}", text, text2);
					}
				}
			}
		}

		private static readonly string TransportProcessName = "EdgeTransport.exe";

		private static object instanceInitializationLock = new object();

		private static FrameworkAggregationConfiguration instance;

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.EventLogTracer.Category, "MSExchangeTransportSyncWorker");

		private TimeSpan deltaSyncSettingsUpdateInterval;

		private TimeSpan deltaSyncEndPointUnreachableThreshold;

		private int imapMaxFoldersSupported;
	}
}
