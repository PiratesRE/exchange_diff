using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public sealed class UnifiedCompliancePrerequisite
	{
		public UnifiedCompliancePrerequisite(Uri spRootSiteUrl, Uri spTenantAdminUrl, IEnumerable<string> prerequisiteList)
		{
			this.SharepointRootSiteUrl = spRootSiteUrl;
			this.SharepointTenantAdminUrl = spTenantAdminUrl;
			if (prerequisiteList != null)
			{
				char[] separator = new char[]
				{
					':'
				};
				foreach (string text in prerequisiteList)
				{
					if (!string.IsNullOrEmpty(text))
					{
						string[] array = text.Split(separator, 2);
						if (array != null && array.Length > 0 && !string.IsNullOrEmpty(array[0]))
						{
							string key = array[0];
							string value = string.Empty;
							if (array.Length > 1)
							{
								value = array[1];
							}
							if (!this.prerequisiteDictionary.ContainsKey(key))
							{
								this.prerequisiteDictionary.Add(key, value);
							}
						}
					}
				}
			}
		}

		public Uri SharepointRootSiteUrl { get; private set; }

		public Uri SharepointTenantAdminUrl { get; private set; }

		public string SharepointSuccessInitializedUtc
		{
			get
			{
				return this.GetValue("SharepointSuccessInitializedUtc");
			}
			set
			{
				this.SetValue("SharepointSuccessInitializedUtc", value);
			}
		}

		public string SharepointPolicyCenterSiteUrl
		{
			get
			{
				return this.GetValue("SharepointPolicyCenterSiteUrl");
			}
			set
			{
				this.SetValue("SharepointPolicyCenterSiteUrl", value);
			}
		}

		private void SetValue(string key, string value)
		{
			if (this.prerequisiteDictionary.ContainsKey(key))
			{
				this.prerequisiteDictionary[key] = value;
				return;
			}
			this.prerequisiteDictionary.Add(key, value);
		}

		private string GetValue(string key)
		{
			if (this.prerequisiteDictionary.ContainsKey(key))
			{
				return this.prerequisiteDictionary[key];
			}
			return string.Empty;
		}

		internal bool IsSharepointInitialized
		{
			get
			{
				return !string.IsNullOrEmpty(this.SharepointSuccessInitializedUtc) && !string.IsNullOrEmpty(this.SharepointPolicyCenterSiteUrl);
			}
		}

		internal bool CanInitializeSharepoint
		{
			get
			{
				return this.SharepointRootSiteUrl != null && this.SharepointTenantAdminUrl != null;
			}
		}

		internal IEnumerable<string> ToPrerequisiteList()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> keyValuePair in this.prerequisiteDictionary)
			{
				list.Add(string.Format("{0}{1}{2}", keyValuePair.Key, ':', keyValuePair.Value));
			}
			return list;
		}

		internal void Redact()
		{
			string text;
			string text2;
			this.SharepointTenantAdminUrl = SuppressingPiiData.Redact(this.SharepointTenantAdminUrl, out text, out text2);
			this.SharepointRootSiteUrl = SuppressingPiiData.Redact(this.SharepointRootSiteUrl, out text, out text2);
			foreach (string key in this.prerequisiteDictionary.Keys.ToList<string>())
			{
				this.prerequisiteDictionary[key] = SuppressingPiiData.Redact(this.prerequisiteDictionary[key]);
			}
		}

		private const char ColonSeparator = ':';

		private Dictionary<string, string> prerequisiteDictionary = new Dictionary<string, string>();
	}
}
