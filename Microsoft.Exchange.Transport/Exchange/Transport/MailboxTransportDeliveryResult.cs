using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal class MailboxTransportDeliveryResult
	{
		private MailboxTransportDeliveryResult(SmtpResponse messageLevelSmtpResponse, TimeSpan? messageLevelRetryInterval, bool messageLevelResubmit, bool retryQueue, IList<MailboxTransportDeliveryResult.RecipientResponse> recipientResponses)
		{
			this.messageLevelSmtpResponse = messageLevelSmtpResponse;
			this.messageLevelRetryInterval = messageLevelRetryInterval;
			this.messageLevelResubmit = messageLevelResubmit;
			this.retryQueue = retryQueue;
			this.recipientResponses = recipientResponses;
		}

		public static bool TryParse(SmtpResponse smtpResponse, out MailboxTransportDeliveryResult result, out string parseError)
		{
			result = null;
			parseError = null;
			if (smtpResponse.SmtpResponseType == SmtpResponseType.Unknown)
			{
				parseError = "smtpResponse is of Unknown response type";
			}
			else if (smtpResponse.StatusText.Length <= 1)
			{
				parseError = "smtp response should be multi-line";
			}
			else if (!smtpResponse.StatusText[0].EndsWith("STOREDRV.Deliver; delivery result banner", StringComparison.OrdinalIgnoreCase))
			{
				parseError = string.Format(CultureInfo.InvariantCulture, "first line of response is not the expected banner: <{0}>", new object[]
				{
					smtpResponse.StatusText[0]
				});
			}
			else if (smtpResponse.StatusText.Length == 2)
			{
				parseError = MailboxTransportDeliveryResult.ParseSingleLineResponse(smtpResponse, out result);
			}
			else
			{
				parseError = MailboxTransportDeliveryResult.ParseMultiLineResponse(smtpResponse, out result);
			}
			return parseError == null;
		}

		public SmtpResponse MessageLevelSmtpResponse
		{
			get
			{
				return this.messageLevelSmtpResponse;
			}
		}

		public TimeSpan? MessageLevelRetryInterval
		{
			get
			{
				return this.messageLevelRetryInterval;
			}
		}

		public bool MessageLevelResubmit
		{
			get
			{
				return this.messageLevelResubmit;
			}
		}

		public bool RetryQueue
		{
			get
			{
				return this.retryQueue;
			}
		}

		public int RecipientResponseCount
		{
			get
			{
				return this.recipientResponses.Count;
			}
		}

		public IEnumerable<MailboxTransportDeliveryResult.RecipientResponse> RecipientResponses
		{
			get
			{
				return this.recipientResponses;
			}
		}

		public MailboxTransportDeliveryResult.RecipientResponse GetRecipientResponseAt(int index)
		{
			return this.recipientResponses[index];
		}

		private static string ParseSingleLineResponse(SmtpResponse smtpResponse, out MailboxTransportDeliveryResult result)
		{
			result = null;
			SmtpResponse smtpResponse2;
			bool flag;
			bool flag2;
			bool flag3;
			bool flag4;
			TimeSpan? timeSpan;
			string text = MailboxTransportDeliveryResult.ParseLine(smtpResponse.StatusText[1], out smtpResponse2, out flag, out flag2, out flag3, out flag4, out timeSpan);
			if (text != null)
			{
				return text;
			}
			result = new MailboxTransportDeliveryResult(smtpResponse2, timeSpan, flag, flag2, new List<MailboxTransportDeliveryResult.RecipientResponse>());
			return null;
		}

		private static string ParseMultiLineResponse(SmtpResponse smtpResponse, out MailboxTransportDeliveryResult result)
		{
			result = null;
			IList<MailboxTransportDeliveryResult.RecipientResponse> list = new List<MailboxTransportDeliveryResult.RecipientResponse>();
			string text;
			for (int i = 2; i < smtpResponse.StatusText.Length; i++)
			{
				string stringToParse = smtpResponse.StatusText[i];
				SmtpResponse smtpResponse2;
				bool flag;
				bool flag2;
				bool retryOnDuplicateDelivery;
				bool flag3;
				TimeSpan? timeSpan;
				text = MailboxTransportDeliveryResult.ParseLine(stringToParse, out smtpResponse2, out flag, out flag2, out retryOnDuplicateDelivery, out flag3, out timeSpan);
				if (text != null)
				{
					return text;
				}
				AckStatus ackStatus;
				if (flag)
				{
					ackStatus = AckStatus.Resubmit;
				}
				else if (flag3)
				{
					ackStatus = AckStatus.SuccessNoDsn;
				}
				else if (smtpResponse2.SmtpResponseType == SmtpResponseType.Success)
				{
					ackStatus = AckStatus.Success;
				}
				else if (smtpResponse2.SmtpResponseType == SmtpResponseType.TransientError)
				{
					ackStatus = AckStatus.Retry;
				}
				else
				{
					if (smtpResponse2.SmtpResponseType != SmtpResponseType.PermanentError)
					{
						return string.Format(CultureInfo.InvariantCulture, "invalid recipient response type: <{0}>", new object[]
						{
							smtpResponse2.SmtpResponseType
						});
					}
					ackStatus = AckStatus.Fail;
				}
				list.Add(new MailboxTransportDeliveryResult.RecipientResponse(ackStatus, smtpResponse2, retryOnDuplicateDelivery));
			}
			SmtpResponse smtpResponse3;
			bool flag4;
			bool flag5;
			bool flag6;
			bool flag7;
			TimeSpan? timeSpan2;
			text = MailboxTransportDeliveryResult.ParseLine(smtpResponse.StatusText[1], out smtpResponse3, out flag4, out flag5, out flag6, out flag7, out timeSpan2);
			if (text != null)
			{
				return text;
			}
			result = new MailboxTransportDeliveryResult(smtpResponse3, timeSpan2, flag4, flag5, list);
			return null;
		}

		private static string ParseLine(string stringToParse, out SmtpResponse response, out bool resubmit, out bool retryQueue, out bool retryOnDuplicateDelivery, out bool skipDsn, out TimeSpan? retryInterval)
		{
			resubmit = false;
			retryQueue = false;
			retryOnDuplicateDelivery = false;
			skipDsn = false;
			retryInterval = null;
			response = SmtpResponse.Empty;
			Match match = MailboxTransportDeliveryResult.responseSuffixRegex.Match(stringToParse);
			if (!match.Success)
			{
				return string.Format(CultureInfo.InvariantCulture, "string did not match smtp response suffix regex <{0}>", new object[]
				{
					stringToParse
				});
			}
			string text = stringToParse.Substring(0, match.Index);
			if (!SmtpResponse.TryParse(text, out response))
			{
				return string.Format(CultureInfo.InvariantCulture, "parsing smtp response line failed: <{0}>", new object[]
				{
					text
				});
			}
			string value = match.Groups["KeyCapture"].Value;
			string value2 = match.Groups["ValueCapture"].Value;
			if (string.Equals(value, "retryinterval", StringComparison.OrdinalIgnoreCase))
			{
				TimeSpan value3;
				if (!TimeSpan.TryParse(value2, out value3))
				{
					return string.Format(CultureInfo.InvariantCulture, "failed to parse retry interval value <{0}>", new object[]
					{
						value2
					});
				}
				retryInterval = new TimeSpan?(value3);
			}
			else if (string.Equals(value, "resubmit", StringComparison.OrdinalIgnoreCase))
			{
				if (!bool.TryParse(value2, out resubmit))
				{
					return string.Format(CultureInfo.InvariantCulture, "failed to parse resubmit value <{0}>", new object[]
					{
						value2
					});
				}
			}
			else if (string.Equals(value, "retryonduplicatedelivery", StringComparison.OrdinalIgnoreCase))
			{
				if (!bool.TryParse(value2, out retryOnDuplicateDelivery))
				{
					return string.Format(CultureInfo.InvariantCulture, "failed to parse retryOnDuplicateDelivery value <{0}>", new object[]
					{
						value2
					});
				}
			}
			else if (string.Equals(value, "retryqueue", StringComparison.OrdinalIgnoreCase))
			{
				if (!bool.TryParse(value2, out retryQueue))
				{
					return string.Format(CultureInfo.InvariantCulture, "failed to parse retryQueue value <{0}>", new object[]
					{
						value2
					});
				}
			}
			else if (string.Equals(value, "skipdsn", StringComparison.OrdinalIgnoreCase) && !bool.TryParse(value2, out skipDsn))
			{
				return string.Format(CultureInfo.InvariantCulture, "failed to parse skipDsn value <{0}>", new object[]
				{
					value2
				});
			}
			return null;
		}

		public const int MaxNumRecipientsToSend = 47;

		public const string RetryQueueKeyName = "retryqueue";

		public const string RetryIntervalKeyName = "retryinterval";

		public const string ResubmitKeyName = "resubmit";

		public const string RetryOnDuplicateDeliveryKeyName = "retryonduplicatedelivery";

		public const string SkipDsnKeyName = "skipdsn";

		public const string ResponseBanner = "STOREDRV.Deliver; delivery result banner";

		private const string KeyCaptureName = "KeyCapture";

		private const string ValueCaptureName = "ValueCapture";

		private static readonly string responseSuffixRegexString = string.Format("\\[(?<{0}>{1})\\=(?<{2}>{3})\\]$", new object[]
		{
			"KeyCapture",
			"[a-zA-Z]{1,30}",
			"ValueCapture",
			"[\\w\\.\\:\\-]{1,50}"
		});

		private static readonly Regex responseSuffixRegex = new Regex(MailboxTransportDeliveryResult.responseSuffixRegexString, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		private readonly SmtpResponse messageLevelSmtpResponse;

		private readonly bool retryQueue;

		private readonly TimeSpan? messageLevelRetryInterval;

		private readonly bool messageLevelResubmit;

		private readonly IList<MailboxTransportDeliveryResult.RecipientResponse> recipientResponses;

		internal struct RecipientResponse
		{
			public RecipientResponse(AckStatus ackStatus, SmtpResponse smtpResponse, bool retryOnDuplicateDelivery)
			{
				this.ackStatus = ackStatus;
				this.smtpResponse = smtpResponse;
				this.retryOnDuplicateDelivery = retryOnDuplicateDelivery;
			}

			public AckStatus AckStatus
			{
				get
				{
					return this.ackStatus;
				}
			}

			public SmtpResponse SmtpResponse
			{
				get
				{
					return this.smtpResponse;
				}
			}

			public bool RetryOnDuplicateDelivery
			{
				get
				{
					return this.retryOnDuplicateDelivery;
				}
			}

			private readonly AckStatus ackStatus;

			private readonly SmtpResponse smtpResponse;

			private readonly bool retryOnDuplicateDelivery;
		}
	}
}
