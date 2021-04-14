using System;
using System.Collections;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class TaskVirtualListView : VirtualListView2
	{
		public TaskVirtualListView(UserContext userContext, string id, ColumnId sortedColumn, SortOrder sortOrder, Folder dataFolder, QueryFilter queryFilter, SearchScope folderScope, bool userCanCreateItem) : this(userContext, id, sortedColumn, sortOrder, dataFolder, queryFilter, folderScope, userCanCreateItem, false)
		{
		}

		public TaskVirtualListView(UserContext userContext, string id, ColumnId sortedColumn, SortOrder sortOrder, Folder dataFolder, QueryFilter queryFilter, SearchScope folderScope, bool userCanCreateItem, bool isFiltered) : base(userContext, id, false, sortedColumn, sortOrder, isFiltered)
		{
			this.dataFolder = dataFolder;
			this.queryFilter = queryFilter;
			this.folderScope = folderScope;
			this.userCanCreateItem = userCanCreateItem;
		}

		protected override Folder DataFolder
		{
			get
			{
				return this.dataFolder;
			}
		}

		public override ViewType ViewType
		{
			get
			{
				return ViewType.Task;
			}
		}

		public override string OehNamespace
		{
			get
			{
				return "TskVLV";
			}
		}

		protected override ListViewContents2 CreateListViewContents()
		{
			ListViewContents2 listViewContents = new TaskSingleLineList(TaskVirtualListView.taskViewDescriptor, base.SortedColumn, base.SortOrder, base.UserContext, this.folderScope);
			ColumnId sortedColumn = base.SortedColumn;
			if (sortedColumn == ColumnId.DueDate)
			{
				listViewContents = new TimeGroupByList2(ColumnId.DueDate, base.SortOrder, (ItemList2)listViewContents, base.UserContext);
			}
			else
			{
				Column column = ListViewColumns.GetColumn(base.SortedColumn);
				if (column.GroupType == GroupType.Expanded)
				{
					listViewContents = new GroupByList2(base.SortedColumn, base.SortOrder, (ItemList2)listViewContents, base.UserContext);
				}
			}
			return listViewContents;
		}

		protected override IListViewDataSource CreateDataSource(Hashtable properties)
		{
			return new FolderListViewDataSource(base.UserContext, properties, this.dataFolder, this.GetSortByProperties(), this.queryFilter);
		}

		protected override void OnBeforeRender()
		{
			base.MakePropertyPublic("t");
			base.MakePropertyPublic("read");
			base.MakePropertyPublic("MM");
			base.MakePropertyPublic("fPhsh");
			base.MakePropertyPublic("fMR");
			base.MakePropertyPublic("fRR");
			base.MakePropertyPublic("fDoR");
			base.MakePropertyPublic("fAT");
			base.MakePropertyPublic("s");
			base.MakePropertyPublic("fRplR");
			base.MakePropertyPublic("fRAR");
			base.MakePropertyPublic("fFR");
		}

		protected override void InternalRenderData(TextWriter writer)
		{
			base.InternalRenderData(writer);
		}

		public override void RenderListViewClasses(TextWriter writer)
		{
			if (this.userCanCreateItem)
			{
				writer.Write(" class=\"taskListView\"");
				return;
			}
			writer.Write(" class=\"taskListViewNoQuickTask\"");
		}

		protected override void RenderInlineControl(TextWriter writer)
		{
			if (this.userCanCreateItem)
			{
				writer.Write("<div id=divQTsk dtTdy=\"");
				writer.Write(DateTimeUtilities.GetJavascriptDate(DateTimeUtilities.GetLocalTime().Date));
				writer.Write("\" sDtFmt=\"");
				Utilities.HtmlEncode(base.UserContext.UserOptions.DateFormat, writer);
				writer.Write("\"><div id=\"divAddTask\" tabindex=\"0\" class=\"fltAfter\">");
				base.UserContext.RenderThemeImageWithToolTip(writer, ThemeFileId.AddTask, string.Empty, -2096871403, new string[0]);
				writer.Write("</div><div id=\"divDueDateImage\" class=\"fltAfter\">");
				base.UserContext.RenderThemeImage(writer, ThemeFileId.DownArrowGrey);
				writer.Write(string.Format(CultureInfo.InvariantCulture, "</div><div id=\"{0}\" class=\"fltAfter\" tabindex=\"0\" nowrap>", new object[]
				{
					"divDueDate"
				}));
				writer.Write(LocalizedStrings.GetHtmlEncoded(-481406887));
				writer.Write("</div><div id=\"divSbj\"><input id=\"txtTaskSubject\" type=\"text\" value=\"");
				writer.Write(LocalizedStrings.GetHtmlEncoded(488278548));
				writer.Write("\" maxLength=\"255\"></div></div>");
				return;
			}
			writer.Write(string.Format(CultureInfo.InvariantCulture, "<div id=\"{0}\" sytle=\"display:none\"></div>", new object[]
			{
				"divDueDate"
			}));
		}

		private SortBy[] GetSortByProperties()
		{
			Column column = ListViewColumns.GetColumn(base.SortedColumn);
			if (base.SortedColumn == ColumnId.DueDate)
			{
				return new SortBy[]
				{
					new SortBy(TaskSchema.DueDate, base.SortOrder)
				};
			}
			if (base.SortedColumn == ColumnId.TaskIcon)
			{
				return new SortBy[]
				{
					new SortBy(StoreObjectSchema.ItemClass, base.SortOrder),
					new SortBy(ItemSchema.IconIndex, base.SortOrder),
					new SortBy(TaskSchema.DueDate, SortOrder.Ascending)
				};
			}
			return new SortBy[]
			{
				new SortBy(column[0], base.SortOrder),
				new SortBy(TaskSchema.DueDate, SortOrder.Ascending)
			};
		}

		protected override bool HasInlineControl
		{
			get
			{
				return true;
			}
		}

		public const string DueDateId = "divDueDate";

		private static readonly ViewDescriptor taskViewDescriptor = new ViewDescriptor(ColumnId.DueDate, false, new ColumnId[]
		{
			ColumnId.TaskIcon,
			ColumnId.MarkCompleteCheckbox,
			ColumnId.Importance,
			ColumnId.HasAttachment,
			ColumnId.Subject,
			ColumnId.DueDate,
			ColumnId.Categories,
			ColumnId.TaskFlag
		});

		private Folder dataFolder;

		private QueryFilter queryFilter;

		private SearchScope folderScope;

		private bool userCanCreateItem;
	}
}
