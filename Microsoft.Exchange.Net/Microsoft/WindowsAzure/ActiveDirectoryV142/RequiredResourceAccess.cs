using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class RequiredResourceAccess
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static RequiredResourceAccess CreateRequiredResourceAccess(Guid resourceAppId, Collection<RequiredAppPermission> requiredAppPermissions)
		{
			RequiredResourceAccess requiredResourceAccess = new RequiredResourceAccess();
			requiredResourceAccess.resourceAppId = resourceAppId;
			if (requiredAppPermissions == null)
			{
				throw new ArgumentNullException("requiredAppPermissions");
			}
			requiredResourceAccess.requiredAppPermissions = requiredAppPermissions;
			return requiredResourceAccess;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Guid resourceAppId
		{
			get
			{
				return this._resourceAppId;
			}
			set
			{
				this._resourceAppId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<RequiredAppPermission> requiredAppPermissions
		{
			get
			{
				return this._requiredAppPermissions;
			}
			set
			{
				this._requiredAppPermissions = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid _resourceAppId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<RequiredAppPermission> _requiredAppPermissions = new Collection<RequiredAppPermission>();
	}
}
