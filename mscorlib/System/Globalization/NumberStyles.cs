using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum NumberStyles
	{
		[__DynamicallyInvokable]
		None = 0,
		[__DynamicallyInvokable]
		AllowLeadingWhite = 1,
		[__DynamicallyInvokable]
		AllowTrailingWhite = 2,
		[__DynamicallyInvokable]
		AllowLeadingSign = 4,
		[__DynamicallyInvokable]
		AllowTrailingSign = 8,
		[__DynamicallyInvokable]
		AllowParentheses = 16,
		[__DynamicallyInvokable]
		AllowDecimalPoint = 32,
		[__DynamicallyInvokable]
		AllowThousands = 64,
		[__DynamicallyInvokable]
		AllowExponent = 128,
		[__DynamicallyInvokable]
		AllowCurrencySymbol = 256,
		[__DynamicallyInvokable]
		AllowHexSpecifier = 512,
		[__DynamicallyInvokable]
		Integer = 7,
		[__DynamicallyInvokable]
		HexNumber = 515,
		[__DynamicallyInvokable]
		Number = 111,
		[__DynamicallyInvokable]
		Float = 167,
		[__DynamicallyInvokable]
		Currency = 383,
		[__DynamicallyInvokable]
		Any = 511
	}
}
