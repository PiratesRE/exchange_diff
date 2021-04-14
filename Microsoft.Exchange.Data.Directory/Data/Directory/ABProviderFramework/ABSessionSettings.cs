using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ABSessionSettings : IABSessionSettings
	{
		public void Set(string propertyName, object propertyValue)
		{
			this.settings[propertyName] = propertyValue;
		}

		public bool TryGet<T>(string propertyName, out T propertyValue)
		{
			object obj;
			if (this.settings.TryGetValue(propertyName, out obj))
			{
				propertyValue = (T)((object)obj);
				return true;
			}
			propertyValue = default(T);
			return false;
		}

		public T Get<T>(string propertyName)
		{
			return (T)((object)this.settings[propertyName]);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ABSessionSettings = ");
			if (this.settings.Count == 0)
			{
				stringBuilder.AppendLine("{}");
			}
			else
			{
				stringBuilder.AppendLine("{");
				foreach (KeyValuePair<string, object> keyValuePair in this.settings)
				{
					stringBuilder.AppendFormat("  {0} = {1}{2}", keyValuePair.Key, (keyValuePair.Value == null) ? "<null>" : keyValuePair.Value.ToString(), Environment.NewLine);
				}
				stringBuilder.AppendLine("}");
			}
			return stringBuilder.ToString();
		}

		public const string Provider = "Provider";

		public const string UserName = "UserName";

		public const string Password = "Password";

		public const string Domain = "Domain";

		public const string Email = "Email";

		public const string Uri = "Uri";

		public const string OrganizationId = "OrganizationId";

		public const string SearchRoot = "SearchRoot";

		public const string Lcid = "Lcid";

		public const string ConsistencyMode = "ConsistencyMode";

		public const string ClientSecurityContext = "ClientSecurityContext";

		public const string Disabled = "Disabled";

		public const string SubscriptionGuid = "SubscriptionGuid";

		public const string Subscription = "Subscription";

		public const string SyncLog = "SyncLog";

		public const string ExchangeVersion = "ExchangeVersion";

		private Dictionary<string, object> settings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
	}
}
