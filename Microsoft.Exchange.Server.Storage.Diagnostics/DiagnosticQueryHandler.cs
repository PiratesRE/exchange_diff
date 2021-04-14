using System;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal sealed class DiagnosticQueryHandler : StoreDiagnosticInfoHandler
	{
		private DiagnosticQueryHandler(DiagnosticQueryFactory factory) : base("ManagedStoreQueryHandler")
		{
			this.factory = factory;
		}

		public static DiagnosticQueryHandler Create(DiagnosticQueryFactory factory)
		{
			return new DiagnosticQueryHandler(factory);
		}

		public override XElement GetDiagnosticQuery(DiagnosableParameters parameters)
		{
			XElement result;
			try
			{
				DiagnosticQueryParser parser = this.factory.CreateParser(parameters.Argument);
				DiagnosticQueryRetriever diagnosticQueryRetriever = this.factory.CreateRetriever(parser, parameters);
				DiagnosticQueryXmlFormatter diagnosticQueryXmlFormatter = DiagnosticQueryXmlFormatter.Create(diagnosticQueryRetriever.Results);
				XElement content = diagnosticQueryXmlFormatter.FormatResults();
				result = new XElement(base.GetDiagnosticComponentName(), content);
			}
			catch (DiagnosticQueryException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				XElement content2 = DiagnosticQueryXmlFormatter.FormatException(ex);
				result = new XElement(base.GetDiagnosticComponentName(), content2);
			}
			return result;
		}

		internal string Run(string query)
		{
			string result;
			try
			{
				DiagnosticQueryParser parser = this.factory.CreateParser(query);
				DiagnosableParameters parameters = DiagnosableParameters.Create(query, false, false, string.Empty);
				DiagnosticQueryRetriever diagnosticQueryRetriever = this.factory.CreateRetriever(parser, parameters);
				DiagnosticQueryTableFormatter diagnosticQueryTableFormatter = DiagnosticQueryTableFormatter.Create(diagnosticQueryRetriever.Results);
				StringBuilder stringBuilder = diagnosticQueryTableFormatter.FormatResults();
				result = stringBuilder.ToString();
			}
			catch (DiagnosticQueryException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				StringBuilder stringBuilder2 = DiagnosticQueryTableFormatter.FormatException(ex);
				result = stringBuilder2.ToString();
			}
			return result;
		}

		private readonly DiagnosticQueryFactory factory;
	}
}
