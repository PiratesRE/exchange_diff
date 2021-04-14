using System;
using System.Net;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsFeedbackResponse
	{
		private ApnsFeedbackResponse(string token, ExDateTime timestamp)
		{
			this.Token = token;
			this.TimeStamp = timestamp;
		}

		public ExDateTime TimeStamp { get; private set; }

		public string Token { get; private set; }

		public static ApnsFeedbackResponse FromApnsFormat(byte[] binaryForm)
		{
			ArgumentValidator.ThrowIfNull("binaryForm", binaryForm);
			ArgumentValidator.ThrowIfInvalidValue<byte[]>("binaryForm", binaryForm, (byte[] x) => x.Length == 38);
			ExDateTime timestamp = ApnsFeedbackResponse.ExtractTimestamp(binaryForm);
			IPAddress.NetworkToHostOrder(BitConverter.ToInt16(binaryForm, 4));
			string token = ApnsFeedbackResponse.ExtractToken(binaryForm);
			return new ApnsFeedbackResponse(token, timestamp);
		}

		public static ApnsFeedbackResponse FromFeedbackFileEntry(string feedbackFileLine)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("feedbackFileLine", feedbackFileLine);
			ExDateTime timestamp = default(ExDateTime);
			Exception ex = null;
			string[] array = feedbackFileLine.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 2 && array[0].Length == 64)
			{
				try
				{
					HexConverter.HexStringToByteArray(array[0]);
					timestamp = ExDateTime.Parse(ExTimeZone.UtcTimeZone, array[1]);
					return new ApnsFeedbackResponse(array[0], timestamp);
				}
				catch (FormatException ex2)
				{
					ex = ex2;
				}
			}
			throw new ApnsFeedbackException(Strings.ApnsFeedbackResponseInvalidFileLine(feedbackFileLine, (ex == null) ? string.Empty : ex.Message), ex);
		}

		public override string ToString()
		{
			if (this.toStringCache == null)
			{
				this.toStringCache = string.Format("{{token:{0}; timestamp:{1}}}", this.Token, this.TimeStamp);
			}
			return this.toStringCache;
		}

		private static ExDateTime ExtractTimestamp(byte[] binaryForm)
		{
			int num = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(binaryForm, 0));
			return Constants.EpochBaseTime.AddSeconds((double)num);
		}

		private static string ExtractToken(byte[] binaryForm)
		{
			return HexConverter.ByteArrayToHexString(binaryForm, 6, 32);
		}

		public const int Length = 38;

		private const int TimestampOffset = 0;

		private const int TokenLengthOffset = 4;

		private const int TokenOffset = 6;

		private const int StringTokenSize = 64;

		private string toStringCache;
	}
}
