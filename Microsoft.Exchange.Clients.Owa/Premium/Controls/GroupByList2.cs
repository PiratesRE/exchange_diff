using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class GroupByList2 : ListViewContents2
	{
		internal GroupByList2(ColumnId groupByColumn, SortOrder sortOrder, ItemList2 itemList, UserContext userContext) : base(userContext)
		{
			this.groupByColumn = ListViewColumns.GetColumn(groupByColumn);
			this.sortOrder = sortOrder;
			this.itemList = itemList;
			IDictionaryEnumerator enumerator = itemList.Properties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				base.AddProperty((PropertyDefinition)enumerator.Key);
			}
			for (int i = 0; i < this.groupByColumn.PropertyCount; i++)
			{
				base.AddProperty(this.groupByColumn[i]);
			}
		}

		public override ViewDescriptor ViewDescriptor
		{
			get
			{
				return this.itemList.ViewDescriptor;
			}
		}

		public ItemList2 ItemList
		{
			get
			{
				return this.itemList;
			}
		}

		public Column GroupByColumn
		{
			get
			{
				return this.groupByColumn;
			}
		}

		public SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		protected IGroupRange[] GroupRange
		{
			get
			{
				return this.groupRange;
			}
		}

		protected void SetGroupRange(IGroupRange[] groupRange)
		{
			this.groupRange = groupRange;
		}

		protected override void ValidatedRender(TextWriter writer, int startRange, int endRange)
		{
			writer.Write("<div class=\"baseIL gbIL\" id=\"");
			writer.Write("divVLVIL");
			writer.Write("\"");
			if (this.itemList.ViewDescriptor.IsFixedWidth)
			{
				writer.Write(" style=\"width:");
				writer.Write(this.itemList.ViewDescriptor.Width);
				writer.Write("em\"");
			}
			writer.Write(">");
			this.itemList.DataSource = this.DataSource;
			this.RenderGroups(writer, startRange, endRange);
			writer.Write("</div>");
		}

		protected void RenderGroups(TextWriter writer, int startRange, int endRange)
		{
			GroupType groupType = this.groupByColumn.GroupType;
			if (groupType != GroupType.Expanded)
			{
				return;
			}
			if (this.groupRange == null)
			{
				this.DataSource.MoveToItem(startRange);
				object obj = this.GetItemGroupByValue();
				string groupValueString = obj as string;
				bool skipHeader = this.ShouldSkipFirstHeader(obj, groupValueString);
				while (this.DataSource.MoveNext())
				{
					object itemGroupByValue = this.GetItemGroupByValue();
					string itemValueString = itemGroupByValue as string;
					if (!GroupByList2.IsItemInGroup(obj, groupValueString, itemGroupByValue, itemValueString))
					{
						this.RenderGroup(writer, startRange, this.DataSource.CurrentItem - 1, null, skipHeader);
						startRange = this.DataSource.CurrentItem;
						obj = itemGroupByValue;
						groupValueString = (obj as string);
						skipHeader = false;
					}
				}
				if (startRange <= this.DataSource.CurrentItem)
				{
					this.RenderGroup(writer, startRange, this.DataSource.CurrentItem - 1, null, skipHeader);
					return;
				}
			}
			else
			{
				this.DataSource.MoveToItem(startRange);
				int num;
				if (this.sortOrder == SortOrder.Ascending)
				{
					num = 0;
				}
				else
				{
					num = this.groupRange.Length - 1;
				}
				while (0 <= num && num < this.groupRange.Length && this.DataSource.CurrentItem < this.DataSource.RangeCount)
				{
					startRange = int.MinValue;
					bool skipHeader2 = false;
					while (this.groupRange[num].IsInGroup(this.DataSource, this.groupByColumn))
					{
						if (startRange == -2147483648)
						{
							startRange = this.DataSource.CurrentItem;
							if (startRange == 0)
							{
								skipHeader2 = this.ShouldSkipFirstHeader(this.groupRange[num]);
							}
						}
						if (!this.DataSource.MoveNext())
						{
							break;
						}
					}
					if (startRange != -2147483648)
					{
						this.RenderGroup(writer, startRange, this.DataSource.CurrentItem - 1, this.groupRange[num], skipHeader2);
					}
					if (this.sortOrder == SortOrder.Ascending)
					{
						num++;
					}
					else
					{
						num--;
					}
				}
			}
		}

		private static bool IsItemInGroup(object groupValue, string groupValueString, object itemValue, string itemValueString)
		{
			bool flag = groupValueString != null;
			bool flag2 = itemValueString != null;
			if (flag && flag2)
			{
				return string.Equals(groupValueString, itemValueString, StringComparison.CurrentCultureIgnoreCase);
			}
			if (flag || flag2)
			{
				return false;
			}
			if (groupValue == null)
			{
				return itemValue == null;
			}
			return groupValue.Equals(itemValue);
		}

		private bool ShouldSkipFirstHeader(object groupValue, string groupValueString)
		{
			if (!base.IsForVirtualListView || this.DataSource.StartRange == 0)
			{
				return false;
			}
			int currentItem = this.DataSource.CurrentItem;
			this.DataSource.MoveToItem(currentItem - 1);
			object itemGroupByValue = this.GetItemGroupByValue();
			string itemValueString = itemGroupByValue as string;
			bool result = GroupByList2.IsItemInGroup(groupValue, groupValueString, itemGroupByValue, itemValueString);
			this.DataSource.MoveToItem(currentItem);
			return result;
		}

		private bool ShouldSkipFirstHeader(IGroupRange firstGroupRange)
		{
			if (!base.IsForVirtualListView || this.DataSource.StartRange == 0)
			{
				return false;
			}
			int currentItem = this.DataSource.CurrentItem;
			this.DataSource.MoveToItem(currentItem - 1);
			bool result = firstGroupRange.IsInGroup(this.DataSource, this.groupByColumn);
			this.DataSource.MoveToItem(currentItem);
			return result;
		}

		internal object GetItemGroupByValue()
		{
			object obj = this.DataSource.GetItemProperty<object>(this.groupByColumn[0]);
			if (obj is PropertyError || obj == null)
			{
				obj = string.Empty;
			}
			return obj;
		}

		internal string GetItemGroupByValueString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
			{
				if (this.groupRange == null)
				{
					this.RenderGroupHeader(stringWriter, null);
				}
				else
				{
					if (this.sortOrder != SortOrder.Ascending)
					{
						int i = this.groupRange.Length - 1;
					}
					for (int i = 0; i < this.groupRange.Length; i++)
					{
						if (this.groupRange[i].IsInGroup(this.DataSource, this.groupByColumn))
						{
							this.RenderGroupHeader(stringWriter, this.groupRange[i]);
							break;
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		private void RenderGroup(TextWriter writer, int startRange, int endRange, IGroupRange group, bool skipHeader)
		{
			this.DataSource.MoveToItem(startRange);
			writer.Write("<div gh=1");
			if (!base.IsForVirtualListView)
			{
				writer.Write(" id=\"");
				writer.Write("us");
				writer.Write("\"");
			}
			writer.Write(" class=\"gh");
			if (startRange == 0)
			{
				writer.Write(" fgh");
			}
			if (skipHeader)
			{
				writer.Write(" hide");
			}
			writer.Write("\"");
			if (this.groupByColumn.Id == ColumnId.MailIcon || this.groupByColumn.Id == ColumnId.ContactIcon)
			{
				writer.Write(" style=\"padding-left:3px;padding-right:3px;\"");
			}
			writer.Write("><div class=\"ghcnts\">");
			this.RenderGroupHeader(writer, group);
			writer.Write("</div></div>");
			writer.Write("<div id=\"gc\">");
			if (base.IsForVirtualListView)
			{
				this.itemList.RenderForVirtualListView(writer, startRange, endRange);
			}
			else
			{
				this.itemList.Render(writer, startRange, endRange);
			}
			writer.Write("</div>");
		}

		private void RenderGroupHeader(TextWriter writer, IGroupRange group)
		{
			if (group != null)
			{
				Utilities.RenderDirectionEnhancedValue(writer, Utilities.HtmlEncode(group.Header), base.UserContext.IsRtl);
				return;
			}
			ColumnId id = this.groupByColumn.Id;
			if (id <= ColumnId.ContactIcon)
			{
				if (id != ColumnId.MailIcon)
				{
					switch (id)
					{
					case ColumnId.HasAttachment:
					{
						bool itemProperty = this.DataSource.GetItemProperty<bool>(ItemSchema.HasAttachment, false);
						if (itemProperty)
						{
							writer.Write(this.GetHtmlEncodedLocalizeString(-67583501));
							return;
						}
						writer.Write(this.GetHtmlEncodedLocalizeString(549410256));
						return;
					}
					case ColumnId.Importance:
					{
						Importance importance = Importance.Normal;
						object itemProperty2 = this.DataSource.GetItemProperty<object>(ItemSchema.Importance);
						if (itemProperty2 is Importance || itemProperty2 is int)
						{
							importance = (Importance)itemProperty2;
						}
						switch (importance)
						{
						case Importance.Low:
							writer.Write(this.GetHtmlEncodedLocalizeString(-1013519457));
							return;
						case Importance.High:
							writer.Write(this.GetHtmlEncodedLocalizeString(-1275094763));
							return;
						}
						writer.Write(this.GetHtmlEncodedLocalizeString(1896035036));
						return;
					}
					default:
						if (id != ColumnId.ContactIcon)
						{
							goto IL_2C1;
						}
						break;
					}
				}
			}
			else if (id <= ColumnId.ConversationIcon)
			{
				if (id != ColumnId.TaskIcon)
				{
					if (id != ColumnId.ConversationIcon)
					{
						goto IL_2C1;
					}
					string itemProperty3 = this.DataSource.GetItemProperty<string>(ConversationItemSchema.ConversationMessageClasses);
					Utilities.RenderDirectionEnhancedValue(writer, Utilities.HtmlEncode(ItemClassType.GetDisplayString(itemProperty3)), base.UserContext.IsRtl);
					return;
				}
			}
			else
			{
				switch (id)
				{
				case ColumnId.ConversationHasAttachment:
				{
					bool itemProperty4 = this.DataSource.GetItemProperty<bool>(ConversationItemSchema.ConversationHasAttach, false);
					if (itemProperty4)
					{
						writer.Write(this.GetHtmlEncodedLocalizeString(-67583501));
						return;
					}
					writer.Write(this.GetHtmlEncodedLocalizeString(549410256));
					return;
				}
				case ColumnId.ConversationSenderList:
					goto IL_2C1;
				case ColumnId.ConversationImportance:
				{
					Importance importance2 = Importance.Normal;
					object itemProperty5 = this.DataSource.GetItemProperty<object>(ConversationItemSchema.ConversationImportance);
					if (itemProperty5 is Importance || itemProperty5 is int)
					{
						importance2 = (Importance)itemProperty5;
					}
					switch (importance2)
					{
					case Importance.Low:
						writer.Write(this.GetHtmlEncodedLocalizeString(-1013519457));
						return;
					case Importance.High:
						writer.Write(this.GetHtmlEncodedLocalizeString(-1275094763));
						return;
					}
					writer.Write(this.GetHtmlEncodedLocalizeString(1896035036));
					return;
				}
				default:
					if (id != ColumnId.ConversationFlagStatus)
					{
						goto IL_2C1;
					}
					switch (this.DataSource.GetItemProperty<int>(ConversationItemSchema.ConversationFlagStatus, 0))
					{
					case 1:
						writer.Write(this.GetHtmlEncodedLocalizeString(-1576429907));
						return;
					case 2:
						writer.Write(this.GetHtmlEncodedLocalizeString(-568934371));
						return;
					default:
						writer.Write(this.GetHtmlEncodedLocalizeString(-41558891));
						return;
					}
					break;
				}
			}
			string itemProperty6 = this.DataSource.GetItemProperty<string>(StoreObjectSchema.ItemClass);
			Utilities.RenderDirectionEnhancedValue(writer, Utilities.HtmlEncode(ItemClassType.GetDisplayString(itemProperty6)), base.UserContext.IsRtl);
			return;
			IL_2C1:
			object itemProperty7 = this.DataSource.GetItemProperty<object>(this.groupByColumn[0]);
			string text = itemProperty7 as string;
			if (itemProperty7 == null || itemProperty7 is PropertyError || (text != null && text.Length == 0))
			{
				writer.Write(this.GetHtmlEncodedLocalizeString(-1001801028));
				return;
			}
			base.RenderColumn(writer, this.groupByColumn.Id);
		}

		private ItemList2 itemList;

		private Column groupByColumn;

		private SortOrder sortOrder;

		private IGroupRange[] groupRange;
	}
}
