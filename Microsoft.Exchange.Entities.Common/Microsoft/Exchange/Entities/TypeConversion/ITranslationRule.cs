using System;

namespace Microsoft.Exchange.Entities.TypeConversion
{
	public interface ITranslationRule<in TLeft, in TRight>
	{
		void FromLeftToRightType(TLeft left, TRight right);

		void FromRightToLeftType(TLeft left, TRight right);
	}
}
