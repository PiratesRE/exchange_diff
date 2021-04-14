using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class DaySearchRecognitionResultHandler : IMobileSpeechRecognitionResultHandler
	{
		public DaySearchRecognitionResultHandler(ExTimeZone timeZone)
		{
			this.timeZone = timeZone;
		}

		public void ProcessAndFormatSpeechRecognitionResults(string result, out string jsonResponse, out SpeechRecognitionProcessor.SpeechHttpStatus httpStatus)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering DaySearchRecognitionResultHandler.ProcessAndFormatSpeechRecognitionResults");
			jsonResponse = null;
			httpStatus = SpeechRecognitionProcessor.SpeechHttpStatus.Success;
			using (XmlReader xmlReader = XmlReader.Create(new StringReader(result)))
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				CalendarSpeechRecoResultType calendarSpeechRecoResultType = CalendarSpeechRecoResultType.None;
				while (xmlReader.Read())
				{
					if (xmlReader.IsStartElement("Day"))
					{
						string text = xmlReader.ReadString();
						if (!int.TryParse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num) || num < 1 || num > 31)
						{
							throw new ArgumentException("Invalid day value " + text);
						}
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Valid day value read: {0}", num);
					}
					if (xmlReader.IsStartElement("Month"))
					{
						string text2 = xmlReader.ReadString();
						if (!int.TryParse(text2, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num2) || num2 < 1 || num2 > 12)
						{
							throw new ArgumentException("Invalid month value " + text2);
						}
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Valid month value read: {0}", num2);
					}
					if (xmlReader.IsStartElement("Year"))
					{
						string text3 = xmlReader.ReadString();
						if (!int.TryParse(text3, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num3) || num3 < 1999 || num3 > 2100)
						{
							throw new ArgumentException("Invalid year value " + text3);
						}
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Valid month value read: {0}", num3);
					}
					if (xmlReader.IsStartElement("RecoEvent"))
					{
						string a = xmlReader.ReadString();
						if (string.Equals(a, "recoPartialDate", StringComparison.OrdinalIgnoreCase))
						{
							calendarSpeechRecoResultType = CalendarSpeechRecoResultType.PartialDate;
						}
						else
						{
							if (!string.Equals(a, "recoCompleteDate", StringComparison.OrdinalIgnoreCase))
							{
								throw new ArgumentException("Invalid RecoResultType");
							}
							calendarSpeechRecoResultType = CalendarSpeechRecoResultType.CompleteDate;
						}
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<CalendarSpeechRecoResultType>((long)this.GetHashCode(), "Valid RecoEvent value read: {0}", calendarSpeechRecoResultType);
					}
				}
				DayTimeDurationRecoResult[] obj;
				if (calendarSpeechRecoResultType == CalendarSpeechRecoResultType.None)
				{
					obj = new DayTimeDurationRecoResult[0];
				}
				else
				{
					if (num == 0 || num2 == 0 || num3 == 0)
					{
						throw new ArgumentException("No valid results from speech recognition");
					}
					obj = new DayTimeDurationRecoResult[]
					{
						new DayTimeDurationRecoResult
						{
							ResultType = calendarSpeechRecoResultType,
							Date = new ExDateTime(this.timeZone, num3, num2, num).ToString("s"),
							Duration = 0,
							AllDayEvent = false
						}
					};
				}
				jsonResponse = DayTimeDurationRecoResult.JsonSerialize(obj);
			}
		}

		private readonly ExTimeZone timeZone;
	}
}
