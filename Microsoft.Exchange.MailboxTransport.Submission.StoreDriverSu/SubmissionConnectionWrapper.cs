using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal sealed class SubmissionConnectionWrapper : DisposeTrackableBase
	{
		internal SubmissionConnectionWrapper(SubmissionConnection connection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			this.connection = connection;
		}

		public ulong Id
		{
			get
			{
				return this.connection.Id;
			}
		}

		public void SubmissionSuccessful(long size, int recipients)
		{
			if (this.connection == null)
			{
				throw new InvalidOperationException();
			}
			this.connection.SubmissionSuccessful(size, recipients);
			this.connection = null;
		}

		public void SubmissionAborted(string reason)
		{
			if (this.connection == null)
			{
				throw new InvalidOperationException();
			}
			this.connection.SubmissionAborted(reason);
			this.connection = null;
		}

		public void SubmissionFailed(string description)
		{
			if (this.connection == null)
			{
				throw new InvalidOperationException();
			}
			this.connection.SubmissionFailed(description);
			this.connection = null;
		}

		public override string ToString()
		{
			if (this.connection == null)
			{
				return "Wrapper on released connection.";
			}
			return "Wrapper on " + this.connection.ToString();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SubmissionConnectionWrapper>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.connection != null)
			{
				throw new InvalidOperationException("The underlying submission connection was not released.");
			}
		}

		private SubmissionConnection connection;
	}
}
