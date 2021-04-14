using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public interface IExportHandler : IDisposable
	{
		event EventHandler<ExportStatusEventArgs> OnReportStatistics;

		IExportContext ExportContext { get; }

		ISearchResults SearchResults { get; }

		OperationStatus CurrentStatus { get; }

		bool IsDocIdHintFlightingEnabled { get; set; }

		void EnsureAuthentication(ICredentialHandler credentialHandler, Uri configurationEwsUrl = null);

		void Prepare();

		void Export();

		void Stop();

		void Rollback();
	}
}
