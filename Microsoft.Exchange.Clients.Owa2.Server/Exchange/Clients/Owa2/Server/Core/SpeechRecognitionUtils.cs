using System;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class SpeechRecognitionUtils
	{
		internal static SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs GetCombinedPeopleSearchResult(SpeechRecognition galRecoHelper, SpeechRecognition personalContactsRecoHelper, MobileSpeechRecoResultType highestRecoResultType)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string>(0L, "Gal Result response text:'{0}'", galRecoHelper.Results.ResponseText);
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string>(0L, "Personal Contacts Result response text:'{0}'", personalContactsRecoHelper.Results.ResponseText);
			string galResults = string.Empty;
			string personalContactsResults = string.Empty;
			if (galRecoHelper.Results.HttpStatus == SpeechRecognitionProcessor.SpeechHttpStatus.Success && galRecoHelper.ResultType == highestRecoResultType)
			{
				galResults = galRecoHelper.Results.ResponseText;
			}
			if (personalContactsRecoHelper.Results.HttpStatus == SpeechRecognitionProcessor.SpeechHttpStatus.Success && personalContactsRecoHelper.ResultType == highestRecoResultType)
			{
				personalContactsResults = personalContactsRecoHelper.Results.ResponseText;
			}
			string text = SpeechRecognitionUtils.CombineGALandPersonalContactXMLResults(galResults, personalContactsResults, highestRecoResultType);
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string>(0L, "Response Text to send to ResultHandler:'{0}'", text);
			SpeechRecognitionProcessor.SpeechHttpStatus httpStatus = SpeechRecognitionProcessor.SpeechHttpStatus.Success;
			if (galRecoHelper.Results.HttpStatus != SpeechRecognitionProcessor.SpeechHttpStatus.Success && personalContactsRecoHelper.Results.HttpStatus != SpeechRecognitionProcessor.SpeechHttpStatus.Success)
			{
				httpStatus = personalContactsRecoHelper.Results.HttpStatus;
				text = string.Empty;
			}
			return new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(text, httpStatus);
		}

		internal static MobileSpeechRecoResultType ParseMobileScenarioXML(string result)
		{
			string parsedResultType = string.Empty;
			using (XmlReader xmlReader = XmlReader.Create(new StringReader(result)))
			{
				if (xmlReader.ReadToFollowing("MobileReco"))
				{
					xmlReader.MoveToAttribute("ResultType");
					parsedResultType = xmlReader.ReadContentAsString();
				}
			}
			return SpeechRecognitionUtils.GetMobileSpeechResultType(parsedResultType);
		}

		private static string CombineGALandPersonalContactXMLResults(string galResults, string personalContactsResults, MobileSpeechRecoResultType resultType)
		{
			string text = SpeechRecognitionUtils.ExtractResultsFromXML(galResults, resultType);
			string text2 = SpeechRecognitionUtils.ExtractResultsFromXML(personalContactsResults, resultType);
			return string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><MobileReco ResultType=\"{4}\"><{0}>{1}</{0}><{2}>{3}</{2}></MobileReco>", new object[]
			{
				"GALSearch",
				text,
				"PersonalContactSearch",
				text2,
				resultType.ToString()
			});
		}

		private static string ExtractResultsFromXML(string results, MobileSpeechRecoResultType resultType)
		{
			string result = string.Format("<MobileReco {0}=\"{1}\"/>", "ResultType", resultType.ToString());
			if (!string.IsNullOrEmpty(results))
			{
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(results)))
				{
					if (xmlReader.IsStartElement("MobileReco"))
					{
						result = xmlReader.ReadOuterXml();
					}
				}
			}
			return result;
		}

		private static MobileSpeechRecoResultType GetMobileSpeechResultType(string parsedResultType)
		{
			MobileSpeechRecoResultType result;
			if (!EnumValidator.TryParse<MobileSpeechRecoResultType>(parsedResultType, EnumParseOptions.IgnoreCase, out result))
			{
				result = MobileSpeechRecoResultType.None;
			}
			return result;
		}
	}
}
