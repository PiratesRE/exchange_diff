using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class MailboxSessionEstablisher
	{
		internal static event EventHandler<MailboxConnectionArgs> OnMailboxConnectionAttempted;

		public static MailboxSession OpenAsAdmin(ExchangePrincipal mailboxOwner, CultureInfo cultureInfo, string clientInfoString)
		{
			MailboxSession returnValue = null;
			MailboxSessionEstablisher.ConnectAndRaise(delegate
			{
				returnValue = MailboxSession.OpenAsAdmin(mailboxOwner, cultureInfo, clientInfoString);
			});
			return returnValue;
		}

		public static bool ConnectWithStatus(MailboxSession session)
		{
			bool returnValue = false;
			MailboxSessionEstablisher.ConnectAndRaise(delegate
			{
				returnValue = session.ConnectWithStatus();
			});
			return returnValue;
		}

		private static void ConnectAndRaise(MailboxSessionEstablisher.CatchMe method)
		{
			bool connected = false;
			try
			{
				method();
				connected = true;
			}
			finally
			{
				if (MailboxSessionEstablisher.OnMailboxConnectionAttempted != null)
				{
					MailboxSessionEstablisher.OnMailboxConnectionAttempted(null, new MailboxConnectionArgs(connected));
				}
			}
		}

		private delegate void CatchMe();
	}
}
