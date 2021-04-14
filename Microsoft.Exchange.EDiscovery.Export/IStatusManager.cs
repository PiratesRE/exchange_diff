using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IStatusManager : IDisposable
	{
		SourceInformationCollection AllSourceInformation { get; }

		OperationStatus CurrentStatus { get; }

		bool BeginProcedure(ProcedureType procedureRequest);

		void EndProcedure();

		void Checkpoint(string sourceId);

		void Rollback(bool removeStatusLog);
	}
}
