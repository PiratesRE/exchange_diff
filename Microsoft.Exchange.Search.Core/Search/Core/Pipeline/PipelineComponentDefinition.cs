using System;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	[Serializable]
	public sealed class PipelineComponentDefinition
	{
		public PipelineComponentDefinition()
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("PipelineComponentDefinition", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.PipelineLoaderTracer, (long)this.GetHashCode());
		}

		[XmlElement]
		public string Name { get; set; }

		[XmlElement]
		public int Order { get; set; }

		[XmlElement]
		public PipelineComponentFactoryDefinition Factory { get; set; }

		[XmlArray(IsNullable = false)]
		[XmlArrayItem(ElementName = "Configuration")]
		public PipelineComponentConfigDefinition[] Configurations { get; set; }

		[XmlElement(IsNullable = false)]
		public PipelineDefinition Pipeline { get; set; }

		private IPipelineComponentConfig ComponentConfig
		{
			get
			{
				if (this.cachedComponentConfig == null)
				{
					this.diagnosticsSession.TraceDebug("Creating an instance of pipeline component config", new object[0]);
					this.cachedComponentConfig = new PipelineComponentConfig(this.Configurations);
				}
				return this.cachedComponentConfig;
			}
		}

		private IPipelineComponentFactory ComponentFactory
		{
			get
			{
				if (this.cachedComponentFactory == null)
				{
					if (this.Factory == null)
					{
						throw new ArgumentNullException("Factory");
					}
					this.diagnosticsSession.TraceDebug<string>("Loading assembly {0}", this.Factory.AssemblyFullName);
					Assembly assembly = Assembly.Load(this.Factory.AssemblyFullName);
					this.diagnosticsSession.TraceDebug<string>("Creating an instance of type {0}", this.Factory.TypeFullName);
					this.cachedComponentFactory = (assembly.CreateInstance(this.Factory.TypeFullName) as IPipelineComponentFactory);
				}
				if (this.cachedComponentFactory == null)
				{
					throw new InvalidOperationException("Cannot find a valid factory for the component");
				}
				return this.cachedComponentFactory;
			}
		}

		internal IPipelineComponent CreateComponent(IPipelineContext context, IPipeline nestedPipeline)
		{
			if (this.Pipeline == null)
			{
				this.diagnosticsSession.TraceDebug<string>("Creating an instance of component of pipeline: {0}", this.Name);
				return this.ComponentFactory.CreateInstance(this.ComponentConfig, context);
			}
			if (nestedPipeline == null)
			{
				throw new InvalidOperationException("The definition of component requires a nested pipeline");
			}
			this.diagnosticsSession.TraceDebug<string, string>("Creating an instance of component {0} with nested pipeline {1}", this.Name, this.Pipeline.Name);
			return this.ComponentFactory.CreateInstance(this.ComponentConfig, context, nestedPipeline);
		}

		private readonly IDiagnosticsSession diagnosticsSession;

		private IPipelineComponentConfig cachedComponentConfig;

		private IPipelineComponentFactory cachedComponentFactory;
	}
}
