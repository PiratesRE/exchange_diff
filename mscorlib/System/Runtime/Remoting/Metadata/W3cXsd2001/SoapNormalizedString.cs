using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapNormalizedString : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "normalizedString";
			}
		}

		public string GetXsdType()
		{
			return SoapNormalizedString.XsdType;
		}

		public SoapNormalizedString()
		{
		}

		public SoapNormalizedString(string value)
		{
			this._value = this.Validate(value);
		}

		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = this.Validate(value);
			}
		}

		public override string ToString()
		{
			return SoapType.Escape(this._value);
		}

		public static SoapNormalizedString Parse(string value)
		{
			return new SoapNormalizedString(value);
		}

		private string Validate(string value)
		{
			if (value == null || value.Length == 0)
			{
				return value;
			}
			char[] anyOf = new char[]
			{
				'\r',
				'\n',
				'\t'
			};
			int num = value.LastIndexOfAny(anyOf);
			if (num > -1)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_SOAPInteropxsdInvalid", new object[]
				{
					"xsd:normalizedString",
					value
				}));
			}
			return value;
		}

		private string _value;
	}
}
