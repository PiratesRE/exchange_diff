using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleToRAPAssignmentDefinition
	{
		public RoleType Type { get; private set; }

		public RoleToRAPAssignmentDefinition(RoleType roleType, string[] neededFeatures, string introducedInBuild)
		{
			this.Type = roleType;
			this.neededFeatures = neededFeatures;
			this.introducedInBuild = introducedInBuild;
		}

		public ExchangeBuild IntroducedInBuild
		{
			get
			{
				if (this.introducedInBuild == null)
				{
					return new ExchangeBuild(0, 0, 0, 0);
				}
				return ExchangeBuild.Parse(this.introducedInBuild);
			}
		}

		public bool SatisfyCondition(List<string> enabledFeatures)
		{
			bool result = false;
			if (enabledFeatures == null || this.neededFeatures == null)
			{
				result = true;
			}
			else
			{
				foreach (string item in this.neededFeatures)
				{
					if (enabledFeatures.Contains(item))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private string[] neededFeatures;

		private readonly string introducedInBuild;
	}
}
