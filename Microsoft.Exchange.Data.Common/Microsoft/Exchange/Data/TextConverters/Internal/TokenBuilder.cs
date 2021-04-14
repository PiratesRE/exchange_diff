using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Exchange.Data.TextConverters.Internal
{
	internal abstract class TokenBuilder
	{
		public TokenBuilder(Token token, char[] buffer, int maxRuns, bool testBoundaryConditions)
		{
			int num = 64;
			if (!testBoundaryConditions)
			{
				this.maxRuns = maxRuns;
			}
			else
			{
				this.maxRuns = 55;
				num = 7;
			}
			this.token = token;
			this.token.Buffer = buffer;
			this.token.RunList = new Token.RunEntry[num];
		}

		public Token Token
		{
			get
			{
				return this.token;
			}
		}

		public bool IsStarted
		{
			get
			{
				return this.state != 0;
			}
		}

		public bool Valid
		{
			get
			{
				return this.tokenValid;
			}
		}

		public int TotalLength
		{
			get
			{
				return this.tailOffset - this.token.Whole.HeadOffset;
			}
		}

		public void BufferChanged(char[] newBuffer, int newBase)
		{
			if (newBuffer != this.token.Buffer || newBase != this.token.Whole.HeadOffset)
			{
				this.token.Buffer = newBuffer;
				if (newBase != this.token.Whole.HeadOffset)
				{
					int deltaOffset = newBase - this.token.Whole.HeadOffset;
					this.Rebase(deltaOffset);
				}
			}
		}

		public virtual void Reset()
		{
			if (this.state > 0)
			{
				this.token.Reset();
				this.tailOffset = 0;
				this.tokenValid = false;
				this.state = 0;
			}
		}

		public TokenId MakeEmptyToken(TokenId tokenId)
		{
			this.token.TokenId = tokenId;
			this.state = 5;
			this.tokenValid = true;
			return tokenId;
		}

		public TokenId MakeEmptyToken(TokenId tokenId, int argument)
		{
			this.token.TokenId = tokenId;
			this.token.Argument = argument;
			this.state = 5;
			this.tokenValid = true;
			return tokenId;
		}

		public TokenId MakeEmptyToken(TokenId tokenId, Encoding tokenEncoding)
		{
			this.token.TokenId = tokenId;
			this.token.TokenEncoding = tokenEncoding;
			this.state = 5;
			this.tokenValid = true;
			return tokenId;
		}

		public void StartText(int baseOffset)
		{
			this.token.TokenId = TokenId.Text;
			this.state = 10;
			this.token.Whole.HeadOffset = baseOffset;
			this.tailOffset = baseOffset;
		}

		public void EndText()
		{
			this.state = 5;
			this.tokenValid = true;
			this.token.WholePosition.Rewind(this.token.Whole);
			this.AddSentinelRun();
		}

		public void SkipRunIfNecessary(int start, uint skippedRunKind)
		{
			if (start != this.tailOffset)
			{
				this.AddInvalidRun(start, skippedRunKind);
			}
		}

		public bool PrepareToAddMoreRuns(int numRuns, int start, uint skippedRunKind)
		{
			return (start == this.tailOffset && this.token.Whole.Tail + numRuns < this.token.RunList.Length) || this.SlowPrepareToAddMoreRuns(numRuns, start, skippedRunKind);
		}

		public bool SlowPrepareToAddMoreRuns(int numRuns, int start, uint skippedRunKind)
		{
			if (start != this.tailOffset)
			{
				numRuns++;
			}
			if (this.token.Whole.Tail + numRuns < this.token.RunList.Length || this.ExpandRunsArray(numRuns))
			{
				if (start != this.tailOffset)
				{
					this.AddInvalidRun(start, skippedRunKind);
				}
				return true;
			}
			return false;
		}

		public bool PrepareToAddMoreRuns(int numRuns)
		{
			return this.token.Whole.Tail + numRuns < this.token.RunList.Length || this.ExpandRunsArray(numRuns);
		}

		[Conditional("DEBUG")]
		public void AssertPreparedToAddMoreRuns(int numRuns)
		{
		}

		[Conditional("DEBUG")]
		public void AssertCanAddMoreRuns(int numRuns)
		{
		}

		[Conditional("DEBUG")]
		public void AssertCurrentRunPosition(int position)
		{
		}

		[Conditional("DEBUG")]
		public void DebugPrepareToAddMoreRuns(int numRuns)
		{
		}

		public void AddTextRun(RunTextType textType, int start, int end)
		{
			this.AddRun((RunType)2147483648U, textType, 67108864U, start, end, 0);
		}

		public void AddLiteralTextRun(RunTextType textType, int start, int end, int literal)
		{
			this.AddRun((RunType)3221225472U, textType, 67108864U, start, end, literal);
		}

		public void AddSpecialRun(RunKind kind, int startEnd, int value)
		{
			this.AddRun(RunType.Special, RunTextType.Unknown, (uint)kind, this.tailOffset, startEnd, value);
		}

		internal void AddRun(RunType type, RunTextType textType, uint kind, int start, int end, int value)
		{
			Token.RunEntry[] runList = this.token.RunList;
			Token token = this.token;
			int tail;
			token.Whole.Tail = (tail = token.Whole.Tail) + 1;
			runList[tail].Initialize(type, textType, kind, end - start, value);
			this.tailOffset = end;
		}

		internal void AddInvalidRun(int offset, uint kind)
		{
			Token.RunEntry[] runList = this.token.RunList;
			Token token = this.token;
			int tail;
			token.Whole.Tail = (tail = token.Whole.Tail) + 1;
			runList[tail].Initialize(RunType.Invalid, RunTextType.Unknown, kind, offset - this.tailOffset, 0);
			this.tailOffset = offset;
		}

		internal void AddNullRun(uint kind)
		{
			Token.RunEntry[] runList = this.token.RunList;
			Token token = this.token;
			int tail;
			token.Whole.Tail = (tail = token.Whole.Tail) + 1;
			runList[tail].Initialize(RunType.Invalid, RunTextType.Unknown, kind, 0, 0);
		}

		internal void AddSentinelRun()
		{
			this.token.RunList[this.token.Whole.Tail].InitializeSentinel();
		}

		protected virtual void Rebase(int deltaOffset)
		{
			Token token = this.token;
			token.Whole.HeadOffset = token.Whole.HeadOffset + deltaOffset;
			Token token2 = this.token;
			token2.WholePosition.RunOffset = token2.WholePosition.RunOffset + deltaOffset;
			this.tailOffset += deltaOffset;
		}

		private bool ExpandRunsArray(int numRuns)
		{
			int num = Math.Min(this.maxRuns, Math.Max(this.token.RunList.Length * 2, this.token.Whole.Tail + numRuns + 1));
			if (num - this.token.Whole.Tail < numRuns + 1)
			{
				return false;
			}
			Token.RunEntry[] array = new Token.RunEntry[num];
			Array.Copy(this.token.RunList, 0, array, 0, this.token.Whole.Tail);
			this.token.RunList = array;
			return true;
		}

		protected const byte BuildStateInitialized = 0;

		protected const byte BuildStateEnded = 5;

		protected const byte FirstStarted = 10;

		protected const byte BuildStateText = 10;

		protected byte state;

		protected Token token;

		protected int maxRuns;

		protected int tailOffset;

		protected bool tokenValid;
	}
}
