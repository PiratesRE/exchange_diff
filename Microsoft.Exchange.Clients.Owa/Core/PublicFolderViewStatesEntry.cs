using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class PublicFolderViewStatesEntry
	{
		internal PublicFolderViewStatesEntry(string folderId)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			this.folderId = folderId;
		}

		internal PublicFolderViewStatesEntry()
		{
		}

		internal bool Dirty
		{
			get
			{
				return this.dirty;
			}
		}

		internal string FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		internal long LastAccessedDateTimeTicks
		{
			get
			{
				return this.lastAccessedDateTimeTicks;
			}
			set
			{
				this.lastAccessedDateTimeTicks = value;
			}
		}

		internal void UpdateLastAccessedDateTimeTicks()
		{
			this.lastAccessedDateTimeTicks = ExDateTime.UtcNow.UtcTicks;
		}

		internal int CalendarViewType
		{
			get
			{
				if (this.calendarViewType != null)
				{
					return this.calendarViewType.Value;
				}
				return 1;
			}
			set
			{
				this.calendarViewType = new int?(value);
				this.dirty = true;
			}
		}

		internal int DailyViewDays
		{
			get
			{
				if (this.dailyViewDays != null)
				{
					return this.dailyViewDays.Value;
				}
				return 1;
			}
			set
			{
				this.dailyViewDays = new int?(value);
				this.dirty = true;
			}
		}

		internal bool? MultiLine
		{
			get
			{
				return this.multiLine;
			}
			set
			{
				this.multiLine = value;
				this.dirty = true;
			}
		}

		internal int? ReadingPanePosition
		{
			get
			{
				return this.readingPanePosition;
			}
			set
			{
				this.readingPanePosition = value;
				this.dirty = true;
			}
		}

		internal int ReadingPanePositionMultiDay
		{
			get
			{
				if (this.readingPanePositionMultiDay != null)
				{
					return this.readingPanePositionMultiDay.Value;
				}
				return 0;
			}
			set
			{
				this.readingPanePositionMultiDay = new int?(value);
				this.dirty = true;
			}
		}

		internal string SortColumn
		{
			get
			{
				return this.sortColumn;
			}
			set
			{
				this.sortColumn = value;
				this.dirty = true;
			}
		}

		internal int? SortOrder
		{
			get
			{
				return this.sortOrder;
			}
			set
			{
				this.sortOrder = value;
				this.dirty = true;
			}
		}

		internal int? ViewHeight
		{
			get
			{
				return this.viewHeight;
			}
			set
			{
				this.viewHeight = value;
				this.dirty = true;
			}
		}

		internal int? ViewWidth
		{
			get
			{
				return this.viewWidth;
			}
			set
			{
				this.viewWidth = value;
				this.dirty = true;
			}
		}

		internal void RenderEntryXml(XmlTextWriter xmlWriter, string entryName)
		{
			if (xmlWriter == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (entryName == null)
			{
				throw new ArgumentNullException("entryName");
			}
			xmlWriter.WriteStartElement(entryName);
			xmlWriter.WriteAttributeString("folderId", this.folderId);
			xmlWriter.WriteAttributeString("lastAccessedDateTimeTicks", this.lastAccessedDateTimeTicks.ToString());
			if (this.calendarViewType != null)
			{
				xmlWriter.WriteAttributeString("calendarViewType", this.calendarViewType.Value.ToString());
			}
			if (this.dailyViewDays != null)
			{
				xmlWriter.WriteAttributeString("dailyViewDays", this.dailyViewDays.Value.ToString());
			}
			if (this.multiLine != null)
			{
				xmlWriter.WriteAttributeString("multiLine", this.multiLine.Value.ToString());
			}
			if (this.readingPanePosition != null)
			{
				xmlWriter.WriteAttributeString("readingPanePosition", this.readingPanePosition.Value.ToString());
			}
			if (this.readingPanePositionMultiDay != null)
			{
				xmlWriter.WriteAttributeString("readingPanePositionMultiDay", this.readingPanePositionMultiDay.Value.ToString());
			}
			if (!string.IsNullOrEmpty(this.sortColumn))
			{
				xmlWriter.WriteAttributeString("sortColumn", this.sortColumn);
			}
			if (this.sortOrder != null)
			{
				xmlWriter.WriteAttributeString("sortOrder", this.sortOrder.Value.ToString());
			}
			if (this.viewHeight != null)
			{
				xmlWriter.WriteAttributeString("viewHeight", this.viewHeight.Value.ToString());
			}
			if (this.viewWidth != null)
			{
				xmlWriter.WriteAttributeString("viewWidth", this.viewWidth.Value.ToString());
			}
			xmlWriter.WriteFullEndElement();
			this.dirty = false;
		}

		internal void ParseEntry(XmlTextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentException("reader cannot be null");
			}
			try
			{
				if (reader.HasAttributes)
				{
					int i = 0;
					while (i < reader.AttributeCount)
					{
						reader.MoveToAttribute(i);
						string name;
						if ((name = reader.Name) == null)
						{
							goto IL_23A;
						}
						if (<PrivateImplementationDetails>{912F2AED-BF68-4DDC-9379-4CB89AA1AA01}.$$method0x6001267-1 == null)
						{
							<PrivateImplementationDetails>{912F2AED-BF68-4DDC-9379-4CB89AA1AA01}.$$method0x6001267-1 = new Dictionary<string, int>(11)
							{
								{
									"folderId",
									0
								},
								{
									"lastAccessedDateTimeTicks",
									1
								},
								{
									"calendarViewType",
									2
								},
								{
									"dailyViewDays",
									3
								},
								{
									"multiLine",
									4
								},
								{
									"readingPanePosition",
									5
								},
								{
									"readingPanePositionMultiDay",
									6
								},
								{
									"sortColumn",
									7
								},
								{
									"sortOrder",
									8
								},
								{
									"viewHeight",
									9
								},
								{
									"viewWidth",
									10
								}
							};
						}
						int num;
						if (!<PrivateImplementationDetails>{912F2AED-BF68-4DDC-9379-4CB89AA1AA01}.$$method0x6001267-1.TryGetValue(name, out num))
						{
							goto IL_23A;
						}
						switch (num)
						{
						case 0:
							this.folderId = reader.Value;
							break;
						case 1:
							this.lastAccessedDateTimeTicks = long.Parse(reader.Value);
							break;
						case 2:
						{
							int num2 = int.Parse(reader.Value);
							if (FolderViewStates.ValidateCalendarViewType((CalendarViewType)num2))
							{
								this.calendarViewType = new int?(num2);
							}
							break;
						}
						case 3:
						{
							int num2 = int.Parse(reader.Value);
							if (num2 > 7)
							{
								num2 = 7;
							}
							else if (num2 < 1)
							{
								num2 = 1;
							}
							this.dailyViewDays = new int?(num2);
							break;
						}
						case 4:
							this.multiLine = new bool?(Convert.ToBoolean(reader.Value));
							break;
						case 5:
							this.readingPanePosition = new int?(int.Parse(reader.Value));
							break;
						case 6:
							this.readingPanePositionMultiDay = new int?(int.Parse(reader.Value));
							break;
						case 7:
							this.sortColumn = reader.Value;
							break;
						case 8:
							this.sortOrder = new int?(int.Parse(reader.Value));
							break;
						case 9:
							this.viewHeight = new int?(int.Parse(reader.Value));
							break;
						case 10:
							this.viewWidth = new int?(int.Parse(reader.Value));
							break;
						default:
							goto IL_23A;
						}
						IL_240:
						i++;
						continue;
						IL_23A:
						PublicFolderViewStatesEntry.ThrowParserException(reader);
						goto IL_240;
					}
					reader.MoveToElement();
				}
			}
			catch (FormatException)
			{
				PublicFolderViewStatesEntry.ThrowParserException(reader);
			}
			catch (OverflowException)
			{
				PublicFolderViewStatesEntry.ThrowParserException(reader);
			}
			if (this.folderId == null)
			{
				PublicFolderViewStatesEntry.ThrowParserException(reader);
			}
		}

		private static void ThrowParserException(XmlTextReader reader)
		{
			throw new OwaPublicFolderViewStatesParseException(string.Format(CultureInfo.InvariantCulture, "Invalid request. Line number: {0} Position: {1}.{2}", new object[]
			{
				reader.LineNumber.ToString(CultureInfo.InvariantCulture),
				reader.LinePosition.ToString(CultureInfo.InvariantCulture),
				"Parsing PublicFolderViewStates' Entry"
			}), null, null);
		}

		private const string LastAccessedDateTimeTicksAttribute = "lastAccessedDateTimeTicks";

		private const string FolderIdAttribute = "folderId";

		private const string CalendarViewTypeAttribute = "calendarViewType";

		private const string DailyViewDaysAttribute = "dailyViewDays";

		private const string MultiLineAttribute = "multiLine";

		private const string ReadingPanePositionAttribute = "readingPanePosition";

		private const string ReadingPanePositionMultiDayAttribute = "readingPanePositionMultiDay";

		private const string SortColumnAttribute = "sortColumn";

		private const string SortOrderAttribute = "sortOrder";

		private const string ViewHeightAttribute = "viewHeight";

		private const string ViewWidthAttribute = "viewWidth";

		private long lastAccessedDateTimeTicks;

		private string folderId;

		private bool dirty;

		private int? calendarViewType;

		private int? dailyViewDays;

		private bool? multiLine;

		private int? readingPanePosition;

		private int? readingPanePositionMultiDay;

		private string sortColumn;

		private int? sortOrder;

		private int? viewHeight;

		private int? viewWidth;

		internal class LastAccessDateTimeTicksComparer : IComparer<PublicFolderViewStatesEntry>
		{
			public int Compare(PublicFolderViewStatesEntry x, PublicFolderViewStatesEntry y)
			{
				if (x.LastAccessedDateTimeTicks < y.LastAccessedDateTimeTicks)
				{
					return 1;
				}
				if (x.LastAccessedDateTimeTicks > y.LastAccessedDateTimeTicks)
				{
					return -1;
				}
				return 0;
			}
		}
	}
}
