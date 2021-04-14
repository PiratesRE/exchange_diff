using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class ServicePlanTaskInfo : OrgTaskInfo
	{
		[XmlAttribute]
		public string FeatureName { get; set; }
	}
}
