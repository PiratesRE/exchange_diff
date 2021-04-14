using System;
using System.ComponentModel;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Common
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	public class LocalizedDisplayNameAttribute : DisplayNameAttribute, ILocalizedString
	{
		public LocalizedDisplayNameAttribute()
		{
		}

		public LocalizedDisplayNameAttribute(LocalizedString displayname)
		{
			this.displayname = displayname;
		}

		public sealed override string DisplayName
		{
			[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
			get
			{
				return this.displayname;
			}
		}

		public LocalizedString LocalizedString
		{
			get
			{
				return this.displayname;
			}
		}

		private LocalizedString displayname;
	}
}
