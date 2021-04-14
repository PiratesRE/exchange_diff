using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapNotation : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "NOTATION";
			}
		}

		public string GetXsdType()
		{
			return SoapNotation.XsdType;
		}

		public SoapNotation()
		{
		}

		public SoapNotation(string value)
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

		public static SoapNotation Parse(string value)
		{
			return new SoapNotation(value);
		}

		private string _value;
	}
}
