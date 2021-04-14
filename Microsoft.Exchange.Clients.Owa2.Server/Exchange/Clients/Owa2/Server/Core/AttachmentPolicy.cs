using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AttachmentPolicy
	{
		internal AttachmentPolicy(string[] blockFileTypes, string[] blockMimeTypes, string[] forceSaveFileTypes, string[] forceSaveMimeTypes, string[] allowFileTypes, string[] allowMimeTypes, AttachmentPolicyLevel treatUnknownTypeAs, bool directFileAccessOnPublicComputersEnabled, bool directFileAccessOnPrivateComputersEnabled, bool forceWacViewingFirstOnPublicComputers, bool forceWacViewingFirstOnPrivateComputers, bool wacViewingOnPublicComputersEnabled, bool wacViewingOnPrivateComputersEnabled, bool forceWebReadyDocumentViewingFirstOnPublicComputers, bool forceWebReadyDocumentViewingFirstOnPrivateComputers, bool webReadyDocumentViewingOnPublicComputersEnabled, bool webReadyDocumentViewingOnPrivateComputersEnabled, string[] webReadyFileTypes, string[] webReadyMimeTypes, string[] webReadyDocumentViewingSupportedFileTypes, string[] webReadyDocumentViewingSupportedMimeTypes, bool webReadyDocumentViewingForAllSupportedTypes)
		{
			this.treatUnknownTypeAs = treatUnknownTypeAs;
			this.directFileAccessOnPublicComputersEnabled = directFileAccessOnPublicComputersEnabled;
			this.directFileAccessOnPrivateComputersEnabled = directFileAccessOnPrivateComputersEnabled;
			this.forceWacViewingFirstOnPublicComputers = forceWacViewingFirstOnPublicComputers;
			this.forceWacViewingFirstOnPrivateComputers = forceWacViewingFirstOnPrivateComputers;
			this.wacViewingOnPublicComputersEnabled = wacViewingOnPublicComputersEnabled;
			this.wacViewingOnPrivateComputersEnabled = wacViewingOnPrivateComputersEnabled;
			this.forceWebReadyDocumentViewingFirstOnPublicComputers = forceWebReadyDocumentViewingFirstOnPublicComputers;
			this.forceWebReadyDocumentViewingFirstOnPrivateComputers = forceWebReadyDocumentViewingFirstOnPrivateComputers;
			this.webReadyDocumentViewingOnPublicComputersEnabled = webReadyDocumentViewingOnPublicComputersEnabled;
			this.webReadyDocumentViewingOnPrivateComputersEnabled = webReadyDocumentViewingOnPrivateComputersEnabled;
			this.webReadyDocumentViewingForAllSupportedTypes = webReadyDocumentViewingForAllSupportedTypes;
			this.webReadyFileTypes = webReadyFileTypes;
			Array.Sort<string>(this.webReadyFileTypes);
			this.webReadyMimeTypes = webReadyMimeTypes;
			Array.Sort<string>(this.webReadyMimeTypes);
			this.webReadyDocumentViewingSupportedFileTypes = webReadyDocumentViewingSupportedFileTypes;
			Array.Sort<string>(this.webReadyDocumentViewingSupportedFileTypes);
			this.webReadyDocumentViewingSupportedMimeTypes = webReadyDocumentViewingSupportedMimeTypes;
			Array.Sort<string>(this.webReadyDocumentViewingSupportedMimeTypes);
			this.fileTypeLevels = AttachmentPolicy.LoadDictionary(blockFileTypes, forceSaveFileTypes, allowFileTypes);
			this.mimeTypeLevels = AttachmentPolicy.LoadDictionary(blockMimeTypes, forceSaveMimeTypes, allowMimeTypes);
			this.blockedFileTypes = blockFileTypes;
			this.blockedMimeTypes = blockMimeTypes;
			this.forceSaveFileTypes = forceSaveFileTypes;
			this.forceSaveMimeTypes = forceSaveMimeTypes;
			this.allowedFileTypes = allowFileTypes;
			this.allowedMimeTypes = allowMimeTypes;
			this.policyData = new OwaAttachmentPolicyData
			{
				AllowFileTypes = this.allowedFileTypes,
				AllowMimeTypes = this.allowedMimeTypes,
				BlockFileTypes = this.blockedFileTypes,
				BlockMimeTypes = this.blockedMimeTypes,
				DirectFileAccessOnPrivateComputersEnabled = directFileAccessOnPrivateComputersEnabled,
				DirectFileAccessOnPublicComputersEnabled = directFileAccessOnPublicComputersEnabled,
				ForceSaveFileTypes = forceSaveFileTypes,
				ForceSaveMimeTypes = forceSaveMimeTypes,
				ForceWacViewingFirstOnPrivateComputers = forceWacViewingFirstOnPrivateComputers,
				ForceWacViewingFirstOnPublicComputers = forceWacViewingFirstOnPublicComputers,
				ForceWebReadyDocumentViewingFirstOnPrivateComputers = forceWebReadyDocumentViewingFirstOnPrivateComputers,
				ForceWebReadyDocumentViewingFirstOnPublicComputers = forceWebReadyDocumentViewingFirstOnPublicComputers,
				TreatUnknownTypeAs = treatUnknownTypeAs.ToString(),
				WacViewingOnPrivateComputersEnabled = wacViewingOnPrivateComputersEnabled,
				WacViewingOnPublicComputersEnabled = wacViewingOnPublicComputersEnabled,
				WebReadyDocumentViewingForAllSupportedTypes = webReadyDocumentViewingForAllSupportedTypes,
				WebReadyDocumentViewingOnPrivateComputersEnabled = webReadyDocumentViewingOnPrivateComputersEnabled,
				WebReadyDocumentViewingOnPublicComputersEnabled = webReadyDocumentViewingOnPublicComputersEnabled,
				WebReadyDocumentViewingSupportedFileTypes = webReadyDocumentViewingSupportedFileTypes,
				WebReadyDocumentViewingSupportedMimeTypes = webReadyDocumentViewingSupportedMimeTypes,
				WebReadyFileTypes = webReadyFileTypes,
				WebReadyMimeTypes = webReadyMimeTypes
			};
		}

		internal AttachmentPolicyLevel TreatUnknownTypeAs
		{
			get
			{
				return this.treatUnknownTypeAs;
			}
		}

		internal OwaAttachmentPolicyData PolicyData
		{
			get
			{
				return this.policyData;
			}
		}

		internal bool GetDirectFileAccessEnabled(bool isPublicLogon)
		{
			if (isPublicLogon)
			{
				return this.directFileAccessOnPublicComputersEnabled;
			}
			return this.directFileAccessOnPrivateComputersEnabled;
		}

		internal bool GetWacViewingEnabled(bool isPublicLogon)
		{
			if (isPublicLogon)
			{
				return this.wacViewingOnPublicComputersEnabled;
			}
			return this.wacViewingOnPrivateComputersEnabled;
		}

		internal bool GetForceWacViewingFirstEnabled(bool isPublicLogon)
		{
			if (isPublicLogon)
			{
				return this.forceWacViewingFirstOnPublicComputers;
			}
			return this.forceWacViewingFirstOnPrivateComputers;
		}

		internal AttachmentPolicyLevel GetLevel(string attachmentType, AttachmentPolicy.TypeSignifier typeSignifier)
		{
			AttachmentPolicyLevel result = AttachmentPolicyLevel.Unknown;
			switch (typeSignifier)
			{
			case AttachmentPolicy.TypeSignifier.File:
				result = AttachmentPolicy.FindLevel(this.fileTypeLevels, attachmentType);
				break;
			case AttachmentPolicy.TypeSignifier.Mime:
				result = AttachmentPolicy.FindLevel(this.mimeTypeLevels, attachmentType);
				break;
			}
			return result;
		}

		internal AttachmentPolicyType CreateAttachmentPolicyType(UserContext userContext, UserAgent userAgent, WacConfigData wacData)
		{
			AttachmentPolicyType attachmentPolicyType = new AttachmentPolicyType();
			attachmentPolicyType.DirectFileAccessOnPublicComputersEnabled = this.directFileAccessOnPublicComputersEnabled;
			attachmentPolicyType.DirectFileAccessOnPrivateComputersEnabled = this.directFileAccessOnPrivateComputersEnabled;
			attachmentPolicyType.AllowedFileTypes = this.allowedFileTypes;
			attachmentPolicyType.AllowedMimeTypes = this.allowedMimeTypes;
			attachmentPolicyType.BlockedFileTypes = this.blockedFileTypes;
			attachmentPolicyType.BlockedMimeTypes = this.blockedMimeTypes;
			attachmentPolicyType.ForceSaveFileTypes = this.forceSaveFileTypes;
			attachmentPolicyType.ForceSaveMimeTypes = this.forceSaveMimeTypes;
			attachmentPolicyType.ActionForUnknownFileAndMIMETypes = this.treatUnknownTypeAs.ToString();
			if (userAgent != null && (string.Equals(userAgent.Platform, "iPhone", StringComparison.OrdinalIgnoreCase) || string.Equals(userAgent.Platform, "iPad", StringComparison.OrdinalIgnoreCase)))
			{
				string[] collection = new string[]
				{
					".dotm",
					".ppsm",
					".pptm",
					".xlsb",
					".xlsm",
					".wma"
				};
				List<string> list = new List<string>();
				list.AddRange(this.blockedFileTypes);
				list.AddRange(collection);
				attachmentPolicyType.BlockedFileTypes = list.ToArray();
			}
			attachmentPolicyType.AttachmentDataProviderAvailable = AttachmentPolicy.IsAttachmentDataProviderAvailable(wacData);
			userContext.SetWacEditingEnabled(wacData);
			attachmentPolicyType.WacViewableFileTypes = wacData.WacViewableFileTypes;
			attachmentPolicyType.WacEditableFileTypes = (userContext.IsWacEditingEnabled ? wacData.WacEditableFileTypes : new string[0]);
			attachmentPolicyType.WacViewingOnPublicComputersEnabled = this.wacViewingOnPublicComputersEnabled;
			attachmentPolicyType.WacViewingOnPrivateComputersEnabled = this.wacViewingOnPrivateComputersEnabled;
			attachmentPolicyType.ForceWacViewingFirstOnPublicComputers = this.forceWacViewingFirstOnPublicComputers;
			attachmentPolicyType.ForceWacViewingFirstOnPrivateComputers = this.forceWacViewingFirstOnPrivateComputers;
			attachmentPolicyType.ForceWebReadyDocumentViewingFirstOnPublicComputers = this.forceWebReadyDocumentViewingFirstOnPublicComputers;
			attachmentPolicyType.ForceWebReadyDocumentViewingFirstOnPrivateComputers = this.forceWebReadyDocumentViewingFirstOnPrivateComputers;
			attachmentPolicyType.WebReadyDocumentViewingOnPublicComputersEnabled = this.webReadyDocumentViewingOnPublicComputersEnabled;
			attachmentPolicyType.WebReadyDocumentViewingOnPrivateComputersEnabled = this.webReadyDocumentViewingOnPrivateComputersEnabled;
			attachmentPolicyType.WebReadyFileTypes = this.webReadyFileTypes;
			attachmentPolicyType.WebReadyMimeTypes = this.webReadyMimeTypes;
			attachmentPolicyType.WebReadyDocumentViewingSupportedFileTypes = this.webReadyDocumentViewingSupportedFileTypes;
			attachmentPolicyType.WebReadyDocumentViewingSupportedMimeTypes = this.webReadyDocumentViewingSupportedMimeTypes;
			attachmentPolicyType.WebReadyDocumentViewingForAllSupportedTypes = this.webReadyDocumentViewingForAllSupportedTypes;
			attachmentPolicyType.DirectFileAccessOnPrivateComputersEnabled = this.directFileAccessOnPrivateComputersEnabled;
			attachmentPolicyType.DirectFileAccessOnPublicComputersEnabled = this.directFileAccessOnPublicComputersEnabled;
			return attachmentPolicyType;
		}

		internal static bool IsAttachmentDataProviderAvailable(WacConfigData wacData)
		{
			return !string.IsNullOrEmpty(wacData.OneDriveDocumentsUrl) || AttachmentDataProviderManager.IsMockDataProviderEnabled();
		}

		internal static WacConfigData ReadAggregatedWacData(UserContext userContext, UserConfigurationManager.IAggregationContext ctx)
		{
			return UserContextUtilities.ReadAggregatedType<WacConfigData>(ctx, "OWA.WacData", () => AttachmentPolicy.GetWacConfigData(userContext));
		}

		private static WacConfigData GetWacConfigData(UserContext userContext)
		{
			WacConfigData wacConfigData = new WacConfigData();
			if (userContext == null)
			{
				throw new NullReferenceException("Retail assert: UserContext is null.");
			}
			if (userContext.FeaturesManager == null)
			{
				throw new NullReferenceException("Retail assert: UserContext.FeaturesManager is null.");
			}
			if (WacConfiguration.Instance == null)
			{
				throw new NullReferenceException("Retail assert: WacConfiguration.Instance is null.");
			}
			if (WacDiscoveryManager.Instance == null)
			{
				throw new NullReferenceException("Retail assert: WacDiscoveryManager.Instance is null.");
			}
			if (WacDiscoveryManager.Instance.WacDiscoveryResult == null)
			{
				throw new NullReferenceException("Retail assert: WacDiscoveryManager.Instance.WacDiscoveryResult is null.");
			}
			if (!WacConfiguration.Instance.BlockWacViewingThroughUI)
			{
				if (userContext.FeaturesManager.ClientServerSettings.DocCollab.Enabled || userContext.FeaturesManager.ClientServerSettings.ModernAttachments.Enabled)
				{
					wacConfigData.IsWacEditingEnabled = WacConfiguration.Instance.EditingEnabled;
				}
				else
				{
					wacConfigData.IsWacEditingEnabled = false;
				}
				try
				{
					wacConfigData.WacViewableFileTypes = WacDiscoveryManager.Instance.WacDiscoveryResult.WacViewableFileTypes;
					wacConfigData.WacEditableFileTypes = WacDiscoveryManager.Instance.WacDiscoveryResult.WacEditableFileTypes;
					wacConfigData.WacDiscoverySucceeded = true;
				}
				catch (WacDiscoveryFailureException ex)
				{
					OwaDiagnostics.PublishMonitoringEventNotification(ExchangeComponent.OwaDependency.Name, "DocCollab", ex.Message, ResultSeverityLevel.Error);
					wacConfigData.WacViewableFileTypes = new string[0];
					wacConfigData.WacEditableFileTypes = new string[0];
					wacConfigData.WacDiscoverySucceeded = false;
					wacConfigData.IsWacEditingEnabled = false;
				}
			}
			if (userContext.IsBposUser)
			{
				string text;
				string text2;
				wacConfigData.OneDriveDiscoverySucceeded = OneDriveProAttachmentDataProvider.TryGetBposDocumentsInfoFromBox(userContext, CallContext.Current, out text, out text2);
				wacConfigData.OneDriveDocumentsUrl = (text ?? string.Empty);
				wacConfigData.OneDriveDocumentsDisplayName = (text2 ?? string.Empty);
			}
			else
			{
				wacConfigData.OneDriveDiscoverySucceeded = true;
				wacConfigData.OneDriveDocumentsUrl = string.Empty;
				wacConfigData.OneDriveDocumentsDisplayName = string.Empty;
			}
			return wacConfigData;
		}

		private static SortedDictionary<string, AttachmentPolicyLevel> LoadDictionary(string[] block, string[] forceSave, string[] allow)
		{
			string[][] array = new string[3][];
			AttachmentPolicyLevel[] array2 = new AttachmentPolicyLevel[3];
			array[1] = block;
			array[2] = forceSave;
			array[0] = allow;
			array2[1] = AttachmentPolicyLevel.Block;
			array2[2] = AttachmentPolicyLevel.ForceSave;
			array2[0] = AttachmentPolicyLevel.Allow;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					array[i] = new string[0];
				}
			}
			SortedDictionary<string, AttachmentPolicyLevel> sortedDictionary = new SortedDictionary<string, AttachmentPolicyLevel>(StringComparer.OrdinalIgnoreCase);
			for (int j = 0; j <= 2; j++)
			{
				for (int k = 0; k < array[j].Length; k++)
				{
					string key = array[j][k];
					if (!sortedDictionary.ContainsKey(key))
					{
						sortedDictionary.Add(key, array2[j]);
					}
				}
			}
			return sortedDictionary;
		}

		private static AttachmentPolicyLevel FindLevel(SortedDictionary<string, AttachmentPolicyLevel> dictionary, string attachmentType)
		{
			AttachmentPolicyLevel result;
			if (!dictionary.TryGetValue(attachmentType, out result))
			{
				return AttachmentPolicyLevel.Unknown;
			}
			return result;
		}

		private readonly OwaAttachmentPolicyData policyData;

		private readonly AttachmentPolicyLevel treatUnknownTypeAs;

		private readonly bool directFileAccessOnPrivateComputersEnabled;

		private readonly bool directFileAccessOnPublicComputersEnabled;

		private readonly bool forceWacViewingFirstOnPrivateComputers;

		private readonly bool forceWacViewingFirstOnPublicComputers;

		private readonly bool wacViewingOnPrivateComputersEnabled;

		private readonly bool wacViewingOnPublicComputersEnabled;

		private readonly bool forceWebReadyDocumentViewingFirstOnPublicComputers;

		private readonly bool forceWebReadyDocumentViewingFirstOnPrivateComputers;

		private readonly bool webReadyDocumentViewingOnPublicComputersEnabled;

		private readonly bool webReadyDocumentViewingOnPrivateComputersEnabled;

		private readonly bool webReadyDocumentViewingForAllSupportedTypes;

		private string[] webReadyFileTypes;

		private string[] webReadyMimeTypes;

		private string[] webReadyDocumentViewingSupportedFileTypes;

		private string[] webReadyDocumentViewingSupportedMimeTypes;

		private string[] allowedFileTypes;

		private string[] allowedMimeTypes;

		private string[] forceSaveFileTypes;

		private string[] forceSaveMimeTypes;

		private string[] blockedFileTypes;

		private string[] blockedMimeTypes;

		private SortedDictionary<string, AttachmentPolicyLevel> fileTypeLevels;

		private SortedDictionary<string, AttachmentPolicyLevel> mimeTypeLevels;

		public enum TypeSignifier
		{
			File,
			Mime
		}

		private enum LevelPrecedence
		{
			First,
			Allow = 0,
			Block,
			ForceSave,
			Last = 2
		}
	}
}
