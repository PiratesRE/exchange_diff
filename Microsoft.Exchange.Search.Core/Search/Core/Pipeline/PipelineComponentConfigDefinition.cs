using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	[Serializable]
	public sealed class PipelineComponentConfigDefinition
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string Value { get; set; }
	}
}
