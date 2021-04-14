using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class PerTenantPolicyTipMessageConfig : TenantConfigurationCacheableItem<PolicyTipMessageConfig>
	{
		public override long ItemSize
		{
			get
			{
				if (this.tenantPolicyTipMessageConfigsDictionary == null)
				{
					return 18L;
				}
				long num = 18L;
				return num + (this.estimatedTenantPolicyTipMessageConfigsDictionarySize + 8L);
			}
		}

		public override void ReadData(IConfigurationSession session)
		{
			ADPagedReader<PolicyTipMessageConfig> adpagedReader = session.FindPaged<PolicyTipMessageConfig>(session.GetOrgContainerId().GetDescendantId(PolicyTipMessageConfig.PolicyTipMessageConfigContainer), QueryScope.SubTree, null, null, 0);
			IEnumerable<PolicyTipMessageConfig> source = adpagedReader.ReadAllPages();
			this.tenantPolicyTipMessageConfigsDictionary = source.ToDictionary((PolicyTipMessageConfig m) => Tuple.Create<string, PolicyTipMessageConfigAction>(m.Locale, m.Action), (PolicyTipMessageConfig m) => m.Value);
			long num = 0L;
			foreach (KeyValuePair<Tuple<string, PolicyTipMessageConfigAction>, string> keyValuePair in this.tenantPolicyTipMessageConfigsDictionary)
			{
				Tuple<string, PolicyTipMessageConfigAction> key = keyValuePair.Key;
				num += (long)key.Item1.Length;
				num += 4L;
				num += (long)keyValuePair.Value.Length;
			}
			this.estimatedTenantPolicyTipMessageConfigsDictionarySize = num;
		}

		public string GetPolicyTipMessage(string locale, PolicyTipMessageConfigAction action)
		{
			return PerTenantPolicyTipMessageConfig.GetPolicyTipMessage(locale, action, this.tenantPolicyTipMessageConfigsDictionary);
		}

		public static string GetPolicyTipMessage(string locale, PolicyTipMessageConfigAction action, Dictionary<Tuple<string, PolicyTipMessageConfigAction>, string> tenantPolicyTipMessageConfigsDictionary)
		{
			if (tenantPolicyTipMessageConfigsDictionary == null || (string.IsNullOrEmpty(locale) && action != PolicyTipMessageConfigAction.Url))
			{
				return null;
			}
			if (action == PolicyTipMessageConfigAction.Url)
			{
				locale = string.Empty;
			}
			string text = null;
			Tuple<string, PolicyTipMessageConfigAction> key = new Tuple<string, PolicyTipMessageConfigAction>(locale, action);
			tenantPolicyTipMessageConfigsDictionary.TryGetValue(key, out text);
			if (text == null)
			{
				int num = locale.IndexOf('-');
				if (num > 0)
				{
					locale = locale.Substring(0, num);
					text = PerTenantPolicyTipMessageConfig.GetPolicyTipMessage(locale, action, tenantPolicyTipMessageConfigsDictionary);
				}
			}
			return text;
		}

		private const int FixedClrObjectOverhead = 18;

		private long estimatedTenantPolicyTipMessageConfigsDictionarySize;

		private Dictionary<Tuple<string, PolicyTipMessageConfigAction>, string> tenantPolicyTipMessageConfigsDictionary;
	}
}
