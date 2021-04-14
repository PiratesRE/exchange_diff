using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal class DiagnosticsSessionFactory : IDiagnosticsSessionFactory
	{
		internal static DiagnosticsLogConfig.LogDefaults GracefulDegradationLogDefaults
		{
			get
			{
				if (DiagnosticsSessionFactory.gracefulDegradationLogDefaults == null)
				{
					DiagnosticsSessionFactory.gracefulDegradationLogDefaults = DiagnosticsSessionFactory.CreateLogDefaults(DiagnosticsSessionFactory.GracefulDegradationLogGuid, "Search Graceful Degradation Logs", "Search\\GracefulDegradation", "GracefulDegradation_");
				}
				return DiagnosticsSessionFactory.gracefulDegradationLogDefaults;
			}
		}

		internal static DiagnosticsLogConfig.LogDefaults DictionaryLogDefaults
		{
			get
			{
				if (DiagnosticsSessionFactory.dictionaryLogDefaults == null)
				{
					DiagnosticsSessionFactory.dictionaryLogDefaults = DiagnosticsSessionFactory.CreateLogDefaults(DiagnosticsSessionFactory.DictionaryLogGuid, "Search Dictionary Logs", "Search\\Dictionary", "Dictionary_");
				}
				return DiagnosticsSessionFactory.dictionaryLogDefaults;
			}
		}

		private static DiagnosticsLogConfig.LogDefaults LogDefaults
		{
			get
			{
				if (DiagnosticsSessionFactory.logDefaults == null)
				{
					DiagnosticsSessionFactory.logDefaults = DiagnosticsSessionFactory.CreateLogDefaults(DiagnosticsSessionFactory.ServiceLogGuid, "Search Diagnostics Logs", "Search", "Search_");
				}
				return DiagnosticsSessionFactory.logDefaults;
			}
		}

		private static DiagnosticsLogConfig.LogDefaults CrawlerLogDefaults
		{
			get
			{
				if (DiagnosticsSessionFactory.crawlerLogDefaults == null)
				{
					DiagnosticsSessionFactory.crawlerLogDefaults = DiagnosticsSessionFactory.CreateLogDefaults(DiagnosticsSessionFactory.CrawlerLogGuid, "Search Crawler Logs", "Search\\Crawler", "SearchCrawler_");
				}
				return DiagnosticsSessionFactory.crawlerLogDefaults;
			}
		}

		public static void SetDefaults(Guid eventLogComponentGuid, string serviceName, string logTypeName, string logFilePath, string logFilePrefix, string logComponent)
		{
			DiagnosticsSessionFactory.logDefaults = new DiagnosticsLogConfig.LogDefaults(eventLogComponentGuid, serviceName, logTypeName, logFilePath, logFilePrefix, logComponent);
		}

		public IDiagnosticsSession CreateComponentDiagnosticsSession(string componentName, Trace tracer, long traceContext)
		{
			return this.CreateComponentDiagnosticsSession(componentName, null, tracer, traceContext);
		}

		public IDiagnosticsSession CreateComponentDiagnosticsSession(string componentName, string eventLogSourceName, Trace tracer, long traceContext)
		{
			return new DiagnosticsSession(componentName, eventLogSourceName, (tracer == null) ? ExTraceGlobals.GeneralTracer : tracer, traceContext, null, DiagnosticsSessionFactory.LogDefaults, DiagnosticsSessionFactory.CrawlerLogDefaults, DiagnosticsSessionFactory.GracefulDegradationLogDefaults, DiagnosticsSessionFactory.DictionaryLogDefaults);
		}

		public IDiagnosticsSession CreateDocumentDiagnosticsSession(IIdentity documentId, Trace tracer)
		{
			return new DiagnosticsSession("Document", null, (tracer == null) ? ExTraceGlobals.GeneralTracer : tracer, (long)documentId.GetHashCode(), documentId, DiagnosticsSessionFactory.LogDefaults, DiagnosticsSessionFactory.CrawlerLogDefaults, DiagnosticsSessionFactory.GracefulDegradationLogDefaults, DiagnosticsSessionFactory.DictionaryLogDefaults);
		}

		private static DiagnosticsLogConfig.LogDefaults CreateLogDefaults(Guid guid, string logTypeName, string path, string prefix)
		{
			string path2;
			try
			{
				path2 = ExchangeSetupContext.InstallPath;
			}
			catch (SetupVersionInformationCorruptException)
			{
				path2 = string.Empty;
			}
			return new DiagnosticsLogConfig.LogDefaults(guid, ComponentInstance.Globals.Search.ServiceName, logTypeName, Path.Combine(path2, "Logging", path), prefix, "SearchLogs");
		}

		private static readonly Guid ServiceLogGuid = Guid.Parse("c87fb454-7dfe-4559-af8c-3905438e1398");

		private static readonly Guid CrawlerLogGuid = Guid.Parse("f58e945b-181f-412e-8c46-96bd3b05727d");

		private static readonly Guid GracefulDegradationLogGuid = Guid.Parse("2f753801-8857-477b-a4c5-8605253df53d");

		private static readonly Guid DictionaryLogGuid = Guid.Parse("9151E90B-B707-42F1-A24B-98B47079450B");

		private static DiagnosticsLogConfig.LogDefaults logDefaults;

		private static DiagnosticsLogConfig.LogDefaults crawlerLogDefaults;

		private static DiagnosticsLogConfig.LogDefaults gracefulDegradationLogDefaults;

		private static DiagnosticsLogConfig.LogDefaults dictionaryLogDefaults;
	}
}
