using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System
{
	[Serializable]
	internal struct Currency
	{
		public Currency(decimal value)
		{
			this.m_value = decimal.ToCurrency(value).m_value;
		}

		internal Currency(long value, int ignored)
		{
			this.m_value = value;
		}

		public static Currency FromOACurrency(long cy)
		{
			return new Currency(cy, 0);
		}

		public long ToOACurrency()
		{
			return this.m_value;
		}

		[SecuritySafeCritical]
		public static decimal ToDecimal(Currency c)
		{
			decimal result = 0m;
			Currency.FCallToDecimal(ref result, c);
			return result;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FCallToDecimal(ref decimal result, Currency c);

		internal long m_value;
	}
}
