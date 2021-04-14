using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.Configuration;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public abstract class EditItemForm : OwaForm, IRegistryOnlyForm
	{
		internal EditItemForm()
		{
		}

		internal EditItemForm(bool setNoCacheNoStore) : base(setNoCacheNoStore)
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "exdltdrft", false);
			if (!string.IsNullOrEmpty(queryStringParameter))
			{
				this.DeleteExistingDraft = true;
			}
			string queryStringParameter2 = Utilities.GetQueryStringParameter(base.Request, "fId", false);
			if (queryStringParameter2 != null)
			{
				this.targetFolderId = OwaStoreObjectId.CreateFromString(queryStringParameter2);
			}
			if (base.Item == null)
			{
				string queryStringParameter3 = Utilities.GetQueryStringParameter(base.Request, "email", false);
				if (!string.IsNullOrEmpty(queryStringParameter3))
				{
					StoreObjectId mailboxItemStoreObjectId = null;
					if (MailToParser.TryParseMailTo(queryStringParameter3, base.UserContext, out mailboxItemStoreObjectId))
					{
						base.OwaContext.PreFormActionId = OwaStoreObjectId.CreateFromMailboxItemId(mailboxItemStoreObjectId);
						this.DeleteExistingDraft = true;
					}
				}
			}
			string queryStringParameter4 = Utilities.GetQueryStringParameter(base.Request, "fRABcc", false);
			this.isReplyAllBcc = (0 == string.CompareOrdinal("1", queryStringParameter4));
		}

		protected bool DeleteExistingDraft
		{
			get
			{
				return this.deleteExistingDraft;
			}
			set
			{
				this.deleteExistingDraft = value;
			}
		}

		internal OwaStoreObjectId TargetFolderId
		{
			get
			{
				return this.targetFolderId;
			}
		}

		protected bool IsTargetFolderIdNull
		{
			get
			{
				return this.TargetFolderId == null;
			}
		}

		protected override bool IsPublicItem
		{
			get
			{
				if (base.Item != null)
				{
					return base.IsPublicItem;
				}
				return !this.IsTargetFolderIdNull && this.TargetFolderId.IsPublic;
			}
		}

		protected override bool IsOtherMailboxItem
		{
			get
			{
				if (base.Item != null)
				{
					return base.IsOtherMailboxItem;
				}
				return !this.IsTargetFolderIdNull && this.TargetFolderId.IsOtherMailbox;
			}
		}

		protected EditorContextMenu EditorContextMenu
		{
			get
			{
				if (this.editorContextMenu == null)
				{
					this.editorContextMenu = new EditorContextMenu(base.UserContext);
				}
				return this.editorContextMenu;
			}
		}

		protected virtual ResizeImageMenu ResizeImageMenu
		{
			get
			{
				if (this.resizeImageMenu == null)
				{
					this.resizeImageMenu = new ResizeImageMenu(base.UserContext);
				}
				return this.resizeImageMenu;
			}
		}

		protected bool IsReplyAllBcc
		{
			get
			{
				return this.isReplyAllBcc;
			}
		}

		protected virtual void RenderJavaScriptEncodedTargetFolderId()
		{
			if (this.TargetFolderId != null)
			{
				Utilities.JavascriptEncode(this.TargetFolderId.ToBase64String(), base.Response.Output);
			}
		}

		protected void RenderDialogHelper()
		{
			base.SanitizingResponse.Write("<div class=\"offscreen\">");
			base.SanitizingResponse.Write("<object id=\"dlgHelper\" classid=\"clsid:3050f819-98b5-11cf-bb82-00aa00bdce0b\" width=\"0px\" height=\"0px\" viewastext></object>");
			base.SanitizingResponse.Write("</div>");
		}

		protected void RenderSilverlightAttachmentManagerControl()
		{
			if (base.IsSilverlightEnabled)
			{
				int num = (base.UserContext.BrowserPlatform != BrowserPlatform.Macintosh) ? 0 : 1;
				int height = num;
				base.RenderSilverlight("AttachmentManager", "sl_attMgr", num, height, "<span></span>");
			}
		}

		protected void RenderDataNeededBySilverlightAttachmentManager()
		{
			if (base.IsSilverlightEnabled)
			{
				RenderingUtilities.RenderSmallIconTable(base.SanitizingResponse, true);
				RenderingUtilities.RenderSmallIconTable(base.SanitizingResponse, false);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_OpenParen", 6409762);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_CloseParen", -1023695022);
				RenderingUtilities.RenderWebReadyPolicy(base.SanitizingResponse, base.UserContext);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "a_sWRDV", AttachmentWell.GetWebReadyLink(base.UserContext));
				RenderingUtilities.RenderAttachmentBlockingPolicy(base.SanitizingResponse, base.UserContext, false);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_WrnLevelOneReadWrite", -2118248931);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "a_sMBG", base.UserContext.ExchangePrincipal.MailboxInfo.MailboxGuid.ToString());
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_ErrAttSilverlightFailure", 1330586559);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_ErrCannotConnectToEwsRetry", -158529231);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_ErrFileNotFound", 469568033);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_ErrFileAlreadyInUse", -1934316340);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_ErrAttTooLrg", -178989031);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_ErrSingleAttTooLrg", 1582744855);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_AttNmDiv", 440361970);
				this.RenderMaximumRequestLength(base.SanitizingResponse);
				this.RenderMaximumUserMessageSize(base.UserContext, base.SanitizingResponse);
				RenderingUtilities.RenderInteger(base.SanitizingResponse, "a_iEMOEA", 65536);
				RenderingUtilities.RenderInteger(base.SanitizingResponse, "a_iMaxAtt", 499);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_ErrAttTooMany", SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(1025276934), new object[]
				{
					499
				}));
				int input = 0;
				if (base.Item != null)
				{
					AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(base.Item, false, base.UserContext);
					input = AttachmentUtility.GetTotalAttachmentSize(attachmentCollection);
				}
				RenderingUtilities.RenderInteger(base.SanitizingResponse, "a_sl_iTotalAttachmentSize", input);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_ImgFileTypes", -16092782);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_FileTypesSep", 952162300);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_InsertingInlineImage", -242321595);
				RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "L_ErrNonImageFile", -1293887935);
				this.RenderDelegateMailboxInfo();
			}
		}

		private void RenderDelegateMailboxInfo()
		{
			if (base.Item != null && base.UserContext != null)
			{
				MailboxSession mailboxSession = base.Item.Session as MailboxSession;
				if (mailboxSession != null)
				{
					IExchangePrincipal mailboxOwner = mailboxSession.MailboxOwner;
					if (mailboxOwner != null && mailboxOwner != base.UserContext.ExchangePrincipal)
					{
						RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "a_sDelegateMailboxGuid", mailboxOwner.MailboxInfo.MailboxGuid.ToString());
						byte[] bytes = Encoding.UTF8.GetBytes(mailboxOwner.LegacyDn);
						RenderingUtilities.RenderStringVariable(base.SanitizingResponse, "a_sDelegateMailboxLegacyDN", Convert.ToBase64String(bytes));
					}
				}
			}
		}

		protected void RenderMaximumRequestLength(TextWriter writer)
		{
			int input = 4194304;
			System.Configuration.Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
			HttpRuntimeSection httpRuntimeSection = configuration.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
			if (httpRuntimeSection != null)
			{
				input = httpRuntimeSection.MaxRequestLength * 1024;
			}
			RenderingUtilities.RenderInteger(writer, "a_iMaxRequestLength", input);
		}

		protected void RenderMaximumUserMessageSize(UserContext userContext, TextWriter writer)
		{
			int? maximumMessageSize = Utilities.GetMaximumMessageSize(userContext);
			if (maximumMessageSize == null)
			{
				return;
			}
			RenderingUtilities.RenderInteger(writer, "a_iMUMS", maximumMessageSize.Value / 1024);
		}

		protected void RenderEditorIframe(string className)
		{
			this.RenderIframe("ifBdy", className);
		}

		private void RenderIframe(string id, string className)
		{
			base.SanitizingResponse.Write("<iframe id=\"");
			base.SanitizingResponse.Write(id);
			base.SanitizingResponse.Write("\" ");
			base.SanitizingResponse.Write("class=");
			base.SanitizingResponse.Write("'");
			base.SanitizingResponse.Write(className);
			base.SanitizingResponse.Write("'");
			base.SanitizingResponse.Write(" frameborder=\"0\" src=\"");
			base.UserContext.RenderBlankPage(Utilities.PremiumScriptPath, base.SanitizingResponse);
			base.SanitizingResponse.Write("\"></iframe>");
		}

		private const string FolderIDParameterName = "fId";

		private const string ReplyAllBccParameterName = "fRABcc";

		protected const string OpenAction = "Open";

		private OwaStoreObjectId targetFolderId;

		private bool deleteExistingDraft;

		private EditorContextMenu editorContextMenu;

		private ResizeImageMenu resizeImageMenu;

		private bool isReplyAllBcc;
	}
}
