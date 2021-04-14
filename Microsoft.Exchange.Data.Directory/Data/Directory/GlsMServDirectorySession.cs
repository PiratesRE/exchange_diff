using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory
{
	internal class GlsMServDirectorySession : IGlobalDirectorySession
	{
		internal static GlsLookupMode GlsLookupMode
		{
			get
			{
				return GlsMServDirectorySession.glsLookupMode.Value;
			}
		}

		internal static bool ShouldScanAllForests
		{
			get
			{
				return GlsMServDirectorySession.shouldScanAllForests.Value;
			}
		}

		internal GlsMServDirectorySession(string redirectFormat)
		{
			this.glsSession = new Lazy<GlsDirectorySession>(() => new GlsDirectorySession(), true);
			this.mservSession = new Lazy<MServDirectorySession>(() => new MServDirectorySession(redirectFormat), true);
		}

		public string GetRedirectServer(string memberName)
		{
			string fqdn = null;
			this.ExecuteGlobalRead(delegate(IGlobalDirectorySession session)
			{
				fqdn = session.GetRedirectServer(memberName);
				return true;
			});
			return fqdn;
		}

		public bool TryGetRedirectServer(string memberName, out string fqdn)
		{
			string outfqdn = null;
			bool result = this.ExecuteGlobalRead((IGlobalDirectorySession session) => session.TryGetRedirectServer(memberName, out outfqdn));
			fqdn = outfqdn;
			return result;
		}

		public string GetRedirectServer(Guid orgGuid)
		{
			string fqdn = null;
			this.ExecuteGlobalRead(delegate(IGlobalDirectorySession session)
			{
				fqdn = session.GetRedirectServer(orgGuid);
				return true;
			});
			return fqdn;
		}

		public bool TryGetRedirectServer(Guid orgGuid, out string fqdn)
		{
			string outfqdn = null;
			bool result = this.ExecuteGlobalRead((IGlobalDirectorySession session) => session.TryGetRedirectServer(orgGuid, out outfqdn));
			fqdn = outfqdn;
			return result;
		}

		public bool TryGetDomainFlag(string domainFqdn, GlsDomainFlags flag, out bool value)
		{
			bool outValue = false;
			bool result = this.ExecuteGlobalRead((IGlobalDirectorySession session) => session.TryGetDomainFlag(domainFqdn, flag, out outValue));
			value = outValue;
			return result;
		}

		public void SetDomainFlag(string domainFqdn, GlsDomainFlags flag, bool value)
		{
			this.ExecuteGlobalWrite(delegate(IGlobalDirectorySession session)
			{
				session.SetDomainFlag(domainFqdn, flag, value);
			}, delegate(IGlobalDirectorySession session)
			{
				session.SetDomainFlag(domainFqdn, flag, !value);
			});
		}

		public bool TryGetTenantFlag(Guid externalDirectoryOrganizationId, GlsTenantFlags tenantFlags, out bool value)
		{
			bool outValue = false;
			bool result = this.ExecuteGlobalRead((IGlobalDirectorySession session) => session.TryGetTenantFlag(externalDirectoryOrganizationId, tenantFlags, out outValue));
			value = outValue;
			return result;
		}

		public void SetTenantFlag(Guid externalDirectoryOrganizationId, GlsTenantFlags tenantFlags, bool value)
		{
			this.ExecuteGlobalWrite(delegate(IGlobalDirectorySession session)
			{
				session.SetTenantFlag(externalDirectoryOrganizationId, tenantFlags, value);
			}, delegate(IGlobalDirectorySession session)
			{
				session.SetTenantFlag(externalDirectoryOrganizationId, tenantFlags, !value);
			});
		}

		public void AddTenant(Guid externalDirectoryOrganizationId, string resourceForestFqdn, string accountForestFqdn, string smtpNextHopDomain, GlsTenantFlags tenantFlags, string tenantContainerCN)
		{
			this.ExecuteGlobalWrite(delegate(IGlobalDirectorySession session)
			{
				session.AddTenant(externalDirectoryOrganizationId, resourceForestFqdn, accountForestFqdn, smtpNextHopDomain, tenantFlags, tenantContainerCN);
			}, delegate(IGlobalDirectorySession session)
			{
				session.RemoveTenant(externalDirectoryOrganizationId);
			});
		}

		public void AddTenant(Guid externalDirectoryOrganizationId, CustomerType tenantType, string ffoRegion, string ffoVersion)
		{
			throw new NotSupportedException("AddTenant for FFO properties only supported directly through GlsDirectorySession");
		}

		public void AddMSAUser(string msaUserNetID, string msaUserMemberName, Guid externalDirectoryOrganizationId)
		{
			this.ExecuteGlobalWriteNoUndo(delegate(IGlobalDirectorySession session)
			{
				session.AddMSAUser(msaUserNetID, msaUserMemberName, externalDirectoryOrganizationId);
			}, true);
		}

		public void UpdateTenant(Guid externalDirectoryOrganizationId, string resourceForestFqdn, string accountForestFqdn, string smtpNextHopDomain, GlsTenantFlags tenantFlags, string tenantContainerCN)
		{
			this.ExecuteGlobalWriteNoUndo(delegate(IGlobalDirectorySession session)
			{
				session.UpdateTenant(externalDirectoryOrganizationId, resourceForestFqdn, accountForestFqdn, smtpNextHopDomain, tenantFlags, tenantContainerCN);
			});
		}

		public void UpdateMSAUser(string msaUserNetID, string msaUserMemberName, Guid externalDirectoryOrganizationId)
		{
			this.ExecuteGlobalWriteNoUndo(delegate(IGlobalDirectorySession session)
			{
				session.UpdateMSAUser(msaUserNetID, msaUserMemberName, externalDirectoryOrganizationId);
			}, true);
		}

		public void RemoveTenant(Guid externalDirectoryOrganizationId)
		{
			this.ExecuteGlobalWriteNoUndo(delegate(IGlobalDirectorySession session)
			{
				session.RemoveTenant(externalDirectoryOrganizationId);
			});
		}

		public void RemoveMSAUser(string msaUserNetID)
		{
			this.ExecuteGlobalWriteNoUndo(delegate(IGlobalDirectorySession session)
			{
				session.RemoveMSAUser(msaUserNetID);
			}, true);
		}

		public bool TryGetTenantType(Guid externalDirectoryOrganizationId, out CustomerType tenantType)
		{
			throw new NotSupportedException("TryGetTenantType only supported directly through GlsDirectorySession");
		}

		public bool TryGetTenantForestsByDomain(string domainFqdn, out Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string smtpNextHopDomain, out string tenantContainerCN, out bool dataFromOfflineService)
		{
			string outResourceForestFqdn = null;
			string outAccountForestFqdn = null;
			Guid outExternalDirectoryOrganizationId = Guid.Empty;
			string outsmtpNextHopDomain = null;
			string outTenantContainerCN = null;
			bool dataFromMserv = false;
			bool outDataFromOfflineService = false;
			bool result = this.ExecuteGlobalRead(delegate(IGlobalDirectorySession session)
			{
				dataFromMserv = (session is MServDirectorySession);
				return session.TryGetTenantForestsByDomain(domainFqdn, out outExternalDirectoryOrganizationId, out outResourceForestFqdn, out outAccountForestFqdn, out outsmtpNextHopDomain, out outTenantContainerCN, out outDataFromOfflineService);
			});
			resourceForestFqdn = outResourceForestFqdn;
			accountForestFqdn = outAccountForestFqdn;
			externalDirectoryOrganizationId = outExternalDirectoryOrganizationId;
			smtpNextHopDomain = outsmtpNextHopDomain;
			tenantContainerCN = outTenantContainerCN;
			dataFromOfflineService = outDataFromOfflineService;
			return result;
		}

		public bool TryGetTenantForestsByOrgGuid(Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string tenantContainerCN, out bool dataFromOfflineService)
		{
			string outResourceForestFqdn = null;
			string outAccountForestFqdn = null;
			string outTenantContainerCN = null;
			bool outDataFromOfflineService = false;
			bool result = this.ExecuteGlobalRead((IGlobalDirectorySession session) => session.TryGetTenantForestsByOrgGuid(externalDirectoryOrganizationId, out outResourceForestFqdn, out outAccountForestFqdn, out outTenantContainerCN, out outDataFromOfflineService));
			resourceForestFqdn = outResourceForestFqdn;
			accountForestFqdn = outAccountForestFqdn;
			tenantContainerCN = outTenantContainerCN;
			dataFromOfflineService = outDataFromOfflineService;
			return result;
		}

		public bool TryGetTenantForestsByMSAUserNetID(string msaUserNetID, out Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string tenantContainerCN)
		{
			string outResourceForestFqdn = null;
			string outAccountForestFqdn = null;
			string outTenantContainerCN = null;
			Guid outExternalDirectoryOrganizationId = Guid.Empty;
			bool result = this.ExecuteGlobalRead((IGlobalDirectorySession session) => session.TryGetTenantForestsByMSAUserNetID(msaUserNetID, out outExternalDirectoryOrganizationId, out outResourceForestFqdn, out outAccountForestFqdn, out outTenantContainerCN), true);
			externalDirectoryOrganizationId = outExternalDirectoryOrganizationId;
			resourceForestFqdn = outResourceForestFqdn;
			accountForestFqdn = outAccountForestFqdn;
			tenantContainerCN = outTenantContainerCN;
			return result;
		}

		public bool TryGetMSAUserMemberName(string msaUserNetID, out string msaUserMemberName)
		{
			string outMSAUserMemberName = null;
			bool result = this.ExecuteGlobalRead((IGlobalDirectorySession session) => session.TryGetMSAUserMemberName(msaUserNetID, out outMSAUserMemberName), true);
			msaUserMemberName = outMSAUserMemberName;
			return result;
		}

		public void SetAccountForest(Guid externalDirectoryOrganizationId, string value, string tenantContainerCN = null)
		{
			this.ExecuteGlobalWriteNoUndo(delegate(IGlobalDirectorySession session)
			{
				session.SetAccountForest(externalDirectoryOrganizationId, value, tenantContainerCN);
			});
		}

		public void SetResourceForest(Guid externalDirectoryOrganizationId, string value)
		{
			this.ExecuteGlobalWriteNoUndo(delegate(IGlobalDirectorySession session)
			{
				session.SetAccountForest(externalDirectoryOrganizationId, value, null);
			});
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
			this.ExecuteGlobalWriteNoUndo(delegate(IGlobalDirectorySession session)
			{
				session.UpdateAcceptedDomain(externalDirectoryOrganizationId, domainFqdn);
			});
		}

		public void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain, bool nego2Enabled, bool oauth2ClientProfileEnabled)
		{
			this.ExecuteGlobalWrite(delegate(IGlobalDirectorySession session)
			{
				session.AddAcceptedDomain(externalDirectoryOrganizationId, domainFqdn, isInitialDomain, nego2Enabled, oauth2ClientProfileEnabled);
			}, delegate(IGlobalDirectorySession session)
			{
				session.RemoveAcceptedDomain(externalDirectoryOrganizationId, domainFqdn);
			});
		}

		public void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain, string ffoRegion, string ffoServiceVersion)
		{
			throw new NotSupportedException("AddAcceptedDomain for FFO properties only supported directly through GlsDirectorySession");
		}

		public void RemoveAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn)
		{
			bool flag = GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.BothGLSAndMServ || GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.GlsOnly || GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.MServReadsDisabled;
			if (flag && this.glsSession.Value.DomainExists(domainFqdn))
			{
				this.glsSession.Value.RemoveAcceptedDomain(externalDirectoryOrganizationId, domainFqdn);
			}
			bool flag2 = GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.BothGLSAndMServ || GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.MServReadsDisabled || GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.MServOnly;
			if (flag2)
			{
				this.mservSession.Value.RemoveAcceptedDomain(externalDirectoryOrganizationId, domainFqdn);
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
			bool msaUserExists = false;
			this.ExecuteGlobalRead(delegate(IGlobalDirectorySession session)
			{
				msaUserExists = session.MSAUserExists(msaUserNetID);
				return true;
			}, true);
			return msaUserExists;
		}

		private static GlsLookupMode InitializeLookupMode()
		{
			string value = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(GlsMServDirectorySession.RegistryKey))
				{
					value = ((registryKey != null) ? ((string)registryKey.GetValue(GlsMServDirectorySession.GlobalDirectoryLookupTypeValue, null)) : null);
				}
			}
			catch (SecurityException ex)
			{
				ExTraceGlobals.GLSTracer.TraceError<string>(0L, "SecurityException: {0}", ex.Message);
			}
			catch (UnauthorizedAccessException ex2)
			{
				ExTraceGlobals.GLSTracer.TraceError<string>(0L, "UnauthorizedAccessException: {0}", ex2.Message);
			}
			GlsLookupMode result;
			if (Enum.TryParse<GlsLookupMode>(value, true, out result))
			{
				return result;
			}
			if (DatacenterRegistry.IsForefrontForOffice())
			{
				return GlsLookupMode.GlsOnly;
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 694, "InitializeLookupMode", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\GlsMServDirectorySession.cs");
			ADSite localSite = topologyConfigurationSession.GetLocalSite();
			if (localSite.DistinguishedName.EndsWith("DC=extest,DC=microsoft,DC=com"))
			{
				return GlsLookupMode.MServOnly;
			}
			return GlsLookupMode.BothGLSAndMServ;
		}

		private static bool InitializeScanMode()
		{
			string a = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(GlsMServDirectorySession.RegistryKey))
				{
					a = ((registryKey != null) ? ((string)registryKey.GetValue(GlsMServDirectorySession.GlobalDirectoryScanTypeValue, null)) : null);
				}
			}
			catch (SecurityException ex)
			{
				ExTraceGlobals.GLSTracer.TraceError<string>(0L, "SecurityException: {0}", ex.Message);
			}
			catch (UnauthorizedAccessException ex2)
			{
				ExTraceGlobals.GLSTracer.TraceError<string>(0L, "UnauthorizedAccessException: {0}", ex2.Message);
			}
			return a != "0";
		}

		private bool ExecuteGlobalRead(GlobalLookup lookup)
		{
			return this.ExecuteGlobalRead(lookup, false);
		}

		private bool ExecuteGlobalRead(GlobalLookup lookup, bool skipMServRead)
		{
			bool flag = false;
			bool flag2 = (GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.BothGLSAndMServ || GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.MServOnly) && !skipMServRead;
			bool flag3 = GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.BothGLSAndMServ || GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.GlsOnly || GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.MServReadsDisabled;
			if (flag3)
			{
				try
				{
					flag = lookup(this.glsSession.Value);
				}
				catch (GlsTenantNotFoundException ex)
				{
					if (!flag2)
					{
						throw;
					}
				}
				if (flag || !flag2)
				{
					return flag;
				}
			}
			return flag2 && lookup(this.mservSession.Value);
		}

		private void ExecuteGlobalWriteNoUndo(GlobalWrite action)
		{
			this.ExecuteGlobalWrite(action, null, false);
		}

		private void ExecuteGlobalWriteNoUndo(GlobalWrite action, bool skipWriteToMServ)
		{
			this.ExecuteGlobalWrite(action, null, skipWriteToMServ);
		}

		private void ExecuteGlobalWrite(GlobalWrite action, GlobalWrite undoAction)
		{
			this.ExecuteGlobalWrite(action, undoAction, false);
		}

		private void ExecuteGlobalWrite(GlobalWrite action, GlobalWrite undoAction, bool skipWriteToMServ)
		{
			bool flag = (GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.BothGLSAndMServ || GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.MServReadsDisabled || GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.MServOnly) && !skipWriteToMServ;
			bool flag2 = GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.BothGLSAndMServ || GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.GlsOnly || GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.MServReadsDisabled;
			if (flag2)
			{
				action(this.glsSession.Value);
			}
			if (flag)
			{
				bool flag3 = true;
				try
				{
					action(this.mservSession.Value);
					flag3 = false;
				}
				finally
				{
					if (flag3 && undoAction != null && flag2)
					{
						undoAction(this.glsSession.Value);
					}
				}
			}
		}

		private static readonly string RegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		private static readonly string GlobalDirectoryScanTypeValue = "GlobalDirectoryScanType";

		private static readonly string GlobalDirectoryLookupTypeValue = "GlobalDirectoryLookupType";

		private static readonly Lazy<GlsLookupMode> glsLookupMode = new Lazy<GlsLookupMode>(new Func<GlsLookupMode>(GlsMServDirectorySession.InitializeLookupMode), LazyThreadSafetyMode.PublicationOnly);

		private static readonly Lazy<bool> shouldScanAllForests = new Lazy<bool>(new Func<bool>(GlsMServDirectorySession.InitializeScanMode), LazyThreadSafetyMode.PublicationOnly);

		private readonly Lazy<GlsDirectorySession> glsSession;

		private readonly Lazy<MServDirectorySession> mservSession;
	}
}
