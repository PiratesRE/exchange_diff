using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "RpcClientAccess")]
	public sealed class GetRpcClientAccess : GetSingletonSystemConfigurationObjectTask<ExchangeRpcClientAccess>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

		protected override ObjectId RootId
		{
			get
			{
				if (this.casServer == null)
				{
					return null;
				}
				return ExchangeRpcClientAccess.FromServerId(this.casServer.Id);
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.Server != null)
			{
				this.casServer = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
				return;
			}
			this.casServer = null;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			ExchangeRpcClientAccess exchangeRpcClientAccess = (ExchangeRpcClientAccess)dataObject;
			exchangeRpcClientAccess.CompleteAllCalculatedProperties(this.casServer ?? this.ConfigurationSession.Read<Server>(exchangeRpcClientAccess.Server));
			base.WriteResult(dataObject);
			TaskLogger.LogExit();
		}

		private Server casServer;
	}
}
