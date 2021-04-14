using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface IExecutionDiagnostics
	{
		int MailboxNumber { get; }

		byte OperationId { get; }

		byte OperationType { get; }

		byte ClientType { get; }

		byte OperationFlags { get; }

		int CorrelationId { get; }

		string DiagnosticInformationForWatsonReport { get; }

		void OnExceptionCatch(Exception exception);

		void OnExceptionCatch(Exception exception, object diagnosticData);

		void OnUnhandledException(Exception exception);
	}
}
