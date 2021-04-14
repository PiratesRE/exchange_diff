using System;
using System.Collections;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	internal class ProvisioningContext
	{
		public string TaskName
		{
			get
			{
				return this.taskName;
			}
		}

		public IDictionary UserSpecifiedParameters
		{
			get
			{
				return this.userSpecifiedParameters;
			}
		}

		public ProvisioningContext(string taskName, IDictionary userSpecifiedParameters)
		{
			this.taskName = taskName;
			this.userSpecifiedParameters = userSpecifiedParameters;
		}

		private string taskName;

		private IDictionary userSpecifiedParameters;
	}
}
