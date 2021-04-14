using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AntispamCommon;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.HeaderConversion
{
	internal class SclHeaders
	{
		public SclHeaders(int sclDeleteThreshold, int ehfSpamScoreJunkThreshold, int ehfSpamScoreDeleteTreshold, int partnerSpamScoreJunkThreshold, int partnerSpamScoreDeleteTreshold, IEnumerable<IPRange> hotmailWomsIPRanges)
		{
			this.Initialize(sclDeleteThreshold, ehfSpamScoreJunkThreshold, ehfSpamScoreDeleteTreshold, partnerSpamScoreJunkThreshold, partnerSpamScoreDeleteTreshold, hotmailWomsIPRanges);
		}

		public int SclDeleteThreshold
		{
			get
			{
				return this.sclDeleteThreshold;
			}
			set
			{
				this.sclDeleteThreshold = value;
			}
		}

		public void UpdateScl(HeaderList headers)
		{
			int? num;
			SclHeaders.SpamHeaderConverter spamHeaderConverter;
			this.UpdateScl(headers, null, null, null, out num, out spamHeaderConverter);
		}

		public void UpdateScl(HeaderList headers, string agentName, string eventTopic, EndOfHeadersEventArgs eventArgs, out int? sclOut, out SclHeaders.SpamHeaderConverter spamHeaderConverter)
		{
			sclOut = null;
			spamHeaderConverter = null;
			if (headers == null)
			{
				throw new ArgumentNullException("headers");
			}
			int arg;
			if (CommonUtils.TryGetValidScl(headers, out arg))
			{
				SclHeaders.Tracer.TraceDebug<int>((long)this.GetHashCode(), "X-MS-Exchange-Organization-SCL header already exists with value '{0}'", arg);
				return;
			}
			foreach (SclHeaders.SpamHeaderConverter spamHeaderConverter2 in this.headerConverters)
			{
				Header header = spamHeaderConverter2.FindSpamHeader(headers);
				if (header != null)
				{
					int? spamScore = spamHeaderConverter2.ExtractSpamScore(header);
					int num = spamHeaderConverter2.ConvertToScl(spamScore, this.sclDeleteThreshold);
					if (!string.IsNullOrEmpty(agentName) && !string.IsNullOrEmpty(eventTopic) && eventArgs != null)
					{
						this.LogStampScl(agentName, eventTopic, eventArgs, num, header);
					}
					SclHeaders.Tracer.TraceDebug<int>((long)this.GetHashCode(), "Writing X-MS-Exchange-Organization-SCL header with a value of '{0}'.", num);
					Header header2 = Header.Create("X-MS-Exchange-Organization-SCL");
					header2.Value = num.ToString(CultureInfo.InvariantCulture);
					headers.AppendChild(header2);
					sclOut = new int?(num);
					spamHeaderConverter = spamHeaderConverter2;
					break;
				}
			}
		}

		private static bool IsHeader(Header header, string name)
		{
			return string.Equals(header.Name, name, StringComparison.OrdinalIgnoreCase);
		}

		private static IEnumerable<Header> GetCandidateHeaders(HeaderList headers, bool immediatePerimeter)
		{
			bool foundReceivedHeaders = false;
			bool hasMoreHeaders = false;
			using (MimeNode.Enumerator<Header> enumerator = headers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.HeaderId != HeaderId.Received)
					{
						hasMoreHeaders = true;
						break;
					}
					foundReceivedHeaders = true;
				}
				if (foundReceivedHeaders && hasMoreHeaders)
				{
					do
					{
						yield return enumerator.Current;
					}
					while (enumerator.MoveNext() && (!immediatePerimeter || enumerator.Current.HeaderId != HeaderId.Received));
				}
			}
			yield break;
		}

		private void Initialize(int sclDeleteThreshold, int ehfSpamScoreJunkThreshold, int ehfSpamScoreDeleteThreshold, int partnerSpamScoreJunkThreshold, int partnerSpamScoreDeleteThreshold, IEnumerable<IPRange> hotmailWomsIPRanges)
		{
			this.sclDeleteThreshold = sclDeleteThreshold;
			this.headerConverters = new List<SclHeaders.SpamHeaderConverter>();
			this.headerConverters.Add(new SclHeaders.HotmailSpamHeaderConverter(hotmailWomsIPRanges));
			this.headerConverters.Add(new SclHeaders.EhfSpamHeaderConverter(ehfSpamScoreJunkThreshold, ehfSpamScoreDeleteThreshold));
			this.headerConverters.Add(new SclHeaders.PartnerSpamHeaderConverter(partnerSpamScoreJunkThreshold, partnerSpamScoreDeleteThreshold));
		}

		private void LogStampScl(string agentName, string eventTopic, EndOfHeadersEventArgs eventArgs, int scl, Header sourceHeader)
		{
			LogEntry logEntry = new LogEntry("SCL", scl.ToString(), sourceHeader.Name + ":" + sourceHeader.Value);
			AgentLog.Instance.LogStampScl(agentName, eventTopic, eventArgs, eventArgs.SmtpSession, eventArgs.MailItem, logEntry);
		}

		private const int MaxHeaderLength = 256;

		private static readonly Trace Tracer = ExTraceGlobals.UtilitiesTracer;

		private int sclDeleteThreshold;

		private List<SclHeaders.SpamHeaderConverter> headerConverters;

		public abstract class SpamHeaderConverter
		{
			public abstract Header FindSpamHeader(HeaderList headers);

			public abstract int? ExtractSpamScore(Header header);

			public abstract int ConvertToScl(int? spamScore, int sclDeleteThreshold);
		}

		public class HotmailSpamHeaderConverter : SclHeaders.SpamHeaderConverter
		{
			public HotmailSpamHeaderConverter(IEnumerable<IPRange> womsIPRanges)
			{
				this.womsIPRanges = new List<IPRange>(womsIPRanges);
			}

			public override Header FindSpamHeader(HeaderList headers)
			{
				if (this.IsNdr(headers))
				{
					AsciiTextHeader asciiTextHeader = headers.FindFirst("X-MS-Exchange-Organization-OriginalClientIPAddress") as AsciiTextHeader;
					if (asciiTextHeader != null && this.IsLegitimateNdr(asciiTextHeader))
					{
						return asciiTextHeader;
					}
				}
				return this.FindDeliveryHeader(headers);
			}

			public override int? ExtractSpamScore(Header header)
			{
				int? result = new int?(-1);
				if (header != null && !string.IsNullOrEmpty(header.Value) && header.Value.Length <= 256)
				{
					if (string.Equals(header.Name, "X-MS-Exchange-Organization-OriginalClientIPAddress", StringComparison.OrdinalIgnoreCase))
					{
						result = new int?(0);
					}
					else
					{
						result = this.ExtractDValue(header);
					}
				}
				return result;
			}

			public override int ConvertToScl(int? spamScore, int sclDeleteThreshold)
			{
				int valueOrDefault = spamScore.GetValueOrDefault();
				if (spamScore != null)
				{
					switch (valueOrDefault)
					{
					case 0:
						return 0;
					case 1:
						break;
					case 2:
						return sclDeleteThreshold - 1;
					case 3:
						return sclDeleteThreshold;
					default:
						return sclDeleteThreshold - 1;
					}
				}
				return 1;
			}

			private static bool TryDecodeHeader(Header header, out string decodedHeader)
			{
				decodedHeader = null;
				try
				{
					byte[] bytes = Convert.FromBase64String(header.Value);
					decodedHeader = Encoding.ASCII.GetString(bytes);
				}
				catch (FormatException)
				{
				}
				catch (ArgumentException)
				{
				}
				return !string.IsNullOrEmpty(decodedHeader);
			}

			private static bool IsValidPairWithName(string pair, char name)
			{
				return pair.Length >= 2 && pair[0] == name && pair[1] == '=';
			}

			private Header FindDeliveryHeader(HeaderList headers)
			{
				Header header = null;
				bool flag = false;
				foreach (Header header2 in SclHeaders.GetCandidateHeaders(headers, true))
				{
					if (SclHeaders.IsHeader(header2, "X-Message-Delivery"))
					{
						header = header2;
					}
					else if (SclHeaders.IsHeader(header2, "X-Message-Info"))
					{
						flag = true;
						break;
					}
				}
				if (flag && header != null)
				{
					SclHeaders.Tracer.TraceDebug<string>((long)this.GetHashCode(), "X-Message-Delivery header found with value: {0}", header.Value);
					return header;
				}
				return null;
			}

			private int? ExtractDValue(Header header)
			{
				int? result = new int?(-1);
				string text;
				if (SclHeaders.HotmailSpamHeaderConverter.TryDecodeHeader(header, out text))
				{
					string[] array = text.Split(new char[]
					{
						';'
					});
					if (array != null && array.Length > 0 && SclHeaders.HotmailSpamHeaderConverter.IsValidPairWithName(array[0], 'V'))
					{
						result = null;
						bool flag = false;
						foreach (string text2 in array)
						{
							if (SclHeaders.HotmailSpamHeaderConverter.IsValidPairWithName(text2, 'D'))
							{
								result = new int?(-1);
								if (!flag)
								{
									flag = true;
									int num;
									if (text2.Length > 2 && int.TryParse(text2.Substring(2), out num) && num >= 0 && num <= 3)
									{
										result = new int?(num);
									}
								}
							}
						}
					}
				}
				return result;
			}

			private bool IsNdr(HeaderList headers)
			{
				ContentTypeHeader contentTypeHeader = headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
				if (contentTypeHeader != null && contentTypeHeader.MediaType != null && contentTypeHeader.SubType != null && contentTypeHeader.MediaType.StartsWith("multipart", StringComparison.OrdinalIgnoreCase) && contentTypeHeader.SubType.StartsWith("report", StringComparison.OrdinalIgnoreCase))
				{
					SclHeaders.Tracer.TraceDebug<string>((long)this.GetHashCode(), "This message has a content type of {0} and is being treated as a report.", contentTypeHeader.SubType);
					return true;
				}
				return false;
			}

			private bool IsLegitimateNdr(AsciiTextHeader clientIPAddressHeader)
			{
				IPAddress ipaddress;
				if (clientIPAddressHeader.Value != null && IPAddress.TryParse(clientIPAddressHeader.Value, out ipaddress))
				{
					foreach (IPRange iprange in this.womsIPRanges)
					{
						if (iprange.Contains(ipaddress))
						{
							SclHeaders.Tracer.TraceDebug<IPAddress, IPRange>((long)this.GetHashCode(), "Client IP Address {0} matches WOMS IP range {1}.", ipaddress, iprange);
							return true;
						}
					}
					return false;
				}
				return false;
			}

			private const string MessageDeliveryHeaderName = "X-Message-Delivery";

			private const string MessageInfoHeaderName = "X-Message-Info";

			private const int InvalidD = -1;

			private const int MinD = 0;

			private const int MaxD = 3;

			private const int NdrDValue = 0;

			private List<IPRange> womsIPRanges;
		}

		public class EhfSpamHeaderConverter : SclHeaders.SpamHeaderConverter
		{
			public EhfSpamHeaderConverter(int ehfSpamScoreJunkThreshold, int ehfSpamScoreDeleteThreshold)
			{
				this.ehfSpamScoreJunkThreshold = ehfSpamScoreJunkThreshold;
				this.ehfSpamScoreDeleteThreshold = ehfSpamScoreDeleteThreshold;
			}

			public override Header FindSpamHeader(HeaderList headers)
			{
				Header result = null;
				foreach (Header header in SclHeaders.GetCandidateHeaders(headers, true))
				{
					if (SclHeaders.IsHeader(header, "X-SpamScore"))
					{
						SclHeaders.Tracer.TraceDebug((long)this.GetHashCode(), "X-SpamScore header found");
						result = header;
					}
					else if (SclHeaders.IsHeader(header, "X-SafeListed-IP"))
					{
						SclHeaders.Tracer.TraceDebug((long)this.GetHashCode(), "X-SafeListed-IP header found");
						result = header;
						break;
					}
				}
				return result;
			}

			public override int? ExtractSpamScore(Header header)
			{
				int? result = new int?(int.MinValue);
				if (header != null)
				{
					string text;
					if (header.TryGetValue(out text))
					{
						if (!string.IsNullOrEmpty(text) && text.Length <= 256)
						{
							int value;
							if (string.Equals(header.Name, "X-SafeListed-IP", StringComparison.OrdinalIgnoreCase))
							{
								result = null;
							}
							else if (int.TryParse(text, out value))
							{
								result = new int?(value);
							}
							else
							{
								SclHeaders.Tracer.TraceError<string, string>((long)this.GetHashCode(), "EHF spam header {0} has non-integer value: {1}", header.Name, text);
							}
						}
					}
					else
					{
						SclHeaders.Tracer.TraceError<string>((long)this.GetHashCode(), "EHF spam header value could not be obtained: {0}", header.Name);
					}
				}
				return result;
			}

			public override int ConvertToScl(int? spamScore, int sclDeleteThreshold)
			{
				if (spamScore == -2147483648)
				{
					return sclDeleteThreshold - 1;
				}
				if (spamScore == null)
				{
					return 0;
				}
				if (spamScore < this.ehfSpamScoreJunkThreshold)
				{
					return 1;
				}
				if (spamScore < this.ehfSpamScoreDeleteThreshold)
				{
					return sclDeleteThreshold - 1;
				}
				return sclDeleteThreshold;
			}

			private const string SpamScoreHeaderName = "X-SpamScore";

			private const string SafeListedHeaderName = "X-SafeListed-IP";

			private const int InvalidSpamScore = -2147483648;

			private readonly int ehfSpamScoreJunkThreshold;

			private readonly int ehfSpamScoreDeleteThreshold;
		}

		public class PartnerSpamHeaderConverter : SclHeaders.SpamHeaderConverter
		{
			public PartnerSpamHeaderConverter(int partnerSpamScoreJunkThreshold, int partnerSpamScoreDeleteThreshold)
			{
				this.partnerSpamScoreJunkThreshold = partnerSpamScoreJunkThreshold;
				this.partnerSpamScoreDeleteThreshold = partnerSpamScoreDeleteThreshold;
			}

			public override Header FindSpamHeader(HeaderList headers)
			{
				Header result = null;
				foreach (Header header in SclHeaders.GetCandidateHeaders(headers, false))
				{
					if (SclHeaders.IsHeader(header, "X-PartnerSpamRecommendation"))
					{
						SclHeaders.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Partner spam recommendation header found: {0}", header.Name);
						result = header;
						break;
					}
				}
				return result;
			}

			public override int? ExtractSpamScore(Header header)
			{
				int? result = new int?(-1);
				if (header != null)
				{
					string text;
					if (header.TryGetValue(out text))
					{
						if (!string.IsNullOrEmpty(text) && text.Length <= 256)
						{
							int num;
							if (int.TryParse(text, out num) && num >= 0)
							{
								result = new int?(num);
							}
							else
							{
								SclHeaders.Tracer.TraceError<string, string>((long)this.GetHashCode(), "Partner spam header {0} has non-integer value: {1}", header.Name, text);
							}
						}
					}
					else
					{
						SclHeaders.Tracer.TraceError<string>((long)this.GetHashCode(), "Partner spam header value could not be obtained: {0}", header.Name);
					}
				}
				return result;
			}

			public override int ConvertToScl(int? spamScore, int sclDeleteThreshold)
			{
				if (spamScore == null)
				{
					throw new ArgumentNullException("spamScore");
				}
				if (spamScore < -1)
				{
					throw new ArgumentException("Spam score should not be less than InvalidSpamScore", "spamScore");
				}
				if (spamScore == -1)
				{
					return sclDeleteThreshold - 1;
				}
				if (spamScore < this.partnerSpamScoreJunkThreshold)
				{
					return 1;
				}
				if (spamScore < this.partnerSpamScoreDeleteThreshold)
				{
					return sclDeleteThreshold - 1;
				}
				return sclDeleteThreshold;
			}

			private const string SpamScoreHeaderName = "X-PartnerSpamRecommendation";

			private const int InvalidSpamScore = -1;

			private readonly int partnerSpamScoreJunkThreshold;

			private readonly int partnerSpamScoreDeleteThreshold;
		}
	}
}
