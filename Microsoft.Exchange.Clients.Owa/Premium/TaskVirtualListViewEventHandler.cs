using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("TskVLV")]
	[OwaEventObjectId(typeof(OwaStoreObjectId))]
	[OwaEventSegmentation(Feature.Tasks)]
	internal sealed class TaskVirtualListViewEventHandler : FolderVirtualListViewEventHandler2
	{
		public new static void Register()
		{
			FolderVirtualListViewEventHandler2.Register();
			OwaEventRegistry.RegisterHandler(typeof(TaskVirtualListViewEventHandler));
		}

		protected override void PersistFilter()
		{
			if (!base.UserContext.IsWebPartRequest)
			{
				int num = (int)base.GetParameter("fltr");
				int folderProperty = Utilities.GetFolderProperty<int>(base.ContextFolder, ViewStateProperties.ViewFilter, 1);
				if (num != folderProperty)
				{
					base.ContextFolder[ViewStateProperties.ViewFilter] = num;
					base.ContextFolder.Save();
				}
			}
		}

		[OwaEvent("MarkComplete")]
		[OwaEventParameter("Itms", typeof(ObjectId), true)]
		[OwaEventParameter("mkIncmp", typeof(bool), false, true)]
		public void MarkComplete()
		{
			ExTraceGlobals.TasksCallTracer.TraceDebug((long)this.GetHashCode(), "TaskVirtualListViewEventHandler.MarkComplete");
			bool flag = base.IsParameterSet("mkIncmp") && (bool)base.GetParameter("mkIncmp");
			OwaStoreObjectId[] array = (OwaStoreObjectId[])base.GetParameter("Itms");
			for (int i = 0; i < array.Length; i++)
			{
				using (Item item = Utilities.GetItem<Item>(base.UserContext, array[i], this.prefetchProperties))
				{
					TaskVirtualListViewEventHandler.ThrowIfAssignedTask(item);
					item.OpenAsReadWrite();
					if (!flag)
					{
						FlagEventHandler.FlagComplete(item);
					}
					else
					{
						FlagEventHandler.SetFlag(item, FlagAction.Default, null);
					}
					Utilities.SaveItem(item);
				}
			}
		}

		[OwaEventParameter("imp", typeof(Importance))]
		[OwaEvent("SetImportance")]
		[OwaEventParameter("id", typeof(ObjectId))]
		public void SetImportance()
		{
			ExTraceGlobals.TasksCallTracer.TraceDebug((long)this.GetHashCode(), "TaskVirtualListViewEventHandler.SetImportance");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("id");
			Importance importance = (Importance)base.GetParameter("imp");
			using (Item item = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId, this.prefetchProperties))
			{
				TaskVirtualListViewEventHandler.ThrowIfAssignedTask(item);
				item.OpenAsReadWrite();
				item.Importance = importance;
				Utilities.SaveItem(item);
			}
		}

		[OwaEvent("GetDatePicker")]
		public void GetDatePicker()
		{
			ExTraceGlobals.TasksCallTracer.TraceDebug((long)this.GetHashCode(), "TaskVirtualListViewEventHandler.GetDatePicker");
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			DatePicker datePicker = new DatePicker("divDueDateDP", localTime, 12);
			this.Writer.Write("<div id=\"divDueDateDropDown\" class=\"pu\" style=\"display:none\">");
			datePicker.Render(this.Writer);
			this.Writer.Write("</div>");
		}

		[OwaEvent("SetDueDate")]
		[OwaEventParameter("id", typeof(ObjectId))]
		[OwaEventParameter("ddt", typeof(ExDateTime), false, true)]
		public void SetDueDate()
		{
			ExTraceGlobals.TasksCallTracer.TraceDebug((long)this.GetHashCode(), "TaskVirtualListViewEventHandler.SetDueDate");
			ExDateTime? exDateTime = (ExDateTime?)base.GetParameter("ddt");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("id");
			using (Item item = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId, this.prefetchProperties))
			{
				TaskVirtualListViewEventHandler.ThrowIfAssignedTask(item);
				item.OpenAsReadWrite();
				Task task = item as Task;
				if (task != null)
				{
					task.DueDate = exDateTime;
					if (task.StartDate != null && exDateTime != null && task.StartDate.Value > exDateTime.Value)
					{
						task.StartDate = exDateTime;
					}
				}
				else
				{
					string property = ItemUtility.GetProperty<string>(item, ItemSchema.FlagRequest, LocalizedStrings.GetNonEncoded(-1950847676));
					ExDateTime? startDate = ItemUtility.GetProperty<ExDateTime?>(item, ItemSchema.UtcStartDate, null);
					if (exDateTime == null)
					{
						startDate = null;
					}
					else if (startDate != null && startDate.Value > exDateTime.Value)
					{
						startDate = exDateTime;
					}
					item.SetFlag(property, startDate, exDateTime);
				}
				Utilities.SaveItem(item);
				this.Writer.Write("<div id=data dtDD=\"");
				ExDateTime date = DateTimeUtilities.GetLocalTime().Date;
				ExDateTime date2 = (exDateTime != null) ? exDateTime.Value : date;
				this.Writer.Write(DateTimeUtilities.GetJavascriptDate(date2));
				this.Writer.Write("\"");
				if (exDateTime != null && exDateTime.Value.Date < date)
				{
					this.Writer.Write(" fOD=1");
				}
				this.Writer.Write(">");
				if (exDateTime != null)
				{
					this.Writer.Write(exDateTime.Value.ToString(base.UserContext.UserOptions.DateFormat));
				}
				else
				{
					this.Writer.Write("&nbsp;");
				}
				this.Writer.Write("</div>");
			}
		}

		[OwaEventParameter("id", typeof(OwaStoreObjectId), false, true)]
		[OwaEvent("CreateTask")]
		[OwaEventParameter("ddt", typeof(ExDateTime), false, true)]
		[OwaEventParameter("fId", typeof(ObjectId))]
		[OwaEventParameter("sbj", typeof(string), false, true)]
		public void CreateTask()
		{
			ExTraceGlobals.TasksCallTracer.TraceDebug((long)this.GetHashCode(), "TaskVirtualListViewEventHandler.CreateTask");
			if (!base.IsParameterSet("sbj") && !base.IsParameterSet("ddt"))
			{
				throw new OwaInvalidRequestException("Cannot create task without subject or due date.");
			}
			OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
			if (Utilities.IsDefaultFolderId(base.UserContext, folderId, DefaultFolderType.ToDoSearch))
			{
				folderId = base.UserContext.TasksFolderOwaId;
			}
			using (Task task = Utilities.CreateItem<Task>(folderId))
			{
				if (!base.IsParameterSet("id"))
				{
					task[TaskSchema.TaskOwner] = base.UserContext.ExchangePrincipal.MailboxInfo.DisplayName;
				}
				if (base.IsParameterSet("sbj"))
				{
					string text = (string)base.GetParameter("sbj");
					if (text.Length > 255)
					{
						throw new OwaInvalidRequestException("Cannot create task with subject greater than 255 characters.");
					}
					task.Subject = text;
				}
				if (base.IsParameterSet("ddt"))
				{
					task.DueDate = new ExDateTime?((ExDateTime)base.GetParameter("ddt"));
				}
				Utilities.SaveItem(task);
				task.Load();
				this.Writer.Write(Utilities.GetIdAsString(task));
			}
		}

		protected override VirtualListView2 GetListView()
		{
			base.BindToFolder();
			return new TaskVirtualListView(base.UserContext, "divVLV", this.ListViewState.SortedColumn, this.ListViewState.SortOrder, base.DataFolder, this.GetViewFilter(), base.SearchScope, Utilities.CanCreateItemInFolder(base.ContextFolder), base.IsFiltered);
		}

		protected override QueryFilter GetViewFilter()
		{
			TaskFilterType filterType = TaskFilterType.All;
			if (base.IsParameterSet("fltr"))
			{
				filterType = (TaskFilterType)base.GetParameter("fltr");
			}
			base.FolderQueryFilter = TaskView.GetFilter(filterType);
			if (!Utilities.IsPublic(base.ContextFolder) && base.GetParameter("srchf") != null)
			{
				if (base.FolderQueryFilter == null)
				{
					base.FolderQueryFilter = TaskVirtualListViewEventHandler.taskItemFilter;
				}
				else
				{
					base.FolderQueryFilter = new AndFilter(new QueryFilter[]
					{
						base.FolderQueryFilter,
						TaskVirtualListViewEventHandler.taskItemFilter
					});
				}
			}
			return base.GetViewFilter();
		}

		protected override void RenderSearchInPublicFolder(TextWriter writer)
		{
			AdvancedFindComponents advancedFindComponents = AdvancedFindComponents.Categories | AdvancedFindComponents.SearchTextInSubject | AdvancedFindComponents.SearchButton;
			base.RenderAdvancedFind(this.Writer, advancedFindComponents, null);
		}

		protected override void RenderAdvancedFind(TextWriter writer, OwaStoreObjectId folderId)
		{
			base.RenderAdvancedFind(writer, AdvancedFindComponents.Categories, folderId);
		}

		private static void ThrowIfAssignedTask(Item item)
		{
			TaskType property = (TaskType)ItemUtility.GetProperty<int>(item, TaskSchema.TaskType, 0);
			if (TaskUtilities.IsAssignedTaskType(property))
			{
				throw new OwaInvalidRequestException("Assigned tasks cannot be edited");
			}
		}

		public const string EventNamespace = "TskVLV";

		public const string MethodMarkComplete = "MarkComplete";

		public const string MethodSetImportance = "SetImportance";

		public const string MethodGetDatePicker = "GetDatePicker";

		public const string MethodSetDueDate = "SetDueDate";

		public const string MethodCreateTask = "CreateTask";

		public const string MarkIncomplete = "mkIncmp";

		public const string Importance = "imp";

		public const string Subject = "sbj";

		private static TextFilter taskFilter = new TextFilter(StoreObjectSchema.ItemClass, "IPM.Task", MatchOptions.FullString, MatchFlags.IgnoreCase);

		private static TextFilter custonTaskFilter = new TextFilter(StoreObjectSchema.ItemClass, "IPM.Task.", MatchOptions.Prefix, MatchFlags.IgnoreCase);

		private static TextFilter taskRequestFilter = new TextFilter(StoreObjectSchema.ItemClass, "IPM.TaskRequest", MatchOptions.FullString, MatchFlags.IgnoreCase);

		private static TextFilter customTaskRequestFilter = new TextFilter(StoreObjectSchema.ItemClass, "IPM.TaskRequest.", MatchOptions.Prefix, MatchFlags.IgnoreCase);

		private static ComparisonFilter flagCompleteFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.FlagStatus, FlagStatus.Complete);

		private static ComparisonFilter flaggedFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.FlagStatus, FlagStatus.Flagged);

		private static QueryFilter taskItemFilter = new OrFilter(new QueryFilter[]
		{
			TaskVirtualListViewEventHandler.taskFilter,
			TaskVirtualListViewEventHandler.custonTaskFilter,
			TaskVirtualListViewEventHandler.taskRequestFilter,
			TaskVirtualListViewEventHandler.customTaskRequestFilter,
			TaskVirtualListViewEventHandler.flagCompleteFilter,
			TaskVirtualListViewEventHandler.flaggedFilter
		});

		private PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
		{
			TaskSchema.TaskType
		};
	}
}
