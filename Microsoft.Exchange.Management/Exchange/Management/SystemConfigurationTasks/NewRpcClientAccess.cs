using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "RpcClientAccess", SupportsShouldProcess = true)]
	public sealed class NewRpcClientAccess : NewFixedNameSystemConfigurationObjectTask<ExchangeRpcClientAccess>
	{
		[Parameter(ValueFromPipeline = true, Mandatory = true)]
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

		[Parameter(Mandatory = false)]
		public bool EncryptionRequired
		{
			get
			{
				return this.DataObject.EncryptionRequired;
			}
			set
			{
				this.DataObject.EncryptionRequired = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaximumConnections
		{
			get
			{
				return this.DataObject.MaximumConnections;
			}
			set
			{
				this.DataObject.MaximumConnections = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BlockedClientVersions
		{
			get
			{
				return this.DataObject.BlockedClientVersions;
			}
			set
			{
				this.DataObject.BlockedClientVersions = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewRpcClientAccess(this.DataObject.Server.Name);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ExchangeRpcClientAccess exchangeRpcClientAccess = (ExchangeRpcClientAccess)base.PrepareDataObject();
			this.server = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, this.RootId, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
			if (ExchangeRpcClientAccess.CanCreateUnder(this.server))
			{
				exchangeRpcClientAccess.SetId(ExchangeRpcClientAccess.FromServerId(this.server.Id));
			}
			else
			{
				base.WriteError(new ServerRoleOperationException(Strings.ClientAccessRoleAbsent(this.server.Name)), ErrorCategory.OpenError, this.server);
			}
			TaskLogger.LogExit();
			return exchangeRpcClientAccess;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			ExchangeRpcClientAccess exchangeRpcClientAccess = (ExchangeRpcClientAccess)dataObject;
			exchangeRpcClientAccess.CompleteAllCalculatedProperties(this.server);
			base.WriteResult(dataObject);
			TaskLogger.LogExit();
		}

		internal static string SelectServerWithEqualProbability(List<string> servers)
		{
			Random random = new Random(Environment.TickCount);
			return servers[random.Next(0, servers.Count)];
		}

		private Server server;
	}
}
