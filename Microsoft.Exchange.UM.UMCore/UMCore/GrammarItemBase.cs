using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class GrammarItemBase : IEquatable<GrammarItemBase>
	{
		internal GrammarItemBase(float weight)
		{
			this.weight = Math.Max(1E-05f, (float)Math.Round((double)weight, 5));
		}

		internal float Weight
		{
			get
			{
				return this.weight;
			}
		}

		public abstract bool IsEmpty { get; }

		public abstract bool Equals(GrammarItemBase otherItemBase);

		public override bool Equals(object obj)
		{
			GrammarItemBase grammarItemBase = obj as GrammarItemBase;
			return grammarItemBase != null && this.Equals(grammarItemBase);
		}

		public override int GetHashCode()
		{
			return this.InternalGetHashCode();
		}

		public override string ToString()
		{
			if (1f == this.weight)
			{
				return string.Format(CultureInfo.InvariantCulture, "\r\n            <item >{0}\r\n            </item>", new object[]
				{
					this.GetInnerItem()
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "\r\n            <item weight='{0}'>{1}\r\n            </item>", new object[]
			{
				this.weight.ToString("N5", CultureInfo.InvariantCulture),
				this.GetInnerItem()
			});
		}

		protected abstract string GetInnerItem();

		protected abstract int InternalGetHashCode();

		private float weight = 1f;
	}
}
