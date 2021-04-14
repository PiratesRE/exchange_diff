using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapId : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "ID";
			}
		}

		public string GetXsdType()
		{
			return SoapId.XsdType;
		}

		public SoapId()
		{
		}

		public SoapId(string value)
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

		public static SoapId Parse(string value)
		{
			return new SoapId(value);
		}

		private string _value;
	}
}
