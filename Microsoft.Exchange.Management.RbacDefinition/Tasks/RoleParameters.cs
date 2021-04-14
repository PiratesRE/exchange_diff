using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleParameters
	{
		public RoleParameters(string[] neededFeatures, string cmdletParameters)
		{
			this.parameters = cmdletParameters;
			this.requiredFeatures = neededFeatures;
		}

		public bool TryAddToRoleEntry(List<string> enabledFeatures, StringBuilder sb)
		{
			bool result = false;
			if (enabledFeatures == null)
			{
				result = true;
				if (!string.IsNullOrEmpty(this.parameters))
				{
					sb.Append(",");
					sb.Append(this.parameters);
				}
			}
			else
			{
				foreach (string item in this.requiredFeatures)
				{
					if (enabledFeatures.Contains(item))
					{
						result = true;
						if (!string.IsNullOrEmpty(this.parameters))
						{
							sb.Append(",");
							sb.Append(this.parameters);
							break;
						}
					}
				}
			}
			return result;
		}

		public bool TryAddToRoleEntryFilteringProhibitedActions(List<string> enabledFeatures, List<string> prohibitedActions, StringBuilder sb)
		{
			if (prohibitedActions == null)
			{
				throw new ArgumentNullException("prohibitedActions");
			}
			IEnumerable<string> source = this.RequiredFeatures.Intersect(prohibitedActions, StringComparer.OrdinalIgnoreCase);
			return source.Count<string>() == 0 && this.TryAddToRoleEntry(enabledFeatures, sb);
		}

		public string[] RequiredFeatures
		{
			get
			{
				return this.requiredFeatures;
			}
		}

		private string[] requiredFeatures;

		private readonly string parameters;
	}
}
