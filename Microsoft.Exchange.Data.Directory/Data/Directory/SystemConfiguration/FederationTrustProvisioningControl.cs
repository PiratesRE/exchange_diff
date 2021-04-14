using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class FederationTrustProvisioningControl
	{
		private static string GetKeyValue(string key, string federationControls)
		{
			if (string.IsNullOrEmpty(federationControls))
			{
				return string.Empty;
			}
			int num = federationControls.IndexOf(key + FederationTrustProvisioningControl.valueSeparator);
			if (0 > num)
			{
				return string.Empty;
			}
			int num2 = num + key.Length + 1;
			if (federationControls.Length <= num2)
			{
				return string.Empty;
			}
			int num3 = federationControls.IndexOf(FederationTrustProvisioningControl.controlSeparator, num2);
			int num4 = num3 - num2;
			if (0 <= num3 && 0 < num4)
			{
				return federationControls.Substring(num2, num4);
			}
			return federationControls.Substring(num2);
		}

		private static string PutKeyValue(string key, string value, string federationControls)
		{
			if (string.IsNullOrEmpty(federationControls))
			{
				if (!string.IsNullOrEmpty(value))
				{
					return key + FederationTrustProvisioningControl.valueSeparator + value + FederationTrustProvisioningControl.controlSeparator;
				}
				return string.Empty;
			}
			else
			{
				string[] array = federationControls.ToUpper().Split(FederationTrustProvisioningControl.controlSeparatorList, StringSplitOptions.RemoveEmptyEntries);
				if (array != null && array.Length != 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					bool flag = false;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].StartsWith(key))
						{
							if (!flag)
							{
								flag = true;
								if (!string.IsNullOrEmpty(value))
								{
									stringBuilder.Append(key + FederationTrustProvisioningControl.valueSeparator + value + FederationTrustProvisioningControl.controlSeparator);
								}
							}
						}
						else
						{
							stringBuilder.Append(array[i] + FederationTrustProvisioningControl.controlSeparator);
						}
					}
					if (!flag && !string.IsNullOrEmpty(value))
					{
						stringBuilder.Append(key + FederationTrustProvisioningControl.valueSeparator + value + FederationTrustProvisioningControl.controlSeparator);
					}
					return stringBuilder.ToString();
				}
				if (!string.IsNullOrEmpty(value))
				{
					return key + FederationTrustProvisioningControl.valueSeparator + value + FederationTrustProvisioningControl.controlSeparator;
				}
				return string.Empty;
			}
		}

		public static string GetNamespaceProvisioner(string federationControls)
		{
			string keyValue = FederationTrustProvisioningControl.GetKeyValue("NAMESPACEPROVISIONER", federationControls);
			if (!string.IsNullOrEmpty(keyValue))
			{
				return keyValue;
			}
			return "EXTERNALPROCESS";
		}

		public static string PutNamespaceProvisioner(string namespaceProvisioner, string federationControls)
		{
			return FederationTrustProvisioningControl.PutKeyValue("NAMESPACEPROVISIONER", namespaceProvisioner, federationControls);
		}

		public static string GetAdministratorProvisioningId(string federationControls)
		{
			return FederationTrustProvisioningControl.GetKeyValue("ADMINKEY", federationControls);
		}

		public static string PutAdministratorProvisioningId(string administrationProvisioiningKey, string federationControls)
		{
			return FederationTrustProvisioningControl.PutKeyValue("ADMINKEY", administrationProvisioiningKey, federationControls);
		}

		internal const string WindowsLiveDomainServices = "WINDOWSLIVEDOMAINSERVICES";

		internal const string WindowsLiveDomainServices2 = "WINDOWSLIVEDOMAINSERVICES2";

		internal const string ExternalProcess = "EXTERNALPROCESS";

		private const string AdminKeyKeyword = "ADMINKEY";

		private const string NamespaceProvisionerKeyword = "NAMESPACEPROVISIONER";

		private static readonly char[] controlSeparatorList = new char[]
		{
			';'
		};

		private static readonly string controlSeparator = ";";

		private static readonly string valueSeparator = "=";
	}
}
