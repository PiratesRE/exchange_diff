using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IMAPResponse
	{
		internal IMAPResponse(SyncLogSession log)
		{
			this.log = log;
			this.parseContext = new IMAPResponse.ParseContext();
			this.responseBuffer = new BufferBuilder(1024);
			this.responseLines = new List<string>(20);
			this.responseLiterals = new List<Stream>(1);
		}

		internal bool IsComplete
		{
			get
			{
				return this.responseIsComplete;
			}
		}

		internal bool IsWaitingForLiteral
		{
			get
			{
				return this.waitingForLiteral;
			}
		}

		internal IMAPStatus Status
		{
			get
			{
				return this.status;
			}
		}

		internal int LiteralBytesRemaining
		{
			get
			{
				return this.literalBytesRemaining;
			}
		}

		internal int TotalLiteralBytesExpected
		{
			get
			{
				return this.curLiteralBytes;
			}
		}

		internal IList<string> ResponseLines
		{
			get
			{
				return this.responseLines.AsReadOnly();
			}
		}

		internal static Exception Fail(string prefix, IMAPCommand command, string context)
		{
			StringBuilder stringBuilder = new StringBuilder(40);
			if (prefix != null)
			{
				stringBuilder.Append(prefix);
				stringBuilder.Append(": ");
			}
			stringBuilder.Append(command.ToPiiCleanString());
			if (context != null)
			{
				stringBuilder.Append(": [");
				stringBuilder.Append(context);
				stringBuilder.Append("]");
			}
			return SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new IMAPException(stringBuilder.ToString()), true);
		}

		internal void Reset(IMAPCommand newCommand)
		{
			if (newCommand == null)
			{
				this.commandId = null;
				this.checkForUnsolicitedByeResponse = true;
			}
			else
			{
				this.commandId = newCommand.CommandId;
				this.checkForUnsolicitedByeResponse = (newCommand.CommandType != IMAPCommandType.Logout);
			}
			this.status = IMAPStatus.Unknown;
			this.responseIsComplete = false;
			this.waitingForLiteral = false;
			this.lastByteRead = 0;
			this.literalBytesRemaining = 0;
			this.curLiteralBytes = 0;
			this.responseBuffer.Reset();
			this.responseLines.Clear();
			this.responseLiterals.Clear();
			if (this.inFlightLiteral != null)
			{
				this.inFlightLiteral.Dispose();
				this.inFlightLiteral = null;
			}
		}

		internal int AddData(byte[] data, int offset, int size)
		{
			int num = size + offset;
			int num2 = offset;
			while (num2 < num && !this.responseIsComplete)
			{
				if (this.literalBytesRemaining > 0)
				{
					int num3 = Math.Min(this.literalBytesRemaining, num - num2);
					this.inFlightLiteral.Write(data, num2, num3);
					num2 += num3 - 1;
					this.literalBytesRemaining -= num3;
					if (this.literalBytesRemaining == 0)
					{
						this.inFlightLiteral.Position = 0L;
						this.responseLiterals.Add(this.inFlightLiteral);
						this.inFlightLiteral = null;
						this.curLiteralBytes = 0;
					}
				}
				else
				{
					this.responseBuffer.Append(data[num2]);
					if (this.lastByteRead == 13 && data[num2] == 10)
					{
						string text = Encoding.ASCII.GetString(this.responseBuffer.GetBuffer(), 0, this.responseBuffer.Length).Trim();
						this.responseLines.Add(text);
						this.responseBuffer.Reset();
						this.ProcessLineDuringReading(text);
					}
					this.lastByteRead = data[num2];
				}
				num2++;
			}
			return num2 - offset;
		}

		internal bool TryParseIntoResult(IMAPCommand command, ref IMAPResultData result)
		{
			SyncUtilities.ThrowIfArgumentNull("command", command);
			SyncUtilities.ThrowIfArgumentNull("result", result);
			if (this.responseLines.Count == 0)
			{
				return false;
			}
			IMAPResponse.TryProcessLine lineProcessor = this.GetLineProcessor(command);
			if (lineProcessor == null)
			{
				result.FailureException = IMAPResponse.Fail("Unknown CommandType", command, null);
				return false;
			}
			string text = this.responseLines[this.responseLines.Count - 1];
			if (!text.StartsWith(command.CommandId + ' ', StringComparison.Ordinal))
			{
				result.FailureException = IMAPResponse.Fail(null, command, text);
				return false;
			}
			IMAPStatus imapstatus = IMAPResponse.CheckAndReturnCommandCompletionCode(text, command.CommandId);
			if (IMAPStatus.Ok != imapstatus)
			{
				return true;
			}
			this.parseContext.Reset(this.responseLines, this.responseLiterals);
			while (!this.parseContext.Error && this.parseContext.MoveNextLines())
			{
				string currentLine = this.parseContext.CurrentLine;
				if (currentLine.Length > 0 && !lineProcessor(command, result) && !IMAPResponse.IsUntaggedResponseLine(currentLine) && !text.StartsWith(command.CommandId + ' ', StringComparison.Ordinal))
				{
					this.log.LogVerbose((TSLID)872UL, IMAPCommClient.Tracer, "Unprocessed line [{0}] in response to command [{1}].", new object[]
					{
						currentLine,
						command.ToPiiCleanString()
					});
				}
			}
			return !this.parseContext.Error;
		}

		internal string GetLastResponseLine()
		{
			if (this.responseLines.Count > 0)
			{
				return this.responseLines[this.responseLines.Count - 1];
			}
			return null;
		}

		private static bool AllowWhitespaceInKey(string fetchResults, int idx, string key)
		{
			if (key == "BODY")
			{
				int num = idx + 1;
				while (num < fetchResults.Length && fetchResults[num] == ' ')
				{
					num++;
				}
				return num < fetchResults.Length && fetchResults[num] == '[';
			}
			return false;
		}

		private static bool TryParseLiteralSize(string details, out int sizeOfPendingLiteral)
		{
			bool result = false;
			sizeOfPendingLiteral = 0;
			Match match;
			if (IMAPResponse.CheckResponse(details, IMAPResponse.LiteralDelimiter, out match))
			{
				result = IMAPResponse.SafeConvert(match, 1, out sizeOfPendingLiteral);
			}
			return result;
		}

		private static IMAPStatus CheckAndReturnCommandCompletionCode(string line, string commandId)
		{
			string text = IMAPResponse.SafeSubstring(line, commandId.Length + 1);
			if (text.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
			{
				return IMAPStatus.Ok;
			}
			if (text.StartsWith("NO", StringComparison.OrdinalIgnoreCase))
			{
				return IMAPStatus.No;
			}
			if (text.StartsWith("BAD", StringComparison.OrdinalIgnoreCase))
			{
				return IMAPStatus.Bad;
			}
			return IMAPStatus.Unknown;
		}

		private static bool CheckResponse(string strToCheck, Regex expectedExpression, out Match match)
		{
			match = expectedExpression.Match(strToCheck);
			return match != null && match.Success;
		}

		private static bool IsUntaggedResponseLine(string line)
		{
			return line.Length > 0 && line[0] == '*';
		}

		private static bool IsServerWaitingForLiteralLine(string line)
		{
			return line.Length > 0 && line[0] == '+';
		}

		private static bool SafeGet(Match match, int groupIdx, out string captureValue)
		{
			bool result = false;
			captureValue = null;
			if (match != null && match.Groups.Count > groupIdx)
			{
				captureValue = match.Groups[groupIdx].Value;
				result = true;
			}
			return result;
		}

		private static bool SafeConvert(Match match, int groupIdx, out int converted)
		{
			bool result = false;
			converted = 0;
			string text;
			if (IMAPResponse.SafeGet(match, groupIdx, out text) && !string.IsNullOrEmpty(text))
			{
				text = text.Trim();
				result = int.TryParse(text, out converted);
			}
			return result;
		}

		private static bool SafeConvert(Match match, int groupIdx, out uint converted)
		{
			bool result = false;
			converted = 0U;
			string text;
			if (IMAPResponse.SafeGet(match, groupIdx, out text) && !string.IsNullOrEmpty(text))
			{
				text = text.Trim();
				result = uint.TryParse(text, out converted);
			}
			return result;
		}

		private static string SafeSubstring(string incoming, int idxToStart)
		{
			if (incoming == null || incoming.Length <= idxToStart)
			{
				return string.Empty;
			}
			return incoming.Substring(idxToStart);
		}

		private void ProcessLineDuringReading(string responseLine)
		{
			this.waitingForLiteral = false;
			if (!IMAPResponse.TryParseLiteralSize(responseLine, out this.curLiteralBytes))
			{
				if (IMAPResponse.IsUntaggedResponseLine(responseLine))
				{
					if (this.commandId == null)
					{
						if (IMAPResponse.UnsolicitedOkResponse.IsMatch(responseLine))
						{
							this.responseIsComplete = true;
							this.status = IMAPStatus.Ok;
						}
						else if (IMAPResponse.UnsolicitedNoResponse.IsMatch(responseLine))
						{
							this.responseIsComplete = true;
							this.status = IMAPStatus.No;
						}
						else if (IMAPResponse.UnsolicitedBadResponse.IsMatch(responseLine))
						{
							this.responseIsComplete = true;
							this.status = IMAPStatus.Bad;
						}
					}
					if (this.checkForUnsolicitedByeResponse && IMAPResponse.UnsolicitedByeResponse.IsMatch(responseLine))
					{
						this.responseIsComplete = true;
						this.status = IMAPStatus.Bye;
						return;
					}
				}
				else
				{
					if (IMAPResponse.IsServerWaitingForLiteralLine(responseLine))
					{
						this.waitingForLiteral = true;
						return;
					}
					if (this.commandId != null && responseLine.StartsWith(this.commandId + ' ', StringComparison.Ordinal))
					{
						this.responseIsComplete = true;
						this.status = IMAPResponse.CheckAndReturnCommandCompletionCode(responseLine, this.commandId);
					}
				}
				return;
			}
			if (this.curLiteralBytes > 0)
			{
				this.literalBytesRemaining = this.curLiteralBytes;
				this.inFlightLiteral = TemporaryStorage.Create();
				return;
			}
			this.responseLiterals.Add(null);
		}

		private bool SimpleResponseProcessor(IMAPCommand command, IMAPResultData resultData)
		{
			bool result = false;
			string currentLine = this.parseContext.CurrentLine;
			if (IMAPResponse.UnsolicitedByeResponse.IsMatch(currentLine))
			{
				result = true;
				resultData.FailureException = IMAPResponse.Fail("Server disconnected", command, currentLine);
			}
			return result;
		}

		private bool CapabilityResponseProcessor(IMAPCommand command, IMAPResultData resultData)
		{
			bool flag = this.SimpleResponseProcessor(command, resultData);
			Match match;
			if (!flag && !this.parseContext.Error && IMAPResponse.CheckResponse(this.parseContext.CurrentLine, IMAPResponse.CapabilityResponse, out match))
			{
				string text;
				if (IMAPResponse.SafeGet(match, 1, out text))
				{
					string[] array = text.Split(new char[]
					{
						' '
					});
					foreach (string item in array)
					{
						resultData.Capabilities.Add(item);
					}
					flag = true;
				}
				else
				{
					resultData.FailureException = IMAPResponse.Fail("Failed to parse result from CAPABILITY", command, this.parseContext.CurrentLine);
					this.parseContext.Error = true;
				}
			}
			return flag;
		}

		private bool LogoutResponseProcessor(IMAPCommand command, IMAPResultData resultData)
		{
			string currentLine = this.parseContext.CurrentLine;
			return IMAPResponse.UnsolicitedByeResponse.IsMatch(currentLine) || this.SimpleResponseProcessor(command, resultData);
		}

		private bool SelectResponseProcessor(IMAPCommand command, IMAPResultData resultData)
		{
			if (resultData.Mailboxes.Count == 0)
			{
				resultData.Mailboxes.Add((IMAPMailbox)command.CommandParameters[0]);
			}
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				Match match;
				if (IMAPResponse.CheckResponse(currentLine, IMAPResponse.OkDataResponse, out match))
				{
					if (match.Groups.Count == 2)
					{
						string value = match.Groups[1].Value;
						Match match2;
						if (IMAPResponse.CheckResponse(value, IMAPResponse.UidValidity, out match2))
						{
							uint num;
							this.parseContext.Error = !IMAPResponse.SafeConvert(match2, 1, out num);
							if (!this.parseContext.Error)
							{
								resultData.Mailboxes[0].UidValidity = new long?((long)((ulong)num));
							}
							else
							{
								resultData.FailureException = IMAPResponse.Fail("Invalid UIDVALIDITY Response", command, match2.ToString());
							}
						}
						if (IMAPResponse.CheckResponse(value, IMAPResponse.UidNext, out match2))
						{
							uint num2;
							this.parseContext.Error = !IMAPResponse.SafeConvert(match2, 1, out num2);
							if (!this.parseContext.Error)
							{
								resultData.Mailboxes[0].UidNext = new long?((long)((ulong)num2));
							}
							else
							{
								resultData.FailureException = IMAPResponse.Fail("Invalid UIDNEXT Response", command, match2.ToString());
							}
						}
						if (IMAPResponse.CheckResponse(value, IMAPResponse.PermanentFlags, out match2))
						{
							string stringForm;
							this.parseContext.Error = !IMAPResponse.SafeGet(match2, 1, out stringForm);
							if (!this.parseContext.Error)
							{
								resultData.Mailboxes[0].PermanentFlags = IMAPUtils.ConvertStringFormToFlags(stringForm);
							}
							else
							{
								resultData.FailureException = IMAPResponse.Fail("Invalid PERMANENTFLAGS Response", command, match2.ToString());
							}
						}
					}
					else
					{
						resultData.FailureException = IMAPResponse.Fail("Invalid OK Response", command, match.ToString());
					}
					flag = true;
				}
				else if (IMAPResponse.CheckResponse(currentLine, IMAPResponse.ExistsResponse, out match))
				{
					uint num3;
					this.parseContext.Error = !IMAPResponse.SafeConvert(match, 1, out num3);
					if (!this.parseContext.Error && num3 < 2147483647U)
					{
						resultData.Mailboxes[0].NumberOfMessages = new int?((int)num3);
					}
					else
					{
						resultData.FailureException = IMAPResponse.Fail("Invalid EXISTS Response", command, match.ToString());
					}
					flag = true;
				}
				else if (currentLine.StartsWith(command.CommandId, StringComparison.OrdinalIgnoreCase))
				{
					if (!currentLine.ToUpperInvariant().Contains("READ-WRITE"))
					{
						resultData.Mailboxes[0].IsWritable = false;
					}
					flag = true;
				}
			}
			return flag;
		}

		private bool StatusResponseProcessor(IMAPCommand command, IMAPResultData resultData)
		{
			if (resultData.Mailboxes.Count == 0)
			{
				resultData.Mailboxes.Add((IMAPMailbox)command.CommandParameters[0]);
			}
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				Match match;
				if (IMAPResponse.CheckResponse(currentLine, IMAPResponse.StatusResponse, out match))
				{
					if (match.Groups.Count == 3)
					{
						uint num;
						this.parseContext.Error = !IMAPResponse.SafeConvert(match, 2, out num);
						if (!this.parseContext.Error)
						{
							resultData.Mailboxes[0].UidNext = new long?((long)((ulong)num));
						}
						else
						{
							resultData.FailureException = IMAPResponse.Fail("Invalid STATUS UIDNEXT Response", command, match.ToString());
						}
					}
					else
					{
						resultData.FailureException = IMAPResponse.Fail("Invalid STATUS Response", command, match.ToString());
					}
					flag = true;
				}
			}
			return flag;
		}

		private bool FetchResponseProcessor(IMAPCommand command, IMAPResultData resultData)
		{
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				if (IMAPResponse.IsUntaggedResponseLine(currentLine))
				{
					Match match;
					if (IMAPResponse.CheckResponse(currentLine, IMAPResponse.FetchResponse, out match))
					{
						this.ProcessFetch(command, match, resultData);
						flag = true;
					}
					else if (!IMAPResponse.IsUntaggedResponseLine(currentLine))
					{
						resultData.FailureException = IMAPResponse.Fail("Unexpected response", command, currentLine);
						this.parseContext.Error = true;
					}
				}
			}
			return flag;
		}

		private bool SearchResponseProcessor(IMAPCommand command, IMAPResultData resultData)
		{
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				Match match;
				if (IMAPResponse.IsUntaggedResponseLine(currentLine) && IMAPResponse.CheckResponse(currentLine, IMAPResponse.SearchResponse, out match))
				{
					string text;
					if (IMAPResponse.SafeGet(match, 1, out text))
					{
						string[] array = text.Split(new char[]
						{
							' '
						});
						foreach (string item in array)
						{
							resultData.MessageUids.Add(item);
						}
						this.log.LogVerbose((TSLID)873UL, "SearchResponseProcessor parsed {0} MessageUids from SEARCH response", new object[]
						{
							array.Length
						});
						flag = true;
					}
					else
					{
						resultData.FailureException = IMAPResponse.Fail("Invalid Search response", command, currentLine);
						this.parseContext.Error = true;
					}
				}
			}
			return flag;
		}

		private bool ListResponseProcessor(IMAPCommand command, IMAPResultData resultData)
		{
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				Match match;
				if (IMAPResponse.IsUntaggedResponseLine(currentLine) && IMAPResponse.CheckResponse(currentLine, IMAPResponse.ListResponse, out match))
				{
					string text;
					string text2;
					string text3;
					if (IMAPResponse.SafeGet(match, 1, out text) && IMAPResponse.SafeGet(match, 2, out text2) && IMAPResponse.SafeGet(match, 3, out text3))
					{
						string text4 = text.ToUpperInvariant();
						char? separator = null;
						if (string.IsNullOrEmpty(text3))
						{
							resultData.FailureException = IMAPResponse.Fail("Invalid List response. Empty mailbox name.", command, currentLine);
							this.parseContext.Error = true;
							return true;
						}
						text3 = this.ConvertQuotedStringIfRequired(text3);
						if (string.IsNullOrEmpty(text3))
						{
							resultData.FailureException = IMAPResponse.Fail("Invalid List response. Could not convert the mailbox name.", command, currentLine);
							this.parseContext.Error = true;
							return true;
						}
						if (text2.Length == 1)
						{
							separator = new char?(text2[0]);
						}
						else if (text2.Length == 2 && text2[0] == '\\')
						{
							separator = new char?(text2[1]);
						}
						if (separator != null && text3[text3.Length - 1] == separator.Value)
						{
							return true;
						}
						if (IMAPResponse.LiteralDelimiter.IsMatch(text3))
						{
							if (!this.parseContext.MoveNextLiterals())
							{
								resultData.FailureException = IMAPResponse.Fail("Failed to read literal mailbox name", command, currentLine);
								this.parseContext.Error = true;
								return true;
							}
							Stream currentLiteral = this.parseContext.CurrentLiteral;
							currentLiteral.Position = 0L;
							using (StreamReader streamReader = new StreamReader(currentLiteral))
							{
								text3 = streamReader.ReadToEnd();
							}
							this.log.LogVerbose((TSLID)874UL, "Folder received as literal: {0} => {1}", new object[]
							{
								currentLine,
								text3
							});
						}
						IMAPMailbox imapmailbox = new IMAPMailbox(text3);
						imapmailbox.Separator = separator;
						if (text4.Contains("\\NOSELECT"))
						{
							imapmailbox.IsSelectable = false;
						}
						if (text4.Contains("\\HASNOCHILDREN"))
						{
							imapmailbox.HasChildren = new bool?(false);
						}
						if (text4.Contains("\\HASCHILDREN"))
						{
							imapmailbox.HasChildren = new bool?(true);
						}
						if (text4.Contains("\\NOINFERIORS") || text2 == "NIL")
						{
							imapmailbox.NoInferiors = true;
							imapmailbox.HasChildren = new bool?(false);
						}
						resultData.Mailboxes.Add(imapmailbox);
						flag = true;
					}
					else
					{
						resultData.FailureException = IMAPResponse.Fail("Invalid List response. Could not parse it.", command, currentLine);
						this.parseContext.Error = true;
					}
				}
			}
			return flag;
		}

		private bool AppendResponseProcessor(IMAPCommand command, IMAPResultData resultData)
		{
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				Match match;
				if (currentLine.StartsWith(command.CommandId, StringComparison.OrdinalIgnoreCase) && IMAPResponse.CheckResponse(currentLine, IMAPResponse.AppendUidResponse, out match))
				{
					uint num;
					this.parseContext.Error = !IMAPResponse.SafeConvert(match, 2, out num);
					if (!this.parseContext.Error)
					{
						resultData.MessageUids.Add(num.ToString());
						this.log.LogVerbose((TSLID)875UL, "AppendResponseProcessor parsed uidMessage {0} from APPENDUID response", new object[]
						{
							num
						});
					}
					else
					{
						this.log.LogInformation((TSLID)876UL, "Invalid APPENDUID uidMessage {0}. Ignoring response.", new object[]
						{
							num
						});
					}
					flag = true;
				}
			}
			return flag;
		}

		private IMAPResponse.TryProcessLine GetLineProcessor(IMAPCommand command)
		{
			IMAPCommandType commandType = command.CommandType;
			IMAPResponse.TryProcessLine result;
			switch (commandType)
			{
			case IMAPCommandType.None:
			case IMAPCommandType.Noop:
			case IMAPCommandType.CreateMailbox:
			case IMAPCommandType.DeleteMailbox:
			case IMAPCommandType.RenameMailbox:
			case IMAPCommandType.Store:
			case IMAPCommandType.Expunge:
			case IMAPCommandType.Id:
			case IMAPCommandType.Starttls:
			case IMAPCommandType.Authenticate:
				result = new IMAPResponse.TryProcessLine(this.SimpleResponseProcessor);
				break;
			case IMAPCommandType.Login:
			case IMAPCommandType.Capability:
				result = new IMAPResponse.TryProcessLine(this.CapabilityResponseProcessor);
				break;
			case IMAPCommandType.Logout:
				result = new IMAPResponse.TryProcessLine(this.LogoutResponseProcessor);
				break;
			case IMAPCommandType.Select:
				result = new IMAPResponse.TryProcessLine(this.SelectResponseProcessor);
				break;
			case IMAPCommandType.Fetch:
			{
				IList<string> list = (IList<string>)command.CommandParameters[3];
				this.fetchMessageIds = list.Contains("BODY.PEEK[HEADER.FIELDS (Message-ID)]");
				this.fetchMessageSizes = list.Contains("RFC822.SIZE");
				this.fetchMessageInternalDates = list.Contains("INTERNALDATE");
				result = new IMAPResponse.TryProcessLine(this.FetchResponseProcessor);
				break;
			}
			case IMAPCommandType.Append:
				result = new IMAPResponse.TryProcessLine(this.AppendResponseProcessor);
				break;
			case IMAPCommandType.Search:
				result = new IMAPResponse.TryProcessLine(this.SearchResponseProcessor);
				break;
			case IMAPCommandType.List:
				result = new IMAPResponse.TryProcessLine(this.ListResponseProcessor);
				break;
			case IMAPCommandType.Status:
				result = new IMAPResponse.TryProcessLine(this.StatusResponseProcessor);
				break;
			default:
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unknown command type {0}, no line processor.", new object[]
				{
					commandType
				}));
			}
			return result;
		}

		private void ProcessFetch(IMAPCommand command, Match match, IMAPResultData resultData)
		{
			string text;
			string text2;
			if (IMAPResponse.SafeGet(match, 1, out text) && IMAPResponse.SafeGet(match, 2, out text2))
			{
				text2 = text2.Trim();
				if (text2.StartsWith("(", StringComparison.Ordinal))
				{
					if (!resultData.MessageSeqNumsHashSet.Contains(text))
					{
						int num;
						if (int.TryParse(text, out num))
						{
							resultData.MessageSeqNumsHashSet.Add(text);
							resultData.MessageSeqNums.Add(num);
							resultData.UidAlreadySeen = false;
							if (text2.EndsWith(")", StringComparison.Ordinal))
							{
								this.ProcessFetchResults(command, text2, resultData);
							}
							else
							{
								StringBuilder cachedBuilder = this.parseContext.CachedBuilder;
								cachedBuilder.Length = 0;
								cachedBuilder.Append(text2);
								bool flag;
								do
								{
									if (this.parseContext.MoveNextLines())
									{
										string currentLine = this.parseContext.CurrentLine;
										cachedBuilder.Append(" ");
										cachedBuilder.Append(currentLine);
										flag = currentLine.EndsWith(")", StringComparison.Ordinal);
									}
									else
									{
										resultData.FailureException = IMAPResponse.Fail("Read past end of response", command, cachedBuilder.ToString());
										this.parseContext.Error = true;
										flag = true;
									}
								}
								while (!flag);
								this.ProcessFetchResults(command, cachedBuilder.ToString(), resultData);
							}
							if (!this.parseContext.Error && (resultData.LowestSequenceNumber == null || num < resultData.LowestSequenceNumber))
							{
								resultData.LowestSequenceNumber = new int?(num);
							}
						}
						else
						{
							resultData.FailureException = IMAPResponse.Fail("Invalid FETCH data, cannot parse the sequence number.", command, text2.ToString());
							this.parseContext.Error = true;
						}
					}
					else
					{
						this.log.LogError((TSLID)877UL, IMAPCommClient.Tracer, "Invalid FETCH data, already processed a line with the same message sequence number: {0}, command: {1}, fetchContents: {2}", new object[]
						{
							text,
							command.ToPiiCleanString(),
							text2
						});
					}
				}
				else
				{
					this.parseContext.Error = true;
					resultData.FailureException = IMAPResponse.Fail("Missing FETCH data", command, text2);
				}
			}
			else
			{
				this.parseContext.Error = true;
				resultData.FailureException = IMAPResponse.Fail("Parse error", command, match.ToString());
			}
			if (resultData.FailureException != null)
			{
				this.log.LogError((TSLID)878UL, IMAPCommClient.Tracer, "Error encountered in ProcessFetch: {0}", new object[]
				{
					resultData.FailureException
				});
			}
		}

		private void ProcessFetchResults(IMAPCommand command, string fetchResults, IMAPResultData resultData)
		{
			int count = resultData.MessageUids.Count;
			int num = 0;
			string text = null;
			StringBuilder cachedBuilder = this.parseContext.CachedBuilder;
			cachedBuilder.Length = 0;
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < fetchResults.Length; i++)
			{
				char c = fetchResults[i];
				if (!flag3 && (c == '(' || c == '['))
				{
					stringBuilder.Append(c);
					flag2 = false;
					if (num > 0)
					{
						cachedBuilder.Append(c);
					}
					num++;
				}
				else if (!flag3 && (c == ')' || c == ']'))
				{
					stringBuilder.Append(c);
					flag2 = false;
					num--;
					if (num > 0)
					{
						cachedBuilder.Append(c);
					}
				}
				else if (c == '"')
				{
					stringBuilder.Append(c);
					flag2 = false;
					flag3 = !flag3;
				}
				else if (!flag3 && c == ' ' && num == 1)
				{
					stringBuilder.Append(c);
					flag2 = false;
					string text2 = cachedBuilder.ToString();
					if (text == null)
					{
						if (IMAPResponse.AllowWhitespaceInKey(fetchResults, i, text2))
						{
							cachedBuilder.Append(c);
						}
						else
						{
							text = text2;
							cachedBuilder.Length = 0;
							flag = false;
						}
					}
					else
					{
						this.ProcessFetchKeyValueToResult(command, resultData, text, text2);
						text = null;
						flag = true;
						cachedBuilder.Length = 0;
					}
				}
				else
				{
					if (flag && cachedBuilder.Length < 4)
					{
						stringBuilder.Append(c);
					}
					else if (!flag2)
					{
						stringBuilder.Append('#');
						flag2 = true;
					}
					cachedBuilder.Append(c);
				}
			}
			if (text != null)
			{
				string value = cachedBuilder.ToString();
				this.ProcessFetchKeyValueToResult(command, resultData, text, value);
			}
			if (num != 0)
			{
				this.parseContext.Error = true;
				resultData.FailureException = IMAPResponse.Fail("Unbalanced FETCH response", command, fetchResults);
			}
			if (this.fetchMessageIds && resultData.MessageIds.Count == count)
			{
				this.log.LogDebugging((TSLID)879UL, IMAPCommClient.Tracer, "No MessageId found in this line of the FETCH result: {0}", new object[]
				{
					stringBuilder.ToString()
				});
				resultData.MessageIds.Add(null);
			}
			if (this.fetchMessageSizes && resultData.MessageSizes.Count == count)
			{
				resultData.MessageSizes.Add(0L);
			}
			if (resultData.MessageUids.Count == count)
			{
				this.log.LogError((TSLID)880UL, IMAPCommClient.Tracer, "No UID element found (which is mandatory) in this line of the FETCH result: {0}", new object[]
				{
					stringBuilder.ToString()
				});
				resultData.MessageUids.Add(null);
			}
			if (resultData.MessageFlags.Count == count)
			{
				resultData.MessageFlags.Add(IMAPMailFlags.None);
			}
			if (this.fetchMessageInternalDates && resultData.MessageInternalDates.Count == count)
			{
				this.log.LogDebugging((TSLID)881UL, IMAPCommClient.Tracer, "No INTERNALDATE found in this line of the FETCH result: {0}", new object[]
				{
					stringBuilder.ToString()
				});
				resultData.MessageInternalDates.Add(null);
			}
		}

		private void ProcessFetchKeyValueToResult(IMAPCommand command, IMAPResultData resultData, string key, string value)
		{
			IList<string> list = (IList<string>)command.CommandParameters[3];
			if (list != null && (list.Contains(key) || IMAPResponse.HeaderFetch.IsMatch(key) || IMAPResponse.BodyKeyRegex.IsMatch(key)))
			{
				if ("UID".Equals(key, StringComparison.OrdinalIgnoreCase) && !resultData.UidAlreadySeen)
				{
					resultData.MessageUids.Add(value);
					resultData.UidAlreadySeen = true;
					return;
				}
				if ("RFC822.SIZE".Equals(key, StringComparison.OrdinalIgnoreCase))
				{
					long item = 0L;
					if (!long.TryParse(value, out item))
					{
						resultData.FailureException = IMAPResponse.Fail("Could not parse message size.", command, key);
						this.parseContext.Error = true;
					}
					resultData.MessageSizes.Add(item);
					return;
				}
				if ("FLAGS".Equals(key, StringComparison.OrdinalIgnoreCase))
				{
					resultData.MessageFlags.Add(IMAPUtils.ConvertStringFormToFlags(value));
					return;
				}
				if ("INTERNALDATE".Equals(key, StringComparison.OrdinalIgnoreCase))
				{
					DateTime dateTime = SyncUtilities.ToDateTime(value);
					resultData.MessageInternalDates.Add(new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, dateTime)));
					return;
				}
				Match match;
				if (IMAPResponse.BodyKeyRegex.IsMatch(key))
				{
					if (this.parseContext.MoveNextLiterals())
					{
						resultData.MessageStream = this.parseContext.CurrentLiteral;
						return;
					}
					resultData.FailureException = IMAPResponse.Fail("More literal references than literals", command, key);
					this.parseContext.Error = true;
					return;
				}
				else if (IMAPResponse.CheckResponse(key, IMAPResponse.HeaderFetch, out match))
				{
					this.ProcessFetchHeaderDataToResult(command, resultData, key);
					return;
				}
			}
			else
			{
				this.log.LogVerbose((TSLID)882UL, IMAPCommClient.Tracer, "Unexpected token while processing FETCH data: {0} = {1}", new object[]
				{
					key,
					value
				});
			}
		}

		private void ProcessFetchHeaderDataToResult(IMAPCommand command, IMAPResultData resultData, string key)
		{
			if (!this.parseContext.MoveNextLiterals() || this.parseContext.CurrentLiteral == null)
			{
				this.log.LogDebugging((TSLID)1364UL, IMAPCommClient.Tracer, "No message id literal for command: {0}, key: {1}", new object[]
				{
					command,
					key
				});
				return;
			}
			Stream currentLiteral = this.parseContext.CurrentLiteral;
			currentLiteral.Position = 0L;
			string text = null;
			bool flag = false;
			this.responseBuffer.Reset();
			int num;
			while ((num = currentLiteral.ReadByte()) != -1)
			{
				if (flag && num == 10 && text != null && this.responseBuffer.Length > 0)
				{
					string @string = Encoding.ASCII.GetString(this.responseBuffer.GetBuffer(), 0, this.responseBuffer.Length);
					if ("Message-ID".Equals(text, StringComparison.OrdinalIgnoreCase))
					{
						resultData.MessageIds.Add(@string.Trim());
					}
					else
					{
						this.log.LogVerbose((TSLID)883UL, IMAPCommClient.Tracer, "Ignoring header field: {0}: {1}", new object[]
						{
							text,
							@string.Trim()
						});
					}
					text = null;
					this.responseBuffer.Reset();
				}
				if (num == 13)
				{
					flag = true;
				}
				else
				{
					flag = false;
					if (num == 58 && text == null && this.responseBuffer.Length > 0)
					{
						text = Encoding.ASCII.GetString(this.responseBuffer.GetBuffer(), 0, this.responseBuffer.Length);
						this.responseBuffer.Reset();
					}
					else if ((num != 32 && num != 9) || this.responseBuffer.Length != 0)
					{
						this.responseBuffer.Append((byte)num);
					}
				}
			}
		}

		private string ConvertQuotedStringIfRequired(string incoming)
		{
			if (incoming != null && incoming.Length > 1 && incoming[0] == '"' && incoming[incoming.Length - 1] == '"')
			{
				StringBuilder cachedBuilder = this.parseContext.CachedBuilder;
				cachedBuilder.Length = 0;
				for (int i = 1; i < incoming.Length - 1; i++)
				{
					if (incoming[i] == '\\')
					{
						i++;
						if (i == incoming.Length - 1)
						{
							return null;
						}
					}
					cachedBuilder.Append(incoming[i]);
				}
				if (!cachedBuilder.ToString().Equals(incoming))
				{
					this.log.LogVerbose((TSLID)1411UL, IMAPCommClient.Tracer, "Incoming string {0} converted to {1}", new object[]
					{
						incoming,
						cachedBuilder.ToString()
					});
				}
				return cachedBuilder.ToString();
			}
			return incoming;
		}

		internal const string UIDValidityKey = "UIDVALIDITY";

		internal const string UIDNextKey = "UIDNEXT";

		internal const string UIDKey = "UID";

		internal const string MessageSizeEstimateKey = "RFC822.SIZE";

		internal const string FlagsKey = "FLAGS";

		internal const string InternalDateKey = "INTERNALDATE";

		internal const string Body = "BODY";

		internal const string BodyKey = "BODY.PEEK[]";

		internal const string BodyHeaderFieldsMessageId = "BODY.PEEK[HEADER.FIELDS (Message-ID)]";

		private const string OkResponsePrefix = "OK";

		private const string NoResponsePrefix = "NO";

		private const string BadResponsePrefix = "BAD";

		private const string NonSelectableFolderFlag = "\\NOSELECT";

		private const string HasChildrenFolderFlag = "\\HASCHILDREN";

		private const string HasNoChildrenFolderFlag = "\\HASNOCHILDREN";

		private const string NoInferiorsFolderFlag = "\\NOINFERIORS";

		private const string MessageIdHeader = "Message-ID";

		private const string ReadWriteFlag = "READ-WRITE";

		private const int DefaultBufferSize = 1024;

		private const int DefaultLineCollectionSize = 20;

		private const int DefaultLiteralCollectionSize = 1;

		private const char ServerLiteralRequestIndicator = '+';

		private const char TokenDelimiter = ' ';

		private const int MaxKeyCharactersLogged = 4;

		private static readonly Regex OkDataResponse = new Regex("\\* OK \\[(.*)\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex UnsolicitedOkResponse = new Regex("\\* OK(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex UnsolicitedNoResponse = new Regex("\\* NO(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex UnsolicitedBadResponse = new Regex("\\* BAD(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex UnsolicitedByeResponse = new Regex("\\* BYE(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex FetchResponse = new Regex("\\* ([0-9]+) FETCH (.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex StatusResponse = new Regex("\\* {1,2}STATUS (.*) \\(UIDNEXT ([0-9]+)\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex ListResponse = new Regex("\\* LIST \\((.*)\\) \"?(NIL|\\\\?[^ \"]?)\"? (.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex SearchResponse = new Regex("\\* SEARCH (.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex AppendUidResponse = new Regex(".+ .+ \\[APPENDUID ([0-9]+) ([0-9]+)\\].*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex ExistsResponse = new Regex("\\* ([0-9]+) EXISTS", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex CapabilityResponse = new Regex("\\* CAPABILITY (.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex UidValidity = new Regex("UIDVALIDITY ([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex UidNext = new Regex("UIDNEXT ([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex StatusUidNext = new Regex("UIDNEXT \\(([0-9]+)\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex PermanentFlags = new Regex("PERMANENTFLAGS \\((.*)\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex LiteralDelimiter = new Regex("{([0-9]+)}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex BodyKeyRegex = new Regex("BODY\\s*\\[\\s*\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex HeaderFetch = new Regex("BODY\\s*\\[HEADER.FIELDS \\((.+)\\)\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private string commandId;

		private BufferBuilder responseBuffer;

		private byte lastByteRead;

		private List<string> responseLines;

		private Stream inFlightLiteral;

		private List<Stream> responseLiterals;

		private bool checkForUnsolicitedByeResponse;

		private bool responseIsComplete;

		private bool waitingForLiteral;

		private IMAPStatus status;

		private int literalBytesRemaining;

		private int curLiteralBytes;

		private IMAPResponse.ParseContext parseContext;

		private SyncLogSession log;

		private bool fetchMessageIds;

		private bool fetchMessageInternalDates;

		private bool fetchMessageSizes;

		private delegate bool TryProcessLine(IMAPCommand command, IMAPResultData resultData);

		private class ParseContext
		{
			internal ParseContext()
			{
			}

			internal bool Error
			{
				get
				{
					return this.parseError;
				}
				set
				{
					this.parseError = value;
				}
			}

			internal string CurrentLine
			{
				get
				{
					return this.lineItr.Current;
				}
			}

			internal Stream CurrentLiteral
			{
				get
				{
					return this.literalItr.Current;
				}
			}

			internal StringBuilder CachedBuilder
			{
				get
				{
					return this.cachedBuilder;
				}
			}

			internal void Reset(IList<string> responseLines, IList<Stream> responseLiterals)
			{
				this.lineItr = responseLines.GetEnumerator();
				this.literalItr = responseLiterals.GetEnumerator();
				this.parseError = false;
				this.cachedBuilder.Length = 0;
			}

			internal bool MoveNextLines()
			{
				return this.lineItr.MoveNext();
			}

			internal bool MoveNextLiterals()
			{
				return this.literalItr.MoveNext();
			}

			private StringBuilder cachedBuilder = new StringBuilder();

			private IEnumerator<string> lineItr;

			private IEnumerator<Stream> literalItr;

			private bool parseError;
		}
	}
}
