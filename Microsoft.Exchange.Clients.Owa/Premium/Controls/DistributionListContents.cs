using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class DistributionListContents : LegacySingleLineItemList
	{
		protected override bool IsRenderColumnShadow
		{
			get
			{
				return false;
			}
		}

		protected override string ListViewStyle
		{
			get
			{
				return "dlIL";
			}
		}

		public DistributionListContents(UserContext userContext, ViewDescriptor viewDescriptor) : base(viewDescriptor, ColumnId.MemberDisplayName, SortOrder.Ascending, userContext)
		{
			base.AddProperty(ItemSchema.Id);
			base.AddProperty(RecipientSchema.EmailAddrType);
			base.AddProperty(ParticipantSchema.OriginItemId);
		}

		private static void RenderAttributes(TextWriter writer, string name, string value)
		{
			writer.Write(" ");
			writer.Write(name);
			writer.Write("=\"");
			Utilities.HtmlEncode(value, writer);
			writer.Write("\"");
		}

		protected override void RenderItemMetaDataExpandos(TextWriter writer)
		{
			AddressOrigin itemProperty = this.DataSource.GetItemProperty<AddressOrigin>(ItemSchema.RecipientType, AddressOrigin.Unknown);
			int itemProperty2 = this.DataSource.GetItemProperty<int>(RecipientSchema.EmailAddrType, 0);
			DistributionListContents.RenderAttributes(writer, "_dn", this.DataSource.GetItemProperty<string>(StoreObjectSchema.DisplayName, string.Empty));
			DistributionListContents.RenderAttributes(writer, "_em", this.DataSource.GetItemProperty<string>(ParticipantSchema.EmailAddress, string.Empty));
			DistributionListContents.RenderAttributes(writer, "_rt", this.DataSource.GetItemProperty<string>(ParticipantSchema.RoutingType, string.Empty));
			string name = "_ao";
			int num = (int)itemProperty;
			DistributionListContents.RenderAttributes(writer, name, num.ToString());
			if (itemProperty == AddressOrigin.Store)
			{
				DistributionListContents.RenderAttributes(writer, "_id", this.DataSource.GetItemProperty<string>(ParticipantSchema.OriginItemId, string.Empty));
				if (itemProperty2 != 0)
				{
					DistributionListContents.RenderAttributes(writer, "_ei", itemProperty2.ToString());
				}
			}
			base.RenderItemMetaDataExpandos(writer);
		}

		protected override void RenderTableCellAttributes(TextWriter writer, ColumnId columnId)
		{
			string text = null;
			if (columnId == ColumnId.MemberDisplayName)
			{
				text = this.DataSource.GetItemProperty<string>(StoreObjectSchema.DisplayName, null);
			}
			else if (columnId == ColumnId.MemberEmail)
			{
				Participant itemProperty = this.DataSource.GetItemProperty<Participant>(ContactSchema.Email1, null);
				if (itemProperty != null)
				{
					string text2;
					ContactUtilities.GetParticipantEmailAddress(itemProperty, out text, out text2);
				}
			}
			if (text != null)
			{
				DistributionListContents.RenderAttributes(writer, "title", text);
			}
		}

		protected override bool RenderIcon(TextWriter writer)
		{
			bool result = true;
			string itemClass;
			if ((itemClass = this.DataSource.GetItemClass()) != null)
			{
				if (itemClass == "IPM.DistList" || itemClass == "AD.RecipientType.Group")
				{
					base.UserContext.RenderThemeImage(writer, ThemeFileId.DistributionListOther);
					return result;
				}
				if (itemClass == "IPM.Contact")
				{
					base.UserContext.RenderThemeImage(writer, ThemeFileId.Contact);
					return result;
				}
				if (itemClass == "AD.RecipientType.User" || itemClass == "OneOff")
				{
					base.UserContext.RenderThemeImage(writer, ThemeFileId.DistributionListUser);
					return result;
				}
			}
			writer.Write("<img src=\"\">");
			return result;
		}
	}
}
