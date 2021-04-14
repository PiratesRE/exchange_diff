using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EasExtensionCapabilities : ServerCapabilities
	{
		internal EasExtensionCapabilities()
		{
		}

		internal EasExtensionCapabilities(IEnumerable<string> capabilities)
		{
			foreach (string text in capabilities)
			{
				if (text.StartsWith("1="))
				{
					string s = text.Substring("1=".Length, text.Length - "1=".Length);
					int num;
					if (int.TryParse(s, NumberStyles.HexNumber, null, out num))
					{
						foreach (KeyValuePair<EasExtensionsVersion1, string> keyValuePair in EasExtensionCapabilities.CapabilitiesMap)
						{
							EasExtensionsVersion1 key = keyValuePair.Key;
							if ((num & (int)key) != 0)
							{
								base.Add(keyValuePair.Value);
							}
						}
						base.Add("1=");
					}
				}
			}
		}

		internal bool SupportsVersion1
		{
			get
			{
				return base.Supports("1=");
			}
		}

		internal bool SupportsExtensions(EasExtensionsVersion1 extension)
		{
			foreach (KeyValuePair<EasExtensionsVersion1, string> keyValuePair in EasExtensionCapabilities.CapabilitiesMap)
			{
				EasExtensionsVersion1 key = keyValuePair.Key;
				if (extension.HasFlag(key) && !base.Supports(keyValuePair.Value))
				{
					return false;
				}
			}
			return true;
		}

		internal string RequestExtensions(EasExtensionsVersion1 extension)
		{
			string str = "1=";
			int num = (int)extension;
			return str + num.ToString("X4");
		}

		internal const string HeaderExtensionString = "X-OLK-Extension";

		private const string FolderTypesCapability = "FolderTypes";

		private const string SystemCategoriesCapability = "SystemCategories";

		private const string DefaultFromAddressCapability = "DefaultFromAddress";

		private const string ArchiveCapability = "Archive";

		private const string UnsubscribeCapability = "Unsubscribe";

		private const string MessageUploadCapability = "MessageUpload";

		private const string AdvanedSearchCapability = "AdvanedSearch";

		private const string PicwDataCapability = "PicwData";

		private const string TrueMessageReadCapability = "TrueMessageRead";

		private const string RulesCapability = "Rules";

		private const string ExtendedDateFiltersCapability = "ExtendedDateFilters";

		private const string SmsExtensionsCapability = "SmsExtensions";

		private const string ActionableSearchCapability = "ActionableSearch";

		private const string FolderPermissionCapability = "FolderPermission";

		private const string FolderExtensionTypeCapability = "FolderExtensionType";

		private const string VoiceMailExtensionCapability = "VoiceMailExtension";

		private const string Version1Capability = "1=";

		private static readonly Dictionary<EasExtensionsVersion1, string> CapabilitiesMap = new Dictionary<EasExtensionsVersion1, string>
		{
			{
				EasExtensionsVersion1.FolderTypes,
				"FolderTypes"
			},
			{
				EasExtensionsVersion1.SystemCategories,
				"SystemCategories"
			},
			{
				EasExtensionsVersion1.DefaultFromAddress,
				"DefaultFromAddress"
			},
			{
				EasExtensionsVersion1.Archive,
				"Archive"
			},
			{
				EasExtensionsVersion1.Unsubscribe,
				"Unsubscribe"
			},
			{
				EasExtensionsVersion1.MessageUpload,
				"MessageUpload"
			},
			{
				EasExtensionsVersion1.AdvanedSearch,
				"AdvanedSearch"
			},
			{
				EasExtensionsVersion1.PicwData,
				"PicwData"
			},
			{
				EasExtensionsVersion1.TrueMessageRead,
				"TrueMessageRead"
			},
			{
				EasExtensionsVersion1.Rules,
				"Rules"
			},
			{
				EasExtensionsVersion1.ExtendedDateFilters,
				"ExtendedDateFilters"
			},
			{
				EasExtensionsVersion1.SmsExtensions,
				"SmsExtensions"
			},
			{
				EasExtensionsVersion1.ActionableSearch,
				"ActionableSearch"
			},
			{
				EasExtensionsVersion1.FolderPermission,
				"FolderPermission"
			},
			{
				EasExtensionsVersion1.FolderExtensionType,
				"FolderExtensionType"
			},
			{
				EasExtensionsVersion1.VoiceMailExtension,
				"VoiceMailExtension"
			}
		};
	}
}
