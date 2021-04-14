using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ClientExtensionUserParametersType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ClientExtensionUserParameters
	{
		[XmlAttribute]
		public string UserId { get; set; }

		[XmlAttribute]
		public bool EnabledOnly { get; set; }

		[XmlArrayItem("String", IsNullable = false)]
		public string[] UserEnabledExtensions { get; set; }

		[XmlArrayItem("String", IsNullable = false)]
		public string[] UserDisabledExtensions { get; set; }

		internal bool IsEnabledByUser(string extensionId)
		{
			if (this.UserEnabledExtensions == null || this.UserEnabledExtensions.Length == 0)
			{
				return false;
			}
			if (this.userEnabledExtensionsHashSet == null)
			{
				this.userEnabledExtensionsHashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (string extensionId2 in this.UserEnabledExtensions)
				{
					this.userEnabledExtensionsHashSet.Add(ExtensionDataHelper.FormatExtensionId(extensionId2));
				}
			}
			return this.userEnabledExtensionsHashSet.Contains(ExtensionDataHelper.FormatExtensionId(extensionId));
		}

		internal bool IsDisabledByUser(string extensionId)
		{
			if (this.UserDisabledExtensions == null || this.UserDisabledExtensions.Length == 0)
			{
				return false;
			}
			if (this.userDisabledExtensionsHashSet == null)
			{
				this.userDisabledExtensionsHashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (string extensionId2 in this.UserDisabledExtensions)
				{
					this.userDisabledExtensionsHashSet.Add(ExtensionDataHelper.FormatExtensionId(extensionId2));
				}
			}
			return this.userDisabledExtensionsHashSet.Contains(ExtensionDataHelper.FormatExtensionId(extensionId));
		}

		private HashSet<string> userEnabledExtensionsHashSet;

		private HashSet<string> userDisabledExtensionsHashSet;
	}
}
