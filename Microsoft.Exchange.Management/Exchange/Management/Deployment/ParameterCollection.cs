using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[XmlRoot(ElementName = "Parameters")]
	public class ParameterCollection : List<Parameter>
	{
	}
}
