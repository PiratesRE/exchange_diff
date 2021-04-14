using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Filtering.Streams;

namespace Microsoft.Filtering.Results
{
	public class RuleAgentResultUtils
	{
		public static bool HasExceededProcessingLimit(StreamIdentity identity)
		{
			return identity.Notifications.Any((Notification n) => RuleAgentResultUtils.IsExceededProcessingLimitCode(n.Code));
		}

		public static bool IsUnsupported(StreamIdentity identity)
		{
			bool flag = identity.Content == null || !identity.Content.IsTextAvailable;
			bool flag2 = identity.Properties.ContainsKey("StreamIdentity.Leaf");
			if (!flag2)
			{
				return false;
			}
			if (!flag)
			{
				return identity.Notifications.Any((Notification n) => RuleAgentResultUtils.IsUnsupportedCode(n.Code));
			}
			return true;
		}

		public static bool IsEncrypted(StreamIdentity identity)
		{
			return identity.Notifications.Any((Notification n) => RuleAgentResultUtils.IsEncryptedCode(n.Code));
		}

		public static bool HasPermanentError(StreamIdentity identity)
		{
			return identity.Notifications.Any((Notification n) => RuleAgentResultUtils.IsPermanentErrorCode(n.Code));
		}

		public static void ValidateResults(FilteringResults results)
		{
			if (results.Streams.Any(new Func<StreamIdentity, bool>(RuleAgentResultUtils.HasPermanentError)))
			{
				throw new ResultsValidationException("A permanent text extraction error was encountered while getting attachment content", results);
			}
			foreach (ScanResult scanResult in results.ScanResults)
			{
				if (scanResult.ErrorInfo.RawReturnCode == -2147220991)
				{
					throw new ClassificationEngineInvalidOobConfigurationException("A permanent text extraction error was encountered while scanning. Scan engine failed to load OOB classifications", results);
				}
				if (scanResult.ErrorInfo.RawReturnCode == -2147220981)
				{
					throw new ClassificationEngineInvalidCustomConfigurationException("A permanent text extraction error was encountered while scanning. Scan engine failed to load custom classifications", results);
				}
			}
			if (ResultsExtensions.HasCategoryErrorForType(results, 16))
			{
				throw new ResultsValidationException("The Classification Engine encountered an error while scanning", results);
			}
		}

		public static FilteringElapsedTimes CalculateElapsedTimes(FilteringResponse response)
		{
			FilteringResults results = response.Results;
			Func<string, long> func = (string key) => results.Streams.Sum(delegate(StreamIdentity si)
			{
				long result;
				if (!si.Properties.TryGetInt64(key, ref result))
				{
					return 0L;
				}
				return result;
			});
			FilteringElapsedTimes filteringElapsedTimes = new FilteringElapsedTimes();
			filteringElapsedTimes.Total = response.ElapsedTime;
			filteringElapsedTimes.Scanning = TimeSpan.FromMilliseconds((double)results.ScanResults.Sum((ScanResult sr) => sr.ElapsedTime));
			filteringElapsedTimes.Parsing = TimeSpan.FromMilliseconds((double)func("ScanningPipeline::ElapsedTimeKeys::Parsing"));
			filteringElapsedTimes.TextExtraction = TimeSpan.FromMilliseconds((double)func("ScanningPipeline::ElapsedTimeKeys::TextExtraction"));
			return filteringElapsedTimes;
		}

		public static StreamContent GetSubjectPrependedStreamContent(StreamIdentity identity)
		{
			return new StreamContent(RuleAgentResultUtils.GetSubjectPrependedStream(identity));
		}

		public static TextReader GetSubjectPrependedReader(StreamIdentity identity)
		{
			return new StreamReader(RuleAgentResultUtils.GetSubjectPrependedStream(identity), Encoding.Unicode, false, 1024, true);
		}

		public static IDictionary<string, string> GetCustomProperties(StreamIdentity identity)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in identity.Properties.Keys)
			{
				if (text.StartsWith(RuleAgentResultUtils.customPropertiesPrefix))
				{
					dictionary.Add(text.Substring(RuleAgentResultUtils.customPropertiesPrefix.Length), identity.Properties.GetString(text));
				}
			}
			return dictionary;
		}

		private static Stream GetSubjectPrependedStream(StreamIdentity identity)
		{
			if (identity.Properties.ContainsKey("Parsing::ParsingKeys::MessageBody"))
			{
				string subject = identity.Properties.ContainsKey("Parsing::ParsingKeys::TruncatedSubject") ? identity.Properties.GetString("Parsing::ParsingKeys::TruncatedSubject") : (identity.Properties.GetString("Parsing::ParsingKeys::Subject") + " ");
				return new SubjectPrependedStream(subject, identity.Content.TextReadStream);
			}
			return identity.Content.TextReadStream;
		}

		private static bool IsExceededProcessingLimitCode(NotificationCode code)
		{
			switch (code)
			{
			case 67141633:
			case 67141634:
			case 67141635:
			case 67141638:
			case 67141641:
				return true;
			}
			return false;
		}

		private static bool IsUnsupportedCode(NotificationCode code)
		{
			switch (code)
			{
			case -2080374783:
			case -2080374781:
				break;
			case -2080374782:
				return false;
			default:
				switch (code)
				{
				case 67141636:
				case 67141637:
				case 67141639:
				case 67141640:
					break;
				case 67141638:
					return false;
				default:
					return false;
				}
				break;
			}
			return true;
		}

		private static bool IsPermanentErrorCode(NotificationCode code)
		{
			return -2080374782 == code;
		}

		private static bool IsEncryptedCode(NotificationCode code)
		{
			return 67141636 == code;
		}

		private static string customPropertiesPrefix = "CustomProperty::";
	}
}
