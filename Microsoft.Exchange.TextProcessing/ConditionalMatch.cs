using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class ConditionalMatch : IMatch
	{
		internal ConditionalMatch(IMatch match, IMatch precondition)
		{
			this.precondition = precondition;
			this.match = match;
		}

		internal ConditionalMatch(ConditionalMatch original, MatchFactory factory)
		{
			this.precondition = factory.Copy(original.precondition);
			this.match = factory.Copy(original.match);
		}

		public bool IsMatch(TextScanContext data)
		{
			return this.precondition.IsMatch(data) && this.match.IsMatch(data);
		}

		private IMatch match;

		private IMatch precondition;
	}
}
