using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public abstract class ADExchangeServiceVirtualDirectory : ExchangeVirtualDirectory
	{
		protected static void AddOrRemoveAuthenticationMethod(List<AuthenticationMethod> authenticationMethods, bool? authenticationMethodFlag, params AuthenticationMethod[] applicableAuthenticationMethods)
		{
			if (authenticationMethodFlag != null)
			{
				if (authenticationMethodFlag.Value)
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
		}

		private void UpdateInternalAndExternalAuthenticationMethods()
		{
			List<AuthenticationMethod> list = new List<AuthenticationMethod>();
			if (base.InternalAuthenticationMethods != null)
			{
				list.AddRange(base.InternalAuthenticationMethods);
			}
			ADExchangeServiceVirtualDirectory.AddOrRemoveAuthenticationMethod(list, this.LiveIdNegotiateAuthentication, new AuthenticationMethod[]
			{
				AuthenticationMethod.LiveIdNegotiate
			});
			ADExchangeServiceVirtualDirectory.AddOrRemoveAuthenticationMethod(list, this.AdfsAuthentication, new AuthenticationMethod[]
			{
				AuthenticationMethod.Adfs
			});
			ADExchangeServiceVirtualDirectory.AddOrRemoveAuthenticationMethod(list, this.WSSecurityAuthentication, new AuthenticationMethod[]
			{
				AuthenticationMethod.WSSecurity
			});
			ADExchangeServiceVirtualDirectory.AddOrRemoveAuthenticationMethod(list, this.LiveIdBasicAuthentication, new AuthenticationMethod[]
			{
				AuthenticationMethod.LiveIdBasic
			});
			List<AuthenticationMethod> authenticationMethods = list;
			bool? authenticationMethodFlag = this.BasicAuthentication;
			AuthenticationMethod[] applicableAuthenticationMethods = new AuthenticationMethod[1];
			ADExchangeServiceVirtualDirectory.AddOrRemoveAuthenticationMethod(authenticationMethods, authenticationMethodFlag, applicableAuthenticationMethods);
			ADExchangeServiceVirtualDirectory.AddOrRemoveAuthenticationMethod(list, this.DigestAuthentication, new AuthenticationMethod[]
			{
				AuthenticationMethod.Digest
			});
			ADExchangeServiceVirtualDirectory.AddOrRemoveAuthenticationMethod(list, this.WindowsAuthentication, new AuthenticationMethod[]
			{
				AuthenticationMethod.Ntlm,
				AuthenticationMethod.WindowsIntegrated
			});
			ADExchangeServiceVirtualDirectory.AddOrRemoveAuthenticationMethod(list, this.OAuthAuthentication, new AuthenticationMethod[]
			{
				AuthenticationMethod.OAuth
			});
			MultiValuedProperty<AuthenticationMethod> multiValuedProperty = new MultiValuedProperty<AuthenticationMethod>(list);
			base.InternalAuthenticationMethods = multiValuedProperty;
			base.ExternalAuthenticationMethods = multiValuedProperty;
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

		public new MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			get
			{
				return base.InternalAuthenticationMethods;
			}
		}

		public new MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
		{
			get
			{
				return base.ExternalAuthenticationMethods;
			}
		}

		public bool? LiveIdNegotiateAuthentication
		{
			get
			{
				return this.liveIdNegotiateAuthentication;
			}
			set
			{
				this.liveIdNegotiateAuthentication = value;
				this.UpdateInternalAndExternalAuthenticationMethods();
			}
		}

		public bool? WSSecurityAuthentication
		{
			get
			{
				return this.wsSecurityAuthentication;
			}
			set
			{
				this.wsSecurityAuthentication = value;
				this.UpdateInternalAndExternalAuthenticationMethods();
			}
		}

		public bool? LiveIdBasicAuthentication
		{
			get
			{
				return this.liveIdBasicAuthentication;
			}
			set
			{
				this.liveIdBasicAuthentication = value;
				this.UpdateInternalAndExternalAuthenticationMethods();
			}
		}

		public bool? BasicAuthentication
		{
			get
			{
				return this.basicAuthentication;
			}
			set
			{
				this.basicAuthentication = value;
				this.UpdateInternalAndExternalAuthenticationMethods();
			}
		}

		public bool? DigestAuthentication
		{
			get
			{
				return this.digestAuthentication;
			}
			set
			{
				this.digestAuthentication = value;
				this.UpdateInternalAndExternalAuthenticationMethods();
			}
		}

		public bool? WindowsAuthentication
		{
			get
			{
				return this.windowsAuthentication;
			}
			set
			{
				this.windowsAuthentication = value;
				this.UpdateInternalAndExternalAuthenticationMethods();
			}
		}

		public bool? OAuthAuthentication
		{
			get
			{
				return this.oAuthAuthentication;
			}
			set
			{
				this.oAuthAuthentication = value;
				this.UpdateInternalAndExternalAuthenticationMethods();
			}
		}

		public bool? AdfsAuthentication
		{
			get
			{
				return this.adfsAuthentication;
			}
			set
			{
				this.adfsAuthentication = value;
				this.UpdateInternalAndExternalAuthenticationMethods();
			}
		}

		private bool? basicAuthentication = null;

		private bool? digestAuthentication = null;

		private bool? windowsAuthentication = null;

		private bool? liveIdBasicAuthentication = null;

		private bool? wsSecurityAuthentication = null;

		private bool? liveIdNegotiateAuthentication = null;

		private bool? oAuthAuthentication = null;

		private bool? adfsAuthentication = null;
	}
}
