using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Assistants;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Stop", "ManagedFolderAssistant", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class StopElcAssistant : SystemConfigurationObjectActionTask<ServerIdParameter, Server>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageStopELCAssistant;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true)]
		public override ServerIdParameter Identity
		{
			get
			{
				return (ServerIdParameter)(base.Fields["Identity"] ?? new ServerIdParameter());
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return base.GlobalConfigSession;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ELCTaskHelper.VerifyIsInConfigScopes(this.DataObject, base.SessionSettings, new Task.TaskErrorLoggingDelegate(base.WriteError));
			string fqdn = this.DataObject.Fqdn;
			AssistantsRpcClient assistantsRpcClient = new AssistantsRpcClient(fqdn);
			int num = 3;
			try
			{
				IL_37:
				assistantsRpcClient.Stop("ElcAssistant");
			}
			catch (RpcException ex)
			{
				num--;
				if ((ex.ErrorCode == 1753 || ex.ErrorCode == 1727) && num > 0)
				{
					goto IL_37;
				}
				base.WriteError(new TaskException(RpcUtility.MapRpcErrorCodeToMessage(ex.ErrorCode, fqdn)), ErrorCategory.InvalidOperation, null);
				goto IL_37;
			}
			TaskLogger.LogExit();
		}
	}
}
