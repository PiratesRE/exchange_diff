using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class ClientRbac
	{
		internal static JsonDictionary<bool> GetRbacData()
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>(ClientRbac.rbacRoles.Count + ClientRbac.HybridEcpFeatures.Count, StringComparer.OrdinalIgnoreCase);
			RbacPrincipal rbacPrincipal = RbacPrincipal.Current;
			foreach (string text in ClientRbac.rbacRoles)
			{
				dictionary.Add(text, rbacPrincipal.IsInRole(text));
			}
			foreach (EcpFeature ecpFeature in ClientRbac.HybridEcpFeatures)
			{
				string name = ecpFeature.GetName();
				dictionary.Add(name, rbacPrincipal.IsInRole(name));
			}
			return new JsonDictionary<bool>(dictionary);
		}

		[Conditional("DEBUG")]
		private static void IsValidKeyCheck(Dictionary<string, bool> dictionary, string key)
		{
			if (dictionary.ContainsKey(key))
			{
				throw new InvalidOperationException("Key '" + key + "' already added before. Please note the RBAC role on server side is not case sensitive.");
			}
			if (key.Contains("+") || key.Contains(","))
			{
				throw new InvalidOperationException("Key '" + key + "' contains '+' or ',', which is not supported. Please split them and add one by one.");
			}
		}

		public static List<EcpFeature> HybridEcpFeatures = new List<EcpFeature>
		{
			EcpFeature.Onboarding,
			EcpFeature.SetupHybridConfiguration
		};

		private static List<string> rbacRoles = new List<string>
		{
			"Enterprise",
			"LiveID",
			"HybridAdmin",
			"MyBaseOptions",
			"Get-Notification"
		};
	}
}
