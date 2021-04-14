using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADVirtualDirectory : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADVirtualDirectory.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADAutodiscoverVirtualDirectory.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADPowerShellCommonVirtualDirectory.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADOwaVirtualDirectory.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADWebServicesVirtualDirectory.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "msExchMobileVirtualDirectory"),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADOabVirtualDirectory.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADRpcHttpVirtualDirectory.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADAvailabilityForeignConnectorVirtualDirectory.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADEcpVirtualDirectory.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADE12UMVirtualDirectory.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADMapiVirtualDirectory.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADO365SuiteServiceVirtualDirectory.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADSnackyServiceVirtualDirectory.MostDerivedClass)
				});
			}
		}

		internal static object ServerGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
				if (adobjectId == null && (ObjectState)propertyBag[ADObjectSchema.ObjectState] != ObjectState.New)
				{
					throw new InvalidOperationException(DirectoryStrings.IdIsNotSet);
				}
				result = ((adobjectId == null) ? null : adobjectId.AncestorDN(3));
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Server", ex.Message), ADVirtualDirectorySchema.Server, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		internal static object ServerNameGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
				if (adobjectId == null && (ObjectState)propertyBag[ADObjectSchema.ObjectState] != ObjectState.New)
				{
					throw new InvalidOperationException(DirectoryStrings.IdIsNotSet);
				}
				result = ((adobjectId == null) ? null : ADVirtualDirectory.GetServerNameFromVDirObjectId(adobjectId));
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Server", ex.Message), ADVirtualDirectorySchema.Server, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		internal static object InternalAuthenticationMethodsGetter(IPropertyBag propertyBag)
		{
			AuthenticationMethodFlags authenticationMethodFlags = (AuthenticationMethodFlags)propertyBag[ADVirtualDirectorySchema.InternalAuthenticationMethodFlags];
			return ADVirtualDirectory.AuthenticationMethodFlagsToAuthenticationMethodPropertyValue(authenticationMethodFlags);
		}

		internal static object ExternalAuthenticationMethodsGetter(IPropertyBag propertyBag)
		{
			AuthenticationMethodFlags authenticationMethodFlags = (AuthenticationMethodFlags)propertyBag[ADVirtualDirectorySchema.ExternalAuthenticationMethodFlags];
			return ADVirtualDirectory.AuthenticationMethodFlagsToAuthenticationMethodPropertyValue(authenticationMethodFlags);
		}

		internal static MultiValuedProperty<AuthenticationMethod> AuthenticationMethodFlagsToAuthenticationMethodPropertyValue(AuthenticationMethodFlags authenticationMethodFlags)
		{
			if (authenticationMethodFlags != AuthenticationMethodFlags.None)
			{
				List<AuthenticationMethod> list = new List<AuthenticationMethod>(3);
				if ((authenticationMethodFlags & AuthenticationMethodFlags.Basic) == AuthenticationMethodFlags.Basic)
				{
					list.Add(AuthenticationMethod.Basic);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.Fba) == AuthenticationMethodFlags.Fba)
				{
					list.Add(AuthenticationMethod.Fba);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.Ntlm) == AuthenticationMethodFlags.Ntlm)
				{
					list.Add(AuthenticationMethod.Ntlm);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.Digest) == AuthenticationMethodFlags.Digest)
				{
					list.Add(AuthenticationMethod.Digest);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.WindowsIntegrated) == AuthenticationMethodFlags.WindowsIntegrated)
				{
					list.Add(AuthenticationMethod.WindowsIntegrated);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.LiveIdFba) == AuthenticationMethodFlags.LiveIdFba)
				{
					list.Add(AuthenticationMethod.LiveIdFba);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.LiveIdBasic) == AuthenticationMethodFlags.LiveIdBasic)
				{
					list.Add(AuthenticationMethod.LiveIdBasic);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.WSSecurity) == AuthenticationMethodFlags.WSSecurity)
				{
					list.Add(AuthenticationMethod.WSSecurity);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.Certificate) == AuthenticationMethodFlags.Certificate)
				{
					list.Add(AuthenticationMethod.Certificate);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.NegoEx) == AuthenticationMethodFlags.NegoEx)
				{
					list.Add(AuthenticationMethod.NegoEx);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.LiveIdNegotiate) == AuthenticationMethodFlags.LiveIdNegotiate)
				{
					list.Add(AuthenticationMethod.LiveIdNegotiate);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.OAuth) == AuthenticationMethodFlags.OAuth)
				{
					list.Add(AuthenticationMethod.OAuth);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.Adfs) == AuthenticationMethodFlags.Adfs)
				{
					list.Add(AuthenticationMethod.Adfs);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.Kerberos) == AuthenticationMethodFlags.Kerberos)
				{
					list.Add(AuthenticationMethod.Kerberos);
				}
				if ((authenticationMethodFlags & AuthenticationMethodFlags.Negotiate) == AuthenticationMethodFlags.Negotiate)
				{
					list.Add(AuthenticationMethod.Negotiate);
				}
				return new MultiValuedProperty<AuthenticationMethod>(list);
			}
			return ADVirtualDirectory.EmptyAuthenticationMethodPropertyValue;
		}

		internal static void InternalAuthenticationMethodsSetter(object value, IPropertyBag propertyBag)
		{
			AuthenticationMethodFlags authenticationMethodFlags = ADVirtualDirectory.AuthenticationMethodPropertyValueToAuthenticationMethodFlags((MultiValuedProperty<AuthenticationMethod>)value);
			propertyBag[ADVirtualDirectorySchema.InternalAuthenticationMethodFlags] = authenticationMethodFlags;
		}

		internal static void ExternalAuthenticationMethodsSetter(object value, IPropertyBag propertyBag)
		{
			AuthenticationMethodFlags authenticationMethodFlags = ADVirtualDirectory.AuthenticationMethodPropertyValueToAuthenticationMethodFlags((MultiValuedProperty<AuthenticationMethod>)value);
			propertyBag[ADVirtualDirectorySchema.ExternalAuthenticationMethodFlags] = authenticationMethodFlags;
		}

		internal static AuthenticationMethodFlags AuthenticationMethodPropertyValueToAuthenticationMethodFlags(MultiValuedProperty<AuthenticationMethod> authenticationMethods)
		{
			AuthenticationMethodFlags authenticationMethodFlags = AuthenticationMethodFlags.None;
			if (authenticationMethods != null)
			{
				foreach (AuthenticationMethod authenticationMethod in authenticationMethods)
				{
					if (authenticationMethod == AuthenticationMethod.Basic)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.Basic;
					}
					else if (authenticationMethod == AuthenticationMethod.Fba)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.Fba;
					}
					else if (authenticationMethod == AuthenticationMethod.Ntlm)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.Ntlm;
					}
					else if (authenticationMethod == AuthenticationMethod.Digest)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.Digest;
					}
					else if (authenticationMethod == AuthenticationMethod.WindowsIntegrated)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.WindowsIntegrated;
					}
					else if (authenticationMethod == AuthenticationMethod.LiveIdFba)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.LiveIdFba;
					}
					else if (authenticationMethod == AuthenticationMethod.LiveIdBasic)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.LiveIdBasic;
					}
					else if (authenticationMethod == AuthenticationMethod.WSSecurity)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.WSSecurity;
					}
					else if (authenticationMethod == AuthenticationMethod.Certificate)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.Certificate;
					}
					else if (authenticationMethod == AuthenticationMethod.NegoEx)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.NegoEx;
					}
					else if (authenticationMethod == AuthenticationMethod.LiveIdNegotiate)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.LiveIdNegotiate;
					}
					else if (authenticationMethod == AuthenticationMethod.OAuth)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.OAuth;
					}
					else if (authenticationMethod == AuthenticationMethod.Adfs)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.Adfs;
					}
					else if (authenticationMethod == AuthenticationMethod.Kerberos)
					{
						authenticationMethodFlags |= AuthenticationMethodFlags.Kerberos;
					}
					else
					{
						if (authenticationMethod != AuthenticationMethod.Negotiate)
						{
							throw new ArgumentOutOfRangeException("value");
						}
						authenticationMethodFlags |= AuthenticationMethodFlags.Negotiate;
					}
				}
			}
			return authenticationMethodFlags;
		}

		public ServerVersion AdminDisplayVersion
		{
			get
			{
				return (ServerVersion)this[ADVirtualDirectorySchema.AdminDisplayVersion];
			}
			internal set
			{
				this[ADVirtualDirectorySchema.AdminDisplayVersion] = value;
			}
		}

		public ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[ADVirtualDirectorySchema.Server];
			}
		}

		public Uri InternalUrl
		{
			get
			{
				return (Uri)this[ADVirtualDirectorySchema.InternalUrl];
			}
			set
			{
				this[ADVirtualDirectorySchema.InternalUrl] = value;
			}
		}

		public MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			get
			{
				return (MultiValuedProperty<AuthenticationMethod>)this[ADVirtualDirectorySchema.InternalAuthenticationMethods];
			}
			set
			{
				this[ADVirtualDirectorySchema.InternalAuthenticationMethods] = value;
			}
		}

		public Uri ExternalUrl
		{
			get
			{
				return (Uri)this[ADVirtualDirectorySchema.ExternalUrl];
			}
			set
			{
				this[ADVirtualDirectorySchema.ExternalUrl] = value;
			}
		}

		public MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
		{
			get
			{
				return (MultiValuedProperty<AuthenticationMethod>)this[ADVirtualDirectorySchema.ExternalAuthenticationMethods];
			}
			set
			{
				this[ADVirtualDirectorySchema.ExternalAuthenticationMethods] = value;
			}
		}

		internal static string GetServerNameFromVDirObjectId(ADObjectId vDirObjectId)
		{
			ADObjectId adobjectId = vDirObjectId.AncestorDN(3);
			return adobjectId.Name;
		}

		private static readonly string mostDerivedClass = "msExchVirtualDirectory";

		private static readonly MultiValuedProperty<AuthenticationMethod> EmptyAuthenticationMethodPropertyValue = new MultiValuedProperty<AuthenticationMethod>();

		private static readonly ADObjectSchema schema = ObjectSchema.GetInstance<ADVirtualDirectoryProperties>();
	}
}
