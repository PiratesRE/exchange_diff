using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Mserve;

namespace Microsoft.Exchange.Data.Directory
{
	internal class MServDirectorySession : IGlobalDirectorySession
	{
		static MServDirectorySession()
		{
			MServDirectorySession.InitializePartnerIdMap();
		}

		internal MServDirectorySession(string redirectFormat)
		{
			this.redirectFormat = redirectFormat;
		}

		public string GetRedirectServer(string memberName)
		{
			bool flag;
			return this.GetRedirectServerFromMemberName(memberName, out flag, true);
		}

		public bool TryGetRedirectServer(string memberName, out string fqdn)
		{
			bool flag;
			fqdn = this.GetRedirectServerFromMemberName(memberName, out flag, false);
			return flag || string.Empty != fqdn;
		}

		public string GetRedirectServer(Guid orgGuid)
		{
			string address = string.Format("43BA6209CC0F4542958F65F8BF1CDED6@{0}.exchangereserved", orgGuid.ToString());
			int partnerId = MServDirectorySession.ReadMservEntry(address);
			bool flag;
			return this.GetRedirectServerFromPartnerId(partnerId, out flag, true);
		}

		public bool TryGetRedirectServer(Guid orgGuid, out string fqdn)
		{
			string address = string.Format("43BA6209CC0F4542958F65F8BF1CDED6@{0}.exchangereserved", orgGuid.ToString());
			int partnerId = MServDirectorySession.ReadMservEntry(address);
			bool flag;
			fqdn = this.GetRedirectServerFromPartnerId(partnerId, out flag, false);
			return flag || string.Empty != fqdn;
		}

		public bool TryGetDomainFlag(string domainFqdn, GlsDomainFlags flag, out bool value)
		{
			string address = MServDirectorySession.EntryIdForGlsDomainFlag(domainFqdn, flag);
			int num = MServDirectorySession.ReadMservEntry(address);
			if (num == -1)
			{
				value = false;
			}
			else
			{
				value = (num > 0);
			}
			return true;
		}

		public void SetDomainFlag(string domainFqdn, GlsDomainFlags flag, bool value)
		{
			string address = MServDirectorySession.EntryIdForGlsDomainFlag(domainFqdn, flag);
			MServDirectorySession.RemoveMserveEntry(address, value ? 0 : 1);
			MServDirectorySession.AddMserveEntry(address, value ? 1 : 0);
		}

		public bool TryGetTenantFlag(Guid externalDirectoryOrganizationId, GlsTenantFlags tenantFlags, out bool value)
		{
			value = false;
			return false;
		}

		public void SetTenantFlag(Guid externalDirectoryOrganizationId, GlsTenantFlags tenantFlags, bool value)
		{
		}

		public void AddTenant(Guid externalDirectoryOrganizationId, string resourceForestFqdn, string accountForestFqdn, string smtpNextHopDomain, GlsTenantFlags tenantFlags, string tenantContainerCN)
		{
			this.UpdateTenantMServEntry(externalDirectoryOrganizationId, false);
		}

		public void AddTenant(Guid externalDirectoryOrganizationId, CustomerType tenantType, string ffoRegion, string ffoVersion)
		{
			throw new NotSupportedException("AddTenant for FFO properties only supported directly through GlsDirectorySession");
		}

		public void AddMSAUser(string msaUserNetID, string msaUserMemberName, Guid externalDirectoryOrganizationId)
		{
			throw new NotSupportedException("AddUser only supported directly through GlsDirectorySession");
		}

		public void UpdateTenant(Guid externalDirectoryOrganizationId, string resourceForestFqdn, string accountForestFqdn, string smtpNextHopDomain, GlsTenantFlags tenantFlags, string tenantContainerCN)
		{
			this.UpdateTenantMServEntry(externalDirectoryOrganizationId, true);
		}

		public void UpdateMSAUser(string msaUserNetID, string msaUserMemberName, Guid externalDirectoryOrganizationId)
		{
			throw new NotSupportedException("UpdateUser only supported directly through GlsDirectorySession");
		}

