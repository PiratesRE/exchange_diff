using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapName : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "Name";
			}
		}

		public string GetXsdType()
		{
			return SoapName.XsdType;
		}

		public SoapName()
		{
		}

		public SoapName(string value)
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

		public static SoapName Parse(string value)
		{
			return new SoapName(value);
		}

		private string _value;
	}
}
