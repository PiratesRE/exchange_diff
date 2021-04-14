using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	internal class PendingResponseParser : ResponseParser
	{
		public PendingResponseParser(HttpStatusCode httpStatusCode, ResponseCode responseCode, int maxResponseSize, PerfDateTime perfDateTime) : base(httpStatusCode, responseCode, maxResponseSize)
		{
			this.perfDateTime = perfDateTime;
			this.state = (base.IsSuccessful ? PendingResponseParser.ParseState.NewCommandLine : PendingResponseParser.ParseState.ResponseData);
		}

		public override void PutData(ArraySegment<byte> dataFragment)
		{
			base.CheckDisposed();
			int i = 0;
			this.responseBodySize = new long?((long)dataFragment.Count + ((this.responseBodySize != null) ? this.responseBodySize.Value : 0L));
			while (i < dataFragment.Count)
			{
				string text = null;
				switch (this.state)
				{
				case PendingResponseParser.ParseState.NewCommandLine:
					if (this.TryParseLine(dataFragment, ref i, out text))
					{
						this.AddParsedMetadataHistory(text);
						if (string.Compare(text, "DONE", true) == 0)
						{
							this.state = PendingResponseParser.ParseState.NewHeaderLine;
						}
					}
					break;
				case PendingResponseParser.ParseState.NewHeaderLine:
					if (this.TryParseLine(dataFragment, ref i, out text))
					{
						if (string.IsNullOrWhiteSpace(text))
						{
							this.state = PendingResponseParser.ParseState.ResponseData;
						}
						else
						{
							int num = text.IndexOf(':');
							if (num == -1)
							{
								throw ProtocolException.FromResponseCode((LID)46624, string.Format("Additional header not formatted correctly; [{0}]", text), ResponseCode.InvalidPayload, null);
							}
							string header = text.Substring(0, num).Trim();
							string headerValue = text.Substring(num + 1).Trim();
							base.SetHeader(header, headerValue);
						}
					}
					break;
				case PendingResponseParser.ParseState.ResponseData:
				{
					int num2 = dataFragment.Count - i;
					if (num2 > 0)
					{
						this.responseDataSize = new long?((long)num2 + ((this.responseDataSize != null) ? this.responseDataSize.Value : 0L));
						base.WriteResponseData(new ArraySegment<byte>(dataFragment.Array, dataFragment.Offset + i, num2));
						i += num2;
					}
					break;
				}
				}
			}
		}

		public override void Done()
		{
			base.CheckDisposed();
			if (this.state != PendingResponseParser.ParseState.ResponseData)
			{
				throw ProtocolException.FromResponseCode((LID)63008, "Response not properly formatted as it didn't contain a DONE followed by an empty line.", ResponseCode.InvalidPayload, null);
			}
		}

		public override void AppendParserDiagnosticInformation(StringBuilder stringBuilder)
		{
			if (this.responseBodySize != null)
			{
				stringBuilder.AppendFormat("..[BODY SIZE: {0}]\r\n", this.responseBodySize.Value);
			}
			if (base.HttpStatusCode == HttpStatusCode.OK)
			{
				this.GetMetadataAndAdditionalHeaders(stringBuilder);
				if (this.responseDataSize != null)
				{
					stringBuilder.AppendFormat("..[DATA SIZE: {0}]\r\n", this.responseDataSize.Value);
				}
			}
			string failureInfo = base.FailureInfo;
			if (!string.IsNullOrWhiteSpace(failureInfo))
			{
				stringBuilder.Append(failureInfo);
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PendingResponseParser>(this);
		}

		private bool TryParseLine(ArraySegment<byte> dataFragment, ref int index, out string newLine)
		{
			newLine = null;
			while (index < dataFragment.Count)
			{
				byte b = dataFragment.Array[dataFragment.Offset + index];
				index++;
				if (this.foundCR)
				{
					if (b != 10)
					{
						throw ProtocolException.FromResponseCode((LID)38432, "Expecting LF after CR in response", ResponseCode.InvalidPayload, null);
					}
					this.foundCR = false;
					if (this.parseLineSize > 0)
					{
						newLine = Encoding.UTF8.GetString(this.parseLine, 0, this.parseLineSize);
						this.parseLineSize = 0;
					}
					else
					{
						newLine = string.Empty;
					}
					return true;
				}
				else if (b == 13)
				{
					this.foundCR = true;
				}
				else
				{
					if (this.parseLineSize >= this.parseLine.Length)
					{
						if (this.parseLine.Length >= 4096)
						{
							throw ProtocolException.FromResponseCode((LID)54816, "Line is too long", ResponseCode.InvalidPayload, null);
						}
						byte[] destinationArray = new byte[Math.Min(this.parseLine.Length * 2, 4096)];
						Array.Copy(this.parseLine, destinationArray, this.parseLine.Length);
						this.parseLine = destinationArray;
					}
					this.parseLine[this.parseLineSize] = b;
					this.parseLineSize++;
				}
			}
			return false;
		}

		private void AddParsedMetadataHistory(string line)
		{
			this.parsedMetadataHistoryTime[this.parsedMetadataHistoryIndex] = this.perfDateTime.UtcNow;
			this.parsedMetadataHistory[this.parsedMetadataHistoryIndex] = line;
			this.parsedMetadataHistoryIndex = (this.parsedMetadataHistoryIndex + 1) % this.parsedMetadataHistory.Length;
		}

		private void GetMetadataAndAdditionalHeaders(StringBuilder stringBuilder)
		{
			bool flag = true;
			DateTime d = DateTime.MinValue;
			int num = this.parsedMetadataHistoryIndex;
			for (int i = 0; i < this.parsedMetadataHistory.Length; i++)
			{
				if (this.parsedMetadataHistory[num] != null)
				{
					if (flag)
					{
						stringBuilder.AppendFormat("{0} [@{1}]\r\n", this.parsedMetadataHistory[num], this.parsedMetadataHistoryTime[num].ToString("o"));
						d = this.parsedMetadataHistoryTime[num];
						flag = false;
					}
					else
					{
						TimeSpan timeSpan = this.parsedMetadataHistoryTime[num] - d;
						stringBuilder.AppendFormat("{0} [+{1}]\r\n", this.parsedMetadataHistory[num], timeSpan.ToString("c"));
					}
				}
				num = (num + 1) % this.parsedMetadataHistory.Length;
			}
			if (base.AdditionalHeaders != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in base.AdditionalHeaders)
				{
					stringBuilder.AppendFormat("{0}: {1}\r\n", keyValuePair.Key, keyValuePair.Value);
				}
			}
			stringBuilder.Append("\r\n");
		}

		private const int MaxLineLength = 4096;

		private const int DefaultLineLength = 32;

		private readonly PerfDateTime perfDateTime;

		private readonly string[] parsedMetadataHistory = new string[100];

		private readonly DateTime[] parsedMetadataHistoryTime = new DateTime[100];

		private int parseLineSize;

		private PendingResponseParser.ParseState state;

		private byte[] parseLine = new byte[32];

		private int parsedMetadataHistoryIndex;

		private bool foundCR;

		private long? responseBodySize = null;

		private long? responseDataSize = null;

		private enum ParseState
		{
			NewCommandLine,
			NewHeaderLine,
			ResponseData
		}
	}
}
