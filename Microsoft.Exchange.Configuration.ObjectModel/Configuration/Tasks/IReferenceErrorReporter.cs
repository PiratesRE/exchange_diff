using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal interface IReferenceErrorReporter
	{
		void ReportError(Task.ErrorLoggerDelegate writeError);

		void ValidateReference(string parameter, string referenceValue, ValidateReferenceDelegate validateReferenceMethood);
	}
}
