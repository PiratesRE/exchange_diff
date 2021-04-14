using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class TaskSingleLineList : SingleLineItemContentsForVirtualList
	{
		public TaskSingleLineList(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext, SearchScope folderScope) : base(viewDescriptor, sortedColumn, sortOrder, userContext, folderScope)
		{
			base.AddProperty(ItemSchema.Id);
			base.AddProperty(StoreObjectSchema.ItemClass);
			base.AddProperty(ItemSchema.IsResponseRequested);
			base.AddProperty(TaskSchema.TaskType);
			base.AddProperty(TaskSchema.IsTaskRecurring);
			base.AddProperty(TaskSchema.RecurrenceType);
			base.AddProperty(ItemSchema.IconIndex);
			base.AddProperty(MessageItemSchema.IsRead);
			base.AddProperty(MessageItemSchema.ReceivedRepresentingEmailAddress);
			base.AddProperty(CalendarItemBaseSchema.OrganizerEmailAddress);
			base.AddProperty(StoreObjectSchema.IsRestricted);
			base.AddProperty(MessageItemSchema.DRMRights);
		}

		protected override bool IsAssignedTask
		{
			get
			{
				TaskType itemProperty = (TaskType)this.DataSource.GetItemProperty<int>(TaskSchema.TaskType, 0);
				return TaskUtilities.IsAssignedTaskType(itemProperty);
			}
		}

		protected override void RenderItemMetaDataExpandos(TextWriter writer)
		{
			base.RenderItemMetaDataExpandos(writer);
			if (this.IsAssignedTask)
			{
				writer.Write(" fAT=1");
			}
			bool itemProperty = this.DataSource.GetItemProperty<bool>(TaskSchema.IsTaskRecurring, false);
			if (itemProperty)
			{
				writer.Write(" fRT=1");
			}
			RecurrenceType itemProperty2 = this.DataSource.GetItemProperty<RecurrenceType>(TaskSchema.RecurrenceType, RecurrenceType.None);
			if (TaskUtilities.IsRegeneratingRecurrenceType(itemProperty2))
			{
				writer.Write(" fRgT=1");
			}
			ExDateTime itemProperty3 = this.DataSource.GetItemProperty<ExDateTime>(TaskSchema.DueDate, ExDateTime.MinValue);
			ExDateTime date = DateTimeUtilities.GetLocalTime().Date;
			if (itemProperty3 > ExDateTime.MinValue && itemProperty3.Date < date)
			{
				writer.Write(" fOD=1");
			}
			ExDateTime date2 = (itemProperty3 > ExDateTime.MinValue) ? itemProperty3 : date;
			writer.Write(" dtDD=\"");
			writer.Write(DateTimeUtilities.GetJavascriptDate(date2));
			writer.Write("\"");
			Importance itemProperty4 = this.DataSource.GetItemProperty<Importance>(ItemSchema.Importance, Importance.Normal);
			writer.Write(" iI=");
			writer.Write((int)itemProperty4);
			base.RenderFlagState(writer);
			bool itemProperty5 = this.DataSource.GetItemProperty<bool>(StoreObjectSchema.IsRestricted, false);
			if (itemProperty5 && base.UserContext.IsIrmEnabled)
			{
				ContentRight itemProperty6 = (ContentRight)this.DataSource.GetItemProperty<int>(MessageItemSchema.DRMRights, 0);
				RenderingUtilities.RenderExpando(writer, "fRplR", itemProperty6.IsUsageRightGranted(ContentRight.Reply) ? 0 : 1);
				RenderingUtilities.RenderExpando(writer, "fRAR", itemProperty6.IsUsageRightGranted(ContentRight.ReplyAll) ? 0 : 1);
				RenderingUtilities.RenderExpando(writer, "fFR", itemProperty6.IsUsageRightGranted(ContentRight.Forward) ? 0 : 1);
			}
			base.RenderMeetingRequestExpandos(writer);
		}

		protected override void RenderItemRowStyle(TextWriter writer)
		{
			bool itemProperty = this.DataSource.GetItemProperty<bool>(ItemSchema.IsComplete, false);
			if (itemProperty)
			{
				writer.Write(" tskCmp");
				return;
			}
			ExDateTime itemProperty2 = this.DataSource.GetItemProperty<ExDateTime>(TaskSchema.DueDate, ExDateTime.MinValue);
			if (itemProperty2 > ExDateTime.MinValue && itemProperty2.Date < DateTimeUtilities.GetLocalTime().Date)
			{
				writer.Write(" tskOvd");
			}
		}

		protected override void RenderTableCellAttributes(TextWriter writer, ColumnId columnId)
		{
			if (columnId == ColumnId.Subject)
			{
				string itemProperty = this.DataSource.GetItemProperty<string>(ItemSchema.Subject);
				if (!string.IsNullOrEmpty(itemProperty))
				{
					writer.Write(" class=tskStl");
				}
				writer.Write(" id=sb");
				return;
			}
			if (columnId == ColumnId.DueDate)
			{
				if (this.DataSource.GetItemProperty<ExDateTime?>(TaskSchema.DueDate, null) != null)
				{
					writer.Write(" class=tskStl");
				}
				if (!this.IsAssignedTask)
				{
					writer.Write(" id=dd");
					return;
				}
			}
			else if (columnId == ColumnId.Importance)
			{
				writer.Write(" id=");
				writer.Write("divImp");
			}
		}

		protected override string ColumnClassPrefix
		{
			get
			{
				return "taskColumn";
			}
		}

		protected override bool RenderIcon(TextWriter writer)
		{
			string itemProperty = this.DataSource.GetItemProperty<string>(StoreObjectSchema.ItemClass);
			bool itemProperty2 = this.DataSource.GetItemProperty<bool>(StoreObjectSchema.IsRestricted, false);
			int itemProperty3 = this.DataSource.GetItemProperty<int>(ItemSchema.IconIndex, -1);
			bool itemProperty4 = this.DataSource.GetItemProperty<bool>(MessageItemSchema.IsRead, false);
			return ListViewContentsRenderingUtilities.RenderTaskIcon(writer, base.UserContext, itemProperty, itemProperty3, itemProperty4, itemProperty2);
		}

		private const string ImportanceId = "divImp";

		private const string IsReplyRestricted = "fRplR";

		private const string IsReplyAllRestricted = "fRAR";

		private const string IsForwardRestricted = "fFR";
	}
}
