using System;
using Microsoft.Exchange.Clients.Owa2.Server.Core;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal interface IClientWatsonParameters
	{
		IConsolidationSymbolsMap ConsolidationSymbolsMap { get; }

		IJavaScriptSymbolsMap<AjaxMinSymbolForJavaScript> MinificationSymbolsMapForJavaScript { get; }

		IJavaScriptSymbolsMap<AjaxMinSymbolForScriptSharp> MinificationSymbolsMapForScriptSharp { get; }

		IJavaScriptSymbolsMap<ScriptSharpSymbolWrapper> ObfuscationSymbolsMap { get; }

		SendClientWatsonReportAction ReportAction { get; }

		string OwaVersion { get; }

		string ExchangeSourcesPath { get; }

		bool IsErrorOverReportQuota(int hashCode);
	}
}
