using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.HttpProxy
{
	public class Logoff : OwaPage
	{
		protected Logoff.LogoffReason Reason
		{
			get
			{
				return this.reason;
			}
		}

		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected string Message
		{
			get
			{
				if (this.Reason != Logoff.LogoffReason.ChangePassword)
				{
					return LocalizedStrings.GetHtmlEncoded(1735477837);
				}
				if (base.IsDownLevelClient)
				{
					return LocalizedStrings.GetHtmlEncoded(252488134);
				}
				return LocalizedStrings.GetHtmlEncoded(575439440);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			if (base.Request.IsChangePasswordLogoff())
			{
				this.reason = Logoff.LogoffReason.ChangePassword;
			}
			base.OnLoad(e);
		}

		private Logoff.LogoffReason reason;

		protected enum LogoffReason
		{
			UserInitiated,
			ChangePassword
		}
	}
}
