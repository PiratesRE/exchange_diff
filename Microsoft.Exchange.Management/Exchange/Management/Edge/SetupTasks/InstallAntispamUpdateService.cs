using System;
using System.Management.Automation;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[Cmdlet("Install", "AntispamUpdateService")]
	[LocDescription(Strings.IDs.InstallAntispamUpdateServiceTask)]
	public class InstallAntispamUpdateService : ManageAntispamUpdateService
	{
		[Parameter(Mandatory = false)]
		public string[] ServicesDependedOnParameter
		{
			get
			{
				return base.Fields["ServicesDependedOn"] as string[];
			}
			set
			{
				base.Fields["ServicesDependedOn"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool StartAutomatically
		{
			internal get
			{
				return base.StartMode == ServiceStartMode.Automatic;
			}
			set
			{
				base.StartMode = (value ? ServiceStartMode.Automatic : ServiceStartMode.Manual);
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.ServicesDependedOnParameter != null)
			{
				foreach (string serviceName in this.ServicesDependedOnParameter)
				{
					if (!Utils.GetServiceExists(serviceName))
					{
						base.WriteError(new ArgumentException(Strings.InvalidServicesDependedOn(serviceName), "ServicesDependedOn"), ErrorCategory.InvalidArgument, null);
					}
				}
			}
			base.ServicesDependedOn = this.ServicesDependedOnParameter;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}

		public const string ServicesDependedOnParamName = "ServicesDependedOn";
	}
}
