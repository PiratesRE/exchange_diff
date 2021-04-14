using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class RequiredAppPermission
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static RequiredAppPermission CreateRequiredAppPermission(Guid permissionId, bool directAccessGrant, Collection<string> impersonationAccessGrants)
		{
			RequiredAppPermission requiredAppPermission = new RequiredAppPermission();
			requiredAppPermission.permissionId = permissionId;
			requiredAppPermission.directAccessGrant = directAccessGrant;
			if (impersonationAccessGrants == null)
			{
				throw new ArgumentNullException("impersonationAccessGrants");
			}
			requiredAppPermission.impersonationAccessGrants = impersonationAccessGrants;
			return requiredAppPermission;
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
		public bool directAccessGrant
		{
			get
			{
				return this._directAccessGrant;
			}
			set
			{
				this._directAccessGrant = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> impersonationAccessGrants
		{
			get
			{
				return this._impersonationAccessGrants;
			}
			set
			{
				this._impersonationAccessGrants = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid _permissionId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _directAccessGrant;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _impersonationAccessGrants = new Collection<string>();
	}
}
