using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Enable", "OutlookAnywhere", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class EnableRpcHttp : NewExchangeVirtualDirectory<ADRpcHttpVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				bool flag = this.ExternalHostname != null && !string.IsNullOrEmpty(this.ExternalHostname.ToString());
				bool flag2 = this.InternalHostname != null && !string.IsNullOrEmpty(this.InternalHostname.ToString());
				if (flag && flag2)
				{
					return Strings.ConfirmationMessageEnableRpcHttpExternalAndInternal(this.WebSiteName.ToString(), base.Server.ToString(), this.ExternalHostname.ToString(), this.ExternalClientAuthenticationMethod.ToString(), this.InternalHostname.ToString(), this.InternalClientAuthenticationMethod.ToString(), this.IISAuthenticationMethods.ToString());
				}
				if (flag2)
				{
					return Strings.ConfirmationMessageEnableRpcHttpInternalOnly(this.WebSiteName.ToString(), base.Server.ToString(), this.InternalHostname.ToString(), this.InternalClientAuthenticationMethod.ToString(), this.IISAuthenticationMethods.ToString());
				}
				return Strings.ConfirmationMessageEnableRpcHttpExternalOnly(this.WebSiteName.ToString(), base.Server.ToString(), this.ExternalHostname.ToString(), this.ExternalClientAuthenticationMethod.ToString(), this.IISAuthenticationMethods.ToString());
			}
		}

		public EnableRpcHttp()
		{
			this.Name = "Rpc";
			this.WebSiteName = null;
		}

		private new string WebSiteName
		{
			get
			{
				return base.WebSiteName;
			}
			set
			{
				base.WebSiteName = value;
			}
		}

		private new string Path
		{
			get
			{
				return base.Path;
			}
			set
			{
				base.Path = value;
			}
		}

		private new string AppPoolId
		{
			get
			{
				return base.AppPoolId;
			}
			set
			{
				base.AppPoolId = value;
			}
		}

		private new string ApplicationRoot
		{
			get
			{
				return base.ApplicationRoot;
			}
			set
			{
				base.ApplicationRoot = value;
			}
		}

		private new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		private new Uri InternalUrl
		{
			get
			{
				return base.InternalUrl;
			}
			set
			{
				base.InternalUrl = value;
			}
		}

		private new Uri ExternalUrl
		{
			get
			{
				return base.ExternalUrl;
			}
			set
			{
				base.ExternalUrl = value;
			}
		}

		private new MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			get
			{
				return base.InternalAuthenticationMethods;
			}
			set
			{
				base.InternalAuthenticationMethods = value;
			}
		}

		private new MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
		{
			get
			{
				return base.ExternalAuthenticationMethods;
			}
			set
			{
				base.ExternalAuthenticationMethods = value;
			}
		}

		protected override bool FailOnVirtualDirectoryAlreadyExists()
		{
			return false;
		}

		protected override bool InternalShouldCreateMetabaseObject()
		{
			return false;
		}

		[Parameter(Mandatory = true)]
		public bool SSLOffloading
		{
			get
			{
				return this.DataObject.SSLOffloading;
			}
			set
			{
				this.DataObject.SSLOffloading = value;
			}
		}

		[Parameter]
		public Hostname ExternalHostname
		{
			get
			{
				return this.DataObject.ExternalHostname;
			}
			set
			{
				this.DataObject.ExternalHostname = value;
			}
		}

		[Parameter]
		public Hostname InternalHostname
		{
			get
			{
				return this.DataObject.InternalHostname;
			}
			set
			{
				this.DataObject.InternalHostname = value;
			}
		}

		[Parameter]
		public AuthenticationMethod DefaultAuthenticationMethod
		{
			get
			{
				return AuthenticationMethod.Misconfigured;
			}
			set
			{
				this.externalClientAuthenticationMethod = value;
				this.internalClientAuthenticationMethod = value;
				this.iisAuthenticationMethods = new MultiValuedProperty<AuthenticationMethod>();
				this.iisAuthenticationMethods.Add(value);
			}
		}

		[Parameter]
		public AuthenticationMethod ExternalClientAuthenticationMethod
		{
			get
			{
				return this.externalClientAuthenticationMethod;
			}
			set
			{
				this.externalClientAuthenticationMethod = value;
			}
		}

		[Parameter]
		public AuthenticationMethod InternalClientAuthenticationMethod
		{
			get
			{
				return this.internalClientAuthenticationMethod;
			}
			set
			{
				this.internalClientAuthenticationMethod = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<AuthenticationMethod> IISAuthenticationMethods
		{
			get
			{
				return this.iisAuthenticationMethods;
			}
			set
			{
				this.iisAuthenticationMethods = value;
			}
		}

		[Parameter]
		public Uri XropUrl
		{
			get
			{
				return this.DataObject.XropUrl;
			}
			set
			{
				this.DataObject.XropUrl = value;
			}
		}

		[Parameter]
		public bool ExternalClientsRequireSsl
		{
			get
			{
				return this.externalClientsRequireSsl ?? false;
			}
			set
			{
				this.externalClientsRequireSsl = new bool?(value);
			}
		}

		[Parameter]
		public bool InternalClientsRequireSsl
		{
			get
			{
				return this.internalClientsRequireSsl ?? false;
			}
			set
			{
				this.internalClientsRequireSsl = new bool?(value);
			}
		}

		private string StringFromList(List<Server> serverList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Server server in serverList)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(server.Fqdn);
			}
			return stringBuilder.ToString();
		}

		private bool IsInstalled()
		{
			ADRpcHttpVirtualDirectory[] array = (base.DataSession as IConfigurationSession).Find<ADRpcHttpVirtualDirectory>((ADObjectId)base.OwningServer.Identity, QueryScope.SubTree, null, null, 1);
			return array.Length > 0;
		}

		private void ValidateParameterValues()
		{
			bool flag = this.ExternalHostname != null && !string.IsNullOrEmpty(this.ExternalHostname.ToString());
			bool flag2 = this.InternalHostname != null && !string.IsNullOrEmpty(this.InternalHostname.ToString());
			if (!flag && !flag2)
			{
				base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyEitherInternalOrExternalHostName), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				return;
			}
			if (flag)
			{
				if (this.ExternalClientAuthenticationMethod == AuthenticationMethod.Misconfigured)
				{
					base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyExternalClientAuthenticationMethodOrDefaultAuthenticationMethod), ErrorCategory.InvalidArgument, this.DataObject.Identity);
					return;
				}
				if (this.externalClientsRequireSsl == null)
				{
					base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyExternalClientsRequireSslParameter), ErrorCategory.InvalidArgument, this.DataObject.Identity);
					return;
				}
				if (this.ExternalClientAuthenticationMethod == AuthenticationMethod.Basic && !this.ExternalClientsRequireSsl)
				{
					base.WriteError(new ArgumentException(Strings.ErrorRpcHttpBasicAuthOverHttpDisallowed), ErrorCategory.InvalidArgument, this.DataObject.Identity);
					return;
				}
				this.DataObject.ExternalClientAuthenticationMethod = this.ExternalClientAuthenticationMethod;
				this.DataObject.ExternalClientsRequireSsl = this.ExternalClientsRequireSsl;
			}
			else
			{
				this.DataObject.ExternalClientAuthenticationMethod = AuthenticationMethod.Negotiate;
			}
			if (flag2)
			{
				if (this.InternalClientAuthenticationMethod == AuthenticationMethod.Misconfigured)
				{
					base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyInternalClientAuthenticationMethodOrDefaultAuthenticationMethod), ErrorCategory.InvalidArgument, this.DataObject.Identity);
					return;
				}
				if (this.internalClientsRequireSsl == null)
				{
					base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyInternalClientsRequireSslParameter), ErrorCategory.InvalidArgument, this.DataObject.Identity);
					return;
				}
				if (this.InternalClientAuthenticationMethod == AuthenticationMethod.Basic && !this.InternalClientsRequireSsl)
				{
					base.WriteError(new ArgumentException(Strings.ErrorRpcHttpBasicAuthOverHttpDisallowed), ErrorCategory.InvalidArgument, this.DataObject.Identity);
					return;
				}
				this.DataObject.InternalClientAuthenticationMethod = this.InternalClientAuthenticationMethod;
				this.DataObject.InternalClientsRequireSsl = this.InternalClientsRequireSsl;
			}
			else
			{
				this.DataObject.InternalClientAuthenticationMethod = AuthenticationMethod.Negotiate;
			}
			if (this.IISAuthenticationMethods == null)
			{
				base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyEitherIISAuthenticationMethodsOrDefaultAuthenticationMethod), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				return;
			}
			if (!this.SSLOffloading && ((flag && !this.ExternalClientsRequireSsl) || (flag2 && !this.InternalClientsRequireSsl)))
			{
				base.WriteError(new ArgumentException(Strings.ErrorRpcHttpSSLOffloadingDisabled), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			this.DataObject.IISAuthenticationMethods = this.iisAuthenticationMethods;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.ValidateParameterValues();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			try
			{
				if (this.IsInstalled())
				{
					base.WriteError(new ArgumentException(Strings.ErrorRpcHttpAlreadyEnabled(base.OwningServer.Fqdn), string.Empty), ErrorCategory.InvalidArgument, this.DataObject.Identity);
					return;
				}
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, Microsoft.Exchange.Data.Directory.SystemConfiguration.Server.E2007MinVersion);
				IEnumerable<Server> enumerable = (base.DataSession as IConfigurationSession).FindPaged<Server>(null, QueryScope.SubTree, filter, null, 0);
				List<Server> list = new List<Server>();
				List<Server> list2 = new List<Server>();
				List<Server> list3 = new List<Server>();
				foreach (Server server in enumerable)
				{
					if (!server.IsExchange2003OrLater)
					{
						list.Add(server);
					}
					else if (!server.IsExchange2003Sp1OrLater)
					{
						list2.Add(server);
					}
					else if (!server.IsPreE12FrontEnd && !server.IsPreE12RPCHTTPEnabled)
					{
						list3.Add(server);
					}
				}
				if (list.Count > 0)
				{
					string servers = this.StringFromList(list);
					this.WriteWarning(Strings.RpcHttpE2kServers(servers));
				}
				if (list2.Count > 0)
				{
					string servers2 = this.StringFromList(list2);
					this.WriteWarning(Strings.RpcHttpE2k3Servers(servers2));
				}
				if (list3.Count > 0)
				{
					string servers3 = this.StringFromList(list3);
					this.WriteWarning(Strings.RpcHttpTiSp1FeatureDisabled(servers3));
				}
				if (list2.Count > 0 || list3.Count > 0)
				{
					string text = this.StringFromList(list2);
					string text2 = this.StringFromList(list3);
					if (text.Length > 0 && text2.Length > 0)
					{
						text += ", ";
					}
					text += text2;
					this.WriteWarning(Strings.RpcHttpOldOSServers(text));
				}
				this.WriteWarning(Strings.RpcHttpAvailability(this.DataObject.ServerName));
			}
			catch (ADTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, this.DataObject.Identity);
			}
			catch (DataValidationException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidData, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		private const string VirtualDirectoryName = "Rpc";

		private AuthenticationMethod externalClientAuthenticationMethod = AuthenticationMethod.Misconfigured;

		private AuthenticationMethod internalClientAuthenticationMethod = AuthenticationMethod.Misconfigured;

		private MultiValuedProperty<AuthenticationMethod> iisAuthenticationMethods;

		private bool? externalClientsRequireSsl;

		private bool? internalClientsRequireSsl;
	}
}
