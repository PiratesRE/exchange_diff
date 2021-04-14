using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "RpcClientAccess", SupportsShouldProcess = true, DefaultParameterSetName = "Server")]
	public sealed class SetRpcClientAccess : SetSingletonSystemConfigurationObjectTask<ExchangeRpcClientAccess>
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

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetExchangeRpcClientAccessServer(this.Server.ToString());
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.Server != null)
				{
					Server server = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
					return ExchangeRpcClientAccess.FromServerId(server.Id);
				}
				return null;
			}
		}
	}
}
