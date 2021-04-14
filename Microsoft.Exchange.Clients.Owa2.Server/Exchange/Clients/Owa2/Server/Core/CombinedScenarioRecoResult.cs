using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CombinedScenarioRecoResult
	{
		[DataMember]
		public string RequestId { get; set; }

		[DataMember]
		public string Text { get; set; }

		[DataMember]
		public string JsonResponse { get; set; }

		[IgnoreDataMember]
		public CombinedScenarioResultType ResultType { get; set; }

		[DataMember(Name = "ResultType")]
		public string ResultTypeString
		{
			get
			{
				return this.ResultType.ToString();
			}
			set
			{
				this.ResultType = (CombinedScenarioResultType)Enum.Parse(typeof(CombinedScenarioResultType), value);
			}
		}

		public static CombinedScenarioRecoResult JsonDeserialize(string result, Type targetType)
		{
			return (CombinedScenarioRecoResult)SpeechRecognitionResultHandler.JsonDeserialize(result, targetType);
		}

		public static string JsonSerialize(object obj)
		{
			return SpeechRecognitionResultHandler.JsonSerialize(obj);
		}

		public string ToJsonString()
		{
			return CombinedScenarioRecoResult.JsonSerialize(this);
		}

		internal static CombinedScenarioResultType MapSpeechRecoResultTypeToCombinedRecoResultType(MobileSpeechRecoResultType resultType)
		{
			switch (resultType)
			{
			case MobileSpeechRecoResultType.DaySearch:
				return CombinedScenarioResultType.DaySearch;
			case MobileSpeechRecoResultType.AppointmentCreation:
				return CombinedScenarioResultType.AppointmentCreation;
			case MobileSpeechRecoResultType.FindPeople:
				return CombinedScenarioResultType.FindPeople;
			case MobileSpeechRecoResultType.EmailPeople:
				return CombinedScenarioResultType.EmailPeople;
			case MobileSpeechRecoResultType.None:
				return CombinedScenarioResultType.None;
			default:
				throw new ArgumentOutOfRangeException("ResultType", resultType, "ResultType does not have a mapping");
			}
		}
	}
}
