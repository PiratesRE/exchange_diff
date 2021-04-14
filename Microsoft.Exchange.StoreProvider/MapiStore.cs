using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.XropService;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Mapi.Security;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiStore : MapiProp
	{
		private static EntryIdType GetEntryIdType(byte[] entryId)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(27756, 24, 0L, "MapiStore.GetEntryIdType params: entryId={0}", TraceUtils.DumpEntryId(entryId));
			}
			if (entryId == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryId");
			}
			if (entryId.Length < 1)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("entryId", "Cannot be zero length");
			}
			IExRpcManageInterface exRpcManageInterface = null;
			int num = 0;
			try
			{
				exRpcManageInterface = ExRpcModule.GetExRpcManage();
				int entryIdType = exRpcManageInterface.GetEntryIdType(entryId.Length, entryId, out num);
				if (entryIdType != 0)
				{
					MapiExceptionHelper.ThrowIfError(string.Format("Unable to get entryId type '{0}'.", TraceUtils.DumpEntryId(entryId)), entryIdType, exRpcManageInterface, null);
				}
			}
			finally
			{
				exRpcManageInterface.DisposeIfValid();
			}
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<int>(19564, 24, 0L, "MapiStore.GetEntryIdType results: {0}", num);
			}
			return (EntryIdType)num;
		}

		private static MapiStore OpenMapiStore(string serverDn, string userDn, string mailboxDn, Guid guidMailbox, Guid guidMdb, string userName, string domainName, string password, string httpProxyServerName, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, bool wantRedirect, out string correctServerDN, ClientIdentityInfo clientIdentity, bool unifiedLogon, string applicationId, Client xropClient, bool wantWebServices, byte[] clientSessionInfo, TimeSpan connectionTimeout, TimeSpan callTimeout, byte[] tenantHint)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace(11362, 24, 0L, "MapiStore.OpenMapiStore params: serverDn={0}, userDn={1}, mailboxDn={2}, guidMailbox={3}, guidMdb={4}, userName={5}, domainName={6}, password=*****, httpProxyServerName={7} connectFlags={8}, openStoreFlags={9}, cultureInfo={10}, wantRedirect={11}, applicationId={12}wantWebServices={13}, connectionTimeout={14}, callTimeout={15}", new object[]
				{
					TraceUtils.MakeString(serverDn),
					TraceUtils.MakeString(userDn),
					TraceUtils.MakeString(mailboxDn),
					guidMailbox.ToString(),
					guidMdb.ToString(),
					TraceUtils.MakeString(userName),
					TraceUtils.MakeString(domainName),
					TraceUtils.MakeString(httpProxyServerName),
					connectFlags.ToString(),
					storeFlags.ToString(),
					(cultureInfo == null) ? "MailboxDefault" : cultureInfo.DisplayName,
					wantRedirect.ToString(),
					applicationId ?? "None",
					wantWebServices,
					connectionTimeout,
					callTimeout
				});
			}
			ExRpcConnection exRpcConnection = null;
			MapiStore mapiStore = null;
			HashSet<string> hashSet = new HashSet<string>();
			string text = serverDn;
			bool flag = (storeFlags & OpenStoreFlag.Public) == OpenStoreFlag.None;
			ExRpcConnectionCreateFlag exRpcConnectionCreateFlag = ExRpcConnectionCreateFlag.UseLcidString | ExRpcConnectionCreateFlag.UseLcidSort | ExRpcConnectionCreateFlag.UseCpid | ExRpcConnectionCreateFlag.UseRpcBufferSize | ExRpcConnectionCreateFlag.CompressUp | ExRpcConnectionCreateFlag.CompressDown | ExRpcConnectionCreateFlag.PackedDown | ExRpcConnectionCreateFlag.XorMagicUp | ExRpcConnectionCreateFlag.XorMagicDown;
			if (applicationId != null && applicationId.Length == 0)
			{
				throw MapiExceptionHelper.ArgumentException("applicationId", "Cannot be an empty string");
			}
			if (flag && wantRedirect)
			{
				throw MapiExceptionHelper.ArgumentException("wantRedirect", "Cannot be used with private store");
			}
			if (wantWebServices)
			{
				if (xropClient == null)
				{
					throw MapiExceptionHelper.ArgumentException("xropClient", "Must supply for web services request");
				}
				if (userName == null)
				{
					throw MapiExceptionHelper.ArgumentException("userName", "Must be passed for web services request");
				}
				if (domainName != null)
				{
					throw MapiExceptionHelper.ArgumentException("domainName", "Not valid for web services request");
				}
				if (password != null)
				{
					throw MapiExceptionHelper.ArgumentException("password", "Not valid for web services request");
				}
				if (guidMailbox != Guid.Empty)
				{
					throw MapiExceptionHelper.ArgumentException("guidMailbox", "Not valid for web services request");
				}
				if (guidMdb != Guid.Empty)
				{
					throw MapiExceptionHelper.ArgumentException("guidMdb", "Not valid for web services request");
				}
				if (httpProxyServerName != null)
				{
					throw MapiExceptionHelper.ArgumentException("httpProxyServerName", "Not valid for web services request");
				}
				if (clientIdentity != null)
				{
					throw MapiExceptionHelper.ArgumentException("clientIdentity", "Not valid for web services request");
				}
				if (!flag)
				{
					throw MapiExceptionHelper.ArgumentException("storeFlags", "Flag OpenStoreFlag.Public not valid for web services request");
				}
				if ((connectFlags & (ConnectFlag.UseDelegatedAuthPrivilege | ConnectFlag.UseLegacyUdpNotifications | ConnectFlag.UseTransportPrivilege | ConnectFlag.UseReadOnlyPrivilege | ConnectFlag.UseReadWritePrivilege | ConnectFlag.LocalRpcOnly | ConnectFlag.UseHTTPS | ConnectFlag.UseNTLM | ConnectFlag.UseRpcUniqueBinding | ConnectFlag.UseRpcContextPool)) != ConnectFlag.None)
				{
					throw MapiExceptionHelper.ArgumentException("connectFlags", "Not all ConnectFlag values are supported for web services request");
				}
				exRpcConnectionCreateFlag |= ExRpcConnectionCreateFlag.WebServices;
				goto IL_28C;
			}
			else
			{
				if (xropClient != null)
				{
					throw MapiExceptionHelper.ArgumentException("xropClient", "Cannot be used with non-web services request");
				}
				if (clientSessionInfo != null)
				{
					throw MapiExceptionHelper.ArgumentException("clientSessionInfo", "Cannot be used with non-web services request");
				}
				if ((connectFlags & ConnectFlag.RemoteSystemService) != ConnectFlag.None)
				{
					throw MapiExceptionHelper.ArgumentException("connectFlags", "RemoteSystemService only supported on web services request");
				}
				goto IL_28C;
			}
			try
			{
				for (;;)
				{
					IL_28C:
					correctServerDN = null;
					int lcidString;
					int lcidSort;
					int cpid;
					MapiCultureInfo.RetrieveConnectParameters(cultureInfo, out lcidString, out lcidSort, out cpid);
					exRpcConnection = MapiStore.GetDefaultExRpcConnectionFactory().Create(new ExRpcConnectionInfo(exRpcConnectionCreateFlag, connectFlags, text, guidMdb, userDn, userName, domainName, password, httpProxyServerName, 0, lcidString, lcidSort, cpid, 0, 98304, xropClient, clientSessionInfo, applicationId, connectionTimeout, callTimeout));
					mapiStore = exRpcConnection.OpenMsgStore(storeFlags, mailboxDn, guidMailbox, guidMdb, out correctServerDN, ((connectFlags & ConnectFlag.UseDelegatedAuthPrivilege) != ConnectFlag.None) ? clientIdentity : null, ((connectFlags & ConnectFlag.UseDelegatedAuthPrivilege) != ConnectFlag.None) ? userDn : null, unifiedLogon, applicationId, tenantHint ?? null, cultureInfo);
					if (mapiStore != null)
					{
						break;
					}
					exRpcConnection.Dispose();
					exRpcConnection = null;
					if (flag)
					{
						goto Block_28;
					}
					if (!wantRedirect)
					{
						goto Block_29;
					}
					if (!string.IsNullOrEmpty(correctServerDN))
					{
						if (!hashSet.Contains(correctServerDN))
						{
							hashSet.Add(correctServerDN);
							text = correctServerDN;
							if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
							{
								ComponentTrace<MapiNetTags>.Trace<string>(9106, 24, 0L, "MapiStore.OpenMapiStore: public store redirected to {0}.", TraceUtils.MakeString(text));
							}
						}
						else
						{
							if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
							{
								ComponentTrace<MapiNetTags>.Trace<string>(13202, 24, 0L, "MapiStore.OpenMapiStore: redirection loop detected on server {0}.", TraceUtils.MakeString(correctServerDN));
							}
							MapiExceptionHelper.ThrowIfError(string.Format("Loop detected while redirecting to the public folder server {0}. Servers already tried: {1}", TraceUtils.MakeString(correctServerDN), TraceUtils.DumpMvString(hashSet.ToArray<string>())), -2147221233);
						}
					}
					else
					{
						if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
						{
							ComponentTrace<MapiNetTags>.Trace<string>(15250, 24, 0L, "MapiStore.OpenMapiStore: public store is not found on server {0} and no redirection provided.", TraceUtils.MakeString(text));
						}
						MapiExceptionHelper.ThrowIfError(string.Format("Public Store is not found on server {0}.", TraceUtils.MakeString(text)), -2147221233);
					}
					if (!wantRedirect)
					{
						goto IL_43F;
					}
				}
				exRpcConnection = null;
				goto IL_43F;
				Block_28:
				throw MapiExceptionHelper.WrongServerException(string.Format("Unable to open mailbox {0} on server {1}.", TraceUtils.MakeString(mailboxDn), TraceUtils.MakeString(text)));
				Block_29:
				throw MapiExceptionHelper.WrongServerException(string.Format("Unable to open public folder {0} on server {1}. Redirecting is not requested.", TraceUtils.MakeString(mailboxDn), TraceUtils.MakeString(text)));
				IL_43F:;
			}
			finally
			{
				if (exRpcConnection != null)
				{
					exRpcConnection.Dispose();
					exRpcConnection = null;
				}
			}
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(10130, 24, 0L, "MapiStore.OpenMapiStore results: mapiStore={0}, correctServerDn={1}.", TraceUtils.MakeHash(mapiStore), TraceUtils.MakeString(correctServerDN));
			}
			return mapiStore;
		}

		public static void SetLocalServerFqdn(string localServerFqdn)
		{
			if (MapiStore.localServerFqdnInitialized || string.IsNullOrEmpty(localServerFqdn))
			{
				return;
			}
			lock (MapiStore.syncLock)
			{
				if (!MapiStore.localServerFqdnInitialized)
				{
					MapiStore.localServerFqdn = localServerFqdn;
					MapiStore.localServerFqdnInitialized = true;
				}
			}
		}

		public static string GetLocalServerFqdn()
		{
			return MapiStore.localServerFqdn;
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, string mailboxDn, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, ClientIdentityInfo clientIdentity, CultureInfo cultureInfo, string applicationId, byte[] tenantPartitionHint)
		{
			return MapiStore.OpenMailbox(serverDn, userDn, mailboxDn, userName, domainName, password, connectFlags, storeFlags, clientIdentity, cultureInfo, applicationId, tenantPartitionHint, false);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, string mailboxDn, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, ClientIdentityInfo clientIdentity, CultureInfo cultureInfo, string applicationId, byte[] tenantPartitionHint, bool unifiedLogon)
		{
			string text = null;
			return MapiStore.OpenMapiStore(serverDn, userDn, mailboxDn, Guid.Empty, Guid.Empty, userName, domainName, password, null, connectFlags, storeFlags & ~OpenStoreFlag.Public, cultureInfo, false, out text, clientIdentity, unifiedLogon, applicationId, null, false, null, TimeSpan.Zero, TimeSpan.Zero, tenantPartitionHint);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, Guid guidMailbox, Guid guidMdb, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string applicationId, byte[] tenantPartitionHint)
		{
			return MapiStore.OpenMailbox(serverDn, userDn, guidMailbox, guidMdb, userName, domainName, password, connectFlags, storeFlags, cultureInfo, windowsIdentity, applicationId, tenantPartitionHint, false);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, Guid guidMailbox, Guid guidMdb, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string applicationId, byte[] tenantPartitionHint, bool unifiedLogon)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverDn, userDn, null, guidMailbox, guidMdb, userName, domainName, password, null, connectFlags, storeFlags & ~OpenStoreFlag.Public, cultureInfo, false, out text, clientIdentity, unifiedLogon, applicationId, null, false, null, TimeSpan.Zero, TimeSpan.Zero, tenantPartitionHint);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, Guid guidMailbox, Guid guidMdb, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string applicationId, TimeSpan connectionTimeout, TimeSpan callTimeout, byte[] tenantPartitionHint)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverDn, userDn, null, guidMailbox, guidMdb, userName, domainName, password, null, connectFlags, storeFlags & ~OpenStoreFlag.Public, cultureInfo, false, out text, clientIdentity, false, applicationId, null, false, null, connectionTimeout, callTimeout, tenantPartitionHint);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, Guid guidMailbox, Guid guidMdb, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, ClientIdentityInfo clientIdentity, string applicationId, TimeSpan connectionTimeout, TimeSpan callTimeout, byte[] tenantPartitionHint)
		{
			string text = null;
			return MapiStore.OpenMapiStore(serverDn, userDn, null, guidMailbox, guidMdb, userName, domainName, password, null, connectFlags, storeFlags & ~OpenStoreFlag.Public, cultureInfo, false, out text, clientIdentity, false, applicationId, null, false, null, connectionTimeout, callTimeout, tenantPartitionHint);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, Guid guidMailbox, Guid guidMdb, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, ClientIdentityInfo clientIdentity, string applicationId, byte[] tenantPartitionHint)
		{
			return MapiStore.OpenMailbox(serverDn, userDn, guidMailbox, guidMdb, userName, domainName, password, connectFlags, storeFlags, cultureInfo, clientIdentity, applicationId, tenantPartitionHint, false);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, Guid guidMailbox, Guid guidMdb, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, ClientIdentityInfo clientIdentity, string applicationId, byte[] tenantPartitionHint, bool unifiedLogon)
		{
			string text = null;
			return MapiStore.OpenMapiStore(serverDn, userDn, null, guidMailbox, guidMdb, userName, domainName, password, null, connectFlags, storeFlags & ~OpenStoreFlag.Public, cultureInfo, false, out text, clientIdentity, unifiedLogon, applicationId, null, false, null, TimeSpan.Zero, TimeSpan.Zero, tenantPartitionHint);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, string mailboxDn, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string applicationId, byte[] tenantPartitionHint)
		{
			return MapiStore.OpenMailbox(serverDn, userDn, mailboxDn, userName, domainName, password, connectFlags, storeFlags, cultureInfo, windowsIdentity, applicationId, tenantPartitionHint, false);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, string mailboxDn, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string applicationId, byte[] tenantPartitionHint, bool unifiedLogon)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverDn, userDn, mailboxDn, Guid.Empty, Guid.Empty, userName, domainName, password, null, connectFlags, storeFlags & ~OpenStoreFlag.Public, cultureInfo, false, out text, clientIdentity, unifiedLogon, applicationId, null, false, null, TimeSpan.Zero, TimeSpan.Zero, tenantPartitionHint);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, string mailboxDn, string userName, string domainName, string password, string httpProxyServerName, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string applicationId)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverDn, userDn, mailboxDn, Guid.Empty, Guid.Empty, userName, domainName, password, httpProxyServerName, connectFlags, storeFlags & ~OpenStoreFlag.Public, cultureInfo, false, out text, clientIdentity, false, applicationId, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, string mailboxDn, string userName, string domainName, string password, string httpProxyServerName, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string applicationId, TimeSpan connectionTimeout, TimeSpan callTimeout)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverDn, userDn, mailboxDn, Guid.Empty, Guid.Empty, userName, domainName, password, httpProxyServerName, connectFlags, storeFlags & ~OpenStoreFlag.Public, cultureInfo, false, out text, clientIdentity, false, applicationId, null, false, null, connectionTimeout, callTimeout, null);
		}

		public static MapiStore OpenMailbox(string serverDn, string userDn, string mailboxDn, string userName, string domainName, string password, string httpProxyServerName, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, ClientIdentityInfo clientIdentity, string applicationId, TimeSpan connectionTimeout, TimeSpan callTimeout)
		{
			string text = null;
			return MapiStore.OpenMapiStore(serverDn, userDn, mailboxDn, Guid.Empty, Guid.Empty, userName, domainName, password, httpProxyServerName, connectFlags, storeFlags & ~OpenStoreFlag.Public, cultureInfo, false, out text, clientIdentity, false, applicationId, null, false, null, connectionTimeout, callTimeout, null);
		}

		public static MapiStore OpenRemoteMailbox(string targetUri, string userDn, string mailboxDn, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, Client xropClient, string userSmtpAddress, string applicationId, byte[] clientSessionInfo)
		{
			string text = null;
			if (userDn == null && (connectFlags & ConnectFlag.UseAdminPrivilege) != ConnectFlag.None)
			{
				userDn = "*/cn=Microsoft Federated System Attendant";
				storeFlags |= OpenStoreFlag.UseAdminPrivilege;
				storeFlags &= ~OpenStoreFlag.NoLocalization;
			}
			else if ((connectFlags & ConnectFlag.RemoteSystemService) != ConnectFlag.None)
			{
				throw MapiExceptionHelper.ArgumentException("connectFlags", "RemoteSystemService only supported on Federated System Attendant web services request");
			}
			if (!string.IsNullOrEmpty(applicationId))
			{
				if ((storeFlags & OpenStoreFlag.NoExtendedFlags) == OpenStoreFlag.NoExtendedFlags)
				{
					throw MapiExceptionHelper.ArgumentException("storeFlags", "When passing applicationId, NoExtendedFlags should not be used.");
				}
				storeFlags |= (OpenStoreFlag)((ulong)int.MinValue);
			}
			else
			{
				storeFlags |= OpenStoreFlag.NoExtendedFlags;
			}
			return MapiStore.OpenMapiStore(targetUri, userDn, mailboxDn, Guid.Empty, Guid.Empty, userSmtpAddress, null, null, null, connectFlags, storeFlags & ~OpenStoreFlag.Public, cultureInfo, false, out text, null, false, applicationId, xropClient, true, clientSessionInfo, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStore(string serverFQDN, Guid databaseGuid, string userLegacyDN, ConnectFlag connectFlags, OpenStoreFlag openStoreFlags, CultureInfo cultureInfo, ClientIdentityInfo clientIdentityInfo, string clientInfo)
		{
			string text = null;
			return MapiStore.OpenMapiStore(serverFQDN, userLegacyDN, null, Guid.Empty, databaseGuid, null, null, null, null, connectFlags, openStoreFlags | OpenStoreFlag.Public | OpenStoreFlag.IgnoreHomeMdb, cultureInfo, false, out text, clientIdentityInfo, false, clientInfo, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStore(string serverFQDN, Guid databaseGuid, string userLegacyDN, ConnectFlag connectFlags, OpenStoreFlag openStoreFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string clientInfo)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverFQDN, userLegacyDN, null, Guid.Empty, databaseGuid, null, null, null, null, connectFlags, openStoreFlags | OpenStoreFlag.Public | OpenStoreFlag.IgnoreHomeMdb, cultureInfo, false, out text, clientIdentity, false, clientInfo, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStore(string serverFQDN, Guid databaseGuid, string userLegacyDN, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag openStoreFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string clientInfo)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverFQDN, userLegacyDN, null, Guid.Empty, databaseGuid, userName, domainName, password, null, connectFlags, openStoreFlags | OpenStoreFlag.Public | OpenStoreFlag.IgnoreHomeMdb, cultureInfo, false, out text, clientIdentity, false, clientInfo, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStore(string serverFQDN, Guid databaseGuid, string userLegacyDN, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag openStoreFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string clientInfo, TimeSpan connectionTimeout, TimeSpan callTimeout)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverFQDN, userLegacyDN, null, Guid.Empty, databaseGuid, userName, domainName, password, null, connectFlags, openStoreFlags | OpenStoreFlag.Public | OpenStoreFlag.IgnoreHomeMdb, cultureInfo, false, out text, clientIdentity, false, clientInfo, null, false, null, connectionTimeout, callTimeout, null);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, string clientInfo)
		{
			return MapiStore.OpenPublicStore(serverDn, userDn, null, null, null, ConnectFlag.None, OpenStoreFlag.None, Thread.CurrentThread.CurrentCulture, null, clientInfo);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, CultureInfo cultureInfo, string clientInfo)
		{
			return MapiStore.OpenPublicStore(serverDn, userDn, null, null, null, ConnectFlag.None, OpenStoreFlag.None, cultureInfo, null, clientInfo);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, string userName, string domainName, string password, string clientInfo)
		{
			return MapiStore.OpenPublicStore(serverDn, userDn, userName, domainName, password, ConnectFlag.None, OpenStoreFlag.None, Thread.CurrentThread.CurrentCulture, null, clientInfo);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, string userName, string domainName, string password, CultureInfo cultureInfo, string clientInfo)
		{
			return MapiStore.OpenPublicStore(serverDn, userDn, userName, domainName, password, ConnectFlag.None, OpenStoreFlag.None, cultureInfo, null, clientInfo);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, string clientInfo)
		{
			return MapiStore.OpenPublicStore(serverDn, userDn, userName, domainName, password, connectFlags, storeFlags, Thread.CurrentThread.CurrentCulture, null, clientInfo);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, WindowsIdentity windowsIdentity, string clientInfo)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverDn, userDn, null, Guid.Empty, Guid.Empty, userName, domainName, password, null, connectFlags, storeFlags | OpenStoreFlag.Public, Thread.CurrentThread.CurrentCulture, true, out text, clientIdentity, false, clientInfo, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, string clientInfo)
		{
			string text = null;
			return MapiStore.OpenMapiStore(serverDn, userDn, null, Guid.Empty, Guid.Empty, userName, domainName, password, null, connectFlags, storeFlags | OpenStoreFlag.Public, cultureInfo, true, out text, null, false, clientInfo, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, string userName, string domainName, string password, string httpProxyServerName, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, string clientInfo)
		{
			string text = null;
			return MapiStore.OpenMapiStore(serverDn, userDn, null, Guid.Empty, Guid.Empty, userName, domainName, password, httpProxyServerName, connectFlags, storeFlags | OpenStoreFlag.Public, cultureInfo, true, out text, null, false, clientInfo, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string applicationId)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverDn, userDn, null, Guid.Empty, Guid.Empty, userName, domainName, password, null, connectFlags, storeFlags | OpenStoreFlag.Public, cultureInfo, true, out text, clientIdentity, false, applicationId, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, string userName, string domainName, string password, string httpProxyServerName, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string applicationId)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverDn, userDn, null, Guid.Empty, Guid.Empty, userName, domainName, password, httpProxyServerName, connectFlags, storeFlags | OpenStoreFlag.Public, cultureInfo, true, out text, clientIdentity, false, applicationId, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, string userName, string domainName, string password, string httpProxyServerName, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, ClientIdentityInfo clientIdentity, string applicationId)
		{
			string text;
			return MapiStore.OpenMapiStore(serverDn, userDn, null, Guid.Empty, Guid.Empty, userName, domainName, password, httpProxyServerName, connectFlags, storeFlags | OpenStoreFlag.Public, cultureInfo, true, out text, clientIdentity, false, applicationId, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStore(string serverDn, string userDn, string userName, string domainName, string password, string httpProxyServerName, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, WindowsIdentity windowsIdentity, string applicationId, TimeSpan connectionTimeout, TimeSpan callTimeout)
		{
			string text = null;
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverDn, userDn, null, Guid.Empty, Guid.Empty, userName, domainName, password, httpProxyServerName, connectFlags, storeFlags | OpenStoreFlag.Public, cultureInfo, true, out text, clientIdentity, false, applicationId, null, false, null, connectionTimeout, callTimeout, null);
		}

		public static MapiStore OpenPublicStore(string serverDn, Guid mdbGuid, ClientIdentityInfo clientIdentity, string userDnAs, ConnectFlag connectFlags, OpenStoreFlag openStoreFlags, CultureInfo cultureInfo, string applicationId)
		{
			string text;
			return MapiStore.OpenMapiStore(serverDn, ((connectFlags & ConnectFlag.UseDelegatedAuthPrivilege) != ConnectFlag.None) ? serverDn : userDnAs, null, Guid.Empty, mdbGuid, null, null, null, null, connectFlags, openStoreFlags | OpenStoreFlag.Public, cultureInfo, true, out text, clientIdentity, false, applicationId, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStoreNoRedirect(string serverDn, string userDn, string userName, string domainName, string password, string httpProxyServerName, ConnectFlag connectFlags, OpenStoreFlag storeFlags, out string correctServerDN, WindowsIdentity windowsIdentity, string applicationId)
		{
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverDn, userDn, null, Guid.Empty, Guid.Empty, userName, domainName, password, httpProxyServerName, connectFlags, storeFlags | OpenStoreFlag.Public, Thread.CurrentThread.CurrentCulture, false, out correctServerDN, clientIdentity, false, applicationId, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static MapiStore OpenPublicStoreNoRedirect(string serverDn, string userDn, string userName, string domainName, string password, ConnectFlag connectFlags, OpenStoreFlag storeFlags, CultureInfo cultureInfo, out string correctServerDN, WindowsIdentity windowsIdentity, string applicationId)
		{
			ClientIdentityInfo clientIdentity = null;
			if (windowsIdentity != null)
			{
				clientIdentity = new ClientIdentityInfo(windowsIdentity.Token);
			}
			return MapiStore.OpenMapiStore(serverDn, userDn, null, Guid.Empty, Guid.Empty, userName, domainName, password, null, connectFlags, storeFlags | OpenStoreFlag.Public, cultureInfo, false, out correctServerDN, clientIdentity, false, applicationId, null, false, null, TimeSpan.Zero, TimeSpan.Zero, null);
		}

		public static byte[] GetAddressBookEntryIdFromLegacyDN(string userLegacyDN)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(14226, 24, 0L, "MapiStore.GetAddressBookEntryIdFromLegacyDN params: userLegacyDN={0}", userLegacyDN);
			}
			IExRpcManageInterface exRpcManageInterface = null;
			byte[] result;
			try
			{
				exRpcManageInterface = ExRpcModule.GetExRpcManage();
				byte[] array;
				int num = exRpcManageInterface.CreateAddressBookEntryIdFromLegacyDN(userLegacyDN, out array);
				if (num != 0)
				{
					MapiExceptionHelper.ThrowIfError(string.Format("Unable to create a user entry ID from legacy DN '{0}'.", userLegacyDN), num, exRpcManageInterface, null);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(12178, 24, 0L, "MapiStore.GetAddressBookEntryIdFromLegacyDN results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				exRpcManageInterface.DisposeIfValid();
			}
			return result;
		}

		public static string GetLegacyDNFromAddressBookEntryId(byte[] entryID)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(17106, 24, 0L, "MapiStore.GetLegacyDNFromAddressBookEntryId params: entryID={0}", TraceUtils.DumpEntryId(entryID));
			}
			IExRpcManageInterface exRpcManageInterface = null;
			string text = null;
			string result;
			try
			{
				exRpcManageInterface = ExRpcModule.GetExRpcManage();
				int num = exRpcManageInterface.CreateLegacyDNFromAddressBookEntryId(entryID.Length, entryID, out text);
				if (num != 0)
				{
					MapiExceptionHelper.ThrowIfError(string.Format("Unable to create a user legacy DN from entry ID '{0}'.", TraceUtils.DumpEntryId(entryID)), num, exRpcManageInterface, null);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(25298, 24, 0L, "MapiStore.GetLegacyDNFromAddressBookEntryId results: legacyDN={0}", text);
				}
				result = text;
			}
			finally
			{
				exRpcManageInterface.DisposeIfValid();
			}
			return result;
		}

		public static byte[] GetAddressBookEntryIdFromLocalDirectorySID(byte[] localDirectoryUserSid)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<int, string>(58957, 24, 0L, "MapiStore.GetAddressBookEntryIdFromLocalDirectorySID params: sid=[len={0}, data={1}]", localDirectoryUserSid.Length, TraceUtils.DumpBytes(localDirectoryUserSid));
			}
			IExRpcManageInterface exRpcManageInterface = null;
			byte[] result;
			try
			{
				byte[] array = null;
				exRpcManageInterface = ExRpcModule.GetExRpcManage();
				int num = exRpcManageInterface.CreateAddressBookEntryIdFromLocalDirectorySID(localDirectoryUserSid.Length, localDirectoryUserSid, out array);
				if (num != 0)
				{
					MapiExceptionHelper.ThrowIfError("Unable to create a user entry ID from local directory SID.", num, exRpcManageInterface, null);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(34381, 24, 0L, "MapiStore.GetAddressBookEntryIdFromLocalDirectorySID results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				exRpcManageInterface.DisposeIfValid();
			}
			return result;
		}

		public static byte[] GetLocalDirectorySIDFromAddressBookEntryId(byte[] entryID)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(50765, 24, 0L, "MapiStore.GetLocalDirectorySIDFromAddressBookEntryId params: entryID={0}", TraceUtils.DumpEntryId(entryID));
			}
			IExRpcManageInterface exRpcManageInterface = null;
			byte[] result;
			try
			{
				byte[] array = null;
				exRpcManageInterface = ExRpcModule.GetExRpcManage();
				int num = exRpcManageInterface.CreateLocalDirectorySIDFromAddressBookEntryId(entryID.Length, entryID, out array);
				if (num != 0)
				{
					MapiExceptionHelper.ThrowIfError(string.Format("Unable to create local directory user SID from entry ID '{0}'.", TraceUtils.DumpEntryId(entryID)), num, exRpcManageInterface, null);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<int, string>(47693, 24, 0L, "MapiStore.GetLocalDirectorySIDFromAddressBookEntryId results: sid=[len={0}, data={1}]", array.Length, TraceUtils.DumpBytes(array));
				}
				result = array;
			}
			finally
			{
				exRpcManageInterface.DisposeIfValid();
			}
			return result;
		}

		public static IdSet GetIdSetFromMapiManifestBlob(PropTag ptag, byte[] mapiManifestBlob)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<PropTag>(41308, 24, 0L, "MapiStore.GetIdSetFromMapiManifestBlob ptag={0}", ptag);
			}
			IExRpcManageInterface exRpcManageInterface = null;
			IdSet result = null;
			try
			{
				exRpcManageInterface = ExRpcModule.GetExRpcManage();
				byte[] array = null;
				using (MemoryStream memoryStream = MapiManifest.ExtractIdSetBlobFromManifestBlob(mapiManifestBlob))
				{
					MapiIStream iStream = new MapiIStream(memoryStream);
					int num = exRpcManageInterface.CreateIdSetBlobFromIStream(ptag, iStream, out array);
					if (num != 0)
					{
						if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
						{
							ComponentTrace<MapiNetTags>.Trace<PropTag>(57692, 24, 0L, "unable to create idset for proptag {0}", ptag);
						}
						MapiExceptionHelper.ThrowIfError(string.Format("Unable to create idset for proptag '{0}'.", ptag), num, exRpcManageInterface, null);
					}
				}
				if (array != null)
				{
					if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
					{
						ComponentTrace<MapiNetTags>.Trace<int, string>(45404, 24, 0L, "MapiStore.GetIdSetFromMapiManifestBlob results: sid=[len={0}, data={1}]", array.Length, TraceUtils.DumpBytes(array));
					}
					using (Reader reader = Reader.CreateBufferReader(array))
					{
						result = IdSet.ParseWithReplGuids(reader);
						if (reader.Length != reader.Position)
						{
							string message = "id set is corrupted- byte length mismatch";
							if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
							{
								ComponentTrace<MapiNetTags>.Trace(61788, 24, 0L, message);
							}
							MapiExceptionHelper.ThrowIfError(message, -2147221221, exRpcManageInterface, null);
						}
					}
				}
			}
			catch (BufferParseException ex)
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<BufferParseException>(37212, 24, 0L, "Couldn't parse id set:{0}", ex);
				}
				MapiExceptionHelper.ThrowIfError("couldn't parse id set", -2147221221, exRpcManageInterface, ex);
			}
			catch (IOException ex2)
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<IOException>(33116, 24, 0L, "Error reading bytes of manifest:{0}", ex2);
				}
				MapiExceptionHelper.ThrowIfError("Error reading bytes of manifest", -2147221221, exRpcManageInterface, ex2);
			}
			finally
			{
				exRpcManageInterface.DisposeIfValid();
			}
			return result;
		}

		public static bool IsFolderEntryId(byte[] entryId)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(16274, 24, 0L, "MapiStore.IsFolderEntryId params: entryId={0}", TraceUtils.DumpEntryId(entryId));
			}
			EntryIdType entryIdType = MapiStore.GetEntryIdType(entryId);
			bool flag = (entryIdType & EntryIdType.Message) != EntryIdType.Message;
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<bool>(8594, 24, 0L, "MapiStore.IsFolderEntryId results: {0}", flag);
			}
			return flag;
		}

		public static bool IsMessageEntryId(byte[] entryId)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(12690, 24, 0L, "MapiStore.IsMessageEntryId params: entryId={0}", TraceUtils.DumpEntryId(entryId));
			}
			EntryIdType entryIdType = MapiStore.GetEntryIdType(entryId);
			bool flag = (entryIdType & EntryIdType.Message) == EntryIdType.Message;
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<bool>(10642, 24, 0L, "MapiStore.IsMessageEntryId results: {0}", flag);
			}
			return flag;
		}

		public static bool IsPublicEntryId(byte[] entryId)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(21356, 24, 0L, "MapiStore.IsPublicEntryId params: entryId={0}", TraceUtils.DumpEntryId(entryId));
			}
			EntryIdType entryIdType = MapiStore.GetEntryIdType(entryId);
			bool flag = (entryIdType & EntryIdType.Public) == EntryIdType.Public;
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<bool>(29548, 24, 0L, "MapiStore.IsPublicEntryId results: {0}", flag);
			}
			return flag;
		}

		public static byte[] GetFolderEntryIdFromMessageEntryId(byte[] entryId)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(18540, 24, 0L, "MapiStore.GetFolderEntryIdFromMessageEntryId params: entryId={0}", TraceUtils.DumpEntryId(entryId));
			}
			if (entryId == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryId");
			}
			if (entryId.Length < 1)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("entryId", "Cannot be zero length");
			}
			IExRpcManageInterface exRpcManageInterface = null;
			byte[] result;
			try
			{
				byte[] array = null;
				exRpcManageInterface = ExRpcModule.GetExRpcManage();
				int folderEntryIdFromMessageEntryId = exRpcManageInterface.GetFolderEntryIdFromMessageEntryId(entryId.Length, entryId, out array);
				if (folderEntryIdFromMessageEntryId != 0)
				{
					MapiExceptionHelper.ThrowIfError(string.Format("Unable to get folder entryId from message entryId '{0}'.", TraceUtils.DumpEntryId(entryId)), folderEntryIdFromMessageEntryId, exRpcManageInterface, null);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(26732, 24, 0L, "MapiStore.GetFolderEntryIdFromMessageEntryId results: {0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				exRpcManageInterface.DisposeIfValid();
			}
			return result;
		}

		internal MapiStore(IExMapiStore iMsgStore, IMsgStore externalIMsgStore, ExRpcConnection exRpcConnection, string applicationId) : base(iMsgStore, externalIMsgStore, null, MapiStore.IMsgStoreGuids)
		{
			this.iMsgStore = iMsgStore;
			this.exRpcConnection = exRpcConnection;
			this.childObjects = new DisposableRef(null);
			this.nonIpmSubtreeEntryId = null;
			this.applicationId = applicationId;
			this.creationTime = DateTime.UtcNow;
			this.apartmentState = Thread.CurrentThread.GetApartmentState();
			this.creationThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		internal static void SetTestDefaultExRpcConnectionFactory(IExRpcConnectionFactory testConnectionFactory)
		{
			MapiStore.defaultExRpcConnectionFactory = testConnectionFactory;
		}

		internal IntPtr IMsgStore
		{
			get
			{
				return ((SafeExMapiStoreHandle)this.iMsgStore).DangerousGetHandle();
			}
		}

		internal DisposableRef AddChildReference(IForceReportDisposeTrackable iObject)
		{
			return this.childObjects.AddToList(iObject);
		}

		internal void LockConnection()
		{
			if (this.exRpcConnection != null)
			{
				this.exRpcConnection.Lock();
			}
		}

		internal void UnlockConnection()
		{
			if (this.exRpcConnection != null)
			{
				this.exRpcConnection.Unlock();
			}
		}

		internal Exception LastLowLevelException
		{
			get
			{
				if (this.exRpcConnection != null)
				{
					return this.exRpcConnection.InternalLowLevelException;
				}
				return null;
			}
		}

		public void SetIdentityToDispose(IDisposable identityToDispose)
		{
			this.identityToDispose = identityToDispose;
		}

		private void DisposeAllChildren()
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace(14738, 24, (long)this.GetHashCode(), "MapiStore.DisposeAllChildren");
			}
			base.CheckDisposed();
			for (;;)
			{
				DisposableRef disposableRef = this.childObjects.NextRef(this.childObjects);
				if (disposableRef == null)
				{
					break;
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(9618, 24, (long)this.GetHashCode(), "Child {0} object not properly disposed (obj = {1})", disposableRef.Child.ToString(), TraceUtils.MakeHash(disposableRef.Child));
				}
				disposableRef.Child.ForceLeakReport();
				disposableRef.Dispose();
				DisposableRef.RemoveFromList(disposableRef);
			}
		}

		internal override NamedProp[] GetNamesFromIDs(Guid guidPropSet, int ulFlags, ICollection<PropTag> tags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			NamedProp[] namesFromIDs;
			using (MapiFolder rootFolder = this.GetRootFolder())
			{
				namesFromIDs = rootFolder.GetNamesFromIDs(guidPropSet, ulFlags, tags);
			}
			return namesFromIDs;
		}

		internal MapiStore OpenAlternateStore(string mailboxDn, Guid guidMailbox, Guid guidMdb, OpenStoreFlag storeFlags, CultureInfo cultureInfo, out string correctServerDn, bool wantCorrectServer, bool unifiedLogon, string applicationId, byte[] tenantPartitionHint)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			MapiStore mapiStore = null;
			bool flag = (storeFlags & OpenStoreFlag.Public) == OpenStoreFlag.None;
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace(13714, 24, (long)this.GetHashCode(), "MapiStore.OpenAlternateStore params: mailboxDn={0}, guidMailbox={1}, guidMdb={2}, openStoreFlags={3}, cultureInfo={4}", new object[]
				{
					TraceUtils.MakeString(mailboxDn),
					guidMailbox.ToString(),
					guidMdb.ToString(),
					storeFlags.ToString(),
					(cultureInfo == null) ? "null" : cultureInfo.DisplayName
				});
			}
			this.LockConnection();
			try
			{
				mapiStore = this.exRpcConnection.OpenMsgStore(storeFlags, mailboxDn, guidMailbox, guidMdb, out correctServerDn, null, null, unifiedLogon, applicationId, tenantPartitionHint ?? null, cultureInfo);
				if (mapiStore == null)
				{
					if (!string.IsNullOrEmpty(correctServerDn))
					{
						if (!wantCorrectServer)
						{
							if (flag)
							{
								throw MapiExceptionHelper.WrongServerException("Unable to open mailbox on server.  The mailbox has been moved to server " + correctServerDn);
							}
							throw MapiExceptionHelper.WrongServerException("Unable to open public store on server.  The public store is located on server " + correctServerDn);
						}
					}
					else
					{
						if (flag)
						{
							throw MapiExceptionHelper.CallFailedException("Unable to open mailbox on server");
						}
						throw MapiExceptionHelper.CallFailedException("Unable to open public store on server");
					}
				}
			}
			finally
			{
				this.UnlockConnection();
			}
			return mapiStore;
		}

		public bool IsE15Store
		{
			get
			{
				base.CheckDisposed();
				return this.IsVersionGreaterOrEqualThan(MapiStore.E15StoreVersion);
			}
		}

		public bool ClassicRulesInterfaceAvailable
		{
			get
			{
				base.CheckDisposed();
				this.LockConnection();
				base.BlockExternalObjectCheck();
				bool result;
				try
				{
					if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
					{
						ComponentTrace<MapiNetTags>.Trace(17424, 24, (long)this.GetHashCode(), "MapiStore.ClassicRulesInterfaceAvailable.");
					}
					bool flag;
					this.iMsgStore.GetIsRulesInterfaceAvailable(out flag);
					result = flag;
				}
				finally
				{
					this.UnlockConnection();
				}
				return result;
			}
		}

		public override void SuppressDisposeTracker()
		{
			IDisposeTrackable disposeTrackable = this.identityToDispose as IDisposeTrackable;
			if (disposeTrackable != null)
			{
				disposeTrackable.SuppressDisposeTracker();
			}
			if (this.exRpcConnection != null)
			{
				this.exRpcConnection.SuppressDisposeTracker();
			}
			base.SuppressDisposeTracker();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.LockConnection();
				try
				{
					if (this.identityToDispose != null)
					{
						this.identityToDispose.Dispose();
						this.identityToDispose = null;
					}
					if (this.childObjects != null)
					{
						this.DisposeAllChildren();
						this.childObjects.Dispose();
					}
					if (this.notificationHandles != null)
					{
						foreach (MapiNotificationHandle mapiNotificationHandle in this.notificationHandles)
						{
							if (mapiNotificationHandle.IsValid)
							{
								if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
								{
									ComponentTrace<MapiNetTags>.Trace<string>(60192, 24, (long)this.GetHashCode(), "MapiStore.Dispose Notification Handle: connection={0}", mapiNotificationHandle.Connection.ToString());
								}
								base.UnregisterNotificationHelper(mapiNotificationHandle.NotificationCallbackId);
								int num = this.iMsgStore.Unadvise(mapiNotificationHandle.Connection);
								mapiNotificationHandle.MarkAsInvalid();
								if (num != 0 && ComponentTrace<MapiNetTags>.CheckEnabled(24))
								{
									ComponentTrace<MapiNetTags>.Trace<int>(35616, 24, (long)this.GetHashCode(), "MapiStore.Dispose Notification Handle Unadvise failed, hr={0}", num);
								}
							}
						}
					}
					base.InternalDispose(disposing);
				}
				finally
				{
					this.iMsgStore = null;
					this.UnlockConnection();
				}
				if (this.exRpcConnection != null)
				{
					this.exRpcConnection.RemoveStoreReference(this);
					this.exRpcConnection = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiStore>(this);
		}

		public MapiNotificationHandle Advise(byte[] entryId, AdviseFlags eventMask, MapiNotificationHandler handler, MapiNotificationClientFlags notificationClientFlags = (MapiNotificationClientFlags)0)
		{
			return this.Advise(entryId, eventMask, handler, NotificationCallbackMode.Async, notificationClientFlags);
		}

		public MapiNotificationHandle Advise(byte[] entryId, AdviseFlags eventMask, MapiNotificationHandler handler, NotificationCallbackMode mode, MapiNotificationClientFlags notificationClientFlags = (MapiNotificationClientFlags)0)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiNotificationHandle result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string, string>(11666, 24, (long)this.GetHashCode(), "MapiStore.Advise params: entryId={0}, eventMask={1}, handler={2}", TraceUtils.DumpEntryId(entryId), eventMask.ToString(), TraceUtils.MakeHash(handler));
				}
				NotificationHelper notificationHelper = new NotificationHelper(handler, mode == NotificationCallbackMode.Sync);
				MapiNotificationHandle mapiNotificationHandle = new MapiNotificationHandle(handler);
				IntPtr zero = IntPtr.Zero;
				bool flag = false;
				ulong num = base.RegisterNotificationHelper(notificationHelper);
				try
				{
					int num2 = this.iMsgStore.AdviseEx(entryId, eventMask, NotificationCallbackHelper.Instance.IntPtrOnNotifyDelegate, num, out zero);
					mapiNotificationHandle.SetConnection(zero, num);
					if (num2 != 0)
					{
						base.ThrowIfError("Unable to register an event notification callback.", num2);
					}
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						base.UnregisterNotificationHelper(num);
					}
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(15762, 24, (long)this.GetHashCode(), "MapiStore.Advise results: result={0}", zero.ToString());
				}
				if ((notificationClientFlags & MapiNotificationClientFlags.AutoDisposeNotificationHandles) != (MapiNotificationClientFlags)0)
				{
					if (this.notificationHandles == null)
					{
						this.notificationHandles = new List<MapiNotificationHandle>();
					}
					this.notificationHandles.Add(mapiNotificationHandle);
				}
				result = mapiNotificationHandle;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetReceiveFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] receiveFolderEntryId;
			try
			{
				string text = null;
				receiveFolderEntryId = this.GetReceiveFolderEntryId(null, out text);
			}
			finally
			{
				this.UnlockConnection();
			}
			return receiveFolderEntryId;
		}

		public byte[] GetReceiveFolder(string messageClass)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] receiveFolderEntryId;
			try
			{
				string text = null;
				receiveFolderEntryId = this.GetReceiveFolderEntryId(messageClass, out text);
			}
			finally
			{
				this.UnlockConnection();
			}
			return receiveFolderEntryId;
		}

		public byte[] GetReceiveFolder(string messageClass, out string explicitSuperClass)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] receiveFolderEntryId;
			try
			{
				receiveFolderEntryId = this.GetReceiveFolderEntryId(messageClass, out explicitSuperClass);
			}
			finally
			{
				this.UnlockConnection();
			}
			return receiveFolderEntryId;
		}

		public byte[] GetReceiveFolderEntryId()
		{
			base.CheckDisposed();
			this.LockConnection();
			byte[] receiveFolderEntryId;
			try
			{
				string text = null;
				receiveFolderEntryId = this.GetReceiveFolderEntryId(null, out text);
			}
			finally
			{
				this.UnlockConnection();
			}
			return receiveFolderEntryId;
		}

		public byte[] GetReceiveFolderEntryId(string messageClass)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] receiveFolderEntryId;
			try
			{
				string text = null;
				receiveFolderEntryId = this.GetReceiveFolderEntryId(messageClass, out text);
			}
			finally
			{
				this.UnlockConnection();
			}
			return receiveFolderEntryId;
		}

		public byte[] GetReceiveFolderEntryId(string messageClass, out string explicitSuperClass)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			int num = 0;
			FaultInjectionUtils.FaultInjectionTracer.TraceTest<int>(2179345725U, ref num);
			if (num != 0)
			{
				Thread.Sleep(TimeSpan.FromSeconds((double)num));
			}
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(8850, 24, (long)this.GetHashCode(), "MapiStore.GetReceiveFolder params: messageClass={0}", TraceUtils.MakeString(messageClass));
				}
				byte[] array;
				int receiveFolder = this.iMsgStore.GetReceiveFolder(messageClass, int.MinValue, out array, out explicitSuperClass);
				if (receiveFolder != 0)
				{
					base.ThrowIfError("Unable to get receive folder.", receiveFolder);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(12946, 24, (long)this.GetHashCode(), "MapiStore.GetReceiveFolder resultEntryId={0}, explicitSuperClass={1}", TraceUtils.DumpEntryId(array), TraceUtils.MakeString(explicitSuperClass));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public PropValue[][] GetReceiveFolderInfo()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			PropValue[][] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(33059, 24, (long)this.GetHashCode(), "MapiStore.GetReceiveFolderInfo");
				}
				PropValue[][] array;
				int receiveFolderInfo = this.iMsgStore.GetReceiveFolderInfo(out array);
				if (receiveFolderInfo != 0)
				{
					base.ThrowIfError("Unable to get receive folder table.", receiveFolderInfo);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(49443, 24, (long)this.GetHashCode(), "MapiStore.GetReceiveFolderInfo results: rows={0}", TraceUtils.DumpPropValsMatrix(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetTransportQueueFolderId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(34709, 24, (long)this.GetHashCode(), "MapiStore.GetTransportQueueFolderId.");
				}
				long fid;
				int transportQueueFolderId = this.iMsgStore.GetTransportQueueFolderId(out fid);
				if (transportQueueFolderId != 0)
				{
					base.ThrowIfError("Unable to get transport queue folder.", transportQueueFolderId);
				}
				result = this.CreateEntryId(fid);
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public void SetReceiveFolder(string messageClass, byte[] entryID)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(10898, 24, (long)this.GetHashCode(), "MapiStore.SetReceiveFolder params: messageClass={0}, entryID={1}", TraceUtils.MakeString(messageClass), TraceUtils.DumpEntryId(entryID));
				}
				int num = this.iMsgStore.SetReceiveFolder(messageClass, int.MinValue, entryID);
				if (num != 0)
				{
					base.ThrowIfError("Unable to get receive folder.", num);
				}
			}
			finally
			{
				this.UnlockConnection();
			}
		}

		public void AbortSubmit(byte[] entryID)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(14994, 24, (long)this.GetHashCode(), "MapiStore.AbortSubmit params: entryID={0}", TraceUtils.DumpEntryId(entryID));
				}
				int num = this.iMsgStore.AbortSubmit(entryID, 0);
				if (num != 0)
				{
					base.ThrowIfError("Unable to abort message submission.", num);
				}
			}
			finally
			{
				this.UnlockConnection();
			}
		}

		public object OpenEntry(byte[] entryID)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			object result;
			try
			{
				result = this.OpenEntry(entryID, OpenEntryFlags.BestAccess | OpenEntryFlags.DeferredErrors);
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public object OpenEntry(byte[] entryID, OpenEntryFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			object result;
			try
			{
				FaultInjectionUtils.FaultInjectionTracer.TraceTest(3295030589U);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(9874, 24, (long)this.GetHashCode(), "MapiStore.OpenEntry params: entryID={0}, flags={1}", TraceUtils.DumpEntryId(entryID), flags.ToString());
				}
				object obj = null;
				int objType = 0;
				IExInterface exInterface = null;
				int ulFlags = (int)(flags & ~OpenEntryFlags.DontThrowIfEntryIsMissing);
				try
				{
					int num = this.iMsgStore.OpenEntry(entryID, Guid.Empty, ulFlags, out objType, out exInterface);
					if (num == -2147221233 && (flags & OpenEntryFlags.DontThrowIfEntryIsMissing) != OpenEntryFlags.None)
					{
						return null;
					}
					if (num != 0)
					{
						base.ThrowIfError("Unable to open entry ID.", num);
					}
					obj = MapiContainer.Wrap(exInterface, objType, this);
					exInterface = null;
				}
				finally
				{
					exInterface.DisposeIfValid();
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(13970, 24, (long)this.GetHashCode(), "MapiStore.OpenEntry results: objWrap={0}", TraceUtils.MakeHash(obj));
				}
				result = obj;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetRootFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder result;
			try
			{
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(null);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(11922, 24, (long)this.GetHashCode(), "MapiStore.GetRootFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetRootFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				result = (byte[])base.GetProp(PropTag.RootEntryId).Value;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetIpmSubtreeFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder result;
			try
			{
				byte[] entryID = (byte[])base.GetProp(PropTag.IpmSubtreeEntryId).Value;
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(entryID);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(16018, 24, (long)this.GetHashCode(), "MapiStore.GetIpmSubtreeFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetIpmSubtreeFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				result = (byte[])base.GetProp(PropTag.IpmSubtreeEntryId).Value;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetNonIpmSubtreeFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder result;
			try
			{
				byte[] entryID = (byte[])base.GetProp(PropTag.NonIpmSubtreeEntryId).Value;
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(entryID);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(8338, 24, (long)this.GetHashCode(), "MapiStore.GetNonIpmSubtreeFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetNonIpmSubtreeFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				if (this.nonIpmSubtreeEntryId == null)
				{
					this.nonIpmSubtreeEntryId = base.GetProp(PropTag.NonIpmSubtreeEntryId).GetBytes();
					if (this.nonIpmSubtreeEntryId == null)
					{
						using (MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(null))
						{
							this.nonIpmSubtreeEntryId = mapiFolder.GetProp(PropTag.EntryId).GetBytes();
						}
					}
				}
				result = this.nonIpmSubtreeEntryId;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetEFormsRegistryFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				result = (byte[])base.GetProp(PropTag.EFormsRegistryEntryId).Value;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetFreeBusyFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				result = (byte[])base.GetProp(PropTag.FreeBusyEntryId).Value;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetOfflineAddressBookFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				result = (byte[])base.GetProp(PropTag.OfflineAddressBookEntryId).Value;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetLocalSiteFreeBusyFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				result = (byte[])base.GetProp(PropTag.LocalSiteFreeBusyEntryId).Value;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetLocalSiteOfflineAddressBookFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				result = (byte[])base.GetProp(PropTag.LocalSiteOfflineAddressBookEntryId).Value;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetLocaleEFormsRegistryFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				result = (byte[])base.GetProp(PropTag.LocaleEFormsRegistryEntryId).Value;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetDeferredActionFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				result = (byte[])base.GetProp(PropTag.DeferredActionFolderEntryID).Value;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetInboxFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder mapiFolder = null;
			bool flag = false;
			MapiFolder result;
			try
			{
				byte[] inboxFolderEntryId = this.GetInboxFolderEntryId();
				mapiFolder = (MapiFolder)this.OpenEntry(inboxFolderEntryId);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(12434, 24, (long)this.GetHashCode(), "MapiStore.GetInboxFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				flag = true;
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
				if (!flag && mapiFolder != null)
				{
					mapiFolder.Dispose();
					mapiFolder = null;
				}
			}
			return result;
		}

		public byte[] GetInboxFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				byte[] array = (byte[])base.GetProp(PropTag.IpmInboxEntryId).Value;
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(10386, 24, (long)this.GetHashCode(), "MapiStore.GetInboxFolderEntryId results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetSentItemsFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder result;
			try
			{
				byte[] entryID = (byte[])base.GetProp(PropTag.IpmSentMailEntryId).Value;
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(entryID);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(14482, 24, (long)this.GetHashCode(), "MapiStore.GetSentItemsFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetSentItemsFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				byte[] array = (byte[])base.GetProp(PropTag.IpmSentMailEntryId).Value;
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(9362, 24, (long)this.GetHashCode(), "MapiStore.GetSentItemsFolderEntryId results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetOutboxFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder result;
			try
			{
				byte[] entryID = (byte[])base.GetProp(PropTag.IpmOutboxEntryId).Value;
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(entryID);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(13458, 24, (long)this.GetHashCode(), "MapiStore.GetOutboxFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetOutboxFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				byte[] array = (byte[])base.GetProp(PropTag.IpmOutboxEntryId).Value;
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(11410, 24, (long)this.GetHashCode(), "MapiStore.GetOutboxFolderEntryId results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetDeletedItemsFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder result;
			try
			{
				byte[] entryID = (byte[])base.GetProp(PropTag.IpmWasteBasketEntryId).Value;
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(entryID);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(15506, 24, (long)this.GetHashCode(), "MapiStore.GetDeletedItemsFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetDeletedItemsFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				byte[] array = (byte[])base.GetProp(PropTag.IpmWasteBasketEntryId).Value;
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(8978, 24, (long)this.GetHashCode(), "MapiStore.GetDeletedItemsFolderEntryId results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetSpoolerQueueFolderEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(31468, 24, (long)this.GetHashCode(), "MapiStore.GetSpoolerQueueFolderEntryId");
				}
				byte[] array = (byte[])base.GetProp(PropTag.SpoolerQueueEntryId).Value;
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(17132, 24, (long)this.GetHashCode(), "MapiStore.GetSpoolerQueueFolderEntryId results: fid={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetCalendarFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder result;
			try
			{
				byte[] specialFolderEntryId = this.GetSpecialFolderEntryId(PropTag.CalendarFolderEntryId);
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(specialFolderEntryId);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(13074, 24, (long)this.GetHashCode(), "MapiStore.GetCalendarFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetContactsFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder result;
			try
			{
				byte[] specialFolderEntryId = this.GetSpecialFolderEntryId(PropTag.ContactsFolderEntryId);
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(specialFolderEntryId);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(11026, 24, (long)this.GetHashCode(), "MapiStore.GetContactsFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetDraftsFolder()
		{
			base.CheckDisposed();
			this.LockConnection();
			MapiFolder result;
			try
			{
				byte[] specialFolderEntryId = this.GetSpecialFolderEntryId(PropTag.DraftsFolderEntryId);
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(specialFolderEntryId);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(15122, 24, (long)this.GetHashCode(), "MapiStore.GetDraftsFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetJournalFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder result;
			try
			{
				byte[] specialFolderEntryId = this.GetSpecialFolderEntryId(PropTag.JournalFolderEntryId);
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(specialFolderEntryId);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(10002, 24, (long)this.GetHashCode(), "MapiStore.GetJournalFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetTasksFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder result;
			try
			{
				byte[] specialFolderEntryId = this.GetSpecialFolderEntryId(PropTag.TasksFolderEntryId);
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(specialFolderEntryId);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(14098, 24, (long)this.GetHashCode(), "MapiStore.GetTasksFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public MapiFolder GetNotesFolder()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			MapiFolder result;
			try
			{
				byte[] specialFolderEntryId = this.GetSpecialFolderEntryId(PropTag.NotesFolderEntryId);
				MapiFolder mapiFolder = (MapiFolder)this.OpenEntry(specialFolderEntryId);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(12050, 24, (long)this.GetHashCode(), "MapiStore.GetNotesFolder results: folder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetSpecialFolderEntryId(PropTag specialFolderPropTag)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(16146, 24, (long)this.GetHashCode(), "MapiStore.GetSpecialFolderEntryId params: specialFolderPropTag={0}", TraceUtils.DumpPropTag(specialFolderPropTag));
				}
				using (MapiFolder inboxFolder = this.GetInboxFolder())
				{
					byte[] bytes = inboxFolder.GetProp(specialFolderPropTag).GetBytes();
					if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
					{
						ComponentTrace<MapiNetTags>.Trace<string>(8466, 24, (long)this.GetHashCode(), "MapiStore.GetSpecialFolderEntryId results: entryId={0}", TraceUtils.DumpEntryId(bytes));
					}
					result = bytes;
				}
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public void Unadvise(MapiNotificationHandle handle)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			try
			{
				if (handle.IsValid)
				{
					if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
					{
						ComponentTrace<MapiNetTags>.Trace<string>(12562, 24, (long)this.GetHashCode(), "MapiStore.Unadvise params: connection={0}", handle.Connection.ToString());
					}
					base.UnregisterNotificationHelper(handle.NotificationCallbackId);
					int num = this.iMsgStore.Unadvise(handle.Connection);
					handle.MarkAsInvalid();
					if (num != 0)
					{
						base.ThrowIfError("Unable to unregister an event notification callback.", num);
					}
				}
			}
			finally
			{
				this.UnlockConnection();
			}
		}

		public byte[] CreateEntryId(long fid)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			return this.InternalCreateEntryId(fid, 0L);
		}

		public byte[] CreateEntryId(long fid, long mid)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (mid == 0L)
			{
				throw MapiExceptionHelper.InvalidParameterException("mid");
			}
			if (fid == 0L)
			{
				throw MapiExceptionHelper.InvalidParameterException("fid");
			}
			return this.InternalCreateEntryId(fid, mid);
		}

		private byte[] InternalCreateEntryId(long fid, long mid)
		{
			this.LockConnection();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(9490, 24, (long)this.GetHashCode(), "MapiStore.CreateEntryId params: fid={0}, mid={1}", fid.ToString(), mid.ToString());
				}
				byte[] array;
				int num = this.iMsgStore.CreateEntryId(fid, mid, mid != 0L, true, out array);
				if (num != 0)
				{
					base.ThrowIfError("Unable to create a long-term entry ID from FID/MID pair.", num);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(13586, 24, (long)this.GetHashCode(), "MapiStore.CreateEntryId results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] CreatePublicEntryId(long fid)
		{
			return this.CreatePublicEntryId(fid, 0L);
		}

		public byte[] CreatePublicEntryId(long fid, long mid)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(50657, 24, (long)this.GetHashCode(), "MapiStore.CreatePublicEntryId params: fid={0}, mid={1}", fid.ToString(), mid.ToString());
				}
				byte[] array;
				int num = this.iMsgStore.CreatePublicEntryId(fid, mid, mid != 0L, out array);
				if (num != 0)
				{
					base.ThrowIfError("Unable to create a public long-term entry ID from FID/MID pair.", num);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(50717, 24, (long)this.GetHashCode(), "MapiStore.CreatePublicEntryId results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public long GetFidFromEntryId(byte[] entryId)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			long result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(62953, 24, (long)this.GetHashCode(), "MapiStore.GetFidFromEntryId params: entryId={0}", TraceUtils.DumpEntryId(entryId));
				}
				if (entryId == null)
				{
					throw MapiExceptionHelper.ArgumentNullException("entryId");
				}
				if (entryId.Length < 1)
				{
					throw MapiExceptionHelper.ArgumentOutOfRangeException("entryId", "Cannot be zero length");
				}
				bool flag;
				long num;
				long num2;
				int shortTermIdsFromLongTermEntryId = this.iMsgStore.GetShortTermIdsFromLongTermEntryId(entryId, out flag, out num, out num2);
				if (shortTermIdsFromLongTermEntryId != 0)
				{
					base.ThrowIfError("Unable to obtain the FID and MID from the entry id.", shortTermIdsFromLongTermEntryId);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<long>(38377, 24, (long)this.GetHashCode(), "MapiStore.GetFidFromEntryId results: fid={0}", num);
				}
				result = num;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public Guid GetMailboxInstanceGuid()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			Guid result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(30444, 24, (long)this.GetHashCode(), "MapiStore.GetMailboxInstanceGuid");
				}
				Guid guid;
				int mailboxInstanceGuid = this.iMsgStore.GetMailboxInstanceGuid(out guid);
				if (mailboxInstanceGuid != 0)
				{
					base.ThrowIfError("Unable to get a Mailbox Instance Guid.", mailboxInstanceGuid);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<Guid>(19180, 24, (long)this.GetHashCode(), "MapiStore.GetMailboxInstanceGuid results: mailboxInstanceGuid={0}", guid);
				}
				result = guid;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public void GetMdbIdMapping(out ushort replidServer, out Guid guidServer)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(27372, 24, (long)this.GetHashCode(), "MapiStore.GetMdbIdMapping");
				}
				int mdbIdMapping = this.iMsgStore.GetMdbIdMapping(out replidServer, out guidServer);
				if (mdbIdMapping != 0)
				{
					base.ThrowIfError("Unable to get an Mdb ID mapping pair.", mdbIdMapping);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<ushort, Guid>(23276, 24, (long)this.GetHashCode(), "MapiStore.GetMdbIdMapping results: replidServer={0}, guidServer={1}", replidServer, guidServer);
				}
			}
			finally
			{
				this.UnlockConnection();
			}
		}

		public long GetMidFromMessageEntryId(byte[] entryId)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			long result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(54761, 24, (long)this.GetHashCode(), "MapiStore.GetMidFromMessageEntryId params: entryId={0}", TraceUtils.DumpEntryId(entryId));
				}
				if (entryId == null)
				{
					throw MapiExceptionHelper.ArgumentNullException("entryId");
				}
				if (entryId.Length < 1)
				{
					throw MapiExceptionHelper.ArgumentOutOfRangeException("entryId", "Cannot be zero length");
				}
				bool flag;
				long num;
				long num2;
				int shortTermIdsFromLongTermEntryId = this.iMsgStore.GetShortTermIdsFromLongTermEntryId(entryId, out flag, out num, out num2);
				if (shortTermIdsFromLongTermEntryId != 0)
				{
					base.ThrowIfError("Unable to obtain the FID and MID from the entry id.", shortTermIdsFromLongTermEntryId);
				}
				if (!flag)
				{
					throw MapiExceptionHelper.ArgumentException("entryId", "The entry id is not a message entry id.");
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<long>(42473, 24, (long)this.GetHashCode(), "MapiStore.GetFidFromEntryId results: mid={0}", num2);
				}
				result = num2;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetFolderEntryId(string folderLegacyDN)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(11538, 24, (long)this.GetHashCode(), "MapiStore.GetFolderEntryId params: folderLegacyDN={0}", TraceUtils.MakeString(folderLegacyDN));
				}
				byte[] array;
				int num = this.iMsgStore.CreateEntryIdFromLegacyDN(folderLegacyDN, out array);
				if (num != 0)
				{
					base.ThrowIfError(string.Format("Unable to create a long-term entry ID from legacy DN '{0}'.", folderLegacyDN), num);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(15634, 24, (long)this.GetHashCode(), "MapiStore.GetFolderEntryId results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GetParentEntryId(byte[] entryId)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (entryId == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryId");
			}
			if (entryId.Length < 1)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("entryId", "Cannot be zero length");
			}
			this.LockConnection();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(8722, 24, (long)this.GetHashCode(), "MapiStore.GetParentEntryId params: entryId={0}", TraceUtils.DumpEntryId(entryId));
				}
				byte[] array = null;
				EntryIdType entryIdType = MapiStore.GetEntryIdType(entryId);
				bool flag = (entryIdType & EntryIdType.Message) == EntryIdType.Message;
				if (flag)
				{
					array = MapiStore.GetFolderEntryIdFromMessageEntryId(entryId);
				}
				else
				{
					int parentEntryId = this.iMsgStore.GetParentEntryId(entryId, out array);
					if (parentEntryId != 0)
					{
						base.ThrowIfError(string.Format("Unable to determine if entryId is for a folder '{0}'.", TraceUtils.DumpEntryId(entryId)), parentEntryId);
					}
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(12818, 24, (long)this.GetHashCode(), "MapiStore.GetParentEntryId results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public string[] GetAddressTypes()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			string[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(21202, 24, (long)this.GetHashCode(), "MapiStore.GetAddressTypes:");
				}
				string[] array;
				int addressTypes = this.iMsgStore.GetAddressTypes(out array);
				if (addressTypes != 0)
				{
					base.ThrowIfError("Unable to read server address types.", addressTypes);
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public bool BackoffNow()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			bool result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(19794, 24, (long)this.GetHashCode(), "MapiStore.BackoffNow:");
				}
				int num = 0;
				int num2 = this.iMsgStore.BackoffNow(out num);
				if (num2 != 0)
				{
					base.ThrowIfError("Unable to read backoff state.", num2);
				}
				bool flag = num != 0;
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<bool>(27986, 24, (long)this.GetHashCode(), "MapiStore.BackoffNow results: now={0}", flag);
				}
				result = flag;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] CompressEntryId(byte[] entryId)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (entryId == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryId");
			}
			if (entryId.Length < 1)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("entryId", "Cannot be zero length");
			}
			this.LockConnection();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(23890, 24, (long)this.GetHashCode(), "MapiStore.CompressEntryId params: entryId={0}", TraceUtils.DumpEntryId(entryId));
				}
				byte[] array;
				int num = this.iMsgStore.CompressEntryId(entryId, out array);
				if (num != 0)
				{
					base.ThrowIfError(string.Format("Unable to compress entryId {0}.", TraceUtils.DumpEntryId(entryId)), num);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(32082, 24, (long)this.GetHashCode(), "MapiStore.CompressEntryId results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] ExpandEntryId(byte[] entryId)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (entryId == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryId");
			}
			if (entryId.Length < 1)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("entryId", "Cannot be zero length");
			}
			this.LockConnection();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(17746, 24, (long)this.GetHashCode(), "MapiStore.ExpandEntryId params: entryId={0}", TraceUtils.DumpEntryId(entryId));
				}
				byte[] array;
				int num = this.iMsgStore.ExpandEntryId(entryId, out array);
				if (num != 0)
				{
					base.ThrowIfError(string.Format("Unable to expand compressed entryId {0}.", TraceUtils.DumpEntryId(entryId)), num);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(25938, 24, (long)this.GetHashCode(), "MapiStore.ExpandEntryId results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] GlobalIdFromId(long id)
		{
			base.CheckDisposed();
			this.LockConnection();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<long>(16440, 24, (long)this.GetHashCode(), "MapiStore.GlobalIdFromId params: id={0:X}", id);
				}
				byte[] array;
				int num = this.iMsgStore.CreateGlobalIdFromId(id, out array);
				if (num != 0)
				{
					base.ThrowIfError(string.Format("Unable to convert id {0:X}.", id), num);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(17124, 24, (long)this.GetHashCode(), "MapiStore.GlobalIdFromId results: gid={0}", TraceUtils.DumpArray(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public long IdFromGlobalId(byte[] gid)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (gid == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("gid");
			}
			if (gid.Length != MapiStore.GlobalIdLength)
			{
				throw MapiExceptionHelper.ArgumentException("gid", "Value not the right size");
			}
			this.LockConnection();
			long result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(17128, 24, (long)this.GetHashCode(), "MapiStore.IdFromGlobalId params: gid={0}", TraceUtils.DumpArray(gid));
				}
				long num2;
				int num = this.iMsgStore.CreateIdFromGlobalId(gid, out num2);
				if (num != 0)
				{
					base.ThrowIfError(string.Format("Unable to convert id {0:X}.", TraceUtils.DumpEntryId(gid)), num);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<long>(17136, 24, (long)this.GetHashCode(), "MapiStore.IdFromGlobalId results: id={0:X}", num2);
				}
				result = num2;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[] MapActionsToMDBActions(RuleAction[] actions)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(17176, 24, (long)this.GetHashCode(), "MapiStore.MapActionsToMDBActions");
				}
				byte[] array;
				int num = this.iMsgStore.MapActionsToMDBActions(actions, out array);
				if (num != 0)
				{
					base.ThrowIfError(string.Format("Unable to map actions to MDB actions.", new object[0]), num);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(17172, 24, (long)this.GetHashCode(), "MapiStore.MapActionsToMDBActions results: blob={0}", TraceUtils.DumpArray(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public void GetLocalRepIds(uint cid, out Guid replGuid, out byte[] globCount)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(65473, 24, (long)this.GetHashCode(), "MapiStore.GetLocalRepIds");
				}
				MapiLtidNative mapiLtidNative;
				int localRepIds = this.iMsgStore.GetLocalRepIds(cid, out mapiLtidNative);
				if (localRepIds != 0)
				{
					base.ThrowIfError(string.Format("Unable to get {0} local rep IDs.", cid), localRepIds);
				}
				replGuid = mapiLtidNative.replGuid;
				globCount = mapiLtidNative.globCount;
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<Guid, string>(40897, 24, (long)this.GetHashCode(), "MapiStore.GetLocalRepIds results: guid={0}, globcount={1}", mapiLtidNative.replGuid, TraceUtils.DumpArray(mapiLtidNative.globCount));
				}
			}
			finally
			{
				this.UnlockConnection();
			}
		}

		public void SetSpooler()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(39493, 24, (long)this.GetHashCode(), "MapiStore.SetSpooler");
				}
				int num = this.iMsgStore.SetSpooler();
				if (num != 0)
				{
					base.ThrowIfError("Unable to set spooler logon.", num);
				}
			}
			finally
			{
				this.UnlockConnection();
			}
		}

		public void SpoolerSetMessageLockState(byte[] entryId, MapiStore.MessageLockState messageLockState)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (entryId == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryId");
			}
			if (entryId.Length < 1)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("entryId", "Cannot be zero length");
			}
			this.LockConnection();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(39749, 24, (long)this.GetHashCode(), "MapiStore.SpoolerSetMessageLockState");
				}
				int num = this.iMsgStore.SpoolerSetMessageLockState(entryId, (int)messageLockState);
				if (num != 0)
				{
					base.ThrowIfError("Unable to set spooler message lock state.", num);
				}
			}
			finally
			{
				this.UnlockConnection();
			}
		}

		public void SpoolerNotifyMessageNewMail(byte[] entryId, string messageClass, int messageFlags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (entryId == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryId");
			}
			if (entryId.Length < 1)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("entryId", "Cannot be zero length");
			}
			if (messageClass == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("messageClass");
			}
			this.LockConnection();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(40005, 24, (long)this.GetHashCode(), "MapiStore.SpoolerNotifyMessageNewMail");
				}
				int num = this.iMsgStore.SpoolerNotifyMessageNewMail(entryId, messageClass, messageFlags);
				if (num != 0)
				{
					base.ThrowIfError("Unable to notify message new mail.", num);
				}
			}
			finally
			{
				this.UnlockConnection();
			}
		}

		public byte[] GetLocalDirectoryEntryId()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[] result;
			try
			{
				byte[] array = (byte[])base.GetProp(PropTag.LocalDirectoryEntryId).Value;
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(41149, 24, (long)this.GetHashCode(), "MapiStore.GetLocalDirectoryEntryId results: entryId={0}", TraceUtils.DumpEntryId(array));
				}
				result = array;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public int VersionMajor
		{
			get
			{
				base.CheckDisposed();
				base.BlockExternalObjectCheck();
				this.LockConnection();
				int versionMajor;
				try
				{
					versionMajor = this.exRpcConnection.VersionMajor;
				}
				finally
				{
					this.UnlockConnection();
				}
				return versionMajor;
			}
		}

		public int VersionMinor
		{
			get
			{
				base.CheckDisposed();
				base.BlockExternalObjectCheck();
				this.LockConnection();
				int versionMinor;
				try
				{
					versionMinor = this.exRpcConnection.VersionMinor;
				}
				finally
				{
					this.UnlockConnection();
				}
				return versionMinor;
			}
		}

		public int BuildMajor
		{
			get
			{
				base.CheckDisposed();
				base.BlockExternalObjectCheck();
				this.LockConnection();
				int buildMajor;
				try
				{
					buildMajor = this.exRpcConnection.BuildMajor;
				}
				finally
				{
					this.UnlockConnection();
				}
				return buildMajor;
			}
		}

		public int BuildMinor
		{
			get
			{
				base.CheckDisposed();
				base.BlockExternalObjectCheck();
				this.LockConnection();
				int buildMinor;
				try
				{
					buildMinor = this.exRpcConnection.BuildMinor;
				}
				finally
				{
					this.UnlockConnection();
				}
				return buildMinor;
			}
		}

		public bool IsVersionGreaterOrEqualThan(int[] VersionInfo)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (VersionInfo == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("VersionInfo");
			}
			if (VersionInfo.Length != 4)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("VersionInfo", "Array should have 4 elements.");
			}
			return this.VersionMajor >= VersionInfo[0] && (this.VersionMajor > VersionInfo[0] || (this.VersionMinor >= VersionInfo[1] && (this.VersionMinor > VersionInfo[1] || (this.BuildMajor >= VersionInfo[2] && (this.BuildMajor > VersionInfo[2] || this.BuildMinor >= VersionInfo[3])))));
		}

		public MapiStore OpenAlternateMailbox(string mailboxDn, OpenStoreFlag storeFlags, CultureInfo cultureInfo, out string correctServerDn, string applicationId)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string, string>(52339, 24, (long)this.GetHashCode(), "MapiStore.OpenAlternateMailbox params: mailboxDn={0}, openStoreFlags={1}, cultureInfo={2}", TraceUtils.MakeString(mailboxDn), storeFlags.ToString(), (cultureInfo == null) ? "null" : cultureInfo.DisplayName);
			}
			return this.OpenAlternateStore(mailboxDn, Guid.Empty, Guid.Empty, storeFlags & ~OpenStoreFlag.Public, cultureInfo, out correctServerDn, true, false, applicationId, null);
		}

		public MapiStore OpenAlternateMailbox(string mailboxDn, OpenStoreFlag storeFlags, CultureInfo cultureInfo, string applicationId, byte[] tenantPartitionHint)
		{
			string text = null;
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string, string>(52595, 24, (long)this.GetHashCode(), "MapiStore.OpenAlternateMailbox params: mailboxDn={0}, openStoreFlags={1}, cultureInfo={2}", TraceUtils.MakeString(mailboxDn), storeFlags.ToString(), (cultureInfo == null) ? "null" : cultureInfo.DisplayName);
			}
			return this.OpenAlternateStore(mailboxDn, Guid.Empty, Guid.Empty, storeFlags & ~OpenStoreFlag.Public, cultureInfo, out text, false, false, applicationId, tenantPartitionHint);
		}

		public MapiStore OpenAlternateMailbox(Guid guidMailbox, Guid guidMdb, OpenStoreFlag storeFlags, CultureInfo cultureInfo, out string correctServerDn, string applicationId)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace(52883, 24, (long)this.GetHashCode(), "MapiStore.OpenAlternateMailbox params: guidMailbox={0}, guidMdb={1}, openStoreFlags={2}, cultureInfo={3}", new object[]
				{
					guidMailbox.ToString(),
					guidMdb.ToString(),
					storeFlags.ToString(),
					(cultureInfo == null) ? "null" : cultureInfo.DisplayName
				});
			}
			return this.OpenAlternateStore(null, guidMailbox, guidMdb, storeFlags & ~OpenStoreFlag.Public, cultureInfo, out correctServerDn, true, false, applicationId, null);
		}

		public MapiStore OpenAlternateMailbox(Guid guidMailbox, Guid guidMdb, OpenStoreFlag storeFlags, CultureInfo cultureInfo, string applicationId, byte[] tenantPartitionHint)
		{
			string text = null;
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace(53107, 24, (long)this.GetHashCode(), "MapiStore.OpenAlternateMailbox params: guidMailbox={0}, guidMdb={1}, openStoreFlags={2}, cultureInfo={3}", new object[]
				{
					guidMailbox.ToString(),
					guidMdb.ToString(),
					storeFlags.ToString(),
					(cultureInfo == null) ? "null" : cultureInfo.DisplayName
				});
			}
			return this.OpenAlternateStore(null, guidMailbox, guidMdb, storeFlags & ~OpenStoreFlag.Public, cultureInfo, out text, false, false, applicationId, tenantPartitionHint);
		}

		public MapiStore OpenAlternatePublicStore(OpenStoreFlag storeFlags, CultureInfo cultureInfo, out string correctServerDn, string applicationId)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(53619, 24, (long)this.GetHashCode(), "MapiStore.OpenAlternatePublicStore params: openStoreFlags={0}, cultureInfo={1}", storeFlags.ToString(), (cultureInfo == null) ? "null" : cultureInfo.DisplayName);
			}
			return this.OpenAlternateStore(null, Guid.Empty, Guid.Empty, storeFlags | OpenStoreFlag.Public, cultureInfo, out correctServerDn, true, false, applicationId, null);
		}

		public MapiStore OpenAlternatePublicStore(OpenStoreFlag storeFlags, out string correctServerDn, string applicationId)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(11794, 24, (long)this.GetHashCode(), "MapiStore.OpenAlternatePublicStore params: openStoreFlags={0}", storeFlags.ToString());
			}
			return this.OpenAlternateStore(null, Guid.Empty, Guid.Empty, storeFlags | OpenStoreFlag.Public, null, out correctServerDn, true, false, applicationId, null);
		}

		public MapiStore OpenAlternatePublicStore(OpenStoreFlag storeFlags, CultureInfo cultureInfo, string applicationId)
		{
			string text = null;
			if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(53907, 24, (long)this.GetHashCode(), "MapiStore.OpenAlternatePublicStore params: openStoreFlags={0}, cultureInfo={1}", storeFlags.ToString(), (cultureInfo == null) ? "null" : cultureInfo.DisplayName);
			}
			return this.OpenAlternateStore(null, Guid.Empty, Guid.Empty, storeFlags | OpenStoreFlag.Public, cultureInfo, out text, false, false, applicationId, null);
		}

		public void ReadPerUserInformation(byte[] longtermId, bool wantIfChanged, int dataOffset, int maxDataSize, out bool hasFinished, out byte[] data)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(62707, 24, (long)this.GetHashCode(), "MapiStore.ReadPerUserInformation params: longtermId={0}, wantIfChanged={1} dataOffset={2}, maxDataSize={3}", new object[]
					{
						TraceUtils.DumpBytes(longtermId),
						wantIfChanged,
						dataOffset,
						maxDataSize
					});
				}
				int perUser = this.iMsgStore.GetPerUser(longtermId, wantIfChanged, dataOffset, maxDataSize, out data, out hasFinished);
				if (perUser != 0)
				{
					base.ThrowIfError("MapiStore.ReadPerUserInformation failed", perUser);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void WritePerUserInformation(byte[] longtermId, bool hasFinished, int dataOffset, byte[] data, Guid guidReplica)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(38131, 24, (long)this.GetHashCode(), "MapiStore.WritePerUserInformation params: longtermId={0}, hasFinished={1}, dataOffset={2}, data={3}, guidReplica={4}", new object[]
					{
						TraceUtils.DumpBytes(longtermId),
						hasFinished,
						dataOffset,
						TraceUtils.DumpBytes(data),
						guidReplica
					});
				}
				Guid? guidReplica2 = null;
				if (this.IsE15Store)
				{
					if (dataOffset == 0)
					{
						guidReplica2 = new Guid?(guidReplica);
					}
				}
				else if (guidReplica != Guid.Empty)
				{
					guidReplica2 = new Guid?(guidReplica);
				}
				int num = this.iMsgStore.SetPerUser(longtermId, guidReplica2, dataOffset, data, data.Length, hasFinished);
				if (num != 0)
				{
					base.ThrowIfError("MapiStore.WritePerUserInformation failed", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public uint GetEffectiveRights(byte[] addressBookId, byte[] entryId)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			uint result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(49759, 24, (long)this.GetHashCode(), "MapiStore.GetEffectiveRights params: addressBookId={0}, entryId={2}", TraceUtils.DumpBytes(addressBookId), TraceUtils.DumpBytes(entryId));
				}
				uint num = 0U;
				int effectiveRights = this.iMsgStore.GetEffectiveRights(addressBookId, entryId, out num);
				if (effectiveRights != 0)
				{
					base.ThrowIfError("MapiStore.GetEffectiveRights failed", effectiveRights);
				}
				result = num;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public ExRpcConnection ExRpcConnection
		{
			get
			{
				return this.exRpcConnection;
			}
		}

		public string ApplicationId
		{
			get
			{
				return this.applicationId;
			}
		}

		public Guid GetPerUserGuid(Guid replGuid, byte[] globCount)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			Guid result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(42227, 24, (long)this.GetHashCode(), "MapiStore.GetPerUserGuid");
				}
				MapiLtidNative ltid = default(MapiLtidNative);
				ltid.replGuid = replGuid;
				ltid.globCount = globCount;
				Guid guid;
				int perUserGuid = this.iMsgStore.GetPerUserGuid(ltid, out guid);
				if (perUserGuid != 0)
				{
					base.ThrowIfError("Unable to get per user guid.", perUserGuid);
				}
				result = guid;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public byte[][] GetPerUserLongTermIds(Guid guid)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			byte[][] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(58611, 24, (long)this.GetHashCode(), "MapiStore.GetPerUserLongTermIds");
				}
				MapiLtidNative[] array;
				int perUserLtids = this.iMsgStore.GetPerUserLtids(guid, out array);
				if (perUserLtids != 0)
				{
					base.ThrowIfError("Unable to get per user long term ids.", perUserLtids);
				}
				byte[][] array2 = new byte[array.Length][];
				for (int i = 0; i < array2.Length; i++)
				{
					byte[] array3 = new byte[MapiLtidNative.Size];
					ExBitConverter.Write(array[i].replGuid, array3, 0);
					Array.Copy(array[i].globCount, 0, array3, 16, array[i].globCount.Length);
					array2[i] = array3;
				}
				result = array2;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public bool GetAllPerUserLongTermIds(byte[] lastLtid, out PerUserData[] perUserDatas)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			bool result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(53596, 24, (long)this.GetHashCode(), "MapiStore.GetAllPerUserLongTermIds");
				}
				if (lastLtid == null)
				{
					lastLtid = new byte[MapiLtidNative.Size];
				}
				MapiPUDNative[] array;
				bool flag;
				int allPerUserLtids = ((SafeExMapiStoreHandle)this.iMsgStore).GetAllPerUserLtids(lastLtid, out array, out flag);
				if (allPerUserLtids != 0)
				{
					base.ThrowIfError("Unable to get per user long term ids.", allPerUserLtids);
				}
				perUserDatas = new PerUserData[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					StoreLongTermId longTermId = new StoreLongTermId(array[i].ltid.replGuid, array[i].ltid.globCount);
					perUserDatas[i] = new PerUserData(longTermId, array[i].replGuid);
				}
				result = flag;
			}
			finally
			{
				this.UnlockConnection();
			}
			return result;
		}

		public void PrereadMessages(params byte[][] entryIds)
		{
			base.CheckDisposed();
			if (entryIds == null || entryIds.GetLength(0) == 0)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryIds");
			}
			this.LockConnection();
			try
			{
				if (!this.exRpcConnection.IsWebServiceConnection)
				{
					int num = this.iMsgStore.PrereadMessages(entryIds);
					if (num != 0)
					{
						base.ThrowIfError("Preread Messages failed.", num);
					}
				}
			}
			finally
			{
				this.UnlockConnection();
			}
		}

		public PerRPCPerformanceStatistics GetStorePerRPCStats()
		{
			return this.exRpcConnection.GetStorePerRPCStats();
		}

		public void ClearStorePerRPCStats()
		{
			this.exRpcConnection.ClearStorePerRPCStats();
		}

		public RpcStatistics GetRpcStatistics()
		{
			return this.exRpcConnection.GetRpcStatistics();
		}

		public void ClearRpcStatistics()
		{
			this.exRpcConnection.ClearRpcStatistics();
		}

		public void ExecuteWithInternalAccess(Action actionDelegate)
		{
			base.CheckDisposed();
			this.LockConnection();
			try
			{
				this.exRpcConnection.ExecuteWithInternalAccess(actionDelegate);
			}
			finally
			{
				this.UnlockConnection();
			}
		}

		public void CheckForNotifications()
		{
			base.CheckDisposed();
			this.exRpcConnection.CheckForNotifications();
		}

		public bool IsDead
		{
			get
			{
				base.CheckDisposed();
				return this.exRpcConnection.IsDead;
			}
		}

		public void SetCurrentActivityInfo(Guid activityId, string component, string protocol, string action)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			this.LockConnection();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(0, 24, (long)this.GetHashCode(), "MapiStore.SetCurrentActivityInfo: ActivityId={0}, component={1}, protocol={2}, action={3}", new object[]
					{
						activityId,
						component,
						protocol,
						action
					});
				}
				int num = ((SafeExMapiStoreHandle)this.iMsgStore).SetCurrentActivityInfo(activityId, component, protocol, action);
				if (num != 0)
				{
					base.ThrowIfError("Unexpected error while setting action.", num);
				}
			}
			finally
			{
				this.UnlockConnection();
			}
		}

		public bool IsMoveDestination
		{
			get
			{
				return this.CheckInTransitStatusFlag(InTransitStatus.MoveDestination);
			}
		}

		public bool IsMoveSource
		{
			get
			{
				return this.CheckInTransitStatusFlag(InTransitStatus.MoveSource);
			}
		}

		private bool CheckInTransitStatusFlag(InTransitStatus mask)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			bool result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(24))
				{
					ComponentTrace<MapiNetTags>.Trace(0, 24, (long)this.GetHashCode(), "MapiStore.CheckInTransitStatusFlag");
				}
				uint num;
				int inTransitStatus = this.iMsgStore.GetInTransitStatus(out num);
				if (inTransitStatus != 0)
				{
					base.ThrowIfError("Unexpected error while calling GetInTransitStatus.", inTransitStatus);
				}
				result = ((num & (uint)mask) == (uint)mask);
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		private static IExRpcConnectionFactory GetDefaultExRpcConnectionFactory()
		{
			if (MapiStore.defaultExRpcConnectionFactory == null)
			{
				IExRpcConnectionFactory exRpcConnectionFactory = new ExRpcConnectionFactory(new CrossServerConnectionPolicy(CrossServerDiagnostics.Instance, ClientBehaviorOverrides.Instance));
				MapiStore.defaultExRpcConnectionFactory = exRpcConnectionFactory;
			}
			return MapiStore.defaultExRpcConnectionFactory;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MapiStore()
		{
			int[] array = new int[4];
			array[0] = 15;
			MapiStore.E15StoreVersion = array;
			MapiStore.defaultExRpcConnectionFactory = null;
			MapiStore.localServerFqdnInitialized = false;
			MapiStore.syncLock = new object();
		}

		public const byte[] DefaultTenantHint = null;

		private const string FederatedSystemAttendantLegacyDn = "*/cn=Microsoft Federated System Attendant";

		private static Guid[] IMsgStoreGuids = new Guid[]
		{
			InterfaceIds.IMsgStoreGuid
		};

		private static int GlobalIdLength = 22;

		private static readonly int[] E15StoreVersion;

		private static IExRpcConnectionFactory defaultExRpcConnectionFactory;

		private static string localServerFqdn;

		private static bool localServerFqdnInitialized;

		private static object syncLock;

		private IDisposable identityToDispose;

		private IExMapiStore iMsgStore;

		private ExRpcConnection exRpcConnection;

		private DisposableRef childObjects;

		private byte[] nonIpmSubtreeEntryId;

		private string applicationId;

		private List<MapiNotificationHandle> notificationHandles;

		private DateTime creationTime;

		private ApartmentState apartmentState;

		private int creationThreadId;

		internal enum MessageLockState
		{
			Lock,
			Unlock,
			Finished
		}
	}
}
