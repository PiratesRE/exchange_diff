using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.TextMessaging.MobileDriver.Resources;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal abstract class PureSplitterBase : ISplitter
	{
		public abstract bool OnePass { get; }

		public abstract PartType PartType { get; }

		public BookmarkRetriever Split(string text)
		{
			bool flag = false;
			return new BookmarkRetriever(this.PartType, this.Split(text, null, -1, out flag));
		}

		public BookmarkRetriever NumberedSplit(ProportionedText.PresentationContent group, string paddingFormat)
		{
			string text = group.ToString();
			BookmarkRetriever bookmarkRetriever = this.Split(text);
			int count = bookmarkRetriever.Parts.Count;
			if (1 >= count)
			{
				return bookmarkRetriever;
			}
			if (!group.IsGrouped)
			{
				StringBuilder stringBuilder = new StringBuilder(text);
				List<Bookmark> list = new List<Bookmark>(new Bookmark[]
				{
					bookmarkRetriever.Segments[0]
				});
				int num = -1;
				string text2 = null;
				bool flag = false;
				for (;;)
				{
					if (list[list.Count - 1].PartNumber != num)
					{
						if (-1 != num && text2.Length >= new BookmarkRetriever(this.PartType, list).Parts[num].CharacterCount)
						{
							break;
						}
						num = list[list.Count - 1].PartNumber;
						text2 = string.Format(paddingFormat, 1 + num, count);
						stringBuilder.Insert(list[list.Count - 1].BeginLocation, text2);
						list.RemoveAt(list.Count - 1);
					}
					IList<Bookmark> collection = this.Split(stringBuilder.ToString(), list, 1, out flag);
					list.Clear();
					list.AddRange(collection);
					if (list[list.Count - 1].PartNumber >= count)
					{
						count = new BookmarkRetriever(this.PartType, this.Split(stringBuilder.ToString(), list, -1, out flag)).Parts.Count;
						stringBuilder.Length = 0;
						stringBuilder.Append(text);
						list.Clear();
						list.Add(bookmarkRetriever.Segments[0]);
						num = -1;
						flag = true;
					}
					if (!flag && list[list.Count - 1].PartNumber == num)
					{
						goto Block_8;
					}
				}
				throw new MobileDriverDataException(Strings.ErrorTooManyParts);
				Block_8:
				BookmarkHelper.RebuildBookmarksWithNewText(list, list[list.Count - 1].FullText);
				return new BookmarkRetriever(this.PartType, list);
			}
			bookmarkRetriever = this.Split(text);
			count = bookmarkRetriever.Parts.Count;
			return this.NumberedSplit(group, count, paddingFormat, 0);
		}

		private BookmarkRetriever NumberedSplit(ProportionedText.PresentationContent group, int messageCount, string paddingFormat, int depth)
		{
			if (depth > 100)
			{
				throw new MobileDriverDataException(Strings.ErrorTooManyParts);
			}
			int num = 0;
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder(200);
			BookmarkRetriever bookmarkRetriever = null;
			List<Bookmark> list = new List<Bookmark>();
			int num3 = 0;
			int num4 = 0;
			string value = string.Format(paddingFormat, ++num4, messageCount);
			stringBuilder.Append(value);
			bool flag = true;
			int num5 = 0;
			while (group.PresentationTexts.Count > num5)
			{
				if (!flag || !string.IsNullOrEmpty(group.PresentationTexts[num5].Text.Trim()))
				{
					flag = false;
					if (group.PresentationTexts[num5].GroupId != num3)
					{
						num3 = group.PresentationTexts[num5].GroupId;
						BookmarkRetriever bookmarkRetriever2 = this.Split(stringBuilder.ToString());
						if (bookmarkRetriever2.Parts.Count == 1)
						{
							num = num5;
							bookmarkRetriever = bookmarkRetriever2;
						}
						else
						{
							if (num == num2)
							{
								return null;
							}
							num5 = num + 1;
							num2 = num;
							num3 = group.PresentationTexts[num5].GroupId;
							list.Add(bookmarkRetriever.Parts[0]);
							stringBuilder = new StringBuilder();
							if (messageCount > 1)
							{
								value = string.Format(paddingFormat, ++num4, messageCount);
							}
							stringBuilder.Append(value);
							flag = true;
						}
					}
					else
					{
						stringBuilder.Append(group.PresentationTexts[num5].Text);
					}
				}
				num5++;
			}
			if (!flag)
			{
				BookmarkRetriever bookmarkRetriever2 = this.Split(stringBuilder.ToString());
				list.Add(bookmarkRetriever2.Parts[0]);
			}
			if (messageCount < list.Count)
			{
				return this.NumberedSplit(group, list.Count, paddingFormat, depth + 1);
			}
			return new BookmarkRetriever(this.PartType, list);
		}

		internal abstract IList<Bookmark> Split(string text, IEnumerable<Bookmark> existing, int desiredCount, out bool more);
	}
}
