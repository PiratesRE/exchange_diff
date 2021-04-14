using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class EditDistributionList : EditItemForm, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			ExTraceGlobals.ContactsCallTracer.TraceDebug((long)this.GetHashCode(), "EditDistributionList.OnLoad");
			base.OnLoad(e);
			this.distributionList = base.Initialize<DistributionList>(false, EditDistributionList.prefetchProperties);
			this.listView = new DistributionListMemberListView(base.UserContext, this.distributionList, ColumnId.MemberDisplayName, SortOrder.Ascending);
			if (this.distributionList != null)
			{
				InfobarMessageBuilder.AddFlag(this.infobar, base.Item, base.UserContext);
			}
			this.toolbar = new EditDistributionListToolbar(this.distributionList);
			this.recipientWell = new MessageRecipientWell();
		}

		protected RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		protected bool IsDLNull
		{
			get
			{
				return this.distributionList == null;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected void RenderMemberListView()
		{
			this.listView.Render(base.Response.Output, ListView.RenderFlags.Behavior | ListView.RenderFlags.Contents | ListView.RenderFlags.Headers);
		}

		protected void RenderNotes()
		{
			BodyConversionUtilities.RenderMeetingPlainTextBody(base.Response.Output, base.Item, base.UserContext, false);
		}

		protected void RenderAddMemberToListButton()
		{
			RenderingUtilities.RenderButton(base.Response.Output, "btnAddToLst", string.Empty, string.Empty, "&nbsp;&nbsp;" + LocalizedStrings.GetHtmlEncoded(-100912444) + "&nbsp;&nbsp;");
		}

		protected void RenderRemoveButton()
		{
			RenderingUtilities.RenderButton(base.Response.Output, "btnRmv", string.Empty, "onBtnRmv()", LocalizedStrings.GetHtmlEncoded(1967506178));
		}

		protected void RenderMembersButton()
		{
			RenderingUtilities.RenderButton(base.Response.Output, "btnMmb", string.Empty, string.Empty, LocalizedStrings.GetHtmlEncoded(-878744874));
		}

		protected string DistributionListName
		{
			get
			{
				if (!this.IsDLNull)
				{
					return Utilities.HtmlEncode(this.distributionList.DisplayName);
				}
				return string.Empty;
			}
		}

		protected Toolbar Toolbar
		{
			get
			{
				return this.toolbar;
			}
		}

		protected void RenderCategories()
		{
			if (base.Item != null)
			{
				CategorySwatch.RenderCategories(base.OwaContext, base.SanitizingResponse, base.Item);
			}
		}

		protected void RenderCategoriesJavascriptArray()
		{
			CategorySwatch.RenderCategoriesJavascriptArray(base.SanitizingResponse, base.Item);
		}

		protected void RenderTitle()
		{
			if (this.distributionList == null)
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-1576800949));
				return;
			}
			string displayName = this.distributionList.DisplayName;
			if (string.IsNullOrEmpty(displayName))
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-1576800949));
				return;
			}
			Utilities.HtmlEncode(displayName, base.Response.Output);
		}

		protected static int StoreObjectTypeDistributionList
		{
			get
			{
				return 18;
			}
		}

		private static readonly PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
		{
			ItemSchema.Categories,
			ItemSchema.FlagStatus,
			ItemSchema.FlagCompleteTime,
			MessageItemSchema.ReplyTime,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.ReminderDueBy,
			ItemSchema.ReminderIsSet,
			StoreObjectSchema.EffectiveRights
		};

		private Infobar infobar = new Infobar();

		private MessageRecipientWell recipientWell;

		private Toolbar toolbar;

		private DistributionList distributionList;

		private ListView listView;
	}
}
