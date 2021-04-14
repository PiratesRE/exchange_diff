using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapMonthDay : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "gMonthDay";
			}
		}

		public string GetXsdType()
		{
			return SoapMonthDay.XsdType;
		}

		public SoapMonthDay()
		{
		}

		public SoapMonthDay(DateTime value)
		{
			this._value = value;
		}

		public DateTime Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		public override string ToString()
		{
			return this._value.ToString("'--'MM'-'dd", CultureInfo.InvariantCulture);
		}

		public static SoapMonthDay Parse(string value)
		{
			return new SoapMonthDay(DateTime.ParseExact(value, SoapMonthDay.formats, CultureInfo.InvariantCulture, DateTimeStyles.None));
		}

		private DateTime _value = DateTime.MinValue;

		private static string[] formats = new string[]
		{
			"--MM-dd",
			"--MM-ddzzz"
		};
	}
}
