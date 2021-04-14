using System;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Security
{
	public class OrgIdMailboxNotFoundException : OrgIdLogonException
	{
		protected override string ErrorMessageFormatString
		{
			get
			{
				return Strings.MailboxNotFoundErrorMessage;
			}
		}

		public override Strings.IDs ErrorMessageStringId
		{
			get
			{
				return 1753500428;
			}
		}

		public virtual ErrorMode? ErrorMode
		{
			get
			{
				return null;
			}
		}

		public OrgIdMailboxNotFoundException(string userName, string logoutUrl) : base(userName, logoutUrl)
		{
		}
	}
}
