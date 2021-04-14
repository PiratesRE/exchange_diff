using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class DateTimeandDurationRecognitionResultHandler : IMobileSpeechRecognitionResultHandler
	{
		public DateTimeandDurationRecognitionResultHandler(ExTimeZone timeZone)
		{
			this.timeZone = timeZone;
		}

		public void ProcessAndFormatSpeechRecognitionResults(string result, out string jsonResponse, out SpeechRecognitionProcessor.SpeechHttpStatus httpStatus)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering DateTimeandDurationRecognitionResultHandler.ProcessAndFormatSpeechRecognitionResults");
			jsonResponse = null;
			httpStatus = SpeechRecognitionProcessor.SpeechHttpStatus.Success;
			using (XmlReader xmlReader = XmlReader.Create(new StringReader(result)))
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				CalendarSpeechRecoResultType calendarSpeechRecoResultType = CalendarSpeechRecoResultType.None;
				int num5 = 0;
				int num6 = 0;
				while (xmlReader.Read())
				{
					if (xmlReader.IsStartElement("Day"))
					{
						string text = xmlReader.ReadString();
						if (!int.TryParse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num) || num < 1 || num > 31)
						{
							throw new ArgumentException("Invalid day value " + text);
						}
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Valid Day value read: {0}", num);
					}
					if (xmlReader.IsStartElement("Month"))
					{
						string text2 = xmlReader.ReadString();
						if (!int.TryParse(text2, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num2) || num2 < 1 || num2 > 12)
						{
							throw new ArgumentException("Invalid month value " + text2);
						}
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Valid Month value read: {0}", num2);
					}
					if (xmlReader.IsStartElement("Year"))
					{
						string text3 = xmlReader.ReadString();
						if (!int.TryParse(text3, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num3) || num3 < 2000 || num3 > 2099)
						{
							throw new ArgumentException("Invalid year value " + text3);
						}
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Valid Year value read: {0}", num3);
					}
					if (xmlReader.IsStartElement("DurationInMinutes"))
					{
						string text4 = xmlReader.ReadString();
						if (!int.TryParse(text4, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num4) || num4 < 0)
						{
							throw new ArgumentException("Invalid duration value " + text4);
						}
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Valid Duration value read: {0}", num4);
					}
					if (xmlReader.IsStartElement("StartHour"))
					{
						string text5 = xmlReader.ReadString();
						if (!int.TryParse(text5, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num5) || num5 < 0 || num5 > 24)
						{
							throw new ArgumentException("Invalid hour value " + text5);
						}
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Valid Start Hour value read: {0}", num5);
					}
					if (xmlReader.IsStartElement("StartMinute"))
					{
						string text6 = xmlReader.ReadString();
						if (!int.TryParse(text6, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num6) || num6 < 0 || num6 > 60)
						{
							throw new ArgumentException("Invalid hour value " + text6);
						}
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Valid Start Minute value read: {0}", num6);
					}
					if (xmlReader.IsStartElement("RecoEvent"))
					{
						string text7 = xmlReader.ReadString();
						if (string.Equals(text7, "recoCompleteDateWithStartTime", StringComparison.OrdinalIgnoreCase))
						{
							calendarSpeechRecoResultType = CalendarSpeechRecoResultType.CompleteDateWithStartTime;
						}
						else if (string.Equals(text7, "recoCompleteDate", StringComparison.OrdinalIgnoreCase))
						{
							calendarSpeechRecoResultType = CalendarSpeechRecoResultType.CompleteDate;
						}
						else
						{
							if (!string.Equals(text7, "recoCompleteDateWithStartTimeAndDuration", StringComparison.OrdinalIgnoreCase))
							{
								throw new ArgumentException("Invalid RecoResultType: " + text7);
							}
							calendarSpeechRecoResultType = CalendarSpeechRecoResultType.CompleteDateWithStartTimeAndDuration;
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
					if (num == 0 || num2 == 0 || num3 == 0 || num4 == 0)
					{
						throw new ArgumentException("No valid results from speech recognition");
					}
					DayTimeDurationRecoResult dayTimeDurationRecoResult = new DayTimeDurationRecoResult();
					dayTimeDurationRecoResult.ResultType = calendarSpeechRecoResultType;
					dayTimeDurationRecoResult.Date = new ExDateTime(this.timeZone, num3, num2, num, num5, num6, 0).ToString("s");
					dayTimeDurationRecoResult.AllDayEvent = (num5 == 0 && num6 == 0 && num4 >= 1440 && num4 % 1440 == 0);
					if (dayTimeDurationRecoResult.AllDayEvent)
					{
						dayTimeDurationRecoResult.Duration = num4 - 1;
					}
					else
					{
						dayTimeDurationRecoResult.Duration = num4;
					}
					obj = new DayTimeDurationRecoResult[]
					{
						dayTimeDurationRecoResult
					};
				}
				jsonResponse = DayTimeDurationRecoResult.JsonSerialize(obj);
			}
		}

		private readonly ExTimeZone timeZone;
	}
}
