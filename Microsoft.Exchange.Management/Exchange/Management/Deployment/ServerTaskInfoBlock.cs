using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class ServerTaskInfoBlock : TaskInfoBlock
	{
		public ServerTaskInfoEntry Standalone
		{
			get
			{
				if (this.standalone == null)
				{
					this.standalone = new ServerTaskInfoEntry();
				}
				return this.standalone;
			}
			set
			{
				this.standalone = value;
			}
		}

		internal override string GetTask(InstallationCircumstances circumstance)
		{
			switch (circumstance)
			{
			case InstallationCircumstances.Standalone:
				return this.Standalone.Task;
			}
			return string.Empty;
		}

		private ServerTaskInfoEntry standalone;
	}
}
