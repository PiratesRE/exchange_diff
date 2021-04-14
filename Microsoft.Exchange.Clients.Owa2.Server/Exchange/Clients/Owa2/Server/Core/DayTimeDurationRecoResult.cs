using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class DayTimeDurationRecoResult
	{
		[DataMember]
		public bool AllDayEvent { get; set; }

		[DataMember]
		public string Date { get; set; }

		[DataMember]
		public int Duration { get; set; }

		[IgnoreDataMember]
		public CalendarSpeechRecoResultType ResultType { get; set; }

		[DataMember(Name = "ResultType")]
		public string ResultTypeString
		{
			get
			{
				return this.ResultType.ToString();
			}
			set
			{
				this.ResultType = (CalendarSpeechRecoResultType)Enum.Parse(typeof(CalendarSpeechRecoResultType), value);
			}
		}

		public static DayTimeDurationRecoResult JsonDeserialize(string result, Type targetType)
		{
			return (DayTimeDurationRecoResult)SpeechRecognitionResultHandler.JsonDeserialize(result, targetType);
		}

		public static string JsonSerialize(object obj)
		{
			return SpeechRecognitionResultHandler.JsonSerialize(obj);
		}

		public string ToJsonString()
		{
			return DayTimeDurationRecoResult.JsonSerialize(this);
		}
	}
}
