using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal abstract class PathCondition : Condition
	{
		protected PathCondition(string pathName)
		{
			this.PathName = pathName;
		}

		protected string PathName
		{
			get
			{
				return this.pathName;
			}
			set
			{
				this.pathName = value;
			}
		}

		private string pathName;
	}
}
