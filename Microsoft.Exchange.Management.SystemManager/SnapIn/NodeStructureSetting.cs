using System;

namespace Microsoft.Exchange.Management.SnapIn
{
	[Serializable]
	public class NodeStructureSetting
	{
		public string DisplayName { get; set; }

		public Uri Uri { get; set; }

		public bool LogonWithDefaultCredential { get; set; }

		public string CredentialKey { get; set; }

		public NodeStructureSettingState State { get; set; }

		public string Key { get; set; }

		public OrganizationType Type { get; set; }
	}
}
