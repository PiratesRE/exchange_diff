using System;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementConsole;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal sealed class ScopeNodeProgressProvider : IProgressProvider
	{
		public ScopeNodeProgressProvider(ScopeNode scopeNode)
		{
			this.scopeNode = scopeNode;
		}

		public IProgress CreateProgress(string operationName)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<string, string>(0L, "-->ScopeNodeProgressProvider.CreateProgress: {0}, {1}", this.scopeNode.LanguageIndependentName, operationName);
			CustomStatus customStatus = new CustomStatus();
			IProgress result = new StatusProgress(customStatus, this.scopeNode.SnapIn);
			customStatus.Title = ExchangeUserControl.RemoveAccelerator(operationName);
			customStatus.Start(this.scopeNode);
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<string, string>(0L, "<--ScopeNodeProgressProvider.CreateProgress: {0}, {1}", this.scopeNode.LanguageIndependentName, operationName);
			return result;
		}

		private ScopeNode scopeNode;
	}
}
