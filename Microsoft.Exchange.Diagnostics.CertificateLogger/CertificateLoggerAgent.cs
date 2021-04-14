using System;
using Microsoft.Exchange.Diagnostics.Service;
using Microsoft.Exchange.Diagnostics.Service.Common;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.CertificateLogger
{
	public sealed class CertificateLoggerAgent : RetentionAgent
	{
		public CertificateLoggerAgent(string enforcedDirectory, TimeSpan retentionPeriod, int maxDirectorySizeMBytes, TimeSpan checkInterval, bool logDataLossMessage) : base(enforcedDirectory, retentionPeriod, maxDirectorySizeMBytes, checkInterval, logDataLossMessage)
		{
			TimeSpan configTimeSpan = Configuration.GetConfigTimeSpan("CertificateLogger_MonitoringInterval", TimeSpan.FromDays(0.0), TimeSpan.MaxValue, TimeSpan.FromDays(1.0));
			long maxDirectorySize = (long)(maxDirectorySizeMBytes * 1024 * 1024);
			long configLong = Configuration.GetConfigLong("CertificateLogger_MaxFileSize", 1048576L, 104857600L, 10485760L);
			int configInt = Configuration.GetConfigInt("CertificateLogger_MaxBufferSize", 0, 10485760, 1024);
			TimeSpan configTimeSpan2 = Configuration.GetConfigTimeSpan("CertificateLogger_StreamFlushInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(60.0), TimeSpan.FromSeconds(5.0));
			this.certLogger = new CertificateLogger(enforcedDirectory, configTimeSpan, maxDirectorySize, configLong, configInt, configTimeSpan2);
			Logger.LogInformationMessage("Started certificate log monitor.", new object[0]);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.certLogger != null)
			{
				this.certLogger.Dispose();
			}
			base.InternalDispose(disposing);
		}

		private const string MonitoringIntervalConfigurationLabel = "CertificateLogger_MonitoringInterval";

		private const string MaxFileSizeConfigurationLabel = "CertificateLogger_MaxFileSize";

		private const string MaxBufferSizeConfigurationLabel = "CertificateLogger_MaxBufferSize";

		private const string StreamFlushIntervalConfigurationLabel = "CertificateLogger_StreamFlushInterval";

		private readonly CertificateLogger certLogger;
	}
}
