using System;
using System.Xml;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal delegate void WriteStreamData(XmlWriter writer, object item, object buffer, object[] parameters);
}
