using System;
using System.Threading;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal static class FailFast
	{
		public static void Fail(Exception exception)
		{
			ParameterizedThreadStart start = new ParameterizedThreadStart(FailFast.ThrowException);
			Thread thread = new Thread(start);
			FailFast.UnexpectedSubmissionException parameter = new FailFast.UnexpectedSubmissionException(exception);
			thread.Start(parameter);
			thread.Join();
		}

		private static void ThrowException(object exception)
		{
			throw (Exception)exception;
		}

		[Serializable]
		internal class UnexpectedSubmissionException : LocalizedException
		{
			public UnexpectedSubmissionException(Exception innerException) : base(new LocalizedString(Strings.UnexpectedException), innerException)
			{
			}
		}
	}
}
