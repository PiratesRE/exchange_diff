using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	internal sealed class PipelineComponentConfig : IPipelineComponentConfig
	{
		internal PipelineComponentConfig(PipelineComponentConfigDefinition[] configurations)
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("PipelineComponentConfig", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.PipelineLoaderTracer, (long)this.GetHashCode());
			if (configurations != null && configurations.Length > 0)
			{
				this.diagnosticsSession.TraceDebug<int>("Created with {0} configurations", configurations.Length);
				this.cachedConfigurations = new Dictionary<string, string>(configurations.Length);
				foreach (PipelineComponentConfigDefinition pipelineComponentConfigDefinition in configurations)
				{
					string name = pipelineComponentConfigDefinition.Name;
					string value = pipelineComponentConfigDefinition.Value;
					this.diagnosticsSession.TraceDebug<string, string>("Adding configuration pair of {0}: {1}", name, value);
					this.cachedConfigurations.Add(name, value);
				}
				return;
			}
			this.diagnosticsSession.TraceDebug("Created with empty configurations", new object[0]);
		}

		public string this[string keyName]
		{
			get
			{
				if (this.cachedConfigurations == null)
				{
					this.diagnosticsSession.TraceDebug("Null is returned due to empty configurations", new object[0]);
					return null;
				}
				string text;
				if (!this.cachedConfigurations.TryGetValue(keyName, out text))
				{
					this.diagnosticsSession.TraceDebug<string>("Null is returned since {0} doesn't exist in configurations", keyName);
					return null;
				}
				this.diagnosticsSession.TraceDebug<string, string>("{1} is returned for configuration {0}", keyName, text);
				return text;
			}
		}

		private readonly Dictionary<string, string> cachedConfigurations;

		private readonly IDiagnosticsSession diagnosticsSession;
	}
}
