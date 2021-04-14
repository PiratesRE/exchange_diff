using System;

namespace Microsoft.Exchange.Data.Globalization.Iso2022Jp
{
	internal enum EscapeState
	{
		Begin,
		Esc_1,
		Esc_Dollar_2,
		Esc_OpenParen_2,
		Esc_Ampersand_2,
		Esc_K_2,
		Esc_Dollar_OpenParen_3,
		Esc_Dollar_CloseParen_3,
		Esc_Ampersand_At_3,
		Esc_Ampersand_At_Esc_4,
		Esc_Ampersand_At_Esc_Dollar_5,
		Esc_Esc_Reset,
		Esc_SISO_Reset
	}
}
