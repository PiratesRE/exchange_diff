using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class AttachFileDialog : OwaPage, IRegistryOnlyForm
	{
		protected AttachmentAddResult AttachResult
		{
			get
			{
				return this.attachResult;
			}
		}

		public ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
			}
		}

		protected bool IsInline
		{
			get
			{
				return this.isInline;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			string action = base.OwaContext.FormsRegistryContext.Action;
			this.isInline = !string.IsNullOrEmpty(action);
			if (base.Request.HttpMethod == "POST")
			{
				this.ProcessPost();
			}
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.item != null)
			{
				this.item.Dispose();
				this.item = null;
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

		private void ProcessPost()
		{
			string text = base.Request.Form["mId"];
			string text2 = base.Request.Form["mCK"];
			string bodyMarkup = base.Request.Form["sHtmlBdy"];
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				this.item = Utilities.GetItem<Item>(base.UserContext, text, text2, new PropertyDefinition[0]);
				if (base.UserContext.IsIrmEnabled)
				{
					Utilities.IrmDecryptIfRestricted(this.item, base.UserContext, true);
				}
			}
			else
			{
				StoreObjectType itemType = StoreObjectType.Message;
				string text3 = base.Request.Form["iT"];
				int num;
				if (text3 != null && int.TryParse(text3, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
				{
					itemType = (StoreObjectType)num;
				}
				OwaStoreObjectId destinationFolderId = null;
				string text4 = base.Request.Form["sFldId"];
				if (!string.IsNullOrEmpty(text4))
				{
					destinationFolderId = OwaStoreObjectId.CreateFromString(text4);
				}
				this.item = Utilities.CreateImplicitDraftItem(itemType, destinationFolderId);
				this.item.Save(SaveMode.ResolveConflicts);
				this.item.Load();
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.ItemsCreated.Increment();
				}
			}
			this.attachmentLinks = new List<SanitizedHtmlString>(base.Request.Files.Count);
			if (!this.IsInline)
			{
				this.attachResult = AttachmentUtility.AddAttachment(this.item, base.Request.Files, base.UserContext, false, bodyMarkup);
			}
			else
			{
				this.attachResult = AttachmentUtility.AddAttachment(this.item, base.Request.Files, base.UserContext, true, bodyMarkup, out this.attachmentLinks);
			}
			this.item.Load();
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.ItemsUpdated.Increment();
			}
			if (!this.IsInline)
			{
				this.attachmentWellRenderObjects = null;
				bool isPublicLogon = base.UserContext.IsPublicLogon;
				this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(this.item, null, isPublicLogon);
			}
		}

		protected void RenderFormAction()
		{
			base.SanitizingResponse.Write("?ae=Dialog&t=AttachFileDialog");
			if (this.IsInline)
			{
				base.SanitizingResponse.Write("&a=InsertImage");
			}
		}

		protected void RenderInlineImageLinks()
		{
			base.SanitizingResponse.Write("new Array(");
			for (int i = 0; i < this.attachmentLinks.Count; i++)
			{
				if (i != 0)
				{
					base.SanitizingResponse.Write(",");
				}
				base.SanitizingResponse.Write("'");
				base.SanitizingResponse.Write(this.attachmentLinks[i]);
				base.SanitizingResponse.Write("'");
			}
			base.SanitizingResponse.Write(");");
		}

		protected void RenderImageAttachmentInfobar()
		{
			if (this.AttachResult.ResultCode != AttachmentAddResultCode.NoError)
			{
				new StringBuilder();
				new SanitizedHtmlString(string.Empty);
				Infobar infobar = new Infobar();
				infobar.AddMessage(this.AttachResult.Message, InfobarMessageType.Warning, AttachmentWell.AttachmentInfobarErrorHtmlTag);
				infobar.Render(base.SanitizingResponse);
			}
		}

		protected void RenderSizeAttribute()
		{
			BrowserPlatform browserPlatform = Utilities.GetBrowserPlatform(base.Request.UserAgent);
			BrowserType browserType = Utilities.GetBrowserType(base.Request.UserAgent);
			string empty = string.Empty;
			string name = Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture().Name;
			bool flag = name == "ja-JP" || name == "ko-KR" || name == "zh-CN" || name == "zh-TW" || name == "zh-HK" || name == "zh-MO" || name == "zh-SG";
			if (BrowserType.Firefox == browserType)
			{
				if (BrowserPlatform.Macintosh == browserPlatform)
				{
					if (flag)
					{
						base.Response.Write(" size=\"43\"");
						return;
					}
					base.Response.Write(" size=\"50\"");
					return;
				}
				else if (browserPlatform == BrowserPlatform.Windows)
				{
					if (flag)
					{
						base.Response.Write(" size=\"56\"");
						return;
					}
					base.Response.Write(" size=\"50\"");
				}
			}
		}

		protected void RenderJavascriptEncodedItemId()
		{
			if (this.item != null)
			{
				Utilities.JavascriptEncode(Utilities.GetIdAsString(this.item), base.Response.Output);
			}
		}

		protected void RenderJavascriptEncodedItemChangeKey()
		{
			if (this.item != null)
			{
				Utilities.JavascriptEncode(this.item.Id.ChangeKeyAsBase64String(), base.Response.Output);
			}
		}

		protected void RenderUpgradeToSilverlight()
		{
			SanitizedHtmlString sanitizedHtmlString = SanitizedHtmlString.Format("<a href=\"http://www.microsoft.com/silverlight/get-started/install/default.aspx\" target=_blank title=\"{0}\" class=\"updSL\">{1}</a>", new object[]
			{
				LocalizedStrings.GetNonEncoded(1095922679),
				LocalizedStrings.GetNonEncoded(1095922679)
			});
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(391987695), new object[]
			{
				sanitizedHtmlString
			}));
		}

		protected void RenderFileUploadButtonText()
		{
			Strings.IDs localizedId;
			Strings.IDs localizedId2;
			if (this.IsInline)
			{
				localizedId = 1319799963;
				localizedId2 = 695427503;
			}
			else
			{
				localizedId = -1952546783;
				localizedId2 = -547159221;
			}
			Strings.IDs localizedId3;
			if (base.UserContext.BrowserType == BrowserType.IE || base.UserContext.BrowserType == BrowserType.Firefox)
			{
				localizedId3 = 1698608150;
			}
			else
			{
				localizedId3 = 1368786137;
			}
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(1264365152), new object[]
			{
				LocalizedStrings.GetNonEncoded(localizedId),
				LocalizedStrings.GetNonEncoded(localizedId3),
				LocalizedStrings.GetNonEncoded(localizedId2)
			}));
		}

		protected void RenderDialogTitle()
		{
			if (this.IsInline)
			{
				base.SanitizingResponse.Write(SanitizedHtmlString.FromStringId(-553630704));
				return;
			}
			base.SanitizingResponse.Write(SanitizedHtmlString.FromStringId(-1551177844));
		}

		protected string GetButtonText()
		{
			Strings.IDs localizedID;
			if (this.IsInline)
			{
				localizedID = 695427503;
			}
			else
			{
				localizedID = -547159221;
			}
			return LocalizedStrings.GetHtmlEncoded(localizedID);
		}

		private const int CopyBufferSize = 32768;

		private ArrayList attachmentWellRenderObjects;

		private Item item;

		private AttachmentAddResult attachResult = AttachmentAddResult.NoError;

		private List<SanitizedHtmlString> attachmentLinks;

		private bool isInline;
	}
}
