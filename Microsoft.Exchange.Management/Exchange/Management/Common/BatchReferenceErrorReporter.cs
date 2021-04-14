using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	internal class BatchReferenceErrorReporter : IReferenceErrorReporter
	{
		public void ReportError(Task.ErrorLoggerDelegate writeError)
		{
			if (this.errors.Count > 0)
			{
				List<ReferenceParameterException> list = new List<ReferenceParameterException>();
				foreach (KeyValuePair<string, List<ReferenceException>> keyValuePair in this.errors)
				{
					ReferenceParameterException item = new ReferenceParameterException(Strings.ErrorReferenceParameter(keyValuePair.Key), keyValuePair.Key, keyValuePair.Value.ToArray());
					list.Add(item);
				}
				MultiReferenceParameterException exception = new MultiReferenceParameterException(Strings.ErrorMultiReferenceParameter(string.Join(", ", this.errors.Keys.ToArray<string>()), string.Join(", ", this.referenceValues.ToArray<string>())), list.ToArray());
				writeError(exception, ExchangeErrorCategory.Client, null);
			}
		}

		void IReferenceErrorReporter.ValidateReference(string parameter, string rawValue, ValidateReferenceDelegate validateReferenceMethood)
		{
			try
			{
				validateReferenceMethood(new Task.ErrorLoggerDelegate(this.WriteError));
			}
			catch (ManagementObjectNotFoundException innerException)
			{
				ReferenceNotFoundException error = new ReferenceNotFoundException(rawValue, innerException);
				this.AddError(parameter, error);
			}
			catch (ManagementObjectAmbiguousException innerException2)
			{
				ReferenceAmbiguousException error2 = new ReferenceAmbiguousException(rawValue, innerException2);
				this.AddError(parameter, error2);
			}
			catch (BatchReferenceErrorReporter.ValidationException ex)
			{
				LocalizedException ex2 = ex.InnerException as LocalizedException;
				ReferenceInvalidException error3;
				if (ex2 != null)
				{
					error3 = new ReferenceInvalidException(rawValue, ex2);
				}
				else
				{
					error3 = new ReferenceInvalidException(rawValue, ex.InnerException);
				}
				this.AddError(parameter, error3);
			}
		}

		private void AddError(string parameter, ReferenceException error)
		{
			if (!this.errors.ContainsKey(parameter))
			{
				this.errors.Add(parameter, new List<ReferenceException>());
			}
			this.errors[parameter].Add(error);
			if (!this.referenceValues.Contains(error.ReferenceValue))
			{
				this.referenceValues.Add(error.ReferenceValue);
			}
		}

		private void WriteError(Exception exception, ExchangeErrorCategory category, object target)
		{
			throw new BatchReferenceErrorReporter.ValidationException(exception);
		}

		private Dictionary<string, List<ReferenceException>> errors = new Dictionary<string, List<ReferenceException>>();

		private HashSet<string> referenceValues = new HashSet<string>();

		private class ValidationException : Exception
		{
			public ValidationException(Exception exception) : base(null, exception)
			{
			}
		}
	}
}