		public void RemoveTenant(Guid externalDirectoryOrganizationId)
		{
			int partnerId = this.GetLocalSite().PartnerId;
			string[] array = new string[]
			{
				string.Format("43BA6209CC0F4542958F65F8BF1CDED6@{0}.exchangereserved", externalDirectoryOrganizationId.ToString())
			};
			foreach (string address in array)
			{
				MServDirectorySession.RemoveMserveEntry(address, partnerId);
			}
		}

		public void RemoveMSAUser(string msaUserNetID)
		{
			throw new NotSupportedException("RemoveUser only supported directly through GlsDirectorySession");
		}

		public bool TryGetTenantType(Guid externalDirectoryOrganizationId, out CustomerType tenantType)
		{
			throw new NotSupportedException("TryGetTenantType only supported directly through GlsDirectorySession");
		}

		public bool TryGetTenantForestsByDomain(string domainFqdn, out Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string smtpNextHopDomain, out string tenantContainerCN)
		{
			bool flag;
			return this.TryGetTenantForestsByDomain(domainFqdn, out externalDirectoryOrganizationId, out resourceForestFqdn, out accountForestFqdn, out smtpNextHopDomain, out tenantContainerCN, out flag);
		}

		public bool TryGetTenantForestsByDomain(string domainFqdn, out Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string smtpNextHopDomain, out string tenantContainerCN, out bool dataFromOfflineService)
		{
			string partnerIdEntryKey = string.Format("E5CB63F56E8B4b69A1F70C192276D6AD@{0}", domainFqdn);
			resourceForestFqdn = this.GetResourceForestFqdnFromMservKey(partnerIdEntryKey);
			externalDirectoryOrganizationId = Guid.Empty;
			smtpNextHopDomain = string.Empty;
			accountForestFqdn = resourceForestFqdn;
			tenantContainerCN = null;
			dataFromOfflineService = false;
			return resourceForestFqdn != null;
		}

		public bool TryGetTenantForestsByOrgGuid(Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string tenantContainerCN, out bool dataFromOfflineService)
		{
			string partnerIdEntryKey = string.Format("43BA6209CC0F4542958F65F8BF1CDED6@{0}.exchangereserved", externalDirectoryOrganizationId.ToString());
			resourceForestFqdn = this.GetResourceForestFqdnFromMservKey(partnerIdEntryKey);
			accountForestFqdn = resourceForestFqdn;
			tenantContainerCN = null;
			dataFromOfflineService = false;
			return resourceForestFqdn != null;
		}

		public bool TryGetTenantForestsByMSAUserNetID(string msaUserNetID, out Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string tenantContainerCN)
		{
			throw new NotSupportedException("TryGetTenantForestsByMSAUserNetID only supported directly through GlsDirectorySession");
		}

		public bool TryGetMSAUserMemberName(string msaUserNetID, out string msaUserMemberName)
		{
			throw new NotSupportedException("TryGetMSAUserMemberName only supported directly through GlsDirectorySession");
		}

		private string GetResourceForestFqdnFromMservKey(string partnerIdEntryKey)
		{
			string result = null;
			int partnerId = MServDirectorySession.ReadMservEntry(partnerIdEntryKey);
			if (!this.TryGetForestFqdnFromPartnerId(partnerId, out result))
			{
				return null;
			}
			return result;
		}

		public void SetAccountForest(Guid externalDirectoryOrganizationId, string value, string tenantContainerCN = null)
		{
		}

		public void SetResourceForest(Guid externalDirectoryOrganizationId, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException(value);
			}
			ExTraceGlobals.MServTracer.TraceDebug<Guid, string>(0L, "SetResourceForest({0}, {1}) is a NO OP in MSERV, not making any changes", externalDirectoryOrganizationId, value);
		}

		public void SetTenantVersion(Guid externalDirectoryOrganizationId, string newTenantVersion)
		{
			throw new NotSupportedException("SetTenantVersion only supported directly through GlsDirectorySession");
		}

		public bool TryGetTenantDomains(Guid externalDirectoryOrganizationId, out string[] acceptedDomainFqdns)
		{
			throw new NotImplementedException();
		}

