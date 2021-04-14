using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class Scopes
	{
		public const string ReadSelf = "@R:Self";

		public const string WriteSelf = "@W:Self";

		public const string ReadMyGAL = "@R:MyGAL";

		public const string WriteMyGAL = "@W:MyGAL";

		public const string ReadOrg = "@R:Organization";

		public const string WriteOrg = "@W:Organization";

		public const string WriteSelfOrOrg = "@W:Self|Organization";

		public const string ReadMyDistributionGroups = "@R:MyDistributionGroups";

		public const string WriteMyDistributionGroups = "@W:MyDistributionGroups";

		public const string OrgConfig = "@C:OrganizationConfig";
	}
}
