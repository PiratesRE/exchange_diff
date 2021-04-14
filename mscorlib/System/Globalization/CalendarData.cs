using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Globalization
{
	internal class CalendarData
	{
		private CalendarData()
		{
		}

		static CalendarData()
		{
			CalendarData calendarData = new CalendarData();
			calendarData.sNativeName = "Gregorian Calendar";
			calendarData.iTwoDigitYearMax = 2029;
			calendarData.iCurrentEra = 1;
			calendarData.saShortDates = new string[]
			{
				"MM/dd/yyyy",
				"yyyy-MM-dd"
			};
			calendarData.saLongDates = new string[]
			{
				"dddd, dd MMMM yyyy"
			};
			calendarData.saYearMonths = new string[]
			{
				"yyyy MMMM"
			};
			calendarData.sMonthDay = "MMMM dd";
			calendarData.saEraNames = new string[]
			{
				"A.D."
			};
			calendarData.saAbbrevEraNames = new string[]
			{
				"AD"
			};
			calendarData.saAbbrevEnglishEraNames = new string[]
			{
				"AD"
			};
			calendarData.saDayNames = new string[]
			{
				"Sunday",
				"Monday",
				"Tuesday",
				"Wednesday",
				"Thursday",
				"Friday",
				"Saturday"
			};
			calendarData.saAbbrevDayNames = new string[]
			{
				"Sun",
				"Mon",
				"Tue",
				"Wed",
				"Thu",
				"Fri",
				"Sat"
			};
			calendarData.saSuperShortDayNames = new string[]
			{
				"Su",
				"Mo",
				"Tu",
				"We",
				"Th",
				"Fr",
				"Sa"
			};
			calendarData.saMonthNames = new string[]
			{
				"January",
				"February",
				"March",
				"April",
				"May",
				"June",
				"July",
				"August",
				"September",
				"October",
				"November",
				"December",
				string.Empty
			};
			calendarData.saAbbrevMonthNames = new string[]
			{
				"Jan",
				"Feb",
				"Mar",
				"Apr",
				"May",
				"Jun",
				"Jul",
				"Aug",
				"Sep",
				"Oct",
				"Nov",
				"Dec",
				string.Empty
			};
			calendarData.saMonthGenitiveNames = calendarData.saMonthNames;
			calendarData.saAbbrevMonthGenitiveNames = calendarData.saAbbrevMonthNames;
			calendarData.saLeapYearMonthNames = calendarData.saMonthNames;
			calendarData.bUseUserOverrides = false;
			CalendarData.Invariant = calendarData;
		}

		internal CalendarData(string localeName, int calendarId, bool bUseUserOverrides)
		{
			this.bUseUserOverrides = bUseUserOverrides;
			if (!CalendarData.nativeGetCalendarData(this, localeName, calendarId))
			{
				if (this.sNativeName == null)
				{
					this.sNativeName = string.Empty;
				}
				if (this.saShortDates == null)
				{
					this.saShortDates = CalendarData.Invariant.saShortDates;
				}
				if (this.saYearMonths == null)
				{
					this.saYearMonths = CalendarData.Invariant.saYearMonths;
				}
				if (this.saLongDates == null)
				{
					this.saLongDates = CalendarData.Invariant.saLongDates;
				}
				if (this.sMonthDay == null)
				{
					this.sMonthDay = CalendarData.Invariant.sMonthDay;
				}
				if (this.saEraNames == null)
				{
					this.saEraNames = CalendarData.Invariant.saEraNames;
				}
				if (this.saAbbrevEraNames == null)
				{
					this.saAbbrevEraNames = CalendarData.Invariant.saAbbrevEraNames;
				}
				if (this.saAbbrevEnglishEraNames == null)
				{
					this.saAbbrevEnglishEraNames = CalendarData.Invariant.saAbbrevEnglishEraNames;
				}
				if (this.saDayNames == null)
				{
					this.saDayNames = CalendarData.Invariant.saDayNames;
				}
				if (this.saAbbrevDayNames == null)
				{
					this.saAbbrevDayNames = CalendarData.Invariant.saAbbrevDayNames;
				}
				if (this.saSuperShortDayNames == null)
				{
					this.saSuperShortDayNames = CalendarData.Invariant.saSuperShortDayNames;
				}
				if (this.saMonthNames == null)
				{
					this.saMonthNames = CalendarData.Invariant.saMonthNames;
				}
				if (this.saAbbrevMonthNames == null)
				{
					this.saAbbrevMonthNames = CalendarData.Invariant.saAbbrevMonthNames;
				}
			}
			this.saShortDates = CultureData.ReescapeWin32Strings(this.saShortDates);
			this.saLongDates = CultureData.ReescapeWin32Strings(this.saLongDates);
			this.saYearMonths = CultureData.ReescapeWin32Strings(this.saYearMonths);
			this.sMonthDay = CultureData.ReescapeWin32String(this.sMonthDay);
			if ((ushort)calendarId == 4)
			{
				if (CultureInfo.IsTaiwanSku)
				{
					this.sNativeName = "中華民國曆";
				}
				else
				{
					this.sNativeName = string.Empty;
				}
			}
			if (this.saMonthGenitiveNames == null || string.IsNullOrEmpty(this.saMonthGenitiveNames[0]))
			{
				this.saMonthGenitiveNames = this.saMonthNames;
			}
			if (this.saAbbrevMonthGenitiveNames == null || string.IsNullOrEmpty(this.saAbbrevMonthGenitiveNames[0]))
			{
				this.saAbbrevMonthGenitiveNames = this.saAbbrevMonthNames;
			}
			if (this.saLeapYearMonthNames == null || string.IsNullOrEmpty(this.saLeapYearMonthNames[0]))
			{
				this.saLeapYearMonthNames = this.saMonthNames;
			}
			this.InitializeEraNames(localeName, calendarId);
			this.InitializeAbbreviatedEraNames(localeName, calendarId);
			if (calendarId == 3)
			{
				this.saAbbrevEnglishEraNames = JapaneseCalendar.EnglishEraNames();
			}
			else
			{
				this.saAbbrevEnglishEraNames = new string[]
				{
					""
				};
			}
			this.iCurrentEra = this.saEraNames.Length;
		}

		private void InitializeEraNames(string localeName, int calendarId)
		{
			switch ((ushort)calendarId)
			{
			case 1:
				if (this.saEraNames == null || this.saEraNames.Length == 0 || string.IsNullOrEmpty(this.saEraNames[0]))
				{
					this.saEraNames = new string[]
					{
						"A.D."
					};
					return;
				}
				return;
			case 2:
			case 13:
				this.saEraNames = new string[]
				{
					"A.D."
				};
				return;
			case 3:
			case 14:
				this.saEraNames = JapaneseCalendar.EraNames();
				return;
			case 4:
				if (CultureInfo.IsTaiwanSku)
				{
					this.saEraNames = new string[]
					{
						"中華民國"
					};
					return;
				}
				this.saEraNames = new string[]
				{
					string.Empty
				};
				return;
			case 5:
				this.saEraNames = new string[]
				{
					"단기"
				};
				return;
			case 6:
			case 23:
				if (localeName == "dv-MV")
				{
					this.saEraNames = new string[]
					{
						"ހިޖްރީ"
					};
					return;
				}
				this.saEraNames = new string[]
				{
					"بعد الهجرة"
				};
				return;
			case 7:
				this.saEraNames = new string[]
				{
					"พ.ศ."
				};
				return;
			case 8:
				this.saEraNames = new string[]
				{
					"C.E."
				};
				return;
			case 9:
				this.saEraNames = new string[]
				{
					"ap. J.-C."
				};
				return;
			case 10:
			case 11:
			case 12:
				this.saEraNames = new string[]
				{
					"م"
				};
				return;
			case 22:
				if (this.saEraNames == null || this.saEraNames.Length == 0 || string.IsNullOrEmpty(this.saEraNames[0]))
				{
					this.saEraNames = new string[]
					{
						"ه.ش"
					};
					return;
				}
				return;
			}
			this.saEraNames = CalendarData.Invariant.saEraNames;
		}

		private void InitializeAbbreviatedEraNames(string localeName, int calendarId)
		{
			CalendarId calendarId2 = (CalendarId)calendarId;
			if (calendarId2 <= CalendarId.JULIAN)
			{
				switch (calendarId2)
				{
				case CalendarId.GREGORIAN:
					if (this.saAbbrevEraNames == null || this.saAbbrevEraNames.Length == 0 || string.IsNullOrEmpty(this.saAbbrevEraNames[0]))
					{
						this.saAbbrevEraNames = new string[]
						{
							"AD"
						};
						return;
					}
					return;
				case CalendarId.GREGORIAN_US:
					break;
				case CalendarId.JAPAN:
					goto IL_96;
				case CalendarId.TAIWAN:
					this.saAbbrevEraNames = new string[1];
					if (this.saEraNames[0].Length == 4)
					{
						this.saAbbrevEraNames[0] = this.saEraNames[0].Substring(2, 2);
						return;
					}
					this.saAbbrevEraNames[0] = this.saEraNames[0];
					return;
				case CalendarId.KOREA:
					goto IL_14B;
				case CalendarId.HIJRI:
					goto IL_A2;
				default:
					if (calendarId2 != CalendarId.JULIAN)
					{
						goto IL_14B;
					}
					break;
				}
				this.saAbbrevEraNames = new string[]
				{
					"AD"
				};
				return;
			}
			if (calendarId2 != CalendarId.JAPANESELUNISOLAR)
			{
				if (calendarId2 != CalendarId.PERSIAN)
				{
					if (calendarId2 != CalendarId.UMALQURA)
					{
						goto IL_14B;
					}
					goto IL_A2;
				}
				else
				{
					if (this.saAbbrevEraNames == null || this.saAbbrevEraNames.Length == 0 || string.IsNullOrEmpty(this.saAbbrevEraNames[0]))
					{
						this.saAbbrevEraNames = this.saEraNames;
						return;
					}
					return;
				}
			}
			IL_96:
			this.saAbbrevEraNames = JapaneseCalendar.AbbrevEraNames();
			return;
			IL_A2:
			if (localeName == "dv-MV")
			{
				this.saAbbrevEraNames = new string[]
				{
					"ހ."
				};
				return;
			}
			this.saAbbrevEraNames = new string[]
			{
				"هـ"
			};
			return;
			IL_14B:
			this.saAbbrevEraNames = this.saEraNames;
		}

		internal static CalendarData GetCalendarData(int calendarId)
		{
			string name = CalendarData.CalendarIdToCultureName(calendarId);
			return CultureInfo.GetCultureInfo(name).m_cultureData.GetCalendar(calendarId);
		}

		private static string CalendarIdToCultureName(int calendarId)
		{
			switch (calendarId)
			{
			case 2:
				return "fa-IR";
			case 3:
				return "ja-JP";
			case 4:
				return "zh-TW";
			case 5:
				return "ko-KR";
			case 6:
			case 10:
			case 23:
				return "ar-SA";
			case 7:
				return "th-TH";
			case 8:
				return "he-IL";
			case 9:
				return "ar-DZ";
			case 11:
			case 12:
				return "ar-IQ";
			}
			return "en-US";
		}

		internal void FixupWin7MonthDaySemicolonBug()
		{
			int num = CalendarData.FindUnescapedCharacter(this.sMonthDay, ';');
			if (num > 0)
			{
				this.sMonthDay = this.sMonthDay.Substring(0, num);
			}
		}

		private static int FindUnescapedCharacter(string s, char charToFind)
		{
			bool flag = false;
			int length = s.Length;
			for (int i = 0; i < length; i++)
			{
				char c = s[i];
				if (c != '\'')
				{
					if (c != '\\')
					{
						if (!flag && charToFind == c)
						{
							return i;
						}
					}
					else
					{
						i++;
					}
				}
				else
				{
					flag = !flag;
				}
			}
			return -1;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int nativeGetTwoDigitYearMax(int calID);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool nativeGetCalendarData(CalendarData data, string localeName, int calendar);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int nativeGetCalendars(string localeName, bool useUserOverride, [In] [Out] int[] calendars);

		internal const int MAX_CALENDARS = 23;

		internal string sNativeName;

		internal string[] saShortDates;

		internal string[] saYearMonths;

		internal string[] saLongDates;

		internal string sMonthDay;

		internal string[] saEraNames;

		internal string[] saAbbrevEraNames;

		internal string[] saAbbrevEnglishEraNames;

		internal string[] saDayNames;

		internal string[] saAbbrevDayNames;

		internal string[] saSuperShortDayNames;

		internal string[] saMonthNames;

		internal string[] saAbbrevMonthNames;

		internal string[] saMonthGenitiveNames;

		internal string[] saAbbrevMonthGenitiveNames;

		internal string[] saLeapYearMonthNames;

		internal int iTwoDigitYearMax = 2029;

		internal int iCurrentEra;

		internal bool bUseUserOverrides;

		internal static CalendarData Invariant;
	}
}
