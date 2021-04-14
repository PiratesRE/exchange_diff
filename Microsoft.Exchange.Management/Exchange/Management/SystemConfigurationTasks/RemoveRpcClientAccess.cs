using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "RPCClientAccess", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveRpcClientAccess : RemoveSingletonSystemConfigurationObjectTask<ExchangeRpcClientAccess>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveExchangeRpcClientAccess(this.casServer.Name);
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.rootId == null)
				{
					this.casServer = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
					this.rootId = ExchangeRpcClientAccess.FromServerId(this.casServer.Id);
				}
				return this.rootId;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.DataObject == null)
			{
				base.WriteError(new LocalizedException(Strings.ClientAccessRoleAbsent(this.casServer.Name)), ErrorCategory.ObjectNotFound, this.casServer);
			}
			TaskLogger.LogExit();
		}

		private ObjectId rootId;

		private Server casServer;
	}
}
