using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "OutlookAnywhere", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetRpcHttp : SetExchangeVirtualDirectory<ADRpcHttpVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetRpcHttp(this.Identity.ToString());
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

		[Parameter]
		public bool SSLOffloading
		{
			get
			{
				return (bool)base.Fields["SSLOffloading"];
			}
			set
			{
				base.Fields["SSLOffloading"] = value;
			}
		}

		[Parameter]
		public bool ExternalClientsRequireSsl
		{
			get
			{
				return (bool)base.Fields["ExternalClientsRequireSsl"];
			}
			set
			{
				base.Fields["ExternalClientsRequireSsl"] = value;
			}
		}

		[Parameter]
		public bool InternalClientsRequireSsl
		{
			get
			{
				return (bool)base.Fields["InternalClientsRequireSsl"];
			}
			set
			{
				base.Fields["InternalClientsRequireSsl"] = value;
			}
		}

		[Parameter]
		public string ExternalHostname
		{
			get
			{
				return (string)base.Fields["ExternalHostname"];
			}
			set
			{
				base.Fields["ExternalHostname"] = value;
			}
		}

		[Parameter]
		public string InternalHostname
		{
			get
			{
				return (string)base.Fields["InternalHostname"];
			}
			set
			{
				base.Fields["InternalHostname"] = value;
			}
		}

		[Parameter]
		public AuthenticationMethod DefaultAuthenticationMethod
		{
			get
			{
				return (AuthenticationMethod)base.Fields["DefaultAuthenticationMethod"];
			}
			set
			{
				base.Fields["DefaultAuthenticationMethod"] = value;
			}
		}

		[Parameter]
		public AuthenticationMethod ExternalClientAuthenticationMethod
		{
			get
			{
				return (AuthenticationMethod)base.Fields["ExternalClientAuthenticationMethod"];
			}
			set
			{
				base.Fields["ExternalClientAuthenticationMethod"] = value;
			}
		}

		[Parameter]
		public AuthenticationMethod InternalClientAuthenticationMethod
		{
			get
			{
				return (AuthenticationMethod)base.Fields["InternalClientAuthenticationMethod"];
			}
			set
			{
				base.Fields["InternalClientAuthenticationMethod"] = value;
			}
		}

		[Parameter]
		public Uri XropUrl
		{
			get
			{
				return (Uri)base.Fields["XropUrl"];
			}
			set
			{
				base.Fields["XropUrl"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<AuthenticationMethod> IISAuthenticationMethods
		{
			get
			{
				return (MultiValuedProperty<AuthenticationMethod>)base.Fields["IISAuthenticationMethods"];
			}
			set
			{
				base.Fields["IISAuthenticationMethods"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADRpcHttpVirtualDirectory adrpcHttpVirtualDirectory = (ADRpcHttpVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (base.Fields.Contains("ExternalHostname"))
			{
				if (!string.IsNullOrEmpty(this.ExternalHostname))
				{
					if (!base.Fields.Contains("ExternalClientsRequireSsl"))
					{
						base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyExternalClientsRequireSslParameter), ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
					}
					if (!base.Fields.Contains("DefaultAuthenticationMethod") && !base.Fields.Contains("ExternalClientAuthenticationMethod"))
					{
						base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyExternalClientAuthenticationParameter), ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
					}
					try
					{
						adrpcHttpVirtualDirectory.ExternalHostname = new Hostname(this.ExternalHostname);
						goto IL_D4;
					}
					catch (ArgumentException exception)
					{
						base.WriteError(exception, ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
						goto IL_D4;
					}
				}
				adrpcHttpVirtualDirectory.ExternalHostname = null;
			}
			IL_D4:
			if (base.Fields.Contains("InternalHostname"))
			{
				if (!string.IsNullOrEmpty(this.InternalHostname))
				{
					if (!base.Fields.Contains("InternalClientsRequireSsl"))
					{
						base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyInternalClientsRequireSslParameter), ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
					}
					try
					{
						adrpcHttpVirtualDirectory.InternalHostname = new Hostname(this.InternalHostname);
						goto IL_14C;
					}
					catch (ArgumentException exception2)
					{
						base.WriteError(exception2, ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
						goto IL_14C;
					}
				}
				adrpcHttpVirtualDirectory.InternalHostname = null;
			}
			IL_14C:
			if (base.Fields["ExternalClientsRequireSsl"] != null)
			{
				adrpcHttpVirtualDirectory.ExternalClientsRequireSsl = this.ExternalClientsRequireSsl;
			}
			if (base.Fields["InternalClientsRequireSsl"] != null)
			{
				adrpcHttpVirtualDirectory.InternalClientsRequireSsl = this.InternalClientsRequireSsl;
			}
			if (base.Fields.Contains("SSLOffloading"))
			{
				adrpcHttpVirtualDirectory.SSLOffloading = this.SSLOffloading;
			}
			if (base.Fields.Contains("XropUrl"))
			{
				adrpcHttpVirtualDirectory.XropUrl = this.XropUrl;
			}
			if (base.Fields.Contains("DefaultAuthenticationMethod"))
			{
				if (base.Fields.Contains("ExternalClientAuthenticationMethod") || base.Fields.Contains("InternalClientAuthenticationMethod") || base.Fields.Contains("IISAuthenticationMethods"))
				{
					base.WriteError(new ArgumentException(Strings.ErrorRpcHttpInvalidSwitchCombo), ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
				}
				if (this.DefaultAuthenticationMethod == AuthenticationMethod.Negotiate)
				{
					this.WriteWarning(Strings.WarnRpcHttpNegotiateCoexistence);
				}
				adrpcHttpVirtualDirectory.ExternalClientAuthenticationMethod = this.DefaultAuthenticationMethod;
				adrpcHttpVirtualDirectory.InternalClientAuthenticationMethod = this.DefaultAuthenticationMethod;
				adrpcHttpVirtualDirectory.IISAuthenticationMethods = new MultiValuedProperty<AuthenticationMethod>();
				adrpcHttpVirtualDirectory.IISAuthenticationMethods.Add(this.DefaultAuthenticationMethod);
			}
			else
			{
				if ((base.Fields.Contains("ExternalClientAuthenticationMethod") && this.ExternalClientAuthenticationMethod == AuthenticationMethod.Negotiate) || (base.Fields.Contains("InternalClientAuthenticationMethod") && this.InternalClientAuthenticationMethod == AuthenticationMethod.Negotiate))
				{
					this.WriteWarning(Strings.WarnRpcHttpNegotiateCoexistence);
				}
				if (base.Fields.Contains("ExternalClientAuthenticationMethod"))
				{
					adrpcHttpVirtualDirectory.ExternalClientAuthenticationMethod = this.ExternalClientAuthenticationMethod;
				}
				if (base.Fields.Contains("InternalClientAuthenticationMethod"))
				{
					adrpcHttpVirtualDirectory.InternalClientAuthenticationMethod = this.InternalClientAuthenticationMethod;
				}
				if (base.Fields.Contains("IISAuthenticationMethods"))
				{
					adrpcHttpVirtualDirectory.IISAuthenticationMethods = this.IISAuthenticationMethods;
				}
			}
			bool flag = adrpcHttpVirtualDirectory.ExternalHostname != null && !string.IsNullOrEmpty(adrpcHttpVirtualDirectory.ExternalHostname.ToString());
			bool flag2 = adrpcHttpVirtualDirectory.InternalHostname != null && !string.IsNullOrEmpty(adrpcHttpVirtualDirectory.InternalHostname.ToString());
			if (!flag && !flag2)
			{
				base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyEitherInternalOrExternalHostName), ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
			}
			if (flag && adrpcHttpVirtualDirectory.ExternalClientAuthenticationMethod == AuthenticationMethod.Misconfigured)
			{
				base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyExternalClientAuthenticationMethodOrDefaultAuthenticationMethod), ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
			}
			if (flag && adrpcHttpVirtualDirectory.ExternalClientAuthenticationMethod == AuthenticationMethod.Basic && !adrpcHttpVirtualDirectory.ExternalClientsRequireSsl)
			{
				base.WriteError(new ArgumentException(Strings.ErrorRpcHttpBasicAuthOverHttpDisallowed), ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
			}
			if (flag2 && adrpcHttpVirtualDirectory.InternalClientAuthenticationMethod == AuthenticationMethod.Misconfigured)
			{
				base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyInternalClientAuthenticationMethodOrDefaultAuthenticationMethod), ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
			}
			if (flag2 && adrpcHttpVirtualDirectory.InternalClientAuthenticationMethod == AuthenticationMethod.Basic && !adrpcHttpVirtualDirectory.InternalClientsRequireSsl)
			{
				base.WriteError(new ArgumentException(Strings.ErrorRpcHttpBasicAuthOverHttpDisallowed), ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
			}
			if (!adrpcHttpVirtualDirectory.SSLOffloading && ((flag && !adrpcHttpVirtualDirectory.ExternalClientsRequireSsl) || (flag2 && !adrpcHttpVirtualDirectory.InternalClientsRequireSsl)))
			{
				base.WriteError(new ArgumentException(Strings.ErrorRpcHttpSSLOffloadingDisabled), ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
			}
			if (adrpcHttpVirtualDirectory.IISAuthenticationMethods == null)
			{
				base.WriteError(new ArgumentException(Strings.ErrorRpcHttpMustSpecifyEitherIISAuthenticationMethodsOrDefaultAuthenticationMethod), ErrorCategory.InvalidArgument, adrpcHttpVirtualDirectory.Identity);
			}
			return adrpcHttpVirtualDirectory;
		}

		private const string SSLOffloadingKey = "SSLOffloading";

		private const string ExternalHostnameKey = "ExternalHostname";

		private const string InternalHostnameKey = "InternalHostname";

		private const string ExternalClientAuthenticationMethodKey = "ExternalClientAuthenticationMethod";

		private const string InternalClientAuthenticationMethodKey = "InternalClientAuthenticationMethod";

		private const string ExternalClientsRequireSslKey = "ExternalClientsRequireSsl";

		private const string InternalClientsRequireSslKey = "InternalClientsRequireSsl";

		private const string IISAuthenticationMethodsKey = "IISAuthenticationMethods";

		private const string DefaultAuthenticationMethodKey = "DefaultAuthenticationMethod";

		private const string XropUrlKey = "XropUrl";
	}
}
