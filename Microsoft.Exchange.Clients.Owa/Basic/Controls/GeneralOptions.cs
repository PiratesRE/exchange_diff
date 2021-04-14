using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class GeneralOptions : OptionsBase
	{
		public GeneralOptions(OwaContext owaContext, TextWriter writer) : base(owaContext, writer)
		{
			this.CommitAndLoad();
		}

		private void Load()
		{
			this.isOptimizedForAccessibility = this.userContext.UserOptions.IsOptimizedForAccessibility;
			this.isRichClientFeatureEnabled = this.userContext.IsFeatureEnabled(Feature.RichClient);
		}

		private void CommitAndLoad()
		{
			this.Load();
			bool flag = false;
			if (Utilities.IsPostRequest(this.request) && !string.IsNullOrEmpty(base.Command) && this.isRichClientFeatureEnabled)
			{
				this.isOptimizedForAccessibility = (Utilities.GetFormParameter(this.request, "chkOptAcc", false) != null);
				if (this.userContext.UserOptions.IsOptimizedForAccessibility != this.isOptimizedForAccessibility)
				{
					this.userContext.UserOptions.IsOptimizedForAccessibility = this.isOptimizedForAccessibility;
					flag = true;
				}
				if (flag)
				{
					try
					{
						this.userContext.UserOptions.CommitChanges();
						base.SetSavedSuccessfully(true);
					}
					catch (StorageTransientException)
					{
						base.SetSavedSuccessfully(false);
					}
					catch (StoragePermanentException)
					{
						base.SetSavedSuccessfully(false);
					}
				}
			}
		}

		public override void Render()
		{
			this.RenderAccessibilityOptions();
		}

		public override void RenderScript()
		{
			base.RenderJSVariable("a_fOptAcc", this.isOptimizedForAccessibility.ToString().ToLowerInvariant());
		}

		private void RenderAccessibilityOptions()
		{
			string format = "<input type=\"checkbox\" name=\"{0}\"{1}{3} id=\"{0}\" onclick=\"return onClkChkBx(this);\" value=\"1\"><label for=\"{0}\">{2}</label>";
			base.RenderHeaderRow(ThemeFileId.AboutOwa, 951662406);
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1435977365));
			this.writer.Write("<ul><li>{0}</li><li>{1}</li><li>{2}</li></ul>", LocalizedStrings.GetHtmlEncoded(2267445), LocalizedStrings.GetHtmlEncoded(405551972), LocalizedStrings.GetHtmlEncoded(-1160531969));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write(format, new object[]
			{
				"chkOptAcc",
				this.isOptimizedForAccessibility ? " checked" : string.Empty,
				LocalizedStrings.GetHtmlEncoded(-2119250240),
				this.isRichClientFeatureEnabled ? string.Empty : " disabled"
			});
			this.writer.Write("<div id=\"olo\">{0}</div>", LocalizedStrings.GetHtmlEncoded(this.isRichClientFeatureEnabled ? -1771373774 : 1767653808));
			this.writer.Write("</td></tr>");
		}

		private const string FormOptimizeForAccessibility = "chkOptAcc";

		private const string FormJavaScriptOptimizeForAccessibility = "a_fOptAcc";

		private bool isOptimizedForAccessibility;

		private bool isRichClientFeatureEnabled;
	}
}
