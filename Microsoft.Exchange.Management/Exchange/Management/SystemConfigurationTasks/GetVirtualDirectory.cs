using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class GetVirtualDirectory<T> : GetSystemConfigurationObjectTask<VirtualDirectoryIdParameter, T> where T : ADVirtualDirectory, new()
	{
		[Parameter(Mandatory = true, ParameterSetName = "Server", ValueFromPipeline = true)]
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
				return this.rootId;
			}
		}

		protected override void InternalValidate()
		{
			if (this.Server != null)
			{
				Server server = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
				if (base.HasErrors)
				{
					return;
				}
				if (!server.IsClientAccessServer && !server.IsMailboxServer && !server.IsHubTransportServer && !server.IsUnifiedMessagingServer && !server.IsFrontendTransportServer && !server.IsFfoWebServiceRole && !server.IsCafeServer && !server.IsOSPRole)
				{
					base.WriteError(server.GetServerRoleError(ServerRole.Mailbox | ServerRole.ClientAccess | ServerRole.UnifiedMessaging | ServerRole.HubTransport | ServerRole.FrontendTransport | ServerRole.FfoWebService | ServerRole.OSP), ErrorCategory.InvalidOperation, this.Server);
					return;
				}
				this.rootId = (ADObjectId)server.Identity;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			try
			{
				if (ServerIdParameter.HasRole((ADObjectId)dataObject.Identity, ServerRole.Cafe | ServerRole.Mailbox | ServerRole.ClientAccess | ServerRole.UnifiedMessaging | ServerRole.HubTransport | ServerRole.FrontendTransport | ServerRole.FfoWebService | ServerRole.OSP, base.DataSession))
				{
					ADVirtualDirectory advirtualDirectory = dataObject as ADVirtualDirectory;
					if (advirtualDirectory != null)
					{
						advirtualDirectory.AdminDisplayVersion = Microsoft.Exchange.Data.Directory.SystemConfiguration.Server.GetServerVersion(advirtualDirectory.Server.Name);
					}
					base.WriteResult(dataObject);
				}
			}
			catch (InvalidOperationException)
			{
				base.WriteError(new InvalidADObjectOperationException(Strings.ErrorFoundInvalidADObject(((ADObjectId)dataObject.Identity).ToDNString())), ErrorCategory.InvalidOperation, this.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		private ADObjectId rootId;
	}
}
