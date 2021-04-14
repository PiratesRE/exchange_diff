using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "ServerMonitor", SupportsShouldProcess = true)]
	public sealed class SetServerMonitor : Task
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetServerMonitor(this.Name, this.TargetResource ?? string.Empty, this.Repairing);
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public string TargetResource
		{
			get
			{
				return this.targetResource;
			}
			set
			{
				this.targetResource = value;
			}
		}

		[Parameter(Mandatory = true)]
		public bool Repairing
		{
			get
			{
				return this.isRepairing;
			}
			set
			{
				this.isRepairing = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				LocalizedException ex = null;
				try
				{
					RpcSetServerMonitor.Invoke((!string.IsNullOrWhiteSpace(this.Server.Fqdn)) ? this.Server.Fqdn : this.Server.ToString(), this.Name, this.TargetResource, new bool?(this.isRepairing), 30000);
				}
				catch (ActiveMonitoringServerException ex2)
				{
					ex = ex2;
				}
				catch (ActiveMonitoringServerTransientException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					base.WriteError(ex, ExchangeErrorCategory.ServerOperation, null);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private bool isRepairing;

		private string targetResource = string.Empty;
	}
}
