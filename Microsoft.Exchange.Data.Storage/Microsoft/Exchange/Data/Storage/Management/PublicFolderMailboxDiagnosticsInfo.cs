using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class PublicFolderMailboxDiagnosticsInfo : ConfigurableObject
	{
		public PublicFolderMailboxDiagnosticsInfo(string displayName) : base(new SimplePropertyBag(SimpleProviderObjectSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			if (string.IsNullOrEmpty(displayName))
			{
				throw new ArgumentNullException("displayName");
			}
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = new PublicFolderMailboxDiagnosticsInfoId();
			this.DisplayName = displayName;
			this.propertyBag.ResetChangeTracking();
		}

		public string DisplayName
		{
			get
			{
				return (string)this[PublicFolderDiagnosticsInfoSchema.DisplayName];
			}
			private set
			{
				this[PublicFolderDiagnosticsInfoSchema.DisplayName] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return PublicFolderMailboxDiagnosticsInfo.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		public PublicFolderMailboxSynchronizerInfo SyncInfo
		{
			get
			{
				return (PublicFolderMailboxSynchronizerInfo)this[PublicFolderDiagnosticsInfoSchema.SyncInfo];
			}
			set
			{
				this[PublicFolderDiagnosticsInfoSchema.SyncInfo] = value;
			}
		}

		public PublicFolderMailboxAssistantInfo AssistantInfo
		{
			get
			{
				return (PublicFolderMailboxAssistantInfo)this[PublicFolderDiagnosticsInfoSchema.AssistantInfo];
			}
			set
			{
				this[PublicFolderDiagnosticsInfoSchema.AssistantInfo] = value;
			}
		}

		public PublicFolderMailboxDumpsterInfo DumpsterInfo
		{
			get
			{
				return (PublicFolderMailboxDumpsterInfo)this[PublicFolderDiagnosticsInfoSchema.DumpsterInfo];
			}
			set
			{
				this[PublicFolderDiagnosticsInfoSchema.DumpsterInfo] = value;
			}
		}

		public PublicFolderMailboxHierarchyInfo HierarchyInfo
		{
			get
			{
				return (PublicFolderMailboxHierarchyInfo)this[PublicFolderDiagnosticsInfoSchema.HierarchyInfo];
			}
			set
			{
				this[PublicFolderDiagnosticsInfoSchema.HierarchyInfo] = value;
			}
		}

		internal static PublicFolderMailboxDiagnosticsInfo Load(OrganizationId organizationId, Guid contentMailboxGuid, DiagnosticsLoadFlags loadFlags, Action<LocalizedString, LocalizedString, int> writeProgress)
		{
			PublicFolderMailboxDiagnosticsInfo result;
			using (PublicFolderSession publicFolderSession = PublicFolderSession.OpenAsAdmin(organizationId, null, contentMailboxGuid, null, CultureInfo.CurrentCulture, "Client=Management;Action=Get-PublicFolderMailboxDiagnostics", null))
			{
				result = PublicFolderMailboxDiagnosticsInfo.Load(publicFolderSession, loadFlags, writeProgress);
			}
			return result;
		}

		internal static PublicFolderMailboxDiagnosticsInfo Load(PublicFolderSession session, DiagnosticsLoadFlags loadFlags, Action<LocalizedString, LocalizedString, int> writeProgress)
		{
			PublicFolderMailboxDiagnosticsInfo publicFolderMailboxDiagnosticsInfo = new PublicFolderMailboxDiagnosticsInfo("Public Folder Diagnostics Information");
			publicFolderMailboxDiagnosticsInfo.SyncInfo = (PublicFolderMailboxDiagnosticsInfo.LoadMailboxInfo<PublicFolderMailboxSynchronizerInfo>(session, "PublicFolderSyncInfo", "PublicFolderLastSyncCylceLog") as PublicFolderMailboxSynchronizerInfo);
			publicFolderMailboxDiagnosticsInfo.AssistantInfo = (PublicFolderMailboxDiagnosticsInfo.LoadMailboxInfo<PublicFolderMailboxAssistantInfo>(session, "PublicFolderAssistantInfo", "PublicFolderLastAssistantCycleLog") as PublicFolderMailboxAssistantInfo);
			if ((loadFlags & DiagnosticsLoadFlags.DumpsterInfo) != DiagnosticsLoadFlags.Default)
			{
				publicFolderMailboxDiagnosticsInfo.DumpsterInfo = PublicFolderMailboxDumpsterInfo.LoadInfo(session, writeProgress);
			}
			if ((loadFlags & DiagnosticsLoadFlags.HierarchyInfo) != DiagnosticsLoadFlags.Default)
			{
				publicFolderMailboxDiagnosticsInfo.HierarchyInfo = PublicFolderMailboxHierarchyInfo.LoadInfo(session, writeProgress);
			}
			return publicFolderMailboxDiagnosticsInfo;
		}

		private static PublicFolderMailboxMonitoringInfo LoadMailboxInfo<TValue>(PublicFolderSession session, string stateInfoConfigurationName, string logInfoConfigurationName) where TValue : PublicFolderMailboxMonitoringInfo, new()
		{
			TValue tvalue = Activator.CreateInstance<TValue>();
			using (Folder folder = Folder.Bind(session, session.GetTombstonesRootFolderId()))
			{
				using (UserConfiguration configuration = UserConfiguration.GetConfiguration(folder, new UserConfigurationName(stateInfoConfigurationName, ConfigurationNameKind.Name), UserConfigurationTypes.Dictionary))
				{
					tvalue.LastAttemptedSyncTime = (PublicFolderMailboxDiagnosticsInfo.GetMetadataValue(configuration, "LastAttemptedSyncTime") as ExDateTime?);
					tvalue.LastFailedSyncTime = (PublicFolderMailboxDiagnosticsInfo.GetMetadataValue(configuration, "LastFailedSyncTime") as ExDateTime?);
					tvalue.LastSuccessfulSyncTime = (PublicFolderMailboxDiagnosticsInfo.GetMetadataValue(configuration, "LastSuccessfulSyncTime") as ExDateTime?);
					tvalue.LastSyncFailure = (PublicFolderMailboxDiagnosticsInfo.GetMetadataValue(configuration, "LastSyncFailure") as string);
					tvalue.NumberofAttemptsAfterLastSuccess = (PublicFolderMailboxDiagnosticsInfo.GetMetadataValue(configuration, "NumberofAttemptsAfterLastSuccess") as int?);
					tvalue.FirstFailedSyncTimeAfterLastSuccess = (PublicFolderMailboxDiagnosticsInfo.GetMetadataValue(configuration, "FirstFailedSyncTimeAfterLastSuccess") as ExDateTime?);
					PublicFolderMailboxSynchronizerInfo publicFolderMailboxSynchronizerInfo = tvalue as PublicFolderMailboxSynchronizerInfo;
					if (publicFolderMailboxSynchronizerInfo != null)
					{
						publicFolderMailboxSynchronizerInfo.NumberOfBatchesExecuted = (PublicFolderMailboxDiagnosticsInfo.GetMetadataValue(configuration, "NumberOfBatchesExecuted") as int?);
						publicFolderMailboxSynchronizerInfo.NumberOfFoldersToBeSynced = (PublicFolderMailboxDiagnosticsInfo.GetMetadataValue(configuration, "NumberOfFoldersToBeSynced") as int?);
						publicFolderMailboxSynchronizerInfo.NumberOfFoldersSynced = (PublicFolderMailboxDiagnosticsInfo.GetMetadataValue(configuration, "NumberOfFoldersSynced") as int?);
						publicFolderMailboxSynchronizerInfo.BatchSize = (PublicFolderMailboxDiagnosticsInfo.GetMetadataValue(configuration, "BatchSize") as int?);
					}
				}
				using (UserConfiguration configuration2 = UserConfiguration.GetConfiguration(folder, new UserConfigurationName(logInfoConfigurationName, ConfigurationNameKind.Name), UserConfigurationTypes.Stream))
				{
					using (Stream stream = configuration2.GetStream())
					{
						using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress, true))
						{
							using (MemoryStream memoryStream = new MemoryStream())
							{
								gzipStream.CopyTo(memoryStream);
								tvalue.LastSyncCycleLog = Encoding.ASCII.GetString(memoryStream.ToArray());
							}
						}
					}
				}
			}
			return tvalue;
		}

		private static object GetMetadataValue(UserConfiguration metadata, string name)
		{
			IDictionary dictionary = metadata.GetDictionary();
			if (dictionary.Contains(name))
			{
				return dictionary[name];
			}
			return null;
		}

		private static readonly PublicFolderDiagnosticsInfoSchema schema = ObjectSchema.GetInstance<PublicFolderDiagnosticsInfoSchema>();
	}
}
