using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class MessageListViewContents : ListViewContents
	{
		public MessageListViewContents(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, bool showFolderNameTooltip, UserContext userContext) : base(viewDescriptor, sortedColumn, sortOrder, showFolderNameTooltip, userContext)
		{
			base.AddProperty(MessageItemSchema.IsRead);
			base.AddProperty(MessageItemSchema.IsDraft);
			base.AddProperty(ItemSchema.IconIndex);
			base.AddProperty(MessageItemSchema.MessageInConflict);
			base.AddProperty(MessageItemSchema.HasBeenSubmitted);
			base.AddProperty(ItemSchema.Id);
			base.AddProperty(StoreObjectSchema.ItemClass);
		}

		protected override bool RenderItemRowStyle(TextWriter writer, int itemIndex)
		{
			bool flag = true;
			object itemProperty = base.DataSource.GetItemProperty(itemIndex, MessageItemSchema.IsRead);
			if (itemProperty is bool)
			{
				flag = (bool)itemProperty;
			}
			if (!flag)
			{
				writer.Write(" style=\"font-weight:bold;\"");
				return true;
			}
			return false;
		}

		protected override bool IsItemForCompose(int itemIndex)
		{
			string itemPropertyString = base.DataSource.GetItemPropertyString(itemIndex, StoreObjectSchema.ItemClass);
			if (!ObjectClass.IsMessage(itemPropertyString, false) && !ObjectClass.IsMeetingMessage(itemPropertyString))
			{
				return false;
			}
			bool itemPropertyBool = base.DataSource.GetItemPropertyBool(itemIndex, MessageItemSchema.HasBeenSubmitted, true);
			bool itemPropertyBool2 = base.DataSource.GetItemPropertyBool(itemIndex, MessageItemSchema.IsDraft, false);
			return itemPropertyBool2 && !itemPropertyBool;
		}

		protected override bool RenderIcon(TextWriter writer, int itemIndex)
		{
			string itemClass = base.DataSource.GetItemProperty(itemIndex, StoreObjectSchema.ItemClass) as string;
			int itemPropertyInt = base.DataSource.GetItemPropertyInt(itemIndex, ItemSchema.IconIndex, -1);
			bool itemPropertyBool = base.DataSource.GetItemPropertyBool(itemIndex, MessageItemSchema.MessageInConflict, false);
			bool itemPropertyBool2 = base.DataSource.GetItemPropertyBool(itemIndex, MessageItemSchema.IsRead, false);
			return ListViewContentsRenderingUtilities.RenderMessageIcon(writer, base.UserContext, itemClass, itemPropertyBool2, itemPropertyBool, itemPropertyInt);
		}
	}
}
