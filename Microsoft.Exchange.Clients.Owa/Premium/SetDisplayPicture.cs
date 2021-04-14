using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class SetDisplayPicture : OwaPage, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			if (base.Request.HttpMethod == "POST")
			{
				string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "a", false);
				if (!string.IsNullOrEmpty(queryStringParameter))
				{
					if (queryStringParameter.Equals("upload") && base.Request.Files.Count > 0)
					{
						HttpPostedFile file = base.Request.Files[0];
						this.setDisplayPictureResult = DisplayPictureUtility.UploadDisplayPicture(file, base.UserContext);
						this.currentState = ChangePictureDialogState.Upload;
						return;
					}
					if (queryStringParameter.Equals("clear"))
					{
						this.setDisplayPictureResult = DisplayPictureUtility.ClearDisplayPicture(base.UserContext);
						this.currentState = ChangePictureDialogState.Save;
						return;
					}
					if (queryStringParameter.Equals("change"))
					{
						this.setDisplayPictureResult = DisplayPictureUtility.SaveDisplayPicture(base.UserContext);
						this.currentState = ChangePictureDialogState.Save;
						return;
					}
				}
			}
			else
			{
				base.UserContext.UploadedDisplayPicture = null;
			}
		}

		protected override void OnError(EventArgs e)
		{
			Exception lastError = base.Server.GetLastError();
			if (lastError is HttpException)
			{
				base.Server.ClearError();
				Utilities.TransferToErrorPage(base.OwaContext, LocalizedStrings.GetNonEncoded(-1440270008));
				return;
			}
			base.OnError(e);
		}

		protected bool IsClosingWindow()
		{
			return this.currentState == ChangePictureDialogState.Save && this.setDisplayPictureResult.ResultCode == SetDisplayPictureResultCode.NoError;
		}

		protected void RenderTitle()
		{
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(-1336564560));
		}

		protected void RenderDescription()
		{
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(-864891586));
		}

		protected void RenderCurrentPictureTitle()
		{
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(765872725));
		}

		protected void RenderClearPictureTitle()
		{
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(-984343499));
		}

		protected void RenderClearPictureNote()
		{
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(2047483241));
		}

		protected void RenderChangeLink()
		{
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(641627222));
		}

		protected void RenderFileUploaded()
		{
			if (base.UserContext.UploadedDisplayPicture != null)
			{
				base.SanitizingResponse.Write("1");
			}
		}

		protected void RenderErrorInfobar()
		{
			if (this.setDisplayPictureResult.ResultCode != SetDisplayPictureResultCode.NoError)
			{
				Infobar infobar = new Infobar();
				infobar.AddMessage(this.setDisplayPictureResult.ErrorMessage, InfobarMessageType.Error, "divSetPicErr");
				infobar.Render(base.Response.Output);
			}
		}

		protected void RenderFormAction()
		{
			base.SanitizingResponse.Write("?ae=Dialog&t=SetDisplayPicture");
		}

		protected void RenderPicture(bool showDoughboy)
		{
			if (!showDoughboy && base.UserContext.UploadedDisplayPicture != null)
			{
				RenderingUtilities.RenderDisplayPicture(base.Response.Output, base.UserContext, RenderingUtilities.GetADPictureUrl(string.Empty, string.Empty, base.UserContext, true, true), 64, true, ThemeFileId.DoughboyPerson);
				return;
			}
			string srcUrl = showDoughboy ? string.Empty : RenderingUtilities.GetADPictureUrl(base.UserContext.ExchangePrincipal.LegacyDn, "EX", base.UserContext, true);
			RenderingUtilities.RenderDisplayPicture(base.Response.Output, base.UserContext, srcUrl, 64, true, ThemeFileId.DoughboyPerson);
		}

		protected void RenderImageLargeHtml()
		{
			if (!string.IsNullOrEmpty(this.setDisplayPictureResult.ImageLargeHtml))
			{
				Utilities.JavascriptEncode(this.setDisplayPictureResult.ImageLargeHtml, base.Response.Output);
			}
		}

		protected void RenderImageSmallHtml()
		{
			if (!string.IsNullOrEmpty(this.setDisplayPictureResult.ImageSmallHtml))
			{
				Utilities.JavascriptEncode(this.setDisplayPictureResult.ImageSmallHtml, base.Response.Output);
			}
		}

		private SetDisplayPictureResult setDisplayPictureResult = SetDisplayPictureResult.NoError;

		private ChangePictureDialogState currentState;
	}
}
