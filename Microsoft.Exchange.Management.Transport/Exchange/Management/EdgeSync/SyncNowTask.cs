using System;
using System.ComponentModel;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.EdgeSync;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[Cmdlet("Start", "EdgeSynchronization", SupportsShouldProcess = true)]
	public sealed class SyncNowTask : DataAccessTask<Server>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageStartEdgeSynchronization;
			}
		}

		[Parameter(Mandatory = false)]
		public ServerIdParameter Server
		{
			get
			{
				return this.serverId;
			}
			set
			{
				this.serverId = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TargetServer
		{
			get
			{
				return this.targetServer;
			}
			set
			{
				this.targetServer = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ForceFullSync
		{
			get
			{
				return this.forceFullSync;
			}
			set
			{
				this.forceFullSync = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ForceUpdateCookie
		{
			get
			{
				return this.forceUpdateCookie;
			}
			set
			{
				this.forceUpdateCookie = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 127, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\SyncNowTask.cs");
		}

		protected override void InternalValidate()
		{
			if (this.serverId == null)
			{
				this.serverId = new ServerIdParameter();
			}
			this.server = (Server)base.GetDataObject<Server>(this.serverId, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound((string)this.serverId)), new LocalizedString?(Strings.ErrorServerNotUnique((string)this.serverId)));
			if (!this.server.IsHubTransportServer)
			{
				base.WriteError(new InvalidOperationException(Strings.SynNowCanOnlyRunAgainstHub), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				this.client = new EdgeSyncRpcClient(this.server.Name);
				StartResults startResults = this.TryStartSyncNow();
				if (startResults == StartResults.Started)
				{
					for (;;)
					{
						GetResultResults getResultResults = GetResultResults.NoMoreData;
						byte[] syncNowResult = this.client.GetSyncNowResult(ref getResultResults);
						if (getResultResults == GetResultResults.NoMoreData)
						{
							break;
						}
						if (getResultResults == GetResultResults.Error)
						{
							base.WriteError(new SyncNowFailedToRunException(), ErrorCategory.InvalidOperation, null);
						}
						else if (syncNowResult == null)
						{
							Thread.Sleep(1000);
						}
						else
						{
							Status sendToPipeline = Status.Deserialize(syncNowResult);
							base.WriteObject(sendToPipeline);
						}
					}
				}
				else if (startResults == StartResults.AlreadyStarted)
				{
					base.WriteError(new SyncNowAlreadyStartedException(), ErrorCategory.InvalidOperation, null);
				}
				else
				{
					base.WriteError(new SyncNowFailedToRunException(), ErrorCategory.InvalidOperation, null);
				}
			}
			catch (RpcException ex)
			{
				this.WriteTranslatedError(ex);
			}
		}

		private StartResults TryStartSyncNow()
		{
			int num = 3;
			while (num-- > 0)
			{
				try
				{
					return this.client.StartSyncNow(this.targetServer, this.forceFullSync, this.forceUpdateCookie);
				}
				catch (RpcException ex)
				{
					RpcError errorCode = (RpcError)ex.ErrorCode;
					if ((errorCode != RpcError.EndpointNotRegistered && errorCode != RpcError.RemoteDidNotExecute) || num == 0)
					{
						throw;
					}
				}
			}
			return StartResults.ErrorOnStart;
		}

		private void WriteTranslatedError(RpcException ex)
		{
			RpcError errorCode = (RpcError)ex.ErrorCode;
			if (errorCode == RpcError.ServerUnavailable)
			{
				base.WriteError(new ServerUnavailableException(), ErrorCategory.ReadError, null);
				return;
			}
			if (errorCode != RpcError.EndpointNotRegistered)
			{
				base.WriteError(new Win32Exception(ex.ErrorCode), ErrorCategory.NotSpecified, null);
				return;
			}
			base.WriteError(new EndPointNotRegisteredException(), ErrorCategory.ReadError, null);
		}

		private EdgeSyncRpcClient client;

		private ServerIdParameter serverId;

		private Server server;

		private string targetServer = string.Empty;

		private SwitchParameter forceFullSync;

		private SwitchParameter forceUpdateCookie;
	}
}
