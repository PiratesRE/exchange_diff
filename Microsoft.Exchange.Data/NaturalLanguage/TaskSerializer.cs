using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TaskSerializer : BaseSerializer<Task>
	{
		protected override XmlSerializer GetSerializer()
		{
			return TaskSerializer.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(Task[]), new XmlRootAttribute("Tasks"));
	}
}
