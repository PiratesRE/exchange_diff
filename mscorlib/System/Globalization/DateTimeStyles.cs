using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum DateTimeStyles
	{
		[__DynamicallyInvokable]
		None = 0,
		[__DynamicallyInvokable]
		AllowLeadingWhite = 1,
		[__DynamicallyInvokable]
		AllowTrailingWhite = 2,
		[__DynamicallyInvokable]
		AllowInnerWhite = 4,
		[__DynamicallyInvokable]
		AllowWhiteSpaces = 7,
		[__DynamicallyInvokable]
		NoCurrentDateDefault = 8,
		[__DynamicallyInvokable]
		AdjustToUniversal = 16,
		[__DynamicallyInvokable]
		AssumeLocal = 32,
		[__DynamicallyInvokable]
		AssumeUniversal = 64,
		[__DynamicallyInvokable]
		RoundtripKind = 128
	}
}
