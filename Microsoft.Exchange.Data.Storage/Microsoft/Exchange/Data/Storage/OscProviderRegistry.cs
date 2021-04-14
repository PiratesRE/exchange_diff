using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class OscProviderRegistry
	{
		internal static Guid GetGuidFromName(string name)
		{
			Guid result;
			if (!OscProviderRegistry.TryGetGuidFromName(name, out result))
			{
				throw new ArgumentException("Unknown provider", "name");
			}
			return result;
		}

		internal static bool TryGetGuidFromName(string name, out Guid providerGuid)
		{
			OscProviderRegistry.OscProviderInfo oscProviderInfo;
			if (OscProviderRegistry.NameToInfoMap.TryGetValue(name, out oscProviderInfo))
			{
				providerGuid = oscProviderInfo.ProviderId;
				return true;
			}
			if (Guid.TryParse(name, out providerGuid))
			{
				return true;
			}
			providerGuid = Guid.Empty;
			return false;
		}

		internal static bool TryGetNameFromGuid(Guid providerGuid, out string name)
		{
			OscProviderRegistry.OscProviderInfo oscProviderInfo;
			if (OscProviderRegistry.GuidToInfoMap.TryGetValue(providerGuid, out oscProviderInfo))
			{
				name = oscProviderInfo.Name;
				return true;
			}
			name = string.Empty;
			return false;
		}

		internal static bool TryGetNetworkId(string provider, out string networkId)
		{
			OscProviderRegistry.OscProviderInfo oscProviderInfo;
			if (OscProviderRegistry.NameToInfoMap.TryGetValue(provider, out oscProviderInfo))
			{
				if (string.IsNullOrWhiteSpace(oscProviderInfo.NetworkId))
				{
					networkId = string.Empty;
					return false;
				}
				networkId = oscProviderInfo.NetworkId;
				return true;
			}
			else
			{
				Guid key;
				if (!Guid.TryParse(provider, out key) || !OscProviderRegistry.GuidToInfoMap.TryGetValue(key, out oscProviderInfo))
				{
					networkId = string.Empty;
					return false;
				}
				if (string.IsNullOrWhiteSpace(oscProviderInfo.NetworkId))
				{
					networkId = string.Empty;
					return false;
				}
				networkId = oscProviderInfo.NetworkId;
				return true;
			}
		}

		internal static DefaultFolderType GetParentFolder(Guid provider)
		{
			return DefaultFolderType.Root;
		}

		internal static string GetDefaultFolderDisplayName(Guid provider)
		{
			OscProviderRegistry.OscProviderInfo oscProviderInfo;
			if (!OscProviderRegistry.GuidToInfoMap.TryGetValue(provider, out oscProviderInfo))
			{
				throw new ArgumentException("Unknown provider", "provider");
			}
			if (string.IsNullOrWhiteSpace(oscProviderInfo.DefaultFolderDisplayName))
			{
				throw new ArgumentException("Unknown folder display name", "provider");
			}
			return oscProviderInfo.DefaultFolderDisplayName;
		}

		internal static bool TryGetDefaultFolderDisplayName(Guid provider, out string defaultDisplayName)
		{
			OscProviderRegistry.OscProviderInfo oscProviderInfo;
			if (!OscProviderRegistry.GuidToInfoMap.TryGetValue(provider, out oscProviderInfo))
			{
				defaultDisplayName = string.Empty;
				return false;
			}
			if (string.IsNullOrWhiteSpace(oscProviderInfo.DefaultFolderDisplayName))
			{
				defaultDisplayName = string.Empty;
				return false;
			}
			defaultDisplayName = oscProviderInfo.DefaultFolderDisplayName;
			return true;
		}

		private static readonly OscProviderRegistry.OscProviderInfo FacebookInfo = new OscProviderRegistry.OscProviderInfo
		{
			Name = "Facebook",
			ProviderId = OscProviderGuids.Facebook,
			NetworkId = "",
			DefaultFolderDisplayName = "Facebook"
		};

		private static readonly OscProviderRegistry.OscProviderInfo LinkedInInfo = new OscProviderRegistry.OscProviderInfo
		{
			Name = "LinkedIn",
			ProviderId = OscProviderGuids.LinkedIn,
			NetworkId = "linkedin",
			DefaultFolderDisplayName = "LinkedIn"
		};

		private static readonly OscProviderRegistry.OscProviderInfo SharePointInfo = new OscProviderRegistry.OscProviderInfo
		{
			Name = "SharePoint",
			ProviderId = OscProviderGuids.SharePoint
		};

		private static readonly OscProviderRegistry.OscProviderInfo WindowsLiveInfo = new OscProviderRegistry.OscProviderInfo
		{
			Name = "WindowsLive",
			ProviderId = OscProviderGuids.WindowsLive
		};

		private static readonly OscProviderRegistry.OscProviderInfo XingInfo = new OscProviderRegistry.OscProviderInfo
		{
			Name = "XING",
			ProviderId = OscProviderGuids.Xing,
			NetworkId = "xing",
			DefaultFolderDisplayName = "XING"
		};

		private static readonly Dictionary<string, OscProviderRegistry.OscProviderInfo> NameToInfoMap = new Dictionary<string, OscProviderRegistry.OscProviderInfo>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"Facebook",
				OscProviderRegistry.FacebookInfo
			},
			{
				"LinkedIn",
				OscProviderRegistry.LinkedInInfo
			},
			{
				"SharePoint",
				OscProviderRegistry.SharePointInfo
			},
			{
				"WindowsLive",
				OscProviderRegistry.WindowsLiveInfo
			},
			{
				"XING",
				OscProviderRegistry.XingInfo
			}
		};

		private static readonly Dictionary<Guid, OscProviderRegistry.OscProviderInfo> GuidToInfoMap = new Dictionary<Guid, OscProviderRegistry.OscProviderInfo>
		{
			{
				OscProviderGuids.LinkedIn,
				OscProviderRegistry.LinkedInInfo
			},
			{
				OscProviderGuids.Facebook,
				OscProviderRegistry.FacebookInfo
			},
			{
				OscProviderGuids.SharePoint,
				OscProviderRegistry.SharePointInfo
			},
			{
				OscProviderGuids.WindowsLive,
				OscProviderRegistry.WindowsLiveInfo
			},
			{
				OscProviderGuids.Xing,
				OscProviderRegistry.XingInfo
			}
		};

		private class OscProviderInfo
		{
			internal string Name { get; set; }

			internal Guid ProviderId { get; set; }

			internal string NetworkId { get; set; }

			internal string DefaultFolderDisplayName { get; set; }
		}
	}
}
