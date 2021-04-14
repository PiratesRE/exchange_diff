using System;
using System.Collections;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class AttachmentManager : OwaForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (base.OwaContext.FormsRegistryContext.Action != null)
			{
				this.action = base.OwaContext.FormsRegistryContext.Action;
			}
			if (string.IsNullOrEmpty(this.action))
			{
				this.messageIdString = Utilities.GetQueryStringParameter(base.Request, "id", true);
				this.changeKeyString = null;
			}
			else
			{
				this.messageIdString = Utilities.GetFormParameter(base.Request, "hidid", true);
				this.changeKeyString = Utilities.GetFormParameter(base.Request, "hidchk", true);
			}
			this.GetItem();
			string a;
			if ((a = this.action) != null)
			{
				if (!(a == "Add"))
				{
					if (a == "Del")
					{
						if (Utilities.GetFormParameter(base.Request, "dLst", false) != null)
						{
							this.RemoveAttachments();
						}
					}
				}
				else
				{
					this.AddAttachments();
				}
			}
			this.attachmentList = AttachmentWell.GetAttachmentInformation(base.Item, base.AttachmentLinks, base.UserContext.IsPublicLogon);
			if (base.Item is Contact)
			{
				OwaForm.RemoveContactPhoto(this.attachmentList);
			}
			CalendarItemBaseData userContextData = EditCalendarItemHelper.GetUserContextData(base.UserContext);
			if (userContextData != null && userContextData.Id != null && !string.IsNullOrEmpty("hidchk") && userContextData.Id.Equals(base.Item.Id.ObjectId) && userContextData.ChangeKey != this.changeKeyString)
			{
				userContextData.ChangeKey = this.changeKeyString;
			}
			this.levelOneFound = AttachmentUtility.IsLevelOneAndBlock(this.attachmentList);
		}

		private void GetItem()
		{
			if (this.changeKeyString != null && this.changeKeyString.Length > 0)
			{
				base.Item = Utilities.GetItem<Item>(base.UserContext, this.messageIdString, this.changeKeyString, new PropertyDefinition[0]);
				return;
			}
			base.Item = Utilities.GetItem<Item>(base.UserContext, this.messageIdString, new PropertyDefinition[0]);
			this.changeKeyString = base.Item.Id.ChangeKeyAsBase64String();
		}

		public void RenderAttachmentList()
		{
			AttachmentWellInfo attachmentWellInfo = null;
			ArrayList previousAttachmentDisplayNames = new ArrayList();
			base.SanitizingResponse.Write("<form name=\"delFrm\" action=\"?ae=Dialog&t=Attach&a=Del\" method=\"POST\">");
			base.SanitizingResponse.Write("<input type=\"hidden\" name=\"{0}\" value=\"{1}\">", "hidid", this.messageIdString);
			base.SanitizingResponse.Write("<input type=\"hidden\" name=\"{0}\" value=\"{1}\">", "hid64bitmsgid", Utilities.UrlEncode(this.messageIdString));
			base.SanitizingResponse.Write("<input type=\"hidden\" name=\"{0}\" value=\"{1}\">", "hidchk", this.changeKeyString);
			base.SanitizingResponse.Write("<div id=\"{0}\">", "lstArId");
			base.SanitizingResponse.Write("<table cellpadding=1 cellspacing=0 border=\"0\" class=\"attchList\"><caption>");
			base.SanitizingResponse.Write(LocalizedStrings.GetNonEncoded(573876176));
			base.SanitizingResponse.Write("</caption><tr>");
			base.SanitizingResponse.Write("<th class=\"lftcrnr\"><img src=\"");
			base.SanitizingResponse.Write(base.UserContext.GetThemeFileUrl(ThemeFileId.Clear));
			base.SanitizingResponse.Write("\" alt=\"\" class=\"invimg\"></th>");
			base.SanitizingResponse.Write("<th class=\"chkbx\">");
			base.SanitizingResponse.Write("<input type=\"checkbox\" name=\"chkAll\" id=\"chkAll\" onclick=\"chkAttchAll();\" alt=\"");
			base.SanitizingResponse.Write(LocalizedStrings.GetNonEncoded(-288583276));
			base.SanitizingResponse.Write("\"></th>");
			base.SanitizingResponse.Write("<th>");
			base.SanitizingResponse.Write(LocalizedStrings.GetNonEncoded(796893232));
			base.SanitizingResponse.Write("</th>");
			base.SanitizingResponse.Write("<th align=\"right\" class=\"sze\">");
			base.SanitizingResponse.Write(LocalizedStrings.GetNonEncoded(-837446919));
			base.SanitizingResponse.Write("</th>");
			base.SanitizingResponse.Write("<th class=\"rtcrnr\"><img src=\"");
			base.SanitizingResponse.Write(base.UserContext.GetThemeFileUrl(ThemeFileId.Clear));
			base.SanitizingResponse.Write("\" alt=\"\" class=\"invimg\"></th>");
			base.SanitizingResponse.Write("</tr>");
			if (this.attachmentList.Count > 0)
			{
				string value = null;
				for (int i = 0; i < this.attachmentList.Count; i++)
				{
					string arg;
					if (i == 0)
					{
						arg = "class=\"frst\"";
						value = "frst ";
					}
					else
					{
						arg = null;
						value = null;
					}
					attachmentWellInfo = (this.attachmentList[i] as AttachmentWellInfo);
					base.SanitizingResponse.Write("<tr>");
					base.SanitizingResponse.Write("<td {0} style=\"padding:0 0 0 0;\"><img src=\"{1}\" alt=\"\" class=\"invimg\"></td>", arg, base.UserContext.GetThemeFileUrl(ThemeFileId.Clear));
					base.SanitizingResponse.Write("<td {0}><input type=checkbox name=\"{1}\" id=\"{1}\" value=\"{2}\" onclick=\"onClkChkBx(this);\"></td>", arg, "dLst", attachmentWellInfo.AttachmentId.ToBase64String());
					base.SanitizingResponse.Write("<td {0}>", arg);
					bool flag = false;
					if (attachmentWellInfo.AttachmentType == AttachmentType.EmbeddedMessage)
					{
						using (Attachment attachment = attachmentWellInfo.OpenAttachment())
						{
							ItemAttachment itemAttachment = attachment as ItemAttachment;
							if (itemAttachment != null)
							{
								using (Item item = itemAttachment.GetItem())
								{
									flag = true;
									AttachmentWell.RenderAttachmentLinkForItem(base.SanitizingResponse, attachmentWellInfo, item, this.messageIdString, base.UserContext, previousAttachmentDisplayNames, AttachmentWell.AttachmentWellFlags.None, false);
								}
							}
						}
					}
					if (!flag)
					{
						AttachmentWell.RenderAttachmentLink(base.SanitizingResponse, AttachmentWellType.ReadWrite, attachmentWellInfo, this.messageIdString, base.UserContext, previousAttachmentDisplayNames, AttachmentWell.AttachmentWellFlags.None, false);
					}
					base.SanitizingResponse.Write("</td>");
					base.SanitizingResponse.Write("<td colspan=2 class=\"");
					base.SanitizingResponse.Write(value);
					base.SanitizingResponse.Write("sze\">");
					double num = 0.0;
					if (attachmentWellInfo.Size > 0L)
					{
						num = (double)attachmentWellInfo.Size / 1024.0 / 1024.0;
						num = Math.Round(num, 2);
						this.attachUsage += num;
					}
					if (num == 0.0)
					{
						base.SanitizingResponse.Write(" < 0.01");
					}
					else
					{
						base.SanitizingResponse.Write(num);
					}
					base.SanitizingResponse.Write(" MB</td>");
					base.SanitizingResponse.Write("</tr>");
				}
			}
			else
			{
				base.SanitizingResponse.Write("<tr>");
				base.SanitizingResponse.Write("<td colspan=5 class=\"noattach\" nowrap>");
				base.SanitizingResponse.Write(LocalizedStrings.GetNonEncoded(-1907299050));
				base.SanitizingResponse.Write("</td>");
				base.SanitizingResponse.Write("</tr>");
			}
			base.SanitizingResponse.Write("</table>");
			base.SanitizingResponse.Write("</div>");
			Utilities.RenderCanaryHidden(base.SanitizingResponse, base.UserContext);
			base.SanitizingResponse.Write("</form>");
		}

		private void AddAttachments()
		{
			InfobarMessageType type = InfobarMessageType.Error;
			AttachmentAddResult attachmentAddResult = AttachmentUtility.AddAttachment(base.Item, base.Request.Files, base.UserContext);
			if (attachmentAddResult.ResultCode != AttachmentAddResultCode.NoError)
			{
				base.Infobar.AddMessageHtml(attachmentAddResult.Message, type);
			}
			base.Item.Load();
			if (attachmentAddResult.ResultCode != AttachmentAddResultCode.IrresolvableConflictWhenSaving)
			{
				this.changeKeyString = base.Item.Id.ChangeKeyAsBase64String();
			}
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.ItemsUpdated.Increment();
			}
		}

		private void RemoveAttachments()
		{
			char[] separator = ",".ToCharArray();
			ArrayList arrayList = new ArrayList();
			string[] array = base.Request.Form["dLst"].Split(separator);
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				AttachmentId attachmentId = AttachmentId.Deserialize(array[i]);
				if (base.Item.AttachmentCollection.Contains(attachmentId))
				{
					arrayList.Add(attachmentId);
				}
			}
			if (arrayList.Count > 0)
			{
				AttachmentUtility.RemoveAttachment(base.Item, arrayList);
				base.Item.Save(SaveMode.ResolveConflicts);
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.ItemsUpdated.Increment();
				}
			}
			base.Item.Load();
			this.changeKeyString = base.Item.Id.ChangeKeyAsBase64String();
		}

		protected void RenderNavigation()
		{
			NavigationModule navigationModule = (base.Item.ClassName == "IPM.Contact") ? NavigationModule.Contacts : NavigationModule.Mail;
			Navigation navigation = new Navigation(navigationModule, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		protected void RenderAttachmentForm()
		{
			base.SanitizingResponse.Write("<div><form name=\"addFrm\" action=\"?ae=Dialog&t=Attach&a=Add\" enctype=\"multipart/form-data\" method=\"POST\">");
			base.SanitizingResponse.Write("<table cellpadding=4 class=\"attchfrm\"><caption>");
			base.SanitizingResponse.Write(LocalizedStrings.GetNonEncoded(-1286941817));
			base.SanitizingResponse.Write("</caption><tr><td class=\"txt\">");
			base.SanitizingResponse.Write("<input type=\"hidden\" name=\"{0}\" value=\"{1}\">", "hidid", this.messageIdString);
			base.SanitizingResponse.Write("<input type=\"hidden\" name=\"{0}\" value=\"{1}\">", "hidchk", this.changeKeyString);
			base.SanitizingResponse.Write(LocalizedStrings.GetNonEncoded(935002604), "<b>", "</b>");
			base.SanitizingResponse.Write("</td></tr>");
			base.SanitizingResponse.Write("<tr><td align=\"right\">");
			base.SanitizingResponse.Write("<input type=\"file\" size=\"35\" name=\"attach\" id=\"attach\" alt=\"Attachment\">");
			base.SanitizingResponse.Write("</td></tr>");
			base.SanitizingResponse.Write("<tr><td align=\"right\">");
			base.SanitizingResponse.Write("<input class=\"btn\" type=\"button\" name=\"attachbtn\" id=\"attachbtn\" alt=\"");
			base.SanitizingResponse.Write(LocalizedStrings.GetNonEncoded(-60366385));
			base.SanitizingResponse.Write("\" value=\"");
			base.SanitizingResponse.Write(LocalizedStrings.GetNonEncoded(-547159221));
			base.SanitizingResponse.Write("\" onclick=\"durUp('");
			base.SanitizingResponse.Write("lstArId");
			base.SanitizingResponse.Write("','remove');\">");
			base.SanitizingResponse.Write("</td></tr>");
			base.SanitizingResponse.Write("</table>");
			Utilities.RenderCanaryHidden(base.SanitizingResponse, base.UserContext);
			base.SanitizingResponse.Write("</form></div>");
		}

		protected void RenderAttachmentToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			toolbar.RenderButton(ToolbarButtons.Done);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.Remove, ToolbarButtonFlags.Sticky);
			toolbar.RenderFill();
			toolbar.RenderButton(ToolbarButtons.CloseImage);
			toolbar.RenderEnd();
		}

		protected void RenderInfoBar()
		{
			if (this.levelOneFound)
			{
				base.Infobar.AddMessageLocalized(-2118248931, InfobarMessageType.Informational);
			}
			base.Infobar.Render(base.SanitizingResponse);
		}

		protected void RenderAttachmentListFooter()
		{
			Toolbar toolbar = new Toolbar(base.SanitizingResponse, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			toolbar.RenderEnd();
		}

		protected override bool ShowInfobar
		{
			get
			{
				return base.ShowInfobar || this.levelOneFound;
			}
		}

		protected string UrlEncodedMessageId
		{
			get
			{
				return HttpUtility.UrlEncode(this.messageIdString);
			}
		}

		protected string MessageIdString
		{
			get
			{
				return this.messageIdString;
			}
		}

		protected string MessageIdString64bitEconded
		{
			get
			{
				return Utilities.UrlEncode(this.messageIdString);
			}
		}

		protected static string PostMessageId
		{
			get
			{
				return "hidid";
			}
		}

		protected override string ItemType
		{
			get
			{
				return base.Item.ClassName;
			}
		}

		private const string DeleteList = "dLst";

		private const string Add = "Add";

		private const string Del = "Del";

		private const string ChangeKey = "hidchk";

		private const string ListAreaId = "lstArId";

		private const string PostMessageIdString = "hidid";

		private const string Hid64bitMessageId = "hid64bitmsgid";

		private const int Limit = 20;

		private string messageIdString;

		private string changeKeyString;

		private double attachUsage;

		private ArrayList attachmentList;

		private bool levelOneFound;

		private string action;
	}
}
