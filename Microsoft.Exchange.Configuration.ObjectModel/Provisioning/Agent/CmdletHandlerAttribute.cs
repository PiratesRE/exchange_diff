using System;

namespace Microsoft.Exchange.Provisioning.Agent
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class CmdletHandlerAttribute : Attribute
	{
		public CmdletHandlerAttribute(string taskName) : this()
		{
			this.taskName = taskName;
		}

		public CmdletHandlerAttribute()
		{
		}

		public string TaskName
		{
			get
			{
				return this.taskName;
			}
			set
			{
				this.taskName = value;
			}
		}

		private string taskName;
	}
}
