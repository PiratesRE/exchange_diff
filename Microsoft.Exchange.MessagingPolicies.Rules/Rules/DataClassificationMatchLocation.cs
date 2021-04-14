using System;
using System.Collections.Generic;
using Microsoft.Filtering;
using Microsoft.Filtering.Results;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class DataClassificationMatchLocation
	{
		public int Offset { get; set; }

		public int Length { get; set; }

		public DataClassificationSourceInfo MatchingSourceInfo { get; set; }

		public List<DataClassificationMatchLocation> SecondaryLocations { get; set; }

		private DataClassificationMatchLocation()
		{
		}

		public DataClassificationMatchLocation(int offset, int length, DataClassificationSourceInfo matchingSourceInfo)
		{
			if (matchingSourceInfo == null)
			{
				throw new ArgumentNullException("matchingSourceInfo");
			}
			this.Offset = offset;
			this.Length = length;
			this.MatchingSourceInfo = matchingSourceInfo;
			this.SecondaryLocations = new List<DataClassificationMatchLocation>();
		}

		internal Tuple<string, string> GetMatchData(MailMessage message, int matchSurroundLength)
		{
			StreamContent subjectPrependedStreamContent = RuleAgentResultUtils.GetSubjectPrependedStreamContent(message.GetUnifiedContentResults().Streams[this.MatchingSourceInfo.SourceId]);
			long num = (long)(this.Offset - matchSurroundLength);
			int startIndex = matchSurroundLength;
			long num2 = (long)matchSurroundLength;
			if (this.Offset - matchSurroundLength < 0)
			{
				num = 0L;
				num2 = (long)this.Offset;
				startIndex = this.Offset;
			}
			num2 += (long)(this.Length + matchSurroundLength);
			string text = subjectPrependedStreamContent.ReadTextChunk(num, (int)num2);
			return new Tuple<string, string>(text.Substring(startIndex, this.Length), text);
		}

		private const string ToStringFormat = "Name:{0}/Id:{1}.";
	}
}
