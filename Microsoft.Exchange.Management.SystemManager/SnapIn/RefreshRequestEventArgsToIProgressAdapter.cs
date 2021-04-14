using System;
using System.ComponentModel;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Management.SystemManager;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal class RefreshRequestEventArgsToIProgressAdapter : IReportProgress
	{
		public RefreshRequestEventArgsToIProgressAdapter(RefreshRequestEventArgs eventArgs)
		{
			this.eventArgs = eventArgs;
		}

		public void ReportProgress(int workProcessed, int totalWork, string statusText, string errorHeader)
		{
			this.eventArgs.ReportProgress(workProcessed, totalWork, statusText, null);
			this.workProcessed = workProcessed;
			this.statusText = statusText;
			if (!string.IsNullOrEmpty(errorHeader))
			{
				this.errorHeader = errorHeader;
			}
		}

		internal string StepDescription
		{
			get
			{
				return this.statusText;
			}
		}

		internal string ErrorHeader
		{
			get
			{
				return this.errorHeader;
			}
		}

		[DefaultValue(0)]
		internal int Value
		{
			get
			{
				return this.workProcessed;
			}
		}

		private int workProcessed;

		private string statusText;

		private string errorHeader;

		private RefreshRequestEventArgs eventArgs;
	}
}
