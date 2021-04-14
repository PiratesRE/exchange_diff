using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class RefreshProgressChangedEventArgs : ProgressChangedEventArgs
	{
		public RefreshProgressChangedEventArgs(RefreshRequestEventArgs request, int workProcessed, int totalWork, string statusText, object userState) : base(RefreshProgressChangedEventArgs.GetPercentage(workProcessed, totalWork), userState)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			this.request = request;
			this.workProcessed = workProcessed;
			this.totalWork = totalWork;
			this.statusText = statusText;
			this.isFirstProgressReport = !request.ReportedProgress;
		}

		private static int GetPercentage(int workProcessed, int totalWork)
		{
			if (totalWork <= 0)
			{
				throw new ArgumentOutOfRangeException("totalWork", totalWork, "totalWork <= 0");
			}
			if (workProcessed < 0)
			{
				throw new ArgumentOutOfRangeException("workProcessed", workProcessed, "workProcessed < 0");
			}
			if (workProcessed > totalWork)
			{
				throw new ArgumentOutOfRangeException("workProcessed", workProcessed, "workProcessed > totalWork");
			}
			return checked(workProcessed * 100) / totalWork;
		}

		public object RequestArgument
		{
			get
			{
				return this.request.Argument;
			}
		}

		public RefreshRequestEventArgs Request
		{
			get
			{
				return this.request;
			}
		}

		public bool CancellationPending
		{
			get
			{
				return this.request.CancellationPending;
			}
		}

		public bool IsFullRefresh
		{
			get
			{
				return this.request.IsFullRefresh;
			}
		}

		public int WorkProcessed
		{
			get
			{
				return this.workProcessed;
			}
		}

		public int TotalWork
		{
			get
			{
				return this.totalWork;
			}
		}

		public string StatusText
		{
			get
			{
				return this.statusText;
			}
		}

		public bool IsFirstProgressReport
		{
			get
			{
				return this.isFirstProgressReport;
			}
		}

		private RefreshRequestEventArgs request;

		private int workProcessed;

		private int totalWork = 100;

		private string statusText = "";

		private bool isFirstProgressReport;
	}
}
