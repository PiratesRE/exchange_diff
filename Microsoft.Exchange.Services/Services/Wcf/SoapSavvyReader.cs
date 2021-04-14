using System;
using System.Xml;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SoapSavvyReader : XmlReader
	{
		public SoapSavvyReader(XmlReader innerReader, SoapSavvyReader.OnSoapSectionChange soapSectionChangeHandler)
		{
			this.innerReader = innerReader;
			this.onSoapSectionChange = soapSectionChangeHandler;
		}

		public override int AttributeCount
		{
			get
			{
				return this.innerReader.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.innerReader.BaseURI;
			}
		}

		public override void Close()
		{
			this.innerReader.Close();
		}

		public override int Depth
		{
			get
			{
				return this.innerReader.Depth;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.innerReader.EOF;
			}
		}

		public override string GetAttribute(int i)
		{
			return this.innerReader.GetAttribute(i);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			return this.innerReader.GetAttribute(name, namespaceURI);
		}

		public override string GetAttribute(string name)
		{
			return this.innerReader.GetAttribute(name);
		}

		public override bool HasValue
		{
			get
			{
				return this.innerReader.HasValue;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.innerReader.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.innerReader.LocalName;
			}
		}

		public override string LookupNamespace(string prefix)
		{
			return this.innerReader.LookupNamespace(prefix);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return this.innerReader.MoveToAttribute(name, ns);
		}

		public override bool MoveToAttribute(string name)
		{
			return this.innerReader.MoveToAttribute(name);
		}

		public override bool MoveToElement()
		{
			return this.innerReader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			return this.innerReader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return this.innerReader.MoveToNextAttribute();
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.innerReader.NameTable;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.innerReader.NamespaceURI;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.innerReader.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.innerReader.Prefix;
			}
		}

		public override bool Read()
		{
			switch (this.soapSection)
			{
			case SoapSavvyReader.SoapSection.Unknown:
				if (this.LocalName == "Envelope" && this.IsSoapElement())
				{
					this.SetSoapSection(SoapSavvyReader.SoapSection.Envelope);
				}
				break;
			case SoapSavvyReader.SoapSection.Envelope:
				if (this.LocalName == "Header" && this.IsSoapElement())
				{
					this.SetSoapSection(SoapSavvyReader.SoapSection.Header);
				}
				else if (this.LocalName == "Body" && this.IsSoapElement())
				{
					this.SetSoapSection(SoapSavvyReader.SoapSection.Body);
				}
				break;
			case SoapSavvyReader.SoapSection.Header:
				if (this.LocalName == "Body" && this.IsSoapElement())
				{
					this.SetSoapSection(SoapSavvyReader.SoapSection.Body);
				}
				break;
			}
			return this.innerReader.Read();
		}

		private bool IsSoapElement()
		{
			return this.NamespaceURI.StartsWith("http://schemas.xmlsoap.org/soap/envelope/", StringComparison.OrdinalIgnoreCase) || this.NamespaceURI.StartsWith("http://www.w3.org/2003/05/soap-envelope", StringComparison.OrdinalIgnoreCase);
		}

		public override bool ReadAttributeValue()
		{
			return this.innerReader.ReadAttributeValue();
		}

		public override ReadState ReadState
		{
			get
			{
				return this.innerReader.ReadState;
			}
		}

		public override void ResolveEntity()
		{
			this.innerReader.ResolveEntity();
		}

		public override string Value
		{
			get
			{
				return this.innerReader.Value;
			}
		}

		internal SoapSavvyReader.SoapSection Section
		{
			get
			{
				return this.soapSection;
			}
		}

		private void SetSoapSection(SoapSavvyReader.SoapSection section)
		{
			this.soapSection = section;
			if (this.onSoapSectionChange != null)
			{
				this.onSoapSectionChange(this, this.soapSection);
			}
		}

		private const string EnvelopeElementName = "Envelope";

		private const string BodyElementName = "Body";

		private const string HeaderElementName = "Header";

		private XmlReader innerReader;

		private SoapSavvyReader.SoapSection soapSection;

		private SoapSavvyReader.OnSoapSectionChange onSoapSectionChange;

		internal delegate void OnSoapSectionChange(SoapSavvyReader reader, SoapSavvyReader.SoapSection section);

		internal enum SoapSection
		{
			Unknown,
			Envelope,
			Header,
			Body
		}
	}
}
