using System;
using System.Collections;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public abstract class SharingSynchronizationException : LocalizedException
	{
		public string ErrorDetails
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				this.BuildErrorDetails(stringBuilder);
				return stringBuilder.ToString();
			}
		}

		public Exception AdditionalException { get; set; }

		public SharingSynchronizationException(LocalizedString localizedString) : base(localizedString)
		{
		}

		public SharingSynchronizationException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.ToString());
			stringBuilder.AppendLine("=============");
			this.BuildErrorDetails(stringBuilder);
			return stringBuilder.ToString();
		}

		private void BuildErrorDetails(StringBuilder errorMessage)
		{
			SharingSynchronizationException.AppendExceptionInformation(errorMessage, this);
			if (this.AdditionalException != null)
			{
				errorMessage.AppendLine("=============");
				SharingSynchronizationException.AppendExceptionInformation(errorMessage, this.AdditionalException);
			}
			errorMessage.AppendLine("=============");
		}

		private static void AppendExceptionInformation(StringBuilder errorMessage, Exception exception)
		{
			while (exception != null)
			{
				errorMessage.AppendFormat("{0}: {1}; ", exception.GetType(), exception.Message);
				SoapFaultException ex = exception as SoapFaultException;
				if (ex != null && ex.Fault != null)
				{
					errorMessage.AppendFormat("Fault: {0};", ex.Fault.InnerXml);
				}
				errorMessage.AppendLine();
				foreach (object obj in exception.Data)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if (dictionaryEntry.Value != null)
					{
						errorMessage.AppendFormat("{0}:{1}{2}{1}", dictionaryEntry.Key, Environment.NewLine, dictionaryEntry.Value);
					}
				}
				exception = exception.InnerException;
				if (exception != null)
				{
					errorMessage.AppendLine("-------------");
				}
			}
		}
	}
}
