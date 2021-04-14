using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.MessageSecurity
{
	internal static class DirectTrust
	{
		public static void Load()
		{
			DirectTrust.reloadTimer = new Timer(new TimerCallback(DirectTrust.UpdateDirectTrustCache), null, -1, -1);
			DirectTrust.UpdateDirectTrustCache(null);
			DirectTrust.RegisterDirectTrustMonitoring();
		}

		public static void Unload()
		{
			if (DirectTrust.serverRequestCookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(DirectTrust.serverRequestCookie);
			}
			if (DirectTrust.reloadTimer != null)
			{
				DirectTrust.reloadTimer.Dispose();
			}
		}

		private static void RegisterDirectTrustMonitoring()
		{
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADObjectId childId = DirectTrust.adSession.GetOrgContainerId().GetChildId("Administrative Groups");
				DirectTrust.serverRequestCookie = ADNotificationAdapter.RegisterChangeNotification<Server>(childId, new ADNotificationCallback(DirectTrust.ServerNotificationDispatch), null);
			}, 3);
			if (adoperationResult.Exception != null)
			{
				throw adoperationResult.Exception;
			}
		}

		private static void ServerNotificationDispatch(ADNotificationEventArgs args)
		{
			try
			{
				if (Interlocked.Increment(ref DirectTrust.notificationHandlerCount) == 1)
				{
					try
					{
						lock (DirectTrust.syncObj)
						{
							if (DirectTrust.notificationProcessingScheduled)
							{
								ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Skip because notification processing already scheduled");
							}
							else
							{
								DirectTrust.reloadTimer.Change((int)DirectTrust.notificationDelay.TotalMilliseconds, -1);
								DirectTrust.notificationProcessingScheduled = true;
								ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Delayed notification processing scheduled");
							}
						}
					}
					catch (ObjectDisposedException)
					{
						DirectTrust.notificationProcessingScheduled = false;
					}
				}
			}
			finally
			{
				Interlocked.Decrement(ref DirectTrust.notificationHandlerCount);
			}
		}

		internal static SecurityIdentifier MapCertToSecurityIdentifier(X509Certificate2 certificate)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			return DirectTrust.MapCertToSecurityIdentifier(new X509Certificate2Wrapper(certificate));
		}

		internal static SecurityIdentifier MapCertToSecurityIdentifier(IX509Certificate2 certificate)
		{
			byte[] publicKey = certificate.GetPublicKey();
			MiniServer miniServer;
			if (!DirectTrust.directTrustCache.TryGetValue(publicKey, out miniServer))
			{
				ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Can't find certificate {0} in direct trust cache", certificate.Subject);
				return new SecurityIdentifier(WellKnownSidType.AnonymousSid, null);
			}
			if ((bool)miniServer[ServerSchema.IsHubTransportServer])
			{
				ExTraceGlobals.GeneralTracer.TraceDebug<string, string>(0L, "Certificate {0} maps to Bridgehead {1}", certificate.Subject, miniServer.Name);
				return WellKnownSids.HubTransportServers;
			}
			if ((bool)miniServer[ServerSchema.IsEdgeServer])
			{
				ExTraceGlobals.GeneralTracer.TraceDebug<string, string>(0L, "Certificate {0} maps to Gateway {1}", certificate.Subject, miniServer.Name);
				return WellKnownSids.EdgeTransportServers;
			}
			ExTraceGlobals.GeneralTracer.TraceError<string, string>(0L, "Certificate {0} maps to unknown server {1}. Default to anonymous", certificate.Subject, miniServer.Name);
			return new SecurityIdentifier(WellKnownSidType.AnonymousSid, null);
		}

		private static void UpdateDirectTrustCache(object state)
		{
			lock (DirectTrust.syncObj)
			{
				if (DirectTrust.isReloadingTopology)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Skip because another thread is already doing reloading the topology");
					return;
				}
				DirectTrust.isReloadingTopology = true;
			}
			try
			{
				Dictionary<byte[], MiniServer> newCache = new Dictionary<byte[], MiniServer>(ArrayComparer<byte>.Comparer);
				bool certificateValid = true;
				bool flag2 = ADNotificationAdapter.TryReadConfigurationPaged<MiniServer>(() => DirectTrust.adSession.FindAllServersWithVersionNumber(Server.E2007MinVersion, DirectTrust.hubOrEdgeRoleFilter, DirectTrust.serverProperties), delegate(MiniServer server)
				{
					byte[] array = (byte[])server[ServerSchema.InternalTransportCertificate];
					if (array != null)
					{
						try
						{
							X509Certificate2 x509Certificate = new X509Certificate2(array);
							byte[] publicKey = x509Certificate.GetPublicKey();
							if (!newCache.ContainsKey(publicKey))
							{
								ExTraceGlobals.GeneralTracer.TraceDebug<string, string>(0L, "Associate certificate {0} with server {1}, and add it to direct trust cache", x509Certificate.Subject, server.Name);
								newCache.Add(publicKey, server);
							}
							return;
						}
						catch (CryptographicException)
						{
							certificateValid = false;
							return;
						}
					}
					ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Skip server {0} because its InternalTransportCertificate is null", server.Name);
				});
				if (flag2 && certificateValid)
				{
					Dictionary<byte[], MiniServer> dictionary = Interlocked.Exchange<Dictionary<byte[], MiniServer>>(ref DirectTrust.directTrustCache, newCache);
					int num = (dictionary != null) ? (newCache.Count - dictionary.Count) : newCache.Count;
					if (num > 0)
					{
						ExTraceGlobals.GeneralTracer.TraceDebug<int>(0L, "There are {0} Exchange servers added to the cache", num);
					}
					else
					{
						ExTraceGlobals.GeneralTracer.TraceDebug<int>(0L, "There are {0} Exchange servers removed from the cache", -num);
					}
				}
				else
				{
					ExTraceGlobals.GeneralTracer.TraceError(0L, "An AD error is preventing us from loading all Edge and Hub servers");
				}
			}
			finally
			{
				lock (DirectTrust.syncObj)
				{
					DirectTrust.reloadTimer.Change((int)DirectTrust.updateInterval.TotalMilliseconds, -1);
					DirectTrust.notificationProcessingScheduled = false;
					DirectTrust.isReloadingTopology = false;
					ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Delayed notification processing reset");
				}
			}
		}

		private static readonly PropertyDefinition[] serverProperties = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			ServerSchema.InternalTransportCertificate,
			ServerSchema.CurrentServerRole
		};

		private static readonly OrFilter hubOrEdgeRoleFilter = new OrFilter(new QueryFilter[]
		{
			new BitMaskAndFilter(ServerSchema.CurrentServerRole, 32UL),
			new BitMaskAndFilter(ServerSchema.CurrentServerRole, 64UL)
		});

		private static readonly object syncObj = new object();

		private static readonly TimeSpan notificationDelay = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan updateInterval = TimeSpan.FromHours(1.0);

		private static Timer reloadTimer;

		private static int notificationHandlerCount;

		private static bool notificationProcessingScheduled;

		private static bool isReloadingTopology;

		private static readonly ITopologyConfigurationSession adSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 84, "adSession", "f:\\15.00.1497\\sources\\dev\\MessageSecurity\\src\\Common\\Core\\DirectTrust.cs");

		private static Dictionary<byte[], MiniServer> directTrustCache = new Dictionary<byte[], MiniServer>(ArrayComparer<byte>.Comparer);

		private static ADNotificationRequestCookie serverRequestCookie;
	}
}
