using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	[Serializable]
	public sealed class PipelineComponentFactoryDefinition
	{
		[XmlAttribute(AttributeName = "Assembly")]
		public string AssemblyFullName { get; set; }

		[XmlText]
		public string TypeFullName { get; set; }
	}
}
