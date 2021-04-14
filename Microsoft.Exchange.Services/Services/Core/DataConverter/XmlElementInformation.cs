using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class XmlElementInformation
	{
		public XmlElementInformation(string localName, string xmlPath, string namespaceUri, ExchangeVersion effectiveVersion)
		{
			this.localName = localName;
			this.xmlPath = xmlPath;
			this.namespaceUri = namespaceUri;
			this.effectiveVersion = effectiveVersion;
		}

		public XmlElementInformation(string localName, string xmlPath, ExchangeVersion effectiveVersion) : this(localName, xmlPath, ServiceXml.DefaultNamespaceUri, effectiveVersion)
		{
		}

		public string NamespaceUri
		{
			get
			{
				return this.namespaceUri;
			}
		}

		public string LocalName
		{
			get
			{
				return this.localName;
			}
		}

		public string Path
		{
			get
			{
				return this.xmlPath;
			}
		}

		public ExchangeVersion EffectiveVersion
		{
			get
			{
				return this.effectiveVersion;
			}
		}

		public override string ToString()
		{
			return this.localName;
		}

		private string namespaceUri = string.Empty;

		private string localName = string.Empty;

		private string xmlPath = string.Empty;

		protected ExchangeVersion effectiveVersion;
	}
}
