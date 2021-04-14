using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapMonth : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "gMonth";
			}
		}

		public string GetXsdType()
		{
			return SoapMonth.XsdType;
		}

		public SoapMonth()
		{
		}

		public SoapMonth(DateTime value)
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
			return this._value.ToString("--MM--", CultureInfo.InvariantCulture);
		}

		public static SoapMonth Parse(string value)
		{
			return new SoapMonth(DateTime.ParseExact(value, SoapMonth.formats, CultureInfo.InvariantCulture, DateTimeStyles.None));
		}

		private DateTime _value = DateTime.MinValue;

		private static string[] formats = new string[]
		{
			"--MM--",
			"--MM--zzz"
		};
	}
}
