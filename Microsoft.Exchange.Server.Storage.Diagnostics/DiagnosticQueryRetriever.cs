using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class DiagnosticQueryRetriever
	{
		protected DiagnosticQueryRetriever(DiagnosticQueryResults results)
		{
			this.results = (results ?? DiagnosticQueryRetriever.EmptyResults);
		}

		public DiagnosticQueryResults Results
		{
			get
			{
				return this.results;
			}
		}

		private static DiagnosticQueryResults EmptyResults
		{
			get
			{
				if (DiagnosticQueryRetriever.emptyResults == null)
				{
					DiagnosticQueryRetriever.emptyResults = DiagnosticQueryResults.Create(new string[0], new Type[0], new uint[0], new object[0][], false, false);
				}
				return DiagnosticQueryRetriever.emptyResults;
			}
		}

		public static DiagnosticQueryRetriever Create(DiagnosticQueryParser parser, DiagnosableParameters parameters)
		{
			return new DiagnosticQueryRetriever(DiagnosticQueryRetriever.EmptyResults);
		}

		private static DiagnosticQueryResults emptyResults;

		private DiagnosticQueryResults results;
	}
}
