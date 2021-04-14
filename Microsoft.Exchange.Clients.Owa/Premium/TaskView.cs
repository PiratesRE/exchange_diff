using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class TaskView : FolderListViewSubPage, IRegistryOnlyForm
	{
		public TaskView() : base(ExTraceGlobals.TasksCallTracer, ExTraceGlobals.TasksTracer)
		{
		}

		protected static int StoreObjectTypeTasksFolder
		{
			get
			{
				return 4;
			}
		}

		protected static int ImportanceLow
		{
			get
			{
				return 0;
			}
		}

		protected static int ImportanceNormal
		{
			get
			{
				return 1;
			}
		}

		protected static int ImportanceHigh
		{
			get
			{
				return 2;
			}
		}

		protected override string ContainerName
		{
			get
			{
				if (base.FolderType == DefaultFolderType.ToDoSearch)
				{
					return LocalizedStrings.GetNonEncoded(-1954334922);
				}
				if (base.IsArchiveMailboxFolder)
				{
					return string.Format(LocalizedStrings.GetNonEncoded(-83764036), base.Folder.DisplayName, Utilities.GetMailboxOwnerDisplayName((MailboxSession)base.Folder.Session));
				}
				if (base.IsOtherMailboxFolder)
				{
					return Utilities.GetFolderNameWithSessionName(base.Folder);
				}
				return base.Folder.DisplayName;
			}
		}

		internal StoreObjectId FolderId
		{
			get
			{
				return base.Folder.Id.ObjectId;
			}
		}

		internal override StoreObjectId DefaultFolderId
		{
			get
			{
				return base.UserContext.FlaggedItemsAndTasksFolderId;
			}
		}

		protected override SortOrder DefaultSortOrder
		{
			get
			{
				return SortOrder.Ascending;
			}
		}

		protected override ColumnId DefaultSortedColumn
		{
			get
			{
				return ColumnId.DueDate;
			}
		}

		protected override ReadingPanePosition DefaultReadingPanePosition
		{
			get
			{
				return ReadingPanePosition.Right;
			}
		}

		protected override bool DefaultMultiLineSetting
		{
			get
			{
				return false;
			}
		}

		protected override bool FindBarOn
		{
			get
			{
				return !base.IsPublicFolder && base.UserContext.UserOptions.MailFindBarOn;
			}
		}

		protected TaskViewContextMenu ContextMenu
		{
			get
			{
				if (this.contextMenu == null)
				{
					this.contextMenu = new TaskViewContextMenu(base.UserContext);
				}
				return this.contextMenu;
			}
		}

		protected TaskViewArrangeByMenu ArrangeByMenu
		{
			get
			{
				if (this.arrangeByMenu == null)
				{
					this.arrangeByMenu = new TaskViewArrangeByMenu();
				}
				return this.arrangeByMenu;
			}
		}

		protected override void LoadViewState()
		{
			base.LoadViewState();
			if (!base.UserContext.IsWebPartRequest)
			{
				this.viewWidth = base.UserContext.GetFolderViewStates(base.Folder).GetViewWidth(381);
			}
			if (!base.UserContext.IsWebPartRequest)
			{
				this.filterType = Utilities.GetFolderProperty<TaskFilterType>(base.Folder, ViewStateProperties.ViewFilter, TaskFilterType.All);
				return;
			}
			if (this.IsFlaggedMailAndTasks && this.SortedColumn == ColumnId.DueDate)
			{
				base.SetSortOrder(SortOrder.Ascending);
			}
		}

		protected override IListView CreateListView(ColumnId sortedColumn, SortOrder sortOrder)
		{
			TaskVirtualListView taskVirtualListView = new TaskVirtualListView(base.UserContext, "divVLV", sortedColumn, sortOrder, base.Folder, TaskView.GetFilter(this.filterType), (base.Folder is SearchFolder) ? SearchScope.AllFoldersAndItems : SearchScope.SelectedFolder, this.CanCreateItem);
			VirtualListView2 virtualListView = taskVirtualListView;
			string name = "iFltr";
			int num = (int)this.filterType;
			virtualListView.AddAttribute(name, num.ToString(CultureInfo.InvariantCulture));
			taskVirtualListView.LoadData(0, 50);
			return taskVirtualListView;
		}

		protected override Toolbar CreateListToolbar()
		{
			return new TaskViewListToolbar(base.IsPublicFolder, base.IsOtherMailboxFolder, base.UserContext.IsWebPartRequest, this.ReadingPanePosition);
		}

		protected override Toolbar CreateActionToolbar()
		{
			return null;
		}

		protected void RenderPontStrings()
		{
			if (base.UserContext.UserOptions.IsPontEnabled(PontType.DeleteFlaggedMessage))
			{
				RenderingUtilities.RenderStringVariable(base.Response.Output, "L_PntMsg", 1701858762);
			}
			if (base.UserContext.UserOptions.IsPontEnabled(PontType.DeleteFlaggedContacts))
			{
				RenderingUtilities.RenderStringVariable(base.Response.Output, "L_PntCnt", -1776379122);
			}
			if (base.UserContext.UserOptions.IsPontEnabled(PontType.DeleteFlaggedItems))
			{
				RenderingUtilities.RenderStringVariable(base.Response.Output, "L_PntMlt", 259109454);
			}
			if (base.UserContext.UserOptions.IsPontEnabled(PontType.DeleteFlaggedContacts) || base.UserContext.UserOptions.IsPontEnabled(PontType.DeleteFlaggedMessage) || base.UserContext.UserOptions.IsPontEnabled(PontType.DeleteFlaggedItems))
			{
				RenderingUtilities.RenderStringVariable(base.Response.Output, "L_Wrn", 1861340610);
				RenderingUtilities.RenderStringVariable(base.Response.Output, "L_Cntnu", -1719707164);
				RenderingUtilities.RenderStringVariable(base.Response.Output, "L_DntShw", -1294868987);
			}
		}

		protected void RenderArrangeByMenu()
		{
			TaskViewArrangeByMenu taskViewArrangeByMenu = new TaskViewArrangeByMenu();
			taskViewArrangeByMenu.Render(base.Response.Output, base.UserContext);
		}

		protected void RenderContextMenu()
		{
			TaskViewContextMenu taskViewContextMenu = new TaskViewContextMenu(base.UserContext);
			taskViewContextMenu.Render(base.Response.Output);
		}

		protected bool IsFlaggedMailAndTasks
		{
			get
			{
				return base.FolderType == DefaultFolderType.ToDoSearch;
			}
		}

		protected bool CanCreateItem
		{
			get
			{
				return Utilities.CanCreateItemInFolder(base.Folder);
			}
		}

		internal static QueryFilter GetFilter(TaskFilterType filterType)
		{
			switch (filterType)
			{
			case TaskFilterType.All:
				return null;
			case TaskFilterType.Active:
				return TaskView.activeFilter;
			case TaskFilterType.Overdue:
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.LessThan, TaskSchema.DueDate, DateTimeUtilities.GetLocalTime().Date);
				return new AndFilter(new QueryFilter[]
				{
					queryFilter,
					TaskView.activeFilter
				});
			}
			case TaskFilterType.Completed:
				return TaskView.completeFilter;
			default:
				throw new OwaInvalidRequestException("Unknown value for TaskFilterType");
			}
		}

		internal static void RenderSecondaryNavigation(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			TaskView.RenderSecondaryNavigationFilter(output, "divTskFlt");
			NavigationHost.RenderNavigationTreeControl(output, userContext, NavigationModule.Tasks);
		}

		internal static void RenderSecondaryNavigationFilter(TextWriter output, string divId)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (string.IsNullOrEmpty(divId))
			{
				throw new ArgumentException("divId should not be null or empty");
			}
			SecondaryNavigationFilter secondaryNavigationFilter = new SecondaryNavigationFilter(divId, LocalizedStrings.GetNonEncoded(-428271462), "onClkTskFlt(\"" + Utilities.JavascriptEncode(divId) + "\")");
			secondaryNavigationFilter.AddFilter(LocalizedStrings.GetNonEncoded(1912141011), 1, false);
			secondaryNavigationFilter.AddFilter(LocalizedStrings.GetNonEncoded(868758546), 2, false);
			secondaryNavigationFilter.AddFilter(LocalizedStrings.GetNonEncoded(-1626869372), 3, false);
			secondaryNavigationFilter.AddFilter(LocalizedStrings.GetNonEncoded(-1035255369), 4, false);
			secondaryNavigationFilter.Render(output);
		}

		public override IEnumerable<string> ExternalScriptFiles
		{
			get
			{
				return this.externalScriptFiles;
			}
		}

		public override SanitizedHtmlString Title
		{
			get
			{
				return new SanitizedHtmlString(this.ContainerName);
			}
		}

		public override string PageType
		{
			get
			{
				return "TaskViewPage";
			}
		}

		private static readonly QueryFilter activeFilter = new AndFilter(new QueryFilter[]
		{
			new NotFilter(new ExistsFilter(ItemSchema.CompleteDate)),
			new NotFilter(new ExistsFilter(ItemSchema.FlagCompleteTime))
		});

		private static readonly QueryFilter completeFilter = new OrFilter(new QueryFilter[]
		{
			new ExistsFilter(ItemSchema.CompleteDate),
			new ExistsFilter(ItemSchema.FlagCompleteTime),
			new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.FlagStatus, FlagStatus.Complete)
		});

		private TaskFilterType filterType = TaskFilterType.All;

		private TaskViewContextMenu contextMenu;

		private TaskViewArrangeByMenu arrangeByMenu;

		private string[] externalScriptFiles = new string[]
		{
			"uview.js",
			"vlv.js",
			"taskvw.js"
		};
	}
}
