using System;
using System.Globalization;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class SenderIdResult
	{
		public SenderIdResult(SenderIdStatus status)
		{
			if (status == SenderIdStatus.Fail)
			{
				throw new ArgumentOutOfRangeException("status", status, "Invalid constructor usage.");
			}
			this.status = status;
			this.failReason = SenderIdFailReason.None;
		}

		public SenderIdResult(SenderIdStatus status, SenderIdFailReason failReason)
		{
			if (status != SenderIdStatus.Fail || failReason == SenderIdFailReason.NotPermitted || failReason == SenderIdFailReason.None)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid constructor usage for {0}, {1}", new object[]
				{
					status,
					failReason
				}));
			}
			this.status = status;
			this.failReason = failReason;
		}

		public SenderIdResult(SenderIdStatus status, SenderIdFailReason failReason, string explanation)
		{
			if (status != SenderIdStatus.Fail || failReason != SenderIdFailReason.NotPermitted)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid constructor usage for {0}, {1}, {2}", new object[]
				{
					status,
					failReason,
					explanation
				}));
			}
			this.status = status;
			this.failReason = failReason;
			this.explanation = explanation;
		}

		public SenderIdStatus Status
		{
			get
			{
				return this.status;
			}
		}

		public SenderIdFailReason FailReason
		{
			get
			{
				return this.failReason;
			}
		}

		public string Explanation
		{
			get
			{
				return this.explanation;
			}
		}

		private readonly SenderIdStatus status;

		private readonly SenderIdFailReason failReason;

		private readonly string explanation;
	}
}
