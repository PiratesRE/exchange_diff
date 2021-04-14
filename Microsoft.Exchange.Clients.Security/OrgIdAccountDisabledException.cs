using System;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Security
{
	public class OrgIdAccountDisabledException : OrgIdMailboxNotFoundException
	{
		protected override string ErrorMessageFormatString
		{
			get
			{
				return Strings.OrgIdAccountDisabledErrorMessage;
			}
		}

		public override Strings.IDs ErrorMessageStringId
		{
			get
			{
				return -106213327;
			}
		}

		public override ErrorMode? ErrorMode
		{
			get
			{
				return new ErrorMode?(Microsoft.Exchange.Clients.Common.ErrorMode.AccountDisabled);
			}
		}

		public OrgIdAccountDisabledException(string userName, string logoutUrl) : base(userName, logoutUrl)
		{
		}
	}
}
