using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MatchAdminFolderFlags : IValidator
	{
		internal MatchAdminFolderFlags(ELCFolderFlags adminFolderFlags)
		{
			this.adminFolderFlags = adminFolderFlags;
		}

		public bool Validate(DefaultFolderContext context, PropertyBag propertyBag)
		{
			ELCFolderFlags? valueAsNullable = propertyBag.GetValueAsNullable<ELCFolderFlags>(InternalSchema.AdminFolderFlags);
			if (valueAsNullable != null && valueAsNullable.Value == this.adminFolderFlags)
			{
				return true;
			}
			if (valueAsNullable == null && this.adminFolderFlags == ELCFolderFlags.DumpsterFolder)
			{
				throw new DefaultFolderPropertyValidationException(ServerStrings.PropertyErrorString("AdminFolderFlags", PropertyErrorCode.NotFound, this.adminFolderFlags.ToString()));
			}
			return false;
		}

		public void SetProperties(DefaultFolderContext context, Folder folder)
		{
			MatchAdminFolderFlags.<>c__DisplayClass2 CS$<>8__locals1 = new MatchAdminFolderFlags.<>c__DisplayClass2();
			CS$<>8__locals1.folder = folder;
			CS$<>8__locals1.<>4__this = this;
			ExTraceGlobals.DefaultFoldersTracer.TraceDebug<ELCFolderFlags, Folder>((long)this.GetHashCode(), "MatchAdminFolderFlags::SetPropertiesInternal. Setting AdminFolderFlags to {0} for folder {1} on the admin session.", this.adminFolderFlags, CS$<>8__locals1.folder);
			if (context.Session.LogonType == LogonType.Admin || (context.Session != null && context.Session.IsMoveUser))
			{
				CS$<>8__locals1.folder[InternalSchema.AdminFolderFlags] = this.adminFolderFlags;
				return;
			}
			using (MailboxSession adminSession = MailboxSession.OpenAsAdmin(context.Session.MailboxOwner, CultureInfo.InvariantCulture, context.Session.ClientInfoString + ";COW=DumpsterFolderFlag"))
			{
				adminSession.BypassAuditing(delegate
				{
					adminSession.BypassAuditsFolderAccessChecking(delegate
					{
						using (Folder folder2 = Folder.Bind(adminSession, CS$<>8__locals1.folder.Id, new PropertyDefinition[]
						{
							FolderSchema.AdminFolderFlags
						}))
						{
							folder2[InternalSchema.AdminFolderFlags] = CS$<>8__locals1.<>4__this.adminFolderFlags;
							folder2.Save();
						}
					});
				});
			}
		}

		private ELCFolderFlags adminFolderFlags;
	}
}