		public void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain)
		{
			this.AddAcceptedDomain(externalDirectoryOrganizationId, domainFqdn, isInitialDomain, false, false);
		}

		public void UpdateAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn)
		{
			this.UpdateAcceptedDomainMservEntry(externalDirectoryOrganizationId, domainFqdn, true);
		}

		public void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain, bool nego2Enabled, bool oauth2ClientProfileEnabled)
		{
			this.UpdateAcceptedDomainMservEntry(externalDirectoryOrganizationId, domainFqdn, false);
			if (nego2Enabled)
			{
				this.SetDomainFlag(domainFqdn, GlsDomainFlags.Nego2Enabled, nego2Enabled);
			}
			if (oauth2ClientProfileEnabled)
			{
				this.SetDomainFlag(domainFqdn, GlsDomainFlags.OAuth2ClientProfileEnabled, oauth2ClientProfileEnabled);
			}
		}

		public void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain, string ffoRegion, string ffoServiceVersion)
		{
			throw new NotSupportedException("AddAcceptedDomain for FFO properties only supported directly through GlsDirectorySession");
		}

		public void RemoveAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn)
		{
			int partnerId = this.GetLocalSite().PartnerId;
			Tuple<string, int>[] array = new Tuple<string, int>[]
			{
				new Tuple<string, int>(string.Format("21668DE042684883B19BCB376E3BE474@{0}", domainFqdn), partnerId),
				new Tuple<string, int>(string.Format("ade5142cfe3d4ff19fed54a7f6087a98@{0}", domainFqdn), 0),
				new Tuple<string, int>(string.Format("0f01471e875a455a80c59def2a36ee3f@{0}", domainFqdn), 0),
				new Tuple<string, int>(string.Format("E5CB63F56E8B4b69A1F70C192276D6AD@{0}", domainFqdn), partnerId)
			};
			foreach (Tuple<string, int> tuple in array)
			{
				MServDirectorySession.RemoveMserveEntry(tuple.Item1, tuple.Item2);
			}
		}

		public void SetDomainVersion(Guid externalDirectoryOrganizationId, string domainFqdn, string newDomainVersion)
		{
			throw new NotSupportedException("SetDomainVersion only supported directly through GlsDirectorySession");
		}

		public IEnumerable<string> GetDomainNamesProvisionedByEXO(IEnumerable<SmtpDomain> domains)
		{
			throw new NotSupportedException("GetDomainNamesProvisionedByEXO only supported directly through GlsDirectorySession");
		}

		public IAsyncResult BeginGetFfoTenantAttributionPropertiesByDomain(SmtpDomain domain, object clientAsyncState, AsyncCallback clientCallback)
		{
			throw new NotSupportedException("BeginGetFfoTenantAttributionPropertiesByDomain only supported directly through GlsDirectorySession");
		}

		public bool TryEndGetFfoTenantAttributionPropertiesByDomain(IAsyncResult asyncResult, out string ffoRegion, out string ffoVersion, out Guid externalDirectoryOrganizationId, out string exoNextHop, out CustomerType tenantType, out DomainIPv6State ipv6Enabled, out string exoResourceForest, out string exoAccountForest, out string exoTenantContainer)
		{
			throw new NotSupportedException("TryEndGetFfoTenantAttributionPropertiesByDomain only supported directly through GlsDirectorySession");
		}

		public IAsyncResult BeginGetFfoTenantAttributionPropertiesByOrgId(Guid externalDirectoryOrganizationId, object clientAsyncState, AsyncCallback clientCallback)
		{
			throw new NotSupportedException("BeginGetFfoTenantAttributionPropertiesByOrgId only supported directly through GlsDirectorySession");
		}

		public bool TryEndGetFfoTenantAttributionPropertiesByOrgId(IAsyncResult asyncResult, out string ffoRegion, out string exoNextHop, out CustomerType tenantType, out string exoResourceForest, out string exoAccountForest, out string exoTenantContainer)
		{
			throw new NotSupportedException("TryEndGetFfoTenantAttributionPropertiesByOrgId only supported directly through GlsDirectorySession");
		}

		public bool TryGetFfoTenantProvisioningProperties(Guid externalDirectoryOrganizationId, out string version, out CustomerType tenantType, out string region)
		{
			throw new NotSupportedException("TryGetFfoTenantProvisioningProperties only supported directly through GlsDirectorySession");
		}

		public bool TenantExists(Guid externalDirectoryOrganizationId, Namespace namespaceToCheck)
		{
			throw new NotSupportedException("TenantExists only supported directly through GlsDirectorySession");
		}

		public bool MSAUserExists(string msaUserNetID)
		{
			throw new NotSupportedException("MSAUserExists only supported directly through GlsDirectorySession");
		}

		private static void InitializePartnerIdMap()
		{
			MServDirectorySession.partnerIdToForestMap = new Dictionary<int, string>();
			MServDirectorySession.partnerIdToForestMap.Add(51003, "APCPRD01.prod.exchangelabs.com");
			MServDirectorySession.partnerIdToForestMap.Add(51012, "APCPRD02.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51021, "APCPRD03.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51022, "APCPRD04.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51023, "APCPRD05.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51024, "APCPRD06.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51025, "APCPRD07.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51026, "APCPRD08.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51002, "EURPRD01.prod.exchangelabs.com");
			MServDirectorySession.partnerIdToForestMap.Add(51007, "EURPRD02.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51013, "EURPRD03.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51014, "EURPRD04.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51015, "EURPRD05.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51016, "EURPRD06.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51017, "EURPRD07.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51000, "PROD.exchangelabs.com");
			MServDirectorySession.partnerIdToForestMap.Add(51004, "NAMPRD02.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51008, "NAMPRD03.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51009, "NAMPRD04.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51010, "NAMPRD05.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51011, "NAMPRD06.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51018, "NAMPRD07.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51019, "NAMPRD08.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51020, "NAMPRD09.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51028, "LAMPRD80.prod.outlook.com");
			MServDirectorySession.partnerIdToForestMap.Add(51005, "NAMSDF01.sdf.exchangelabs.com");
			string[] multiStringValueFromRegistry = Globals.GetMultiStringValueFromRegistry("PartnerIdToForestMappings", 0);
			foreach (string text in multiStringValueFromRegistry)
			{
				string[] array2 = text.Split(new char[]
				{
					':'
				});
				int num = -1;
				SmtpDomain smtpDomain;
				if (array2.Length != 2 || !int.TryParse(array2[0], out num) || !SmtpDomain.TryParse(array2[1], out smtpDomain))
				{
					ExTraceGlobals.MServTracer.TraceError<string>(0L, "Could not parse PartnerId registry override {0}", text);
				}
				else
				{
					ExTraceGlobals.MServTracer.TraceDebug<int, string>(0L, "Adding registry override: {0} -> {1}", num, array2[1]);
					MServDirectorySession.partnerIdToForestMap[num] = array2[1];
				}
			}
		}

		private static List<RecipientSyncOperation> SyncToMserv(string address, RecipientSyncOperation operation)
		{
			int num = 0;
			int partnerId = operation.PartnerId;
			string text = (operation.AddedEntries.Count > 0) ? "Add" : ((operation.RemovedEntries.Count > 0) ? "Remove" : "Read");
			ExTraceGlobals.FaultInjectionTracer.TraceTest(4156960061U);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2546347325U);
			List<RecipientSyncOperation> result;
			for (;;)
			{
				bool flag = false;
				int tickCount = Environment.TickCount;
				Exception ex = null;
				List<RecipientSyncOperation> list = null;
				MserveWebService mserveWebService = null;
				try
				{
					mserveWebService = EdgeSyncMservConnector.CreateDefaultMserveWebService(null);
					mserveWebService.TrackDuplicatedAddEntries = true;
					flag = true;
					ExTraceGlobals.MServTracer.TraceDebug<string, string, int>(0L, "Executing {0} for {1} with PartnerId = {2}", text, address, operation.PartnerId);
					mserveWebService.Synchronize(operation);
					list = mserveWebService.Synchronize();
					result = list;
				}
				catch (InvalidMserveRequestException ex2)
				{
					ex = ex2;
					throw new MServTransientException(DirectoryStrings.TransientMservError(ex2.Message));
				}
				catch (MserveException ex3)
				{
					ex = (ex3.InnerException ?? ex3);
					if (!MserveWebService.IsTransientException(ex3) && (ex3.InnerException == null || (!(ex3.InnerException is WebException) && !(ex3.InnerException is IOException) && !(ex3.InnerException is HttpWebRequestException) && !(ex3.InnerException is DownloadTimeoutException))))
					{
						throw new MServPermanentException(DirectoryStrings.PermanentMservError(ex.Message));
					}
					num++;
					ExTraceGlobals.MServTracer.TraceWarning(0L, "Attempt {0}: got transient exception {1} for {2} ({3})", new object[]
					{
						num,
						ex3.InnerException,
						flag ? text : "MServeWebService creation",
						address
					});
					if (num < MServDirectorySession.retriesAllowed)
					{
						continue;
					}
					throw new MServTransientException(DirectoryStrings.TransientMservError(ex.Message));
				}
				finally
				{
					if (list != null && list.Count > 0 && text == "Read")
					{
						partnerId = list[0].PartnerId;
					}
					string failure = string.Empty;
					int num2 = Environment.TickCount - tickCount;
					if (ex != null)
					{
						failure = ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
					}
					string diagnosticHeader = string.Empty;
					string ipAddress = string.Empty;
					string transactionId = string.Empty;
					if (mserveWebService != null)
					{
						diagnosticHeader = (mserveWebService.LastResponseDiagnosticInfo ?? string.Empty);
						ipAddress = (mserveWebService.LastIpUsed ?? string.Empty);
						transactionId = (mserveWebService.LastResponseTransactionId ?? string.Empty);
					}
					MservProtocolLog.BeginAppend(text, (ex == null) ? "Success" : "Failure", (long)num2, failure, address, partnerId.ToString(), ipAddress, diagnosticHeader, transactionId);
				}
				break;
			}
			return result;
		}

		internal static int ReadMservEntry(string address)
		{
			List<RecipientSyncOperation> list = MServDirectorySession.SyncToMserv(address, new RecipientSyncOperation
			{
				ReadEntries = 
				{
					address
				}
			});
			if (list.Count <= 0)
			{
				return -1;
			}
			return list[0].PartnerId;
		}

		internal static bool AddMserveEntry(string address, int value)
		{
			RecipientSyncOperation recipientSyncOperation = new RecipientSyncOperation();
			recipientSyncOperation.AddedEntries.Add(address);
			recipientSyncOperation.PartnerId = value;
			MServDirectorySession.SyncToMserv(address, recipientSyncOperation);
			return recipientSyncOperation.Synchronized;
		}

		internal static bool RemoveMserveEntry(string address, int value)
		{
			try
			{
				if (MServDirectorySession.RemoveMserveEntryWithExactValue(address, value))
				{
					return true;
				}
			}
			catch (MServPermanentException)
			{
				ExTraceGlobals.MServTracer.TraceWarning<string, int>(0L, "Could not remove {0} with PartnerId {1}, will read the actual value now", address, value);
				value = MServDirectorySession.ReadMservEntry(address);
				if (value == -1)
				{
					ExTraceGlobals.MServTracer.TraceWarning<string>(0L, "Entry {0} does not exist, no need to remove", address);
					return true;
				}
			}
			if (!MServDirectorySession.RemoveMserveEntryWithExactValue(address, value))
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_CannotDeleteMServEntry, address, new object[]
				{
					address
				});
				return false;
			}
			return true;
		}

		private static bool RemoveMserveEntryWithExactValue(string address, int value)
		{
			RecipientSyncOperation recipientSyncOperation = new RecipientSyncOperation();
			recipientSyncOperation.RemovedEntries.Add(address);
			recipientSyncOperation.PartnerId = value;
			MServDirectorySession.SyncToMserv(address, recipientSyncOperation);
			return recipientSyncOperation.Synchronized;
		}

		private static string EntryIdForGlsDomainFlag(string domainFqdn, GlsDomainFlags flag)
		{
			switch (flag)
			{
			case GlsDomainFlags.Nego2Enabled:
				return string.Format("ade5142cfe3d4ff19fed54a7f6087a98@{0}", domainFqdn);
			case GlsDomainFlags.OAuth2ClientProfileEnabled:
				return string.Format("0f01471e875a455a80c59def2a36ee3f@{0}", domainFqdn);
			default:
				throw new ArgumentOutOfRangeException("flag");
			}
		}

		private static void CleanupLegacyEntries(Guid externalDirectoryOrganizationId, string domainFqdn, int partnerId)
		{
			Tuple<string, int>[] array = new Tuple<string, int>[]
			{
				new Tuple<string, int>(string.Format("7f66cd009b304aeda37ffdeea1733ff6@{0}", domainFqdn), partnerId),
				new Tuple<string, int>(string.Format("3da19c7b44a74bd3896daaf008594b6c@{0}.exchangereserved", externalDirectoryOrganizationId.ToString()), partnerId)
			};
			foreach (Tuple<string, int> tuple in array)
			{
				MServDirectorySession.RemoveMserveEntry(tuple.Item1, tuple.Item2);
			}
		}

		private string GetRedirectServerFromMemberName(string memberName, out bool alreadyInTheRightForest, bool throwExceptionsOnTenantNotFound)
		{
			string address = string.Format("E5CB63F56E8B4b69A1F70C192276D6AD@{0}", GlsDirectorySession.ParseMemberName(memberName).Domain);
			int partnerId = MServDirectorySession.ReadMservEntry(address);
			return this.GetRedirectServerFromPartnerId(partnerId, out alreadyInTheRightForest, throwExceptionsOnTenantNotFound);
		}

		private string GetRedirectServerFromPartnerId(int partnerId, out bool alreadyInTheRightForest, bool throwExceptionsOnTenantNotFound)
		{
			alreadyInTheRightForest = false;
			if (partnerId == -1 || partnerId < 50000 || partnerId > 59999)
			{
				if (!throwExceptionsOnTenantNotFound)
				{
					return string.Empty;
				}
				if (partnerId == -1)
				{
					throw new MServTenantNotFoundException(DirectoryStrings.TenantNotFoundInMservError(partnerId.ToString()));
				}
				throw new InvalidOperationException(string.Format("The partner id {0} is out of range", partnerId));
			}
			else
			{
				if (partnerId == this.GetLocalSite().PartnerId)
				{
					ExTraceGlobals.MServTracer.TraceDebug((long)this.GetHashCode(), string.Format("The partner id {0} is the same as the current site id", partnerId));
					alreadyInTheRightForest = true;
				}
				if (string.IsNullOrEmpty(this.redirectFormat))
				{
					throw new ArgumentNullException("redirectFormat");
				}
				return string.Format(CultureInfo.InvariantCulture, this.redirectFormat, new object[]
				{
					partnerId
				});
			}
		}

		internal bool TryGetForestFqdnFromPartnerId(int partnerId, out string forestFqdn)
		{
			if (MServDirectorySession.partnerIdToForestMap.TryGetValue(partnerId, out forestFqdn))
			{
				ExTraceGlobals.MServTracer.TraceDebug((long)this.GetHashCode(), string.Format("The partner id {0} mapped to forest {1}", partnerId, forestFqdn));
				return true;
			}
			ADSite adsite = this.GetLocalSite();
			if (adsite != null && partnerId == adsite.PartnerId)
			{
				ExTraceGlobals.MServTracer.TraceDebug((long)this.GetHashCode(), string.Format("The partner id {0} is the same as the current site id", partnerId));
				forestFqdn = PartitionId.LocalForest.ForestFQDN;
				return true;
			}
			return false;
		}

		internal bool TryGetPartnerIdFromForestFqdn(string forestFqdn, out int partnerId)
		{
			foreach (KeyValuePair<int, string> keyValuePair in MServDirectorySession.partnerIdToForestMap)
			{
				if (keyValuePair.Value.Equals(forestFqdn, StringComparison.OrdinalIgnoreCase))
				{
					partnerId = keyValuePair.Key;
					ExTraceGlobals.MServTracer.TraceDebug((long)this.GetHashCode(), string.Format("The partner id {0} mapped to forest {1}", partnerId, forestFqdn));
					return true;
				}
			}
			if (PartitionId.LocalForest.ForestFQDN.Equals(forestFqdn, StringComparison.OrdinalIgnoreCase))
			{
				ADSite adsite = this.GetLocalSite();
				if (adsite != null)
				{
					partnerId = adsite.PartnerId;
					ExTraceGlobals.MServTracer.TraceDebug((long)this.GetHashCode(), string.Format("The partner id {0} is the same as the current site id", partnerId));
					return true;
				}
			}
			partnerId = -1;
			return false;
		}

		private void UpdateTenantMServEntry(Guid externalDirectoryOrganizationId, bool allowOverwrite)
		{
			string mservEntryKey = string.Format("43BA6209CC0F4542958F65F8BF1CDED6@{0}.exchangereserved", externalDirectoryOrganizationId.ToString());
			this.UpdateMservEntry(externalDirectoryOrganizationId, allowOverwrite, mservEntryKey);
		}

		private void UpdateAcceptedDomainMservEntry(Guid externalDirectoryOrganizationId, string domainFqdn, bool allowOverwrite)
		{
			string mservEntryKey = string.Format("E5CB63F56E8B4b69A1F70C192276D6AD@{0}", domainFqdn);
			int partnerId = this.UpdateMservEntry(externalDirectoryOrganizationId, allowOverwrite, mservEntryKey);
			string mservEntryKey2 = string.Format("21668DE042684883B19BCB376E3BE474@{0}", domainFqdn);
			this.UpdateMservEntry(externalDirectoryOrganizationId, allowOverwrite, mservEntryKey2);
			MServDirectorySession.CleanupLegacyEntries(externalDirectoryOrganizationId, domainFqdn, partnerId);
		}

		private int UpdateMservEntry(Guid externalDirectoryOrganizationId, bool allowOverwrite, string mservEntryKey)
		{
			int partnerId = this.GetLocalSite().PartnerId;
			int num = MServDirectorySession.ReadMservEntry(mservEntryKey);
			if (num == -1)
			{
				MServDirectorySession.AddMserveEntry(mservEntryKey, partnerId);
			}
			else if (num != partnerId)
			{
				if (!allowOverwrite)
				{
					throw new MServPermanentException(DirectoryStrings.TenantAlreadyExistsInMserv(externalDirectoryOrganizationId, num, partnerId));
				}
				MServDirectorySession.RemoveMserveEntry(mservEntryKey, num);
				MServDirectorySession.AddMserveEntry(mservEntryKey, partnerId);
			}
			return num;
		}

		private ADSite GetLocalSite()
		{
			if (MServDirectorySession.localSite == null)
			{
				lock (MServDirectorySession.adSiteLockObject)
				{
					if (MServDirectorySession.localSite == null)
					{
						this.ReadLocalSiteAndResetSiteRefreshTime();
					}
					goto IL_77;
				}
			}
			if (MServDirectorySession.lastLocalSiteRefresh.AddHours((double)MServDirectorySession.localSiteRefreshHours) < ExDateTime.Now)
			{
				try
				{
					if (Monitor.TryEnter(MServDirectorySession.adSiteLockObject))
					{
						this.ReadLocalSiteAndResetSiteRefreshTime();
					}
				}
				finally
				{
					if (Monitor.IsEntered(MServDirectorySession.adSiteLockObject))
					{
						Monitor.Exit(MServDirectorySession.adSiteLockObject);
					}
				}
			}
			IL_77:
			return MServDirectorySession.localSite;
		}

		private void ReadLocalSiteAndResetSiteRefreshTime()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1056, "ReadLocalSiteAndResetSiteRefreshTime", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\MServDirectorySession.cs");
			MServDirectorySession.localSite = topologyConfigurationSession.GetLocalSite();
			MServDirectorySession.lastLocalSiteRefresh = ExDateTime.Now;
		}

		private const string DomainEntryAddressFormatStartNewOrganization = "21668DE042684883B19BCB376E3BE474@{0}";

		internal const string DomainEntryAddressFormatMinorPartnerId = "7f66cd009b304aeda37ffdeea1733ff6@{0}";

		internal const string DomainEntryAddressFormatMinorPartnerIdForOrgGuid = "3da19c7b44a74bd3896daaf008594b6c@{0}.exchangereserved";

		internal const string PartnerIdRegistryOverride = "PartnerIdToForestMappings";

		private const uint MServTransientExceptionLid = 4156960061U;

		private const uint MServPermanentExceptionLid = 2546347325U;

		private readonly string redirectFormat;

		private static int retriesAllowed = 3;

		private static int localSiteRefreshHours = 1;

		private static ADSite localSite;

		private static object adSiteLockObject = new object();

		private static ExDateTime lastLocalSiteRefresh = ExDateTime.MinValue;

		private static Dictionary<int, string> partnerIdToForestMap;
	}
}
