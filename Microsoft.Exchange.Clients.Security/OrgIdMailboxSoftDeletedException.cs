using System;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Security
{
	public class OrgIdMailboxSoftDeletedException : OrgIdMailboxNotFoundException
	{
		protected override string ErrorMessageFormatString
		{
			get
			{
				return Strings.MailboxSoftDeletedErrorMessage;
			}
		}

		public override ErrorMode? ErrorMode
		{
			get
			{
				return new ErrorMode?(Microsoft.Exchange.Clients.Common.ErrorMode.MailboxSoftDeleted);
			}
		}

		public override Strings.IDs ErrorMessageStringId
		{
			get
			{
				return 637041586;
			}
		}

		public OrgIdMailboxSoftDeletedException(string userName, string logoutUrl) : base(userName, logoutUrl)
		{
		}
	}
}
