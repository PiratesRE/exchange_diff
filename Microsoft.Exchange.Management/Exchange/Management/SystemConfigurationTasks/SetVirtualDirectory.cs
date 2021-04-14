using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class SetVirtualDirectory<T> : SetTopologySystemConfigurationObjectTask<VirtualDirectoryIdParameter, T> where T : ADVirtualDirectory, new()
	{
		[Parameter]
		public Uri InternalUrl
		{
			get
			{
				return (Uri)base.Fields[SetVirtualDirectory<T>.InternalUrlKey];
			}
			set
			{
				base.Fields[SetVirtualDirectory<T>.InternalUrlKey] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			get
			{
				return (MultiValuedProperty<AuthenticationMethod>)base.Fields[SetVirtualDirectory<T>.InternalAuthenticationMethodsKey];
			}
			set
			{
				base.Fields[SetVirtualDirectory<T>.InternalAuthenticationMethodsKey] = value;
			}
		}

		[Parameter]
		public Uri ExternalUrl
		{
			get
			{
				return (Uri)base.Fields[SetVirtualDirectory<T>.ExternalUrlKey];
			}
			set
			{
				base.Fields[SetVirtualDirectory<T>.ExternalUrlKey] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
		{
			get
			{
				return (MultiValuedProperty<AuthenticationMethod>)base.Fields[SetVirtualDirectory<T>.ExternalAuthenticationMethodsKey];
			}
			set
			{
				base.Fields[SetVirtualDirectory<T>.ExternalAuthenticationMethodsKey] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADVirtualDirectory advirtualDirectory = (ADVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (base.Fields.Contains(SetVirtualDirectory<T>.InternalUrlKey))
			{
				advirtualDirectory.InternalUrl = (Uri)base.Fields[SetVirtualDirectory<T>.InternalUrlKey];
			}
			if (base.Fields.Contains(SetVirtualDirectory<T>.InternalAuthenticationMethodsKey))
			{
				advirtualDirectory.InternalAuthenticationMethods = (MultiValuedProperty<AuthenticationMethod>)base.Fields[SetVirtualDirectory<T>.InternalAuthenticationMethodsKey];
			}
			if (base.Fields.Contains(SetVirtualDirectory<T>.ExternalUrlKey))
			{
				advirtualDirectory.ExternalUrl = (Uri)base.Fields[SetVirtualDirectory<T>.ExternalUrlKey];
			}
			if (base.Fields.Contains(SetVirtualDirectory<T>.ExternalAuthenticationMethodsKey))
			{
				advirtualDirectory.ExternalAuthenticationMethods = (MultiValuedProperty<AuthenticationMethod>)base.Fields[SetVirtualDirectory<T>.ExternalAuthenticationMethodsKey];
			}
			return advirtualDirectory;
		}

		protected static readonly string InternalUrlKey = "InternalUrl";

		protected static readonly string InternalAuthenticationMethodsKey = "InternalAuthenticationMethods";

		protected static readonly string ExternalUrlKey = "ExternalUrl";

		protected static readonly string ExternalAuthenticationMethodsKey = "ExternalAuthenticationMethods";
	}
}
