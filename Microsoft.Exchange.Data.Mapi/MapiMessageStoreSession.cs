using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Mapi;
using Microsoft.Mapi.Security;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	internal class MapiMessageStoreSession : MapiAdministrationSession
	{
		public MapiMessageStoreSession(string serverExchangeLegacyDn, string userConnectAsExchangeLegacyDn, Fqdn serverFqdn) : base(serverExchangeLegacyDn, serverFqdn)
		{
			this.Reconfigure(userConnectAsExchangeLegacyDn, OpenStoreFlag.UseAdminPrivilege | OpenStoreFlag.IgnoreHomeMdb, null, null);
		}

		public MapiMessageStoreSession(string serverExchangeLegacyDn, string userConnectAsExchangeLegacyDn, Fqdn serverFqdn, Guid databaseGuid) : this(serverExchangeLegacyDn, userConnectAsExchangeLegacyDn, serverFqdn)
		{
		}

		public MapiMessageStoreSession(string serverExchangeLegacyDn, string userConnectAsExchangeLegacyDn, Fqdn serverFqdn, ConsistencyMode consistencyMode) : this(serverExchangeLegacyDn, userConnectAsExchangeLegacyDn, serverFqdn)
		{
			base.ConsistencyMode = consistencyMode;
		}

		public MapiMessageStoreSession(string serverExchangeLegacyDn, string userConnectAsExchangeLegacyDn, Fqdn serverFqdn, OpenStoreFlag openStoreFlags, CultureInfo cultureInformation, ClientIdentityInfo clientIdentityInformation, ConsistencyMode consistencyMode) : this(serverExchangeLegacyDn, userConnectAsExchangeLegacyDn, serverFqdn, consistencyMode)
		{
			this.Reconfigure(userConnectAsExchangeLegacyDn, openStoreFlags, cultureInformation, clientIdentityInformation);
		}

		public MapiMessageStoreSession(string serverExchangeLegacyDn, string userConnectAsExchangeLegacyDn, Fqdn serverFqdn, OpenStoreFlag openStoreFlags, CultureInfo cultureInformation, WindowsIdentity windowsIdentity, ConsistencyMode consistencyMode) : this(serverExchangeLegacyDn, userConnectAsExchangeLegacyDn, serverFqdn, consistencyMode)
		{
			this.Reconfigure(userConnectAsExchangeLegacyDn, openStoreFlags, cultureInformation, windowsIdentity);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.clientSecurityContext != null)
			{
				this.clientSecurityContext.Dispose();
				this.clientSecurityContext = null;
			}
			base.Dispose(disposing);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MapiMessageStoreSession>(this);
		}

		public ClientIdentityInfo ClientIdentityInformation
		{
			get
			{
				return this.clientIdentityInformation;
			}
		}

		public WindowsIdentity WindowsIdentity
		{
			get
			{
				return this.windowsIdentity;
			}
		}

		public string UserConnectAsExchangeLegacyDn
		{
			get
			{
				return this.userConnectAsExchangeLegacyDn;
			}
		}

		public OpenStoreFlag OpenStoreFlags
		{
			get
			{
				return this.openStoreFlags;
			}
		}

		public CultureInfo CultureInformation
		{
			get
			{
				return this.cultureInformation;
			}
		}

		public void SetSecurityAccessToken(ISecurityAccessToken securityAccessToken)
		{
			if (this.clientSecurityContext != null)
			{
				this.clientSecurityContext.Dispose();
				this.clientSecurityContext = null;
			}
			this.clientSecurityContext = new ClientSecurityContext(securityAccessToken);
		}

		public void Reconfigure(string userConnectAsExchangeLegacyDn, OpenStoreFlag openStoreFlags, CultureInfo cultureInformation, ClientIdentityInfo clientIdentityInformation)
		{
			if (userConnectAsExchangeLegacyDn == null)
			{
				throw new ArgumentNullException("userConnectAsExchangeLegacyDn");
			}
			this.clientIdentityInformation = clientIdentityInformation;
			this.userConnectAsExchangeLegacyDn = userConnectAsExchangeLegacyDn;
			this.openStoreFlags = openStoreFlags;
			this.cultureInformation = cultureInformation;
		}

		public void Reconfigure(string userConnectAsExchangeLegacyDn, OpenStoreFlag openStoreFlags, CultureInfo cultureInformation, WindowsIdentity windowsIdentity)
		{
			if (userConnectAsExchangeLegacyDn == null)
			{
				throw new ArgumentNullException("userConnectAsExchangeLegacyDn");
			}
			this.windowsIdentity = windowsIdentity;
			this.userConnectAsExchangeLegacyDn = userConnectAsExchangeLegacyDn;
			this.openStoreFlags = openStoreFlags;
			this.cultureInformation = cultureInformation;
		}

		internal static MapiEntryId GetAddressBookEntryIdFromLegacyDN(string userLegacyDN)
		{
			byte[] binaryEntryId = null;
			try
			{
				binaryEntryId = MapiStore.GetAddressBookEntryIdFromLegacyDN(userLegacyDN);
			}
			catch (MapiRetryableException exception)
			{
				MapiSession.ThrowWrappedException(exception, Strings.ErrorGetAddressBookEntryIdFromLegacyDN(userLegacyDN), null, null);
			}
			catch (MapiPermanentException exception2)
			{
				MapiSession.ThrowWrappedException(exception2, Strings.ErrorGetAddressBookEntryIdFromLegacyDN(userLegacyDN), null, null);
			}
			catch (MapiInvalidOperationException exception3)
			{
				MapiSession.ThrowWrappedException(exception3, Strings.ErrorGetAddressBookEntryIdFromLegacyDN(userLegacyDN), null, null);
			}
			return new MapiEntryId(binaryEntryId);
		}

		internal static string GetLegacyDNFromAddressBookEntryId(MapiEntryId abbEntryId)
		{
			string result = null;
			try
			{
				result = MapiStore.GetLegacyDNFromAddressBookEntryId((byte[])abbEntryId);
			}
			catch (MapiRetryableException exception)
			{
				MapiSession.ThrowWrappedException(exception, Strings.ErrorGetGetLegacyDNFromAddressBookEntryId(abbEntryId.ToString()), null, null);
			}
			catch (MapiPermanentException exception2)
			{
				MapiSession.ThrowWrappedException(exception2, Strings.ErrorGetGetLegacyDNFromAddressBookEntryId(abbEntryId.ToString()), null, null);
			}
			catch (MapiInvalidOperationException exception3)
			{
				MapiSession.ThrowWrappedException(exception3, Strings.ErrorGetGetLegacyDNFromAddressBookEntryId(abbEntryId.ToString()), null, null);
			}
			return result;
		}

		private ClientIdentityInfo clientIdentityInformation;

		private ClientSecurityContext clientSecurityContext;

		private string userConnectAsExchangeLegacyDn;

		private OpenStoreFlag openStoreFlags;

		private CultureInfo cultureInformation;

		private WindowsIdentity windowsIdentity;
	}
}
