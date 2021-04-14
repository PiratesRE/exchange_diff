using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal class DiagnosticQueryFactory
	{
		private static DiagnosticQueryFactory Instance
		{
			get
			{
				if (DiagnosticQueryFactory.instance == null)
				{
					DiagnosticQueryFactory.instance = new DiagnosticQueryFactory();
				}
				return DiagnosticQueryFactory.instance;
			}
		}

		public static DiagnosticQueryFactory CreateFactory()
		{
			return DiagnosticQueryFactory.Instance;
		}

		public DiagnosticQueryParser CreateParser(string query)
		{
			return DiagnosticQueryParser.Create(query);
		}

		public virtual DiagnosticQueryRetriever CreateRetriever(DiagnosticQueryParser parser, DiagnosableParameters parameters)
		{
			return DiagnosticQueryRetriever.Create(parser, parameters);
		}

		private static DiagnosticQueryFactory instance;
	}
}
