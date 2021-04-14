using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SnapIn
{
	[Serializable]
	public class OrganizationSetting
	{
		public string DisplayName { get; set; }

		public Uri Uri { get; set; }

		public bool LogonWithDefaultCredential { get; set; }

		public string CredentialKey { get; set; }

		public string Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
				this.supportedVersionList = null;
			}
		}

		public OrganizationType Type { get; set; }

		public SupportedVersionList SupportedVersionList
		{
			get
			{
				return this.supportedVersionList;
			}
			set
			{
				this.supportedVersionList = value;
			}
		}

		private string key;

		private SupportedVersionList supportedVersionList;
	}
}
