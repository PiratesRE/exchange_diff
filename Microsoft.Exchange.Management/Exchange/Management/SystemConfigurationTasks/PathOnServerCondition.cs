using System;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal abstract class PathOnServerCondition : PathCondition
	{
		protected PathOnServerCondition(string computerName, string pathName) : base(pathName)
		{
			this.ComputerName = computerName;
		}

		protected string ComputerName
		{
			get
			{
				return this.computerName;
			}
			set
			{
				this.computerName = value;
			}
		}

		private string computerName;
	}
}
