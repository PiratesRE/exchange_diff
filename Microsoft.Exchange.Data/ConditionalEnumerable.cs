using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	internal sealed class ConditionalEnumerable<T> : IEnumerable<!0>, IEnumerable
	{
		public ConditionalEnumerable(IEnumerable<T> conditionalEnumerable, IEnumerable<T> secondEnumerable)
		{
			if (conditionalEnumerable == null)
			{
				throw new ArgumentNullException("conditionalEnumerable");
			}
			if (secondEnumerable == null)
			{
				throw new ArgumentNullException("secondEnumerable");
			}
			this.conditionalEnumerable = conditionalEnumerable;
			this.secondEnumerable = secondEnumerable;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ConditionalEnumerator<T>(this.conditionalEnumerable, this.secondEnumerable);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ConditionalEnumerator<T>(this.conditionalEnumerable, this.secondEnumerable);
		}

		private IEnumerable<T> conditionalEnumerable;

		private IEnumerable<T> secondEnumerable;
	}
}
