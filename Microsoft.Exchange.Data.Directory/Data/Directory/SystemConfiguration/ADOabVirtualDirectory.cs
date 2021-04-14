using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public sealed class ADOabVirtualDirectory : ExchangeVirtualDirectory
	{
		private void AddOrRemoveAuthenticationMethod(List<AuthenticationMethod> authenticationMethods, bool authenticationMethodFlag, params AuthenticationMethod[] applicableAuthenticationMethods)
		{
			if (authenticationMethodFlag)
			{
				foreach (AuthenticationMethod item in applicableAuthenticationMethods)
				{
					if (!authenticationMethods.Contains(item))
					{
						authenticationMethods.Add(item);
					}
				}
				return;
			}
			foreach (AuthenticationMethod item2 in applicableAuthenticationMethods)
			{
				authenticationMethods.Remove(item2);
			}
		}

		private void UpdateInternalAndExternalAuthenticationMethods()
		{
			List<AuthenticationMethod> list = new List<AuthenticationMethod>();
			if (base.InternalAuthenticationMethods != null)
			{
				list.AddRange(base.InternalAuthenticationMethods);
			}
			List<AuthenticationMethod> authenticationMethods = list;
			bool basicAuthentication = this.BasicAuthentication;
			AuthenticationMethod[] applicableAuthenticationMethods = new AuthenticationMethod[1];
			this.AddOrRemoveAuthenticationMethod(authenticationMethods, basicAuthentication, applicableAuthenticationMethods);
			this.AddOrRemoveAuthenticationMethod(list, this.WindowsAuthentication, new AuthenticationMethod[]
			{
				AuthenticationMethod.WindowsIntegrated
			});
			this.AddOrRemoveAuthenticationMethod(list, this.OAuthAuthentication, new AuthenticationMethod[]
			{
				AuthenticationMethod.OAuth
			});
			MultiValuedProperty<AuthenticationMethod> multiValuedProperty = new MultiValuedProperty<AuthenticationMethod>(list);
			base.InternalAuthenticationMethods = multiValuedProperty;
			base.ExternalAuthenticationMethods = multiValuedProperty;
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADOabVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADOabVirtualDirectory.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			internal set
			{
				base.Name = value;
			}
		}

		[Parameter]
		public int PollInterval
		{
			get
			{
				return (int)this[ADOabVirtualDirectorySchema.PollInterval];
			}
			set
			{
				this[ADOabVirtualDirectorySchema.PollInterval] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> OfflineAddressBooks
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADOabVirtualDirectorySchema.OfflineAddressBooks];
			}
		}

		public bool RequireSSL
		{
			get
			{
				return (bool)this[ADOabVirtualDirectorySchema.RequireSSL];
			}
			internal set
			{
				this[ADOabVirtualDirectorySchema.RequireSSL] = value;
			}
		}

		public bool BasicAuthentication
		{
			get
			{
				return (bool)this[ADOabVirtualDirectorySchema.BasicAuthentication];
			}
			internal set
			{
				this[ADOabVirtualDirectorySchema.BasicAuthentication] = value;
				this.UpdateInternalAndExternalAuthenticationMethods();
			}
		}

		public bool WindowsAuthentication
		{
			get
			{
				return (bool)this[ADOabVirtualDirectorySchema.WindowsAuthentication];
			}
			internal set
			{
				this[ADOabVirtualDirectorySchema.WindowsAuthentication] = value;
				this.UpdateInternalAndExternalAuthenticationMethods();
			}
		}

		public bool OAuthAuthentication
		{
			get
			{
				return (bool)this[ADOabVirtualDirectorySchema.OAuthAuthentication];
			}
			internal set
			{
				this[ADOabVirtualDirectorySchema.OAuthAuthentication] = value;
				this.UpdateInternalAndExternalAuthenticationMethods();
			}
		}

		private static readonly ADOabVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADOabVirtualDirectorySchema>();

		public static readonly string MostDerivedClass = "msExchOabVirtualDirectory";
	}
}
