using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ContainerInformation : XmlElementInformation
	{
		public ContainerInformation(string localName, string path, string namespaceUri, ExchangeVersion effectiveVersion) : base(localName, path, namespaceUri, effectiveVersion)
		{
		}

		public ContainerInformation(string localName, string path, ExchangeVersion effectiveVersion) : this(localName, path, ServiceXml.DefaultNamespaceUri, effectiveVersion)
		{
		}
	}
}
