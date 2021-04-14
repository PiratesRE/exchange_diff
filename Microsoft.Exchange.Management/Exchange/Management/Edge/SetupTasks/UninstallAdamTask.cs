using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[LocDescription(Strings.IDs.UninstallAdamTask)]
	[Cmdlet("Uninstall", "Adam")]
	public sealed class UninstallAdamTask : Task
	{
		public UninstallAdamTask()
		{
			this.InstanceName = "MSExchange";
		}

		[Parameter(Mandatory = false)]
		public string InstanceName
		{
			get
			{
				return (string)base.Fields["InstanceName"];
			}
			set
			{
				base.Fields["InstanceName"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.InstanceName
			});
			try
			{
				ManageAdamService.UninstallAdam(this.InstanceName);
			}
			catch (AdamUninstallProcessFailureException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			catch (AdamUninstallGeneralFailureWithResultException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
			}
			catch (AdamUninstallErrorException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		public const string InstanceParamName = "InstanceName";
	}
}
