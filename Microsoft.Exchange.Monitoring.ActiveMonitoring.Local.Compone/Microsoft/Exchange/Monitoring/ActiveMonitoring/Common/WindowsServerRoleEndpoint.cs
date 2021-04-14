using System;
using System.Management;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class WindowsServerRoleEndpoint : IEndpoint
	{
		public bool IsDirectoryServiceRoleInstalled
		{
			get
			{
				return this.isDirectoryServiceRoleInstalled;
			}
		}

		public bool IsDhcpServerRoleInstalled
		{
			get
			{
				return this.isDhcpServerRoleInstalled;
			}
		}

		public bool IsDnsServerRoleInstalled
		{
			get
			{
				return this.isDnsServerRoleInstalled;
			}
		}

		public bool IsNatServerRoleInstalled
		{
			get
			{
				return this.isNatServerRoleInstalled;
			}
		}

		public bool RestartOnChange
		{
			get
			{
				return true;
			}
		}

		public Exception Exception { get; set; }

		public void Initialize()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.WindowsServerRoleEndpointTracer, this.traceContext, "Checking Windows server role configuration", null, "Initialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\WindowsServerRoleEndpoint.cs", 147);
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_ServerFeature WHERE ParentID = 0"))
			{
				using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						using (managementObject)
						{
							switch ((uint)managementObject["ID"])
							{
							case 10U:
								this.isDirectoryServiceRoleInstalled = true;
								break;
							case 12U:
								this.isDhcpServerRoleInstalled = true;
								break;
							case 13U:
								this.isDnsServerRoleInstalled = true;
								break;
							case 14U:
								this.isNatServerRoleInstalled = true;
								break;
							}
						}
					}
				}
			}
		}

		public bool DetectChange()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.WindowsServerRoleEndpointTracer, this.traceContext, "Detecting Windows server role configuration change", null, "DetectChange", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\WindowsServerRoleEndpoint.cs", 189);
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_ServerFeature WHERE ParentID = 0"))
			{
				using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						switch ((uint)managementObject["ID"])
						{
						case 10U:
							if (!this.isDirectoryServiceRoleInstalled)
							{
								return true;
							}
							break;
						case 12U:
							if (!this.isDhcpServerRoleInstalled)
							{
								return true;
							}
							break;
						case 13U:
							if (!this.isDnsServerRoleInstalled)
							{
								return true;
							}
							break;
						case 14U:
							if (!this.isNatServerRoleInstalled)
							{
								return true;
							}
							break;
						}
					}
				}
			}
			return false;
		}

		private const string WmiQueryString = "SELECT * FROM Win32_ServerFeature WHERE ParentID = 0";

		private const uint DirectoryServiceFeatureId = 10U;

		private const uint DhcpFeatureId = 12U;

		private const uint DnsFeatureId = 13U;

		private const uint NatFeatureId = 14U;

		private bool isDirectoryServiceRoleInstalled;

		private bool isDhcpServerRoleInstalled;

		private bool isDnsServerRoleInstalled;

		private bool isNatServerRoleInstalled;

		private TracingContext traceContext = TracingContext.Default;
	}
}
