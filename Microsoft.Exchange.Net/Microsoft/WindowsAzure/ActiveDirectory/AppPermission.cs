using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	public class AppPermission
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static AppPermission CreateAppPermission(Collection<string> directAccessGrantTypes, Collection<ImpersonationAccessGrantType> impersonationAccessGrantTypes, bool isDisabled, Guid permissionId)
		{
			AppPermission appPermission = new AppPermission();
			if (directAccessGrantTypes == null)
			{
				throw new ArgumentNullException("directAccessGrantTypes");
			}
			appPermission.directAccessGrantTypes = directAccessGrantTypes;
			if (impersonationAccessGrantTypes == null)
			{
				throw new ArgumentNullException("impersonationAccessGrantTypes");
			}
			appPermission.impersonationAccessGrantTypes = impersonationAccessGrantTypes;
			appPermission.isDisabled = isDisabled;
			appPermission.permissionId = permissionId;
			return appPermission;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string claimValue
		{
			get
			{
				return this._claimValue;
			}
			set
			{
				this._claimValue = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string description
		{
			get
			{
				return this._description;
			}
			set
			{
				this._description = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> directAccessGrantTypes
		{
			get
			{
				return this._directAccessGrantTypes;
			}
			set
			{
				this._directAccessGrantTypes = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string displayName
		{
			get
			{
				return this._displayName;
			}
			set
			{
				this._displayName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<ImpersonationAccessGrantType> impersonationAccessGrantTypes
		{
			get
			{
				return this._impersonationAccessGrantTypes;
			}
			set
			{
				this._impersonationAccessGrantTypes = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool isDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				this._isDisabled = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string origin
		{
			get
			{
				return this._origin;
			}
			set
			{
				this._origin = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Guid permissionId
		{
			get
			{
				return this._permissionId;
			}
			set
			{
				this._permissionId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string resourceScopeType
		{
			get
			{
				return this._resourceScopeType;
			}
			set
			{
				this._resourceScopeType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string userConsentDescription
		{
			get
			{
				return this._userConsentDescription;
			}
			set
			{
				this._userConsentDescription = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string userConsentDisplayName
		{
			get
			{
				return this._userConsentDisplayName;
			}
			set
			{
				this._userConsentDisplayName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _claimValue;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _description;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _directAccessGrantTypes = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ImpersonationAccessGrantType> _impersonationAccessGrantTypes = new Collection<ImpersonationAccessGrantType>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _isDisabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _origin;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid _permissionId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _resourceScopeType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _userConsentDescription;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _userConsentDisplayName;
	}
}
