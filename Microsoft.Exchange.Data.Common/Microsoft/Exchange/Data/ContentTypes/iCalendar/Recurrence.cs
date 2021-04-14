using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	public class Recurrence
	{
		static Recurrence()
		{
			Recurrence.frequencyToEnumTable.Add("SECONDLY", Frequency.Secondly);
			Recurrence.frequencyToEnumTable.Add("MINUTELY", Frequency.Minutely);
			Recurrence.frequencyToEnumTable.Add("HOURLY", Frequency.Hourly);
			Recurrence.frequencyToEnumTable.Add("DAILY", Frequency.Daily);
			Recurrence.frequencyToEnumTable.Add("WEEKLY", Frequency.Weekly);
			Recurrence.frequencyToEnumTable.Add("MONTHLY", Frequency.Monthly);
			Recurrence.frequencyToEnumTable.Add("YEARLY", Frequency.Yearly);
			Recurrence.recurPropToEnumTable.Add("FREQ", RecurrenceProperties.Frequency);
			Recurrence.recurPropToEnumTable.Add("UNTIL", RecurrenceProperties.UntilDate);
			Recurrence.recurPropToEnumTable.Add("COUNT", RecurrenceProperties.Count);
			Recurrence.recurPropToEnumTable.Add("INTERVAL", RecurrenceProperties.Interval);
			Recurrence.recurPropToEnumTable.Add("BYSECOND", RecurrenceProperties.BySecond);
			Recurrence.recurPropToEnumTable.Add("BYMINUTE", RecurrenceProperties.ByMinute);
			Recurrence.recurPropToEnumTable.Add("BYHOUR", RecurrenceProperties.ByHour);
			Recurrence.recurPropToEnumTable.Add("BYDAY", RecurrenceProperties.ByDay);
			Recurrence.recurPropToEnumTable.Add("BYMONTHDAY", RecurrenceProperties.ByMonthDay);
			Recurrence.recurPropToEnumTable.Add("BYYEARDAY", RecurrenceProperties.ByYearDay);
			Recurrence.recurPropToEnumTable.Add("BYWEEKNO", RecurrenceProperties.ByWeek);
			Recurrence.recurPropToEnumTable.Add("BYMONTH", RecurrenceProperties.ByMonth);
			Recurrence.recurPropToEnumTable.Add("BYSETPOS", RecurrenceProperties.BySetPosition);
			Recurrence.recurPropToEnumTable.Add("WKST", RecurrenceProperties.WeekStart);
		}

		public Recurrence()
		{
		}

		public Recurrence(string value) : this(value, null)
		{
		}

		internal Recurrence(string s, ComplianceTracker tracker)
		{
			this.tracker = tracker;
			Recurrence.ParserStates parserStates = Recurrence.ParserStates.Name;
			int length = s.Length;
			string s2 = string.Empty;
			List<string> list = new List<string>();
			int i = 0;
			while (i < length)
			{
				switch (parserStates)
				{
				case Recurrence.ParserStates.Name:
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (i < length)
					{
						char c = s[i++];
						if ((int)c >= ContentLineParser.Dictionary.Length || (byte)(ContentLineParser.Dictionary[(int)c] & ContentLineParser.Tokens.ValueChar) == 0)
						{
							this.SetComplianceStatus(CalendarStrings.InvalidCharacterInRecurrence);
							return;
						}
						if (c == '=')
						{
							break;
						}
						stringBuilder.Append(c);
					}
					s2 = stringBuilder.ToString();
					parserStates = Recurrence.ParserStates.Value;
					break;
				}
				case Recurrence.ParserStates.Value:
				{
					bool flag = false;
					StringBuilder stringBuilder = new StringBuilder();
					while (i < length)
					{
						char c = s[i++];
						if ((int)c >= ContentLineParser.Dictionary.Length || (byte)(ContentLineParser.Dictionary[(int)c] & ContentLineParser.Tokens.ValueChar) == 0)
						{
							this.SetComplianceStatus(CalendarStrings.InvalidCharacterInRecurrence);
							return;
						}
						if (c == ';')
						{
							flag = true;
							parserStates = Recurrence.ParserStates.Name;
							break;
						}
						if (c == ',')
						{
							flag = false;
							break;
						}
						stringBuilder.Append(c);
					}
					list.Add(stringBuilder.ToString());
					if (flag || i == length)
					{
						int num = list.Count;
						RecurrenceProperties recurProp = Recurrence.GetRecurProp(s2);
						if (recurProp <= RecurrenceProperties.ByDay)
						{
							if (recurProp <= RecurrenceProperties.BySecond)
							{
								switch (recurProp)
								{
								case RecurrenceProperties.Frequency:
									if (num > 1)
									{
										this.SetComplianceStatus(CalendarStrings.MultivalueNotPermittedOnFreq);
										return;
									}
									this.freq = Recurrence.GetFrequency(list[0]);
									if (this.freq == Frequency.Unknown)
									{
										this.SetComplianceStatus(CalendarStrings.UnknownFrequencyValue);
										return;
									}
									this.props |= RecurrenceProperties.Frequency;
									break;
								case RecurrenceProperties.UntilDate:
									if (num > 1)
									{
										this.SetComplianceStatus(CalendarStrings.MultivalueNotPermittedOnUntil);
										return;
									}
									if ((this.props & RecurrenceProperties.UntilDate) != RecurrenceProperties.None || (this.props & RecurrenceProperties.UntilDateTime) != RecurrenceProperties.None)
									{
										this.SetComplianceStatus(CalendarStrings.UntilOnlyPermittedOnce);
										return;
									}
									if ((this.props & RecurrenceProperties.Count) != RecurrenceProperties.None)
									{
										this.SetComplianceStatus(CalendarStrings.UntilNotPermittedWithCount);
										return;
									}
									if (list[0].Length > 8)
									{
										this.untilDateTime = CalendarCommon.ParseDateTime(list[0], tracker);
										this.props |= RecurrenceProperties.UntilDateTime;
									}
									else
									{
										this.untilDate = CalendarCommon.ParseDate(list[0], tracker);
										this.props |= RecurrenceProperties.UntilDate;
									}
									break;
								case RecurrenceProperties.Frequency | RecurrenceProperties.UntilDate:
									goto IL_A5C;
								case RecurrenceProperties.Count:
									if ((this.props & RecurrenceProperties.Count) != RecurrenceProperties.None)
									{
										this.SetComplianceStatus(CalendarStrings.CountOnlyPermittedOnce);
										return;
									}
									if ((this.props & RecurrenceProperties.UntilDate) != RecurrenceProperties.None || (this.props & RecurrenceProperties.UntilDateTime) != RecurrenceProperties.None)
									{
										this.SetComplianceStatus(CalendarStrings.CountNotPermittedWithUntil);
										return;
									}
									if (num > 1)
									{
										this.SetComplianceStatus(CalendarStrings.MultivalueNotPermittedOnCount);
										return;
									}
									if (!int.TryParse(list[0], out this.count))
									{
										this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
									}
									this.props |= RecurrenceProperties.Count;
									break;
								default:
									if (recurProp != RecurrenceProperties.Interval)
									{
										if (recurProp != RecurrenceProperties.BySecond)
										{
											goto IL_A5C;
										}
										if ((this.props & RecurrenceProperties.BySecond) != RecurrenceProperties.None)
										{
											this.SetComplianceStatus(CalendarStrings.BySecondOnlyPermittedOnce);
											return;
										}
										this.bySecond = new int[num];
										for (int j = 0; j < num; j++)
										{
											if (!int.TryParse(list[j], out this.bySecond[j]))
											{
												this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
											}
											if (this.bySecond[j] < 0 || this.bySecond[j] > 59)
											{
												this.SetComplianceStatus(CalendarStrings.BySecondOutOfRange);
												return;
											}
										}
										this.props |= RecurrenceProperties.BySecond;
									}
									else
									{
										if ((this.props & RecurrenceProperties.Interval) != RecurrenceProperties.None)
										{
											this.SetComplianceStatus(CalendarStrings.IntervalOnlyPermittedOnce);
											return;
										}
										if (num > 1)
										{
											this.SetComplianceStatus(CalendarStrings.MultivalueNotPermittedOnInterval);
											return;
										}
										if (!int.TryParse(list[0], out this.interval))
										{
											this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
										}
										if (this.interval < 1)
										{
											this.SetComplianceStatus(CalendarStrings.IntervalMustBePositive);
											return;
										}
										this.props |= RecurrenceProperties.Interval;
									}
									break;
								}
							}
							else if (recurProp != RecurrenceProperties.ByMinute)
							{
								if (recurProp != RecurrenceProperties.ByHour)
								{
									if (recurProp != RecurrenceProperties.ByDay)
									{
										goto IL_A5C;
									}
									if ((this.props & RecurrenceProperties.ByDay) != RecurrenceProperties.None)
									{
										this.SetComplianceStatus(CalendarStrings.ByDayOnlyPermittedOnce);
										return;
									}
									this.byDay = new Recurrence.ByDay[num];
									for (int k = 0; k < num; k++)
									{
										string text = list[k];
										if (text.Length != 0)
										{
											int num2 = 0;
											while (num2 < text.Length && text[num2] == ' ')
											{
												num2++;
											}
											if (num2 != text.Length)
											{
												int num3 = num2 - 1;
												char c2;
												do
												{
													c2 = text[++num3];
													if ((int)c2 >= ContentLineParser.Dictionary.Length)
													{
														goto Block_53;
													}
												}
												while (((byte)(ContentLineParser.Dictionary[(int)c2] & ContentLineParser.Tokens.Digit) > 0 || c2 == '+' || c2 == '-') && num3 + 1 < text.Length);
												IL_66C:
												if (num3 != num2)
												{
													int num4 = 0;
													string s3 = text.Substring(num2, num3 - num2);
													if (!int.TryParse(s3, out num4) || num4 == 0)
													{
														this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
													}
													this.byDay[k].OccurrenceNumber = num4;
												}
												while (text[num3] == ' ' && num3 + 1 < text.Length)
												{
													num3++;
												}
												this.byDay[k].Day = this.GetDayOfWeek(text.Substring(num3, text.Length - num3));
												goto IL_700;
												Block_53:
												this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
												goto IL_66C;
											}
										}
										IL_700:;
									}
									this.props |= RecurrenceProperties.ByDay;
								}
								else
								{
									if ((this.props & RecurrenceProperties.ByHour) != RecurrenceProperties.None)
									{
										this.SetComplianceStatus(CalendarStrings.ByHourOnlyPermittedOnce);
										return;
									}
									this.byHour = new int[num];
									for (int l = 0; l < num; l++)
									{
										if (!int.TryParse(list[l], out this.byHour[l]))
										{
											this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
										}
										if (this.byHour[l] < 0 || this.byHour[l] > 23)
										{
											this.SetComplianceStatus(CalendarStrings.ByHourOutOfRange);
											return;
										}
									}
									this.props |= RecurrenceProperties.ByHour;
								}
							}
							else
							{
								if ((this.props & RecurrenceProperties.ByMinute) != RecurrenceProperties.None)
								{
									this.SetComplianceStatus(CalendarStrings.ByMinuteOnlyPermittedOnce);
									return;
								}
								this.byMinute = new int[num];
								for (int m = 0; m < num; m++)
								{
									if (!int.TryParse(list[m], out this.byMinute[m]))
									{
										this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
									}
									if (this.byMinute[m] < 0 || this.byMinute[m] > 59)
									{
										this.SetComplianceStatus(CalendarStrings.ByMinuteOutOfRange);
										return;
									}
								}
								this.props |= RecurrenceProperties.ByMinute;
							}
						}
						else if (recurProp <= RecurrenceProperties.ByWeek)
						{
							if (recurProp != RecurrenceProperties.ByMonthDay)
							{
								if (recurProp != RecurrenceProperties.ByYearDay)
								{
									if (recurProp != RecurrenceProperties.ByWeek)
									{
										goto IL_A5C;
									}
									if ((this.props & RecurrenceProperties.ByWeek) != RecurrenceProperties.None)
									{
										this.SetComplianceStatus(CalendarStrings.ByWeekNoOnlyPermittedOnce);
										return;
									}
									this.byWeekNumber = new int[num];
									for (int n = 0; n < num; n++)
									{
										int num5;
										if (!int.TryParse(list[n], out num5))
										{
											this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
										}
										this.byWeekNumber[n] = num5;
										if (num5 == 0 || num5 > 53 || num5 < -53)
										{
											this.SetComplianceStatus(CalendarStrings.ByWeekNoOutOfRange);
											return;
										}
									}
									this.props |= RecurrenceProperties.ByWeek;
								}
								else
								{
									if ((this.props & RecurrenceProperties.ByYearDay) != RecurrenceProperties.None)
									{
										this.SetComplianceStatus(CalendarStrings.ByYearDayOnlyPermittedOnce);
										return;
									}
									this.byYearDay = new int[num];
									for (int num6 = 0; num6 < num; num6++)
									{
										int num7;
										if (!int.TryParse(list[num6], out num7))
										{
											this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
										}
										this.byYearDay[num6] = num7;
										if (num7 == 0 || num7 > 366 || num7 < -366)
										{
											this.SetComplianceStatus(CalendarStrings.ByYearDayOutOfRange);
											return;
										}
									}
									this.props |= RecurrenceProperties.ByYearDay;
								}
							}
							else
							{
								if ((this.props & RecurrenceProperties.ByMonthDay) != RecurrenceProperties.None)
								{
									this.SetComplianceStatus(CalendarStrings.ByMonthDayOnlyPermittedOnce);
									return;
								}
								this.byMonthDay = new int[num];
								for (int num8 = 0; num8 < num; num8++)
								{
									int num9;
									if (!int.TryParse(list[num8], out num9))
									{
										this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
									}
									this.byMonthDay[num8] = num9;
									if (num9 == 0 || num9 > 31 || num9 < -31)
									{
										this.SetComplianceStatus(CalendarStrings.ByMonthDayOutOfRange);
										return;
									}
								}
								this.props |= RecurrenceProperties.ByMonthDay;
							}
						}
						else if (recurProp != RecurrenceProperties.ByMonth)
						{
							if (recurProp != RecurrenceProperties.BySetPosition)
							{
								if (recurProp != RecurrenceProperties.WeekStart)
								{
									goto IL_A5C;
								}
								if ((this.props & RecurrenceProperties.WeekStart) != RecurrenceProperties.None)
								{
									this.SetComplianceStatus(CalendarStrings.WkStOnlyPermittedOnce);
									return;
								}
								if (num > 1)
								{
									this.SetComplianceStatus(CalendarStrings.MultivalueNotPermittedOnWkSt);
									return;
								}
								this.workWeekStart = this.GetDayOfWeek(list[0]);
								this.props |= RecurrenceProperties.WeekStart;
							}
							else
							{
								if ((this.props & RecurrenceProperties.BySetPosition) != RecurrenceProperties.None)
								{
									this.SetComplianceStatus(CalendarStrings.BySetPosOnlyPermittedOnce);
									return;
								}
								this.bySetPos = new int[num];
								for (int num10 = 0; num10 < num; num10++)
								{
									int num11;
									if (!int.TryParse(list[num10], out num11))
									{
										this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
									}
									this.bySetPos[num10] = num11;
									if (num11 == 0 || num11 > 366 || num11 < -366)
									{
										this.SetComplianceStatus(CalendarStrings.BySetPosOutOfRange);
										return;
									}
								}
								this.props |= RecurrenceProperties.BySetPosition;
							}
						}
						else
						{
							if ((this.props & RecurrenceProperties.ByMonth) != RecurrenceProperties.None)
							{
								this.SetComplianceStatus(CalendarStrings.ByMonthOnlyPermittedOnce);
								return;
							}
							this.byMonth = new int[num];
							for (int num12 = 0; num12 < num; num12++)
							{
								int num13;
								if (!int.TryParse(list[num12], out num13))
								{
									this.SetComplianceStatus(CalendarStrings.InvalidValueFormat);
								}
								this.byMonth[num12] = num13;
								if (num13 < 0 || num13 > 12)
								{
									this.SetComplianceStatus(CalendarStrings.ByMonthOutOfRange);
									return;
								}
							}
							this.props |= RecurrenceProperties.ByMonth;
						}
						IL_A67:
						list = new List<string>();
						break;
						IL_A5C:
						this.SetComplianceStatus(CalendarStrings.UnknownRecurrenceProperty);
						goto IL_A67;
					}
					break;
				}
				}
			}
		}

		public Frequency Frequency
		{
			get
			{
				return this.freq;
			}
			set
			{
				this.props |= RecurrenceProperties.Frequency;
				this.freq = value;
			}
		}

		public DateTime UntilDate
		{
			get
			{
				return this.untilDate;
			}
			set
			{
				this.props &= ~RecurrenceProperties.UntilDateTime;
				this.props |= RecurrenceProperties.UntilDate;
				this.untilDate = value;
			}
		}

		public DateTime UntilDateTime
		{
			get
			{
				return this.untilDateTime;
			}
			set
			{
				this.props &= ~RecurrenceProperties.UntilDate;
				this.props |= RecurrenceProperties.UntilDateTime;
				this.untilDateTime = value;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
			set
			{
				this.props |= RecurrenceProperties.Count;
				this.count = value;
			}
		}

		public int Interval
		{
			get
			{
				return this.interval;
			}
			set
			{
				this.props |= RecurrenceProperties.Interval;
				this.interval = value;
			}
		}

		public int[] BySecond
		{
			get
			{
				return this.bySecond;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.props |= RecurrenceProperties.BySecond;
				this.bySecond = value;
			}
		}

		public int[] ByMinute
		{
			get
			{
				return this.byMinute;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.props |= RecurrenceProperties.ByMinute;
				this.byMinute = value;
			}
		}

		public int[] ByHour
		{
			get
			{
				return this.byHour;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.props |= RecurrenceProperties.ByHour;
				this.byHour = value;
			}
		}

		public Recurrence.ByDay[] ByDayList
		{
			get
			{
				return this.byDay;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.props |= RecurrenceProperties.ByDay;
				this.byDay = value;
			}
		}

		public int[] ByMonthDay
		{
			get
			{
				return this.byMonthDay;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.props |= RecurrenceProperties.ByMonthDay;
				this.byMonthDay = value;
			}
		}

		public int[] ByYearDay
		{
			get
			{
				return this.byYearDay;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.props |= RecurrenceProperties.ByYearDay;
				this.byYearDay = value;
			}
		}

		public int[] ByWeek
		{
			get
			{
				return this.byWeekNumber;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.props |= RecurrenceProperties.ByWeek;
				this.byWeekNumber = value;
			}
		}

		public int[] ByMonth
		{
			get
			{
				return this.byMonth;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.props |= RecurrenceProperties.ByMonth;
				this.byMonth = value;
			}
		}

		public int[] BySetPosition
		{
			get
			{
				return this.bySetPos;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.props |= RecurrenceProperties.BySetPosition;
				this.bySetPos = value;
			}
		}

		public DayOfWeek WorkWeekStart
		{
			get
			{
				return this.workWeekStart;
			}
			set
			{
				this.props |= RecurrenceProperties.WeekStart;
				this.workWeekStart = value;
			}
		}

		public RecurrenceProperties AvailableProperties
		{
			get
			{
				return this.props;
			}
			set
			{
				this.props = value;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if ((this.props & RecurrenceProperties.Frequency) != RecurrenceProperties.None)
			{
				stringBuilder.Append("FREQ");
				stringBuilder.Append('=');
				stringBuilder.Append(Recurrence.GetFrequencyString(this.freq));
			}
			if ((this.props & RecurrenceProperties.UntilDate) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";UNTIL");
				stringBuilder.Append('=');
				stringBuilder.Append(CalendarCommon.FormatDate(this.untilDate));
			}
			if ((this.props & RecurrenceProperties.UntilDateTime) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";UNTIL");
				stringBuilder.Append('=');
				stringBuilder.Append(CalendarCommon.FormatDateTime(this.untilDateTime));
			}
			if ((this.props & RecurrenceProperties.Count) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";COUNT");
				stringBuilder.Append('=');
				stringBuilder.Append(this.count);
			}
			if ((this.props & RecurrenceProperties.Interval) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";INTERVAL");
				stringBuilder.Append('=');
				stringBuilder.Append(this.interval);
			}
			if ((this.props & RecurrenceProperties.BySecond) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";BYSECOND");
				stringBuilder.Append('=');
				this.OutputList(this.bySecond, stringBuilder);
			}
			if ((this.props & RecurrenceProperties.ByMinute) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";BYMINUTE");
				stringBuilder.Append('=');
				this.OutputList(this.byMinute, stringBuilder);
			}
			if ((this.props & RecurrenceProperties.ByHour) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";BYHOUR");
				stringBuilder.Append('=');
				this.OutputList(this.byHour, stringBuilder);
			}
			if ((this.props & RecurrenceProperties.ByDay) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";BYDAY");
				stringBuilder.Append('=');
				int num = this.byDay.Length;
				if (num > 0)
				{
					stringBuilder.Append(this.byDay[0]);
				}
				for (int i = 1; i < num; i++)
				{
					stringBuilder.Append(',');
					stringBuilder.Append(this.byDay[i].ToString());
				}
			}
			if ((this.props & RecurrenceProperties.ByMonthDay) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";BYMONTHDAY");
				stringBuilder.Append('=');
				this.OutputList(this.byMonthDay, stringBuilder);
			}
			if ((this.props & RecurrenceProperties.ByYearDay) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";BYYEARDAY");
				stringBuilder.Append('=');
				this.OutputList(this.byYearDay, stringBuilder);
			}
			if ((this.props & RecurrenceProperties.ByWeek) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";BYWEEKNO");
				stringBuilder.Append('=');
				this.OutputList(this.byWeekNumber, stringBuilder);
			}
			if ((this.props & RecurrenceProperties.ByMonth) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";BYMONTH");
				stringBuilder.Append('=');
				this.OutputList(this.byMonth, stringBuilder);
			}
			if ((this.props & RecurrenceProperties.BySetPosition) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";BYSETPOS");
				stringBuilder.Append('=');
				this.OutputList(this.bySetPos, stringBuilder);
			}
			if ((this.props & RecurrenceProperties.WeekStart) != RecurrenceProperties.None)
			{
				stringBuilder.Append(";WKST");
				stringBuilder.Append('=');
				stringBuilder.Append(Recurrence.GetDayOfWeekString(this.workWeekStart));
			}
			return stringBuilder.ToString();
		}

		private static Frequency GetFrequency(string s)
		{
			Frequency result;
			if (!Recurrence.frequencyToEnumTable.TryGetValue(s.ToUpper(), out result))
			{
				return Frequency.Unknown;
			}
			return result;
		}

		private static RecurrenceProperties GetRecurProp(string s)
		{
			RecurrenceProperties result;
			if (!Recurrence.recurPropToEnumTable.TryGetValue(s.ToUpper(), out result))
			{
				return RecurrenceProperties.None;
			}
			return result;
		}

		private static string GetDayOfWeekString(DayOfWeek d)
		{
			switch (d)
			{
			case DayOfWeek.Sunday:
				return "SU";
			case DayOfWeek.Monday:
				return "MO";
			case DayOfWeek.Tuesday:
				return "TU";
			case DayOfWeek.Wednesday:
				return "WE";
			case DayOfWeek.Thursday:
				return "TH";
			case DayOfWeek.Friday:
				return "FR";
			case DayOfWeek.Saturday:
				return "SA";
			default:
				return string.Empty;
			}
		}

		private static string GetFrequencyString(Frequency f)
		{
			switch (f)
			{
			case Frequency.Secondly:
				return "SECONDLY";
			case Frequency.Minutely:
				return "MINUTELY";
			case Frequency.Hourly:
				return "HOURLY";
			case Frequency.Daily:
				return "DAILY";
			case Frequency.Weekly:
				return "WEEKLY";
			case Frequency.Monthly:
				return "MONTHLY";
			case Frequency.Yearly:
				return "YEARLY";
			default:
				return string.Empty;
			}
		}

		private DayOfWeek GetDayOfWeek(string s)
		{
			string key;
			switch (key = s.ToUpper())
			{
			case "SU":
				return DayOfWeek.Sunday;
			case "MO":
				return DayOfWeek.Monday;
			case "TU":
				return DayOfWeek.Tuesday;
			case "WE":
				return DayOfWeek.Wednesday;
			case "TH":
				return DayOfWeek.Thursday;
			case "FR":
				return DayOfWeek.Friday;
			case "SA":
				return DayOfWeek.Saturday;
			}
			this.SetComplianceStatus(CalendarStrings.UnknownDayOfWeek);
			return DayOfWeek.Sunday;
		}

		private void OutputList(int[] list, StringBuilder s)
		{
			int num = list.Length;
			if (num > 0)
			{
				s.Append(list[0]);
			}
			for (int i = 1; i < num; i++)
			{
				s.Append(',');
				s.Append(list[i].ToString());
			}
		}

		private void SetComplianceStatus(string message)
		{
			if (this.tracker == null)
			{
				throw new InvalidCalendarDataException(message);
			}
			this.tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
		}

		private static Dictionary<string, Frequency> frequencyToEnumTable = new Dictionary<string, Frequency>();

		private static Dictionary<string, RecurrenceProperties> recurPropToEnumTable = new Dictionary<string, RecurrenceProperties>();

		private Frequency freq;

		private DateTime untilDate;

		private DateTime untilDateTime;

		private int count;

		private int interval = 1;

		private int[] bySecond;

		private int[] byMinute;

		private int[] byHour;

		private Recurrence.ByDay[] byDay;

		private int[] byMonthDay;

		private int[] byYearDay;

		private int[] byWeekNumber;

		private int[] byMonth;

		private int[] bySetPos;

		private RecurrenceProperties props;

		private DayOfWeek workWeekStart = DayOfWeek.Monday;

		private ComplianceTracker tracker;

		private enum ParserStates
		{
			Name,
			Value
		}

		public struct ByDay
		{
			public ByDay(DayOfWeek day, int occurrenceNumber)
			{
				this.day = day;
				this.occurrenceNumber = occurrenceNumber;
			}

			public int OccurrenceNumber
			{
				get
				{
					return this.occurrenceNumber;
				}
				set
				{
					this.occurrenceNumber = value;
				}
			}

			public DayOfWeek Day
			{
				get
				{
					return this.day;
				}
				set
				{
					this.day = value;
				}
			}

			public override string ToString()
			{
				string dayOfWeekString = Recurrence.GetDayOfWeekString(this.day);
				if (this.occurrenceNumber != 0)
				{
					return this.occurrenceNumber.ToString() + dayOfWeekString;
				}
				return dayOfWeekString;
			}

			private int occurrenceNumber;

			private DayOfWeek day;
		}
	}
}
