using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class ServerTaskInfoEntry : TaskInfoEntry
	{
		[XmlAttribute]
		public bool UseStandaloneTask
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
	}
}
