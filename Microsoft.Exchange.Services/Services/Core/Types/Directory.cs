using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class Directory
	{
		internal static IRecipientSession CreateADRecipientSessionForOrganization(ADObjectId searchRoot, OrganizationId organizationId)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, string>(0L, "Directory.CreateADRecipientSessionForOrganization. searchRoot: {0} organizationId: {1}", (searchRoot == null) ? "<Null>" : searchRoot.ToString(), (organizationId == null) ? "<Null>" : organizationId.ToString());
			return Directory.CreateADRecipientSessionForOrganization(searchRoot, LocaleMap.GetLcidFromCulture(CultureInfo.CurrentCulture), organizationId);
		}

		internal static IRecipientSession CreateADRecipientSessionForOrganization(ADObjectId searchRoot, int lcid, OrganizationId organizationId)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, int, string>(0L, "Directory.CreateADRecipientSessionForOrganization. searchRoot: {0} lcid: {1} organizationId: {2}", (searchRoot == null) ? "<Null>" : searchRoot.ToString(), lcid, (organizationId == null) ? "<Null>" : organizationId.ToString());
			return Directory.CreateADRecipientSession(searchRoot, lcid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId));
		}

		internal static IRecipientSession CreateGALScopedADRecipientSessionForOrganization(ADObjectId searchRoot, int lcid, OrganizationId organizationId, ClientSecurityContext clientSecurityContext)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "Directory.CreateGALScopedADRecipientSessionForOrganization. searchRoot: {0} organizationId: {1} lcid: {2} clientSecurityContext.userSid: {3}", new object[]
			{
				(searchRoot == null) ? "<Null>" : searchRoot.ToString(),
				(organizationId == null) ? "<Null>" : organizationId.ToString(),
				lcid,
				(clientSecurityContext == null || clientSecurityContext.UserSid == null) ? "<Null>" : clientSecurityContext.UserSid.ToString()
			});
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			if (clientSecurityContext.UserSid == null)
			{
				throw new ArgumentException("clientSecurityContext.UserSid can't be null");
			}
			if (searchRoot != null)
			{
				return Directory.CreateADRecipientSessionForOrganization(searchRoot, lcid, organizationId);
			}
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, adsessionSettings, 114, "CreateGALScopedADRecipientSessionForOrganization", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\Types\\Directory.cs");
			IRecipientSession recipientSession = Directory.CreateADRecipientSession(null, lcid, adsessionSettings);
			AddressBookBase globalAddressList = AddressBookBase.GetGlobalAddressList(clientSecurityContext, tenantOrTopologyConfigurationSession, recipientSession);
			return Directory.CreateADRecipientSession(searchRoot, lcid, ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(organizationId, (globalAddressList == null) ? null : globalAddressList.Id));
		}

		internal static IRecipientSession CreateAddressListScopedADRecipientSessionForOrganization(AddressBookBase addressList, ADObjectId searchRoot, int lcid, OrganizationId organizationId, ClientSecurityContext clientSecurityContext)
		{
			ADObjectId addressListId = (addressList != null) ? addressList.Id : null;
			return Directory.CreateAddressListScopedADRecipientSessionForOrganization(addressListId, searchRoot, lcid, organizationId, clientSecurityContext);
		}

		internal static IRecipientSession CreateAddressListScopedADRecipientSessionForOrganization(ADObjectId addressListId, ADObjectId searchRoot, int lcid, OrganizationId organizationId, ClientSecurityContext clientSecurityContext)
		{
			IRecipientSession result;
			if (addressListId == null)
			{
				result = Directory.CreateGALScopedADRecipientSessionForOrganization(searchRoot, lcid, organizationId, clientSecurityContext);
			}
			else
			{
				result = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, searchRoot, lcid, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(organizationId, addressListId), 187, "CreateAddressListScopedADRecipientSessionForOrganization", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\Types\\Directory.cs");
			}
			return result;
		}

		internal static bool TryFindRecipient(SecurityIdentifier sid, IRecipientSession adRecipientSession, out ADRecipient adRecipient)
		{
			adRecipient = adRecipientSession.FindBySid(sid);
			if (adRecipient == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<SecurityIdentifier>(0L, "ADRecipient for sid {0} was not found", sid);
			}
			return adRecipient != null;
		}

		internal static bool TryFindRecipient(string smtpAddress, IRecipientSession adRecipientSession, out ADRecipient adRecipient)
		{
			SmtpProxyAddress smtpProxyAddress = Directory.GetSmtpProxyAddress(smtpAddress);
			try
			{
				adRecipient = adRecipientSession.FindByProxyAddress(smtpProxyAddress);
			}
			catch (NonUniqueRecipientException innerException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "Found an account with a duplicate primary smtp address: {0}", smtpAddress);
				throw new ADConfigurationException(innerException);
			}
			if (adRecipient == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "ADRecipient for smtpAddress {0} was not found", smtpAddress);
			}
			return adRecipient != null;
		}

		private static IRecipientSession CreateADRecipientSession(ADObjectId searchRoot, int lcid, ADSessionSettings adSessionSettings)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, int>(0L, "Directory.CreateADRecipientSession. searchRoot: {0} lcid: {1} <adSessionSettings>", (searchRoot == null) ? "<Null>" : searchRoot.ToString(), lcid);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, searchRoot, lcid, true, ConsistencyMode.IgnoreInvalid, null, adSessionSettings, 287, "CreateADRecipientSession", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\Types\\Directory.cs");
		}

		private static SmtpProxyAddress GetSmtpProxyAddress(string primarySmtpAddress)
		{
			SmtpProxyAddress result;
			try
			{
				result = new SmtpProxyAddress(primarySmtpAddress, true);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, string>(0L, "[Directory::GetSmptProxyAddress] Invalid smtp address format. Smtp: {0}, Exception message: {1}", primarySmtpAddress, ex.Message);
				throw new InvalidSmtpAddressException(ex);
			}
			return result;
		}

		public static ADSessionSettings SessionSettingsFromAddress(string address)
		{
			if (EWSSettings.IsMultiTenancyEnabled && !string.IsNullOrEmpty(address) && SmtpAddress.IsValidSmtpAddress(address))
			{
				try
				{
					return ADSessionSettings.FromTenantAcceptedDomain(new SmtpAddress(address).Domain);
				}
				catch (CannotResolveTenantNameException)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "[Directory::SessionSettingsFromAddress] Cannot locate organization for address {0}.", address);
				}
			}
			return ADSessionSettings.FromRootOrgScopeSet();
		}

		public static bool TryRunADOperationWithErrorHandling(Action action, out Exception adException)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			adException = null;
			try
			{
				action();
			}
			catch (DataValidationException ex)
			{
				adException = ex;
			}
			catch (ADTransientException ex2)
			{
				adException = ex2;
			}
			catch (DataSourceOperationException ex3)
			{
				adException = ex3;
			}
			if (adException != null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<Exception>(0L, "[Directory::TryExecuteDirectoryOperation] caught exception: {0}", adException);
				return false;
			}
			return true;
		}

		internal static IRecipientSession CreateRootADRecipientSession()
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "Directory.CreateADRecipientSession.  Creating IRecipientSession.");
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 28, "CreateRootADRecipientSession", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Server\\Core\\Types\\Directory.cs");
		}
	}
}
