using System;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class ManagementClassHelper : IManagementClassHelper
	{
		public DateTime LocalBootTime
		{
			get
			{
				if (this.localBootTime == null)
				{
					lock (this.objectForLock)
					{
						if (this.localBootTime == null)
						{
							TimeSpan timeSpan;
							this.localBootTime = new DateTime?(this.GetLocalBootTime(out timeSpan));
						}
					}
				}
				return this.localBootTime.Value;
			}
		}

		public DateTime GetLocalBootTime(out TimeSpan systemUptime)
		{
			long tickCount = NativeMethods.GetTickCount64();
			systemUptime = TimeSpan.FromMilliseconds((double)tickCount);
			return DateTime.UtcNow.Subtract(systemUptime);
		}

		public DateTime GetBootTime(AmServerName machineName)
		{
			ManagementScope managementScope = this.GetManagementScope(machineName);
			ManagementPath path = new ManagementPath("Win32_OperatingSystem");
			ObjectGetOptions options = null;
			DateTime bootTimeWithWmi;
			using (ManagementClass managementClass = new ManagementClass(managementScope, path, options))
			{
				bootTimeWithWmi = this.GetBootTimeWithWmi(managementClass, machineName);
			}
			return bootTimeWithWmi;
		}

		private DateTime GetBootTimeWithWmi(ManagementClass mgmtClass, AmServerName machineName)
		{
			DateTime dateTime = ExDateTime.Now.UniversalTime;
			Exception ex = null;
			try
			{
				using (ManagementObjectCollection instances = mgmtClass.GetInstances())
				{
					if (instances != null)
					{
						using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ManagementBaseObject managementBaseObject = enumerator.Current;
								ManagementObject managementObject = (ManagementObject)managementBaseObject;
								using (managementObject)
								{
									string dmtfDate = (string)managementObject["LastBootupTime"];
									dateTime = ManagementDateTimeConverter.ToDateTime(dmtfDate).ToUniversalTime();
									AmTrace.Debug("GetBootTimeWithWmi: WMI says that the boot time for {0} is {1}.", new object[]
									{
										machineName,
										dateTime
									});
								}
							}
							goto IL_102;
						}
					}
					AmTrace.Error("GetBootTimeWithWmi: WMI could not query the boot time on server {0}: No instances found for management path {1}.", new object[]
					{
						machineName,
						mgmtClass.ClassPath.Path
					});
					ReplayEventLogConstants.Tuple_GetBootTimeWithWmiFailure.LogEvent(string.Empty, new object[]
					{
						machineName,
						Strings.NoInstancesFoundForManagementPath(mgmtClass.ClassPath.Path)
					});
					IL_102:;
				}
			}
			catch (COMException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			catch (ManagementException ex4)
			{
				ex = ex4;
			}
			catch (OutOfMemoryException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				AmTrace.Error("GetBootTimeWithWmi: WMI could not query the boot time on server {0}: {1}", new object[]
				{
					machineName,
					ex
				});
				ReplayEventLogConstants.Tuple_GetBootTimeWithWmiFailure.LogEvent(string.Empty, new object[]
				{
					machineName,
					ex.Message
				});
			}
			return dateTime;
		}

		private ManagementScope GetManagementScope(AmServerName machineName)
		{
			ManagementPath path = new ManagementPath(string.Format("\\\\{0}\\root\\cimv2", machineName.Fqdn));
			AmServerName amServerName = new AmServerName(Environment.MachineName);
			ConnectionOptions connectionOptions = new ConnectionOptions();
			if (!amServerName.Equals(machineName))
			{
				connectionOptions.Authority = string.Format("Kerberos:host/{0}", machineName.Fqdn);
			}
			return new ManagementScope(path, connectionOptions);
		}

		public string LocalComputerFqdn
		{
			get
			{
				if (this.localComputerFqdn == null)
				{
					lock (this.fqdnLock)
					{
						if (this.localComputerFqdn == null)
						{
							this.localComputerFqdn = NativeHelpers.GetLocalComputerFqdn(true);
						}
					}
				}
				return this.localComputerFqdn;
			}
		}

		public string LocalDomainName
		{
			get
			{
				if (this.localDomainName == null)
				{
					lock (this.fqdnLock)
					{
						if (this.localDomainName == null)
						{
							this.localDomainName = NativeHelpers.GetDomainName();
						}
					}
				}
				return this.localDomainName;
			}
		}

		string IManagementClassHelper.LocalMachineName
		{
			get
			{
				if (this.localMachineName == null)
				{
					lock (this.objectForLock)
					{
						if (this.localMachineName == null)
						{
							this.localMachineName = Environment.MachineName;
						}
					}
				}
				return this.localMachineName;
			}
		}

		private string localComputerFqdn;

		private string localDomainName;

		private DateTime? localBootTime = null;

		private string localMachineName;

		private object objectForLock = new object();

		private object fqdnLock = new object();
	}
}
