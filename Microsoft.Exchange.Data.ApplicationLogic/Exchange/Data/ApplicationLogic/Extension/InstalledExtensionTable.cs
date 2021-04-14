using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Security;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.ApplicationLogic.Owa;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class InstalledExtensionTable : DisposeTrackableBase
	{
		internal static bool IsMultiTenancyEnabled
		{
			get
			{
				return InstalledExtensionTable.isMultiTenancyEnabled.Member;
			}
		}

		internal SafeXmlDocument MasterTableXml
		{
			get
			{
				this.LoadXML();
				return this.masterTableXml;
			}
		}

		private InstalledExtensionTable(bool retrieveOnly1_0)
		{
			this.retrieveOnly1_0 = retrieveOnly1_0;
		}

		private static int UpdateCheckFrequencySeconds
		{
			get
			{
				if (InstalledExtensionTable.updateCheckFrequencySeconds == 0)
				{
					string text = ConfigurationManager.AppSettings["UpdateCheckFrequencySeconds"];
					int num = 0;
					if (text != null && int.TryParse(text, out num) && num > 0)
					{
						InstalledExtensionTable.updateCheckFrequencySeconds = num;
					}
					else
					{
						InstalledExtensionTable.updateCheckFrequencySeconds = 259200;
					}
					InstalledExtensionTable.Tracer.TraceDebug<int>(0L, "Agave Update Check Frequency: {0} seconds", InstalledExtensionTable.updateCheckFrequencySeconds);
				}
				return InstalledExtensionTable.updateCheckFrequencySeconds;
			}
		}

		internal Dictionary<string, string> RequestData
		{
			get
			{
				return this.requestData;
			}
		}

		internal static InstalledExtensionTable CreateInstalledExtensionTable(string domain, bool isUserScope, OrgEmptyMasterTableCache orgEmptyMasterTableCache, MailboxSession mailboxSession)
		{
			return InstalledExtensionTable.CreateInstalledExtensionTable(domain, isUserScope, orgEmptyMasterTableCache, null, mailboxSession, false);
		}

		internal static InstalledExtensionTable CreateInstalledExtensionTable(string domain, bool isUserScope, OrgEmptyMasterTableCache orgEmptyMasterTableCache, ExtensionsCache extensionsCache, MailboxSession mailboxSession, bool retrieveOnly1_0 = false)
		{
			StoreId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			InstalledExtensionTable installedExtensionTable = new InstalledExtensionTable(retrieveOnly1_0);
			bool flag = false;
			try
			{
				installedExtensionTable.masterTable = UserConfigurationHelper.GetFolderConfiguration(mailboxSession, defaultFolderId, "ExtensionMasterTable", UserConfigurationTypes.XML, true, false);
				if (isUserScope)
				{
					installedExtensionTable.userId = InstalledExtensionTable.GetWireUserId(InstalledExtensionTable.IsMultiTenancyEnabled ? DirectoryHelper.ReadADRecipient(mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, mailboxSession.MailboxOwner.MailboxInfo.IsArchive, mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid)) : null, mailboxSession.MailboxOwner.ObjectId);
				}
				installedExtensionTable.domain = (domain ?? mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.Domain);
				installedExtensionTable.orgEmptyMasterTableCache = orgEmptyMasterTableCache;
				installedExtensionTable.isOrgMailboxSession = (orgEmptyMasterTableCache != null);
				installedExtensionTable.isUserScope = isUserScope;
				installedExtensionTable.masterTableXml = null;
				installedExtensionTable.extensionsCache = extensionsCache;
				installedExtensionTable.sessionMailboxOwner = mailboxSession.MailboxOwner;
				installedExtensionTable.sessionPreferedCulture = mailboxSession.PreferedCulture;
				installedExtensionTable.sessionClientInfoString = mailboxSession.ClientInfoString;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					installedExtensionTable.Dispose();
					installedExtensionTable = null;
				}
			}
			return installedExtensionTable;
		}

		internal static Exception RunClientExtensionAction(Action action)
		{
			Exception result = null;
			try
			{
				action();
			}
			catch (OwaExtensionOperationException ex)
			{
				result = ex;
			}
			catch (StoragePermanentException ex2)
			{
				result = ex2;
			}
			catch (StorageTransientException ex3)
			{
				result = ex3;
			}
			return result;
		}

		internal static Dictionary<string, ExtensionData> GetDefaultExtensions(IExchangePrincipal mailboxOwner)
		{
			if (InstalledExtensionTable.defaultExtensionTable == null)
			{
				lock (InstalledExtensionTable.defaultExtensionTableLock)
				{
					if (InstalledExtensionTable.defaultExtensionTable == null)
					{
						InstalledExtensionTable.defaultExtensionTable = new DefaultExtensionTable(mailboxOwner, "GetDefaultExtensions");
					}
				}
			}
			return InstalledExtensionTable.defaultExtensionTable.DefaultExtensions;
		}

		public static string GetWireUserId(ADRawEntry adRawEntry, ADObjectId adObjectId)
		{
			if (!InstalledExtensionTable.IsMultiTenancyEnabled)
			{
				return adObjectId.ObjectGuid.ToString();
			}
			string text = adRawEntry[ADRecipientSchema.ExternalDirectoryObjectId] as string;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			InstalledExtensionTable.Tracer.TraceDebug(0L, "ExternalDirectoryObjectId is not configured for user " + adObjectId.ObjectGuid.ToString());
			return string.Empty;
		}

		public void InstallExtension(ExtensionData extensionData, bool overwriteConflict)
		{
			if (extensionData.Type == ExtensionType.MarketPlace && !this.isOrgMailboxSession)
			{
				InstalledExtensionTable.ValidateAndRemoveManifestSignature(extensionData.Manifest, extensionData.ExtensionId, true);
			}
			else
			{
				InstalledExtensionTable.Tracer.TraceDebug<ExtensionType?, string>((long)this.GetHashCode(), "Skipping Signature Validation and Removal as this is only done for Marketplace Apps. Type: {0}, Id: {1}", extensionData.Type, extensionData.ExtensionId);
			}
			ExtensionData.ValidateManifestSize((long)extensionData.Manifest.OuterXml.Length, true);
			if (this.isUserScope || this.isOrgMailboxSession)
			{
				this.AddExtension(extensionData, overwriteConflict);
				this.SaveXML();
				return;
			}
			OrgExtensionTable.SetOrgExtension(this.domain, 0, null, extensionData);
		}

		internal void AddExtension(ExtensionData extensionData, bool overwriteConflict)
		{
			string text = ExtensionDataHelper.FormatExtensionId(extensionData.ExtensionId);
			if (extensionData.Version == null)
			{
				throw new OwaExtensionOperationException(Strings.ErrorExtensionVersionInvalid);
			}
			bool flag = false;
			ExtensionData.ParseEtoken(extensionData.Etoken, extensionData.ExtensionId, this.domain, extensionData.MarketplaceAssetID, false, this.isOrgMailboxSession);
			ExtensionData extensionData2;
			if (this.TryGetExtension(text, out extensionData2))
			{
				if (ExtensionInstallScope.Default == extensionData2.Scope)
				{
					throw new OwaExtensionOperationException(Strings.ErrorCantOverwriteDefaultExtension);
				}
				if (overwriteConflict)
				{
					if (ExtensionInstallScope.User == extensionData2.Scope)
					{
						flag = true;
					}
				}
				else if (this.isUserScope && ExtensionInstallScope.Organization == extensionData2.Scope)
				{
					if (ExtensionType.MarketPlace != extensionData.Type)
					{
						throw new OwaExtensionOperationException(Strings.ErrorExtensionWithIdAlreadyInstalledForOrg);
					}
					throw new OwaExtensionOperationException(Strings.ErrorExtensionAlreadyInstalledForOrg);
				}
				else
				{
					if (extensionData2.Version == null)
					{
						throw new OwaExtensionOperationException(Strings.ErrorExtensionUnableToUpgrade(extensionData.DisplayName));
					}
					if (extensionData.Version >= extensionData2.Version)
					{
						InstalledExtensionTable.Tracer.TraceInformation(this.GetHashCode(), 0L, string.Format("Removing the old version extension {0} {1}.", extensionData.DisplayName, extensionData2.VersionAsString));
						flag = true;
					}
					else
					{
						if (ExtensionType.MarketPlace != extensionData.Type)
						{
							throw new OwaExtensionOperationException(Strings.ErrorExtensionWithIdAlreadyInstalled);
						}
						throw new OwaExtensionOperationException(Strings.ErrorExtensionAlreadyInstalled);
					}
				}
			}
			this.ReplaceExtension(extensionData, flag ? extensionData2 : null, text);
		}

		public void UninstallExtension(string id)
		{
			if (this.isUserScope || this.isOrgMailboxSession)
			{
				this.RemoveExtension(id, true, null);
				return;
			}
			OrgExtensionTable.SetOrgExtension(this.domain, 1, id, null);
		}

		internal void RemoveExtension(string id, bool shouldSave, Version schemaVersion = null)
		{
			string formattedId = ExtensionDataHelper.FormatExtensionId(id);
			Guid guid;
			if (!GuidHelper.TryParseGuid(formattedId, out guid))
			{
				throw new OwaExtensionOperationException(Strings.ErrorReasonInvalidID);
			}
			if (!this.TryConfigureExistingRecord(formattedId, delegate(XmlNode extensionNode)
			{
				ExtensionData extensionData;
				if (this.TryGetProvidedExtension(formattedId, out extensionData))
				{
					throw new OwaExtensionOperationException(Strings.ErrorCannotUninstallProvidedExtension(formattedId));
				}
				if (schemaVersion != null)
				{
					XmlNode xmlNode = extensionNode.SelectSingleNode("manifest");
					string text;
					string input;
					Version v;
					if (xmlNode == null || !ExtensionDataHelper.TryGetOfficeAppSchemaInfo(xmlNode, "http://schemas.microsoft.com/office/appforoffice/", out text, out input) || !Version.TryParse(input, out v) || schemaVersion != v)
					{
						return;
					}
				}
				extensionNode.ParentNode.RemoveChild(extensionNode);
			}, shouldSave))
			{
				throw new OwaExtensionOperationException(Strings.ErrorExtensionNotFound(formattedId));
			}
		}

		public void DisableExtension(string id, DisableReasonType disableReason)
		{
			string text = ExtensionDataHelper.FormatExtensionId(id);
			Guid guid;
			if (!GuidHelper.TryParseGuid(text, out guid))
			{
				throw new OwaExtensionOperationException(Strings.ErrorReasonInvalidID);
			}
			this.ConfigureLocalExtension(text, false, delegate(XmlNode extensionNode)
			{
				this.SetMetaDataNodeValue(extensionNode, "disablereason", disableReason.ToString());
				if (disableReason == DisableReasonType.OutlookClientPerformance)
				{
					this.SetMetaDataNodeValue(extensionNode, "appstatus", "4.1");
					return;
				}
				this.SetMetaDataNodeValue(extensionNode, "appstatus", "4.0");
			});
		}

		public bool TryGetExtension(string extensionId, out ExtensionData extensionData)
		{
			string text;
			return this.TryGetExtension(extensionId, out extensionData, false, out text);
		}

		public bool TryGetExtension(string extensionId, out ExtensionData extensionData, bool isDebug, out string rawOrgMasterTableXml)
		{
			string item = ExtensionDataHelper.FormatExtensionId(extensionId);
			List<ExtensionData> extensions = this.GetExtensions(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				item
			}, false, isDebug, out rawOrgMasterTableXml);
			if (extensions.Count == 1)
			{
				extensionData = extensions[0];
				return true;
			}
			extensionData = null;
			return false;
		}

		internal List<ExtensionData> GetExtensions(HashSet<string> formattedRequestedExtensionIds, bool shouldReturnEnabledOnly)
		{
			string text;
			return this.GetExtensions(formattedRequestedExtensionIds, shouldReturnEnabledOnly, true, false, out text, true);
		}

		internal List<ExtensionData> GetExtensions(HashSet<string> formattedRequestedExtensionIds, bool shouldReturnEnabledOnly, bool isDebug, out string orgMasterTableRawXml)
		{
			return this.GetExtensions(formattedRequestedExtensionIds, shouldReturnEnabledOnly, true, isDebug, out orgMasterTableRawXml, true);
		}

		internal List<ExtensionData> GetExtensions(HashSet<string> formattedRequestedExtensionIds, bool shouldReturnEnabledOnly, bool shouldFailOnGetOrgExtensionsTimeout, bool isDebug, out string orgMasterTableRawXml, bool filterOutDuplicateMasterTableExtensions = true)
		{
			List<ExtensionData> list = new List<ExtensionData>();
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				List<KeyValuePair<string, ExtensionData>> masterTableExtensions = this.GetMasterTableExtensions(formattedRequestedExtensionIds);
				Dictionary<string, ExtensionData> dictionary = this.CreateDictionaryFromExtensionList(masterTableExtensions);
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				Dictionary<string, ExtensionData> providedExtensions;
				if (shouldFailOnGetOrgExtensionsTimeout)
				{
					providedExtensions = this.GetProvidedExtensions(formattedRequestedExtensionIds, shouldReturnEnabledOnly, dictionary, isDebug, out orgMasterTableRawXml);
				}
				else
				{
					providedExtensions = this.GetProvidedExtensionsHandleTimeout(formattedRequestedExtensionIds, shouldReturnEnabledOnly, dictionary, isDebug, out orgMasterTableRawXml);
				}
				long num = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
				List<ExtensionData> mergedList = list;
				IEnumerable<KeyValuePair<string, ExtensionData>> masterTableExtensions2;
				if (!filterOutDuplicateMasterTableExtensions)
				{
					IEnumerable<KeyValuePair<string, ExtensionData>> enumerable = masterTableExtensions;
					masterTableExtensions2 = enumerable;
				}
				else
				{
					masterTableExtensions2 = dictionary;
				}
				this.AddMasterTableExtensionsToMergedList(mergedList, masterTableExtensions2, providedExtensions, shouldReturnEnabledOnly);
				this.AddProvidedExtensionsToMergedList(list, providedExtensions, dictionary, shouldReturnEnabledOnly);
				stopwatch.Stop();
				this.AddRequestData("GE", stopwatch.ElapsedMilliseconds.ToString());
				this.AddRequestData("GM", elapsedMilliseconds.ToString());
				this.AddRequestData("GP", num.ToString());
			}
			catch (Exception exception)
			{
				ExtensionDiagnostics.LogToDatacenterOnly(ApplicationLogicEventLogConstants.Tuple_GetExtensionsFailed, null, new object[]
				{
					this.isOrgMailboxSession ? "GetOrgExtensions" : "GetExtensions",
					ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.sessionMailboxOwner),
					ExtensionDiagnostics.GetLoggedExceptionString(exception)
				});
				throw;
			}
			if (!this.isOrgMailboxSession || !this.isUserScope)
			{
				ExtensionDiagnostics.LogToDatacenterOnly(ApplicationLogicEventLogConstants.Tuple_GetExtensionsSuccess, null, new object[]
				{
					this.isOrgMailboxSession ? "GetOrgExtensions" : "GetExtensions"
				});
			}
			return list;
		}

		internal Dictionary<string, ExtensionData> TestGetExtensionsFromUserFai()
		{
			return this.CreateDictionaryFromExtensionList(this.GetMasterTableExtensions(null));
		}

		private Dictionary<string, ExtensionData> CreateDictionaryFromExtensionList(List<KeyValuePair<string, ExtensionData>> masterTableExtensions)
		{
			Dictionary<string, ExtensionData> dictionary = new Dictionary<string, ExtensionData>(StringComparer.OrdinalIgnoreCase);
			foreach (KeyValuePair<string, ExtensionData> keyValuePair in masterTableExtensions)
			{
				Version schemaVersion = keyValuePair.Value.SchemaVersion;
				ExtensionData extensionData;
				if ((!this.retrieveOnly1_0 || !(schemaVersion != null) || !(schemaVersion != SchemaConstants.SchemaVersion1_0)) && (!dictionary.TryGetValue(keyValuePair.Key, out extensionData) || extensionData == null || (!(schemaVersion == null) && (!(extensionData.SchemaVersion != null) || !(extensionData.SchemaVersion > schemaVersion)))))
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return dictionary;
		}

		private void CopyExtensionMetaData(ExtensionData source, ExtensionData destination)
		{
			destination.IsMandatory = source.IsMandatory;
			destination.IsEnabledByDefault = source.IsEnabledByDefault;
			destination.Enabled = source.Enabled;
			destination.DisableReason = source.DisableReason;
			destination.SpecificUsers = source.SpecificUsers;
		}

		private void AddProvidedExtensionsToMergedList(List<ExtensionData> mergedList, Dictionary<string, ExtensionData> providedExtensions, Dictionary<string, ExtensionData> masterTableExtensions, bool shouldReturnEnabledOnly)
		{
			foreach (KeyValuePair<string, ExtensionData> keyValuePair in providedExtensions)
			{
				ExtensionData value = keyValuePair.Value;
				string key = ExtensionDataHelper.FormatExtensionId(value.ExtensionId);
				ExtensionData extensionData = value;
				ExtensionData extensionData2;
				if (masterTableExtensions.TryGetValue(key, out extensionData2))
				{
					if (this.isOrgMailboxSession)
					{
						extensionData = (value.Clone() as ExtensionData);
						this.CopyExtensionMetaData(extensionData2, extensionData);
					}
					else
					{
						extensionData.Enabled = (value.IsMandatory || extensionData2.Enabled);
					}
				}
				else if (this.isUserScope && !this.isOrgMailboxSession && extensionData.Enabled != (value.IsMandatory || value.IsEnabledByDefault))
				{
					extensionData = (value.Clone() as ExtensionData);
					extensionData.Enabled = (value.IsMandatory || value.IsEnabledByDefault);
					extensionData.EtokenData = value.EtokenData;
				}
				this.AddToMergedList(mergedList, shouldReturnEnabledOnly, extensionData);
			}
		}

		private void AddMasterTableExtensionsToMergedList(List<ExtensionData> mergedList, IEnumerable<KeyValuePair<string, ExtensionData>> masterTableExtensions, Dictionary<string, ExtensionData> providedExtensions, bool shouldReturnEnabledOnly)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Dictionary<string, ExtensionData> dictionary = new Dictionary<string, ExtensionData>(StringComparer.OrdinalIgnoreCase);
			Dictionary<string, ExtensionData> dictionary2 = new Dictionary<string, ExtensionData>(StringComparer.OrdinalIgnoreCase);
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			bool flag = false;
			foreach (KeyValuePair<string, ExtensionData> keyValuePair in masterTableExtensions)
			{
				ExtensionData extensionData = keyValuePair.Value;
				if (KillBitList.Singleton.IsExtensionKilled(extensionData.ExtensionId))
				{
					XmlNode masterTableNode = extensionData.MasterTableNode;
					masterTableNode.ParentNode.RemoveChild(masterTableNode);
					flag = true;
					InstalledExtensionTable.Tracer.TraceInformation(this.GetHashCode(), 0L, string.Format("The extension {0} has been removed by killbit.", extensionData.DisplayName));
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_AppInKillbitListRemoved, null, new object[]
					{
						"KillAppFromMailbox",
						extensionData.ExtensionId,
						ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.sessionMailboxOwner),
						extensionData.DisplayName
					});
				}
				else
				{
					string item = (extensionData.Version == null) ? extensionData.ExtensionId : (extensionData.ExtensionId + extensionData.Version.ToString());
					if (hashSet.Contains(item))
					{
						XmlNode masterTableNode2 = extensionData.MasterTableNode;
						masterTableNode2.ParentNode.RemoveChild(masterTableNode2);
						flag = true;
						InstalledExtensionTable.Tracer.TraceInformation(this.GetHashCode(), 0L, string.Format("The extension {0} has been removed because it's a duplicate entry in master table.", extensionData.DisplayName));
					}
					else
					{
						hashSet.Add(item);
						string key = ExtensionDataHelper.FormatExtensionId(extensionData.ExtensionId);
						if (!providedExtensions.ContainsKey(key) && extensionData.Manifest != null)
						{
							if (this.extensionsCache != null && extensionData.Type != null && extensionData.Type == ExtensionType.MarketPlace)
							{
								ExtensionData extensionData2;
								if (!dictionary.TryGetValue(extensionData.ExtensionId, out extensionData2))
								{
									byte[] manifestBytes = null;
									if (this.extensionsCache.TryGetExtensionUpdate(extensionData, out manifestBytes))
									{
										ExtensionData extensionData3;
										if (this.TryInstallUpdate(extensionData, manifestBytes, out extensionData3))
										{
											flag = true;
											extensionData = extensionData3;
										}
									}
									else
									{
										dictionary[extensionData.ExtensionId] = extensionData;
									}
								}
								else if (extensionData2.Version < extensionData.Version)
								{
									dictionary[extensionData.ExtensionId] = extensionData;
								}
								if (extensionData.EtokenData != null && extensionData.EtokenData.IsRenewalNeeded && !"2.1".Equals(extensionData.AppStatus, StringComparison.OrdinalIgnoreCase))
								{
									dictionary2[extensionData.ExtensionId] = extensionData;
								}
							}
							this.AddToMergedList(mergedList, shouldReturnEnabledOnly, extensionData);
						}
						extensionData.MasterTableNode = null;
					}
				}
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			this.TokenRenewCheck(dictionary2.Values);
			if (this.UpdateCheck(dictionary.Values))
			{
				flag = true;
			}
			long num = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
			if (flag)
			{
				this.SaveXmlIfNoConflict();
			}
			stopwatch.Stop();
			long num2 = stopwatch.ElapsedMilliseconds - num;
			this.AddRequestData("AM", elapsedMilliseconds.ToString());
			this.AddRequestData("CU", num.ToString());
			this.AddRequestData("SU", num2.ToString());
		}

		private bool TryInstallUpdate(ExtensionData masterTableExtension, byte[] manifestBytes, out ExtensionData updatedExtension)
		{
			bool result = false;
			updatedExtension = null;
			InstalledExtensionTable.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Installing update to extension {0}", masterTableExtension.MarketplaceAssetID);
			try
			{
				updatedExtension = ExtensionData.ParseOsfManifest(manifestBytes, manifestBytes.Length, masterTableExtension.MarketplaceAssetID, masterTableExtension.MarketplaceContentMarket, ExtensionType.MarketPlace, masterTableExtension.Scope.Value, masterTableExtension.Enabled, masterTableExtension.DisableReason, string.Empty, masterTableExtension.Etoken);
				this.ReplaceExtension(updatedExtension, masterTableExtension, ExtensionDataHelper.FormatExtensionId(masterTableExtension.ExtensionId));
				result = true;
			}
			catch (OwaExtensionOperationException ex)
			{
				InstalledExtensionTable.Tracer.TraceError<string, OwaExtensionOperationException>((long)this.GetHashCode(), "Update of extension {0} failed. Exception: {1}", masterTableExtension.MarketplaceAssetID, ex);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_ExtensionUpdateFailed, null, new object[]
				{
					"UpdateExtensionFromCache",
					ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.sessionMailboxOwner),
					masterTableExtension.MarketplaceAssetID,
					ExtensionDiagnostics.GetLoggedExceptionString(ex)
				});
			}
			ExtensionDiagnostics.LogToDatacenterOnly(ApplicationLogicEventLogConstants.Tuple_ExtensionUpdateSuccess, null, new object[]
			{
				"UpdateExtensionFromCache",
				ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.sessionMailboxOwner),
				masterTableExtension.MarketplaceAssetID
			});
			return result;
		}

		private void ReplaceExtension(ExtensionData extensionData, ExtensionData extensionDataToReplace, string formattedId)
		{
			if (extensionDataToReplace != null)
			{
				this.CopyExtensionMetaData(extensionDataToReplace, extensionData);
				bool flag = false;
				if (extensionData.SchemaVersion != null && extensionData.SchemaVersion == SchemaConstants.SchemaVersion1_0)
				{
					this.RemoveExtension(formattedId, false, null);
				}
				else if (extensionData.SchemaVersion != null && extensionData.SchemaVersion >= SchemaConstants.SchemaVersion1_1 && extensionDataToReplace.SchemaVersion != null && extensionDataToReplace.SchemaVersion == SchemaConstants.SchemaVersion1_0)
				{
					flag = true;
				}
				else
				{
					this.RemoveExtension(formattedId, false, extensionDataToReplace.SchemaVersion);
					flag = true;
				}
				if (flag && !string.IsNullOrEmpty(extensionData.Etoken))
				{
					this.ConfigureEtoken(extensionDataToReplace.ExtensionId, extensionData.Etoken, false);
				}
			}
			extensionData.InstalledByVersion = ExchangeSetupContext.InstalledVersion;
			this.masterTableXml.FirstChild.AppendChild(this.masterTableXml.ImportNode(extensionData.ConvertToXml(true, this.isOrgMailboxSession), true));
		}

		public static bool IsUpdateCheckTimeExpired(DateTime lastUpdateCheckTime)
		{
			return lastUpdateCheckTime.AddSeconds((double)InstalledExtensionTable.UpdateCheckFrequencySeconds) < DateTime.UtcNow;
		}

		private bool UpdateCheck(ICollection<ExtensionData> marketplaceExtensionQueryList)
		{
			bool result = false;
			if (this.extensionsCache != null)
			{
				XmlNode xmlNode;
				DateTime lastUpdateCheckTime = this.GetLastUpdateCheckTime(out xmlNode);
				if (InstalledExtensionTable.IsUpdateCheckTimeExpired(lastUpdateCheckTime))
				{
					if (marketplaceExtensionQueryList.Count > 0)
					{
						InstalledExtensionTable.Tracer.TraceDebug<int>((long)this.GetHashCode(), "Submitting update query for {0} extensions", marketplaceExtensionQueryList.Count);
						UpdateQueryContext queryContext = new UpdateQueryContext
						{
							Domain = this.domain,
							OrgEmptyMasterTableCache = this.orgEmptyMasterTableCache,
							IsUserScope = this.isUserScope,
							ExchangePrincipal = this.sessionMailboxOwner,
							CultureInfo = this.sessionPreferedCulture,
							ClientInfoString = ExtensionsCache.BuildClientInfoString(this.sessionClientInfoString)
						};
						this.extensionsCache.SubmitUpdateQuery(marketplaceExtensionQueryList, queryContext);
					}
					xmlNode.InnerText = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
					InstalledExtensionTable.Tracer.TraceDebug<string>((long)this.GetHashCode(), "LastUpdateCheckTime set to {0}", xmlNode.InnerText);
					result = true;
				}
			}
			return result;
		}

		private void TokenRenewCheck(ICollection<ExtensionData> marketplaceTokenRenewList)
		{
			if (marketplaceTokenRenewList.Count > 0 && this.extensionsCache != null)
			{
				InstalledExtensionTable.Tracer.TraceDebug<int>((long)this.GetHashCode(), "Submitting token renew query for {0} extensions", marketplaceTokenRenewList.Count);
				TokenRenewQueryContext queryContext = new TokenRenewQueryContext
				{
					Domain = this.domain,
					OrgEmptyMasterTableCache = this.orgEmptyMasterTableCache,
					IsUserScope = this.isUserScope,
					ExchangePrincipal = this.sessionMailboxOwner,
					CultureInfo = this.sessionPreferedCulture,
					ClientInfoString = ExtensionsCache.BuildClientInfoString(this.sessionClientInfoString)
				};
				this.extensionsCache.TokenRenewSubmitter.SubmitRenewQuery(marketplaceTokenRenewList, queryContext);
			}
		}

		public void ConfigureOrgExtension(string extensionId, bool isEnabled, bool isMandatory, bool isEnabledByDefault, ClientExtensionProvidedTo providedTo, string[] specificUsers)
		{
			if (this.isOrgMailboxSession)
			{
				this.ConfigureLocalExtension(extensionId, isEnabled, delegate(XmlNode extensionNode)
				{
					this.SetMetaDataNodeValue(extensionNode, "isMandatory", isMandatory.ToString());
					this.SetMetaDataNodeValue(extensionNode, "isEnabledByDefault", isEnabledByDefault.ToString());
					this.SetMetaDataNodeValue(extensionNode, "providedTo", providedTo.ToString());
					XmlNode xmlNode = extensionNode.SelectSingleNode("users");
					if (xmlNode != null)
					{
						extensionNode.RemoveChild(xmlNode);
					}
					ExtensionData.AppendXmlElement(this.masterTableXml, extensionNode, "users", "user", specificUsers);
				});
				return;
			}
			OrgExtensionTable.ConfigureOrgExtension(this.domain, extensionId, isEnabled, isMandatory, isEnabledByDefault, providedTo, specificUsers);
		}

		public void ConfigureUserExtension(string extensionId, bool isEnabled)
		{
			this.ConfigureLocalExtension(extensionId, isEnabled, null);
		}

		private void ConfigureLocalExtension(string extensionId, bool isEnabled, Action<XmlNode> configurationAction)
		{
			string text = ExtensionDataHelper.FormatExtensionId(extensionId);
			ExtensionData extensionData;
			bool flag = this.TryGetProvidedExtension(text, out extensionData);
			if (this.isUserScope && flag && extensionData.IsMandatory && !isEnabled)
			{
				throw new CannotDisableMandatoryExtensionException();
			}
			if (this.isUserScope)
			{
				string disableReasonString = isEnabled ? DisableReasonType.NotDisabled.ToString() : DisableReasonType.NoReason.ToString();
				Action<XmlNode> configurationAction2 = delegate(XmlNode extensionNode)
				{
					this.SetMetaDataNodeValue(extensionNode, "disablereason", disableReasonString);
				};
				if (!this.TryConfigureExistingRecord(text, configurationAction2, true))
				{
					if (extensionData == null)
					{
						throw new ExtensionNotFoundException(text);
					}
					this.AddConfigurationRecord(text, isEnabled, configurationAction2, extensionData);
				}
			}
			if (!this.TryConfigureExistingRecord(text, delegate(XmlNode extensionNode)
			{
				this.SetMetaDataNodeValue(extensionNode, "enabled", isEnabled.ToString());
				if (configurationAction != null)
				{
					configurationAction(extensionNode);
				}
			}, true))
			{
				this.AddConfigurationRecord(text, isEnabled, configurationAction, extensionData);
			}
		}

		public void ConfigureAppStatus(string appId, string appStatus)
		{
			Action<XmlNode> configurationAction = delegate(XmlNode extensionNode)
			{
				this.SetMetaDataNodeValue(extensionNode, "appstatus", appStatus);
			};
			this.TryConfigureExistingRecord(appId, configurationAction, true);
		}

		public void ConfigureEtoken(string appId, string etoken, bool shouldSave = true)
		{
			Action<XmlNode> configurationAction = delegate(XmlNode extensionNode)
			{
				this.SetMetaDataNodeValue(extensionNode, "entitlementToken", etoken);
			};
			this.TryConfigureExistingRecord(appId, configurationAction, shouldSave);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.masterTable != null)
			{
				this.masterTable.Dispose();
				this.masterTable = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<InstalledExtensionTable>(this);
		}

		private bool TryConfigureExistingRecord(string formattedId, Action<XmlNode> configurationAction, bool shouldSave = true)
		{
			this.LoadXML();
			bool flag = false;
			using (XmlNodeList xmlNodeList = this.masterTableXml.SelectNodes("/ExtensionList/Extension"))
			{
				if (xmlNodeList != null)
				{
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode = (XmlNode)obj;
						XmlNode xmlNode2 = xmlNode.SelectSingleNode("ExtensionId");
						if (xmlNode2 != null && string.Equals(formattedId, xmlNode2.InnerText, StringComparison.OrdinalIgnoreCase))
						{
							configurationAction(xmlNode);
							flag = true;
						}
					}
					if (shouldSave && flag)
					{
						this.SaveXML();
					}
				}
			}
			return flag;
		}

		private void AddConfigurationRecord(string formattedId, bool isEnabled, Action<XmlNode> configurationAction, ExtensionData providedExtension)
		{
			if (providedExtension != null)
			{
				if (this.isUserScope && isEnabled == (providedExtension.IsMandatory || providedExtension.IsEnabledByDefault))
				{
					return;
				}
				XmlNode xmlNode = this.masterTableXml.ImportNode(providedExtension.ConvertToXml(false, this.isOrgMailboxSession), true);
				xmlNode.SelectSingleNode("enabled").InnerText = isEnabled.ToString();
				if (configurationAction != null)
				{
					configurationAction(xmlNode);
				}
				this.masterTableXml.FirstChild.AppendChild(xmlNode);
				this.SaveXML();
			}
		}

		private void AddToMergedList(List<ExtensionData> list, bool shouldReturnEnabledOnly, ExtensionData extensionData)
		{
			if (!this.isUserScope || !shouldReturnEnabledOnly || extensionData.Enabled)
			{
				list.Add(extensionData);
			}
		}

		private void SetMetaDataNodeValue(XmlNode extensionNode, string nodeName, string value)
		{
			XmlNode xmlNode = extensionNode.SelectSingleNode(nodeName);
			if (xmlNode == null)
			{
				xmlNode = this.masterTableXml.CreateElement(nodeName);
				extensionNode.AppendChild(xmlNode);
			}
			xmlNode.InnerText = value;
		}

		private List<KeyValuePair<string, ExtensionData>> GetMasterTableExtensions(HashSet<string> formattedRequestedExtensionIds)
		{
			List<KeyValuePair<string, ExtensionData>> list = new List<KeyValuePair<string, ExtensionData>>();
			if (this.isUserScope || this.isOrgMailboxSession)
			{
				this.LoadXML();
				using (XmlNodeList extensionNodes = this.GetExtensionNodes())
				{
					if (extensionNodes != null && extensionNodes.Count > 0)
					{
						bool flag = false;
						int i = extensionNodes.Count - 1;
						while (i >= 0)
						{
							XmlNode xmlNode = extensionNodes.Item(i);
							ExtensionData extensionData;
							try
							{
								extensionData = ExtensionData.ConvertFromMasterTableXml(xmlNode, this.isOrgMailboxSession, this.domain);
								extensionData.MasterTableNode = xmlNode;
							}
							catch (OwaExtensionOperationException ex)
							{
								InstalledExtensionTable.Tracer.TraceError<string, string, string>((long)this.GetHashCode(), "Master table extension data is invalid:{0}{1}{0}Removing the invalid node:{0}{2}", Environment.NewLine, ex.ToString(), xmlNode.OuterXml);
								ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_InvalidExtensionRemoved, null, new object[]
								{
									"RemoveInvalidExtension",
									ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.sessionMailboxOwner),
									ExtensionDiagnostics.GetLoggedExceptionString(ex),
									xmlNode.OuterXml,
									Environment.NewLine + Environment.NewLine
								});
								xmlNode.ParentNode.RemoveChild(xmlNode);
								flag = true;
								goto IL_138;
							}
							goto IL_10E;
							IL_138:
							i--;
							continue;
							IL_10E:
							string text = ExtensionDataHelper.FormatExtensionId(extensionData.ExtensionId);
							if (formattedRequestedExtensionIds == null || formattedRequestedExtensionIds.Contains(text))
							{
								list.Add(new KeyValuePair<string, ExtensionData>(text, extensionData));
								goto IL_138;
							}
							goto IL_138;
						}
						if (flag)
						{
							this.SaveXmlIfNoConflict();
						}
					}
					else if (this.isOrgMailboxSession)
					{
						this.orgEmptyMasterTableCache.Update(this.sessionMailboxOwner.MailboxInfo.OrganizationId, true);
					}
				}
			}
			return list;
		}

		private DateTime GetLastUpdateCheckTime(out XmlNode lastUpdateCheckTimeXmlNode)
		{
			DateTime minValue = DateTime.MinValue;
			lastUpdateCheckTimeXmlNode = null;
			lastUpdateCheckTimeXmlNode = this.masterTableXml.SelectSingleNode("/ExtensionList/LastUpdateCheckTime");
			if (lastUpdateCheckTimeXmlNode == null)
			{
				InstalledExtensionTable.Tracer.TraceDebug((long)this.GetHashCode(), "lastUpdateCheckTimeXmlNode is null. Adding node.");
				XmlNode documentElement = this.masterTableXml.DocumentElement;
				lastUpdateCheckTimeXmlNode = this.masterTableXml.CreateElement("LastUpdateCheckTime");
				documentElement.AppendChild(lastUpdateCheckTimeXmlNode);
			}
			else if (lastUpdateCheckTimeXmlNode.InnerText != null && !DateTime.TryParse(lastUpdateCheckTimeXmlNode.InnerText, CultureInfo.InvariantCulture, DateTimeStyles.None, out minValue))
			{
				InstalledExtensionTable.Tracer.TraceDebug<string>((long)this.GetHashCode(), "lastUpdateCheckTimeXmlNode '{0}' parse failed.", lastUpdateCheckTimeXmlNode.InnerText);
			}
			return minValue;
		}

		private bool TryGetProvidedExtension(string formattedRequestedExtensionId, out ExtensionData providedExtension)
		{
			string text;
			Dictionary<string, ExtensionData> providedExtensions = this.GetProvidedExtensions(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				formattedRequestedExtensionId
			}, false, null, false, out text);
			return providedExtensions.TryGetValue(formattedRequestedExtensionId, out providedExtension);
		}

		private Dictionary<string, ExtensionData> GetProvidedExtensionsHandleTimeout(HashSet<string> formattedRequestedExtensionIds, bool shouldReturnEnabledOnly, Dictionary<string, ExtensionData> masterTableExtensions, bool isDebug, out string orgMasterTableRawXml)
		{
			Dictionary<string, ExtensionData> result;
			try
			{
				result = this.GetProvidedExtensions(formattedRequestedExtensionIds, shouldReturnEnabledOnly, masterTableExtensions, isDebug, out orgMasterTableRawXml);
			}
			catch (OwaExtensionOperationException ex)
			{
				InstalledExtensionTable.Tracer.TraceError<OwaExtensionOperationException>((long)this.GetHashCode(), "Exception thrown in InstalledExtensionTable.GetProvidedExtensions. Exception: {0}", ex);
				WebException ex2 = null;
				if (ex.InnerException != null)
				{
					ex2 = (ex.InnerException.InnerException as WebException);
				}
				if (ex2 == null || ex2.Status != WebExceptionStatus.Timeout)
				{
					throw;
				}
				InstalledExtensionTable.Tracer.TraceDebug((long)this.GetHashCode(), "Timeout in InstalledExtensionTable.GetProvidedExtensions. Returning empty list.");
				string loggedMailboxIdentifier = ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.sessionMailboxOwner);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_GetOrgExtensionsTimedOut, null, new object[]
				{
					"GetExtensionsHandleTimeout",
					loggedMailboxIdentifier,
					loggedMailboxIdentifier,
					ex2
				});
				orgMasterTableRawXml = string.Empty;
				result = new Dictionary<string, ExtensionData>();
			}
			return result;
		}

		private Dictionary<string, ExtensionData> GetProvidedExtensions(HashSet<string> formattedRequestedExtensionIds, bool shouldReturnEnabledOnly, Dictionary<string, ExtensionData> masterTableExtensions, bool isDebug, out string orgMasterTableRawXml)
		{
			if (!this.isOrgMailboxSession)
			{
				return this.GetOrgProvidedExtensions(formattedRequestedExtensionIds, shouldReturnEnabledOnly, masterTableExtensions, isDebug, out orgMasterTableRawXml);
			}
			orgMasterTableRawXml = (isDebug ? this.MasterTableXml.InnerXml : string.Empty);
			if (formattedRequestedExtensionIds == null)
			{
				return InstalledExtensionTable.GetDefaultExtensions(this.sessionMailboxOwner);
			}
			foreach (string key in formattedRequestedExtensionIds)
			{
				ExtensionData value;
				if (InstalledExtensionTable.GetDefaultExtensions(this.sessionMailboxOwner).TryGetValue(key, out value))
				{
					return new Dictionary<string, ExtensionData>(StringComparer.OrdinalIgnoreCase)
					{
						{
							key,
							value
						}
					};
				}
			}
			return new Dictionary<string, ExtensionData>();
		}

		private Dictionary<string, ExtensionData> GetOrgProvidedExtensions(HashSet<string> formattedRequestedExtensionIds, bool shouldReturnEnabledOnly, Dictionary<string, ExtensionData> masterTableExtensions, bool isDebug, out string orgMasterTableRawXml)
		{
			StringList stringList = null;
			StringList stringList2 = null;
			if (this.isUserScope && shouldReturnEnabledOnly)
			{
				foreach (KeyValuePair<string, ExtensionData> keyValuePair in masterTableExtensions)
				{
					if (keyValuePair.Value.Enabled)
					{
						if (stringList == null)
						{
							stringList = new StringList();
						}
						stringList.Add(keyValuePair.Key);
					}
					else
					{
						if (stringList2 == null)
						{
							stringList2 = new StringList();
						}
						stringList2.Add(keyValuePair.Key);
					}
				}
			}
			StringList requestedExtensionIds = null;
			if (formattedRequestedExtensionIds != null)
			{
				requestedExtensionIds = new StringList(formattedRequestedExtensionIds);
			}
			OrgExtensionTable.RequestData requestData;
			Dictionary<string, ExtensionData> orgExtensions = OrgExtensionTable.GetOrgExtensions(requestedExtensionIds, this.domain, shouldReturnEnabledOnly, this.isUserScope, this.userId, stringList, stringList2, out requestData, isDebug, out orgMasterTableRawXml, this.retrieveOnly1_0);
			if (requestData.ExchangeServiceUri != null)
			{
				this.AddRequestData("OrgHost", requestData.ExchangeServiceUri.Host);
				this.AddRequestData("EWSReqId", requestData.EwsRequestId);
				this.AddRequestData("GCE", requestData.GetClientExtensionTime.ToString());
			}
			this.AddRequestData("CES", requestData.CreateExchangeServiceTime.ToString());
			return orgExtensions;
		}

		private void LoadXML()
		{
			if (this.masterTableXml == null)
			{
				this.masterTableXml = new SafeXmlDocument();
				this.masterTableXml.PreserveWhitespace = true;
				using (Stream xmlStream = this.masterTable.GetXmlStream())
				{
					try
					{
						if (xmlStream != null && xmlStream.Length > 0L)
						{
							this.masterTableXml.Load(xmlStream);
						}
						else
						{
							InstalledExtensionTable.Tracer.TraceDebug((long)this.GetHashCode(), "The manifest xml is empty.");
						}
						if (string.IsNullOrEmpty(this.masterTableXml.InnerXml))
						{
							this.masterTableXml.InnerXml = "<ExtensionList />";
						}
					}
					catch (XmlException arg)
					{
						InstalledExtensionTable.Tracer.TraceDebug<XmlException>((long)this.GetHashCode(), "The manifest xml is corrupted.", arg);
						throw;
					}
				}
				return;
			}
		}

		private void AddRequestData(string key, string value)
		{
			if (this.requestData.ContainsKey(key))
			{
				Dictionary<string, string> dictionary;
				(dictionary = this.requestData)[key] = dictionary[key] + "," + value;
				return;
			}
			this.requestData.Add(key, value);
		}

		internal static int GetElapsedTime(DateTime startTime, DateTime endTime)
		{
			return endTime.Subtract(startTime).Milliseconds;
		}

		internal static bool ValidateAndRemoveManifestSignature(SafeXmlDocument safeXmlDocument, string extensionId, bool shouldThrowOnFailure = true)
		{
			if (InstalledExtensionTable.IsAppSignatureValidationDisabled())
			{
				InstalledExtensionTable.Tracer.TraceDebug<string>(0L, "App Signature Validation Disabled. Skipping Validation and Removal of Signature. App Id: {0}", extensionId);
				return true;
			}
			InstalledExtensionTable.Tracer.TraceDebug<string>(0L, "Do Signature Validation. Id: {0}", extensionId);
			if (SignedXMLVerifier.VerifySignedXml(safeXmlDocument))
			{
				InstalledExtensionTable.Tracer.TraceDebug<string, int>(0L, "Signature Validation succeeded. Id: {0}, XML Length: {1}", extensionId, safeXmlDocument.OuterXml.Length);
				SignedXMLVerifier.RemoveSignature(safeXmlDocument);
				InstalledExtensionTable.Tracer.TraceDebug<string, int>(0L, "Signature Removed. Id: {0}, XML Length: {1}", extensionId, safeXmlDocument.OuterXml.Length);
				return true;
			}
			if (shouldThrowOnFailure)
			{
				throw new OwaExtensionOperationException(Strings.ErrorManifestSignatureInvalidExtension);
			}
			return false;
		}

		internal ConflictResolutionResult SaveXmlIfNoConflict()
		{
			using (Stream xmlStream = this.masterTable.GetXmlStream())
			{
				xmlStream.SetLength(0L);
				this.masterTableXml.Save(xmlStream);
			}
			ConflictResolutionResult conflictResolutionResult = this.masterTable.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				string text = null;
				if (conflictResolutionResult.PropertyConflicts != null)
				{
					InstalledExtensionTable.Tracer.TraceError<int>((long)this.GetHashCode(), "UserConfiguration.Save returned '{0}' property conflicts.", conflictResolutionResult.PropertyConflicts.Length);
					StringBuilder stringBuilder = new StringBuilder();
					foreach (PropertyConflict propertyConflict in conflictResolutionResult.PropertyConflicts)
					{
						stringBuilder.AppendLine(string.Format("Property conflict: DisplayName: '{0}', Resolvable: '{1}', OriginalValue: '{2}', ClientValue: '{3}', ServerValue: '{4}'", new object[]
						{
							(propertyConflict.PropertyDefinition != null) ? propertyConflict.PropertyDefinition.Name : ExtensionDiagnostics.HandleNullObjectTrace(propertyConflict.PropertyDefinition),
							propertyConflict.ConflictResolvable,
							ExtensionDiagnostics.HandleNullObjectTrace(propertyConflict.OriginalValue),
							ExtensionDiagnostics.HandleNullObjectTrace(propertyConflict.ClientValue),
							ExtensionDiagnostics.HandleNullObjectTrace(propertyConflict.ServerValue)
						}));
					}
					text = stringBuilder.ToString();
					InstalledExtensionTable.Tracer.TraceError((long)this.GetHashCode(), text);
				}
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_MastertableSaveFailedSaveConflict, null, new object[]
				{
					"UpdateMasterTable",
					ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.sessionMailboxOwner),
					text
				});
			}
			else
			{
				InstalledExtensionTable.Tracer.Information(0L, "The app master table was saved successfully.");
				if (this.isOrgMailboxSession)
				{
					using (XmlNodeList extensionNodes = this.GetExtensionNodes())
					{
						bool isEmpty = extensionNodes == null || extensionNodes.Count == 0;
						this.orgEmptyMasterTableCache.Update(this.sessionMailboxOwner.MailboxInfo.OrganizationId, isEmpty);
					}
				}
			}
			return conflictResolutionResult;
		}

		internal void SaveXML()
		{
			ConflictResolutionResult conflictResolutionResult = this.SaveXmlIfNoConflict();
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				throw new SaveConflictException(Strings.ErrorMasterTableSaveConflict, conflictResolutionResult);
			}
		}

		private XmlNodeList GetExtensionNodes()
		{
			return this.masterTableXml.SelectNodes("/ExtensionList/Extension");
		}

		private static bool IsAppSignatureValidationDisabled()
		{
			if (InstalledExtensionTable.registryChangeListener == null)
			{
				InstalledExtensionTable.Tracer.TraceDebug(0L, "Setting Registry Change Listener for DisableAppValidation Key.");
				InstalledExtensionTable.registryChangeListener = new RegistryChangeListener(OwaConstants.OwaSetupInstallKey, new EventArrivedEventHandler(InstalledExtensionTable.DisableAppValidationRegistryKeyChangeHandler));
			}
			if (InstalledExtensionTable.disableAppValidation == null)
			{
				InstalledExtensionTable.disableAppValidation = new bool?(InstalledExtensionTable.GetAppSignatureValidationDisabledValueFromRegistry());
			}
			return InstalledExtensionTable.disableAppValidation.Value;
		}

		private static bool GetAppSignatureValidationDisabledValueFromRegistry()
		{
			bool flag = false;
			Exception ex = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(OwaConstants.OwaSetupInstallKey))
				{
					flag = RegistryReader.Instance.GetValue<bool>(registryKey, null, "DisableAppValidation", false);
					InstalledExtensionTable.Tracer.TraceDebug<bool>(0L, "App Signature Validation Disabled From Registry. Value: {0}", flag);
				}
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				InstalledExtensionTable.Tracer.TraceError<string, string>(0L, "Cannot Read Value: {0} from Registry due to Exception {1}. Using false.", "DisableAppValidation", ex.ToString());
				flag = false;
			}
			return flag;
		}

		private static void DisableAppValidationRegistryKeyChangeHandler(object sender, EventArrivedEventArgs e)
		{
			InstalledExtensionTable.Tracer.TraceDebug(0L, "Registry Change Event Occurred. Get DisableAppValidation Key.");
			InstalledExtensionTable.disableAppValidation = new bool?(InstalledExtensionTable.GetAppSignatureValidationDisabledValueFromRegistry());
		}

		private const string UpdateCheckFrequencySecondsKey = "UpdateCheckFrequencySeconds";

		private const string ScenarioNameForGetExtensions = "GetExtensions";

		private const string ScenarioNameForGetExtensionsHandleTimeout = "GetExtensionsHandleTimeout";

		private const string ScenarioNameForGetOrgExtensions = "GetOrgExtensions";

		private const string ScenarioNameForRemoveInvalidExtension = "RemoveInvalidExtension";

		private const string ScenarioUpdateMasterTable = "UpdateMasterTable";

		private const string ScenarioNameForKillApp = "KillAppFromMailbox";

		private const string ScenarioNameForUpdateFromCache = "UpdateExtensionFromCache";

		private const string ExtensionXpath = "/ExtensionList/Extension";

		public const string LastUpdateCheckTimeXpath = "/ExtensionList/LastUpdateCheckTime";

		public const int UpdateCheckFrequencyHoursDefault = 72;

		public const string ExtensionMasterTableName = "ExtensionMasterTable";

		internal const string LastUpdateCheckTimeElementName = "LastUpdateCheckTime";

		private static bool? disableAppValidation = null;

		private UserConfiguration masterTable;

		private SafeXmlDocument masterTableXml;

		private string userId;

		private string domain;

		private bool isOrgMailboxSession;

		private bool isUserScope;

		private ExtensionsCache extensionsCache;

		private OrgEmptyMasterTableCache orgEmptyMasterTableCache;

		private IExchangePrincipal sessionMailboxOwner;

		private CultureInfo sessionPreferedCulture;

		private string sessionClientInfoString;

		private Dictionary<string, string> requestData = new Dictionary<string, string>();

		private readonly bool retrieveOnly1_0;

		private static int updateCheckFrequencySeconds;

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private static volatile DefaultExtensionTable defaultExtensionTable;

		private static RegistryChangeListener registryChangeListener;

		private static object defaultExtensionTableLock = new object();

		private static LazyMember<bool> isMultiTenancyEnabled = new LazyMember<bool>(() => VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled);

		internal static class RequestDataKey
		{
			internal const string GetExtensionsTime = "GE";

			internal const string GetMasterTableTime = "GM";

			internal const string GetProvidedExtensionsTime = "GP";

			internal const string AddMasterTableTime = "AM";

			internal const string CheckUpdatesTime = "CU";

			internal const string SaveMasterTableTime = "SU";

			internal const string OrgMailboxEwsUrlHost = "OrgHost";

			internal const string OrgMailboxEwsRequestId = "EWSReqId";

			internal const string GetOrgExtensionsTime = "GO";

			internal const string GetExtensionsTotalTime = "GET";

			internal const string CreateExchangeServiceTime = "CES";

			internal const string GetClientExtensionTime = "GCE";

			internal const string OrgMailboxAdUserLookupTime = "OAD";

			internal const string WebServiceUrlLookupTime = "WSUrl";

			internal const string CreateExtensionsTime = "CET";

			internal const string GetMarketplaceUrlTime = "GMUT";
		}
	}
}
