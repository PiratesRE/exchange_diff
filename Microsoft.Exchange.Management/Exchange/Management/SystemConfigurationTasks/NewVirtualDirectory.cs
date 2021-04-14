using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class NewVirtualDirectory<T> : NewFixedNameSystemConfigurationObjectTask<T> where T : ADVirtualDirectory, new()
	{
		protected Server OwningServer
		{
			get
			{
				return this.owningServer;
			}
		}

		[Parameter(ValueFromPipeline = true)]
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

		[Parameter(Mandatory = false, Position = 0)]
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

		[Parameter]
		public Uri InternalUrl
		{
			get
			{
				T dataObject = this.DataObject;
				return dataObject.InternalUrl;
			}
			set
			{
				T dataObject = this.DataObject;
				dataObject.InternalUrl = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			get
			{
				T dataObject = this.DataObject;
				return dataObject.InternalAuthenticationMethods;
			}
			set
			{
				T dataObject = this.DataObject;
				dataObject.InternalAuthenticationMethods = value;
			}
		}

		[Parameter]
		public Uri ExternalUrl
		{
			get
			{
				T dataObject = this.DataObject;
				return dataObject.ExternalUrl;
			}
			set
			{
				T dataObject = this.DataObject;
				dataObject.ExternalUrl = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
		{
			get
			{
				T dataObject = this.DataObject;
				return dataObject.ExternalAuthenticationMethods;
			}
			set
			{
				T dataObject = this.DataObject;
				dataObject.ExternalAuthenticationMethods = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADVirtualDirectory advirtualDirectory = (ADVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (!this.ShouldCreateVirtualDirectory())
			{
				base.WriteError(this.owningServer.GetServerRoleError(ServerRole.Mailbox | ServerRole.ClientAccess | ServerRole.UnifiedMessaging | ServerRole.HubTransport), ErrorCategory.InvalidOperation, this.Server);
				return null;
			}
			advirtualDirectory.SetId(this.owningServer.Id.GetChildId("Protocols").GetChildId("HTTP").GetChildId(this.Name));
			return advirtualDirectory;
		}

		protected virtual bool ShouldCreateVirtualDirectory()
		{
			if (this.Server == null)
			{
				this.Server = new ServerIdParameter();
			}
			this.owningServer = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, this.RootId, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
			return !base.HasErrors && (this.OwningServer.IsClientAccessServer || this.OwningServer.IsCafeServer);
		}

		private Server owningServer;
	}
}
