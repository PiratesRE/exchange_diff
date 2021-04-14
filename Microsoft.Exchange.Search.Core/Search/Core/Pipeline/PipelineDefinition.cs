using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	[XmlRoot(ElementName = "Pipeline")]
	[Serializable]
	public sealed class PipelineDefinition
	{
		[XmlElement]
		public string Name { get; set; }

		[XmlElement]
		public int MaxConcurrency { get; set; }

		[XmlElement]
		public int PoisonComponentThreshold { get; set; }

		[XmlArrayItem(ElementName = "Component")]
		[XmlArray]
		public PipelineComponentDefinition[] Components { get; set; }

		internal static PipelineDefinition LoadFrom(string definition)
		{
			PipelineDefinition.diagnosticsSession.TraceDebug<string>("Loading an instance of pipeline definition from string: {0}", definition);
			PipelineDefinition result;
			using (TextReader textReader = new StringReader(definition))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(PipelineDefinition));
				result = (PipelineDefinition)xmlSerializer.Deserialize(textReader);
			}
			return result;
		}

		internal static PipelineDefinition LoadFromFile(string filepath)
		{
			PipelineDefinition.diagnosticsSession.TraceDebug<string>("Loading an instance of pipeline definition from file: {0}", filepath);
			PipelineDefinition result;
			using (XmlReader xmlReader = new XmlTextReader(filepath))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(PipelineDefinition));
				result = (PipelineDefinition)xmlSerializer.Deserialize(xmlReader);
			}
			return result;
		}

		private static readonly IDiagnosticsSession diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("PipelineDefinition", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.PipelineLoaderTracer, 0L);
	}
}
