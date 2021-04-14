using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Common.LocStrings;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class ManageServiceBase : Task
	{
		public ManageServiceBase()
		{
		}

		internal static ServiceControllerStatus GetServiceStatus(string serviceName)
		{
			ServiceControllerStatus status;
			using (ServiceController serviceController = new ServiceController(serviceName))
			{
				status = serviceController.Status;
			}
			return status;
		}

		internal void DoNativeServiceTask(string serviceName, ServiceAccessFlags serviceAccessFlags, ManageServiceBase.NativeServiceTaskDelegate task)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			try
			{
				intPtr = NativeMethods.OpenSCManager(null, null, ServiceControlManagerAccessFlags.AllAccess);
				if (IntPtr.Zero == intPtr)
				{
					base.WriteError(TaskWin32Exception.FromErrorCodeAndVerbose(Marshal.GetLastWin32Error(), Strings.ErrorCannotOpenServiceControllerManager), ErrorCategory.ReadError, null);
				}
				intPtr2 = NativeMethods.OpenService(intPtr, serviceName, serviceAccessFlags);
				if (IntPtr.Zero == intPtr2)
				{
					base.WriteError(TaskWin32Exception.FromErrorCodeAndVerbose(Marshal.GetLastWin32Error(), Strings.ErrorCannotOpenService(serviceName)), ErrorCategory.ReadError, null);
				}
				task(intPtr2);
			}
			finally
			{
				if (IntPtr.Zero != intPtr2 && !NativeMethods.CloseServiceHandle(intPtr2))
				{
					this.WriteError(TaskWin32Exception.FromErrorCodeAndVerbose(Marshal.GetLastWin32Error(), Strings.ErrorCloseServiceHandle), ErrorCategory.InvalidOperation, null, false);
				}
				if (IntPtr.Zero != intPtr && !NativeMethods.CloseServiceHandle(intPtr))
				{
					this.WriteError(TaskWin32Exception.FromErrorCodeAndVerbose(Marshal.GetLastWin32Error(), Strings.ErrorCloseServiceHandle), ErrorCategory.InvalidOperation, null, false);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected void UpdateExecutable(string serviceName, string executablePath)
		{
			TaskLogger.Trace("Updating executable...", new object[0]);
			this.DoNativeServiceTask(serviceName, ServiceAccessFlags.AllAccess, delegate(IntPtr service)
			{
				if (!NativeMethods.ChangeServiceConfig(service, 4294967295U, 4294967295U, 4294967295U, executablePath, null, null, null, null, null, null))
				{
					this.WriteError(TaskWin32Exception.FromErrorCodeAndVerbose(Marshal.GetLastWin32Error(), Strings.ErrorChangeServiceConfig2(serviceName)), ErrorCategory.WriteError, null);
				}
			});
		}

		internal delegate void NativeServiceTaskDelegate(IntPtr serviceHandle);
	}
}
