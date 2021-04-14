using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class DirectReferenceErrorReporter : IReferenceErrorReporter
	{
		internal DirectReferenceErrorReporter(Task.ErrorLoggerDelegate writeError)
		{
			this.writeError = writeError;
		}

		void IReferenceErrorReporter.ReportError(Task.ErrorLoggerDelegate writeError)
		{
		}

		void IReferenceErrorReporter.ValidateReference(string parameter, string referenceValue, ValidateReferenceDelegate validateReferenceMethood)
		{
			validateReferenceMethood(this.writeError);
		}

		private Task.ErrorLoggerDelegate writeError;
	}
}
