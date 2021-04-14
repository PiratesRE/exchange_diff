using System;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Security
{
	public class OrgIdSharedMailboxAccountDisabledException : OrgIdMailboxNotFoundException
	{
		protected override string ErrorMessageFormatString
		{
			get
			{
				return Strings.OrgIdSharedMailboxAccountDisabledErrorMessage;
			}
		}

		public override Strings.IDs ErrorMessageStringId
		{
			get
			{
				return -2011393914;
			}
		}

		public override ErrorMode? ErrorMode
		{
			get
			{
				return new ErrorMode?(Microsoft.Exchange.Clients.Common.ErrorMode.SharedMailboxAccountDisabled);
			}
		}

		public OrgIdSharedMailboxAccountDisabledException(string userName, string logoutUrl) : base(userName, logoutUrl)
		{
		}
	}
}
