using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal static class WsPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (WsPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in WsPerformanceCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchangeWS";

		private static readonly ExPerformanceCounter AddAggregatedAccountRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "AddAggregatedAccount Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddAggregatedAccountRequests = new ExPerformanceCounter("MSExchangeWS", "AddAggregatedAccount Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.AddAggregatedAccountRequestsPerSecond
		});

		public static readonly ExPerformanceCounter AddAggregatedAccountAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "AddAggregatedAccount Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter IsOffice365DomainRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "IsOffice365Domain Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalIsOffice365DomainRequests = new ExPerformanceCounter("MSExchangeWS", "IsOffice365Domain Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.IsOffice365DomainRequestsPerSecond
		});

		public static readonly ExPerformanceCounter IsOffice365DomainAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "IsOffice365Domain Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetAggregatedAccountRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetAggregatedAccount Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetAggregatedAccountRequests = new ExPerformanceCounter("MSExchangeWS", "GetAggregatedAccount Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetAggregatedAccountRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetAggregatedAccountAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetAggregatedAccount Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RemoveAggregatedAccountRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "RemoveAggregatedAccount Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveAggregatedAccountRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveAggregatedAccount Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.RemoveAggregatedAccountRequestsPerSecond
		});

		public static readonly ExPerformanceCounter RemoveAggregatedAccountAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "RemoveAggregatedAccount Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetAggregatedAccountRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetAggregatedAccount Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetAggregatedAccountRequests = new ExPerformanceCounter("MSExchangeWS", "SetAggregatedAccount Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetAggregatedAccountRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetAggregatedAccountAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetAggregatedAccount Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetItemRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetItemRequests = new ExPerformanceCounter("MSExchangeWS", "GetItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetItemRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ConvertIdRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ConvertId Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalConvertIdRequests = new ExPerformanceCounter("MSExchangeWS", "ConvertId Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ConvertIdRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ConvertIdAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ConvertId Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter TotalIdsConvertedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Ids Converted/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalIdsConverted = new ExPerformanceCounter("MSExchangeWS", "Total Number of Ids converted", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalIdsConvertedPerSecond
		});

		private static readonly ExPerformanceCounter CreateItemRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CreateItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateItemRequests = new ExPerformanceCounter("MSExchangeWS", "CreateItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CreateItemRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CreateItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CreateItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter PostModernGroupItem = new ExPerformanceCounter("MSExchangeWS", "PostModernGroupItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalPostModernGroupItemRequests = new ExPerformanceCounter("MSExchangeWS", "PostModernGroupItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.PostModernGroupItem
		});

		public static readonly ExPerformanceCounter PostModernGroupItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "PostModernGroupItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UpdateAndPostModernGroupItem = new ExPerformanceCounter("MSExchangeWS", "UpdateAndPostModernGroupItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateAndPostModernGroupItemRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateAndPostModernGroupItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UpdateAndPostModernGroupItem
		});

		public static readonly ExPerformanceCounter UpdateAndPostModernGroupItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UpdateAndPostModernGroupItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CreateResponseFromModernGroupRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CreateResponseFromModernGroup Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateResponseFromModernGroupRequests = new ExPerformanceCounter("MSExchangeWS", "CreateResponseFromModernGroup Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CreateResponseFromModernGroupRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CreateResponseFromModernGroupAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CreateResponseFromModernGroup Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UploadItemsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UploadItems Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUploadItemsRequests = new ExPerformanceCounter("MSExchangeWS", "UploadItems Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UploadItemsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UploadItemsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UploadItems Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UploadLargeItemRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UploadLargeItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUploadLargeItemRequests = new ExPerformanceCounter("MSExchangeWS", "UploadLargeItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UploadLargeItemRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UploadLargeItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UploadLargeItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ChunkUploadRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ChunkUpload Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalChunkUploadRequests = new ExPerformanceCounter("MSExchangeWS", "ChunkUpload Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ChunkUploadRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ChunkUploadAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ChunkUpload Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CompleteLargeItemUploadRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CompleteLargeItemUpload Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCompleteLargeItemUploadRequests = new ExPerformanceCounter("MSExchangeWS", "CompleteLargeItemUpload Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CompleteLargeItemUploadRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CompleteLargeItemUploadAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CompleteLargeItemUpload Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ExportItemsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ExportItems Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExportItemsRequests = new ExPerformanceCounter("MSExchangeWS", "ExportItems Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ExportItemsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ExportItemsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ExportItems Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter DeleteItemRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "DeleteItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteItemRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.DeleteItemRequestsPerSecond
		});

		public static readonly ExPerformanceCounter DeleteItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "DeleteItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UpdateItemRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UpdateItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateItemRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UpdateItemRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UpdateItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UpdateItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UpdateItemInRecoverableItemsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UpdateItemInRecoverableItems Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateItemInRecoverableItemsRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateItemInRecoverableItems Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UpdateItemInRecoverableItemsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UpdateItemInRecoverableItemsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UpdateItemInRecoverableItems Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter MarkAllItemsAsReadRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "MarkAllItemsAsRead Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMarkAllItemsAsReadRequests = new ExPerformanceCounter("MSExchangeWS", "MarkAllItemsAsRead Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.MarkAllItemsAsReadRequestsPerSecond
		});

		public static readonly ExPerformanceCounter MarkAllItemsAsReadAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "MarkAllItemsAsRead Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter MarkAsJunkRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "MarkAsJunk Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMarkAsJunkRequests = new ExPerformanceCounter("MSExchangeWS", "MarkAsJunk Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.MarkAsJunkRequestsPerSecond
		});

		public static readonly ExPerformanceCounter MarkAsJunkAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "MarkAsJunk Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMarkAsJunkSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "MarkAsJunk Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetClientExtensionRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetClientExtension Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetClientExtensionRequests = new ExPerformanceCounter("MSExchangeWS", "GetClientExtension Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetClientExtensionRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetClientExtensionAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetClientExtension Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetEncryptionConfigurationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetEncryptionConfiguration Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetEncryptionConfigurationRequests = new ExPerformanceCounter("MSExchangeWS", "GetEncryptionConfiguration Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetEncryptionConfigurationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetEncryptionConfigurationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetEncryptionConfiguration Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetClientExtensionRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetClientExtension Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetClientExtensionRequests = new ExPerformanceCounter("MSExchangeWS", "SetClientExtension Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetClientExtensionRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetClientExtensionAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetClientExtension Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetEncryptionConfigurationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetEncryptionConfiguration Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetEncryptionConfigurationRequests = new ExPerformanceCounter("MSExchangeWS", "SetEncryptionConfiguration Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetEncryptionConfigurationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetEncryptionConfigurationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetEncryptionConfiguration Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CreateUnifiedMailboxRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CreateUnifiedMailbox Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateUnifiedMailboxRequests = new ExPerformanceCounter("MSExchangeWS", "CreateUnifiedMailbox Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CreateUnifiedMailboxRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CreateUnifiedMailboxAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CreateUnifiedMailbox Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetAppManifestsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetAppManifests Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetAppManifestsRequests = new ExPerformanceCounter("MSExchangeWS", "GetAppManifests Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetAppManifestsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetAppManifestsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetAppManifests Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetClientExtensionTokenRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetClientExtensionToken Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetClientExtensionTokenRequests = new ExPerformanceCounter("MSExchangeWS", "GetClientExtensionToken Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetClientExtensionTokenRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetClientExtensionTokenAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetClientExtensionToken Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetEncryptionConfigurationTokenRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetEncryptionConfigurationToken Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetEncryptionConfigurationTokenRequests = new ExPerformanceCounter("MSExchangeWS", "GetEncryptionConfigurationToken Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetEncryptionConfigurationTokenRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetEncryptionConfigurationTokenAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetEncryptionConfigurationToken Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter InstallAppRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "InstallApp Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalInstallAppRequests = new ExPerformanceCounter("MSExchangeWS", "InstallApp Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.InstallAppRequestsPerSecond
		});

		public static readonly ExPerformanceCounter InstallAppAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "InstallApp Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UninstallAppRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UninstallApp Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUninstallAppRequests = new ExPerformanceCounter("MSExchangeWS", "UninstallApp Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UninstallAppRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UninstallAppAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UninstallApp Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter DisableAppRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "DisableApp Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDisableAppRequests = new ExPerformanceCounter("MSExchangeWS", "DisableApp Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.DisableAppRequestsPerSecond
		});

		public static readonly ExPerformanceCounter DisableAppAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "DisableApp Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetAppMarketplaceUrlRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetAppMarketplaceUrl Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetAppMarketplaceUrlRequests = new ExPerformanceCounter("MSExchangeWS", "GetAppMarketplaceUrl Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetAppMarketplaceUrlRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetAppMarketplaceUrlAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetAppMarketplaceUrl Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AddImContactToGroupRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "AddImContactToGroup Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddImContactToGroupRequests = new ExPerformanceCounter("MSExchangeWS", "AddImContactToGroup Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.AddImContactToGroupRequestsPerSecond
		});

		public static readonly ExPerformanceCounter AddImContactToGroupAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "AddImContactToGroup Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddImContactToGroupSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "AddImContactToGroup Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RemoveImContactFromGroupRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "RemoveImContactFromGroup Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveImContactFromGroupRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveImContactFromGroup Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.RemoveImContactFromGroupRequestsPerSecond
		});

		public static readonly ExPerformanceCounter RemoveImContactFromGroupAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "RemoveImContactFromGroup Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveImContactFromGroupSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveImContactFromGroup Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AddNewImContactToGroupRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "AddNewImContactToGroup Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddNewImContactToGroupRequests = new ExPerformanceCounter("MSExchangeWS", "AddNewImContactToGroup Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.AddNewImContactToGroupRequestsPerSecond
		});

		public static readonly ExPerformanceCounter AddNewImContactToGroupAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "AddNewImContactToGroup Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddNewImContactToGroupSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "AddNewImContactToGroup Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AddNewTelUriContactToGroupRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "AddNewTelUriContactToGroup Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddNewTelUriContactToGroupRequests = new ExPerformanceCounter("MSExchangeWS", "AddNewTelUriContactToGroup Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.AddNewTelUriContactToGroupRequestsPerSecond
		});

		public static readonly ExPerformanceCounter AddNewTelUriContactToGroupAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "AddNewTelUriContactToGroup Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddNewTelUriContactToGroupSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "AddNewTelUriContactToGroup Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AddDistributionGroupToImListRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "AddDistributionGroupToImList Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddDistributionGroupToImListRequests = new ExPerformanceCounter("MSExchangeWS", "AddDistributionGroupToImList Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.AddDistributionGroupToImListRequestsPerSecond
		});

		public static readonly ExPerformanceCounter AddDistributionGroupToImListAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "AddDistributionGroupToImList Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddDistributionGroupToImListSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "AddDistributionGroupToImList Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AddImGroupRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "AddImGroup Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddImGroupRequests = new ExPerformanceCounter("MSExchangeWS", "AddImGroup Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.AddImGroupRequestsPerSecond
		});

		public static readonly ExPerformanceCounter AddImGroupAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "AddImGroup Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddImGroupSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "AddImGroup Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetImItemListRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetImItemList Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetImItemListRequests = new ExPerformanceCounter("MSExchangeWS", "GetImItemList Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetImItemListRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetImItemListAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetImItemList Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetImItemListSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetImItemList Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetImItemsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetImItems Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetImItemsRequests = new ExPerformanceCounter("MSExchangeWS", "GetImItems Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetImItemsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetImItemsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetImItems Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetImItemsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetImItems Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RemoveContactFromImListRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "RemoveContactFromImList Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveContactFromImListRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveContactFromImList Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.RemoveContactFromImListRequestsPerSecond
		});

		public static readonly ExPerformanceCounter RemoveContactFromImListAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "RemoveContactFromImList Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveContactFromImListSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveContactFromImList Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RemoveDistributionGroupFromImListRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "RemoveDistributionGroupFromImList Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveDistributionGroupFromImListRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveDistributionGroupFromImList Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.RemoveDistributionGroupFromImListRequestsPerSecond
		});

		public static readonly ExPerformanceCounter RemoveDistributionGroupFromImListAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "RemoveDistributionGroupFromImList Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveDistributionGroupFromImListSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveDistributionGroupFromImList Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RemoveImGroupRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "RemoveImGroup Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveImGroupRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveImGroup Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.RemoveImGroupRequestsPerSecond
		});

		public static readonly ExPerformanceCounter RemoveImGroupAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "RemoveImGroup Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveImGroupSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveImGroup Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetImGroupRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetImGroup Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetImGroupRequests = new ExPerformanceCounter("MSExchangeWS", "SetImGroup Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetImGroupRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetImGroupAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetImGroup Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetImGroupSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetImGroup Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetImListMigrationCompletedRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetImListMigrationCompleted Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetImListMigrationCompletedRequests = new ExPerformanceCounter("MSExchangeWS", "SetImListMigrationCompleted Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetImListMigrationCompletedRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetImListMigrationCompletedAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetImListMigrationCompleted Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetImListMigrationCompletedSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetImListMigrationCompleted Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetConversationItemsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetConversationItems Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetConversationItemsRequests = new ExPerformanceCounter("MSExchangeWS", "GetConversationItems Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetConversationItemsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetConversationItemsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetConversationItems Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetModernConversationItemsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetModernConversationItems Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetModernConversationItemsRequests = new ExPerformanceCounter("MSExchangeWS", "GetModernConversationItems Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetModernConversationItemsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetModernConversationItemsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetModernConversationItems Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetThreadedConversationItemsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetThreadedConversationItems Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetThreadedConversationItemsRequests = new ExPerformanceCounter("MSExchangeWS", "GetThreadedConversationItems Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetThreadedConversationItemsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetThreadedConversationItemsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetThreadedConversationItems Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetModernConversationAttachmentsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetModernConversationAttachments Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetModernConversationAttachmentsRequests = new ExPerformanceCounter("MSExchangeWS", "GetModernConversationAttachments Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetModernConversationAttachmentsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetModernConversationAttachmentsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetModernConversationAttachments Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetModernGroupMembershipRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetModernGroupMembership Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetModernGroupMembershipRequests = new ExPerformanceCounter("MSExchangeWS", "SetModernGroupMembership Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetModernGroupMembershipRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetModernGroupMembershipAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetModernGroupMembership Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SendItemRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SendItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSendItemRequests = new ExPerformanceCounter("MSExchangeWS", "SendItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SendItemRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SendItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SendItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ArchiveItemRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ArchiveItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalArchiveItemRequests = new ExPerformanceCounter("MSExchangeWS", "ArchiveItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ArchiveItemRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ArchiveItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ArchiveItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter MoveItemRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "MoveItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMoveItemRequests = new ExPerformanceCounter("MSExchangeWS", "MoveItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.MoveItemRequestsPerSecond
		});

		public static readonly ExPerformanceCounter MoveItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "MoveItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CopyItemRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CopyItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCopyItemRequests = new ExPerformanceCounter("MSExchangeWS", "CopyItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CopyItemRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CopyItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CopyItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetFolderRequests = new ExPerformanceCounter("MSExchangeWS", "GetFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CreateFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CreateFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateFolderRequests = new ExPerformanceCounter("MSExchangeWS", "CreateFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CreateFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CreateFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CreateFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CreateFolderPathRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CreateFolderPath Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateFolderPathRequests = new ExPerformanceCounter("MSExchangeWS", "CreateFolderPath Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CreateFolderPathRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CreateFolderPathAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CreateFolderPath Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CreateManagedFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CreateManagedFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateManagedFolderRequests = new ExPerformanceCounter("MSExchangeWS", "CreateManagedFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CreateManagedFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CreateManagedFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CreateManagedFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter DeleteFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "DeleteFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteFolderRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.DeleteFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter DeleteFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "DeleteFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter EmptyFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "EmptyFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalEmptyFolderRequests = new ExPerformanceCounter("MSExchangeWS", "EmptyFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.EmptyFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter EmptyFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "EmptyFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UpdateFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UpdateFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateFolderRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UpdateFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UpdateFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UpdateFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter MoveFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "MoveFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMoveFolderRequests = new ExPerformanceCounter("MSExchangeWS", "MoveFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.MoveFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter MoveFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "MoveFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CopyFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CopyFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCopyFolderRequests = new ExPerformanceCounter("MSExchangeWS", "CopyFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CopyFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CopyFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CopyFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter FindItemRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "FindItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindItemRequests = new ExPerformanceCounter("MSExchangeWS", "FindItem Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.FindItemRequestsPerSecond
		});

		public static readonly ExPerformanceCounter FindItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "FindItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter FindFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "FindFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindFolderRequests = new ExPerformanceCounter("MSExchangeWS", "FindFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.FindFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter FindFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "FindFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ResolveNamesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ResolveNames Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalResolveNamesRequests = new ExPerformanceCounter("MSExchangeWS", "ResolveNames Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ResolveNamesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ResolveNamesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ResolveNames Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ExpandDLRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ExpandDL Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExpandDLRequests = new ExPerformanceCounter("MSExchangeWS", "ExpandDL Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ExpandDLRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ExpandDLAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ExpandDL Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetPasswordExpirationDateRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetPasswordExpirationDate Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetPasswordExpirationDateRequests = new ExPerformanceCounter("MSExchangeWS", "GetPasswordExpirationDate Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetPasswordExpirationDateRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetPasswordExpirationDateAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetPasswordExpirationDate Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetPasswordExpirationDateSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetPasswordExpirationDate Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CreateAttachmentRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CreateAttachment Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateAttachmentRequests = new ExPerformanceCounter("MSExchangeWS", "CreateAttachment Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CreateAttachmentRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CreateAttachmentAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CreateAttachment Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter DeleteAttachmentRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "DeleteAttachment Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteAttachmentRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteAttachment Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.DeleteAttachmentRequestsPerSecond
		});

		public static readonly ExPerformanceCounter DeleteAttachmentAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "DeleteAttachment Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetAttachmentRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetAttachment Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetAttachmentRequests = new ExPerformanceCounter("MSExchangeWS", "GetAttachment Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetAttachmentRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetAttachmentAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetAttachment Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetClientAccessTokenRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetClientAccessToken Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetClientAccessTokenRequests = new ExPerformanceCounter("MSExchangeWS", "GetClientAccessToken Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetClientAccessTokenRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetClientAccessTokenAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetClientAccessToken Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SubscribeRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "Subscribe Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSubscribeRequests = new ExPerformanceCounter("MSExchangeWS", "Subscribe Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SubscribeRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SubscribeAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "Subscribe Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UnsubscribeRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "Unsubscribe Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUnsubscribeRequests = new ExPerformanceCounter("MSExchangeWS", "Unsubscribe Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UnsubscribeRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UnsubscribeAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "Unsubscribe Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetStreamingEventsRequests = new ExPerformanceCounter("MSExchangeWS", "GetStreamingEvents Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetStreamingEventsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetStreamingEvents Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetStreamingEventsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetStreamingEvents Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetEventsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetEvents Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetEventsRequests = new ExPerformanceCounter("MSExchangeWS", "GetEvents Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetStreamingEventsRequestsPerSecond,
			WsPerformanceCounters.GetEventsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetEventsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetEvents Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetServiceConfigurationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetServiceConfiguration Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetServiceConfigurationRequests = new ExPerformanceCounter("MSExchangeWS", "GetServiceConfiguration Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetServiceConfigurationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetServiceConfigurationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetServiceConfiguration Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetMailTipsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetMailTips Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetMailTipsRequests = new ExPerformanceCounter("MSExchangeWS", "GetMailTips Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetMailTipsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetMailTipsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetMailTips Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SyncFolderHierarchyRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SyncFolderHierarchy Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSyncFolderHierarchyRequests = new ExPerformanceCounter("MSExchangeWS", "SyncFolderHierarchy Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SyncFolderHierarchyRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SyncFolderHierarchyAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SyncFolderHierarchy Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SyncFolderItemsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SyncFolderItems Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSyncFolderItemsRequests = new ExPerformanceCounter("MSExchangeWS", "SyncFolderItems Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SyncFolderItemsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SyncFolderItemsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SyncFolderItems Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetDelegateRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetDelegate Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetDelegateRequests = new ExPerformanceCounter("MSExchangeWS", "GetDelegate Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetDelegateRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetDelegateAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetDelegate Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AddDelegateRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "AddDelegate Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddDelegateRequests = new ExPerformanceCounter("MSExchangeWS", "AddDelegate Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.AddDelegateRequestsPerSecond
		});

		public static readonly ExPerformanceCounter AddDelegateAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "AddDelegate Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RemoveDelegateRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "RemoveDelegate Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveDelegateRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveDelegate Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.RemoveDelegateRequestsPerSecond
		});

		public static readonly ExPerformanceCounter RemoveDelegateAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "RemoveDelegate Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UpdateDelegateRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UpdateDelegate Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateDelegateRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateDelegate Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UpdateDelegateRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UpdateDelegateAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UpdateDelegate Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CreateUserConfigurationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CreateUserConfiguration Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateUserConfigurationRequests = new ExPerformanceCounter("MSExchangeWS", "CreateUserConfiguration Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CreateUserConfigurationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CreateUserConfigurationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CreateUserConfiguration Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUserConfigurationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUserConfiguration Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserConfigurationRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserConfiguration Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUserConfigurationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUserConfigurationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUserConfiguration Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UpdateUserConfigurationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UpdateUserConfiguration Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateUserConfigurationRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateUserConfiguration Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UpdateUserConfigurationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UpdateUserConfigurationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UpdateUserConfiguration Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter DeleteUserConfigurationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "DeleteUserConfiguration Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteUserConfigurationRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteUserConfiguration Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.DeleteUserConfigurationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter DeleteUserConfigurationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "DeleteUserConfiguration Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUserAvailabilityRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUserAvailability Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserAvailabilityRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserAvailability Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUserAvailabilityRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUserAvailabilityAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUserAvailability Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUserOofSettingsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUserOofSettings Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserOofSettingsRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserOofSettings Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUserOofSettingsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUserOofSettingsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUserOofSettings Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetUserOofSettingsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetUserOofSettings Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetUserOofSettingsRequests = new ExPerformanceCounter("MSExchangeWS", "SetUserOofSettings Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetUserOofSettingsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetUserOofSettingsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetUserOofSettings Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetSharingMetadataRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetSharingMetadata Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetSharingMetadataRequests = new ExPerformanceCounter("MSExchangeWS", "GetSharingMetadata Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetSharingMetadataRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetSharingMetadataAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetSharingMetadata Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RefreshSharingFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "RefreshSharingFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRefreshSharingFolderRequests = new ExPerformanceCounter("MSExchangeWS", "RefreshSharingFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.RefreshSharingFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter RefreshSharingFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "RefreshSharingFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetSharingFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetSharingFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetSharingFolderRequests = new ExPerformanceCounter("MSExchangeWS", "GetSharingFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetSharingFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetSharingFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetSharingFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetTeamMailboxRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetTeamMailbox Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetTeamMailboxRequests = new ExPerformanceCounter("MSExchangeWS", "SetTeamMailbox Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetTeamMailboxRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetTeamMailboxAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetTeamMailbox Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetTeamMailboxSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetTeamMailbox Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UnpinTeamMailboxRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UnpinTeamMailbox Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUnpinTeamMailboxRequests = new ExPerformanceCounter("MSExchangeWS", "UnpinTeamMailbox Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UnpinTeamMailboxRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UnpinTeamMailboxAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UnpinTeamMailbox Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUnpinTeamMailboxSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UnpinTeamMailbox Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetRoomListsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetRoomLists Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetRoomListsRequests = new ExPerformanceCounter("MSExchangeWS", "GetRoomLists Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetRoomListsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetRoomListsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetRoomLists Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SubscribeToPushNotificationPerSecond = new ExPerformanceCounter("MSExchangeWS", "SubscribeToPushNotification Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SubscribeToPushNotificationRequests = new ExPerformanceCounter("MSExchangeWS", "SubscribeToPushNotification Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SubscribeToPushNotificationPerSecond
		});

		public static readonly ExPerformanceCounter SubscribeToPushNotificationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SubscribeToPushNotification Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RequestDeviceRegistrationChallengePerSecond = new ExPerformanceCounter("MSExchangeWS", "Push Notification Registration Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RequestDeviceRegistrationChallengeRequest = new ExPerformanceCounter("MSExchangeWS", "RequestDeviceRegistrationChallenge Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.RequestDeviceRegistrationChallengePerSecond
		});

		public static readonly ExPerformanceCounter RequestDeviceRegistrationChallengeAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "RequestDeviceRegistrationChallenge Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeviceRegistrationChallengeRequestSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "RequestDeviceRegistrationChallenge Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UnsubscribeToPushNotificationPerSecond = new ExPerformanceCounter("MSExchangeWS", "UnsubscribeToPushNotification Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UnsubscribeToPushNotificationRequests = new ExPerformanceCounter("MSExchangeWS", "UnsubscribeToPushNotification Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UnsubscribeToPushNotificationPerSecond
		});

		public static readonly ExPerformanceCounter UnsubscribeToPushNotificationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UnsubscribeToPushNotification Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetRoomsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetRooms Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetRoomsRequests = new ExPerformanceCounter("MSExchangeWS", "GetRooms Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetRoomsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetRoomsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetRooms Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter PerformReminderActionRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "PerformReminderAction Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalPerformReminderActionRequests = new ExPerformanceCounter("MSExchangeWS", "PerformReminderAction Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.PerformReminderActionRequestsPerSecond
		});

		public static readonly ExPerformanceCounter PerformReminderActionAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "PerformReminderAction Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetRemindersRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetReminders Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetRemindersRequests = new ExPerformanceCounter("MSExchangeWS", "GetReminders Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetRemindersRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetRemindersAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetReminders Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ProvisionRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "Provision Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalProvisionRequests = new ExPerformanceCounter("MSExchangeWS", "Provision Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ProvisionRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ProvisionAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "Provision Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter LogPushNotificationDataRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "LogPushNotificationData Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalLogPushNotificationDataRequests = new ExPerformanceCounter("MSExchangeWS", "LogPushNotificationData Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.LogPushNotificationDataRequestsPerSecond
		});

		public static readonly ExPerformanceCounter LogPushNotificationDataAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "LogPushNotificationData Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter DeprovisionRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "Deprovision Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeprovisionRequests = new ExPerformanceCounter("MSExchangeWS", "Deprovision Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.DeprovisionRequestsPerSecond
		});

		public static readonly ExPerformanceCounter DeprovisionAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "Deprovision Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter FindConversationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "FindConversation Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindConversationRequests = new ExPerformanceCounter("MSExchangeWS", "FindConversation Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.FindConversationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter FindConversationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "FindConversation Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter FindPeopleRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "FindPeople Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindPeopleRequests = new ExPerformanceCounter("MSExchangeWS", "FindPeople Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.FindPeopleRequestsPerSecond
		});

		public static readonly ExPerformanceCounter FindPeopleAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "FindPeople Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SyncPeopleRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SyncPeople Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSyncPeopleRequests = new ExPerformanceCounter("MSExchangeWS", "SyncPeople Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SyncPeopleRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SyncPeopleAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SyncPeople Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SyncAutoCompleteRecipientsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SyncAutoCompleteRecipients Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSyncAutoCompleteRecipientsRequests = new ExPerformanceCounter("MSExchangeWS", "SyncAutoCompleteRecipients Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SyncAutoCompleteRecipientsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SyncAutoCompleteRecipientsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SyncAutoCompleteRecipients Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetPersonaRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetPersona Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetPersonaRequests = new ExPerformanceCounter("MSExchangeWS", "GetPersona Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetPersonaRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetPersonaAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetPersona Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SyncConversationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SyncConversation Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSyncConversationRequests = new ExPerformanceCounter("MSExchangeWS", "SyncConversation Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SyncConversationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SyncConversationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SyncConversation Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetTimeZoneOffsetsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetTimeZoneOffsets Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetTimeZoneOffsetsRequests = new ExPerformanceCounter("MSExchangeWS", "GetTimeZoneOffsets Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetTimeZoneOffsetsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetTimeZoneOffsetsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetTimeZoneOffsets Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalTimeZoneOffsetsTablesBuilt = new ExPerformanceCounter("MSExchangeWS", "TimeZoneOffsets Tables Built", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter FindMailboxStatisticsByKeywordsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "FindMailboxStatisticsByKeywords Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindMailboxStatisticsByKeywordsRequests = new ExPerformanceCounter("MSExchangeWS", "FindMailboxStatisticsByKeywords Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.FindMailboxStatisticsByKeywordsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter FindMailboxStatisticsByKeywordsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "FindMailboxStatisticsByKeywords Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetSearchableMailboxesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetSearchableMailboxes Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetSearchableMailboxesRequests = new ExPerformanceCounter("MSExchangeWS", "GetSearchableMailboxes Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetSearchableMailboxesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetSearchableMailboxesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetSearchableMailboxes Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SearchMailboxesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SearchMailboxes Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSearchMailboxesRequests = new ExPerformanceCounter("MSExchangeWS", "SearchMailboxes Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SearchMailboxesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SearchMailboxesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SearchMailboxes Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetDiscoverySearchConfigurationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetDiscoverySearchConfiguration Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetDiscoverySearchConfigurationRequests = new ExPerformanceCounter("MSExchangeWS", "GetDiscoverySearchConfiguration Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetDiscoverySearchConfigurationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetDiscoverySearchConfigurationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetDiscoverySearchConfiguration Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetHoldOnMailboxesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetHoldOnMailboxes Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetHoldOnMailboxesRequests = new ExPerformanceCounter("MSExchangeWS", "GetHoldOnMailboxes Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetHoldOnMailboxesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetHoldOnMailboxesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetHoldOnMailboxes Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetHoldOnMailboxesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetHoldOnMailboxes Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetHoldOnMailboxesRequests = new ExPerformanceCounter("MSExchangeWS", "SetHoldOnMailboxes Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetHoldOnMailboxesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetHoldOnMailboxesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetHoldOnMailboxes Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetNonIndexableItemStatisticsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetNonIndexableItemStatistics Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetNonIndexableItemStatisticsRequests = new ExPerformanceCounter("MSExchangeWS", "GetNonIndexableItemStatistics Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetNonIndexableItemStatisticsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetNonIndexableItemStatisticsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetNonIndexableItemStatistics Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetNonIndexableItemDetailsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetNonIndexableItemDetails Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetNonIndexableItemDetailsRequests = new ExPerformanceCounter("MSExchangeWS", "GetNonIndexableItemDetails Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetNonIndexableItemDetailsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetNonIndexableItemDetailsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetNonIndexableItemDetails Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUserRetentionPolicyTagsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUserRetentionPolicyTags Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserRetentionPolicyTagsRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserRetentionPolicyTags Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUserRetentionPolicyTagsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUserRetentionPolicyTagsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUserRetentionPolicyTags Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter PlayOnPhoneRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "PlayOnPhone Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalPlayOnPhoneRequests = new ExPerformanceCounter("MSExchangeWS", "PlayOnPhone Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.PlayOnPhoneRequestsPerSecond
		});

		public static readonly ExPerformanceCounter PlayOnPhoneAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "PlayOnPhone Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetPhoneCallInformationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetPhoneCallInformation Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetPhoneCallInformationRequests = new ExPerformanceCounter("MSExchangeWS", "GetPhoneCallInformation Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetPhoneCallInformationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetPhoneCallInformationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetPhoneCallInformation Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter DisconnectPhoneCallRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "DisconnectPhoneCall Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDisconnectPhoneCallRequests = new ExPerformanceCounter("MSExchangeWS", "DisconnectPhoneCall Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.DisconnectPhoneCallRequestsPerSecond
		});

		public static readonly ExPerformanceCounter DisconnectPhoneCallAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "DisconnectPhoneCall Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CreateUMPromptRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CreateUMPrompt Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateUMPromptRequests = new ExPerformanceCounter("MSExchangeWS", "CreateUMPrompt Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CreateUMPromptRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CreateUMPromptAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CreateUMPrompt Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUMPromptRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUMPrompt Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMPromptRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMPrompt Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUMPromptRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUMPromptAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUMPrompt Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUMPromptNamesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUMPromptNames Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMPromptNamesRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMPromptNames Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUMPromptNamesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUMPromptNamesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUMPromptNames Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter DeleteUMPromptsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "DeleteUMPrompts Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteUMPromptsRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteUMPrompts Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.DeleteUMPromptsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter DeleteUMPromptsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "DeleteUMPrompts Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetServerTimeZonesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetServerTimeZones Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetServerTimeZonesRequests = new ExPerformanceCounter("MSExchangeWS", "GetServerTimeZones Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetServerTimeZonesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetServerTimeZonesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetServerTimeZones Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SendNotificationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SendNotification Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSendNotificationRequests = new ExPerformanceCounter("MSExchangeWS", "SendNotification Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SendNotificationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SendNotificationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SendNotification Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter FindMessageTrackingReportRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "FindMessageTrackingReport Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindMessageTrackingReportRequests = new ExPerformanceCounter("MSExchangeWS", "FindMessageTrackingReport Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.FindMessageTrackingReportRequestsPerSecond
		});

		public static readonly ExPerformanceCounter FindMessageTrackingReportAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "FindMessageTrackingReport Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetMessageTrackingReportRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetMessageTrackingReport Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetMessageTrackingReportRequests = new ExPerformanceCounter("MSExchangeWS", "GetMessageTrackingReport Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetMessageTrackingReportRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetMessageTrackingReportAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetMessageTrackingReport Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ApplyConversationActionRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ApplyConversationAction Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalApplyConversationActionRequests = new ExPerformanceCounter("MSExchangeWS", "ApplyConversationAction Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ApplyConversationActionRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ApplyConversationActionAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ApplyConversationAction Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ExecuteDiagnosticMethodRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ExecuteDiagnosticMethod Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExecuteDiagnosticMethodRequests = new ExPerformanceCounter("MSExchangeWS", "ExecuteDiagnosticMethod Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ExecuteDiagnosticMethodRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ExecuteDiagnosticMethodAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ExecuteDiagnosticMethod Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetInboxRulesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetInboxRules Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetInboxRulesRequests = new ExPerformanceCounter("MSExchangeWS", "GetInboxRules Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetInboxRulesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetInboxRulesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetInboxRules Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetInboxRulesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetInboxRules Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UpdateInboxRulesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UpdateInboxRules Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateInboxRulesRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateInboxRules Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UpdateInboxRulesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UpdateInboxRulesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UpdateInboxRules Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateInboxRulesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateInboxRules Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter IsUMEnabledRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "IsUMEnabled Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalIsUMEnabledRequests = new ExPerformanceCounter("MSExchangeWS", "IsUMEnabled Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.IsUMEnabledRequestsPerSecond
		});

		public static readonly ExPerformanceCounter IsUMEnabledAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "IsUMEnabled Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalIsUMEnabledSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "IsUMEnabled Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUMPropertiesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUMProperties Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMPropertiesRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMProperties Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUMPropertiesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUMPropertiesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUMProperties Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMPropertiesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMProperties Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetOofStatusRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetOofStatus Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetOofStatusRequests = new ExPerformanceCounter("MSExchangeWS", "SetOofStatus Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetOofStatusRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetOofStatusAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetOofStatus Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetOofStatusSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetOofStatus Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetPlayOnPhoneDialStringRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetPlayOnPhoneDialString Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetPlayOnPhoneDialStringRequests = new ExPerformanceCounter("MSExchangeWS", "SetPlayOnPhoneDialString Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetPlayOnPhoneDialStringRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetPlayOnPhoneDialStringAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetPlayOnPhoneDialString Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetPlayOnPhoneDialStringSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetPlayOnPhoneDialString Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetTelephoneAccessFolderEmailRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetTelephoneAccessFolderEmail Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetTelephoneAccessFolderEmailRequests = new ExPerformanceCounter("MSExchangeWS", "SetTelephoneAccessFolderEmail Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetTelephoneAccessFolderEmailRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetTelephoneAccessFolderEmailAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetTelephoneAccessFolderEmail Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetTelephoneAccessFolderEmailSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetTelephoneAccessFolderEmail Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetMissedCallNotificationEnabledRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetMissedCallNotificationEnabled Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetMissedCallNotificationEnabledRequests = new ExPerformanceCounter("MSExchangeWS", "SetMissedCallNotificationEnabled Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetMissedCallNotificationEnabledRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetMissedCallNotificationEnabledAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetMissedCallNotificationEnabled Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetMissedCallNotificationEnabledSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetMissedCallNotificationEnabled Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ResetPINRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ResetPIN Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalResetPINRequests = new ExPerformanceCounter("MSExchangeWS", "ResetPIN Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ResetPINRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ResetPINAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ResetPIN Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalResetPINSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ResetPIN Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetCallInfoRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetCallInfo Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetCallInfoRequests = new ExPerformanceCounter("MSExchangeWS", "GetCallInfo Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetCallInfoRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetCallInfoAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetCallInfo Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetCallInfoSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetCallInfo Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter DisconnectRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "Disconnect Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDisconnectRequests = new ExPerformanceCounter("MSExchangeWS", "Disconnect Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.DisconnectRequestsPerSecond
		});

		public static readonly ExPerformanceCounter DisconnectAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "Disconnect Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDisconnectSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "Disconnect Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter PlayOnPhoneGreetingRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "PlayOnPhoneGreeting Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalPlayOnPhoneGreetingRequests = new ExPerformanceCounter("MSExchangeWS", "PlayOnPhoneGreeting Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.PlayOnPhoneGreetingRequestsPerSecond
		});

		public static readonly ExPerformanceCounter PlayOnPhoneGreetingAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "PlayOnPhoneGreeting Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalPlayOnPhoneGreetingSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "PlayOnPhoneGreeting Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter StreamedEventsPerSecond = new ExPerformanceCounter("MSExchangeWS", "StreamedEvents/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalStreamedEvents = new ExPerformanceCounter("MSExchangeWS", "Total StreamedEvents", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.StreamedEventsPerSecond
		});

		private static readonly ExPerformanceCounter TotalRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRequests = new ExPerformanceCounter("MSExchangeWS", "Total Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalRequestsPerSecond
		});

		public static readonly ExPerformanceCounter AverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter TotalItemsCreatedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Items Created/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalItemsCreated = new ExPerformanceCounter("MSExchangeWS", "Items Created", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalItemsCreatedPerSecond
		});

		private static readonly ExPerformanceCounter TotalItemsDeletedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Items Deleted/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalItemsDeleted = new ExPerformanceCounter("MSExchangeWS", "Items Deleted", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalItemsDeletedPerSecond
		});

		private static readonly ExPerformanceCounter TotalItemsSentPerSecond = new ExPerformanceCounter("MSExchangeWS", "Items Sent/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalItemsSent = new ExPerformanceCounter("MSExchangeWS", "Items Sent", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalItemsSentPerSecond
		});

		private static readonly ExPerformanceCounter TotalItemsReadPerSecond = new ExPerformanceCounter("MSExchangeWS", "Items Read/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalItemsRead = new ExPerformanceCounter("MSExchangeWS", "Items Read", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalItemsReadPerSecond
		});

		private static readonly ExPerformanceCounter TotalItemsUpdatedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Items Updated/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalItemsUpdated = new ExPerformanceCounter("MSExchangeWS", "Items Updated", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalItemsUpdatedPerSecond
		});

		private static readonly ExPerformanceCounter TotalItemsCopiedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Items Copied/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalItemsCopied = new ExPerformanceCounter("MSExchangeWS", "Items Copied", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalItemsCopiedPerSecond
		});

		private static readonly ExPerformanceCounter TotalItemsMovedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Items Moved/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalItemsMoved = new ExPerformanceCounter("MSExchangeWS", "Items Moved", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalItemsMovedPerSecond
		});

		private static readonly ExPerformanceCounter TotalFoldersCreatedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Folders Created/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFoldersCreated = new ExPerformanceCounter("MSExchangeWS", "Folders Created", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalFoldersCreatedPerSecond
		});

		private static readonly ExPerformanceCounter TotalFoldersDeletedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Folders Deleted/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFoldersDeleted = new ExPerformanceCounter("MSExchangeWS", "Folders Deleted", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalFoldersDeletedPerSecond
		});

		private static readonly ExPerformanceCounter TotalFoldersReadPerSecond = new ExPerformanceCounter("MSExchangeWS", "Folders Sent/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFoldersRead = new ExPerformanceCounter("MSExchangeWS", "Folders Read", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalFoldersReadPerSecond
		});

		private static readonly ExPerformanceCounter TotalFoldersUpdatedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Folders Updated/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFoldersUpdated = new ExPerformanceCounter("MSExchangeWS", "Folders Updated", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalFoldersUpdatedPerSecond
		});

		private static readonly ExPerformanceCounter TotalFoldersCopiedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Folders Copied/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFoldersCopied = new ExPerformanceCounter("MSExchangeWS", "Folders Copied", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalFoldersCopiedPerSecond
		});

		private static readonly ExPerformanceCounter TotalFoldersMovedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Folders Moved/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFoldersMoved = new ExPerformanceCounter("MSExchangeWS", "Folders Moved", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalFoldersMovedPerSecond
		});

		private static readonly ExPerformanceCounter TotalFoldersSyncedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Folders Synced/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFoldersSynced = new ExPerformanceCounter("MSExchangeWS", "Folders Synchronized", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalFoldersSyncedPerSecond
		});

		private static readonly ExPerformanceCounter TotalItemsSyncedPerSecond = new ExPerformanceCounter("MSExchangeWS", "Items Synced/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalItemsSynced = new ExPerformanceCounter("MSExchangeWS", "Items Synchronized", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalItemsSyncedPerSecond
		});

		public static readonly ExPerformanceCounter TotalPushNotificationSuccesses = new ExPerformanceCounter("MSExchangeWS", "Push Notifications Succeeded", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalPushNotificationFailures = new ExPerformanceCounter("MSExchangeWS", "Push Notifications Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveStreamingConnections = new ExPerformanceCounter("MSExchangeWS", "Active Streaming Connections", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveSubscriptions = new ExPerformanceCounter("MSExchangeWS", "Active Subscriptions", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter FailedSubscriptionsPerSecond = new ExPerformanceCounter("MSExchangeWS", "Failed Subscriptions/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFailedSubscriptions = new ExPerformanceCounter("MSExchangeWS", "Total Failed Subscriptions", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.FailedSubscriptionsPerSecond
		});

		public static readonly ExPerformanceCounter TotalClientDisconnects = new ExPerformanceCounter("MSExchangeWS", "TotalClientDisconnects", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PID = new ExPerformanceCounter("MSExchangeWS", "Process ID", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CompletedRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "Completed requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCompletedRequests = new ExPerformanceCounter("MSExchangeWS", "Completed Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CompletedRequestsPerSecond
		});

		private static readonly ExPerformanceCounter RequestRejectionsPerSecond = new ExPerformanceCounter("MSExchangeWS", "Request rejections/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRequestRejections = new ExPerformanceCounter("MSExchangeWS", "Request rejections", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.RequestRejectionsPerSecond
		});

		public static readonly ExPerformanceCounter CurrentProxyCalls = new ExPerformanceCounter("MSExchangeWS", "Number of current proxy calls", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ProxyRequestsPerSeconds = new ExPerformanceCounter("MSExchangeWS", "Number of Proxied Requests per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalProxyRequests = new ExPerformanceCounter("MSExchangeWS", "Total number of proxied requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ProxyRequestsPerSeconds
		});

		public static readonly ExPerformanceCounter TotalProxyRequestBytes = new ExPerformanceCounter("MSExchangeWS", "Total number of bytes proxied", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalProxyResponseBytes = new ExPerformanceCounter("MSExchangeWS", "Total number of proxy response bytes", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ProxyFailoversPerSecond = new ExPerformanceCounter("MSExchangeWS", "Number of Proxy Failovers per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalProxyFailovers = new ExPerformanceCounter("MSExchangeWS", "Total number of proxy failover", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ProxyFailoversPerSecond
		});

		public static readonly ExPerformanceCounter ProxyAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "Proxy Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUserPhotoRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUserPhoto Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserPhotoRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserPhoto Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUserPhotoRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUserPhotoAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUserPhoto Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddAggregatedAccountSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "AddAggregatedAccount Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalIsOffice365DomainSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "IsOffice365Domain Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetAggregatedAccountSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetAggregatedAccount Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveAggregatedAccountSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveAggregatedAccount Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetAggregatedAccountSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetAggregatedAccount Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCopyFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CopyFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalArchiveItemItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ArchiveItem Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCopyItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CopyItem Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CreateFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateFolderPathSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CreateFolderPath Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CreateItem Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalPostModernGroupItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "PostModernGroupItem Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateAndPostModernGroupItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateAndPostModernGroupItem Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateResponseFromModernGroupSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CreateResponseFromModernGroup Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateManagedFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CreateManagedFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteItem Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExpandDLSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ExpandDL Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "FindFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "FindItem Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindConversationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "FindConversation Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindPeopleSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "FindPeople Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSyncPeopleSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SyncPeople Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSyncAutoCompleteRecipientsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SyncAutoCompleteRecipients Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetPersonaSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetPersona Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSyncConversationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SyncConversation Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetTimeZoneOffsetsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetTimeZoneOffsets Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetEventsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetEvents Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetStreamingEventsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetStreamingEvents Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetMailTipsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetMailTips Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalPlayOnPhoneSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "PlayOnPhone Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetPhoneCallInformationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetPhoneCallInformation Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDisconnectPhoneCallSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "DisconnectPhoneCall Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateUMPromptSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CreateUMPrompt Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMPromptSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMPrompt Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMPromptNamesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMPromptNames Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteUMPromptsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteUMPrompts Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetServiceConfigurationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetServiceConfiguration Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetItem Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetServerTimeZonesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetServerTimeZones Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMoveFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "MoveFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMoveItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "MoveItem Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalResolveNamesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ResolveNames Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSendItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SendItem Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSubscribeSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "Subscribe Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUnsubscribeSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "Unsubscribe Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateItem Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateItemInRecoverableItemsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateItemInRecoverableItems Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateAttachmentSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CreateAttachment Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteAttachmentSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteAttachment Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetAttachmentSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetAttachment Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetClientAccessTokenSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetClientAccessToken Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSendNotificationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SendNotification Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSyncFolderItemsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SyncFolderItems Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSyncFolderHierarchySuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SyncFolderHierarchy Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalConvertIdSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ConvertId Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetDelegateSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetDelegate Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalAddDelegateSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "AddDelegate Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRemoveDelegateSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "RemoveDelegate Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateDelegateSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateDelegate Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateUserConfigurationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CreateUserConfiguration Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteUserConfigurationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteUserConfiguration Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserConfigurationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserConfiguration Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateUserConfigurationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateUserConfiguration Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserAvailabilitySuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserAvailability Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserOofSettingsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserOofSettings Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetUserOofSettingsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetUserOofSettings Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetSharingMetadataSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetSharingMetadata Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRefreshSharingFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "RefreshSharingFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetSharingFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetSharingFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetRoomListsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetRoomLists Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetRoomsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetRooms Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalPerformReminderActionSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "PerformReminderAction Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetRemindersSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetReminders Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalProvisionSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "Provision Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeprovisionSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "Deprovision Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalLogPushNotificationDataSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "LogPushNotificationData Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindMessageTrackingReportSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "FindMessageTrackingReport Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetMessageTrackingReportSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetMessageTrackingReport Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalApplyConversationActionSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ApplyConversationAction Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalEmptyFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "EmptyFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUploadItemsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UploadItems Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExportItemsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ExportItems Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExecuteDiagnosticMethodSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ExecuteDiagnosticMethod Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFindMailboxStatisticsByKeywordsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "FindMailboxStatisticsByKeywords Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetSearchableMailboxesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetSearchableMailboxes Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSearchMailboxesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SearchMailboxes Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetDiscoverySearchConfigurationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetDiscoverySearchConfiguration Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetHoldOnMailboxesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetHoldOnMailboxes Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetHoldOnMailboxesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetHoldOnMailboxes Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetNonIndexableItemStatisticsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetNonIndexableItemStatistics Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetNonIndexableItemDetailsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetNonIndexableItemDetails Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMarkAllItemsAsReadSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "MarkAllItemsAsRead Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetClientExtensionSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetClientExtension Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetEncryptionConfigurationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetEncryptionConfiguration Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetClientExtensionSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetClientExtension Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetEncryptionConfigurationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetEncryptionConfiguration Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSubscribeToPushNotificationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SubscribeToPushNotification Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUnsubscribeToPushNotificationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UnsubscribeToPushNotification Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateUnifiedMailboxSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CreateUnifiedMailbox Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetAppManifestsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetAppManifests Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalInstallAppSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "InstallApp Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUninstallAppSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UninstallApp Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDisableAppSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "DisableApp Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetAppMarketplaceUrlSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetAppMarketplaceUrl Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetClientExtensionTokenSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetClientExtensionToken Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetEncryptionConfigurationTokenSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetEncryptionConfigurationToken Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetConversationItemsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetConversationItems Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetModernConversationItemsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetModernConversationItems Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetThreadedConversationItemsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetThreadedConversationItems Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetModernConversationAttachmentsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetModernConversationAttachments Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetModernGroupMembershipSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetModernGroupMembership Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserRetentionPolicyTagsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserRetentionPolicyTags Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserPhotoSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserPhoto Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter StartFindInGALSpeechRecognitionRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "StartFindInGALSpeechRecognition Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalStartFindInGALSpeechRecognitionRequests = new ExPerformanceCounter("MSExchangeWS", "StartFindInGALSpeechRecognition Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.StartFindInGALSpeechRecognitionRequestsPerSecond
		});

		public static readonly ExPerformanceCounter StartFindInGALSpeechRecognitionAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "StartFindInGALSpeechRecognition Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalStartFindInGALSpeechRecognitionSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "StartFindInGALSpeechRecognition Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CompleteFindInGALSpeechRecognitionRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CompleteFindInGALSpeechRecognition Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCompleteFindInGALSpeechRecognitionRequests = new ExPerformanceCounter("MSExchangeWS", "CompleteFindInGALSpeechRecognition Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CompleteFindInGALSpeechRecognitionRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CompleteFindInGALSpeechRecognitionAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CompleteFindInGALSpeechRecognition Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCompleteFindInGALSpeechRecognitionSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CompleteFindInGALSpeechRecognition Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CreateUMCallDataRecordRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CreateUMCallDataRecord Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateUMCallDataRecordRequests = new ExPerformanceCounter("MSExchangeWS", "CreateUMCallDataRecord Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CreateUMCallDataRecordRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CreateUMCallDataRecordAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CreateUMCallDataRecord Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateUMCallDataRecordSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CreateUMCallDataRecord Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUMCallDataRecordsRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUMCallDataRecords Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMCallDataRecordsRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMCallDataRecords Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUMCallDataRecordsRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUMCallDataRecordsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUMCallDataRecords Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMCallDataRecordsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMCallDataRecords Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUMCallSummaryRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUMCallSummary Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMCallSummaryRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMCallSummary Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUMCallSummaryRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUMCallSummaryAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUMCallSummary Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMCallSummarySuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMCallSummary Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUserPhotoDataRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUserPhotoData Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserPhotoDataRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserPhotoData Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUserPhotoDataRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUserPhotoDataAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUserPhotoData Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUserPhotoDataSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserPhotoData Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter InitUMMailboxRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "InitUMMailbox Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalInitUMMailboxRequests = new ExPerformanceCounter("MSExchangeWS", "InitUMMailbox Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.InitUMMailboxRequestsPerSecond
		});

		public static readonly ExPerformanceCounter InitUMMailboxAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "InitUMMailbox Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalInitUMMailboxSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "InitUMMailbox Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ResetUMMailboxRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ResetUMMailbox Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalResetUMMailboxRequests = new ExPerformanceCounter("MSExchangeWS", "ResetUMMailbox Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ResetUMMailboxRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ResetUMMailboxAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ResetUMMailbox Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalResetUMMailboxSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ResetUMMailbox Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ValidateUMPinRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ValidateUMPin Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalValidateUMPinRequests = new ExPerformanceCounter("MSExchangeWS", "ValidateUMPin Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ValidateUMPinRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ValidateUMPinAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ValidateUMPin Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalValidateUMPinSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ValidateUMPin Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SaveUMPinRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SaveUMPin Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSaveUMPinRequests = new ExPerformanceCounter("MSExchangeWS", "SaveUMPin Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SaveUMPinRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SaveUMPinAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SaveUMPin Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSaveUMPinSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SaveUMPin Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUMPinRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUMPin Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMPinRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMPin Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUMPinRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUMPinAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUMPin Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMPinSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMPin Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetClientIntentRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetClientIntent Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetClientIntentRequests = new ExPerformanceCounter("MSExchangeWS", "GetClientIntent Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetClientIntentRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetClientIntentAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetClientIntent Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetClientIntentSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetClientIntent Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUMSubscriberCallAnsweringDataRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUMSubscriberCallAnsweringData Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMSubscriberCallAnsweringDataRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMSubscriberCallAnsweringData Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUMSubscriberCallAnsweringDataRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetUMSubscriberCallAnsweringDataAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUMSubscriberCallAnsweringData Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetUMSubscriberCallAnsweringDataSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUMSubscriberCallAnsweringData Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UpdateMailboxAssociationRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UpdateMailboxAssociation Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateMailboxAssociationRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateMailboxAssociation Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UpdateMailboxAssociationRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UpdateMailboxAssociationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UpdateMailboxAssociation Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateMailboxAssociationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateMailboxAssociation Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UpdateGroupMailboxRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UpdateGroupMailbox Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateGroupMailboxRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateGroupMailbox Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UpdateGroupMailboxRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UpdateGroupMailboxAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UpdateGroupMailbox Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateGroupMailboxSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateGroupMailbox Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetCalendarEventRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetCalendarEvent Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetCalendarEventRequests = new ExPerformanceCounter("MSExchangeWS", "GetCalendarEvent Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetCalendarEventRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetCalendarEventAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetCalendarEvent Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetCalendarEventSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetCalendarEvent Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetCalendarViewRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetCalendarView Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetCalendarViewRequests = new ExPerformanceCounter("MSExchangeWS", "GetCalendarView Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetCalendarViewRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetCalendarViewAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetCalendarView Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetCalendarViewSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetCalendarView Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetBirthdayCalendarViewRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetBirthdayCalendarView Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetBirthdayCalendarViewRequests = new ExPerformanceCounter("MSExchangeWS", "GetBirthdayCalendarView Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetBirthdayCalendarViewRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetBirthdayCalendarViewAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetBirthdayCalendarView Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetBirthdayCalendarViewSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetBirthdayCalendarView Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CreateCalendarEventRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CreateCalendarEvent Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateCalendarEventRequests = new ExPerformanceCounter("MSExchangeWS", "CreateCalendarEvent Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CreateCalendarEventRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CreateCalendarEventAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CreateCalendarEvent Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCreateCalendarEventSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CreateCalendarEvent Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter UpdateCalendarEventRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "UpdateCalendarEvent Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateCalendarEventRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateCalendarEvent Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.UpdateCalendarEventRequestsPerSecond
		});

		public static readonly ExPerformanceCounter UpdateCalendarEventAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "UpdateCalendarEvent Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUpdateCalendarEventSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "UpdateCalendarEvent Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter CancelCalendarEventRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "CancelCalendarEvent Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCancelCalendarEventRequests = new ExPerformanceCounter("MSExchangeWS", "CancelCalendarEvent Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.CancelCalendarEventRequestsPerSecond
		});

		public static readonly ExPerformanceCounter CancelCalendarEventAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "CancelCalendarEvent Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCancelCalendarEventSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "CancelCalendarEvent Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RespondToCalendarEventRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "RespondToCalendarEvent Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRespondToCalendarEventRequests = new ExPerformanceCounter("MSExchangeWS", "RespondToCalendarEvent Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.RespondToCalendarEventRequestsPerSecond
		});

		public static readonly ExPerformanceCounter RespondToCalendarEventAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "RespondToCalendarEvent Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRespondToCalendarEventSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "RespondToCalendarEvent Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RefreshGALContactsFolderRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "RefreshGALContactsFolder Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRefreshGALContactsFolderRequests = new ExPerformanceCounter("MSExchangeWS", "RefreshGALContactsFolder Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.RefreshGALContactsFolderRequestsPerSecond
		});

		public static readonly ExPerformanceCounter RefreshGALContactsFolderAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "RefreshGALContactsFolder Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRefreshGALContactsFolderSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "RefreshGALContactsFolder Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SubscribeToConversationChangesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SubscribeToConversationChanges Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSubscribeToConversationChangesRequests = new ExPerformanceCounter("MSExchangeWS", "SubscribeToConversationChanges Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SubscribeToConversationChangesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SubscribeToConversationChangesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SubscribeToConversationChanges Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSubscribeToConversationChangesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SubscribeToConversationChanges Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SubscribeToHierarchyChangesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SubscribeToHierarchyChanges Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSubscribeToHierarchyChangesRequests = new ExPerformanceCounter("MSExchangeWS", "SubscribeToHierarchyChanges Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SubscribeToHierarchyChangesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SubscribeToHierarchyChangesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SubscribeToHierarchyChanges Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSubscribeToHierarchyChangesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SubscribeToHierarchyChanges Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SubscribeToMessageChangesRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SubscribeToMessageChanges Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSubscribeToMessageChangesRequests = new ExPerformanceCounter("MSExchangeWS", "SubscribeToMessageChanges Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SubscribeToMessageChangesRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SubscribeToMessageChangesAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SubscribeToMessageChanges Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSubscribeToMessageChangesSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SubscribeToMessageChanges Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter DeleteCalendarEventRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "DeleteCalendarEvent Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteCalendarEventRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteCalendarEvent Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.DeleteCalendarEventRequestsPerSecond
		});

		public static readonly ExPerformanceCounter DeleteCalendarEventAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "DeleteCalendarEvent Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeleteCalendarEventSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "DeleteCalendarEvent Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ForwardCalendarEventRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ForwardCalendarEvent Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalForwardCalendarEventRequests = new ExPerformanceCounter("MSExchangeWS", "ForwardCalendarEvent Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ForwardCalendarEventRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ForwardCalendarEventAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ForwardCalendarEvent Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalForwardCalendarEventSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ForwardCalendarEvent Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LikeItemRequests = new ExPerformanceCounter("MSExchangeWS", "LikeItem Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter LikeItemPerSecond = new ExPerformanceCounter("MSExchangeWS", "LikeItem Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LikeItemAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "LikeItem Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LikeItemSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "LikeItem Successful Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.LikeItemPerSecond
		});

		public static readonly ExPerformanceCounter GetLikersRequests = new ExPerformanceCounter("MSExchangeWS", "GetLikers Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetLikersPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetLikers Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetLikersAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetLikers Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetLikersSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetLikers Successful Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetLikersPerSecond
		});

		private static readonly ExPerformanceCounter ExpandCalendarEventRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "ExpandCalendarEvent Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExpandCalendarEventRequests = new ExPerformanceCounter("MSExchangeWS", "ExpandCalendarEvent Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.ExpandCalendarEventRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ExpandCalendarEventAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "ExpandCalendarEvent Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExpandCalendarEventSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "ExpandCalendarEvent Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetConversationItemsDiagnosticsRequests = new ExPerformanceCounter("MSExchangeWS", "GetConversationItemsDiagnostics Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetConversationItemsDiagnosticsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetConversationItemsDiagnostics Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetConversationItemsDiagnosticsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetConversationItemsDiagnostics Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetConversationItemsDiagnosticsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetConversationItemsDiagnostics Successful Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetConversationItemsDiagnosticsPerSecond
		});

		private static readonly ExPerformanceCounter GetComplianceConfigurationPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetComplianceConfiguration Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetComplianceConfiguration = new ExPerformanceCounter("MSExchangeWS", "GetComplianceConfiguration Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetComplianceConfigurationPerSecond
		});

		public static readonly ExPerformanceCounter GetComplianceConfigurationAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetComplianceConfiguration Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetComplianceConfigurationSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetComplianceConfiguration Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter PerformInstantSearchRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "PerformInstantSearch Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalPerformInstantSearchRequests = new ExPerformanceCounter("MSExchangeWS", "PerformInstantSearch Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.PerformInstantSearchRequestsPerSecond
		});

		public static readonly ExPerformanceCounter PerformInstantSearchAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "PerformInstantSearch Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PerformInstantSearchSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "PerformInstantSearch Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter EndInstantSearchSessionRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "EndInstantSearchSession Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalEndInstantSearchSessionRequests = new ExPerformanceCounter("MSExchangeWS", "EndInstantSearchSession Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.EndInstantSearchSessionRequestsPerSecond
		});

		public static readonly ExPerformanceCounter EndInstantSearchSessionAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "EndInstantSearchSession Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EndInstantSearchSessionSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "EndInstantSearchSession Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetUserUnifiedGroupsRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserUnifiedGroups Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetUserUnifiedGroupsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetUserUnifiedGroups Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetUserUnifiedGroupsAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetUserUnifiedGroups Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetUserUnifiedGroupsSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetUserUnifiedGroups Successful Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetUserUnifiedGroupsPerSecond
		});

		private static readonly ExPerformanceCounter GetPeopleICommunicateWithRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetPeopleICommunicateWith Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetPeopleICommunicateWithRequests = new ExPerformanceCounter("MSExchangeWS", "GetPeopleICommunicateWith Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetPeopleICommunicateWithRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetPeopleICommunicateWithAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetPeopleICommunicateWith Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetPeopleICommunicateWithSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetPeopleICommunicateWith Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter MaskAutoCompleteRecipientRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "MaskAutoCompleteRecipient Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MaskAutoCompleteRecipientRequests = new ExPerformanceCounter("MSExchangeWS", "MaskAutoCompleteRecipient Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.MaskAutoCompleteRecipientRequestsPerSecond
		});

		public static readonly ExPerformanceCounter MaskAutoCompleteRecipientAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "MaskAutoCompleteRecipient Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMaskAutoCompleteRecipientSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "MaskAutoCompleteRecipient Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetClutterStateRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "GetClutterState Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetClutterStateRequests = new ExPerformanceCounter("MSExchangeWS", "GetClutterState Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.GetClutterStateRequestsPerSecond
		});

		public static readonly ExPerformanceCounter GetClutterStateAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "GetClutterState Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalGetClutterStateSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "GetClutterState Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SetClutterStateRequestsPerSecond = new ExPerformanceCounter("MSExchangeWS", "SetClutterState Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetClutterStateRequests = new ExPerformanceCounter("MSExchangeWS", "SetClutterState Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			WsPerformanceCounters.SetClutterStateRequestsPerSecond
		});

		public static readonly ExPerformanceCounter SetClutterStateAverageResponseTime = new ExPerformanceCounter("MSExchangeWS", "SetClutterState Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSetClutterStateSuccessfulRequests = new ExPerformanceCounter("MSExchangeWS", "SetClutterState Successful Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			WsPerformanceCounters.TotalAddAggregatedAccountRequests,
			WsPerformanceCounters.AddAggregatedAccountAverageResponseTime,
			WsPerformanceCounters.TotalIsOffice365DomainRequests,
			WsPerformanceCounters.IsOffice365DomainAverageResponseTime,
			WsPerformanceCounters.TotalGetAggregatedAccountRequests,
			WsPerformanceCounters.GetAggregatedAccountAverageResponseTime,
			WsPerformanceCounters.TotalRemoveAggregatedAccountRequests,
			WsPerformanceCounters.RemoveAggregatedAccountAverageResponseTime,
			WsPerformanceCounters.TotalSetAggregatedAccountRequests,
			WsPerformanceCounters.SetAggregatedAccountAverageResponseTime,
			WsPerformanceCounters.TotalGetItemRequests,
			WsPerformanceCounters.GetItemAverageResponseTime,
			WsPerformanceCounters.TotalConvertIdRequests,
			WsPerformanceCounters.ConvertIdAverageResponseTime,
			WsPerformanceCounters.TotalIdsConverted,
			WsPerformanceCounters.TotalCreateItemRequests,
			WsPerformanceCounters.CreateItemAverageResponseTime,
			WsPerformanceCounters.TotalPostModernGroupItemRequests,
			WsPerformanceCounters.PostModernGroupItemAverageResponseTime,
			WsPerformanceCounters.TotalUpdateAndPostModernGroupItemRequests,
			WsPerformanceCounters.UpdateAndPostModernGroupItemAverageResponseTime,
			WsPerformanceCounters.TotalCreateResponseFromModernGroupRequests,
			WsPerformanceCounters.CreateResponseFromModernGroupAverageResponseTime,
			WsPerformanceCounters.TotalUploadItemsRequests,
			WsPerformanceCounters.UploadItemsAverageResponseTime,
			WsPerformanceCounters.TotalUploadLargeItemRequests,
			WsPerformanceCounters.UploadLargeItemAverageResponseTime,
			WsPerformanceCounters.TotalChunkUploadRequests,
			WsPerformanceCounters.ChunkUploadAverageResponseTime,
			WsPerformanceCounters.TotalCompleteLargeItemUploadRequests,
			WsPerformanceCounters.CompleteLargeItemUploadAverageResponseTime,
			WsPerformanceCounters.TotalExportItemsRequests,
			WsPerformanceCounters.ExportItemsAverageResponseTime,
			WsPerformanceCounters.TotalDeleteItemRequests,
			WsPerformanceCounters.DeleteItemAverageResponseTime,
			WsPerformanceCounters.TotalUpdateItemRequests,
			WsPerformanceCounters.UpdateItemAverageResponseTime,
			WsPerformanceCounters.TotalUpdateItemInRecoverableItemsRequests,
			WsPerformanceCounters.UpdateItemInRecoverableItemsAverageResponseTime,
			WsPerformanceCounters.TotalMarkAllItemsAsReadRequests,
			WsPerformanceCounters.MarkAllItemsAsReadAverageResponseTime,
			WsPerformanceCounters.TotalMarkAsJunkRequests,
			WsPerformanceCounters.MarkAsJunkAverageResponseTime,
			WsPerformanceCounters.TotalMarkAsJunkSuccessfulRequests,
			WsPerformanceCounters.TotalGetClientExtensionRequests,
			WsPerformanceCounters.GetClientExtensionAverageResponseTime,
			WsPerformanceCounters.TotalGetEncryptionConfigurationRequests,
			WsPerformanceCounters.GetEncryptionConfigurationAverageResponseTime,
			WsPerformanceCounters.TotalSetClientExtensionRequests,
			WsPerformanceCounters.SetClientExtensionAverageResponseTime,
			WsPerformanceCounters.TotalSetEncryptionConfigurationRequests,
			WsPerformanceCounters.SetEncryptionConfigurationAverageResponseTime,
			WsPerformanceCounters.TotalCreateUnifiedMailboxRequests,
			WsPerformanceCounters.CreateUnifiedMailboxAverageResponseTime,
			WsPerformanceCounters.TotalGetAppManifestsRequests,
			WsPerformanceCounters.GetAppManifestsAverageResponseTime,
			WsPerformanceCounters.TotalGetClientExtensionTokenRequests,
			WsPerformanceCounters.GetClientExtensionTokenAverageResponseTime,
			WsPerformanceCounters.TotalGetEncryptionConfigurationTokenRequests,
			WsPerformanceCounters.GetEncryptionConfigurationTokenAverageResponseTime,
			WsPerformanceCounters.TotalInstallAppRequests,
			WsPerformanceCounters.InstallAppAverageResponseTime,
			WsPerformanceCounters.TotalUninstallAppRequests,
			WsPerformanceCounters.UninstallAppAverageResponseTime,
			WsPerformanceCounters.TotalDisableAppRequests,
			WsPerformanceCounters.DisableAppAverageResponseTime,
			WsPerformanceCounters.TotalGetAppMarketplaceUrlRequests,
			WsPerformanceCounters.GetAppMarketplaceUrlAverageResponseTime,
			WsPerformanceCounters.TotalAddImContactToGroupRequests,
			WsPerformanceCounters.AddImContactToGroupAverageResponseTime,
			WsPerformanceCounters.TotalAddImContactToGroupSuccessfulRequests,
			WsPerformanceCounters.TotalRemoveImContactFromGroupRequests,
			WsPerformanceCounters.RemoveImContactFromGroupAverageResponseTime,
			WsPerformanceCounters.TotalRemoveImContactFromGroupSuccessfulRequests,
			WsPerformanceCounters.TotalAddNewImContactToGroupRequests,
			WsPerformanceCounters.AddNewImContactToGroupAverageResponseTime,
			WsPerformanceCounters.TotalAddNewImContactToGroupSuccessfulRequests,
			WsPerformanceCounters.TotalAddNewTelUriContactToGroupRequests,
			WsPerformanceCounters.AddNewTelUriContactToGroupAverageResponseTime,
			WsPerformanceCounters.TotalAddNewTelUriContactToGroupSuccessfulRequests,
			WsPerformanceCounters.TotalAddDistributionGroupToImListRequests,
			WsPerformanceCounters.AddDistributionGroupToImListAverageResponseTime,
			WsPerformanceCounters.TotalAddDistributionGroupToImListSuccessfulRequests,
			WsPerformanceCounters.TotalAddImGroupRequests,
			WsPerformanceCounters.AddImGroupAverageResponseTime,
			WsPerformanceCounters.TotalAddImGroupSuccessfulRequests,
			WsPerformanceCounters.TotalGetImItemListRequests,
			WsPerformanceCounters.GetImItemListAverageResponseTime,
			WsPerformanceCounters.TotalGetImItemListSuccessfulRequests,
			WsPerformanceCounters.TotalGetImItemsRequests,
			WsPerformanceCounters.GetImItemsAverageResponseTime,
			WsPerformanceCounters.TotalGetImItemsSuccessfulRequests,
			WsPerformanceCounters.TotalRemoveContactFromImListRequests,
			WsPerformanceCounters.RemoveContactFromImListAverageResponseTime,
			WsPerformanceCounters.TotalRemoveContactFromImListSuccessfulRequests,
			WsPerformanceCounters.TotalRemoveDistributionGroupFromImListRequests,
			WsPerformanceCounters.RemoveDistributionGroupFromImListAverageResponseTime,
			WsPerformanceCounters.TotalRemoveDistributionGroupFromImListSuccessfulRequests,
			WsPerformanceCounters.TotalRemoveImGroupRequests,
			WsPerformanceCounters.RemoveImGroupAverageResponseTime,
			WsPerformanceCounters.TotalRemoveImGroupSuccessfulRequests,
			WsPerformanceCounters.TotalSetImGroupRequests,
			WsPerformanceCounters.SetImGroupAverageResponseTime,
			WsPerformanceCounters.TotalSetImGroupSuccessfulRequests,
			WsPerformanceCounters.TotalSetImListMigrationCompletedRequests,
			WsPerformanceCounters.SetImListMigrationCompletedAverageResponseTime,
			WsPerformanceCounters.TotalSetImListMigrationCompletedSuccessfulRequests,
			WsPerformanceCounters.TotalGetConversationItemsRequests,
			WsPerformanceCounters.GetConversationItemsAverageResponseTime,
			WsPerformanceCounters.TotalGetModernConversationItemsRequests,
			WsPerformanceCounters.GetModernConversationItemsAverageResponseTime,
			WsPerformanceCounters.TotalGetThreadedConversationItemsRequests,
			WsPerformanceCounters.GetThreadedConversationItemsAverageResponseTime,
			WsPerformanceCounters.TotalGetModernConversationAttachmentsRequests,
			WsPerformanceCounters.GetModernConversationAttachmentsAverageResponseTime,
			WsPerformanceCounters.TotalSetModernGroupMembershipRequests,
			WsPerformanceCounters.SetModernGroupMembershipAverageResponseTime,
			WsPerformanceCounters.TotalSendItemRequests,
			WsPerformanceCounters.SendItemAverageResponseTime,
			WsPerformanceCounters.TotalArchiveItemRequests,
			WsPerformanceCounters.ArchiveItemAverageResponseTime,
			WsPerformanceCounters.TotalMoveItemRequests,
			WsPerformanceCounters.MoveItemAverageResponseTime,
			WsPerformanceCounters.TotalCopyItemRequests,
			WsPerformanceCounters.CopyItemAverageResponseTime,
			WsPerformanceCounters.TotalGetFolderRequests,
			WsPerformanceCounters.GetFolderAverageResponseTime,
			WsPerformanceCounters.TotalCreateFolderRequests,
			WsPerformanceCounters.CreateFolderAverageResponseTime,
			WsPerformanceCounters.TotalCreateFolderPathRequests,
			WsPerformanceCounters.CreateFolderPathAverageResponseTime,
			WsPerformanceCounters.TotalCreateManagedFolderRequests,
			WsPerformanceCounters.CreateManagedFolderAverageResponseTime,
			WsPerformanceCounters.TotalDeleteFolderRequests,
			WsPerformanceCounters.DeleteFolderAverageResponseTime,
			WsPerformanceCounters.TotalEmptyFolderRequests,
			WsPerformanceCounters.EmptyFolderAverageResponseTime,
			WsPerformanceCounters.TotalUpdateFolderRequests,
			WsPerformanceCounters.UpdateFolderAverageResponseTime,
			WsPerformanceCounters.TotalMoveFolderRequests,
			WsPerformanceCounters.MoveFolderAverageResponseTime,
			WsPerformanceCounters.TotalCopyFolderRequests,
			WsPerformanceCounters.CopyFolderAverageResponseTime,
			WsPerformanceCounters.TotalFindItemRequests,
			WsPerformanceCounters.FindItemAverageResponseTime,
			WsPerformanceCounters.TotalFindFolderRequests,
			WsPerformanceCounters.FindFolderAverageResponseTime,
			WsPerformanceCounters.TotalResolveNamesRequests,
			WsPerformanceCounters.ResolveNamesAverageResponseTime,
			WsPerformanceCounters.TotalExpandDLRequests,
			WsPerformanceCounters.ExpandDLAverageResponseTime,
			WsPerformanceCounters.TotalGetPasswordExpirationDateRequests,
			WsPerformanceCounters.GetPasswordExpirationDateAverageResponseTime,
			WsPerformanceCounters.TotalGetPasswordExpirationDateSuccessfulRequests,
			WsPerformanceCounters.TotalCreateAttachmentRequests,
			WsPerformanceCounters.CreateAttachmentAverageResponseTime,
			WsPerformanceCounters.TotalDeleteAttachmentRequests,
			WsPerformanceCounters.DeleteAttachmentAverageResponseTime,
			WsPerformanceCounters.TotalGetAttachmentRequests,
			WsPerformanceCounters.GetAttachmentAverageResponseTime,
			WsPerformanceCounters.TotalGetClientAccessTokenRequests,
			WsPerformanceCounters.GetClientAccessTokenAverageResponseTime,
			WsPerformanceCounters.TotalSubscribeRequests,
			WsPerformanceCounters.SubscribeAverageResponseTime,
			WsPerformanceCounters.TotalUnsubscribeRequests,
			WsPerformanceCounters.UnsubscribeAverageResponseTime,
			WsPerformanceCounters.TotalGetStreamingEventsRequests,
			WsPerformanceCounters.GetStreamingEventsAverageResponseTime,
			WsPerformanceCounters.TotalGetEventsRequests,
			WsPerformanceCounters.GetEventsAverageResponseTime,
			WsPerformanceCounters.TotalGetServiceConfigurationRequests,
			WsPerformanceCounters.GetServiceConfigurationAverageResponseTime,
			WsPerformanceCounters.TotalGetMailTipsRequests,
			WsPerformanceCounters.GetMailTipsAverageResponseTime,
			WsPerformanceCounters.TotalSyncFolderHierarchyRequests,
			WsPerformanceCounters.SyncFolderHierarchyAverageResponseTime,
			WsPerformanceCounters.TotalSyncFolderItemsRequests,
			WsPerformanceCounters.SyncFolderItemsAverageResponseTime,
			WsPerformanceCounters.TotalGetDelegateRequests,
			WsPerformanceCounters.GetDelegateAverageResponseTime,
			WsPerformanceCounters.TotalAddDelegateRequests,
			WsPerformanceCounters.AddDelegateAverageResponseTime,
			WsPerformanceCounters.TotalRemoveDelegateRequests,
			WsPerformanceCounters.RemoveDelegateAverageResponseTime,
			WsPerformanceCounters.TotalUpdateDelegateRequests,
			WsPerformanceCounters.UpdateDelegateAverageResponseTime,
			WsPerformanceCounters.TotalCreateUserConfigurationRequests,
			WsPerformanceCounters.CreateUserConfigurationAverageResponseTime,
			WsPerformanceCounters.TotalGetUserConfigurationRequests,
			WsPerformanceCounters.GetUserConfigurationAverageResponseTime,
			WsPerformanceCounters.TotalUpdateUserConfigurationRequests,
			WsPerformanceCounters.UpdateUserConfigurationAverageResponseTime,
			WsPerformanceCounters.TotalDeleteUserConfigurationRequests,
			WsPerformanceCounters.DeleteUserConfigurationAverageResponseTime,
			WsPerformanceCounters.TotalGetUserAvailabilityRequests,
			WsPerformanceCounters.GetUserAvailabilityAverageResponseTime,
			WsPerformanceCounters.TotalGetUserOofSettingsRequests,
			WsPerformanceCounters.GetUserOofSettingsAverageResponseTime,
			WsPerformanceCounters.TotalSetUserOofSettingsRequests,
			WsPerformanceCounters.SetUserOofSettingsAverageResponseTime,
			WsPerformanceCounters.TotalGetSharingMetadataRequests,
			WsPerformanceCounters.GetSharingMetadataAverageResponseTime,
			WsPerformanceCounters.TotalRefreshSharingFolderRequests,
			WsPerformanceCounters.RefreshSharingFolderAverageResponseTime,
			WsPerformanceCounters.TotalGetSharingFolderRequests,
			WsPerformanceCounters.GetSharingFolderAverageResponseTime,
			WsPerformanceCounters.TotalSetTeamMailboxRequests,
			WsPerformanceCounters.SetTeamMailboxAverageResponseTime,
			WsPerformanceCounters.TotalSetTeamMailboxSuccessfulRequests,
			WsPerformanceCounters.TotalUnpinTeamMailboxRequests,
			WsPerformanceCounters.UnpinTeamMailboxAverageResponseTime,
			WsPerformanceCounters.TotalUnpinTeamMailboxSuccessfulRequests,
			WsPerformanceCounters.TotalGetRoomListsRequests,
			WsPerformanceCounters.GetRoomListsAverageResponseTime,
			WsPerformanceCounters.SubscribeToPushNotificationRequests,
			WsPerformanceCounters.SubscribeToPushNotificationAverageResponseTime,
			WsPerformanceCounters.RequestDeviceRegistrationChallengeRequest,
			WsPerformanceCounters.RequestDeviceRegistrationChallengeAverageResponseTime,
			WsPerformanceCounters.DeviceRegistrationChallengeRequestSuccessfulRequests,
			WsPerformanceCounters.UnsubscribeToPushNotificationRequests,
			WsPerformanceCounters.UnsubscribeToPushNotificationAverageResponseTime,
			WsPerformanceCounters.TotalGetRoomsRequests,
			WsPerformanceCounters.GetRoomsAverageResponseTime,
			WsPerformanceCounters.TotalPerformReminderActionRequests,
			WsPerformanceCounters.PerformReminderActionAverageResponseTime,
			WsPerformanceCounters.TotalGetRemindersRequests,
			WsPerformanceCounters.GetRemindersAverageResponseTime,
			WsPerformanceCounters.TotalProvisionRequests,
			WsPerformanceCounters.ProvisionAverageResponseTime,
			WsPerformanceCounters.TotalLogPushNotificationDataRequests,
			WsPerformanceCounters.LogPushNotificationDataAverageResponseTime,
			WsPerformanceCounters.TotalDeprovisionRequests,
			WsPerformanceCounters.DeprovisionAverageResponseTime,
			WsPerformanceCounters.TotalFindConversationRequests,
			WsPerformanceCounters.FindConversationAverageResponseTime,
			WsPerformanceCounters.TotalFindPeopleRequests,
			WsPerformanceCounters.FindPeopleAverageResponseTime,
			WsPerformanceCounters.TotalSyncPeopleRequests,
			WsPerformanceCounters.SyncPeopleAverageResponseTime,
			WsPerformanceCounters.TotalSyncAutoCompleteRecipientsRequests,
			WsPerformanceCounters.SyncAutoCompleteRecipientsAverageResponseTime,
			WsPerformanceCounters.TotalGetPersonaRequests,
			WsPerformanceCounters.GetPersonaAverageResponseTime,
			WsPerformanceCounters.TotalSyncConversationRequests,
			WsPerformanceCounters.SyncConversationAverageResponseTime,
			WsPerformanceCounters.TotalGetTimeZoneOffsetsRequests,
			WsPerformanceCounters.GetTimeZoneOffsetsAverageResponseTime,
			WsPerformanceCounters.TotalTimeZoneOffsetsTablesBuilt,
			WsPerformanceCounters.TotalFindMailboxStatisticsByKeywordsRequests,
			WsPerformanceCounters.FindMailboxStatisticsByKeywordsAverageResponseTime,
			WsPerformanceCounters.TotalGetSearchableMailboxesRequests,
			WsPerformanceCounters.GetSearchableMailboxesAverageResponseTime,
			WsPerformanceCounters.TotalSearchMailboxesRequests,
			WsPerformanceCounters.SearchMailboxesAverageResponseTime,
			WsPerformanceCounters.TotalGetDiscoverySearchConfigurationRequests,
			WsPerformanceCounters.GetDiscoverySearchConfigurationAverageResponseTime,
			WsPerformanceCounters.TotalGetHoldOnMailboxesRequests,
			WsPerformanceCounters.GetHoldOnMailboxesAverageResponseTime,
			WsPerformanceCounters.TotalSetHoldOnMailboxesRequests,
			WsPerformanceCounters.SetHoldOnMailboxesAverageResponseTime,
			WsPerformanceCounters.TotalGetNonIndexableItemStatisticsRequests,
			WsPerformanceCounters.GetNonIndexableItemStatisticsAverageResponseTime,
			WsPerformanceCounters.TotalGetNonIndexableItemDetailsRequests,
			WsPerformanceCounters.GetNonIndexableItemDetailsAverageResponseTime,
			WsPerformanceCounters.TotalGetUserRetentionPolicyTagsRequests,
			WsPerformanceCounters.GetUserRetentionPolicyTagsAverageResponseTime,
			WsPerformanceCounters.TotalPlayOnPhoneRequests,
			WsPerformanceCounters.PlayOnPhoneAverageResponseTime,
			WsPerformanceCounters.TotalGetPhoneCallInformationRequests,
			WsPerformanceCounters.GetPhoneCallInformationAverageResponseTime,
			WsPerformanceCounters.TotalDisconnectPhoneCallRequests,
			WsPerformanceCounters.DisconnectPhoneCallAverageResponseTime,
			WsPerformanceCounters.TotalCreateUMPromptRequests,
			WsPerformanceCounters.CreateUMPromptAverageResponseTime,
			WsPerformanceCounters.TotalGetUMPromptRequests,
			WsPerformanceCounters.GetUMPromptAverageResponseTime,
			WsPerformanceCounters.TotalGetUMPromptNamesRequests,
			WsPerformanceCounters.GetUMPromptNamesAverageResponseTime,
			WsPerformanceCounters.TotalDeleteUMPromptsRequests,
			WsPerformanceCounters.DeleteUMPromptsAverageResponseTime,
			WsPerformanceCounters.TotalGetServerTimeZonesRequests,
			WsPerformanceCounters.GetServerTimeZonesAverageResponseTime,
			WsPerformanceCounters.TotalSendNotificationRequests,
			WsPerformanceCounters.SendNotificationAverageResponseTime,
			WsPerformanceCounters.TotalFindMessageTrackingReportRequests,
			WsPerformanceCounters.FindMessageTrackingReportAverageResponseTime,
			WsPerformanceCounters.TotalGetMessageTrackingReportRequests,
			WsPerformanceCounters.GetMessageTrackingReportAverageResponseTime,
			WsPerformanceCounters.TotalApplyConversationActionRequests,
			WsPerformanceCounters.ApplyConversationActionAverageResponseTime,
			WsPerformanceCounters.TotalExecuteDiagnosticMethodRequests,
			WsPerformanceCounters.ExecuteDiagnosticMethodAverageResponseTime,
			WsPerformanceCounters.TotalGetInboxRulesRequests,
			WsPerformanceCounters.GetInboxRulesAverageResponseTime,
			WsPerformanceCounters.TotalGetInboxRulesSuccessfulRequests,
			WsPerformanceCounters.TotalUpdateInboxRulesRequests,
			WsPerformanceCounters.UpdateInboxRulesAverageResponseTime,
			WsPerformanceCounters.TotalUpdateInboxRulesSuccessfulRequests,
			WsPerformanceCounters.TotalIsUMEnabledRequests,
			WsPerformanceCounters.IsUMEnabledAverageResponseTime,
			WsPerformanceCounters.TotalIsUMEnabledSuccessfulRequests,
			WsPerformanceCounters.TotalGetUMPropertiesRequests,
			WsPerformanceCounters.GetUMPropertiesAverageResponseTime,
			WsPerformanceCounters.TotalGetUMPropertiesSuccessfulRequests,
			WsPerformanceCounters.TotalSetOofStatusRequests,
			WsPerformanceCounters.SetOofStatusAverageResponseTime,
			WsPerformanceCounters.TotalSetOofStatusSuccessfulRequests,
			WsPerformanceCounters.TotalSetPlayOnPhoneDialStringRequests,
			WsPerformanceCounters.SetPlayOnPhoneDialStringAverageResponseTime,
			WsPerformanceCounters.TotalSetPlayOnPhoneDialStringSuccessfulRequests,
			WsPerformanceCounters.TotalSetTelephoneAccessFolderEmailRequests,
			WsPerformanceCounters.SetTelephoneAccessFolderEmailAverageResponseTime,
			WsPerformanceCounters.TotalSetTelephoneAccessFolderEmailSuccessfulRequests,
			WsPerformanceCounters.TotalSetMissedCallNotificationEnabledRequests,
			WsPerformanceCounters.SetMissedCallNotificationEnabledAverageResponseTime,
			WsPerformanceCounters.TotalSetMissedCallNotificationEnabledSuccessfulRequests,
			WsPerformanceCounters.TotalResetPINRequests,
			WsPerformanceCounters.ResetPINAverageResponseTime,
			WsPerformanceCounters.TotalResetPINSuccessfulRequests,
			WsPerformanceCounters.TotalGetCallInfoRequests,
			WsPerformanceCounters.GetCallInfoAverageResponseTime,
			WsPerformanceCounters.TotalGetCallInfoSuccessfulRequests,
			WsPerformanceCounters.TotalDisconnectRequests,
			WsPerformanceCounters.DisconnectAverageResponseTime,
			WsPerformanceCounters.TotalDisconnectSuccessfulRequests,
			WsPerformanceCounters.TotalPlayOnPhoneGreetingRequests,
			WsPerformanceCounters.PlayOnPhoneGreetingAverageResponseTime,
			WsPerformanceCounters.TotalPlayOnPhoneGreetingSuccessfulRequests,
			WsPerformanceCounters.TotalStreamedEvents,
			WsPerformanceCounters.TotalRequests,
			WsPerformanceCounters.AverageResponseTime,
			WsPerformanceCounters.TotalItemsCreated,
			WsPerformanceCounters.TotalItemsDeleted,
			WsPerformanceCounters.TotalItemsSent,
			WsPerformanceCounters.TotalItemsRead,
			WsPerformanceCounters.TotalItemsUpdated,
			WsPerformanceCounters.TotalItemsCopied,
			WsPerformanceCounters.TotalItemsMoved,
			WsPerformanceCounters.TotalFoldersCreated,
			WsPerformanceCounters.TotalFoldersDeleted,
			WsPerformanceCounters.TotalFoldersRead,
			WsPerformanceCounters.TotalFoldersUpdated,
			WsPerformanceCounters.TotalFoldersCopied,
			WsPerformanceCounters.TotalFoldersMoved,
			WsPerformanceCounters.TotalFoldersSynced,
			WsPerformanceCounters.TotalItemsSynced,
			WsPerformanceCounters.TotalPushNotificationSuccesses,
			WsPerformanceCounters.TotalPushNotificationFailures,
			WsPerformanceCounters.ActiveStreamingConnections,
			WsPerformanceCounters.ActiveSubscriptions,
			WsPerformanceCounters.TotalFailedSubscriptions,
			WsPerformanceCounters.TotalClientDisconnects,
			WsPerformanceCounters.PID,
			WsPerformanceCounters.TotalCompletedRequests,
			WsPerformanceCounters.TotalRequestRejections,
			WsPerformanceCounters.CurrentProxyCalls,
			WsPerformanceCounters.TotalProxyRequests,
			WsPerformanceCounters.TotalProxyRequestBytes,
			WsPerformanceCounters.TotalProxyResponseBytes,
			WsPerformanceCounters.TotalProxyFailovers,
			WsPerformanceCounters.ProxyAverageResponseTime,
			WsPerformanceCounters.TotalGetUserPhotoRequests,
			WsPerformanceCounters.GetUserPhotoAverageResponseTime,
			WsPerformanceCounters.TotalAddAggregatedAccountSuccessfulRequests,
			WsPerformanceCounters.TotalIsOffice365DomainSuccessfulRequests,
			WsPerformanceCounters.TotalGetAggregatedAccountSuccessfulRequests,
			WsPerformanceCounters.TotalRemoveAggregatedAccountSuccessfulRequests,
			WsPerformanceCounters.TotalSetAggregatedAccountSuccessfulRequests,
			WsPerformanceCounters.TotalCopyFolderSuccessfulRequests,
			WsPerformanceCounters.TotalArchiveItemItemSuccessfulRequests,
			WsPerformanceCounters.TotalCopyItemSuccessfulRequests,
			WsPerformanceCounters.TotalCreateFolderSuccessfulRequests,
			WsPerformanceCounters.TotalCreateFolderPathSuccessfulRequests,
			WsPerformanceCounters.TotalCreateItemSuccessfulRequests,
			WsPerformanceCounters.TotalPostModernGroupItemSuccessfulRequests,
			WsPerformanceCounters.TotalUpdateAndPostModernGroupItemSuccessfulRequests,
			WsPerformanceCounters.TotalCreateResponseFromModernGroupSuccessfulRequests,
			WsPerformanceCounters.TotalCreateManagedFolderSuccessfulRequests,
			WsPerformanceCounters.TotalDeleteFolderSuccessfulRequests,
			WsPerformanceCounters.TotalDeleteItemSuccessfulRequests,
			WsPerformanceCounters.TotalExpandDLSuccessfulRequests,
			WsPerformanceCounters.TotalFindFolderSuccessfulRequests,
			WsPerformanceCounters.TotalFindItemSuccessfulRequests,
			WsPerformanceCounters.TotalFindConversationSuccessfulRequests,
			WsPerformanceCounters.TotalFindPeopleSuccessfulRequests,
			WsPerformanceCounters.TotalSyncPeopleSuccessfulRequests,
			WsPerformanceCounters.TotalSyncAutoCompleteRecipientsSuccessfulRequests,
			WsPerformanceCounters.TotalGetPersonaSuccessfulRequests,
			WsPerformanceCounters.TotalSyncConversationSuccessfulRequests,
			WsPerformanceCounters.TotalGetTimeZoneOffsetsSuccessfulRequests,
			WsPerformanceCounters.TotalGetEventsSuccessfulRequests,
			WsPerformanceCounters.TotalGetStreamingEventsSuccessfulRequests,
			WsPerformanceCounters.TotalGetFolderSuccessfulRequests,
			WsPerformanceCounters.TotalGetMailTipsSuccessfulRequests,
			WsPerformanceCounters.TotalPlayOnPhoneSuccessfulRequests,
			WsPerformanceCounters.TotalGetPhoneCallInformationSuccessfulRequests,
			WsPerformanceCounters.TotalDisconnectPhoneCallSuccessfulRequests,
			WsPerformanceCounters.TotalCreateUMPromptSuccessfulRequests,
			WsPerformanceCounters.TotalGetUMPromptSuccessfulRequests,
			WsPerformanceCounters.TotalGetUMPromptNamesSuccessfulRequests,
			WsPerformanceCounters.TotalDeleteUMPromptsSuccessfulRequests,
			WsPerformanceCounters.TotalGetServiceConfigurationSuccessfulRequests,
			WsPerformanceCounters.TotalGetItemSuccessfulRequests,
			WsPerformanceCounters.TotalGetServerTimeZonesSuccessfulRequests,
			WsPerformanceCounters.TotalMoveFolderSuccessfulRequests,
			WsPerformanceCounters.TotalMoveItemSuccessfulRequests,
			WsPerformanceCounters.TotalResolveNamesSuccessfulRequests,
			WsPerformanceCounters.TotalSendItemSuccessfulRequests,
			WsPerformanceCounters.TotalSubscribeSuccessfulRequests,
			WsPerformanceCounters.TotalUnsubscribeSuccessfulRequests,
			WsPerformanceCounters.TotalUpdateFolderSuccessfulRequests,
			WsPerformanceCounters.TotalUpdateItemSuccessfulRequests,
			WsPerformanceCounters.TotalUpdateItemInRecoverableItemsSuccessfulRequests,
			WsPerformanceCounters.TotalCreateAttachmentSuccessfulRequests,
			WsPerformanceCounters.TotalDeleteAttachmentSuccessfulRequests,
			WsPerformanceCounters.TotalGetAttachmentSuccessfulRequests,
			WsPerformanceCounters.TotalGetClientAccessTokenSuccessfulRequests,
			WsPerformanceCounters.TotalSendNotificationSuccessfulRequests,
			WsPerformanceCounters.TotalSyncFolderItemsSuccessfulRequests,
			WsPerformanceCounters.TotalSyncFolderHierarchySuccessfulRequests,
			WsPerformanceCounters.TotalConvertIdSuccessfulRequests,
			WsPerformanceCounters.TotalGetDelegateSuccessfulRequests,
			WsPerformanceCounters.TotalAddDelegateSuccessfulRequests,
			WsPerformanceCounters.TotalRemoveDelegateSuccessfulRequests,
			WsPerformanceCounters.TotalUpdateDelegateSuccessfulRequests,
			WsPerformanceCounters.TotalCreateUserConfigurationSuccessfulRequests,
			WsPerformanceCounters.TotalDeleteUserConfigurationSuccessfulRequests,
			WsPerformanceCounters.TotalGetUserConfigurationSuccessfulRequests,
			WsPerformanceCounters.TotalUpdateUserConfigurationSuccessfulRequests,
			WsPerformanceCounters.TotalGetUserAvailabilitySuccessfulRequests,
			WsPerformanceCounters.TotalGetUserOofSettingsSuccessfulRequests,
			WsPerformanceCounters.TotalSetUserOofSettingsSuccessfulRequests,
			WsPerformanceCounters.TotalGetSharingMetadataSuccessfulRequests,
			WsPerformanceCounters.TotalRefreshSharingFolderSuccessfulRequests,
			WsPerformanceCounters.TotalGetSharingFolderSuccessfulRequests,
			WsPerformanceCounters.TotalGetRoomListsSuccessfulRequests,
			WsPerformanceCounters.TotalGetRoomsSuccessfulRequests,
			WsPerformanceCounters.TotalPerformReminderActionSuccessfulRequests,
			WsPerformanceCounters.TotalGetRemindersSuccessfulRequests,
			WsPerformanceCounters.TotalProvisionSuccessfulRequests,
			WsPerformanceCounters.TotalDeprovisionSuccessfulRequests,
			WsPerformanceCounters.TotalLogPushNotificationDataSuccessfulRequests,
			WsPerformanceCounters.TotalFindMessageTrackingReportSuccessfulRequests,
			WsPerformanceCounters.TotalGetMessageTrackingReportSuccessfulRequests,
			WsPerformanceCounters.TotalApplyConversationActionSuccessfulRequests,
			WsPerformanceCounters.TotalEmptyFolderSuccessfulRequests,
			WsPerformanceCounters.TotalUploadItemsSuccessfulRequests,
			WsPerformanceCounters.TotalExportItemsSuccessfulRequests,
			WsPerformanceCounters.TotalExecuteDiagnosticMethodSuccessfulRequests,
			WsPerformanceCounters.TotalFindMailboxStatisticsByKeywordsSuccessfulRequests,
			WsPerformanceCounters.TotalGetSearchableMailboxesSuccessfulRequests,
			WsPerformanceCounters.TotalSearchMailboxesSuccessfulRequests,
			WsPerformanceCounters.TotalGetDiscoverySearchConfigurationSuccessfulRequests,
			WsPerformanceCounters.TotalGetHoldOnMailboxesSuccessfulRequests,
			WsPerformanceCounters.TotalSetHoldOnMailboxesSuccessfulRequests,
			WsPerformanceCounters.TotalGetNonIndexableItemStatisticsSuccessfulRequests,
			WsPerformanceCounters.TotalGetNonIndexableItemDetailsSuccessfulRequests,
			WsPerformanceCounters.TotalMarkAllItemsAsReadSuccessfulRequests,
			WsPerformanceCounters.TotalGetClientExtensionSuccessfulRequests,
			WsPerformanceCounters.TotalGetEncryptionConfigurationSuccessfulRequests,
			WsPerformanceCounters.TotalSetClientExtensionSuccessfulRequests,
			WsPerformanceCounters.TotalSetEncryptionConfigurationSuccessfulRequests,
			WsPerformanceCounters.TotalSubscribeToPushNotificationSuccessfulRequests,
			WsPerformanceCounters.TotalUnsubscribeToPushNotificationSuccessfulRequests,
			WsPerformanceCounters.TotalCreateUnifiedMailboxSuccessfulRequests,
			WsPerformanceCounters.TotalGetAppManifestsSuccessfulRequests,
			WsPerformanceCounters.TotalInstallAppSuccessfulRequests,
			WsPerformanceCounters.TotalUninstallAppSuccessfulRequests,
			WsPerformanceCounters.TotalDisableAppSuccessfulRequests,
			WsPerformanceCounters.TotalGetAppMarketplaceUrlSuccessfulRequests,
			WsPerformanceCounters.TotalGetClientExtensionTokenSuccessfulRequests,
			WsPerformanceCounters.TotalGetEncryptionConfigurationTokenSuccessfulRequests,
			WsPerformanceCounters.TotalGetConversationItemsSuccessfulRequests,
			WsPerformanceCounters.TotalGetModernConversationItemsSuccessfulRequests,
			WsPerformanceCounters.TotalGetThreadedConversationItemsSuccessfulRequests,
			WsPerformanceCounters.TotalGetModernConversationAttachmentsSuccessfulRequests,
			WsPerformanceCounters.TotalSetModernGroupMembershipSuccessfulRequests,
			WsPerformanceCounters.TotalGetUserRetentionPolicyTagsSuccessfulRequests,
			WsPerformanceCounters.TotalGetUserPhotoSuccessfulRequests,
			WsPerformanceCounters.TotalStartFindInGALSpeechRecognitionRequests,
			WsPerformanceCounters.StartFindInGALSpeechRecognitionAverageResponseTime,
			WsPerformanceCounters.TotalStartFindInGALSpeechRecognitionSuccessfulRequests,
			WsPerformanceCounters.TotalCompleteFindInGALSpeechRecognitionRequests,
			WsPerformanceCounters.CompleteFindInGALSpeechRecognitionAverageResponseTime,
			WsPerformanceCounters.TotalCompleteFindInGALSpeechRecognitionSuccessfulRequests,
			WsPerformanceCounters.TotalCreateUMCallDataRecordRequests,
			WsPerformanceCounters.CreateUMCallDataRecordAverageResponseTime,
			WsPerformanceCounters.TotalCreateUMCallDataRecordSuccessfulRequests,
			WsPerformanceCounters.TotalGetUMCallDataRecordsRequests,
			WsPerformanceCounters.GetUMCallDataRecordsAverageResponseTime,
			WsPerformanceCounters.TotalGetUMCallDataRecordsSuccessfulRequests,
			WsPerformanceCounters.TotalGetUMCallSummaryRequests,
			WsPerformanceCounters.GetUMCallSummaryAverageResponseTime,
			WsPerformanceCounters.TotalGetUMCallSummarySuccessfulRequests,
			WsPerformanceCounters.TotalGetUserPhotoDataRequests,
			WsPerformanceCounters.GetUserPhotoDataAverageResponseTime,
			WsPerformanceCounters.TotalGetUserPhotoDataSuccessfulRequests,
			WsPerformanceCounters.TotalInitUMMailboxRequests,
			WsPerformanceCounters.InitUMMailboxAverageResponseTime,
			WsPerformanceCounters.TotalInitUMMailboxSuccessfulRequests,
			WsPerformanceCounters.TotalResetUMMailboxRequests,
			WsPerformanceCounters.ResetUMMailboxAverageResponseTime,
			WsPerformanceCounters.TotalResetUMMailboxSuccessfulRequests,
			WsPerformanceCounters.TotalValidateUMPinRequests,
			WsPerformanceCounters.ValidateUMPinAverageResponseTime,
			WsPerformanceCounters.TotalValidateUMPinSuccessfulRequests,
			WsPerformanceCounters.TotalSaveUMPinRequests,
			WsPerformanceCounters.SaveUMPinAverageResponseTime,
			WsPerformanceCounters.TotalSaveUMPinSuccessfulRequests,
			WsPerformanceCounters.TotalGetUMPinRequests,
			WsPerformanceCounters.GetUMPinAverageResponseTime,
			WsPerformanceCounters.TotalGetUMPinSuccessfulRequests,
			WsPerformanceCounters.TotalGetClientIntentRequests,
			WsPerformanceCounters.GetClientIntentAverageResponseTime,
			WsPerformanceCounters.TotalGetClientIntentSuccessfulRequests,
			WsPerformanceCounters.TotalGetUMSubscriberCallAnsweringDataRequests,
			WsPerformanceCounters.GetUMSubscriberCallAnsweringDataAverageResponseTime,
			WsPerformanceCounters.TotalGetUMSubscriberCallAnsweringDataSuccessfulRequests,
			WsPerformanceCounters.TotalUpdateMailboxAssociationRequests,
			WsPerformanceCounters.UpdateMailboxAssociationAverageResponseTime,
			WsPerformanceCounters.TotalUpdateMailboxAssociationSuccessfulRequests,
			WsPerformanceCounters.TotalUpdateGroupMailboxRequests,
			WsPerformanceCounters.UpdateGroupMailboxAverageResponseTime,
			WsPerformanceCounters.TotalUpdateGroupMailboxSuccessfulRequests,
			WsPerformanceCounters.TotalGetCalendarEventRequests,
			WsPerformanceCounters.GetCalendarEventAverageResponseTime,
			WsPerformanceCounters.TotalGetCalendarEventSuccessfulRequests,
			WsPerformanceCounters.TotalGetCalendarViewRequests,
			WsPerformanceCounters.GetCalendarViewAverageResponseTime,
			WsPerformanceCounters.TotalGetCalendarViewSuccessfulRequests,
			WsPerformanceCounters.TotalGetBirthdayCalendarViewRequests,
			WsPerformanceCounters.GetBirthdayCalendarViewAverageResponseTime,
			WsPerformanceCounters.TotalGetBirthdayCalendarViewSuccessfulRequests,
			WsPerformanceCounters.TotalCreateCalendarEventRequests,
			WsPerformanceCounters.CreateCalendarEventAverageResponseTime,
			WsPerformanceCounters.TotalCreateCalendarEventSuccessfulRequests,
			WsPerformanceCounters.TotalUpdateCalendarEventRequests,
			WsPerformanceCounters.UpdateCalendarEventAverageResponseTime,
			WsPerformanceCounters.TotalUpdateCalendarEventSuccessfulRequests,
			WsPerformanceCounters.TotalCancelCalendarEventRequests,
			WsPerformanceCounters.CancelCalendarEventAverageResponseTime,
			WsPerformanceCounters.TotalCancelCalendarEventSuccessfulRequests,
			WsPerformanceCounters.TotalRespondToCalendarEventRequests,
			WsPerformanceCounters.RespondToCalendarEventAverageResponseTime,
			WsPerformanceCounters.TotalRespondToCalendarEventSuccessfulRequests,
			WsPerformanceCounters.TotalRefreshGALContactsFolderRequests,
			WsPerformanceCounters.RefreshGALContactsFolderAverageResponseTime,
			WsPerformanceCounters.TotalRefreshGALContactsFolderSuccessfulRequests,
			WsPerformanceCounters.TotalSubscribeToConversationChangesRequests,
			WsPerformanceCounters.SubscribeToConversationChangesAverageResponseTime,
			WsPerformanceCounters.TotalSubscribeToConversationChangesSuccessfulRequests,
			WsPerformanceCounters.TotalSubscribeToHierarchyChangesRequests,
			WsPerformanceCounters.SubscribeToHierarchyChangesAverageResponseTime,
			WsPerformanceCounters.TotalSubscribeToHierarchyChangesSuccessfulRequests,
			WsPerformanceCounters.TotalSubscribeToMessageChangesRequests,
			WsPerformanceCounters.SubscribeToMessageChangesAverageResponseTime,
			WsPerformanceCounters.TotalSubscribeToMessageChangesSuccessfulRequests,
			WsPerformanceCounters.TotalDeleteCalendarEventRequests,
			WsPerformanceCounters.DeleteCalendarEventAverageResponseTime,
			WsPerformanceCounters.TotalDeleteCalendarEventSuccessfulRequests,
			WsPerformanceCounters.TotalForwardCalendarEventRequests,
			WsPerformanceCounters.ForwardCalendarEventAverageResponseTime,
			WsPerformanceCounters.TotalForwardCalendarEventSuccessfulRequests,
			WsPerformanceCounters.LikeItemRequests,
			WsPerformanceCounters.LikeItemAverageResponseTime,
			WsPerformanceCounters.LikeItemSuccessfulRequests,
			WsPerformanceCounters.GetLikersRequests,
			WsPerformanceCounters.GetLikersAverageResponseTime,
			WsPerformanceCounters.GetLikersSuccessfulRequests,
			WsPerformanceCounters.TotalExpandCalendarEventRequests,
			WsPerformanceCounters.ExpandCalendarEventAverageResponseTime,
			WsPerformanceCounters.TotalExpandCalendarEventSuccessfulRequests,
			WsPerformanceCounters.GetConversationItemsDiagnosticsRequests,
			WsPerformanceCounters.GetConversationItemsDiagnosticsAverageResponseTime,
			WsPerformanceCounters.GetConversationItemsDiagnosticsSuccessfulRequests,
			WsPerformanceCounters.TotalGetComplianceConfiguration,
			WsPerformanceCounters.GetComplianceConfigurationAverageResponseTime,
			WsPerformanceCounters.TotalGetComplianceConfigurationSuccessfulRequests,
			WsPerformanceCounters.TotalPerformInstantSearchRequests,
			WsPerformanceCounters.PerformInstantSearchAverageResponseTime,
			WsPerformanceCounters.PerformInstantSearchSuccessfulRequests,
			WsPerformanceCounters.TotalEndInstantSearchSessionRequests,
			WsPerformanceCounters.EndInstantSearchSessionAverageResponseTime,
			WsPerformanceCounters.EndInstantSearchSessionSuccessfulRequests,
			WsPerformanceCounters.GetUserUnifiedGroupsRequests,
			WsPerformanceCounters.GetUserUnifiedGroupsAverageResponseTime,
			WsPerformanceCounters.GetUserUnifiedGroupsSuccessfulRequests,
			WsPerformanceCounters.TotalGetPeopleICommunicateWithRequests,
			WsPerformanceCounters.GetPeopleICommunicateWithAverageResponseTime,
			WsPerformanceCounters.TotalGetPeopleICommunicateWithSuccessfulRequests,
			WsPerformanceCounters.MaskAutoCompleteRecipientRequests,
			WsPerformanceCounters.MaskAutoCompleteRecipientAverageResponseTime,
			WsPerformanceCounters.TotalMaskAutoCompleteRecipientSuccessfulRequests,
			WsPerformanceCounters.TotalGetClutterStateRequests,
			WsPerformanceCounters.GetClutterStateAverageResponseTime,
			WsPerformanceCounters.TotalGetClutterStateSuccessfulRequests,
			WsPerformanceCounters.TotalSetClutterStateRequests,
			WsPerformanceCounters.SetClutterStateAverageResponseTime,
			WsPerformanceCounters.TotalSetClutterStateSuccessfulRequests
		};
	}
}
