using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants.Diagnostics
{
	internal abstract class DiagnosticsProcessorBase
	{
		protected DiagnosticsArgument Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		protected DiagnosticsProcessorBase(DiagnosticsArgument arguments)
		{
			ArgumentValidator.ThrowIfNull("arguments", arguments);
			this.arguments = arguments;
		}

		private readonly DiagnosticsArgument arguments;
	}
}
