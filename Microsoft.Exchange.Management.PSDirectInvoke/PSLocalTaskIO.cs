using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.PSDirectInvoke
{
	internal class PSLocalTaskIO<TOutputType> : ITaskIOPipeline
	{
		public PSLocalTaskIO()
		{
			this.Objects = new List<TOutputType>();
			this.Errors = new List<TaskErrorInfo>();
		}

		public bool CaptureAdditionalIO { get; set; }

		public List<TaskErrorInfo> Errors { get; private set; }

		public List<TOutputType> Objects { get; private set; }

		public List<PSLocalTaskIOData> AdditionalIO { get; private set; }

		public bool WhatIfMode { get; set; }

		public bool WriteVerbose(LocalizedString input, out LocalizedString output)
		{
			this.HandleIO(PSLocalTaskIOType.Verbose, input);
			output = input;
			return false;
		}

		public bool WriteDebug(LocalizedString input, out LocalizedString output)
		{
			this.HandleIO(PSLocalTaskIOType.Debug, input);
			output = input;
			return false;
		}

		public bool WriteWarning(LocalizedString input, string helperUrl, out LocalizedString output)
		{
			this.HandleIO(PSLocalTaskIOType.Warning, input);
			output = input;
			return false;
		}

		public bool WriteError(TaskErrorInfo input, out TaskErrorInfo output)
		{
			output = input;
			TaskErrorInfo taskErrorInfo = new TaskErrorInfo();
			taskErrorInfo.SetErrorInfo(input.Exception, input.ExchangeErrorCategory.Value, input.Target, input.HelpUrl, input.TerminatePipeline, input.IsKnownError);
			this.Errors.Add(taskErrorInfo);
			return false;
		}

		public bool WriteObject(object input, out object output)
		{
			output = input;
			this.Objects.Add((TOutputType)((object)input));
			return false;
		}

		public bool WriteProgress(ExProgressRecord input, out ExProgressRecord output)
		{
			output = input;
			return false;
		}

		public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll, out bool? output)
		{
			output = new bool?(true);
			return false;
		}

		public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption, out bool? output)
		{
			output = new bool?(!this.WhatIfMode);
			return false;
		}

		private void HandleIO(PSLocalTaskIOType type, LocalizedString input)
		{
			if (this.CaptureAdditionalIO)
			{
				if (this.AdditionalIO == null)
				{
					this.AdditionalIO = new List<PSLocalTaskIOData>(10);
				}
				this.AdditionalIO.Add(new PSLocalTaskIOData(type, DateTime.UtcNow, input.ToString()));
			}
		}
	}
}
