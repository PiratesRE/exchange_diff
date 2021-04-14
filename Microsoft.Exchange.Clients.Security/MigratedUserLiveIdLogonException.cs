using System;

namespace Microsoft.Exchange.Clients.Security
{
	public class MigratedUserLiveIdLogonException : OrgIdLogonException
	{
		protected override string ErrorMessageFormatString
		{
			get
			{
				return Strings.MigratedUserLiveIdLogonErrorMessage;
			}
		}

		public override Strings.IDs ErrorMessageStringId
		{
			get
			{
				return 1799660809;
			}
		}

		public MigratedUserLiveIdLogonException(string userName, string logoutUrl) : base(userName, logoutUrl)
		{
		}
	}
}
