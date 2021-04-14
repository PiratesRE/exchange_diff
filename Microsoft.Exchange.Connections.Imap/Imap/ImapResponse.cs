using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ImapResponse
	{
		internal ImapResponse(ILog log)
		{
			this.log = log;
			this.parseContext = new ImapResponse.ParseContext();
			this.responseBuffer = new BufferBuilder(1024);
			this.responseLines = new List<string>(20);
			this.responseLiterals = new List<Stream>(1);
		}

		internal bool IsComplete { get; private set; }

		internal bool IsWaitingForLiteral { get; private set; }

		internal ImapStatus Status { get; private set; }

		internal int LiteralBytesRemaining { get; private set; }

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

		internal static Exception Fail(string prefix, ImapCommand command, string context)
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
			return new ImapCommunicationException(stringBuilder.ToString(), RetryPolicy.Backoff);
		}

		internal void Reset(ImapCommand newCommand)
		{
			if (newCommand == null)
			{
				this.commandId = null;
				this.checkForUnsolicitedByeResponse = true;
			}
			else
			{
				this.commandId = newCommand.CommandId;
				this.checkForUnsolicitedByeResponse = (newCommand.CommandType != ImapCommandType.Logout);
			}
			this.Status = ImapStatus.Unknown;
			this.IsComplete = false;
			this.IsWaitingForLiteral = false;
			this.lastByteRead = 0;
			this.LiteralBytesRemaining = 0;
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
			while (num2 < num && !this.IsComplete)
			{
				if (this.LiteralBytesRemaining > 0)
				{
					int num3 = Math.Min(this.LiteralBytesRemaining, num - num2);
					this.log.Assert(this.inFlightLiteral != null, "inFlightLiteral is null.", new object[0]);
					this.inFlightLiteral.Write(data, num2, num3);
					num2 += num3 - 1;
					this.LiteralBytesRemaining -= num3;
					if (this.LiteralBytesRemaining == 0)
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

		internal bool TryParseIntoResult(ImapCommand command, ref ImapResultData result)
		{
			if (this.responseLines.Count == 0)
			{
				return false;
			}
			ImapResponse.TryProcessLine lineProcessor = this.GetLineProcessor(command);
			if (lineProcessor == null)
			{
				result.FailureException = ImapResponse.Fail("Unknown CommandType", command, null);
				return false;
			}
			string text = this.responseLines[this.responseLines.Count - 1];
			if (!text.StartsWith(command.CommandId + ' ', StringComparison.Ordinal))
			{
				result.FailureException = ImapResponse.Fail(null, command, text);
				return false;
			}
			ImapStatus imapStatus = ImapResponse.CheckAndReturnCommandCompletionCode(text, command.CommandId);
			if (imapStatus != ImapStatus.Ok)
			{
				return true;
			}
			this.parseContext.Reset(this.responseLines, this.responseLiterals);
			while (!this.parseContext.Error && this.parseContext.MoveNextLines())
			{
				string currentLine = this.parseContext.CurrentLine;
				if (currentLine.Length > 0 && !lineProcessor(command, result) && !ImapResponse.IsUntaggedResponseLine(currentLine) && !text.StartsWith(command.CommandId + ' ', StringComparison.Ordinal))
				{
					this.log.Debug("Unprocessed line [{0}] in response to command [{1}].", new object[]
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
			if (this.responseLines.Count <= 0)
			{
				return null;
			}
			return this.responseLines[this.responseLines.Count - 1];
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
			if (ImapResponse.CheckResponse(details, ImapResponse.LiteralDelimiter, out match))
			{
				result = ImapResponse.SafeConvert(match, 1, out sizeOfPendingLiteral);
			}
			return result;
		}

		private static ImapStatus CheckAndReturnCommandCompletionCode(string line, string commandId)
		{
			string text = ImapResponse.SafeSubstring(line, commandId.Length + 1);
			if (text.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
			{
				return ImapStatus.Ok;
			}
			if (text.StartsWith("NO", StringComparison.OrdinalIgnoreCase))
			{
				return ImapStatus.No;
			}
			if (text.StartsWith("BAD", StringComparison.OrdinalIgnoreCase))
			{
				return ImapStatus.Bad;
			}
			return ImapStatus.Unknown;
		}

		private static bool CheckResponse(string strToCheck, Regex expectedExpression, out Match match)
		{
			match = expectedExpression.Match(strToCheck);
			return match.Success;
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
			if (ImapResponse.SafeGet(match, groupIdx, out text) && !string.IsNullOrEmpty(text))
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
			if (ImapResponse.SafeGet(match, groupIdx, out text) && !string.IsNullOrEmpty(text))
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
			this.IsWaitingForLiteral = false;
			if (!ImapResponse.TryParseLiteralSize(responseLine, out this.curLiteralBytes))
			{
				if (ImapResponse.IsUntaggedResponseLine(responseLine))
				{
					if (this.commandId == null)
					{
						if (ImapResponse.UnsolicitedOkResponse.IsMatch(responseLine))
						{
							this.IsComplete = true;
							this.Status = ImapStatus.Ok;
						}
						else if (ImapResponse.UnsolicitedNoResponse.IsMatch(responseLine))
						{
							this.IsComplete = true;
							this.Status = ImapStatus.No;
						}
						else if (ImapResponse.UnsolicitedBadResponse.IsMatch(responseLine))
						{
							this.IsComplete = true;
							this.Status = ImapStatus.Bad;
						}
					}
					if (this.checkForUnsolicitedByeResponse && ImapResponse.UnsolicitedByeResponse.IsMatch(responseLine))
					{
						this.IsComplete = true;
						this.Status = ImapStatus.Bye;
						return;
					}
				}
				else
				{
					if (ImapResponse.IsServerWaitingForLiteralLine(responseLine))
					{
						this.IsWaitingForLiteral = true;
						return;
					}
					if (this.commandId != null && responseLine.StartsWith(this.commandId + ' ', StringComparison.Ordinal))
					{
						this.IsComplete = true;
						this.Status = ImapResponse.CheckAndReturnCommandCompletionCode(responseLine, this.commandId);
					}
				}
				return;
			}
			if (this.curLiteralBytes > 0)
			{
				this.log.Assert(this.inFlightLiteral == null, "No literal should be in-flight.", new object[0]);
				this.LiteralBytesRemaining = this.curLiteralBytes;
				this.inFlightLiteral = TemporaryStorage.Create();
				return;
			}
			this.responseLiterals.Add(null);
		}

		private bool SimpleResponseProcessor(ImapCommand command, ImapResultData resultData)
		{
			bool result = false;
			string currentLine = this.parseContext.CurrentLine;
			if (ImapResponse.UnsolicitedByeResponse.IsMatch(currentLine))
			{
				result = true;
				resultData.FailureException = ImapResponse.Fail("Server disconnected", command, currentLine);
			}
			return result;
		}

		private bool CapabilityResponseProcessor(ImapCommand command, ImapResultData resultData)
		{
			bool flag = this.SimpleResponseProcessor(command, resultData);
			Match match;
			if (!flag && !this.parseContext.Error && ImapResponse.CheckResponse(this.parseContext.CurrentLine, ImapResponse.CapabilityResponse, out match))
			{
				string text;
				if (ImapResponse.SafeGet(match, 1, out text))
				{
					string[] capabilities = text.Split(new char[]
					{
						' '
					});
					resultData.Capabilities = new ImapServerCapabilities(capabilities);
					flag = true;
				}
				else
				{
					resultData.FailureException = ImapResponse.Fail("Failed to parse result from CAPABILITY", command, this.parseContext.CurrentLine);
					this.parseContext.Error = true;
				}
			}
			return flag;
		}

		private bool LogoutResponseProcessor(ImapCommand command, ImapResultData resultData)
		{
			string currentLine = this.parseContext.CurrentLine;
			return ImapResponse.UnsolicitedByeResponse.IsMatch(currentLine) || this.SimpleResponseProcessor(command, resultData);
		}

		private bool SelectResponseProcessor(ImapCommand command, ImapResultData resultData)
		{
			if (resultData.Mailboxes.Count == 0)
			{
				resultData.Mailboxes.Add((ImapMailbox)command.CommandParameters[0]);
			}
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				Match match;
				if (ImapResponse.CheckResponse(currentLine, ImapResponse.OkDataResponse, out match))
				{
					if (match.Groups.Count == 2)
					{
						string value = match.Groups[1].Value;
						Match match2;
						if (ImapResponse.CheckResponse(value, ImapResponse.UidValidity, out match2))
						{
							uint num;
							this.parseContext.Error = !ImapResponse.SafeConvert(match2, 1, out num);
							if (!this.parseContext.Error)
							{
								resultData.Mailboxes[0].UidValidity = new long?((long)((ulong)num));
							}
							else
							{
								resultData.FailureException = ImapResponse.Fail("Invalid UIDVALIDITY Response", command, match2.ToString());
							}
						}
						if (ImapResponse.CheckResponse(value, ImapResponse.UidNext, out match2))
						{
							uint num2;
							this.parseContext.Error = !ImapResponse.SafeConvert(match2, 1, out num2);
							if (!this.parseContext.Error)
							{
								resultData.Mailboxes[0].UidNext = new long?((long)((ulong)num2));
							}
							else
							{
								resultData.FailureException = ImapResponse.Fail("Invalid UIDNEXT Response", command, match2.ToString());
							}
						}
						if (ImapResponse.CheckResponse(value, ImapResponse.PermanentFlags, out match2))
						{
							string stringForm;
							this.parseContext.Error = !ImapResponse.SafeGet(match2, 1, out stringForm);
							if (!this.parseContext.Error)
							{
								resultData.Mailboxes[0].PermanentFlags = ImapUtilities.ConvertStringFormToFlags(stringForm);
							}
							else
							{
								resultData.FailureException = ImapResponse.Fail("Invalid PERMANENTFLAGS Response", command, match2.ToString());
							}
						}
					}
					else
					{
						resultData.FailureException = ImapResponse.Fail("Invalid OK Response", command, match.ToString());
					}
					flag = true;
				}
				else if (ImapResponse.CheckResponse(currentLine, ImapResponse.ExistsResponse, out match))
				{
					uint num3;
					this.parseContext.Error = !ImapResponse.SafeConvert(match, 1, out num3);
					if (!this.parseContext.Error && num3 < 2147483647U)
					{
						resultData.Mailboxes[0].NumberOfMessages = new int?((int)num3);
					}
					else
					{
						resultData.FailureException = ImapResponse.Fail("Invalid EXISTS Response", command, match.ToString());
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

		private bool StatusResponseProcessor(ImapCommand command, ImapResultData resultData)
		{
			if (resultData.Mailboxes.Count == 0)
			{
				resultData.Mailboxes.Add((ImapMailbox)command.CommandParameters[0]);
			}
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				Match match;
				if (ImapResponse.CheckResponse(currentLine, ImapResponse.StatusResponse, out match))
				{
					if (match.Groups.Count == 3)
					{
						uint num;
						this.parseContext.Error = !ImapResponse.SafeConvert(match, 2, out num);
						if (!this.parseContext.Error)
						{
							resultData.Mailboxes[0].UidNext = new long?((long)((ulong)num));
						}
						else
						{
							resultData.FailureException = ImapResponse.Fail("Invalid STATUS UIDNEXT Response", command, match.ToString());
						}
					}
					else
					{
						resultData.FailureException = ImapResponse.Fail("Invalid STATUS Response", command, match.ToString());
					}
					flag = true;
				}
			}
			return flag;
		}

		private bool FetchResponseProcessor(ImapCommand command, ImapResultData resultData)
		{
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				if (ImapResponse.IsUntaggedResponseLine(currentLine))
				{
					Match match;
					if (ImapResponse.CheckResponse(currentLine, ImapResponse.FetchResponse, out match))
					{
						this.ProcessFetch(command, match, resultData);
						flag = true;
					}
					else if (!ImapResponse.IsUntaggedResponseLine(currentLine))
					{
						resultData.FailureException = ImapResponse.Fail("Unexpected response", command, currentLine);
						this.parseContext.Error = true;
					}
				}
			}
			return flag;
		}

		private bool SearchResponseProcessor(ImapCommand command, ImapResultData resultData)
		{
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				Match match;
				if (ImapResponse.IsUntaggedResponseLine(currentLine) && ImapResponse.CheckResponse(currentLine, ImapResponse.SearchResponse, out match))
				{
					string text;
					if (ImapResponse.SafeGet(match, 1, out text))
					{
						string[] array = text.Split(new char[]
						{
							' '
						});
						foreach (string item in array)
						{
							resultData.MessageUids.Add(item);
						}
						this.log.Debug("SearchResponseProcessor parsed {0} MessageUids from SEARCH response", new object[]
						{
							array.Length
						});
						flag = true;
					}
					else
					{
						resultData.FailureException = ImapResponse.Fail("Invalid Search response", command, currentLine);
						this.parseContext.Error = true;
					}
				}
			}
			return flag;
		}

		private bool ListResponseProcessor(ImapCommand command, ImapResultData resultData)
		{
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				Match match;
				if (ImapResponse.IsUntaggedResponseLine(currentLine) && ImapResponse.CheckResponse(currentLine, ImapResponse.ListResponse, out match))
				{
					string text;
					string text2;
					string text3;
					if (ImapResponse.SafeGet(match, 1, out text) && ImapResponse.SafeGet(match, 2, out text2) && ImapResponse.SafeGet(match, 3, out text3))
					{
						string text4 = text.ToUpperInvariant();
						char? separator = null;
						if (string.IsNullOrEmpty(text3))
						{
							resultData.FailureException = ImapResponse.Fail("Invalid List response. Empty mailbox name.", command, currentLine);
							this.parseContext.Error = true;
							return true;
						}
						text3 = this.ConvertQuotedStringIfRequired(text3);
						if (string.IsNullOrEmpty(text3))
						{
							resultData.FailureException = ImapResponse.Fail("Invalid List response. Could not convert the mailbox name.", command, currentLine);
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
						if (ImapResponse.LiteralDelimiter.IsMatch(text3))
						{
							if (!this.parseContext.MoveNextLiterals())
							{
								resultData.FailureException = ImapResponse.Fail("Failed to read literal mailbox name", command, currentLine);
								this.parseContext.Error = true;
								return true;
							}
							Stream currentLiteral = this.parseContext.CurrentLiteral;
							currentLiteral.Position = 0L;
							using (StreamReader streamReader = new StreamReader(currentLiteral))
							{
								text3 = streamReader.ReadToEnd();
							}
							this.log.Debug("Folder received as literal: {0} => {1}", new object[]
							{
								currentLine,
								text3
							});
						}
						ImapMailbox imapMailbox = new ImapMailbox(text3);
						imapMailbox.Separator = separator;
						if (text4.Contains("\\NOSELECT"))
						{
							imapMailbox.IsSelectable = false;
						}
						if (text4.Contains("\\HASNOCHILDREN"))
						{
							imapMailbox.HasChildren = new bool?(false);
						}
						if (text4.Contains("\\HASCHILDREN"))
						{
							imapMailbox.HasChildren = new bool?(true);
						}
						if (text4.Contains("\\NOINFERIORS") || text2 == "NIL")
						{
							imapMailbox.NoInferiors = true;
							imapMailbox.HasChildren = new bool?(false);
						}
						resultData.Mailboxes.Add(imapMailbox);
						flag = true;
					}
					else
					{
						resultData.FailureException = ImapResponse.Fail("Invalid List response. Could not parse it.", command, currentLine);
						this.parseContext.Error = true;
					}
				}
			}
			return flag;
		}

		private bool AppendResponseProcessor(ImapCommand command, ImapResultData resultData)
		{
			bool flag = this.SimpleResponseProcessor(command, resultData);
			if (!flag && !this.parseContext.Error)
			{
				string currentLine = this.parseContext.CurrentLine;
				Match match;
				if (currentLine.StartsWith(command.CommandId, StringComparison.OrdinalIgnoreCase) && ImapResponse.CheckResponse(currentLine, ImapResponse.AppendUidResponse, out match))
				{
					uint num;
					this.parseContext.Error = !ImapResponse.SafeConvert(match, 2, out num);
					if (!this.parseContext.Error)
					{
						resultData.MessageUids.Add(num.ToString());
						this.log.Debug("AppendResponseProcessor parsed uidMessage {0} from APPENDUID response", new object[]
						{
							num
						});
					}
					else
					{
						this.log.Info("Invalid APPENDUID uidMessage {0}. Ignoring response.", new object[]
						{
							num
						});
					}
					flag = true;
				}
			}
			return flag;
		}

		private ImapResponse.TryProcessLine GetLineProcessor(ImapCommand command)
		{
			ImapCommandType commandType = command.CommandType;
			ImapResponse.TryProcessLine result;
			switch (commandType)
			{
			case ImapCommandType.None:
			case ImapCommandType.Noop:
			case ImapCommandType.CreateMailbox:
			case ImapCommandType.DeleteMailbox:
			case ImapCommandType.RenameMailbox:
			case ImapCommandType.Store:
			case ImapCommandType.Expunge:
			case ImapCommandType.Id:
			case ImapCommandType.Starttls:
			case ImapCommandType.Authenticate:
				result = new ImapResponse.TryProcessLine(this.SimpleResponseProcessor);
				break;
			case ImapCommandType.Login:
			case ImapCommandType.Capability:
				result = new ImapResponse.TryProcessLine(this.CapabilityResponseProcessor);
				break;
			case ImapCommandType.Logout:
				result = new ImapResponse.TryProcessLine(this.LogoutResponseProcessor);
				break;
			case ImapCommandType.Select:
				result = new ImapResponse.TryProcessLine(this.SelectResponseProcessor);
				break;
			case ImapCommandType.Fetch:
			{
				IList<string> list = (IList<string>)command.CommandParameters[3];
				this.fetchMessageIds = list.Contains("BODY.PEEK[HEADER.FIELDS (Message-ID)]");
				this.fetchMessageSizes = list.Contains("RFC822.SIZE");
				this.fetchMessageInternalDates = list.Contains("INTERNALDATE");
				result = new ImapResponse.TryProcessLine(this.FetchResponseProcessor);
				break;
			}
			case ImapCommandType.Append:
				result = new ImapResponse.TryProcessLine(this.AppendResponseProcessor);
				break;
			case ImapCommandType.Search:
				result = new ImapResponse.TryProcessLine(this.SearchResponseProcessor);
				break;
			case ImapCommandType.List:
				result = new ImapResponse.TryProcessLine(this.ListResponseProcessor);
				break;
			case ImapCommandType.Status:
				result = new ImapResponse.TryProcessLine(this.StatusResponseProcessor);
				break;
			default:
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unknown command type {0}, no line processor.", new object[]
				{
					commandType
				}));
			}
			return result;
		}

		private void ProcessFetch(ImapCommand command, Match match, ImapResultData resultData)
		{
			string text;
			string text2;
			if (ImapResponse.SafeGet(match, 1, out text) && ImapResponse.SafeGet(match, 2, out text2))
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
										resultData.FailureException = ImapResponse.Fail("Read past end of response", command, cachedBuilder.ToString());
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
							resultData.FailureException = ImapResponse.Fail("Invalid FETCH data, cannot parse the sequence number.", command, text2.ToString());
							this.parseContext.Error = true;
						}
					}
					else
					{
						this.log.Error("Invalid FETCH data, already processed a line with the same message sequence number: {0}, command: {1}, fetchContents: {2}", new object[]
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
					resultData.FailureException = ImapResponse.Fail("Missing FETCH data", command, text2);
				}
			}
			else
			{
				this.parseContext.Error = true;
				resultData.FailureException = ImapResponse.Fail("Parse error", command, match.ToString());
			}
			if (resultData.FailureException != null)
			{
				this.log.Error("Error encountered in ProcessFetch: {0}", new object[]
				{
					resultData.FailureException
				});
			}
		}

		private void ProcessFetchResults(ImapCommand command, string fetchResults, ImapResultData resultData)
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
						if (ImapResponse.AllowWhitespaceInKey(fetchResults, i, text2))
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
				resultData.FailureException = ImapResponse.Fail("Unbalanced FETCH response", command, fetchResults);
			}
			if (this.fetchMessageIds && resultData.MessageIds.Count == count)
			{
				this.log.Debug("No MessageId found in this line of the FETCH result: {0}", new object[]
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
				this.log.Error("No UID element found (which is mandatory) in this line of the FETCH result: {0}", new object[]
				{
					stringBuilder.ToString()
				});
				resultData.MessageUids.Add(null);
			}
			if (resultData.MessageFlags.Count == count)
			{
				resultData.MessageFlags.Add(ImapMailFlags.None);
			}
			if (this.fetchMessageInternalDates && resultData.MessageInternalDates.Count == count)
			{
				this.log.Debug("No INTERNALDATE found in this line of the FETCH result: {0}", new object[]
				{
					stringBuilder.ToString()
				});
				resultData.MessageInternalDates.Add(null);
			}
		}

		private void ProcessFetchKeyValueToResult(ImapCommand command, ImapResultData resultData, string key, string value)
		{
			IList<string> list = (IList<string>)command.CommandParameters[3];
			if (list != null && (list.Contains(key) || ImapResponse.HeaderFetch.IsMatch(key) || ImapResponse.BodyKeyRegex.IsMatch(key)))
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
						resultData.FailureException = ImapResponse.Fail("Could not parse message size.", command, key);
						this.parseContext.Error = true;
					}
					resultData.MessageSizes.Add(item);
					return;
				}
				if ("FLAGS".Equals(key, StringComparison.OrdinalIgnoreCase))
				{
					resultData.MessageFlags.Add(ImapUtilities.ConvertStringFormToFlags(value));
					return;
				}
				if ("INTERNALDATE".Equals(key, StringComparison.OrdinalIgnoreCase))
				{
					DateTime dateTime = MailUtilities.ToDateTime(value);
					resultData.MessageInternalDates.Add(new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, dateTime)));
					return;
				}
				Match match;
				if (ImapResponse.BodyKeyRegex.IsMatch(key))
				{
					if (this.parseContext.MoveNextLiterals())
					{
						resultData.MessageStream = this.parseContext.CurrentLiteral;
						return;
					}
					resultData.FailureException = ImapResponse.Fail("More literal references than literals", command, key);
					this.parseContext.Error = true;
					return;
				}
				else if (ImapResponse.CheckResponse(key, ImapResponse.HeaderFetch, out match))
				{
					this.ProcessFetchHeaderDataToResult(command, resultData, key);
					return;
				}
			}
			else
			{
				this.log.Debug("Unexpected token while processing FETCH data: {0} = {1}", new object[]
				{
					key,
					value
				});
			}
		}

		private void ProcessFetchHeaderDataToResult(ImapCommand command, ImapResultData resultData, string key)
		{
			if (!this.parseContext.MoveNextLiterals() || this.parseContext.CurrentLiteral == null)
			{
				this.log.Debug("No message id literal for command: {0}, key: {1}", new object[]
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
						this.log.Debug("Ignoring header field: {0}: {1}", new object[]
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
					this.log.Debug("Incoming string {0} converted to {1}", new object[]
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

		private readonly List<Stream> responseLiterals;

		private readonly BufferBuilder responseBuffer;

		private readonly List<string> responseLines;

		private readonly ImapResponse.ParseContext parseContext;

		private readonly ILog log;

		private string commandId;

		private byte lastByteRead;

		private Stream inFlightLiteral;

		private bool checkForUnsolicitedByeResponse;

		private int curLiteralBytes;

		private bool fetchMessageIds;

		private bool fetchMessageInternalDates;

		private bool fetchMessageSizes;

		private delegate bool TryProcessLine(ImapCommand command, ImapResultData resultData);

		private sealed class ParseContext
		{
			internal bool Error { get; set; }

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

			internal void Reset(IEnumerable<string> responseLines, IEnumerable<Stream> responseLiterals)
			{
				this.lineItr = responseLines.GetEnumerator();
				this.literalItr = responseLiterals.GetEnumerator();
				this.Error = false;
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

			private readonly StringBuilder cachedBuilder = new StringBuilder();

			private IEnumerator<string> lineItr;

			private IEnumerator<Stream> literalItr;
		}
	}
}
