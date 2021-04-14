using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapIdrefs : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "IDREFS";
			}
		}

		public string GetXsdType()
		{
			return SoapIdrefs.XsdType;
		}

		public SoapIdrefs()
		{
		}

		public SoapIdrefs(string value)
		{
			this._value = value;
		}

		public string Value
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
			return SoapType.Escape(this._value);
		}

		public static SoapIdrefs Parse(string value)
		{
			return new SoapIdrefs(value);
		}

		private string _value;
	}
}
