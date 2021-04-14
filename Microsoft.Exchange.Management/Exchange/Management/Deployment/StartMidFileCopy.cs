using System;
using System.Diagnostics;
using System.Management.Automation;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Start", "MidFileCopy", SupportsShouldProcess = true)]
	public sealed class StartMidFileCopy : ManageSetupBindingTasks
	{
		public static bool StartProcessHidden(string fileName, string arguments, int millisecondsTimeout)
		{
			Process process = Process.Start(new ProcessStartInfo(fileName, arguments)
			{
				WindowStyle = ProcessWindowStyle.Hidden
			});
			process.Start();
			process.WaitForExit();
			if (!process.WaitForExit(millisecondsTimeout))
			{
				process.Kill();
				return false;
			}
			return true;
		}

		public StartMidFileCopy()
		{
			ServiceController serviceController = new ServiceController("WinMgmt");
			if (serviceController.Status != ServiceControllerStatus.Running)
			{
				try
				{
					serviceController.Start();
				}
				catch
				{
					StartMidFileCopy.StartProcessHidden("sc.exe", "config WinMgmt start= auto", 1000);
					int num = 10;
					do
					{
						serviceController.Start();
						Thread.Sleep(100);
						serviceController.Refresh();
					}
					while (serviceController.Status != ServiceControllerStatus.Running && num-- > 0);
				}
			}
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.StartMidFileCopyDescription;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (UninstallMsi.RebootRequiredException != null)
			{
				throw UninstallMsi.RebootRequiredException;
			}
			TaskLogger.LogExit();
		}
	}
}
