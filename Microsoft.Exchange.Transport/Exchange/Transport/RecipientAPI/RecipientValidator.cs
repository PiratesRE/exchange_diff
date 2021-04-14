using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EdgeSync;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal class RecipientValidator : IDisposeTrackable, IDisposable
	{
		public bool Initialized
		{
			get
			{
				return this.initialized;
			}
		}

		protected DateTime LastReload
		{
			get
			{
				return this.lastReload;
			}
			set
			{
				this.lastReload = value;
			}
		}

		protected GuardedTimer Timer
		{
			get
			{
				return this.timer;
			}
		}

		public bool RecipientDoesNotExist(RoutingAddress smtpAddress)
		{
			StringHasher stringHasher = new StringHasher();
			ulong hash = stringHasher.GetHash((string)smtpAddress);
			if (!this.initialized)
			{
				return false;
			}
			bool result;
			try
			{
				this.cacheLock.EnterReadLock();
				result = !this.recipientHashes.ContainsKey(hash);
			}
			finally
			{
				this.cacheLock.ExitReadLock();
			}
			return result;
		}

		public bool Initialize()
		{
			return this.Initialize(false);
		}

		public bool Initialize(bool suppressDisposeTracker)
		{
			Server localServer = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				if (this.configSession == null)
				{
					this.configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 231, "Initialize", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\RecipientAPI\\RecipientValidator.cs");
				}
				localServer = this.configSession.FindLocalServer();
			}, 3);
			if (!adoperationResult.Succeeded)
			{
				RecipientValidator.eventLog.LogEvent(TransportEventLogConstants.Tuple_DirectoryUnavailableLoadingValidationCache, null, new object[]
				{
					adoperationResult.Exception.Message
				});
				return false;
			}
			if (localServer.IsEdgeServer)
			{
				this.edgeRole = true;
			}
			else if (!localServer.IsHubTransportServer)
			{
				throw new InvalidOperationException("wrong location");
			}
			this.timer = new GuardedTimer(new TimerCallback(this.LoadRecipients), null, 0, -1);
			this.disposeTracker = this.GetDisposeTracker();
			if (suppressDisposeTracker)
			{
				this.SuppressDisposeTracker();
			}
			return true;
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.cacheLock != null)
				{
					this.cacheLock.Dispose();
					this.cacheLock = null;
				}
				if (this.timer != null)
				{
					this.timer.Dispose(true);
					this.timer = null;
				}
			}
			this.disposed = true;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RecipientValidator>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		protected virtual bool TryConnectToDirectory(Cookie cookie)
		{
			if (this.connection == null)
			{
				ADObjectId rootId = null;
				PooledLdapConnection sourcePooledConnection = null;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					if (this.recipientSession == null)
					{
						this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 339, "TryConnectToDirectory", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\RecipientAPI\\RecipientValidator.cs");
					}
					sourcePooledConnection = this.recipientSession.GetReadConnection(this.edgeRole ? null : cookie.DomainController, ref rootId);
				}, 3);
				if (!adoperationResult.Succeeded)
				{
					RecipientValidator.eventLog.LogEvent(TransportEventLogConstants.Tuple_DirectoryUnavailableLoadingValidationCache, null, new object[]
					{
						adoperationResult.Exception.Message
					});
					cookie.DomainController = null;
					return false;
				}
				this.connection = new Connection(sourcePooledConnection);
			}
			return true;
		}

		protected virtual bool TryInitializeCookie(bool shouldReload)
		{
			if (shouldReload)
			{
				this.cookies.Clear();
			}
			if (this.edgeRole)
			{
				if (!this.cookies.ContainsKey("OU=MSExchangeGateway"))
				{
					Cookie cookie = new Cookie("OU=MSExchangeGateway");
					this.cookies.Add(cookie.BaseDN, cookie);
				}
				return true;
			}
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADForest localForest = ADForest.GetLocalForest();
				ADCrossRef[] domainPartitions = localForest.GetDomainPartitions();
				foreach (ADCrossRef adcrossRef in domainPartitions)
				{
					string distinguishedName = adcrossRef.NCName.DistinguishedName;
					if (!this.cookies.ContainsKey(distinguishedName))
					{
						Cookie cookie2 = new Cookie(distinguishedName);
						this.cookies.Add(cookie2.BaseDN, cookie2);
					}
				}
			}, 3);
			if (!adoperationResult.Succeeded)
			{
				RecipientValidator.eventLog.LogEvent(TransportEventLogConstants.Tuple_DirectoryUnavailableLoadingValidationCache, null, new object[]
				{
					adoperationResult.Exception.Message
				});
				return false;
			}
			return true;
		}

		private void LoadRecipients(object state)
		{
			bool flag = DateTime.UtcNow - this.lastReload > Components.TransportAppConfig.RecipientValidtor.ReloadInterval;
			if (!this.TryInitializeCookie(flag))
			{
				return;
			}
			Dictionary<ulong, int> dictionary = null;
			if (flag)
			{
				dictionary = new Dictionary<ulong, int>();
			}
			bool flag2 = true;
			Dictionary<string, Cookie> dictionary2 = new Dictionary<string, Cookie>();
			DateTime utcNow = DateTime.UtcNow;
			foreach (string key in this.cookies.Keys)
			{
				Cookie cookie = this.cookies[key];
				Cookie cookie2 = null;
				if (!this.TryConnectToDirectory(cookie))
				{
					return;
				}
				if (this.LoadRecipientsOneDomain(cookie, dictionary, out cookie2))
				{
					cookie2.DomainController = this.connection.Fqdn;
					cookie2.LastUpdated = DateTime.UtcNow;
					dictionary2.Add(cookie2.BaseDN, cookie2);
				}
				else
				{
					flag2 = false;
				}
			}
			foreach (Cookie cookie3 in dictionary2.Values)
			{
				this.cookies[cookie3.BaseDN] = cookie3;
			}
			TimeSpan timeSpan = DateTime.UtcNow.Subtract(utcNow);
			if (flag && flag2)
			{
				Interlocked.Exchange<Dictionary<ulong, int>>(ref this.recipientHashes, dictionary);
				this.lastReload = DateTime.UtcNow;
				RecipientValidator.eventLog.LogEvent(TransportEventLogConstants.Tuple_RecipientValidationCacheLoaded, null, new object[]
				{
					timeSpan.ToString(),
					this.recipients.ToString(),
					this.recipientHashes.Count.ToString()
				});
				this.initialized = true;
			}
			if (this.connection != null)
			{
				this.connection.Dispose();
				this.connection = null;
			}
			this.timer.Change((int)Components.TransportAppConfig.RecipientValidtor.RefreshInterval.TotalMilliseconds, -1);
		}

		private bool LoadRecipientsOneDomain(Cookie cookie, Dictionary<ulong, int> newHashes, out Cookie retCookie)
		{
			retCookie = Cookie.Clone(cookie);
			bool flag = cookie.CookieValue == null;
			try
			{
				foreach (ExSearchResultEntry exSearchResultEntry in this.connection.DirSyncScan(retCookie, "(proxyAddresses=*)", SearchScope.Subtree, RecipientValidator.attributes))
				{
					DirectoryAttribute directoryAttribute = null;
					if (!exSearchResultEntry.IsDeleted && exSearchResultEntry.Attributes.TryGetValue("proxyAddresses", out directoryAttribute))
					{
						int i = 0;
						while (i < directoryAttribute.Count)
						{
							ProxyAddress proxyAddress = null;
							try
							{
								proxyAddress = ProxyAddress.Parse(directoryAttribute[i] as string);
							}
							catch (ArgumentException)
							{
								goto IL_11C;
							}
							goto IL_87;
							IL_11C:
							i++;
							continue;
							IL_87:
							ulong hash;
							if (proxyAddress.Prefix == RecipientValidator.smtpHashPrefix)
							{
								if (!ulong.TryParse(proxyAddress.AddressString, NumberStyles.HexNumber, null, out hash))
								{
									goto IL_11C;
								}
							}
							else
							{
								if (!(proxyAddress is SmtpProxyAddress))
								{
									goto IL_11C;
								}
								hash = RecipientValidator.stringHasher.GetHash(proxyAddress.AddressString);
							}
							if (!flag)
							{
								try
								{
									this.cacheLock.EnterWriteLock();
									if (!this.recipientHashes.ContainsKey(hash))
									{
										this.recipientHashes.Add(hash, 0);
									}
								}
								finally
								{
									this.cacheLock.ExitWriteLock();
								}
								goto IL_11C;
							}
							if (!newHashes.ContainsKey(hash))
							{
								newHashes.Add(hash, 0);
								goto IL_11C;
							}
							goto IL_11C;
						}
						this.recipients++;
					}
				}
			}
			catch (ExDirectoryException ex)
			{
				RecipientValidator.eventLog.LogEvent(TransportEventLogConstants.Tuple_DirectoryUnavailableLoadingValidationCache, null, new object[]
				{
					ex.Message
				});
				return false;
			}
			return true;
		}

		private const string AdamUserContainerNC = "OU=MSExchangeGateway";

		private const string RecipientFilter = "(proxyAddresses=*)";

		private static readonly string[] attributes = new string[]
		{
			"proxyAddresses"
		};

		private static readonly StringHasher stringHasher = new StringHasher();

		private static readonly ProxyAddressPrefix smtpHashPrefix = new CustomProxyAddressPrefix("sh");

		private static ExEventLog eventLog = new ExEventLog(new Guid("8cd349b7-795a-47f7-b99e-6f6a7fb399e1"), TransportEventLog.GetEventSource());

		private GuardedTimer timer;

		private Dictionary<ulong, int> recipientHashes = new Dictionary<ulong, int>();

		private IRecipientSession recipientSession;

		private ITopologyConfigurationSession configSession;

		private Dictionary<string, Cookie> cookies = new Dictionary<string, Cookie>();

		private bool initialized;

		private int recipients;

		private bool edgeRole;

		private Connection connection;

		private DisposeTracker disposeTracker;

		private bool disposed;

		private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

		private DateTime lastReload = DateTime.MinValue;
	}
}
