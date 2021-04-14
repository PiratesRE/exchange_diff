using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapIdref : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "IDREF";
			}
		}

		public string GetXsdType()
		{
			return SoapIdref.XsdType;
		}

		public SoapIdref()
		{
		}

		public SoapIdref(string value)
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

		public static SoapIdref Parse(string value)
		{
			return new SoapIdref(value);
		}

		private string _value;
	}
}
