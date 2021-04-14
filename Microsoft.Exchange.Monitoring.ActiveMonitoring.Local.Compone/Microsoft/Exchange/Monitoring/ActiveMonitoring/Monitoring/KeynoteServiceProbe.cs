using System;
using System.ComponentModel;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Monitoring.Discovery;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Monitoring
{
	public class KeynoteServiceProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition()
		{
			return new ProbeDefinition
			{
				TypeName = typeof(KeynoteServiceProbe).FullName,
				Name = "KeynoteServiceProbe",
				ServiceName = ExchangeComponent.Monitoring.Name,
				RecurrenceIntervalSeconds = KeynoteServiceProbe.ProbeRecurrenceSeconds,
				TimeoutSeconds = 30,
				Enabled = true,
				WorkItemVersion = KeynoteServiceDiscovery.AssemblyVersion,
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				ExecutionLocation = "EXO",
				TargetResource = "Microsoft.Exchange.Monitoring.KeynoteDataReader",
				Account = string.Empty,
				AccountPassword = string.Empty,
				AccountDisplayName = string.Empty,
				Endpoint = string.Empty
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			string targetResource = base.Definition.TargetResource;
			string text = null;
			using (ServiceController serviceController = new ServiceController(targetResource))
			{
				try
				{
					ServiceControllerStatus status = serviceController.Status;
					if (status.Equals(ServiceControllerStatus.Running))
					{
						base.Result.StateAttribute1 = status.ToString();
						return;
					}
				}
				catch (Exception ex)
				{
					text = string.Format("{0} : {1} ", ex.Message, ex.StackTrace);
					InvalidOperationException ex2 = ex as InvalidOperationException;
					if (ex2 != null)
					{
						Win32Exception ex3 = ex2.InnerException as Win32Exception;
						if (ex3 != null && ex3.NativeErrorCode == 1060)
						{
							return;
						}
					}
				}
			}
			base.Result.StateAttribute1 = text;
			throw new Exception(string.Format("Service {0} on server {1} is not in running state.  Current status is {2}", targetResource, Environment.MachineName, text));
		}

		private const string Name = "KeynoteServiceProbe";

		private const int ServiceDoesNotExistError = 1060;

		private static int ProbeRecurrenceSeconds = 600;
	}
}
