using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapDay : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "gDay";
			}
		}

		public string GetXsdType()
		{
			return SoapDay.XsdType;
		}

		public SoapDay()
		{
		}

		public SoapDay(DateTime value)
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
			return this._value.ToString("---dd", CultureInfo.InvariantCulture);
		}

		public static SoapDay Parse(string value)
		{
			return new SoapDay(DateTime.ParseExact(value, SoapDay.formats, CultureInfo.InvariantCulture, DateTimeStyles.None));
		}

		private DateTime _value = DateTime.MinValue;

		private static string[] formats = new string[]
		{
			"---dd",
			"---ddzzz"
		};
	}
}
