using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class TimeGroupByList2 : GroupByList2
	{
		public TimeGroupByList2(ColumnId sortedColumn, SortOrder sortOrder, ItemList2 itemList, UserContext userContext) : base(sortedColumn, sortOrder, itemList, userContext)
		{
			List<TimeRange> timeRanges = TimeRange.GetTimeRanges(userContext);
			TimeGroupByList2.TimeGroupRange[] array = new TimeGroupByList2.TimeGroupRange[timeRanges.Count];
			for (int i = 0; i < timeRanges.Count; i++)
			{
				array[i] = new TimeGroupByList2.TimeGroupRange(timeRanges[i]);
				if (timeRanges[i].Range == TimeRange.RangeId.Today)
				{
					this.todayIndex = i;
				}
			}
			base.SetGroupRange(array);
		}

		public bool IsValid()
		{
			TimeGroupByList2.TimeGroupRange timeGroupRange = base.GroupRange[this.todayIndex] as TimeGroupByList2.TimeGroupRange;
			return timeGroupRange == null || timeGroupRange.IsRangeValid(base.UserContext);
		}

		private int todayIndex;

		private class TimeGroupRange : IGroupRange
		{
			public string Header
			{
				get
				{
					string result = null;
					Strings.IDs? ds = null;
					int range = (int)this.timeRange.Range;
					if (range <= 2048)
					{
						if (range <= 64)
						{
							if (range <= 8)
							{
								switch (range)
								{
								case 1:
									ds = new Strings.IDs?(-1711898952);
									goto IL_26B;
								case 2:
									ds = new Strings.IDs?(344592200);
									goto IL_26B;
								case 3:
									goto IL_26B;
								case 4:
									ds = new Strings.IDs?(-1914587312);
									goto IL_26B;
								default:
									if (range != 8)
									{
										goto IL_26B;
									}
									ds = new Strings.IDs?(189160812);
									goto IL_26B;
								}
							}
							else
							{
								if (range == 16)
								{
									ds = new Strings.IDs?(2044325730);
									goto IL_26B;
								}
								if (range == 32)
								{
									ds = new Strings.IDs?(379600702);
									goto IL_26B;
								}
								if (range != 64)
								{
									goto IL_26B;
								}
							}
						}
						else if (range <= 256)
						{
							if (range != 128 && range != 256)
							{
								goto IL_26B;
							}
						}
						else if (range != 512 && range != 1024 && range != 2048)
						{
							goto IL_26B;
						}
					}
					else if (range <= 65536)
					{
						if (range <= 8192)
						{
							if (range != 4096)
							{
								if (range != 8192)
								{
									goto IL_26B;
								}
								ds = new Strings.IDs?(1433854051);
								goto IL_26B;
							}
						}
						else
						{
							if (range == 16384)
							{
								ds = new Strings.IDs?(-642258687);
								goto IL_26B;
							}
							if (range == 32768)
							{
								ds = new Strings.IDs?(-515206901);
								goto IL_26B;
							}
							if (range != 65536)
							{
								goto IL_26B;
							}
							ds = new Strings.IDs?(1907117282);
							goto IL_26B;
						}
					}
					else if (range <= 524288)
					{
						if (range == 131072)
						{
							ds = new Strings.IDs?(1040160067);
							goto IL_26B;
						}
						if (range == 262144)
						{
							ds = new Strings.IDs?(-1537757390);
							goto IL_26B;
						}
						if (range != 524288)
						{
							goto IL_26B;
						}
						ds = new Strings.IDs?(-1577962566);
						goto IL_26B;
					}
					else
					{
						if (range == 1048576)
						{
							ds = new Strings.IDs?(-367521373);
							goto IL_26B;
						}
						if (range == 2097152)
						{
							ds = new Strings.IDs?(1854511297);
							goto IL_26B;
						}
						if (range != 4194304)
						{
							goto IL_26B;
						}
						ds = new Strings.IDs?(1414246128);
						goto IL_26B;
					}
					result = this.timeRange.Start.ToString("dddd", CultureInfo.CurrentCulture.DateTimeFormat);
					IL_26B:
					if (ds != null)
					{
						result = LocalizedStrings.GetNonEncoded(ds.Value);
					}
					return result;
				}
			}

			public TimeGroupRange(TimeRange timeRange)
			{
				this.timeRange = timeRange;
			}

			public bool IsInGroup(IListViewDataSource dataSource, Column column)
			{
				ExDateTime itemProperty = dataSource.GetItemProperty<ExDateTime>(column[0], ExDateTime.MinValue);
				return this.DoesRangeMatch(itemProperty);
			}

			public bool IsRangeValid(UserContext userContext)
			{
				if (this.timeRange.Range != TimeRange.RangeId.Today)
				{
					throw new InvalidOperationException("This method can be called only on timerange with id Today");
				}
				ExDateTime date = DateTimeUtilities.GetLocalTime(userContext).Date;
				return this.DoesRangeMatch(date);
			}

			private bool DoesRangeMatch(ExDateTime date)
			{
				return (this.timeRange.Start <= date && date < this.timeRange.End) || (this.timeRange.Start == date && this.timeRange.Start == this.timeRange.End);
			}

			private TimeRange timeRange;
		}
	}
}
