using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal abstract class BaseFilterConverter<TInput, TOutput>
	{
		protected TOutput InternalConvert(TInput inputFilter)
		{
			StandbyStackEntry<TInput, TOutput> standbyStackEntry = new StandbyStackEntry<TInput, TOutput>(inputFilter);
			TOutput toutput = default(TOutput);
			int i = 0;
			while (i <= 255)
			{
				if (this.GetFilterChild(standbyStackEntry.Filter, standbyStackEntry.CurrentChild) == null)
				{
					toutput = this.ConvertSingleElement(standbyStackEntry.Filter, standbyStackEntry.WorkingStack);
					if (this.standbyStack.Count == 0)
					{
						return toutput;
					}
					standbyStackEntry = this.standbyStack.Pop();
					standbyStackEntry.WorkingStack.Push(toutput);
					standbyStackEntry.CurrentChild++;
					i++;
				}
				TInput filterChild = this.GetFilterChild(standbyStackEntry.Filter, standbyStackEntry.CurrentChild);
				if (filterChild != null)
				{
					this.standbyStack.Push(standbyStackEntry);
					standbyStackEntry = new StandbyStackEntry<TInput, TOutput>(filterChild);
				}
			}
			this.ThrowTooLongException();
			return default(TOutput);
		}

		protected abstract bool IsLeafExpression(TInput inputFilter);

		protected abstract int GetFilterChildCount(TInput parentFilter);

		protected abstract TInput GetFilterChild(TInput parentFilter, int childIndex);

		protected abstract void ThrowTooLongException();

		protected abstract TOutput ConvertSingleElement(TInput inputFilter, Stack<TOutput> workingStack);

		public const int MaxRestrictionNodeCount = 255;

		private Stack<StandbyStackEntry<TInput, TOutput>> standbyStack = new Stack<StandbyStackEntry<TInput, TOutput>>();
	}
}
