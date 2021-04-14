using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DDICodeBehind
	{
		public DDICodeBehind()
		{
			this.RbacMetaData = new Dictionary<string, List<string>>();
		}

		public Dictionary<string, List<string>> RbacMetaData { get; set; }

		public virtual void ApplyMetaData()
		{
		}

		public void RegisterRbacDependency(string variable, List<string> dependentVariable)
		{
			if (this.RbacMetaData.ContainsKey(variable))
			{
				throw new InvalidOperationException("The same variable cannot be registered twice!");
			}
			this.RbacMetaData[variable] = dependentVariable;
		}
	}
}
