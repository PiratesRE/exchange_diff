using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapNmtoken : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "NMTOKEN";
			}
		}

		public string GetXsdType()
		{
			return SoapNmtoken.XsdType;
		}

		public SoapNmtoken()
		{
		}

		public SoapNmtoken(string value)
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

		public static SoapNmtoken Parse(string value)
		{
			return new SoapNmtoken(value);
		}

		private string _value;
	}
}
