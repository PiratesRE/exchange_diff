using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants.Diagnostics
{
	internal class DiagnosticsProcessor : DiagnosticsProcessorBase
	{
		public new DiagnosticsArgument Arguments
		{
			get
			{
				return base.Arguments;
			}
		}

		public DiagnosticsProcessor(DiagnosableParameters parameters) : base(new DiagnosticsArgument(parameters.Argument))
		{
			this.tbaProcessor = new DiagnosticsTbaProcessor(this.Arguments);
		}

		public XElement Process(TimeBasedAssistantControllerWrapper[] assistantControllers)
		{
			ArgumentValidator.ThrowIfNull("assistantControllers", assistantControllers);
			if (this.Arguments.ArgumentCount != 0)
			{
				return this.tbaProcessor.Process(assistantControllers);
			}
			return DiagnosticsFormatter.FormatHelpElement(this.Arguments);
		}

		private readonly DiagnosticsTbaProcessor tbaProcessor;
	}
}
