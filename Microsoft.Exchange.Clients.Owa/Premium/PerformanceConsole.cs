using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class PerformanceConsole : OwaForm, IRegistryOnlyForm
	{
		public static int PerformanceConsoleWidth
		{
			get
			{
				return 900;
			}
		}

		public static int PerformanceConsoleHeight
		{
			get
			{
				return 641;
			}
		}

		public static bool IsPerformanceConsoleEnabled(ISessionContext sessionContext)
		{
			if (sessionContext == null)
			{
				throw new ArgumentNullException("sessionContext");
			}
			return Globals.CollectPerRequestPerformanceStats && !sessionContext.IsExplicitLogon && !sessionContext.IsWebPartRequest;
		}

		public PerformanceConsole() : base(false)
		{
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		protected void ReadHtml(TextWriter writer)
		{
			OwaContext.Current.UserContext.PerformanceConsoleNotifier.ReadDataAsHtml(writer);
		}

		private Infobar infobar = new Infobar();
	}
}
