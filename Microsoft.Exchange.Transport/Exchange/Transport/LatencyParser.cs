using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class LatencyParser
	{
		protected LatencyParser(Trace tracer)
		{
			this.tracer = tracer;
		}

		public string StringToParse
		{
			get
			{
				return this.stringToParse;
			}
		}

		protected Trace Tracer
		{
			get
			{
				return this.tracer;
			}
		}

		protected static int SkipWhitespaces(string s, int startIndex, int count)
		{
			int num = startIndex + count;
			for (int i = startIndex; i < num; i++)
			{
				if (!char.IsWhiteSpace(s[i]))
				{
					return i;
				}
			}
			return -1;
		}

		protected static bool TryParseLatency(string s, int startIndex, int count, out ushort latencySeconds)
		{
			string s2 = s.Substring(startIndex, count);
			if (!ushort.TryParse(s2, out latencySeconds) || (double)latencySeconds > TransportAppConfig.LatencyTrackerConfig.MaxLatency.TotalSeconds)
			{
				latencySeconds = ushort.MaxValue;
				return false;
			}
			return true;
		}

		protected static bool TryParseLatency(string s, int startIndex, int count, out float latencySeconds)
		{
			string s2 = s.Substring(startIndex, count);
			if (!float.TryParse(s2, out latencySeconds) || (double)latencySeconds > TransportAppConfig.LatencyTrackerConfig.MaxLatency.TotalSeconds)
			{
				latencySeconds = 65535f;
				return false;
			}
			return true;
		}

		protected static bool TryParseDateTime(string s, int startIndex, int count, out DateTime dt)
		{
			string s2 = s.Substring(startIndex, count);
			DateTimeStyles style = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal;
			return DateTime.TryParseExact(s2, "yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo, style, out dt);
		}

		protected bool TryParse(string s, int startIndex, int count)
		{
			this.state = LatencyParser.State.Server;
			this.latencyType = null;
			this.componentNameStart = (this.componentNameLength = -1);
			this.seenTotal = (this.isTotal = false);
			this.stringToParse = s;
			while (this.TryGetNextToken(startIndex, count))
			{
				if (this.tokenLength == 0)
				{
					this.Tracer.TraceError<LatencyParser.State, int, string>(0L, "Latency Parser (State={0}): Consecutive separators at position {1} while extracting a token in string '{2}'", this.state, this.tokenStart, s);
					return false;
				}
				switch (this.state)
				{
				case LatencyParser.State.Component:
					if (!this.ProcessComponentToken())
					{
						return false;
					}
					break;
				case LatencyParser.State.ComponentValue:
					if (!this.ProcessComponentValueToken())
					{
						return false;
					}
					break;
				case LatencyParser.State.ComponentOrPendingComponent:
				{
					char c = this.separator;
					if (c != '\0')
					{
						switch (c)
						{
						case ';':
							goto IL_CC;
						case '<':
							break;
						case '=':
							if (!this.ProcessComponentToken())
							{
								return false;
							}
							goto IL_13E;
						default:
							if (c == '|')
							{
								goto IL_CC;
							}
							break;
						}
						return this.ProcessUnexpectedSeparator();
					}
					IL_CC:
					if (!this.ProcessPendingComponentToken())
					{
						return false;
					}
					break;
				}
				case LatencyParser.State.End:
					throw new InvalidOperationException("Latency Parser: lingering End state in the state machine; string: " + s);
				case LatencyParser.State.PendingComponent:
					if (!this.ProcessPendingComponentToken())
					{
						return false;
					}
					break;
				case LatencyParser.State.PendingComponentOrServer:
					if (this.separator == '=')
					{
						if (!this.ProcessServerToken())
						{
							return false;
						}
					}
					else if (!this.ProcessPendingComponentToken())
					{
						return false;
					}
					break;
				case LatencyParser.State.Server:
					if (!this.ProcessServerToken())
					{
						return false;
					}
					break;
				case LatencyParser.State.ServerFqdn:
					if (!this.ProcessServerFqdnToken())
					{
						return false;
					}
					break;
				}
				IL_13E:
				if (this.separator == '\0')
				{
					break;
				}
				count -= this.tokenStart + this.tokenLength + 1 - startIndex;
				startIndex = this.tokenStart + this.tokenLength + 1;
				if (count <= 0)
				{
					break;
				}
			}
			if (this.state != LatencyParser.State.End)
			{
				this.Tracer.TraceError<string>(0L, "Latency Parser: unexpected end of string in '{0}'", s);
				return false;
			}
			return true;
		}

		protected virtual bool HandleLocalServerFqdn(string s, int startIndex, int count)
		{
			return true;
		}

		protected virtual bool HandleServerFqdn(string s, int startIndex, int count)
		{
			return true;
		}

		protected virtual bool HandleComponentLatency(string s, int componentNameStart, int componentNameLength, int latencyStart, int latencyLength)
		{
			return true;
		}

		protected virtual bool HandleTotalLatency(string s, int startIndex, int count)
		{
			return true;
		}

		protected virtual void HandleTotalComponent(string s, int startIndex, int count)
		{
		}

		protected virtual bool HandlePendingComponent(string s, int startIndex, int count)
		{
			return true;
		}

		private static bool SubstringEquals(string s1, int startIndex, int count, string s2)
		{
			return s2.Length == count && string.Compare(s1, startIndex, s2, 0, count, StringComparison.OrdinalIgnoreCase) == 0;
		}

		private static object TraceSeparator(char separator)
		{
			if (separator != '\0')
			{
				return separator;
			}
			return "<eos>";
		}

		private bool ProcessUnexpectedSeparator()
		{
			this.Tracer.TraceError(0L, "Latency Parser (State={0}): Unexpected separator '{1}' at position {2} in string '{3}'", new object[]
			{
				this.state,
				LatencyParser.TraceSeparator(this.separator),
				this.tokenStart + this.tokenLength,
				this.stringToParse
			});
			return false;
		}

		private bool ProcessUnexpectedToken(string tokenDescr)
		{
			this.Tracer.TraceError(0L, "Latency Parser (State={0}): Unexpected {1}token at position {2} in string '{3}'", new object[]
			{
				this.state,
				tokenDescr,
				this.tokenStart,
				this.stringToParse
			});
			return false;
		}

		private bool ProcessForcedExit()
		{
			this.Tracer.TraceError<LatencyParser.State>(0L, "Latency Parser (State={0}): forced exit", this.state);
			return false;
		}

		private bool ProcessServerToken()
		{
			if (this.separator != '=')
			{
				return this.ProcessUnexpectedSeparator();
			}
			if (LatencyParser.SubstringEquals(this.stringToParse, this.tokenStart, this.tokenLength, "SRV"))
			{
				if (this.latencyType != null && this.latencyType.Value == LatencyParser.LatencyType.LocalServer)
				{
					return this.ProcessUnexpectedToken("server ");
				}
				this.latencyType = new LatencyParser.LatencyType?(LatencyParser.LatencyType.EndToEnd);
			}
			else
			{
				if (!LatencyParser.SubstringEquals(this.stringToParse, this.tokenStart, this.tokenLength, "LSRV"))
				{
					return this.ProcessUnexpectedToken(string.Empty);
				}
				if (this.latencyType != null)
				{
					return this.ProcessUnexpectedToken("local server ");
				}
				this.latencyType = new LatencyParser.LatencyType?(LatencyParser.LatencyType.LocalServer);
			}
			this.seenTotal = false;
			this.state = LatencyParser.State.ServerFqdn;
			return true;
		}

		private bool ProcessServerFqdnToken()
		{
			if (this.separator != ':')
			{
				return this.ProcessUnexpectedSeparator();
			}
			bool flag;
			if (this.latencyType.Value == LatencyParser.LatencyType.LocalServer)
			{
				flag = this.HandleLocalServerFqdn(this.stringToParse, this.tokenStart, this.tokenLength);
			}
			else
			{
				flag = this.HandleServerFqdn(this.stringToParse, this.tokenStart, this.tokenLength);
			}
			if (!flag)
			{
				return this.ProcessForcedExit();
			}
			this.state = LatencyParser.State.ComponentOrPendingComponent;
			return true;
		}

		private bool ProcessComponentToken()
		{
			if (this.separator != '=')
			{
				return this.ProcessUnexpectedSeparator();
			}
			if (this.stringToParse.IndexOf("TOTAL", this.tokenStart, this.tokenLength, StringComparison.OrdinalIgnoreCase) == this.tokenStart)
			{
				if (this.seenTotal)
				{
					this.Tracer.TraceError<LatencyParser.State, string>(0L, "Latency Parser (State={0}): Multiple TOTALs for the same server in string '{1}'", this.state, this.stringToParse);
					return false;
				}
				this.seenTotal = (this.isTotal = true);
				this.HandleTotalComponent(this.stringToParse, this.tokenStart, this.tokenLength);
			}
			else
			{
				this.componentNameStart = this.tokenStart;
				this.componentNameLength = this.tokenLength;
			}
			this.state = LatencyParser.State.ComponentValue;
			return true;
		}

		private bool ProcessComponentValueToken()
		{
			bool flag;
			if (this.isTotal)
			{
				flag = this.HandleTotalLatency(this.stringToParse, this.tokenStart, this.tokenLength);
				this.isTotal = false;
			}
			else
			{
				flag = this.HandleComponentLatency(this.stringToParse, this.componentNameStart, this.componentNameLength, this.tokenStart, this.tokenLength);
				this.componentNameStart = (this.componentNameLength = -1);
			}
			if (!flag)
			{
				return this.ProcessForcedExit();
			}
			char c = this.separator;
			if (c <= '(')
			{
				if (c == '\0')
				{
					this.state = LatencyParser.State.End;
					return true;
				}
				if (c == '(')
				{
					this.state = LatencyParser.State.Component;
					return true;
				}
			}
			else
			{
				if (c == ';')
				{
					this.state = LatencyParser.State.PendingComponentOrServer;
					return true;
				}
				if (c == '|')
				{
					this.state = LatencyParser.State.Component;
					return true;
				}
			}
			return this.ProcessUnexpectedSeparator();
		}

		private bool ProcessPendingComponentToken()
		{
			if (!this.HandlePendingComponent(this.stringToParse, this.tokenStart, this.tokenLength))
			{
				return this.ProcessForcedExit();
			}
			char c = this.separator;
			if (c != '\0')
			{
				if (c != ';')
				{
					if (c != '|')
					{
						return this.ProcessUnexpectedSeparator();
					}
					this.state = LatencyParser.State.PendingComponent;
				}
				else
				{
					this.state = LatencyParser.State.Server;
				}
			}
			else
			{
				this.state = LatencyParser.State.End;
			}
			return true;
		}

		private bool TryGetNextToken(int startIndex, int count)
		{
			if (startIndex < 0 || startIndex >= this.stringToParse.Length)
			{
				string message = string.Format("Index out of range for string with length {0}", this.stringToParse.Length);
				throw new ArgumentOutOfRangeException("startIndex", startIndex, message);
			}
			if (count < 0 || startIndex + count > this.stringToParse.Length)
			{
				string message2 = string.Format("Count out of range for string with length {0} and startIndex {1}", this.stringToParse.Length, startIndex);
				throw new ArgumentOutOfRangeException("count", count, message2);
			}
			this.tokenStart = (this.tokenLength = -1);
			this.separator = '\0';
			this.tokenStart = LatencyParser.SkipWhitespaces(this.stringToParse, startIndex, count);
			if (this.tokenStart == -1)
			{
				return false;
			}
			this.tokenLength = 0;
			while (this.ShouldKeepLooking(startIndex, count))
			{
				this.tokenLength++;
			}
			if (this.tokenStart + this.tokenLength < startIndex + count)
			{
				this.separator = this.stringToParse[this.tokenStart + this.tokenLength];
			}
			return true;
		}

		private bool ShouldKeepLooking(int startIndex, int maxCharToExamine)
		{
			return this.tokenStart + this.tokenLength < startIndex + maxCharToExamine && !LatencyFormatter.IsSeparator(this.stringToParse[this.tokenStart + this.tokenLength]);
		}

		private const char EndOfStringSeparator = '\0';

		private readonly Trace tracer;

		private string stringToParse;

		private int tokenStart;

		private int tokenLength;

		private char separator;

		private LatencyParser.State state;

		private LatencyParser.LatencyType? latencyType;

		private int componentNameStart;

		private int componentNameLength;

		private bool seenTotal;

		private bool isTotal;

		private enum State
		{
			Component,
			ComponentValue,
			ComponentOrPendingComponent,
			End,
			PendingComponent,
			PendingComponentOrServer,
			Server,
			ServerFqdn
		}

		private enum LatencyType
		{
			LocalServer,
			EndToEnd
		}
	}
}
