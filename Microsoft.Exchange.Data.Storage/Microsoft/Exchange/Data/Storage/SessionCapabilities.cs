using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SessionCapabilities
	{
		internal SessionCapabilities(SessionCapabilitiesFlags flags)
		{
			this.Flags = flags;
		}

		public SessionCapabilitiesFlags CapabilitiesFlags
		{
			get
			{
				return this.Flags;
			}
		}

		public bool CanSend
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanSend);
			}
		}

		public bool CanDeliver
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanDeliver);
			}
		}

		public bool CanCreateDefaultFolders
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanCreateDefaultFolders);
			}
		}

		public bool MustHideDefaultFolders
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.MustHideDefaultFolders);
			}
		}

		public bool CanHaveDelegateUsers
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanHaveDelegateUsers);
			}
		}

		public bool CanHaveExternalUsers
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanHaveExternalUsers);
			}
		}

		public bool CanHaveRules
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanHaveRules);
			}
		}

		public bool CanHaveJunkEmailRule
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanHaveJunkEmailRule);
			}
		}

		public bool CanHaveMasterCategoryList
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanHaveMasterCategoryList);
			}
		}

		public bool CanHaveOof
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanHaveOof);
			}
		}

		public bool CanHaveUserConfigurationManager
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanHaveUserConfigurationManager);
			}
		}

		public bool MustCreateFolderHierarchy
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.MustCreateFolderHierarchy);
			}
		}

		public bool CanHaveCulture
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanHaveCulture);
			}
		}

		public bool Default
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.Default);
			}
		}

		public bool CanSetCalendarAPIProperties
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.CanSetCalendarAPIProperties);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.CheckFlags(SessionCapabilitiesFlags.ReadOnly);
			}
		}

		internal SessionCapabilities CloneAndExtendCapabilities(SessionCapabilitiesFlags sessionCapabilitiesFlags)
		{
			return new SessionCapabilities(this.Flags | sessionCapabilitiesFlags);
		}

		private bool CheckFlags(SessionCapabilitiesFlags flags)
		{
			return (this.Flags & flags) == flags;
		}

		internal static SessionCapabilities PrimarySessionCapabilities = new SessionCapabilities(SessionCapabilitiesFlags.Default);

		internal static SessionCapabilities ArchiveSessionCapabilities = new SessionCapabilities(SessionCapabilitiesFlags.CanCreateDefaultFolders | SessionCapabilitiesFlags.MustHideDefaultFolders | SessionCapabilitiesFlags.CanHaveUserConfigurationManager);

		internal static SessionCapabilities MirrorSessionCapabilities = new SessionCapabilities(SessionCapabilitiesFlags.CanSend | SessionCapabilitiesFlags.CanCreateDefaultFolders | SessionCapabilitiesFlags.CanHaveRules | SessionCapabilitiesFlags.CanHaveJunkEmailRule | SessionCapabilitiesFlags.CanHaveMasterCategoryList | SessionCapabilitiesFlags.CanHaveOof | SessionCapabilitiesFlags.CanHaveUserConfigurationManager | SessionCapabilitiesFlags.CanHaveCulture);

		private readonly SessionCapabilitiesFlags Flags;
	}
}
