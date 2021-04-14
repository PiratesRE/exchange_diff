using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class TaskSet : ExtractionSet<Task>
	{
		public TaskSet() : base(new TaskSerializer())
		{
		}

		public static implicit operator TaskSet(Task[] tasks)
		{
			return new TaskSet
			{
				Extractions = tasks
			};
		}

		public static XmlSerializer Serializer
		{
			get
			{
				return TaskSet.serializer;
			}
		}

		public override XmlSerializer GetSerializer()
		{
			return TaskSet.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(TaskSet));
	}
}
