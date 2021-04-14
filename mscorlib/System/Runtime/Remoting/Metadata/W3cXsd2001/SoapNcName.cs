using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapNcName : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "NCName";
			}
		}

		public string GetXsdType()
		{
			return SoapNcName.XsdType;
		}

		public SoapNcName()
		{
		}

		public SoapNcName(string value)
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

		public static SoapNcName Parse(string value)
		{
			return new SoapNcName(value);
		}

		private string _value;
	}
}
