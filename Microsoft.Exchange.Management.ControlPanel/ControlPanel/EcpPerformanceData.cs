using System;
using Microsoft.Exchange.Diagnostics.LatencyDetection;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class EcpPerformanceData
	{
		public static PerformanceDataProvider CreateRbacSession
		{
			get
			{
				if (EcpPerformanceData.createRbacSession == null)
				{
					EcpPerformanceData.createRbacSession = new PerformanceDataProvider("CreateRbacSession");
				}
				return EcpPerformanceData.createRbacSession;
			}
		}

		public static PerformanceDataProvider ActiveRunspace
		{
			get
			{
				if (EcpPerformanceData.activeRunspace == null)
				{
					EcpPerformanceData.activeRunspace = new PerformanceDataProvider("ActiveRunspace");
				}
				return EcpPerformanceData.activeRunspace;
			}
		}

		public static PerformanceDataProvider CreateRunspace
		{
			get
			{
				if (EcpPerformanceData.createRunspace == null)
				{
					EcpPerformanceData.createRunspace = new PerformanceDataProvider("CreateRunspace");
				}
				return EcpPerformanceData.createRunspace;
			}
		}

		internal static PerformanceDataProvider PowerShellInvoke
		{
			get
			{
				if (EcpPerformanceData.powerShellInvoke == null)
				{
					EcpPerformanceData.powerShellInvoke = new PerformanceDataProvider("PowerShellInvoke");
				}
				return EcpPerformanceData.powerShellInvoke;
			}
		}

		internal static PerformanceDataProvider WcfSerialization
		{
			get
			{
				if (EcpPerformanceData.wcfSerialization == null)
				{
					EcpPerformanceData.wcfSerialization = new PerformanceDataProvider("WcfSerialization");
				}
				return EcpPerformanceData.wcfSerialization;
			}
		}

		public static PerformanceDataProvider XamlParsed
		{
			get
			{
				if (EcpPerformanceData.xamlParsed == null)
				{
					EcpPerformanceData.xamlParsed = new PerformanceDataProvider("Xaml Parsed");
				}
				return EcpPerformanceData.xamlParsed;
			}
		}

		public static PerformanceDataProvider DDIServiceExecution
		{
			get
			{
				if (EcpPerformanceData.ddiServiceExecution == null)
				{
					EcpPerformanceData.ddiServiceExecution = new PerformanceDataProvider("DDI Service Execution");
				}
				return EcpPerformanceData.ddiServiceExecution;
			}
		}

		public static PerformanceDataProvider DDITypeConversion
		{
			get
			{
				if (EcpPerformanceData.ddiTypeConversion == null)
				{
					EcpPerformanceData.ddiTypeConversion = new PerformanceDataProvider("DDI Type Conversion");
				}
				return EcpPerformanceData.ddiTypeConversion;
			}
		}

		[ThreadStatic]
		private static PerformanceDataProvider createRbacSession;

		[ThreadStatic]
		private static PerformanceDataProvider activeRunspace;

		[ThreadStatic]
		private static PerformanceDataProvider createRunspace;

		[ThreadStatic]
		private static PerformanceDataProvider powerShellInvoke;

		[ThreadStatic]
		private static PerformanceDataProvider wcfSerialization;

		[ThreadStatic]
		private static PerformanceDataProvider xamlParsed;

		[ThreadStatic]
		private static PerformanceDataProvider ddiServiceExecution;

		[ThreadStatic]
		private static PerformanceDataProvider ddiTypeConversion;
	}
}
