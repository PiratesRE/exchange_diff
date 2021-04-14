using System;
using System.Xml;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal class SoapXmlMalformedException : WSTrustException
	{
		public SoapXmlMalformedException(XmlElement context) : base(WSTrustStrings.SoapXmlMalformedException)
		{
			this.context = context;
		}

		public SoapXmlMalformedException(XmlElement context, XmlNodeDefinition expectedNode) : base(WSTrustStrings.SoapXmlMalformedException)
		{
			this.context = context;
			this.expectedNode = expectedNode;
		}

		public XmlElement Context
		{
			get
			{
				return this.context;
			}
		}

		public XmlNodeDefinition ExpectedNode
		{
			get
			{
				return this.expectedNode;
			}
		}

		public override string ToString()
		{
			if (" Context=" + this.context == null)
			{
				return "<null>";
			}
			if (this.context.OuterXml + " ExpectedNode=" + this.expectedNode != null)
			{
				return this.expectedNode.ToString() + Environment.NewLine + base.ToString();
			}
			return "<null>";
		}

		private XmlElement context;

		private XmlNodeDefinition expectedNode;
	}
}
