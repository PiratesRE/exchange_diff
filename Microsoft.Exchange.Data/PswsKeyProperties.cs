using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Data
{
	internal static class PswsKeyProperties
	{
		internal static List<string> GetPropertiesNeedUrlTokenInputDecode(string cmdletName)
		{
			List<string> result;
			if (!PswsKeyProperties.PropertiesNeedUrlTokenInputDecode.TryGetValue(cmdletName, out result))
			{
				return PswsKeyProperties.DefaultPropertiesNeedUrlTokenInputDecode;
			}
			return result;
		}

		internal static bool IsKeyProperty(object obj, PropertyDefinition property, string propertyInStr)
		{
			if (obj == null)
			{
				return false;
			}
			if (!string.IsNullOrWhiteSpace(propertyInStr))
			{
				return "Identity".Equals(propertyInStr);
			}
			if (obj is ConfigurableObject && property == ((ConfigurableObject)obj).propertyBag.ObjectIdentityPropertyDefinition)
			{
				return true;
			}
			string key = obj.GetType().ToString();
			string name = property.Name;
			string[] source;
			return PswsKeyProperties.PropertiesNeedUrlTokenOutputEncode.TryGetValue(key, out source) && source.Contains(name, StringComparer.OrdinalIgnoreCase);
		}

		internal const string CommonIdentityProperty = "Identity";

		private static readonly Dictionary<string, string[]> PropertiesNeedUrlTokenOutputEncode = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"Microsoft.Exchange.Data.Directory.Permission.RecipientPermission",
				new string[]
				{
					"Trustee"
				}
			},
			{
				"Microsoft.Exchange.Management.RecipientTasks.MailboxAcePresentationObject",
				new string[]
				{
					"User"
				}
			},
			{
				"Microsoft.Exchange.Management.Extension.App",
				new string[]
				{
					"Identity"
				}
			}
		};

		private static readonly List<string> DefaultPropertiesNeedUrlTokenInputDecode = new List<string>
		{
			"Identity"
		};

		private static readonly Dictionary<string, List<string>> PropertiesNeedUrlTokenInputDecode = new Dictionary<string, List<string>>
		{
			{
				"Add-DistributionGroupMember",
				new List<string>
				{
					"Identity",
					"Member"
				}
			},
			{
				"New-CompliancePolicySyncNotification",
				new List<string>()
			},
			{
				"Add-MailboxPermission",
				new List<string>()
			},
			{
				"Add-RecipientPermission",
				new List<string>()
			},
			{
				"Get-MailboxPermission",
				new List<string>
				{
					"Identity",
					"User"
				}
			},
			{
				"Get-RecipientPermission",
				new List<string>
				{
					"Identity",
					"Trustee"
				}
			},
			{
				"Remove-DistributionGroupMember",
				new List<string>
				{
					"Identity",
					"Member"
				}
			},
			{
				"Remove-MailboxPermission",
				new List<string>
				{
					"Identity"
				}
			},
			{
				"Remove-RecipientPermission",
				new List<string>
				{
					"Identity",
					"Trustee"
				}
			}
		};
	}
}
