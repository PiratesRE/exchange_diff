using System;
using System.Text;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
	internal class TextTokenBuilder : TokenBuilder
	{
		public TextTokenBuilder(char[] buffer, int maxRuns, bool testBoundaryConditions) : this(new TextToken(), buffer, maxRuns, testBoundaryConditions)
		{
		}

		public TextTokenBuilder(TextToken token, char[] buffer, int maxRuns, bool testBoundaryConditions) : base(token, buffer, maxRuns, testBoundaryConditions)
		{
		}

		public new TextToken Token
		{
			get
			{
				return (TextToken)base.Token;
			}
		}

		public TextTokenId MakeEmptyToken(TextTokenId tokenId)
		{
			return (TextTokenId)base.MakeEmptyToken((TokenId)tokenId);
		}

		public TextTokenId MakeEmptyToken(TextTokenId tokenId, int argument)
		{
			return (TextTokenId)base.MakeEmptyToken((TokenId)tokenId, argument);
		}

		public TextTokenId MakeEmptyToken(TextTokenId tokenId, Encoding encoding)
		{
			return (TextTokenId)base.MakeEmptyToken((TokenId)tokenId, encoding);
		}

		public void SkipRunIfNecessary(int start, RunKind skippedRunKind)
		{
			base.SkipRunIfNecessary(start, (uint)skippedRunKind);
		}

		public bool PrepareToAddMoreRuns(int numRuns, int start, RunKind skippedRunKind)
		{
			return base.PrepareToAddMoreRuns(numRuns, start, (uint)skippedRunKind);
		}

		public void AddSpecialRun(TextRunKind kind, int startEnd, int value)
		{
			base.AddRun(RunType.Special, RunTextType.Unknown, (uint)kind, this.tailOffset, startEnd, value);
		}
	}
}
