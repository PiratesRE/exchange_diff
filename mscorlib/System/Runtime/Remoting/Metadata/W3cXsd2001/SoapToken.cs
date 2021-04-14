using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapToken : ISoapXsd
	{
		public static string XsdType
		{
			get
			{
				return "token";
			}
		}

		public string GetXsdType()
		{
			return SoapToken.XsdType;
		}

		public SoapToken()
		{
		}

		public SoapToken(string value)
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

		public static SoapToken Parse(string value)
		{
			return new SoapToken(value);
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
				'\t'
			};
			int num = value.LastIndexOfAny(anyOf);
			if (num > -1)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_SOAPInteropxsdInvalid", new object[]
				{
					"xsd:token",
					value
				}));
			}
			if (value.Length > 0 && (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[value.Length - 1])))
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_SOAPInteropxsdInvalid", new object[]
				{
					"xsd:token",
					value
				}));
			}
			num = value.IndexOf("  ");
			if (num > -1)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_SOAPInteropxsdInvalid", new object[]
				{
					"xsd:token",
					value
				}));
			}
			return value;
		}

		private string _value;
	}
}
