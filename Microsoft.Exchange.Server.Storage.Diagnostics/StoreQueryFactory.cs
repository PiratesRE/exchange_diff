using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal class StoreQueryFactory : DiagnosticQueryFactory
	{
		private static DiagnosticQueryFactory Instance
		{
			get
			{
				if (StoreQueryFactory.instance == null)
				{
					StoreQueryFactory.instance = new StoreQueryFactory();
				}
				return StoreQueryFactory.instance;
			}
		}

		public new static DiagnosticQueryFactory CreateFactory()
		{
			return StoreQueryFactory.Instance;
		}

		public override DiagnosticQueryRetriever CreateRetriever(DiagnosticQueryParser parser, DiagnosableParameters parameters)
		{
			return StoreQueryRetriever.Create(parser, parameters);
		}

		private static DiagnosticQueryFactory instance;
	}
}
