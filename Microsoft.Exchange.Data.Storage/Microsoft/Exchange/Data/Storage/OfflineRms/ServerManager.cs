using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Dkm;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.RightsManagementServices.Core;
using Microsoft.RightsManagementServices.Provider;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ServerManager
	{
		private ServerManager()
		{
		}

		public static IConfigurationSession ADSession
		{
			get
			{
				return ServerManager.aDSession;
			}
			set
			{
				ServerManager.aDSession = value;
			}
		}

		private static void InitializeIfNeeded()
		{
			if (!ServerManager.instance.initialized)
			{
				lock (ServerManager.instance.initializeLockObject)
				{
					if (!ServerManager.instance.initialized)
					{
						ServerManager.instance.Initialize();
						ServerManager.instance.initialized = true;
					}
				}
			}
		}

		public static void PromoteToHostingInstance()
		{
			lock (ServerManager.instance.initializeLockObject)
			{
				ServerManager.instance.isHostingInstanceOverride = true;
				ServerManager.instance.initialized = false;
			}
		}

		public static void DemoteFromHostingInstance()
		{
			lock (ServerManager.instance.initializeLockObject)
			{
				ServerManager.instance.isHostingInstanceOverride = false;
				if (ServerManager.instance.isMemberOfResolver != null)
				{
					ServerManager.instance.isMemberOfResolver.Dispose();
					ServerManager.instance.isMemberOfResolver = null;
				}
				ServerManager.instance.isMemberOfAdAdapter = null;
				if (ServerManager.instance.trustedPublishingDomainConfigCache != null)
				{
					ServerManager.instance.trustedPublishingDomainConfigCache.Dispose();
					ServerManager.instance.trustedPublishingDomainConfigCache = null;
				}
				ServerManager.instance.initialized = false;
			}
		}

		public static bool TryGetDkmKey(Guid externalDirectoryOrgId, out ExchangeGroupKey exchangeGroupKey, out Exception exception)
		{
			ServerManager.InitializeIfNeeded();
			exception = null;
			if (externalDirectoryOrgId == Guid.Empty)
			{
				exchangeGroupKey = ServerManager.instance.localForestExchangeGroupKey;
				return true;
			}
			return ServerManager.instance.TryGetExchangeGroupKeyForRemoteTenant(externalDirectoryOrgId, out exchangeGroupKey, out exception);
		}

		public static string GenerateAndSealRACKey(IPerTenantRMSTrustedPublishingDomainConfiguration config)
		{
			ArgumentValidator.ThrowIfNull("config", config);
			ContextProvider contextProvider = null;
			string result;
			try
			{
				contextProvider = new ContextProvider(config);
				IRightsAccountCertificateGenerator rightsAccountCertificateGenerator = GeneratorFactory.CreateRightsAccountCertificateGenerator(contextProvider);
				result = rightsAccountCertificateGenerator.GenerateSealedKey();
			}
			catch (ConfigurationProviderException innerException)
			{
				throw new RightsManagementServerException(ServerStrings.FailedToCreateLicenseGenerator(string.Empty, "Rac"), innerException);
			}
			catch (CertificationException innerException2)
			{
				throw new RightsManagementServerException(ServerStrings.FailedToCreateLicenseGenerator(string.Empty, "Rac"), innerException2);
			}
			finally
			{
				if (contextProvider != null)
				{
					contextProvider.Dispose();
				}
			}
			return result;
		}

		public static string ResealRACKey(IPerTenantRMSTrustedPublishingDomainConfiguration config, string originalSharedKey)
		{
			ArgumentValidator.ThrowIfNull("config", config);
			ArgumentValidator.ThrowIfNullOrEmpty("config.CompressedSLCCertChain", config.CompressedSLCCertChain);
			ArgumentValidator.ThrowIfNullOrEmpty("originalSharedKey", originalSharedKey);
			ContextProvider contextProvider = null;
			string result;
			try
			{
				contextProvider = new ContextProvider(config);
				IRightsAccountCertificateGenerator rightsAccountCertificateGenerator = GeneratorFactory.CreateRightsAccountCertificateGenerator(contextProvider);
				result = rightsAccountCertificateGenerator.ResealKey(originalSharedKey, RMUtil.DecompressSLCCertificate(config.CompressedSLCCertChain));
			}
			catch (ConfigurationProviderException innerException)
			{
				throw new RightsManagementServerException(ServerStrings.FailedToResealKey, innerException);
			}
			catch (CertificationException innerException2)
			{
				throw new RightsManagementServerException(ServerStrings.FailedToResealKey, innerException2);
			}
			catch (RightsManagementException innerException3)
			{
				throw new RightsManagementServerException(ServerStrings.FailedToResealKey, innerException3, true);
			}
			finally
			{
				if (contextProvider != null)
				{
					contextProvider.Dispose();
				}
			}
			return result;
		}

		public static UseLicenseResult[] AcquireUseLicenses(RmsClientManagerContext clientContext, XmlNode[] rightsAccountCertificate, XmlNode[] issuanceLicense, LicenseeIdentity[] licenseeIdentities)
		{
			ArgumentValidator.ThrowIfNull("clientContext", clientContext);
			ArgumentValidator.ThrowIfNull("rightsAccountCertificate", rightsAccountCertificate);
			ArgumentValidator.ThrowIfNull("issuanceLicense", issuanceLicense);
			ArgumentValidator.ThrowIfNull("licenseeIdentities", licenseeIdentities);
			Stopwatch watch = Stopwatch.StartNew();
			ServerManager.InitializeIfNeeded();
			if (ServerManager.instance.isHostingInstance)
			{
				PerTenantRMSTrustedPublishingDomainConfiguration tenantTrustedPublishingDomainConfig = ServerManager.instance.GetTenantTrustedPublishingDomainConfig(clientContext);
				ContextProvider contextProvider = null;
				try
				{
					IEndUserLicenseGenerator endUserLicenseGenerator;
					try
					{
						contextProvider = new ContextProvider(clientContext, tenantTrustedPublishingDomainConfig);
						endUserLicenseGenerator = GeneratorFactory.CreateEndUserLicenseGenerator(contextProvider);
					}
					catch (ConfigurationProviderException ex)
					{
						ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireUseLicense, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed in CreateEndUserLicenseGenerator with Exception: {0}", ServerManagerLog.GetExceptionLogString(ex, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
						throw new RightsManagementServerException(ServerStrings.FailedToCreateLicenseGenerator(clientContext.OrgId.ToString(), "UseLicenseGenerator"), ex);
					}
					catch (LicensingException ex2)
					{
						ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireUseLicense, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed in CreateEndUserLicenseGenerator with WellKnownError {0} and Exception: {1}", ex2.ErrorCode, ServerManagerLog.GetExceptionLogString(ex2, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
						throw new RightsManagementServerException(ServerStrings.FailedToCreateLicenseGenerator(clientContext.OrgId.ToString(), "UseLicenseGenerator"), ex2);
					}
					XrmlCertificateChain xrmlCertificateChain = new XrmlCertificateChain(DrmClientUtils.ConvertXmlNodeArrayToStringArray(issuanceLicense));
					XrmlCertificateChain xrmlCertificateChain2 = new XrmlCertificateChain(DrmClientUtils.ConvertXmlNodeArrayToStringArray(rightsAccountCertificate));
					ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireUseLicense, ServerManagerLog.EventType.Entry, clientContext, string.Format("AcquireLicenses for {0} licensees, they are {1}", licenseeIdentities.Length, ServerManager.GetLicenseeLogString(licenseeIdentities)));
					LicensingBatchResults licensingBatchResults;
					try
					{
						licensingBatchResults = endUserLicenseGenerator.AcquireLicenses(xrmlCertificateChain2, xrmlCertificateChain, licenseeIdentities);
					}
					catch (LicensingException ex3)
					{
						ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireUseLicense, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed useLicenseGenerator.AcquireLicenses with WellKnownErrorCode {0}, Exception: {1}", ex3.ErrorCode, ServerManagerLog.GetExceptionLogString(ex3, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
						throw new RightsManagementServerException(ServerStrings.FailedToAcquireUseLicenses(clientContext.OrgId.ToString()), ex3);
					}
					int num = 0;
					UseLicenseResult[] array = new UseLicenseResult[licensingBatchResults.LicensingResults.Count];
					for (int i = 0; i < licensingBatchResults.LicensingResults.Count; i++)
					{
						StringBuilder stringBuilder = null;
						if (licensingBatchResults.LicensingResults[i].EndUserLicense != null && licensingBatchResults.LicensingResults[i].Error == null)
						{
							stringBuilder = new StringBuilder();
							stringBuilder.Append(licensingBatchResults.LicensingResults[i].EndUserLicense);
							string[] array2 = licensingBatchResults.ServerLicensorCertificateChain.ToStringArray();
							foreach (string value in array2)
							{
								stringBuilder.Append(value);
							}
						}
						LicensingException error = licensingBatchResults.LicensingResults[i].Error;
						RightsManagementServerException ex4 = (error != null) ? new RightsManagementServerException(ServerStrings.FailedToAcquireUseLicenses(clientContext.OrgId.ToString()), error) : null;
						if (error != null)
						{
							num++;
							ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireUseLicense, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed to AcquireLicenses for Licensee {0} with WellKnownErrorCode {1}, Exception: {2}", licenseeIdentities[i], error.ErrorCode, ServerManagerLog.GetExceptionLogString(error, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
						}
						if (stringBuilder != null && ex4 != null)
						{
							throw new InvalidOperationException("LicensingResult.EndUserLicense and LicensingResult.Error are both non-null");
						}
						if (stringBuilder == null && ex4 == null)
						{
							throw new InvalidOperationException("LicensingResult.EndUserLicense and LicensingResult.Error are null");
						}
						array[i] = new UseLicenseResult((stringBuilder != null) ? stringBuilder.ToString() : null, ex4);
					}
					ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireUseLicense, (num == 0) ? ServerManagerLog.EventType.Success : ServerManagerLog.EventType.Error, clientContext, string.Format("AcquireLicenses finishes with successs count {0} and failed count {1}", array.Length - num, num));
					ServerManager.instance.rpcAcquireUseLicensePerformanceMonitor.Record(watch);
					return array;
				}
				finally
				{
					if (contextProvider != null)
					{
						contextProvider.Dispose();
					}
				}
			}
			UseLicenseRpcResult[] array4 = RpcClientWrapper.AcquireUseLicenses(clientContext, rightsAccountCertificate, issuanceLicense, licenseeIdentities);
			UseLicenseResult[] array5 = new UseLicenseResult[array4.Length];
			for (int k = 0; k < array4.Length; k++)
			{
				RightsManagementServerException e = null;
				if (array4[k].ErrorResults != null && array4[k].ErrorResults.Count > 0)
				{
					e = new RightsManagementServerException(array4[k].GetSerializedString(), array4[k].IsPermanentFailure);
				}
				array5[k] = new UseLicenseResult(array4[k].EndUseLicense, e);
			}
			ServerManager.instance.acquireUseLicensePerformanceMonitor.Record(watch);
			return array5;
		}

		public static int GetTenantActiveCryptoMode(RmsClientManagerContext clientContext)
		{
			ArgumentValidator.ThrowIfNull("clientContext", clientContext);
			ServerManager.InitializeIfNeeded();
			if (ServerManager.instance.isHostingInstance)
			{
				PerTenantRMSTrustedPublishingDomainConfiguration tenantTrustedPublishingDomainConfig = ServerManager.instance.GetTenantTrustedPublishingDomainConfig(clientContext);
				return tenantTrustedPublishingDomainConfig.ActiveCryptoMode;
			}
			ActiveCryptoModeRpcResult[] tenantActiveCryptoMode = RpcClientWrapper.GetTenantActiveCryptoMode(clientContext);
			ActiveCryptoModeResult[] array = new ActiveCryptoModeResult[tenantActiveCryptoMode.Length];
			for (int i = 0; i < tenantActiveCryptoMode.Length; i++)
			{
				RightsManagementServerException e = null;
				if (tenantActiveCryptoMode[i].ErrorResults != null && tenantActiveCryptoMode[i].ErrorResults.Count > 0)
				{
					e = new RightsManagementServerException(tenantActiveCryptoMode[i].GetSerializedString(), tenantActiveCryptoMode[i].IsPermanentFailure);
				}
				array[i] = new ActiveCryptoModeResult(tenantActiveCryptoMode[i].ActiveCryptoMode, e);
			}
			return array[0].ActiveCryptoMode;
		}

		public static void AcquireTenantLicenses(RmsClientManagerContext clientContext, XmlNode[] machineCertificateChain, string identity, out XmlNode[] racXml, out XmlNode[] clcXml)
		{
			ArgumentValidator.ThrowIfNull("clientContext", clientContext);
			ArgumentValidator.ThrowIfNull("machineCertificateChain", machineCertificateChain);
			ArgumentValidator.ThrowIfNullOrEmpty("identity", identity);
			racXml = null;
			clcXml = null;
			ServerManager.InitializeIfNeeded();
			if (ServerManager.instance.isHostingInstance)
			{
				PerTenantRMSTrustedPublishingDomainConfiguration tenantTrustedPublishingDomainConfig = ServerManager.instance.GetTenantTrustedPublishingDomainConfig(clientContext);
				ContextProvider contextProvider = null;
				try
				{
					IRightsAccountCertificateGenerator rightsAccountCertificateGenerator;
					IClientLicensorCertificateGenerator clientLicensorCertificateGenerator;
					try
					{
						contextProvider = new ContextProvider(clientContext, tenantTrustedPublishingDomainConfig);
						rightsAccountCertificateGenerator = GeneratorFactory.CreateRightsAccountCertificateGenerator(contextProvider);
						clientLicensorCertificateGenerator = GeneratorFactory.CreateClientLicensorCertificateGenerator(contextProvider);
					}
					catch (ConfigurationProviderException ex)
					{
						ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireTenantLicenses, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed to create Generator with Exception: {0}", ServerManagerLog.GetExceptionLogString(ex, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
						throw new RightsManagementServerException(ServerStrings.FailedToCreateLicenseGenerator(clientContext.OrgId.ToString(), "RacClc"), ex);
					}
					catch (CertificationException ex2)
					{
						ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireTenantLicenses, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed to create Generator with WellKnownErrorCode {0}, Exception: {1}", ex2.ErrorCode, ServerManagerLog.GetExceptionLogString(ex2, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
						throw new RightsManagementServerException(ServerStrings.FailedToCreateLicenseGenerator(clientContext.OrgId.ToString(), "Rac"), ex2);
					}
					catch (PublishingException ex3)
					{
						ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireTenantLicenses, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed to create Generator with WellKnownErrorCode {0}, Exception: {1}", ex3.ErrorCode, ServerManagerLog.GetExceptionLogString(ex3, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
						throw new RightsManagementServerException(ServerStrings.FailedToCreateLicenseGenerator(clientContext.OrgId.ToString(), "Clc"), ex3);
					}
					XrmlCertificateChain xrmlCertificateChain = new XrmlCertificateChain(DrmClientUtils.ConvertXmlNodeArrayToStringArray(machineCertificateChain));
					ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireTenantLicenses, ServerManagerLog.EventType.Entry, clientContext, string.Format("AcquireTenantLicenses for identity {0}", identity));
					XrmlCertificateChain xrmlCertificateChain2;
					XrmlCertificateChain xrmlCertificateChain3;
					try
					{
						string text = ServerManager.ReadOrganizationRacKeysFromIRMConfig(clientContext.OrgId);
						xrmlCertificateChain2 = rightsAccountCertificateGenerator.AcquireCertificate(xrmlCertificateChain, identity, text);
						xrmlCertificateChain3 = clientLicensorCertificateGenerator.AcquireCertificate(xrmlCertificateChain2);
					}
					catch (CertificationException ex4)
					{
						ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireTenantLicenses, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed to AcquireTenantLicenses with WellKnownErrorCode {0}, Exception: {1}", ex4.ErrorCode, ServerManagerLog.GetExceptionLogString(ex4, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
						throw new RightsManagementServerException(ServerStrings.FailedToAcquireRacAndClc(clientContext.OrgId.ToString(), identity), ex4);
					}
					catch (PublishingException ex5)
					{
						ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.AcquireTenantLicenses, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed to AcquireTenantLicenses with WellKnownErrorCode {0}, Exception: {1}", ex5.ErrorCode, ServerManagerLog.GetExceptionLogString(ex5, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
						throw new RightsManagementServerException(ServerStrings.FailedToAcquireRacAndClc(clientContext.OrgId.ToString(), identity), ex5);
					}
					if (!RMUtil.TryConvertCertChainStringArrayToXmlNodeArray(xrmlCertificateChain2.ToStringArray(), out racXml))
					{
						throw new RightsManagementServerException(ServerStrings.FailedToAcquireRacAndClc(clientContext.OrgId.ToString(), identity), true);
					}
					if (!RMUtil.TryConvertCertChainStringArrayToXmlNodeArray(xrmlCertificateChain3.ToStringArray(), out clcXml))
					{
						throw new RightsManagementServerException(ServerStrings.FailedToAcquireRacAndClc(clientContext.OrgId.ToString(), identity), true);
					}
					return;
				}
				finally
				{
					if (contextProvider != null)
					{
						contextProvider.Dispose();
					}
				}
			}
			RpcClientWrapper.AcquireTenantLicenses(clientContext, machineCertificateChain, identity, out racXml, out clcXml);
		}

		public static bool IsMemberOf(RmsClientManagerContext clientContext, string recipientAddress, string groupAddress)
		{
			ArgumentValidator.ThrowIfNull("recipientAddress", recipientAddress);
			ArgumentValidator.ThrowIfNullOrEmpty("groupAddress", groupAddress);
			if (!ServerManager.instance.isHostingInstance)
			{
				throw new InvalidOperationException("Can't call IsMemberOf directly as ServerManager is NOT allowed to host in current calling process");
			}
			if (!SmtpAddress.IsValidSmtpAddress(recipientAddress))
			{
				ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.DirectoryServiceProvider, ServerManagerLog.EventType.Error, clientContext, string.Format("recipientAddress {0} is invalid SMTP Address", recipientAddress));
				return false;
			}
			if (!RoutingAddress.IsValidAddress(groupAddress))
			{
				ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.DirectoryServiceProvider, ServerManagerLog.EventType.Error, clientContext, string.Format("groupAddress {0} is invalid SMTP Address", groupAddress));
				return false;
			}
			ServerManager.InitializeIfNeeded();
			bool result;
			if (ServerManager.CheckForSpecialRmsOnlineTenantMembershipQuery(recipientAddress, groupAddress, out result))
			{
				return result;
			}
			ADRawEntry adrawEntry = clientContext.ResolveRecipient(recipientAddress);
			if (adrawEntry == null)
			{
				ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.DirectoryServiceProvider, ServerManagerLog.EventType.Verbose, clientContext, string.Format("Failed to resolve recipientAddress {0} in Active Directory. Treat IsMemberOf for {1} against {2} as false", recipientAddress, recipientAddress, groupAddress));
				return false;
			}
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)adrawEntry[ADRecipientSchema.EmailAddresses];
			SmtpProxyAddress other = new SmtpProxyAddress(groupAddress, true);
			foreach (ProxyAddress proxyAddress in proxyAddressCollection)
			{
				if (proxyAddress.Equals(other))
				{
					ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.DirectoryServiceProvider, ServerManagerLog.EventType.Success, clientContext, string.Format("IsMemberOf return true as proxyAddress {0} is the same is groupAddress {1}", recipientAddress, groupAddress));
					return true;
				}
			}
			RoutingAddress groupKey = new RoutingAddress(groupAddress);
			bool result2 = ServerManager.instance.isMemberOfResolver.IsMemberOf(clientContext.RecipientSession, adrawEntry.Id, groupKey);
			ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.DirectoryServiceProvider, ServerManagerLog.EventType.Success, clientContext, string.Format("IsMemberOf return {0} for proxyAddress {1} against groupAddress {2}", result2.ToString(CultureInfo.InvariantCulture), recipientAddress, groupAddress));
			return result2;
		}

		internal static bool IsUserMemberOfTenant(string userEmailAddress, ADSessionSettings sessionSettings)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 921, "IsUserMemberOfTenant", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\OfflineRms\\ServerManager.cs");
			return tenantOrRootOrgRecipientSession.IsRecipientInOrg(new SmtpProxyAddress(userEmailAddress, true));
		}

		private static bool CheckForSpecialRmsOnlineTenantMembershipQuery(string recipientAddress, string groupAddress, out bool isMemberOfTenant)
		{
			isMemberOfTenant = false;
			if (new SmtpAddress(groupAddress).Local.Equals("AllStaff-7184AB3F-CCD1-46F3-8233-3E09E9CF0E66", StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					isMemberOfTenant = ServerManager.IsUserMemberOfTenant(recipientAddress, ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(new SmtpAddress(groupAddress).Domain));
				}
				catch (NonUniqueRecipientException ex)
				{
					throw new ADOperationException(ex.LocalizedString, ex);
				}
				return true;
			}
			return false;
		}

		private static string GetLicenseeLogString(IEnumerable<LicenseeIdentity> licenseeIdentities)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (LicenseeIdentity licenseeIdentity in licenseeIdentities)
			{
				stringBuilder.Append("[");
				stringBuilder.Append(licenseeIdentity.PrimaryEmailAddress);
				stringBuilder.Append(licenseeIdentity.IsSuperUser ? "SuperUser" : "RegularUser");
				stringBuilder.Append("]");
			}
			return stringBuilder.ToString();
		}

		private static string ReadOrganizationRacKeysFromIRMConfig(OrganizationId orgId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId);
			IRMConfiguration irmConfiguration = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 1005, "ReadOrganizationRacKeysFromIRMConfig", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\OfflineRms\\ServerManager.cs");
				irmConfiguration = IRMConfiguration.Read(tenantOrTopologyConfigurationSession);
			}, 3);
			if (!adoperationResult.Succeeded)
			{
				throw new RightsManagementServerException(ServerStrings.FailedToReadIRMConfig(orgId.ToString()), adoperationResult.Exception, true);
			}
			if (irmConfiguration == null)
			{
				throw new RightsManagementServerException(ServerStrings.FailedToReadIRMConfig(orgId.ToString()), adoperationResult.Exception, true);
			}
			if (string.IsNullOrEmpty(irmConfiguration.SharedServerBoxRacIdentity))
			{
				throw new RightsManagementServerException(ServerStrings.FailedToReadSharedServerBoxRacIdentityFromIRMConfig(orgId.ToString()), adoperationResult.Exception, true);
			}
			return irmConfiguration.SharedServerBoxRacIdentity;
		}

		private bool TryGetExchangeGroupKeyForRemoteTenant(Guid externalDirectoryOrgId, out ExchangeGroupKey exchangeGroupKey, out Exception exception)
		{
			exchangeGroupKey = null;
			exception = null;
			string resourceForestFqdnByExternalDirectoryOrganizationId;
			try
			{
				resourceForestFqdnByExternalDirectoryOrganizationId = ADAccountPartitionLocator.GetResourceForestFqdnByExternalDirectoryOrganizationId(externalDirectoryOrgId);
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException ex)
			{
				exception = ex;
				return false;
			}
			if (this.remoteForestExchangeGroupKeys.TryGetValue(resourceForestFqdnByExternalDirectoryOrganizationId, out exchangeGroupKey))
			{
				return true;
			}
			string parentContainerDN = string.Format("{0},{1}", "CN=Microsoft,CN=Program Data", NativeHelpers.DistinguishedNameFromCanonicalName(resourceForestFqdnByExternalDirectoryOrganizationId));
			exchangeGroupKey = new ExchangeGroupKey(null, "Microsoft Exchange DKM")
			{
				ParentContainerDN = parentContainerDN
			};
			this.remoteForestExchangeGroupKeys.TryAdd(resourceForestFqdnByExternalDirectoryOrganizationId, exchangeGroupKey);
			return true;
		}

		private void Initialize()
		{
			string processName = Process.GetCurrentProcess().ProcessName;
			this.isHostingInstance = (processName.Equals("EdgeTransport", StringComparison.OrdinalIgnoreCase) || this.isHostingInstanceOverride);
			if (this.isHostingInstance)
			{
				this.trustedPublishingDomainConfigCache = new TenantConfigurationCache<PerTenantRMSTrustedPublishingDomainConfiguration>(ServerManager.CacheSizeInBytes, ServerManager.CacheTimeout, ServerManager.CacheTimeout, null, null);
				this.isMemberOfAdAdapter = new IsMemberOfResolverADAdapter<RoutingAddress>.RoutingAddressResolver(false);
				this.isMemberOfResolver = new IsMemberOfResolver<RoutingAddress>(new ServerManager.OfflineRMSIsMemberOfResolverConfig(), new IsMemberOfResolverPerformanceCounters("OfflineRMSServerManager"), this.isMemberOfAdAdapter);
				ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.ServerInit, ServerManagerLog.EventType.Success, null, "Successfully intialized OfflineRms Server Instance");
			}
		}

		private PerTenantRMSTrustedPublishingDomainConfiguration GetTenantTrustedPublishingDomainConfig(RmsClientManagerContext clientContext)
		{
			PerTenantRMSTrustedPublishingDomainConfiguration value;
			try
			{
				if (ServerManager.aDSession != null)
				{
					value = this.trustedPublishingDomainConfigCache.GetValue(ServerManager.aDSession);
				}
				else
				{
					value = this.trustedPublishingDomainConfigCache.GetValue(clientContext.OrgId);
				}
			}
			catch (ADTransientException ex)
			{
				ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.PerTenantRMSTrustedPublishingDomainConfiguration, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed to GetTenantTrustedPublishingDomainConfig with Exception: {0}", ServerManagerLog.GetExceptionLogString(ex, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
				throw new RightsManagementServerException(ServerStrings.FailedToReadConfiguration, ex, false);
			}
			catch (ADOperationException ex2)
			{
				ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.PerTenantRMSTrustedPublishingDomainConfiguration, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed to GetTenantTrustedPublishingDomainConfig with Exception: {0}", ServerManagerLog.GetExceptionLogString(ex2, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
				throw new RightsManagementServerException(ServerStrings.FailedToReadConfiguration, ex2, true);
			}
			catch (DataValidationException ex3)
			{
				ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.PerTenantRMSTrustedPublishingDomainConfiguration, ServerManagerLog.EventType.Error, clientContext, string.Format("Failed to GetTenantTrustedPublishingDomainConfig with Exception: {0}", ServerManagerLog.GetExceptionLogString(ex3, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
				throw new RightsManagementServerException(ServerStrings.FailedToReadConfiguration, ex3, true);
			}
			return value;
		}

		private const string RmsOnlineTenantMembershipMagicString = "AllStaff-7184AB3F-CCD1-46F3-8233-3E09E9CF0E66";

		private const string Name = "OfflineRMSServerManager";

		private static readonly TimeSpan CacheTimeout = TimeSpan.FromHours(8.0);

		private static readonly long CacheSizeInBytes = (long)ByteQuantifiedSize.FromMB(10UL).ToBytes();

		private static readonly ServerManager instance = new ServerManager();

		private readonly object initializeLockObject = new object();

		private readonly ExchangeGroupKey localForestExchangeGroupKey = new ExchangeGroupKey(null, "Microsoft Exchange DKM");

		private readonly ConcurrentDictionary<string, ExchangeGroupKey> remoteForestExchangeGroupKeys = new ConcurrentDictionary<string, ExchangeGroupKey>();

		private readonly PerformanceMonitor acquireUseLicensePerformanceMonitor = new PerformanceMonitor("AcquireUseLicense", ServerManagerLog.Subcomponent.AcquireUseLicense, 100, TimeSpan.MinValue);

		private readonly PerformanceMonitor rpcAcquireUseLicensePerformanceMonitor = new PerformanceMonitor("RPCAcquireUseLicense", ServerManagerLog.Subcomponent.RpcClientWrapper, 100, TimeSpan.MinValue);

		private static IConfigurationSession aDSession;

		private TenantConfigurationCache<PerTenantRMSTrustedPublishingDomainConfiguration> trustedPublishingDomainConfigCache;

		private IsMemberOfResolverADAdapter<RoutingAddress>.RoutingAddressResolver isMemberOfAdAdapter;

		private IsMemberOfResolver<RoutingAddress> isMemberOfResolver;

		private bool initialized;

		private bool isHostingInstance;

		private bool isHostingInstanceOverride;

		private class OfflineRMSIsMemberOfResolverConfig : IsMemberOfResolverConfig
		{
			public bool Enabled
			{
				get
				{
					return true;
				}
			}

			public long ResolvedGroupsMaxSize
			{
				get
				{
					return 33554432L;
				}
			}

			public TimeSpan ResolvedGroupsExpirationInterval
			{
				get
				{
					return TimeSpan.FromHours(3.0);
				}
			}

			public TimeSpan ResolvedGroupsCleanupInterval
			{
				get
				{
					return TimeSpan.FromHours(1.0);
				}
			}

			public TimeSpan ResolvedGroupsPurgeInterval
			{
				get
				{
					return TimeSpan.FromMinutes(5.0);
				}
			}

			public TimeSpan ResolvedGroupsRefreshInterval
			{
				get
				{
					return TimeSpan.FromMinutes(10.0);
				}
			}

			public long ExpandedGroupsMaxSize
			{
				get
				{
					return 536870912L;
				}
			}

			public TimeSpan ExpandedGroupsExpirationInterval
			{
				get
				{
					return TimeSpan.FromHours(3.0);
				}
			}

			public TimeSpan ExpandedGroupsCleanupInterval
			{
				get
				{
					return TimeSpan.FromHours(1.0);
				}
			}

			public TimeSpan ExpandedGroupsPurgeInterval
			{
				get
				{
					return TimeSpan.FromMinutes(5.0);
				}
			}

			public TimeSpan ExpandedGroupsRefreshInterval
			{
				get
				{
					return TimeSpan.FromMinutes(10.0);
				}
			}
		}
	}
}
