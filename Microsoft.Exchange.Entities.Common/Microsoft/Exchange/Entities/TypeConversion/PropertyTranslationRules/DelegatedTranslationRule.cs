using System;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules
{
	public class DelegatedTranslationRule<TLeft, TRight> : ITranslationRule<TLeft, TRight>
	{
		public DelegatedTranslationRule(Action<TLeft, TRight> leftToRightDelegate, Action<TLeft, TRight> rightToLeftDelegate)
		{
			this.leftToRightDelegate = leftToRightDelegate;
			this.rightToLeftDelegate = rightToLeftDelegate;
		}

		public static void NoOpDelegate(TLeft left, TRight right)
		{
		}

		public void FromLeftToRightType(TLeft left, TRight right)
		{
			this.leftToRightDelegate(left, right);
		}

		public void FromRightToLeftType(TLeft left, TRight right)
		{
			this.rightToLeftDelegate(left, right);
		}

		private readonly Action<TLeft, TRight> leftToRightDelegate;

		private readonly Action<TLeft, TRight> rightToLeftDelegate;
	}
}
