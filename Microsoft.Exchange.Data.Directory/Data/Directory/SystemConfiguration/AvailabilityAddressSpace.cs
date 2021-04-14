using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class AvailabilityAddressSpace : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AvailabilityAddressSpace.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AvailabilityAddressSpace.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return AvailabilityConfig.Container;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string ForestName
		{
			get
			{
				return (string)this[AvailabilityAddressSpaceSchema.ForestName];
			}
			set
			{
				this[AvailabilityAddressSpaceSchema.ForestName] = value;
			}
		}

		public string UserName
		{
			get
			{
				return (string)this[AvailabilityAddressSpaceSchema.UserName];
			}
			internal set
			{
				this[AvailabilityAddressSpaceSchema.UserName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseServiceAccount
		{
			get
			{
				return (bool)this[AvailabilityAddressSpaceSchema.UseServiceAccount];
			}
			set
			{
				this[AvailabilityAddressSpaceSchema.UseServiceAccount] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public AvailabilityAccessMethod AccessMethod
		{
			get
			{
				return (AvailabilityAccessMethod)this[AvailabilityAddressSpaceSchema.AccessMethod];
			}
			set
			{
				this[AvailabilityAddressSpaceSchema.AccessMethod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri ProxyUrl
		{
			get
			{
				return (Uri)this[AvailabilityAddressSpaceSchema.ProxyUrl];
			}
			set
			{
				this[AvailabilityAddressSpaceSchema.ProxyUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri TargetAutodiscoverEpr
		{
			get
			{
				return (Uri)this[AvailabilityAddressSpaceSchema.TargetAutodiscoverEpr];
			}
			set
			{
				this[AvailabilityAddressSpaceSchema.TargetAutodiscoverEpr] = value;
			}
		}

		public ADObjectId ParentPathId
		{
			get
			{
				return AvailabilityConfig.Container;
			}
		}

		internal SecureString GetPassword()
		{
			SecureString secureString = new SecureString();
			string text = (string)this[AvailabilityAddressSpaceSchema.Password];
			if (!string.IsNullOrEmpty(text))
			{
				for (int i = 0; i < text.Length; i++)
				{
					secureString.AppendChar(text[i]);
				}
			}
			return secureString;
		}

		internal void SetPassword(SecureString securePassword)
		{
			if (securePassword != null)
			{
				this[AvailabilityAddressSpaceSchema.Password] = securePassword.ConvertToUnsecureString();
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.UseServiceAccount && !string.IsNullOrEmpty(this.UserName))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ASOnlyOneAuthenticationMethodAllowed, base.Id, string.Empty));
			}
			AvailabilityAccessMethod accessMethod = this.AccessMethod;
			switch (accessMethod)
			{
			case AvailabilityAccessMethod.PerUserFB:
			case AvailabilityAccessMethod.OrgWideFB:
			case AvailabilityAccessMethod.OrgWideFBBasic:
				if (!this.UseServiceAccount && string.IsNullOrEmpty(this.UserName))
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ASAccessMethodNeedsAuthenticationAccount, base.Id, string.Empty));
				}
				break;
			case AvailabilityAccessMethod.PublicFolder:
				if (this.UseServiceAccount || !string.IsNullOrEmpty(this.UserName))
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ASInvalidAuthenticationOptionsForAccessMethod, base.Id, string.Empty));
				}
				break;
			case AvailabilityAccessMethod.InternalProxy:
				if (!this.UseServiceAccount)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ASAccessMethodNeedsAuthenticationAccount, base.Id, string.Empty));
				}
				break;
			default:
				errors.Add(new ObjectValidationError(DirectoryStrings.ASInvalidAccessMethod, base.Id, string.Empty));
				break;
			}
			Uri proxyUrl = this.ProxyUrl;
			if (proxyUrl != null && accessMethod != AvailabilityAccessMethod.InternalProxy)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ASInvalidProxyASUrlOption, base.Id, string.Empty));
			}
		}

		private static AvailabilityAddressSpaceSchema schema = ObjectSchema.GetInstance<AvailabilityAddressSpaceSchema>();

		private static string mostDerivedClass = "msExchAvailabilityAddressSpace";
	}
}
