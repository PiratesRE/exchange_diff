using System;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class WebMethodEntry
	{
		public WebMethodEntry(string name, string category, string alternateSoapRequest, bool isPublic, bool requireUserAdminRole, bool isPartnerAppOnly, string defaultUserRoleType)
		{
			this.Name = name;
			this.Category = category;
			this.AlternateSoapRequestName = alternateSoapRequest;
			this.IsPublic = isPublic;
			this.RequireUserAdminRole = requireUserAdminRole;
			this.IsPartnerAppOnly = isPartnerAppOnly;
			this.DefaultUserRoleType = defaultUserRoleType;
		}

		public string Name { get; private set; }

		public string Category { get; private set; }

		public string AlternateSoapRequestName { get; private set; }

		public bool IsPublic { get; private set; }

		public bool RequireUserAdminRole { get; private set; }

		public bool IsPartnerAppOnly { get; private set; }

		public string DefaultUserRoleType { get; private set; }

		internal static readonly WebMethodEntry JsonWebMethodEntry = new WebMethodEntry("json", "JSON category", "json", true, false, false, null);
	}
}
