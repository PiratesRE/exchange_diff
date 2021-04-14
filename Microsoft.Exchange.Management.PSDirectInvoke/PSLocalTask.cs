using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.PSDirectInvoke
{
	internal class PSLocalTask<T, TResult> : IDisposable where T : Task
	{
		public PSLocalTask(T task)
		{
			this.Task = task;
			this.TaskIO = new PSLocalTaskIO<TResult>();
			task.PrependTaskIOPipelineHandler(this.TaskIO);
		}

		public bool CaptureAdditionalIO
		{
			get
			{
				return this.TaskIO.CaptureAdditionalIO;
			}
			set
			{
				this.TaskIO.CaptureAdditionalIO = value;
			}
		}

		public T Task { get; private set; }

		public bool WhatIfMode
		{
			get
			{
				return this.TaskIO.WhatIfMode;
			}
			set
			{
				T task = this.Task;
				PropertyBag userSpecifiedParameters = task.CurrentTaskContext.InvocationInfo.UserSpecifiedParameters;
				if (value)
				{
					userSpecifiedParameters["WhatIf"] = SwitchParameter.Present;
				}
				else if (userSpecifiedParameters.Contains("WhatIf"))
				{
					userSpecifiedParameters.Remove("WhatIf");
				}
				this.TaskIO.WhatIfMode = value;
			}
		}

		public TResult Result
		{
			get
			{
				if (this.TaskIO.Objects.Count > 0)
				{
					return this.TaskIO.Objects[0];
				}
				return default(TResult);
			}
		}

		public TaskErrorInfo Error
		{
			get
			{
				if (this.TaskIO.Errors.Count > 0)
				{
					return this.TaskIO.Errors[0];
				}
				return null;
			}
		}

		public string ErrorMessage
		{
			get
			{
				if (this.TaskIO.Errors.Count > 0 && this.TaskIO.Errors[0].Exception != null)
				{
					return this.TaskIO.Errors[0].Exception.Message;
				}
				return string.Empty;
			}
		}

		public IList<TResult> AllResults
		{
			get
			{
				return this.TaskIO.Objects;
			}
		}

		public List<TaskErrorInfo> AllErrors
		{
			get
			{
				return this.TaskIO.Errors;
			}
		}

		public List<PSLocalTaskIOData> AdditionalIO
		{
			get
			{
				return this.TaskIO.AdditionalIO;
			}
		}

		private PSLocalTaskIO<TResult> TaskIO { get; set; }

		public void Dispose()
		{
			T task = this.Task;
			task.Dispose();
		}

		private const string WhatIfParameter = "WhatIf";
	}
}
