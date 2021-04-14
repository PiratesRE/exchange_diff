using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapInteger : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "integer";
			}
		}

		public string GetXsdType()
		{
			return SoapInteger.XsdType;
		}

		public SoapInteger()
		{
		}

		public SoapInteger(decimal value)
		{
			this._value = decimal.Truncate(value);
		}

		public decimal Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = decimal.Truncate(value);
			}
		}

		public override string ToString()
		{
			return this._value.ToString(CultureInfo.InvariantCulture);
		}

		public static SoapInteger Parse(string value)
		{
			return new SoapInteger(decimal.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
		}

		private decimal _value;
	}
}
