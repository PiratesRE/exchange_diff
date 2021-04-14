using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class TaskErrorInfo
	{
		public ExchangeErrorCategory? ExchangeErrorCategory { get; private set; }

		public object Target { get; private set; }

		public string HelpUrl { get; private set; }

		public bool TerminatePipeline
		{
			get
			{
				return this.terminatePipeline;
			}
			set
			{
				this.terminatePipeline = (this.terminatePipeline || value);
			}
		}

		public bool HasErrors { get; private set; }

		public Exception Exception { get; private set; }

		public bool IsKnownError { get; private set; }

		public void SetErrorInfo(Exception exception, ExchangeErrorCategory errorCategory, object target, string helpUrl, bool terminatePipeline, bool isKnownError)
		{
			this.HasErrors = true;
			this.Exception = exception;
			this.ExchangeErrorCategory = new ExchangeErrorCategory?(errorCategory);
			this.Target = target;
			this.HelpUrl = helpUrl;
			this.TerminatePipeline = terminatePipeline;
			this.IsKnownError = isKnownError;
		}

		public void ResetErrorInfo()
		{
			this.HasErrors = false;
			this.Exception = null;
			this.ExchangeErrorCategory = new ExchangeErrorCategory?((ExchangeErrorCategory)0);
			this.Target = null;
			this.HelpUrl = null;
			this.TerminatePipeline = false;
			this.IsKnownError = false;
		}

		private bool terminatePipeline;
	}
}
