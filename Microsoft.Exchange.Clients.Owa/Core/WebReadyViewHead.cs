using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class WebReadyViewHead : OwaPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (Utilities.GetQueryStringParameter(base.Request, "pn", false) != null)
			{
				throw new OwaInvalidRequestException("Page number (pn) parameter is not permitted in the current URL");
			}
			this.utilities = new WebReadyViewUtilities(base.OwaContext);
			this.utilities.LoadDocument(false, out this.decryptionStatus);
		}

		protected void RenderMessage(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			AttachmentPolicy.Level levelForAttachment = AttachmentLevelLookup.GetLevelForAttachment(this.utilities.FileExtension, this.utilities.MimeType, base.UserContext);
			if (AttachmentPolicy.Level.Block == levelForAttachment)
			{
				if (base.UserContext.IsBasicExperience)
				{
					output.Write("<table class=tbWIB><tr><td class=\"");
					output.Write(Utilities.GetTDClassForWebReadyViewHead(base.UserContext.IsBasicExperience));
					output.Write("\"><img  class=\"iei errInfo\" src=\"");
					base.OwaContext.UserContext.RenderThemeFileUrl(output, ThemeFileId.Exclaim);
					output.Write("\" alt=\"\"><span class=\"errInfo\">");
				}
				else
				{
					output.Write("<table class=tbWIB><tr><td>");
					base.OwaContext.UserContext.RenderThemeImage(output, ThemeFileId.Exclaim, "iei errInfo", new object[0]);
					output.Write("<span class=\"errInfo\">");
				}
				output.Write(LocalizedStrings.GetHtmlEncoded(437967712));
				output.Write("</span></td></tr></table>");
				return;
			}
			if (AttachmentPolicy.Level.ForceSave == levelForAttachment)
			{
				output.Write("<table class=tbNIB><tr><td class=\"msg ");
				output.Write(Utilities.GetTDClassForWebReadyViewHead(base.UserContext.IsBasicExperience));
				output.Write("\">");
				this.RenderHtmlEncodedSaveAttachmentToDiskMessage();
				output.Write("</td></tr></table>");
				return;
			}
			if (AttachmentPolicy.Level.Allow == levelForAttachment)
			{
				output.Write("<table class=tbNIB><tr><td class=\"msg ");
				output.Write(Utilities.GetTDClassForWebReadyViewHead(base.UserContext.IsBasicExperience));
				output.Write("\">");
				output.Write(base.UserContext.IsBasicExperience ? LocalizedStrings.GetHtmlEncoded(94137446) : LocalizedStrings.GetHtmlEncoded(2080319064));
				output.Write("</td></tr></table>");
			}
		}

		protected void RenderHtmlEncodedSaveAttachmentToDiskMessage()
		{
			if (base.UserContext.IsBasicExperience)
			{
				base.SanitizingResponse.Write(SanitizedHtmlString.FromStringId(-353246432));
				return;
			}
			Utilities.SanitizeHtmlEncode(base.GetSaveAttachmentToDiskMessage(687430467), base.SanitizingResponse);
		}

		protected void RenderVariableSaveAttachmentToDiskMessage()
		{
			string input = string.Empty;
			if (base.UserContext.IsBasicExperience)
			{
				input = LocalizedStrings.GetNonEncoded(-353246432);
			}
			else
			{
				input = base.GetSaveAttachmentToDiskMessage(687430467);
			}
			RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "a_sL2Aw", input);
		}

		protected void RenderOpenLink(TextWriter output)
		{
			this.utilities.RenderOpenLink(output);
		}

		protected bool IsSupportPaging
		{
			get
			{
				return this.utilities.IsSupportPaging;
			}
		}

		protected string FileName
		{
			get
			{
				return this.utilities.FileName;
			}
		}

		protected bool HasIrmError
		{
			get
			{
				return this.decryptionStatus.Failed;
			}
		}

		private WebReadyViewUtilities utilities;

		private RightsManagedMessageDecryptionStatus decryptionStatus;
	}
}
