using System;
using System.Diagnostics;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.EseRepl.Common;
using Microsoft.Exchange.EseRepl.Configuration;
using Microsoft.Practices.Unity;

namespace Microsoft.Exchange.EseRepl
{
	internal static class Dependencies
	{
		public static IUnityContainer Container
		{
			get
			{
				return Dependencies.container;
			}
		}

		public static IRegistryReader RegistryReader
		{
			get
			{
				return Dependencies.container.Resolve<IRegistryReader>();
			}
		}

		public static IRegistryWriter RegistryWriter
		{
			get
			{
				return Dependencies.container.Resolve<IRegistryWriter>();
			}
		}

		public static ITracer DagNetChooserTracer
		{
			get
			{
				return Dependencies.container.Resolve<ITracer>(0.ToString());
			}
		}

		public static ITracer DagNetEnvironmentTracer
		{
			get
			{
				return Dependencies.container.Resolve<ITracer>(1.ToString());
			}
		}

		public static IEseReplConfig Config
		{
			get
			{
				return Dependencies.container.Resolve<IEseReplConfig>();
			}
		}

		public static ISerialization Serializer
		{
			get
			{
				return Dependencies.container.Resolve<ISerialization>();
			}
		}

		public static ITcpConnector TcpConnector
		{
			get
			{
				return Dependencies.tcpConnector;
			}
		}

		internal static void OverrideTcpConnector(ITcpConnector conn)
		{
			Dependencies.tcpConnector = conn;
		}

		private static IAssert Assert
		{
			get
			{
				return Dependencies.container.Resolve<IAssert>();
			}
		}

		[Conditional("DEBUG")]
		public static void AssertDbg(bool condition, string formatString, params object[] parameters)
		{
			Dependencies.Assert.Debug(condition, formatString, parameters);
		}

		[Conditional("DEBUG")]
		public static void AssertDbg(bool condition)
		{
			Dependencies.Assert.Debug(condition, "No info for this assert", new object[0]);
		}

		public static void AssertRtl(bool condition, string formatString, params object[] parameters)
		{
			Dependencies.Assert.Retail(condition, formatString, parameters);
		}

		public static void RegisterAll()
		{
			Dependencies.container.Dispose();
			Dependencies.container = EseReplDependencies.Initialize();
			Dependencies.tcpConnector = new TcpConnector();
		}

		public static void UnregisterAll()
		{
			Dependencies.container.Dispose();
			Dependencies.container = new UnityContainer();
		}

		private static IUnityContainer container = EseReplDependencies.Initialize();

		private static ITcpConnector tcpConnector = new TcpConnector();
	}
}
