using System;
using System.Text;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	public class DiscoverySearchStats
	{
		internal DiscoverySearchStats()
		{
			this.TotalItemsCopied = 0L;
			this.UnsearchableItemsAdded = 0L;
			this.EstimatedItems = 0L;
			this.TotalDuplicateItems = 0L;
			this.SkippedErrorItems = 0L;
		}

		public long TotalItemsCopied { get; set; }

		public long TotalDuplicateItems { get; set; }

		public long EstimatedItems { get; set; }

		public long UnsearchableItemsAdded { get; set; }

		public long SkippedErrorItems { get; set; }

		public static DiscoverySearchStats Parse(string value)
		{
			long estimatedItems = 0L;
			long unsearchableItemsAdded = 0L;
			long totalDuplicateItems = 0L;
			long totalItemsCopied = 0L;
			long skippedErrorItems = 0L;
			if (!string.IsNullOrEmpty(value))
			{
				string[] array = value.Split(new char[]
				{
					'\t'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						':'
					});
					string a;
					if (array3 != null && array3.Length > 1 && (a = array3[0]) != null)
					{
						if (!(a == "TotalCopiedItems"))
						{
							if (!(a == "EstimatedItems"))
							{
								if (!(a == "UnsearchableItemsAdded"))
								{
									if (!(a == "DuplicatesRemoved"))
									{
										if (a == "SkippedErrorItems")
										{
											skippedErrorItems = long.Parse(array3[1]);
										}
									}
									else
									{
										totalDuplicateItems = long.Parse(array3[1]);
									}
								}
								else
								{
									unsearchableItemsAdded = long.Parse(array3[1]);
								}
							}
							else
							{
								estimatedItems = long.Parse(array3[1]);
							}
						}
						else
						{
							totalItemsCopied = long.Parse(array3[1]);
						}
					}
				}
			}
			return new DiscoverySearchStats
			{
				TotalItemsCopied = totalItemsCopied,
				UnsearchableItemsAdded = unsearchableItemsAdded,
				EstimatedItems = estimatedItems,
				TotalDuplicateItems = totalDuplicateItems,
				SkippedErrorItems = skippedErrorItems
			};
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.ToStringHelper<long>(stringBuilder, "EstimatedItems", this.EstimatedItems, false);
			this.ToStringHelper<long>(stringBuilder, "UnsearchableItemsAdded", this.UnsearchableItemsAdded, false);
			this.ToStringHelper<long>(stringBuilder, "DuplicatesRemoved", this.TotalDuplicateItems, false);
			this.ToStringHelper<long>(stringBuilder, "SkippedErrorItems", this.SkippedErrorItems, false);
			this.ToStringHelper<long>(stringBuilder, "TotalCopiedItems", this.TotalItemsCopied, true);
			return stringBuilder.ToString();
		}

		public string ToHtmlString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<table>");
			stringBuilder.Append("<tr>");
			stringBuilder.Append("<td>");
			stringBuilder.Append(DataStrings.EstimatedItems);
			stringBuilder.Append("</td>");
			stringBuilder.Append("<td align=\"right\">");
			stringBuilder.Append(this.EstimatedItems);
			stringBuilder.Append("</td>");
			stringBuilder.Append("</tr>");
			stringBuilder.Append("<tr>");
			stringBuilder.Append("<td>");
			stringBuilder.Append(DataStrings.UnsearchableItemsAdded);
			stringBuilder.Append("</td>");
			stringBuilder.Append("<td align=\"right\">+");
			stringBuilder.Append(this.UnsearchableItemsAdded);
			stringBuilder.Append("</td>");
			stringBuilder.Append("</tr>");
			stringBuilder.Append("<tr>");
			stringBuilder.Append("<td>");
			stringBuilder.Append(DataStrings.DuplicatesRemoved);
			stringBuilder.Append("</td>");
			stringBuilder.Append("<td align=\"right\">-");
			stringBuilder.Append(this.TotalDuplicateItems);
			stringBuilder.Append("</td>");
			stringBuilder.Append("</tr>");
			stringBuilder.Append("<tr>");
			stringBuilder.Append("<td>");
			stringBuilder.Append(DataStrings.CopyErrors);
			stringBuilder.Append("</td>");
			stringBuilder.Append("<td align=\"right\">-");
			stringBuilder.Append(this.SkippedErrorItems);
			stringBuilder.Append("</td>");
			stringBuilder.Append("</tr>");
			stringBuilder.Append("<tr>");
			stringBuilder.Append("<td>");
			stringBuilder.Append(DataStrings.TotalCopiedItems);
			stringBuilder.Append("</td>");
			stringBuilder.Append("<td align=\"right\">=");
			stringBuilder.Append(this.TotalItemsCopied);
			stringBuilder.Append("</td>");
			stringBuilder.Append("</tr>");
			stringBuilder.Append("</table>");
			return stringBuilder.ToString();
		}

		private void ToStringHelper<T>(StringBuilder sb, string name, T value, bool isLast)
		{
			sb.Append(name);
			sb.Append(':');
			sb.Append(value);
			if (!isLast)
			{
				sb.Append('\t');
			}
		}

		public const string TotalItemsCopiedName = "TotalCopiedItems";

		public const string UnsearchableItemsAddedName = "UnsearchableItemsAdded";

		public const string EstimatedItemsName = "EstimatedItems";

		public const string TotalDuplicateItemsName = "DuplicatesRemoved";

		public const string SkippedErrorItemsName = "SkippedErrorItems";
	}
}
