using System;
using System.Xml;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal class SoapFaultException : WSTrustException
	{
		public SoapFaultException(XmlElement fault, string code, string subCode) : base(WSTrustStrings.SoapFaultException)
		{
			this.fault = fault;
			this.code = code;
			this.subCode = subCode;
		}

		public XmlElement Fault
		{
			get
			{
				return this.fault;
			}
		}

		public string Code
		{
			get
			{
				return this.code;
			}
		}

		public string SubCode
		{
			get
			{
				return this.subCode;
			}
		}

		public override string ToString()
		{
			if (" Code=" + this.code == null)
			{
				return "<null>";
			}
			if (this.code + " SubCode=" + this.subCode == null)
			{
				return "<null>";
			}
			if (this.subCode + " Fault=" + this.fault != null)
			{
				return this.fault.OuterXml + Environment.NewLine + base.ToString();
			}
			return "<null>";
		}

		private XmlElement fault;

		private string code;

		private string subCode;
	}
}
