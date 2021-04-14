using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapEntity : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "ENTITY";
			}
		}

		public string GetXsdType()
		{
			return SoapEntity.XsdType;
		}

		public SoapEntity()
		{
		}

		public SoapEntity(string value)
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

		public static SoapEntity Parse(string value)
		{
			return new SoapEntity(value);
		}

		private string _value;
	}
}
