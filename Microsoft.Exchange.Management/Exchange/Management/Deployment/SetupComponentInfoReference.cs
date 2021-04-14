using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[XmlInclude(typeof(SetupComponentInfoReference))]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class SetupComponentInfoReference
	{
		public string RelativeFileLocation;
	}
}
