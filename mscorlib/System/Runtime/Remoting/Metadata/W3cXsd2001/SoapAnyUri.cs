using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapAnyUri : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "anyURI";
			}
		}

		public string GetXsdType()
		{
			return SoapAnyUri.XsdType;
		}

		public SoapAnyUri()
		{
		}

		public SoapAnyUri(string value)
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
			return this._value;
		}

		public static SoapAnyUri Parse(string value)
		{
			return new SoapAnyUri(value);
		}

		private string _value;
	}
}
