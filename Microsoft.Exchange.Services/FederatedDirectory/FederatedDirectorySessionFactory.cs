using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Office.Server.Directory;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal static class FederatedDirectorySessionFactory
	{
		public static DirectorySession Create(ADUser accessingUser, ExchangePrincipal accessingPrincipal)
		{
			ArgumentValidator.ThrowIfNull("accessingUser", accessingUser);
			ArgumentValidator.ThrowIfNull("accessingPrincipal", accessingPrincipal);
			FederatedDirectorySessionFactory.InitializeIfNeeded();
			ExchangeDirectorySessionContext exchangeDirectorySessionContext = new ExchangeDirectorySessionContext(accessingUser, accessingPrincipal);
			FederatedDirectorySessionFactory.Tracer.TraceDebug<ExchangeDirectorySessionContext>(0L, "Created DirectorySession with context: {0}", exchangeDirectorySessionContext);
			return new DirectorySession(exchangeDirectorySessionContext);
		}

		private static void InitializeIfNeeded()
		{
			if (!FederatedDirectorySessionFactory.initialized)
			{
				lock (FederatedDirectorySessionFactory.locker)
				{
					if (!FederatedDirectorySessionFactory.initialized)
					{
						FederatedDirectorySessionFactory.Initialize();
						FederatedDirectorySessionFactory.initialized = true;
					}
				}
			}
		}

		private static void Initialize()
		{
			FederatedDirectorySessionFactory.Tracer.TraceDebug(0L, "Initializing AdapterManager and LogManager");
			LogWriter.Initialize();
			Assembly assembly = Assembly.GetAssembly(typeof(AdapterManager));
			string text = Path.Combine(Path.GetDirectoryName(assembly.Location), "FederatedDirectory.config");
			if (!File.Exists(text))
			{
				text = Path.Combine(ExchangeSetupContext.BinPath, "FederatedDirectory.config");
			}
			FederatedDirectorySessionFactory.Tracer.TraceDebug<string>(0L, "Using config file {0}", text);
			AdapterManager.Initialize(text);
			FederatedDirectorySessionFactory.Tracer.TraceDebug(0L, "AdapterManager and LogManager Initialized");
		}

		private const string ConfigFileName = "FederatedDirectory.config";

		private static readonly Trace Tracer = ExTraceGlobals.FederatedDirectoryTracer;

		private static bool initialized;

		private static object locker = new object();
	}
}
