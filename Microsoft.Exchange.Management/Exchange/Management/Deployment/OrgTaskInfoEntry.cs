using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class OrgTaskInfoEntry : TaskInfoEntry
	{
		[XmlAttribute]
		public bool UseGlobalTask
		{
			get
			{
				return base.UsePrimaryTask;
			}
			set
			{
				base.UsePrimaryTask = value;
			}
		}

		[XmlAttribute]
		public bool UseForReconciliation { get; set; }

		[XmlAttribute]
		public bool RecipientOperation { get; set; }
	}
}
