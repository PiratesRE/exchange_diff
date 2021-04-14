using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	internal sealed class CSharpFilter<T> : QueryFilter
	{
		internal CSharpFilter(CSharpFilter<T>.MatchDelegate matchDelegate)
		{
			if (matchDelegate == null)
			{
				throw new ArgumentNullException("matchDelegate");
			}
			this.matchDelegate = matchDelegate;
		}

		internal bool Match(T item)
		{
			return this.matchDelegate(item);
		}

		public override bool Equals(object obj)
		{
			CSharpFilter<T> csharpFilter = obj as CSharpFilter<T>;
			return csharpFilter != null && !(csharpFilter.GetType() != base.GetType()) && object.ReferenceEquals(this.matchDelegate, csharpFilter.matchDelegate);
		}

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode() ^ this.matchDelegate.GetHashCode();
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(CSharpFilter)");
		}

		private CSharpFilter<T>.MatchDelegate matchDelegate;

		internal delegate bool MatchDelegate(T item);
	}
}
