using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IStatusLog : IDisposable
	{
		void ResetStatusLog(SourceInformationCollection allSourceInformation, OperationStatus status, ExportSettings exportSettings);

		void UpdateSourceStatus(SourceInformation source, int sourceIndex);

		void UpdateStatus(SourceInformationCollection allSourceInformation, OperationStatus status);

		ExportSettings LoadStatus(out SourceInformationCollection allSourceInformaiton, out OperationStatus status);

		void Delete();
	}
}
