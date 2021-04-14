using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxInfoForLinking
	{
		internal string TenantName { get; set; }

		internal Guid MailboxGuid { get; set; }

		internal CultureInfo PreferredCulture { get; set; }

		internal MailboxInfoForLinking()
		{
			this.TenantName = string.Empty;
			this.MailboxGuid = Guid.NewGuid();
			this.PreferredCulture = CultureInfo.InvariantCulture;
		}

		public static MailboxInfoForLinking CreateFromMailboxSession(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (mailboxSession.MailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxSession.MailboxOwner");
			}
			string tenantName = string.Empty;
			if (mailboxSession.MailboxOwner.MailboxInfo.OrganizationId != null && mailboxSession.MailboxOwner.MailboxInfo.OrganizationId.OrganizationalUnit != null)
			{
				tenantName = mailboxSession.MailboxOwner.MailboxInfo.OrganizationId.OrganizationalUnit.ToString();
			}
			CultureInfo preferredCulture = CultureInfo.InvariantCulture;
			if (mailboxSession.Capabilities.CanHaveCulture && mailboxSession.PreferedCulture != null)
			{
				preferredCulture = mailboxSession.PreferedCulture;
			}
			return new MailboxInfoForLinking
			{
				TenantName = tenantName,
				MailboxGuid = mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid,
				PreferredCulture = preferredCulture
			};
		}

		public override string ToString()
		{
			return string.Format("TenantName: {0}, MailboxGuid: {1}", this.TenantName, this.MailboxGuid);
		}
	}
}
