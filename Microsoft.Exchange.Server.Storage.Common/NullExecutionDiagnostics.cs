using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class NullExecutionDiagnostics : IExecutionDiagnostics
	{
		public static NullExecutionDiagnostics Instance
		{
			get
			{
				return NullExecutionDiagnostics.instance;
			}
		}

		public string DiagnosticInformationForWatsonReport
		{
			get
			{
				return "NullExecutionDiagnosticsInformationForWatsonReport";
			}
		}

		int IExecutionDiagnostics.MailboxNumber
		{
			get
			{
				return 0;
			}
		}

		byte IExecutionDiagnostics.OperationId
		{
			get
			{
				return 0;
			}
		}

		byte IExecutionDiagnostics.OperationType
		{
			get
			{
				return 0;
			}
		}

		byte IExecutionDiagnostics.ClientType
		{
			get
			{
				return 0;
			}
		}

		byte IExecutionDiagnostics.OperationFlags
		{
			get
			{
				return 0;
			}
		}

		int IExecutionDiagnostics.CorrelationId
		{
			get
			{
				return 0;
			}
		}

		public void OnExceptionCatch(Exception exception)
		{
			this.OnExceptionCatch(exception, null);
		}

		public void OnExceptionCatch(Exception exception, object diagnosticData)
		{
			ErrorHelper.OnExceptionCatch(0, 0, 0, 0, -1, exception, diagnosticData);
		}

		public void OnUnhandledException(Exception exception)
		{
		}

		private static readonly NullExecutionDiagnostics instance = new NullExecutionDiagnostics();
	}
}
