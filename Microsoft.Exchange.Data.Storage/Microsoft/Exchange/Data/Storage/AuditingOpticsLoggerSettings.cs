using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuditingOpticsLoggerSettings
	{
		internal bool Enabled { get; private set; }

		internal string DirectoryPath { get; private set; }

		internal TimeSpan MaxAge { get; private set; }

		internal ByteQuantifiedSize MaxDirectorySize { get; private set; }

		internal ByteQuantifiedSize MaxFileSize { get; private set; }

		internal ByteQuantifiedSize CacheSize { get; private set; }

		internal TimeSpan FlushInterval { get; private set; }

		internal bool FlushToDisk { get; private set; }

		internal static AuditingOpticsLoggerSettings Load()
		{
			AuditingOpticsLoggerSettings.Tracer.TraceDebug(0L, "Start loading Auditing Optics log settings.");
			AuditingOpticsLoggerSettings auditingOpticsLoggerSettings = new AuditingOpticsLoggerSettings();
			BoolAppSettingsEntry boolAppSettingsEntry = new BoolAppSettingsEntry("LogEnabled", true, AuditingOpticsLoggerSettings.Tracer);
			auditingOpticsLoggerSettings.Enabled = boolAppSettingsEntry.Value;
			StringAppSettingsEntry stringAppSettingsEntry = new StringAppSettingsEntry("LogDirectoryPath", Path.Combine(ExchangeSetupContext.LoggingPath, AuditingOpticsConstants.LoggerComponentName), AuditingOpticsLoggerSettings.Tracer);
			auditingOpticsLoggerSettings.DirectoryPath = stringAppSettingsEntry.Value;
			TimeSpanAppSettingsEntry timeSpanAppSettingsEntry = new TimeSpanAppSettingsEntry("LogFileAgeInDays", TimeSpanUnit.Days, TimeSpan.FromDays(30.0), AuditingOpticsLoggerSettings.Tracer);
			auditingOpticsLoggerSettings.MaxAge = timeSpanAppSettingsEntry.Value;
			ByteQuantifiedSizeAppSettingsEntry byteQuantifiedSizeAppSettingsEntry = new ByteQuantifiedSizeAppSettingsEntry("LogDirectorySizeLimit", ByteQuantifiedSize.Parse("100MB"), AuditingOpticsLoggerSettings.Tracer);
			auditingOpticsLoggerSettings.MaxDirectorySize = byteQuantifiedSizeAppSettingsEntry.Value;
			ByteQuantifiedSizeAppSettingsEntry byteQuantifiedSizeAppSettingsEntry2 = new ByteQuantifiedSizeAppSettingsEntry("LogFileSizeLimit", ByteQuantifiedSize.Parse("10MB"), AuditingOpticsLoggerSettings.Tracer);
			auditingOpticsLoggerSettings.MaxFileSize = byteQuantifiedSizeAppSettingsEntry2.Value;
			ByteQuantifiedSizeAppSettingsEntry byteQuantifiedSizeAppSettingsEntry3 = new ByteQuantifiedSizeAppSettingsEntry("LogCacheSizeLimit", ByteQuantifiedSize.Parse("256KB"), AuditingOpticsLoggerSettings.Tracer);
			auditingOpticsLoggerSettings.CacheSize = byteQuantifiedSizeAppSettingsEntry3.Value;
			TimeSpanAppSettingsEntry timeSpanAppSettingsEntry2 = new TimeSpanAppSettingsEntry("LogFlushIntervalInSeconds", TimeSpanUnit.Seconds, TimeSpan.FromSeconds(60.0), AuditingOpticsLoggerSettings.Tracer);
			auditingOpticsLoggerSettings.FlushInterval = timeSpanAppSettingsEntry2.Value;
			auditingOpticsLoggerSettings.FlushToDisk = true;
			AuditingOpticsLoggerSettings.Tracer.TraceDebug(0L, "The Auditing Optics log settings are loaded successfully.");
			return auditingOpticsLoggerSettings;
		}

		private static readonly Trace Tracer = ExTraceGlobals.AdminAuditLogTracer;
	}
}
