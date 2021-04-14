using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapTime : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "time";
			}
		}

		public string GetXsdType()
		{
			return SoapTime.XsdType;
		}

		public SoapTime()
		{
		}

		public SoapTime(DateTime value)
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
				this._value = new DateTime(1, 1, 1, value.Hour, value.Minute, value.Second, value.Millisecond);
			}
		}

		public override string ToString()
		{
			return this._value.ToString("HH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);
		}

		public static SoapTime Parse(string value)
		{
			string s = value;
			if (value.EndsWith("Z", StringComparison.Ordinal))
			{
				s = value.Substring(0, value.Length - 1) + "-00:00";
			}
			return new SoapTime(DateTime.ParseExact(s, SoapTime.formats, CultureInfo.InvariantCulture, DateTimeStyles.None));
		}

		private DateTime _value = DateTime.MinValue;

		private static string[] formats = new string[]
		{
			"HH:mm:ss.fffffffzzz",
			"HH:mm:ss.ffff",
			"HH:mm:ss.ffffzzz",
			"HH:mm:ss.fff",
			"HH:mm:ss.fffzzz",
			"HH:mm:ss.ff",
			"HH:mm:ss.ffzzz",
			"HH:mm:ss.f",
			"HH:mm:ss.fzzz",
			"HH:mm:ss",
			"HH:mm:sszzz",
			"HH:mm:ss.fffff",
			"HH:mm:ss.fffffzzz",
			"HH:mm:ss.ffffff",
			"HH:mm:ss.ffffffzzz",
			"HH:mm:ss.fffffff",
			"HH:mm:ss.ffffffff",
			"HH:mm:ss.ffffffffzzz",
			"HH:mm:ss.fffffffff",
			"HH:mm:ss.fffffffffzzz",
			"HH:mm:ss.fffffffff",
			"HH:mm:ss.fffffffffzzz"
		};
	}
}
