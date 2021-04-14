using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class RpcDispatch : BaseObject, IRpcDispatch, IDisposable
	{
		public static bool TryGetAuthContextInfo(ClientBinding clientBinding, out ClientSecurityContext clientSecurityContext, out bool isAnonymous, out string serverTarget, out string userDomain, out RpcHttpConnectionProperties httpConnectionProperties)
		{
			clientSecurityContext = null;
			isAnonymous = false;
			serverTarget = null;
			userDomain = string.Empty;
			httpConnectionProperties = null;
			bool result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				string text = null;
				string text2 = string.Empty;
				bool flag = false;
				RpcHttpConnectionProperties rpcHttpConnectionProperties = null;
				ClientSecurityContext clientSecurityContext2 = clientBinding.GetClientSecurityContext();
				if (clientSecurityContext2 == null)
				{
					if (!RpcHttpConnectionRegistration.Instance.TryGet(clientBinding.AssociationGuid, out clientSecurityContext2, out rpcHttpConnectionProperties))
					{
						return false;
					}
					disposeGuard.Add<ClientSecurityContext>(clientSecurityContext2);
					flag = true;
				}
				else
				{
					disposeGuard.Add<ClientSecurityContext>(clientSecurityContext2);
					ClientSecurityContext disposable = null;
					try
					{
						RpcHttpConnectionRegistration.Instance.TryGet(clientBinding.AssociationGuid, out disposable, out rpcHttpConnectionProperties);
					}
					finally
					{
						Util.DisposeIfPresent(disposable);
					}
				}
				if (rpcHttpConnectionProperties != null)
				{
					text = rpcHttpConnectionProperties.ServerTarget;
				}
				else if (!string.IsNullOrEmpty(clientBinding.MailboxId))
				{
					text = clientBinding.MailboxId;
				}
				if (!string.IsNullOrEmpty(text))
				{
					text2 = new SmtpAddress(text).Domain;
				}
				clientSecurityContext = clientSecurityContext2;
				isAnonymous = flag;
				serverTarget = text;
				userDomain = text2;
				httpConnectionProperties = rpcHttpConnectionProperties;
				disposeGuard.Success();
				result = true;
			}
			return result;
		}

		internal RpcDispatch(HandlerFactory handlerFactory, IDriverFactory driverFactory) : this(new UserManager(), handlerFactory, driverFactory, new SlaveAccountTokenMunger(), RpcDispatch.DefaultActiveUsersSweepInterval)
		{
		}

		internal RpcDispatch(UserManager userManager, HandlerFactory handlerFactory, IDriverFactory driverFactory, ITokenMunger tokenMunger, TimeSpan activeUsersSweepInterval)
		{
			this.userManager = userManager;
			this.handlerFactory = handlerFactory;
			this.driverFactory = driverFactory;
			this.tokenMunger = tokenMunger;
			ResourceHealthMonitorManager.Initialize(ResourceHealthComponent.RCA);
			ResourceLoadDelayInfo.Initialize();
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			if (snapshot.RpcClientAccess.ServerInformation.Enabled)
			{
				this.serverInfo = string.Format("BE={0}", NativeHelpers.GetLocalComputerFqdn(false));
			}
			if (activeUsersSweepInterval > TimeSpan.Zero)
			{
				this.userActivityTimer = new Timer(new TimerCallback(this.UpdateActivity), null, activeUsersSweepInterval, activeUsersSweepInterval);
			}
			try
			{
				this.serverIpAddress = Dns.GetHostAddresses(string.Empty).FirstOrDefault<IPAddress>();
			}
			catch (SocketException)
			{
			}
			finally
			{
				if (this.serverIpAddress == null)
				{
					this.serverIpAddress = IPAddress.IPv6None;
				}
			}
		}

		public int MaximumConnections
		{
			get
			{
				return this.connectionList.MaximumConnections;
			}
		}

		private static SingleDirectoryObjectCache<Server, SecurityDescriptor> ServerSecurityDescriptorCache
		{
			get
			{
				if (RpcDispatch.serverSecurityDescriptorCache == null)
				{
					SingleDirectoryObjectCache<Server, SecurityDescriptor> value = new SingleDirectoryObjectCache<Server, SecurityDescriptor>((DateTime created) => DateTime.UtcNow.Subtract(created).TotalMinutes <= 10.0, new Func<Server>(RpcDispatch.GetLocalServer), delegate(Server server)
					{
						if (server != null)
						{
							return server.ReadSecurityDescriptorBlob();
						}
						return null;
					});
					Interlocked.CompareExchange<SingleDirectoryObjectCache<Server, SecurityDescriptor>>(ref RpcDispatch.serverSecurityDescriptorCache, value, null);
				}
				return RpcDispatch.serverSecurityDescriptorCache;
			}
		}

		public void ReportBytesRead(long bytesRead, long uncompressedBytesRead)
		{
			RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.BytesRead.IncrementBy(bytesRead);
			RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.UncompressedBytesRead.IncrementBy(uncompressedBytesRead);
		}

		public void ReportBytesWritten(long bytesWritten, long uncompressedBytesWritten)
		{
			RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.BytesWritten.IncrementBy(bytesWritten);
			RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.UncompressedBytesWritten.IncrementBy(uncompressedBytesWritten);
		}

		public int Connect(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, out IntPtr contextHandle, string userAddress, int flags, int connectionModulus, int codePage, int stringLocale, int sortLocale, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, short[] clientVersion, ArraySegment<byte> auxIn, ArraySegment<byte> auxOut, out int sizeAuxOut, IStandardBudget budget)
		{
			ClientSecurityContext clientSecurityContext = null;
			int result;
			try
			{
				RpcDispatch.PerfmonEnter();
				RpcDispatch.PerfmonRpcEnter();
				Connection connection = null;
				string localDnPrefix = null;
				string localDisplayName = null;
				int newConnectionId = 0;
				string rpcServerTarget = null;
				bool isAnonymousRpcConnection = false;
				RpcHttpConnectionProperties httpConnectionProperties = null;
				contextHandle = IntPtr.Zero;
				dnPrefix = null;
				displayName = null;
				pollsMax = Configuration.ServiceConfiguration.RpcPollsMax;
				retryCount = Configuration.ServiceConfiguration.RpcRetryCount;
				retryDelay = Configuration.ServiceConfiguration.RpcRetryDelay;
				sizeAuxOut = 0;
				TimeSpan localRetryDelay = retryDelay;
				string userDomain = string.Empty;
				string userDn = string.Empty;
				OrganizationId organizationId = null;
				ConnectionFlags connectionFlags = (ConnectionFlags)flags;
				string protocolSequence = clientBinding.ProtocolSequence;
				int localSizeAuxOut = 0;
				AuxiliaryData auxiliaryData = null;
				DispatchOptions dispatchOptions = null;
				RpcErrorCode rpcErrorCode = this.ExecuteWrapper(delegate
				{
					auxiliaryData = AuxiliaryData.Parse(auxIn);
					RpcDispatch.AppendDefaultConnectAuxOutBlocks(auxiliaryData);
					dispatchOptions = new DispatchOptions(clientBinding.ProtocolSequence, auxiliaryData);
					if (!RpcDispatch.TryGetAuthContextInfo(clientBinding, out clientSecurityContext, out isAnonymousRpcConnection, out rpcServerTarget, out userDomain, out httpConnectionProperties))
					{
						throw new RpcServerException(string.Format("Unable to resolve anonymous user based on association guid: {0}", clientBinding.AssociationGuid), RpcErrorCode.AccessDenied);
					}
					string text = null;
					if (!RpcDispatch.TryGetUserDnAndOrganization(userAddress, connectionFlags, ref protocolSequence, out organizationId, out userDn, out text))
					{
						organizationId = null;
						userDn = userAddress;
					}
					else if (!string.IsNullOrEmpty(text))
					{
						userDomain = text;
					}
					newConnectionId = this.connectionList.NextConnectionId();
					return new RpcDispatch.ExecuteParameters(Activity.Create((long)((ulong)this.GetGlobalConnectionId(newConnectionId))), userDn, newConnectionId, dispatchOptions.DoNotRethrowExceptionsOnFailure)
					{
						DropConnectionOnAnyFailure = true,
						RawTraceType = 196609,
						RawInputTracer = delegate(Writer writer)
						{
							RpcDispatch.TraceRawString(writer, userDn);
							writer.WriteInt32(flags);
							writer.WriteInt32(connectionModulus);
							writer.WriteInt32(codePage);
							writer.WriteInt32(stringLocale);
							writer.WriteInt32(sortLocale);
							foreach (short value in clientVersion)
							{
								writer.WriteInt16(value);
							}
							writer.WriteSizedBytesSegment(auxIn, FieldLength.WordSize);
							RpcDispatch.TraceRawString(writer, clientBinding.ClientAddress);
							RpcDispatch.TraceRawString(writer, clientBinding.ServerAddress);
							RpcDispatch.TraceRawString(writer, protocolSequence);
							RpcDispatch.TraceRawString(writer, clientBinding.ClientEndpoint);
							writer.WriteBool(clientBinding.IsEncrypted);
							RpcDispatch.TraceRawString(writer, clientSecurityContext.UserSid.ToString());
						},
						RawOutputTracer = delegate(Writer writer)
						{
							RpcDispatch.TraceRawString(writer, localDisplayName);
							RpcDispatch.TraceRawString(writer, localDnPrefix);
							writer.WriteSizedBytesSegment(auxOut.SubSegment(0, localSizeAuxOut), FieldLength.WordSize);
						}
					};
				}, delegate
				{
					bool flag = false;
					IUser user = null;
					SecurityIdentifier securityIdentifier = null;
					Connection connection = null;
					RpcErrorCode result2;
					try
					{
						ExTraceGlobals.FaultInjectionTracer.TraceTest(3009817917U);
						if (!EnumValidator.IsValidValue<ConnectionFlags>(connectionFlags))
						{
							throw new RpcServerException(string.Format("Unknown ConnectionFlags: {0}", connectionFlags), RpcErrorCode.InvalidParam);
						}
						ClientSecurityContext clientSecurityContext;
						ProtocolLog.LogConnect(clientSecurityContext.UserSid, connectionFlags);
						MapiVersion mapiVersion = new MapiVersion((ushort)clientVersion[0], (ushort)clientVersion[1], (ushort)clientVersion[2], (ushort)clientVersion[3]);
						IPAddress ipaddress = RpcDispatch.ParseIpAddressOrDefault(clientBinding.ClientAddress, IPAddress.IPv6None);
						IPAddress ipaddress2 = RpcDispatch.ParseIpAddressOrDefault(clientBinding.ServerAddress, this.serverIpAddress);
						if (httpConnectionProperties != null)
						{
							ProtocolLog.SetHttpParameters(new string[]
							{
								httpConnectionProperties.SessionCookie
							}, httpConnectionProperties.RequestIds);
							ipaddress = RpcDispatch.ParseIpAddressOrDefault(httpConnectionProperties.ClientIp, ipaddress);
						}
						RpcDispatch.ApplyProtocolRequestInfo(protocolRequestInfo);
						if (dispatchOptions.UseRandomAdditionalRetryDelay)
						{
							localRetryDelay += RpcDispatch.ComputeRandomAdditionalRpcRetryDelay();
						}
						ProtocolLog.SetConnectionParameters(newConnectionId, userDn, mapiVersion, ipaddress, ipaddress2, clientBinding.ProtocolSequence);
						ConnectionInfo connectionInfo = new ConnectionInfo(Activity.Current, clientSecurityContext.UserSid, userDn, connectionFlags, new LocaleInfo(stringLocale, sortLocale, codePage), mapiVersion, ipaddress, ipaddress2, protocolSequence, clientBinding.IsEncrypted, organizationId, rpcServerTarget, isAnonymousRpcConnection, dispatchOptions);
						securityIdentifier = clientSecurityContext.UserSid;
						user = this.userManager.Get(securityIdentifier, userDn, userDomain);
						Activity.Current.RegisterWatsonReportDataProvider(WatsonReportActionType.Connection, user);
						RpcDispatch.UpdateActivityScopeMetadata(user, userDomain, userDn, protocolSequence);
						user.CheckCanConnect();
						if (budget == null)
						{
							budget = this.AcquireBudget(userDomain, userDn, connectionFlags, securityIdentifier);
						}
						Activity.Current.RegisterBudget(budget);
						RpcDispatch.SetOrganizationInfoForProtocolLog(organizationId, user);
						RpcDispatch.AppendOrgAndSessionInfoConnectAuxOutBlocks(auxiliaryData, user, newConnectionId);
						ClientInfo clientInfo = RpcDispatch.CreateClientInfo(connectionInfo, auxiliaryData);
						RpcDispatch.UpdateActivityScopeClientInfo(clientInfo);
						ProtocolLog.SetApplicationParameters(clientInfo.Mode, clientInfo.ProcessName);
						ProtocolLog.SetConnectionParameters(newConnectionId, user.LegacyDistinguishedName, clientInfo.Version, clientInfo.IpAddress, ipaddress2, clientBinding.ProtocolSequence);
						ProtocolLog.SetClientConnectionInfo(RpcDispatch.GetConnectionInfo(auxiliaryData.Input.OfType<ClientConnectionInfoAuxiliaryBlock>()));
						RpcErrorCode rpcErrorCode2 = RpcDispatch.ValidateConnectedClient(user, budget, clientInfo, connectionInfo);
						if (rpcErrorCode2 != RpcErrorCode.None)
						{
							result2 = rpcErrorCode2;
						}
						else
						{
							clientSecurityContext = null;
							if (this.TryMungeClientSecurityContext(clientSecurityContext, user, out clientSecurityContext))
							{
								Util.DisposeIfPresent(clientSecurityContext);
								clientSecurityContext = clientSecurityContext;
								clientSecurityContext = null;
							}
							if ((connectionFlags & ConnectionFlags.UseAdminPrivilege) == ConnectionFlags.UseAdminPrivilege)
							{
								Feature.Stubbed(194134, "In case of UseAdminPrivilege we need to validate connected user has admin permissions on mdb");
							}
							if ((connectionFlags & ConnectionFlags.UseDelegatedAuthPrivilege) == ConnectionFlags.UseDelegatedAuthPrivilege)
							{
								if (user.IsFederatedSystemAttendant)
								{
									throw new RpcServerException("The connected XROP user is not Authorized for Delegated Auth", RpcErrorCode.NotAuthorized);
								}
								if (!this.CanDoConstrainedDelegation(clientSecurityContext))
								{
									throw new RpcServerException("The connected user is not Authorized for Delegated Auth", RpcErrorCode.NotAuthorized);
								}
							}
							if ((connectionFlags & (ConnectionFlags.UseAdminPrivilege | ConnectionFlags.UseDelegatedAuthPrivilege)) == ConnectionFlags.None && !user.IsFederatedSystemAttendant)
							{
								user.CorrelateIdentityWithLegacyDN(clientSecurityContext);
							}
							connection = new Connection(newConnectionId, connectionInfo, clientInfo, user, budget, clientSecurityContext, this.handlerFactory, this.driverFactory);
							auxiliaryData.ReportClientPerformance(this.rpcFailureProcessor, new Predicate<AuxiliaryBlock>(RpcDispatch.IsBlockTrustworthy));
							connection = connection;
							clientSecurityContext = null;
							budget = null;
							if (connection.DispatchOptions.AppendMonitoringAuxiliaryBlock)
							{
								RpcDispatch.AppendMonitoringActivityAuxiliaryBlockAndSerialize(auxiliaryData);
							}
							if (this.serverInfo != null)
							{
								RpcDispatch.AppendServerInformationAuxiliaryBlockAndSerialize(auxiliaryData, this.serverInfo);
							}
							this.AppendIdentityCorrelation(auxiliaryData, (user.OrganizationId == null) ? connectionInfo.OrganizationId : user.OrganizationId);
							localSizeAuxOut = auxiliaryData.Serialize(auxOut).Count;
							this.connectionList.AddConnection(connection);
							connection = null;
							localDisplayName = connection.User.DisplayName;
							localDnPrefix = string.Empty;
							flag = true;
							result2 = RpcErrorCode.None;
						}
					}
					finally
					{
						if (user != null)
						{
							user.Release();
						}
						ClientSecurityContext clientSecurityContext;
						if (!flag)
						{
							if (connection != null)
							{
								connection.UpdateBudgetBalance();
							}
							ProtocolLog.SetActivityData();
							ExTraceGlobals.ConnectRpcTracer.TraceError<string, SecurityIdentifier>(0, Activity.TraceId, "RpcDispatch.Connect failed for user '{0}'. SID: {1}. See FailedRpc tag.", userDn, securityIdentifier);
							Util.DisposeIfPresent(connection);
							Util.DisposeIfPresent(budget);
							Util.DisposeIfPresent(clientSecurityContext);
						}
						clientSecurityContext = null;
					}
					return result2;
				}, delegate(Exception ex)
				{
					if (connection != null && connection.DispatchOptions.AppendMonitoringAuxiliaryBlock)
					{
						RpcDispatch.AppendMonitoringActivityAuxiliaryBlockAndSerialize(auxiliaryData);
					}
					if (this.serverInfo != null)
					{
						RpcDispatch.AppendServerInformationAuxiliaryBlockAndSerialize(auxiliaryData, this.serverInfo);
					}
					localSizeAuxOut = RpcDispatch.AppendExceptionTraceAuxBlockAndSerialize(ex, auxiliaryData, auxOut).Count;
				});
				contextHandle = new IntPtr(Connection.GetConnectionId(connection));
				dnPrefix = localDnPrefix;
				displayName = localDisplayName;
				sizeAuxOut = localSizeAuxOut;
				retryDelay = localRetryDelay;
				result = (int)rpcErrorCode;
			}
			finally
			{
				Util.DisposeIfPresent(clientSecurityContext);
				RpcDispatch.PerfmonRpcLeave();
				RpcDispatch.PerfmonLeave();
			}
			return result;
		}

		public int Disconnect(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, bool rundown)
		{
			int result;
			try
			{
				RpcDispatch.PerfmonEnter();
				RpcDispatch.PerfmonRpcEnter();
				int connectionId = contextHandle.ToInt32();
				Connection connection = null;
				RpcErrorCode rpcErrorCode = RpcErrorCode.None;
				try
				{
					rpcErrorCode = this.ExecuteWrapper(delegate
					{
						ExTraceGlobals.FaultInjectionTracer.TraceTest(3144035645U);
						string userLegacyDN = null;
						if (this.connectionList.TryGetConnection(connectionId, out connection))
						{
							userLegacyDN = connection.ActAsLegacyDN;
						}
						RpcDispatch.ExecuteParameters result2 = new RpcDispatch.ExecuteParameters((connection != null) ? connection.Activity : null, userLegacyDN, connectionId, connection);
						result2.RawTraceType = 196610;
						result2.RawOutputTracer = delegate(Writer writer)
						{
						};
						return result2;
					}, delegate
					{
						RpcDispatch.ApplyProtocolRequestInfo(protocolRequestInfo);
						if (connection != null)
						{
							connection.UpdateBudgetBalance();
							this.MarkForRemoval(connection, rundown ? DisconnectReason.NetworkRundown : DisconnectReason.ClientDisconnect);
						}
						return RpcErrorCode.None;
					}, null);
				}
				catch (ServerUnavailableException)
				{
					rpcErrorCode = RpcErrorCode.None;
				}
				contextHandle = IntPtr.Zero;
				result = (int)rpcErrorCode;
			}
			finally
			{
				RpcDispatch.PerfmonRpcLeave();
				RpcDispatch.PerfmonLeave();
			}
			return result;
		}

		public int Execute(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, IList<ArraySegment<byte>> ropInArray, ArraySegment<byte> ropOut, out int sizeRopOut, ArraySegment<byte> auxIn, ArraySegment<byte> auxOut, out int sizeAuxOut, bool isFake, out byte[] fakeOut)
		{
			RpcDispatch.PerfmonEnter();
			RpcDispatch.PerfmonRpcEnter();
			Connection connection = null;
			ExDateTime utcNow = ExDateTime.UtcNow;
			int result;
			try
			{
				int localSizeRopOut = 0;
				int localSizeAuxOut = 0;
				byte[] localFakeOut = null;
				sizeRopOut = 0;
				sizeAuxOut = 0;
				int localConnectionId = contextHandle.ToInt32();
				AuxiliaryData auxiliaryData = null;
				RpcErrorCode rpcErrorCode = this.ExecuteWrapper(delegate
				{
					auxiliaryData = AuxiliaryData.Parse(auxIn);
					connection = this.connectionList.GetConnection(localConnectionId);
					connection.BeginServerPerformanceCounting();
					return new RpcDispatch.ExecuteParameters(connection.Activity, connection.ActAsLegacyDN, localConnectionId, connection)
					{
						ConsiderAsUserActivity = true,
						RawTraceType = 196611,
						MultiRawInputTracer = RpcDispatch.ExecuteTracer(ropInArray, auxIn, isFake),
						RawOutputTracer = delegate(Writer writer)
						{
							writer.WriteSizedBytesSegment(ropOut.SubSegment(0, localSizeRopOut), FieldLength.WordSize);
							writer.WriteSizedBytesSegment(auxOut.SubSegment(0, localSizeAuxOut), FieldLength.WordSize);
							writer.WriteBool(localFakeOut != null);
						}
					};
				}, delegate
				{
					RpcDispatch.ApplyProtocolRequestInfo(protocolRequestInfo);
					OverBudgetException ex = null;
					if (Activity.Current.Budget.TryCheckOverBudget(out ex))
					{
						ProtocolLog.LogThrottlingOverBudget(ex.PolicyPart, ex.BackoffTime);
						ProtocolLog.LogThrottlingSnapshot(Activity.Current.Budget);
						throw new ClientBackoffException("Client is over budget", ex);
					}
					ResourceUnhealthyException ex2 = null;
					if (ResourceLoadDelayInfo.TryCheckResourceHealth(Activity.Current.Budget, RpcDispatch.DefaultWorkloadSettings, RpcDispatch.LocalCPUResourceKeyOnly, out ex2))
					{
						ProtocolLog.LogCriticalResourceHealth(ex2);
						throw new ClientBackoffException(ex2.Message, ex2);
					}
					Activity.Current.Budget.StartLocal("RpcDispatch.Execute", default(TimeSpan));
					RpcErrorCode result2;
					try
					{
						auxiliaryData.ReportClientPerformance(this.perfCounterProcessor, new Predicate<AuxiliaryBlock>(RpcDispatch.IsBlockTrustworthy));
						auxiliaryData.ReportClientPerformance(this.rpcProcessingTimeProcessor, new Predicate<AuxiliaryBlock>(RpcDispatch.IsBlockTrustworthy));
						auxiliaryData.ReportClientPerformance(this.rpcFailureProcessor, new Predicate<AuxiliaryBlock>(RpcDispatch.IsBlockTrustworthy));
						connection.RopDriver.Execute(ropInArray, ropOut, out localSizeRopOut, auxiliaryData, isFake, out localFakeOut);
						if (connection.DispatchOptions.AppendMonitoringAuxiliaryBlock)
						{
							RpcDispatch.AppendMonitoringActivityAuxiliaryBlockAndSerialize(auxiliaryData);
						}
						localSizeAuxOut = auxiliaryData.Serialize(auxOut).Count;
						ProtocolLog.LogThrottlingSnapshot(Activity.Current.Budget);
						result2 = RpcErrorCode.None;
					}
					finally
					{
						Activity.Current.Budget.EndLocal();
						if (connection.DispatchOptions.EnforceMicrodelays)
						{
							DelayEnforcementResults delayinfo = ResourceLoadDelayInfo.EnforceDelay(Activity.Current.Budget, RpcDispatch.DefaultWorkloadSettings, RpcDispatch.LocalCPUResourceKeyOnly, TimeSpan.MaxValue, null);
							ProtocolLog.LogMicroDelay(delayinfo);
						}
					}
					return result2;
				}, delegate(Exception ex)
				{
					if (connection != null && connection.DispatchOptions.AppendMonitoringAuxiliaryBlock)
					{
						RpcDispatch.AppendMonitoringActivityAuxiliaryBlockAndSerialize(auxiliaryData);
					}
					localSizeAuxOut = RpcDispatch.AppendExceptionTraceAuxBlockAndSerialize(ex, auxiliaryData, auxOut).Count;
				});
				contextHandle = new IntPtr(Connection.GetConnectionId(connection));
				if (rpcErrorCode == RpcErrorCode.None)
				{
					sizeRopOut = localSizeRopOut;
				}
				sizeAuxOut = localSizeAuxOut;
				fakeOut = localFakeOut;
				result = (int)rpcErrorCode;
			}
			finally
			{
				if (connection != null)
				{
					connection.EndServerPerformanceCounting();
				}
				this.UpdateRpcLatencyCounters((double)new TimeSpan(ExDateTime.UtcNow.UtcTicks - utcNow.UtcTicks).Milliseconds);
				RpcDispatch.PerfmonRpcLeave();
				RpcDispatch.PerfmonLeave();
			}
			return result;
		}

		public int NotificationConnect(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, out IntPtr asynchronousContextHandle)
		{
			Connection connection = null;
			RpcErrorCode rpcErrorCode = RpcErrorCode.None;
			asynchronousContextHandle = IntPtr.Zero;
			try
			{
				RpcDispatch.PerfmonEnter();
				RpcDispatch.PerfmonRpcEnter();
				int localConnectionId = contextHandle.ToInt32();
				IntPtr localAsynchronousContextHandle = IntPtr.Zero;
				rpcErrorCode = this.ExecuteWrapper(delegate
				{
					connection = this.connectionList.GetConnection(localConnectionId);
					return new RpcDispatch.ExecuteParameters(connection.Activity, connection.ActAsLegacyDN, localConnectionId, connection);
				}, delegate
				{
					ExTraceGlobals.ConnectRpcTracer.TraceDebug<Connection>(0, Activity.TraceId, "RpcDispatch.NotificationConnect. Connection = {0}", connection);
					RpcDispatch.ApplyProtocolRequestInfo(protocolRequestInfo);
					localAsynchronousContextHandle = new IntPtr(ConnectionList.GetAsyncConnectionHandle(localConnectionId));
					ProtocolLog.LogData<IntPtr>(false, "RpcDispatch.NotificationConnect(out:{0})", localAsynchronousContextHandle);
					return RpcErrorCode.None;
				}, null);
				if (rpcErrorCode == RpcErrorCode.None)
				{
					asynchronousContextHandle = localAsynchronousContextHandle;
				}
			}
			finally
			{
				RpcDispatch.PerfmonRpcLeave();
				RpcDispatch.PerfmonLeave();
			}
			return (int)rpcErrorCode;
		}

		public int Dummy(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding)
		{
			return 0;
		}

		public void NotificationWait(ProtocolRequestInfo protocolRequestInfo, IntPtr asynchronousContextHandle, uint ulFlagsIn, Action<bool, int> completion)
		{
			try
			{
				RpcDispatch.PerfmonEnter();
				RpcDispatch.PerfmonRpcEnter();
				int localConnectionId = ConnectionList.GetSyncConnectionHandle(asynchronousContextHandle.ToInt32());
				Connection connection = null;
				bool flag = false;
				bool flag2 = false;
				RpcErrorCode rpcErrorCode = RpcErrorCode.None;
				try
				{
					rpcErrorCode = this.ExecuteWrapper(delegate
					{
						connection = this.connectionList.GetConnection(localConnectionId);
						return new RpcDispatch.ExecuteParameters(connection.Activity, connection.ActAsLegacyDN, localConnectionId, connection);
					}, delegate
					{
						RpcDispatch.ApplyProtocolRequestInfo(protocolRequestInfo);
						ProtocolLog.LogData<IntPtr, uint>(false, "RpcDispatch.NotificationWait(handle={0} flags={1})", asynchronousContextHandle, ulFlagsIn);
						if (connection.CompletionActionAssigned)
						{
							ExTraceGlobals.AsyncRpcTracer.TraceDebug<Connection>(0, Activity.TraceId, "RpcDispatch.NotificationWait: rejecting a duplicate completion callback. Connection = {0}", connection);
							completion(false, 2030);
							completion = null;
						}
						else if (connection.Handler.NotificationHandler.HasPendingNotifications())
						{
							ExTraceGlobals.AsyncRpcTracer.TraceDebug<Connection>(0, Activity.TraceId, "RpcDispatch.NotificationWait: notifications pending, completing right away. Connection = {0}", connection);
							completion(true, 0);
							completion = null;
						}
						else
						{
							ExTraceGlobals.AsyncRpcTracer.TraceDebug<Connection>(0, Activity.TraceId, "RpcDispatch.NotificationWait: parked. Connection = {0}", connection);
							connection.CompletionAction = completion;
							completion = null;
							connection.Handler.NotificationHandler.RegisterCallback(delegate
							{
								connection.CompleteAction("new events", true, RpcErrorCode.None);
							});
						}
						return RpcErrorCode.None;
					}, null);
				}
				catch (ServerUnavailableException)
				{
					rpcErrorCode = RpcErrorCode.None;
					flag = true;
					flag2 = true;
				}
				if (rpcErrorCode != RpcErrorCode.None || flag2)
				{
					if (connection != null)
					{
						connection.CompleteAction("error parking a call", flag, rpcErrorCode);
					}
					if (completion != null)
					{
						ExTraceGlobals.AsyncRpcTracer.TraceDebug<Connection>(Activity.TraceId, "RpcDispatch.NotificationWait. Complete action which is not saved on this connection. Connection = {0}", connection);
						completion(flag, (int)rpcErrorCode);
					}
				}
			}
			finally
			{
				RpcDispatch.PerfmonRpcLeave();
				RpcDispatch.PerfmonLeave();
			}
		}

		public void DroppedConnection(IntPtr asynchronousContextHandle)
		{
			try
			{
				this.disposeLock.EnterReadLock();
				if (!this.isShuttingDown && !RpcClientAccessService.IsShuttingDown)
				{
					base.CheckDisposed();
					int syncConnectionHandle = ConnectionList.GetSyncConnectionHandle(asynchronousContextHandle.ToInt32());
					ExTraceGlobals.AsyncRpcTracer.TraceDebug<int>((long)syncConnectionHandle, "DroppedConnection. localConnectionId = {0}.", syncConnectionHandle);
					Connection connection = null;
					if (this.connectionList.TryGetConnection(syncConnectionHandle, out connection))
					{
						connection.CompleteAction("async connection dropped", false, RpcErrorCode.None);
					}
					else
					{
						ExTraceGlobals.AsyncRpcTracer.TraceDebug<int>((long)syncConnectionHandle, "DroppedConnection. Cannot find the connection. localConnectionId = {0}.", syncConnectionHandle);
					}
				}
			}
			finally
			{
				try
				{
					this.disposeLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		internal static ArraySegment<byte> BuildDefaultConnectAuxOutBuffer()
		{
			ArraySegment<byte> arraySegment = new ArraySegment<byte>(new byte[4096]);
			int count;
			AuxiliaryData.SerializeAuxiliaryBlocks(Configuration.DefaultEcDoConnectExAuxOutBlocks, arraySegment, out count);
			return arraySegment.SubSegment(0, count);
		}

		internal static void UpdateRpcLatencyCounters(ref double result, double currentValue)
		{
			double num;
			double value;
			do
			{
				num = result;
				value = (num * 1024.0 - num + currentValue) / 1024.0;
			}
			while (Interlocked.CompareExchange(ref result, value, num) != num);
		}

		internal void UpdateActivity(object stateInfo)
		{
			RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ActiveUserCount.RawValue = (long)this.userManager.GetActiveUserCount();
		}

		protected override void InternalDispose()
		{
			try
			{
				this.disposeLock.EnterWriteLock();
				this.isShuttingDown = true;
				Util.DisposeIfPresent(this.userActivityTimer);
				Util.DisposeIfPresent(this.connectionList);
				Util.DisposeIfPresent(this.userManager);
				base.InternalDispose();
			}
			finally
			{
				try
				{
					this.disposeLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RpcDispatch>(this);
		}

		private static void LogErrors(RpcErrorCode? storeError, Exception exception)
		{
			if (exception is RpcServiceException)
			{
				ProtocolLog.LogRpcException((RpcServiceException)exception);
				return;
			}
			if (storeError != null)
			{
				ProtocolLog.LogRpcFailure(RpcDispatch.IsInterestingForProtocolLogging(storeError.Value), storeError.Value, exception);
				return;
			}
			ExTraceGlobals.FailedRpcTracer.TraceDebug(0, Activity.TraceId, "Unhandled exception. See subsequent traces or Watson.");
		}

		private static void ApplyProtocolRequestInfo(ProtocolRequestInfo protocolRequestInfo)
		{
			if (protocolRequestInfo != null)
			{
				ProtocolLog.SetHttpParameters(protocolRequestInfo.Cookies, protocolRequestInfo.RequestIds);
				string clientAddress = protocolRequestInfo.ClientAddress;
				if (!string.IsNullOrEmpty(clientAddress))
				{
					IPAddress clientIpAddress = RpcDispatch.ParseIpAddressOrDefault(clientAddress, IPAddress.IPv6None);
					ProtocolLog.SetClientIpAddress(clientIpAddress);
				}
			}
		}

		private static bool TryStartUsingConnection(Connection connection)
		{
			return connection.TryIncrementUsageCount();
		}

		private static bool ShouldBlockInsufficientVersion(MiniRecipient recipient)
		{
			if (Configuration.ServiceConfiguration.EnableBlockInsufficientClientVersions)
			{
				return true;
			}
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(recipient.GetContext(null), null, null);
			return snapshot.RpcClientAccess.BlockInsufficientClientVersions.Enabled;
		}

		private static void PerfmonEnter()
		{
			RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.Requests.Increment();
		}

		private static void PerfmonLeave()
		{
			RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.Requests.Decrement();
		}

		private static void PerfmonRpcEnter()
		{
			RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.PacketsRate.Increment();
		}

		private static void PerfmonRpcLeave()
		{
		}

		private static void UpdateActivityScopeMetadata(IUser user, string userDomain, string userDn, string protocolSequence)
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null)
			{
				if (!string.IsNullOrEmpty(userDomain))
				{
					currentActivityScope.TenantId = userDomain;
				}
				currentActivityScope.Protocol = RpcProtocolSequence.ToDisplayString(protocolSequence);
				try
				{
					ExchangePrincipal exchangePrincipal = user.GetExchangePrincipal(userDn);
					if (exchangePrincipal != null)
					{
						currentActivityScope.UserEmail = exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
						if (string.IsNullOrEmpty(userDomain) && !string.IsNullOrEmpty(exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()))
						{
							currentActivityScope.TenantId = exchangePrincipal.MailboxInfo.PrimarySmtpAddress.Domain;
						}
						currentActivityScope.UserId = string.Format("ADGuid:{0}", exchangePrincipal.ObjectId.ObjectGuid.ToString());
						currentActivityScope.Puid = ((user.MiniRecipient.NetID != null) ? user.MiniRecipient.NetID.ToString() : string.Empty);
					}
				}
				catch (UserHasNoMailboxException)
				{
				}
				currentActivityScope.Component = "RCA/Mailbox";
			}
		}

		private static void UpdateActivityScopeClientInfo(ClientInfo clientInfo)
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null)
			{
				currentActivityScope.ClientInfo = string.Format("{0}/{1}", clientInfo.ProcessName ?? string.Empty, clientInfo.Version.ToString());
			}
		}

		private static string GetConnectionInfo(IEnumerable<ClientConnectionInfoAuxiliaryBlock> auxiliaryBlocks)
		{
			if (!auxiliaryBlocks.IsEmpty<ClientConnectionInfoAuxiliaryBlock>())
			{
				ClientConnectionInfoAuxiliaryBlock clientConnectionInfoAuxiliaryBlock = auxiliaryBlocks.First<ClientConnectionInfoAuxiliaryBlock>();
				return string.Format("Connection Info[GUID:{0}, Attempts:{1}, Flags:{2}, Ctx:{3}]", new object[]
				{
					clientConnectionInfoAuxiliaryBlock.ConnectionGuid,
					clientConnectionInfoAuxiliaryBlock.ConnectionAttempts,
					clientConnectionInfoAuxiliaryBlock.ConnectionFlags,
					clientConnectionInfoAuxiliaryBlock.ConnectionContextInfo
				});
			}
			return string.Empty;
		}

		private static Server GetLocalServer()
		{
			ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1708, "GetLocalServer", "f:\\15.00.1497\\sources\\dev\\mapimt\\src\\Server\\RpcDispatch.cs");
			return topologyConfigurationSession.FindLocalServer();
		}

		private static bool IsInterestingForProtocolLogging(RpcErrorCode errorCode)
		{
			if (errorCode <= RpcErrorCode.NoRpcInterface)
			{
				if (errorCode != RpcErrorCode.None && errorCode != RpcErrorCode.NoRpcInterface)
				{
					return true;
				}
			}
			else if (errorCode != RpcErrorCode.PartialCompletion && errorCode != RpcErrorCode.SyncClientChangeNewer)
			{
				return true;
			}
			return false;
		}

		private static void TraceRawString(Writer writer, string stringToWrite)
		{
			writer.WriteString8(stringToWrite ?? string.Empty, Encoding.UTF8, StringFlags.IncludeNull);
		}

		private static ClientInfo CreateClientInfo(ConnectionInfo connectionInfo, AuxiliaryData auxiliaryData)
		{
			string machineName = null;
			string processName = null;
			ClientMode mode = ClientMode.Unknown;
			byte[] clientSessionInfo = null;
			foreach (AuxiliaryBlock auxiliaryBlock in auxiliaryData.Input)
			{
				PerfClientInfoAuxiliaryBlock perfClientInfoAuxiliaryBlock = auxiliaryBlock as PerfClientInfoAuxiliaryBlock;
				if (perfClientInfoAuxiliaryBlock != null)
				{
					machineName = perfClientInfoAuxiliaryBlock.MachineName;
					mode = perfClientInfoAuxiliaryBlock.ClientMode;
				}
				else
				{
					PerfProcessInfoAuxiliaryBlock perfProcessInfoAuxiliaryBlock = auxiliaryBlock as PerfProcessInfoAuxiliaryBlock;
					if (perfProcessInfoAuxiliaryBlock != null)
					{
						processName = perfProcessInfoAuxiliaryBlock.ProcessName;
					}
					else
					{
						ClientSessionInfoAuxiliaryBlock clientSessionInfoAuxiliaryBlock = auxiliaryBlock as ClientSessionInfoAuxiliaryBlock;
						if (clientSessionInfoAuxiliaryBlock != null)
						{
							if (connectionInfo.ProtocolSequence != "xrop")
							{
								throw new InvalidParameterException("ClientSessionInfo is only allowed in web service connection.");
							}
							clientSessionInfo = clientSessionInfoAuxiliaryBlock.InfoBlob;
						}
					}
				}
			}
			return new ClientInfo(connectionInfo.ClientIpAddress, machineName, processName, connectionInfo.ClientVersion, mode, clientSessionInfo);
		}

		private static void AppendDefaultConnectAuxOutBlocks(AuxiliaryData auxiliaryData)
		{
			auxiliaryData.AppendOutput(Configuration.DefaultEcDoConnectExAuxOutBlocks);
		}

		private static void AppendOrgAndSessionInfoConnectAuxOutBlocks(AuxiliaryData auxiliaryData, IUser user, int connectionId)
		{
			auxiliaryData.AppendOutput(new ExOrgInfoAuxiliaryBlock(user.ExchangeOrganizationInfo));
			auxiliaryData.AppendOutput(new ServerSessionInfoAuxiliaryBlock(RpcDispatch.GetSessionInfo(connectionId)));
			auxiliaryData.AppendOutput(new ClientControlAuxiliaryBlock(ClientControlFlags.EnablePerfSendToServer | ClientControlFlags.EnableCompression | ClientControlFlags.EnableHttpTunneling, TimeSpan.FromDays(7.0)));
		}

		private static string GetSessionInfo(int connectionId)
		{
			return string.Format("ClientAccessServer={0},ConnectTime={1},ConnectionID={2}", Configuration.ServiceConfiguration.ThisServerFqdn, ExDateTime.Now, connectionId);
		}

		private static RpcErrorCode ValidateConnectedClient(IUser user, IStandardBudget budget, ClientInfo clientInfo, ConnectionInfo connectionInfo)
		{
			OverBudgetException ex = null;
			if (budget.TryCheckOverBudget(out ex))
			{
				ProtocolLog.LogThrottlingOverBudget(ex.PolicyPart, ex.BackoffTime);
				ProtocolLog.LogThrottlingSnapshot(budget);
				throw new ClientBackoffException("Client is over budget", ex);
			}
			try
			{
				budget.StartConnection("RpcDispatch.ValidateConnectedClient");
			}
			catch (OverBudgetException)
			{
				ProtocolLog.LogThrottlingConnectionLimitHit();
				ProtocolLog.LogThrottlingSnapshot(budget);
				return RpcErrorCode.MaxConnectionsExceeded;
			}
			ConnectionFlags connectionFlags = ConnectionFlags.UseAdminPrivilege | ConnectionFlags.UseDelegatedAuthPrivilege | ConnectionFlags.IgnoreNoPublicFolders | ConnectionFlags.RemoteSystemService;
			if ((connectionInfo.ConnectionFlags & ~(connectionFlags != ConnectionFlags.None)) != ConnectionFlags.None)
			{
				return RpcErrorCode.NotSupported;
			}
			ServiceConfiguration serviceConfiguration = Configuration.ServiceConfiguration;
			if (clientInfo.Mode != ClientMode.ExchangeServer)
			{
				if (!serviceConfiguration.IsClientVersionAllowed(clientInfo.Version))
				{
					throw new RpcServerException(string.Format("Client version is not allowed: {0} based on the service-wide settings.", clientInfo.Version), RpcErrorCode.ClientVerDisallowed);
				}
				if (RpcDispatch.ShouldBlockInsufficientVersion(user.MiniRecipient) && !serviceConfiguration.IsClientVersionAllowedInForest(clientInfo.Version))
				{
					throw new RpcServerException(string.Format("Client version is not allowed: {0} based on the forest-wide settings.", clientInfo.Version), RpcErrorCode.ClientVerDisallowed);
				}
				if (user.MapiBlockOutlookVersions.IsIncluded(clientInfo.Version))
				{
					throw new RpcServerException(string.Format("Client version is not allowed: {0} based on the per-mailbox settings.", clientInfo.Version), RpcErrorCode.ClientVerDisallowed);
				}
				if (user.MapiBlockOutlookRpcHttp && connectionInfo.ProtocolSequence == NetworkProtocol.Http.ProtocolName)
				{
					throw new RpcServerException(string.Format("{0} is not allowed for this user.", NetworkProtocol.Http.ProtocolName), RpcErrorCode.RpcHttpDisallowed);
				}
				if (!user.MapiEnabled)
				{
					throw new RpcServerException("MAPI is not enabled for this user.", RpcErrorCode.ProtocolDisabled);
				}
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).RpcClientAccess.RpcHttpClientAccessRulesEnabled.Enabled)
				{
					OrganizationId organizationId = (user.OrganizationId == null) ? connectionInfo.OrganizationId : user.OrganizationId;
					if (organizationId == null)
					{
						organizationId = OrganizationId.ForestWideOrgId;
						ExTraceGlobals.ConnectRpcTracer.TraceDebug(0, Activity.TraceId, "[RpcDispatch::ValidateConnectedClient] Organization ID is null, assuming ForestWideOrgId.");
					}
					bool flag = ClientAccessRulesUtils.ShouldBlockConnection(organizationId, ClientAccessRulesUtils.GetUsernameFromIdInformation(user.MiniRecipient.WindowsLiveID, user.MiniRecipient.MasterAccountSid, user.MiniRecipient.Sid, user.MiniRecipient.Id), ClientAccessProtocol.OutlookAnywhere, new IPEndPoint(connectionInfo.ClientIpAddress, 0), ClientAccessAuthenticationMethod.BasicAuthentication, user.MiniRecipient, delegate(ClientAccessRulesEvaluationContext context)
					{
						ProtocolLog.LogWarning("Blocked by Client Access Rules ({0}:{1})", new object[]
						{
							ClientAccessRulesConstants.ClientAccessRuleName,
							context.CurrentRule.Name
						});
						ExTraceGlobals.ConnectRpcTracer.TraceDebug<string, string>(0, Activity.TraceId, "[RpcDispatch::ValidateConnectedClient] {0} = {1}.", ClientAccessRulesConstants.ClientAccessRuleName, context.CurrentRule.Name);
					}, delegate(double latency)
					{
						if (latency > 50.0)
						{
							ProtocolLog.LogWarning("Client Access Rules High Latency ({0}:{1})", new object[]
							{
								ClientAccessRulesConstants.ClientAccessRulesLatency,
								latency.ToString()
							});
							ExTraceGlobals.ConnectRpcTracer.TraceDebug<string, string>(0, Activity.TraceId, "[RpcDispatch::ValidateConnectedClient] {0} = {1}.", ClientAccessRulesConstants.ClientAccessRulesLatency, latency.ToString());
						}
					});
					if (flag)
					{
						throw new RpcServerException("Connection blocked by Client Access Rules.", RpcErrorCode.ProtocolDisabled);
					}
				}
				if (!connectionInfo.IsAnonymous && serviceConfiguration.IsEncryptionRequired && !connectionInfo.IsEncrypted)
				{
					throw new RpcServerException("Encrypted connection is required.", RpcErrorCode.NotEncrypted);
				}
				if (user.MapiCachedModeRequired && clientInfo.Mode != ClientMode.Cached)
				{
					throw new RpcServerException("Client isn't allowed to connect with non-cached mode", RpcErrorCode.CachedModeRequired);
				}
				if (!user.ExchangeOrganizationInfo.HasFlag(ExOrgInfoFlags.PublicFoldersEnabled) && (connectionInfo.ConnectionFlags & ConnectionFlags.IgnoreNoPublicFolders) == ConnectionFlags.None && clientInfo.Version < MapiVersion.Outlook12)
				{
					throw new RpcServerException("Legacy client isn't allowed to connect because there are no public folders.", RpcErrorCode.ClientVerDisallowed);
				}
			}
			return RpcErrorCode.None;
		}

		private static bool TryGetUserDnAndOrganization(string userAddress, ConnectionFlags flags, ref string protocolSequence, out OrganizationId organizationId, out string userDn, out string domain)
		{
			domain = LegacyDnHelper.GetDomainAndLegacyDnFromAddress(userAddress, out userDn);
			if (!string.IsNullOrEmpty(domain))
			{
				organizationId = null;
				return true;
			}
			return RpcDispatch.TryGetFederatedUserDnAndOrganization(userAddress, flags, ref protocolSequence, out organizationId, out userDn, out domain);
		}

		private static bool TryGetFederatedUserDnAndOrganization(string userAddress, ConnectionFlags flags, ref string protocolSequence, out OrganizationId organizationId, out string userDn, out string domain)
		{
			if (protocolSequence != null && protocolSequence.StartsWith(RpcDispatch.WebServiceProtocolSequencePrefix, StringComparison.OrdinalIgnoreCase))
			{
				domain = protocolSequence.Substring(RpcDispatch.WebServiceProtocolSequencePrefix.Length);
				int num = domain.IndexOf("[");
				if (num >= 0)
				{
					domain = domain.Substring(0, num);
				}
				protocolSequence = "xrop";
				organizationId = DomainToOrganizationIdCache.Singleton.Get(new SmtpDomain(domain));
				if (organizationId == null)
				{
					throw new UnknownUserException(string.Format("Unable to map domain '{0}' to OrganizationId", domain));
				}
				if (LegacyDnHelper.IsFederatedSystemAttendant(userAddress))
				{
					if ((flags & ConnectionFlags.UseAdminPrivilege) == ConnectionFlags.None)
					{
						throw new InvalidParameterException("Cannot use federated system attendant without UseAdminPrivilege and only via Xrop");
					}
					userDn = userAddress;
				}
				else
				{
					if ((flags & ConnectionFlags.RemoteSystemService) == ConnectionFlags.RemoteSystemService)
					{
						throw new InvalidParameterException("Must be federated system attendant to use RemoteSystemService");
					}
					try
					{
						userDn = LegacyDnHelper.ConvertToLegacyDn(userAddress, organizationId, true);
					}
					catch (ObjectNotFoundException)
					{
						throw new UnknownUserException(string.Format("Unable to map userAddress '{0}' to exchangePrincipal", userAddress));
					}
				}
				return true;
			}
			else
			{
				if ((flags & ConnectionFlags.RemoteSystemService) == ConnectionFlags.RemoteSystemService)
				{
					throw new InvalidParameterException(string.Format("Must be federated system attendant to use RemoteSystemService and only via Xrop", new object[0]));
				}
				organizationId = null;
				userDn = null;
				domain = null;
				return false;
			}
		}

		private static IEnumerable<BufferWriter.SerializeDelegate> ExecuteTracer(IList<ArraySegment<byte>> ropInArray, ArraySegment<byte> auxIn, bool isFake)
		{
			using (IEnumerator<ArraySegment<byte>> enumerator = ropInArray.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ArraySegment<byte> ropIn = enumerator.Current;
					yield return delegate(Writer writer)
					{
						writer.WriteSizedBytesSegment(ropIn, FieldLength.WordSize);
						writer.WriteSizedBytesSegment(auxIn, FieldLength.WordSize);
						writer.WriteBool(isFake);
					};
					auxIn = Array<byte>.EmptySegment;
					isFake = true;
				}
			}
			yield break;
		}

		private static bool IsBlockTrustworthy(AuxiliaryBlock auxiliaryBlock)
		{
			return RpcDispatch.IsBlockTrustworthy(auxiliaryBlock as BasePerfMdbSuccessAuxiliaryBlock) && RpcDispatch.IsBlockTrustworthy(auxiliaryBlock as BasePerfFailureAuxiliaryBlock);
		}

		private static bool IsBlockTrustworthy(BasePerfMdbSuccessAuxiliaryBlock block)
		{
			return block == null || (block.Version == 2 && block.BlockServerId == 1 && block.BlockProcessId == 1);
		}

		private static bool IsBlockTrustworthy(BasePerfFailureAuxiliaryBlock block)
		{
			return block == null || (block.Version == 2 && block.BlockServerId == 1 && block.BlockProcessId == 1);
		}

		private static void SetOrganizationInfoForProtocolLog(OrganizationId organizationId, IUser user)
		{
			if (user.OrganizationId != null && user.OrganizationId.OrganizationalUnit != null)
			{
				ProtocolLog.SetOrganizationInfo(user.OrganizationId.OrganizationalUnit.ToCanonicalName());
				return;
			}
			if (organizationId != null && organizationId.OrganizationalUnit != null)
			{
				ProtocolLog.SetOrganizationInfo(organizationId.OrganizationalUnit.ToCanonicalName());
			}
		}

		private static IPAddress ParseIpAddressOrDefault(string bindingAddress, IPAddress defaultIpAddress)
		{
			IPAddress result;
			if (bindingAddress == null || !IPAddress.TryParse(bindingAddress, out result))
			{
				result = defaultIpAddress;
			}
			return result;
		}

		private static void AppendMonitoringActivityAuxiliaryBlockAndSerialize(AuxiliaryData auxiliaryData)
		{
			try
			{
				string activityContent = ProtocolLogSession.GenerateActivityScopeReport(true);
				MonitoringActivityAuxiliaryBlock outputBlock = new MonitoringActivityAuxiliaryBlock(activityContent);
				auxiliaryData.AppendOutput(outputBlock);
			}
			catch (Exception)
			{
			}
		}

		private static void AppendServerInformationAuxiliaryBlockAndSerialize(AuxiliaryData auxiliaryData, string serverInfo)
		{
			try
			{
				ServerInformationAuxiliaryBlock outputBlock = new ServerInformationAuxiliaryBlock(serverInfo);
				auxiliaryData.AppendOutput(outputBlock);
			}
			catch (Exception)
			{
			}
		}

		private static void AppendKeyValueStringAuxiliaryBlockAndSerialize(AuxiliaryData auxiliaryData, string key, string value)
		{
			try
			{
				IdentityCorrelationAuxiliaryBlock outputBlock = new IdentityCorrelationAuxiliaryBlock(key, value);
				auxiliaryData.AppendOutput(outputBlock);
			}
			catch (Exception)
			{
			}
		}

		private static ArraySegment<byte> AppendExceptionTraceAuxBlockAndSerialize(Exception exceptionToTrace, AuxiliaryData auxiliaryData, ArraySegment<byte> serializationBuffer)
		{
			ArraySegment<byte> result = Array<byte>.EmptySegment;
			try
			{
				ExceptionTraceAuxiliaryBlock outputBlock = new ExceptionTraceAuxiliaryBlock(0U, exceptionToTrace.ToString());
				auxiliaryData.AppendOutput(outputBlock);
				result = auxiliaryData.Serialize(serializationBuffer);
			}
			catch (Exception)
			{
			}
			return result;
		}

		private static TimeSpan ComputeRandomAdditionalRpcRetryDelay()
		{
			if (RpcDispatch.threadLocalRandom == null)
			{
				RpcDispatch.threadLocalRandom = new Random(Environment.TickCount);
			}
			return TimeSpan.FromMilliseconds(RpcDispatch.threadLocalRandom.NextDouble() * Configuration.ServiceConfiguration.MaxRandomAdditionalRpcRetryDelay.TotalMilliseconds);
		}

		private static string GetPuid()
		{
			ReferencedActivityScope referencedActivityScope = ReferencedActivityScope.Current;
			if (referencedActivityScope == null || referencedActivityScope.Puid == null)
			{
				return string.Empty;
			}
			return referencedActivityScope.Puid;
		}

		private void AppendIdentityCorrelation(AuxiliaryData auxiliaryData, OrganizationId organizationId)
		{
			string puid = RpcDispatch.GetPuid();
			if (!string.IsNullOrEmpty(puid))
			{
				RpcDispatch.AppendKeyValueStringAuxiliaryBlockAndSerialize(auxiliaryData, "PUID", puid);
			}
			if (organizationId != null)
			{
				string value = organizationId.ToExternalDirectoryOrganizationId();
				if (!string.IsNullOrEmpty(value))
				{
					RpcDispatch.AppendKeyValueStringAuxiliaryBlockAndSerialize(auxiliaryData, "OMSTenantID", value);
				}
			}
		}

		private RpcErrorCode ExecuteWrapper(Func<RpcDispatch.ExecuteParameters> getExecuteParameters, Func<RpcErrorCode> executeDelegate, Action<Exception> exceptionSerializationDelegate)
		{
			IActivityScope threadScope = null;
			ReferencedActivityScope referencedActivityScope = null;
			RpcErrorCode result;
			try
			{
				this.disposeLock.EnterReadLock();
				threadScope = ActivityContext.GetCurrentActivityScope();
				if (this.isShuttingDown || RpcClientAccessService.IsShuttingDown)
				{
					result = RpcErrorCode.Exiting;
				}
				else
				{
					base.CheckDisposed();
					RpcErrorCode? storeError = null;
					Exception ex = null;
					bool flag = false;
					RpcDispatch.ExecuteParameters localParameters = default(RpcDispatch.ExecuteParameters);
					localParameters.DropConnectionOnAnyFailure = true;
					bool connectionInUse = false;
					using (Activity.Guard guard = new Activity.Guard())
					{
						try
						{
							if (Thread.CurrentThread.Name == null)
							{
								Thread.CurrentThread.Name = "RPC Dispatch worker thread";
							}
							localParameters = getExecuteParameters();
							guard.AssociateWithCurrentThread(localParameters.Activity, true);
							flag = (localParameters.UserLegacyDN != null && ExUserTracingAdaptor.Instance.IsTracingEnabledUser(localParameters.UserLegacyDN));
							if (flag)
							{
								BaseTrace.CurrentThreadSettings.EnableTracing();
							}
							this.TraceRawInputBuffer(localParameters);
							if (localParameters.Connection != null)
							{
								if (!RpcDispatch.TryStartUsingConnection(localParameters.Connection))
								{
									throw new SessionDeadException("Connection is already marked for removal");
								}
								connectionInUse = true;
								if (localParameters.Connection.IsDead)
								{
									throw new SessionDeadException("Connection has been previously marked as dead");
								}
								localParameters.Connection.RegisterActivity();
								if (localParameters.ConsiderAsUserActivity)
								{
									localParameters.Connection.User.RegisterActivity();
								}
								referencedActivityScope = localParameters.Connection.GetReferencedActivityScope();
							}
							else
							{
								referencedActivityScope = ReferencedActivityScope.Create(null);
							}
							ActivityContext.SetThreadScope(referencedActivityScope.ActivityScope);
							ProtocolLog.LogNewCall();
							storeError = new RpcErrorCode?(executeDelegate());
							ex = null;
							localParameters.DropConnectionOnAnyFailure = false;
						}
						catch (BufferParseException ex2)
						{
							storeError = new RpcErrorCode?(RpcErrorCode.RpcFormat);
							ex = ex2;
						}
						catch (BufferOutOfMemoryException ex3)
						{
							storeError = new RpcErrorCode?(RpcErrorCode.ServerOOM);
							ex = ex3;
						}
						catch (BufferTooSmallException ex4)
						{
							storeError = new RpcErrorCode?(RpcErrorCode.BufferTooSmall);
							ex = ex4;
						}
						catch (ClientBackoffException innerException)
						{
							storeError = new RpcErrorCode?((RpcErrorCode)RpcServiceException.GetHResultFromStatusCode(1723));
							ex = new ServerTooBusyException("Client is being backed off", innerException);
							if (!localParameters.DoNotRethrowExceptionsOnFailure)
							{
								throw ex;
							}
						}
						catch (OutOfMemoryException ex5)
						{
							storeError = new RpcErrorCode?(RpcErrorCode.ServerOOM);
							ex = ex5;
						}
						catch (RpcServerException ex6)
						{
							storeError = new RpcErrorCode?(ex6.StoreError);
							ex = ex6;
						}
						catch (SessionDeadException innerException2)
						{
							localParameters.DropConnectionOnAnyFailure = true;
							storeError = new RpcErrorCode?((RpcErrorCode)RpcServiceException.GetHResultFromStatusCode(1722));
							ex = new ServerUnavailableException("Connection must be re-established", innerException2);
							if (!localParameters.DoNotRethrowExceptionsOnFailure)
							{
								throw ex;
							}
						}
						catch (RpcServiceException ex7)
						{
							ex = ex7;
							storeError = new RpcErrorCode?((RpcErrorCode)RpcServiceException.GetHResultFromStatusCode(ex7.RpcStatus));
							if (!localParameters.DoNotRethrowExceptionsOnFailure)
							{
								throw;
							}
						}
						finally
						{
							this.EndUsingConnection(localParameters, connectionInUse);
							RpcDispatch.LogErrors(storeError, ex);
							this.TraceRawOutputBuffer(storeError, localParameters);
							if (flag)
							{
								BaseTrace.CurrentThreadSettings.DisableTracing();
							}
						}
					}
					if (ex != null && exceptionSerializationDelegate != null)
					{
						exceptionSerializationDelegate(ex);
					}
					result = storeError.Value;
				}
			}
			finally
			{
				if (referencedActivityScope != null)
				{
					referencedActivityScope.Release();
				}
				ActivityContext.SetThreadScope(threadScope);
				try
				{
					this.disposeLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		private void EndUsingConnection(RpcDispatch.ExecuteParameters localParameters, bool connectionInUse)
		{
			bool flag = connectionInUse;
			if (localParameters.DropConnectionOnAnyFailure)
			{
				if (localParameters.Connection != null && !flag)
				{
					flag = RpcDispatch.TryStartUsingConnection(localParameters.Connection);
				}
				if (flag)
				{
					this.MarkForRemoval(localParameters.Connection, DisconnectReason.ServerDropped);
				}
			}
			if (flag)
			{
				localParameters.Connection.DecrementUsageCount();
			}
		}

		private void TraceRawOutputBuffer(RpcErrorCode? storeError, RpcDispatch.ExecuteParameters localParameters)
		{
			if (localParameters.RawOutputTracer != null)
			{
				this.TraceRawData(localParameters.RawTraceType, false, localParameters.ConnectionId, localParameters.DropConnectionOnAnyFailure, storeError ?? ((RpcErrorCode)(-1)), localParameters.RawOutputTracer);
			}
		}

		private void TraceRawInputBuffer(RpcDispatch.ExecuteParameters localParameters)
		{
			if (ExTraceGlobals.RpcRawBufferTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (localParameters.RawInputTracer != null)
				{
					this.TraceRawData(localParameters.RawTraceType, true, localParameters.ConnectionId, localParameters.Connection != null && localParameters.Connection.IsDead, RpcErrorCode.None, localParameters.RawInputTracer);
				}
				if (localParameters.MultiRawInputTracer != null)
				{
					foreach (BufferWriter.SerializeDelegate traceDelegate in localParameters.MultiRawInputTracer)
					{
						this.TraceRawData(localParameters.RawTraceType, true, localParameters.ConnectionId, localParameters.Connection != null && localParameters.Connection.IsDead, RpcErrorCode.None, traceDelegate);
					}
				}
			}
		}

		private void MarkForRemoval(Connection connection, DisconnectReason disconnectReason)
		{
			connection.MarkForRemoval(delegate
			{
				this.connectionList.RemoveAndDisposeConnection(Connection.GetConnectionId(connection), disconnectReason);
			});
		}

		private void UpdateRpcLatencyCounters(double latency)
		{
			RpcDispatch.UpdateRpcLatencyCounters(ref RpcDispatch.averageLatency, latency);
			RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.AveragedLatency.RawValue = (long)RpcDispatch.averageLatency;
		}

		private bool TryAcquireBudget(SecurityIdentifier sid, string userDomain, bool isUnthrottled, out IStandardBudget budget, out Exception exception)
		{
			bool result = false;
			budget = null;
			exception = null;
			try
			{
				if (isUnthrottled)
				{
					budget = StandardBudget.AcquireUnthrottledBudget(sid.ToString(), BudgetType.ResourceTracking);
				}
				else
				{
					ADSessionSettings settings;
					if (!string.IsNullOrEmpty(userDomain))
					{
						settings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(userDomain);
					}
					else
					{
						settings = ADSessionSettings.FromRootOrgScopeSet();
					}
					budget = StandardBudget.Acquire(sid, BudgetType.Rca, settings);
				}
				result = true;
			}
			catch (DataValidationException ex)
			{
				exception = ex;
			}
			catch (ADTransientException ex2)
			{
				exception = ex2;
			}
			catch (ADOperationException ex3)
			{
				exception = ex3;
			}
			return result;
		}

		private bool CanDoConstrainedDelegation(ClientSecurityContext clientSecurityContext)
		{
			if (clientSecurityContext.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid))
			{
				return true;
			}
			SecurityDescriptor securityDescriptor;
			RpcDispatch.ServerSecurityDescriptorCache.GetValues(out securityDescriptor);
			return clientSecurityContext.HasExtendedRightOnObject(securityDescriptor, WellKnownGuid.StoreConstrainedDelegationExtendedRightGuid);
		}

		private bool TryMungeClientSecurityContext(ClientSecurityContext clientSecurityContext, IUser user, out ClientSecurityContext mungedClientSecurityContext)
		{
			mungedClientSecurityContext = null;
			OrganizationId tenantOrganizationId = user.OrganizationId ?? OrganizationId.ForestWideOrgId;
			if (clientSecurityContext.UserSid.IsWellKnown(WellKnownSidType.NetworkServiceSid) || clientSecurityContext.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid))
			{
				return false;
			}
			SecurityIdentifier connectAsSid = user.ConnectAsSid;
			SecurityIdentifier masterAccountSid = user.MasterAccountSid;
			SecurityIdentifier slaveAccountSid = null;
			if (connectAsSid != null)
			{
				if (clientSecurityContext.UserSid == connectAsSid)
				{
					return false;
				}
				if (masterAccountSid != null && clientSecurityContext.UserSid == masterAccountSid)
				{
					slaveAccountSid = connectAsSid;
				}
			}
			ClientSecurityContext clientSecurityContext2 = null;
			try
			{
				if (this.tokenMunger.TryMungeToken(clientSecurityContext, tenantOrganizationId, slaveAccountSid, out clientSecurityContext2))
				{
					mungedClientSecurityContext = clientSecurityContext2;
					return true;
				}
			}
			catch (TokenMungingException ex)
			{
				if (ExTraceGlobals.ConnectRpcTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ConnectRpcTracer.TraceError(0, Activity.TraceId, ex.Message);
				}
			}
			return false;
		}

		private void TraceRawData(int kind, bool isInput, int connectionId, bool isConnectionDead, RpcErrorCode rpcErrorCode, BufferWriter.SerializeDelegate traceDelegate)
		{
			bool flag = ExTraceGlobals.RpcRawBufferTracer.IsTraceEnabled(TraceType.DebugTrace);
			bool flag2 = flag;
			if (flag2)
			{
				BufferWriter.SerializeDelegate a = delegate(Writer writer)
				{
					writer.WriteUInt32(this.GetGlobalConnectionId(connectionId));
					writer.WriteBool(isConnectionDead);
					writer.WriteInt32((int)rpcErrorCode);
				};
				string text = Convert.ToBase64String(BufferWriter.Serialize((BufferWriter.SerializeDelegate)Delegate.Combine(a, traceDelegate)));
				StringBuilder stringBuilder = new StringBuilder(12 + text.Length + 1);
				stringBuilder.Append("BIN");
				stringBuilder.Append(text.Length.ToString("X8"));
				stringBuilder.Append("|");
				stringBuilder.Append(text);
				stringBuilder.Append("|");
				if (flag)
				{
					ExTraceGlobals.RpcRawBufferTracer.TraceDebug(kind | (isInput ? 16 : 32), Activity.TraceId, stringBuilder.ToString());
				}
			}
		}

		private uint GetGlobalConnectionId(int connectionId)
		{
			int hashCode = this.GetHashCode();
			ushort num = (ushort)(hashCode >> 16 ^ (hashCode & 65535));
			return (uint)((int)num << 16 | connectionId);
		}

		private IStandardBudget AcquireBudget(string userDomain, string userDn, ConnectionFlags connectionFlags, SecurityIdentifier sid)
		{
			IStandardBudget result = null;
			bool flag = false;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(2984652093U, ref flag);
			if (flag)
			{
				result = StandardBudget.AcquireUnthrottledBudget("fakeBudget", BudgetType.Rca);
			}
			else
			{
				Exception ex = null;
				bool isUnthrottled = (connectionFlags & ConnectionFlags.UseAdminPrivilege) != ConnectionFlags.None && sid != null && sid.IsWellKnown(WellKnownSidType.LocalSystemSid);
				if (!this.TryAcquireBudget(sid, userDomain, isUnthrottled, out result, out ex))
				{
					ExTraceGlobals.ConnectRpcTracer.TraceError<string, SecurityIdentifier, Exception>(0, Activity.TraceId, "Connect failed. Failed to acquire budget for user '{0}'. SID: {1}. {2}.", userDn, sid, ex);
					throw new RpcServerException("Connect failed. Failed to acquire budget.", RpcErrorCode.ADError, ex);
				}
			}
			return result;
		}

		private const int RawTraceVersion = 196608;

		private const int RawConnect = 196609;

		private const int RawDisconnect = 196610;

		private const int RawExecute = 196611;

		private const int IsRawInput = 16;

		private const int IsRawOutput = 32;

		private const string PuidKey = "PUID";

		private const string TenantGuidKey = "OMSTenantID";

		private const int LatencySamples = 1024;

		private const string ActivityScopeComponent = "RCA/Mailbox";

		public static readonly string WebServiceProtocolSequencePrefix = "xrop:";

		private static readonly WorkloadSettings DefaultWorkloadSettings = new WorkloadSettings(WorkloadType.Momt, false);

		private static readonly ResourceKey[] LocalCPUResourceKeyOnly = new ResourceKey[]
		{
			ProcessorResourceKey.Local
		};

		private static readonly TimeSpan DefaultActiveUsersSweepInterval = TimeSpan.FromMinutes(2.0);

		private static double averageLatency = 0.0;

		private static SingleDirectoryObjectCache<Server, SecurityDescriptor> serverSecurityDescriptorCache;

		[ThreadStatic]
		private static Random threadLocalRandom;

		private readonly HandlerFactory handlerFactory;

		private readonly IDriverFactory driverFactory;

		private readonly string serverInfo;

		private readonly ConnectionList connectionList = new ConnectionList();

		private readonly UserManager userManager;

		private readonly ITokenMunger tokenMunger;

		private readonly PerformanceCounterProcessor perfCounterProcessor = PerformanceCounterProcessor.Create();

		private readonly RpcProcessingTimeProcessor rpcProcessingTimeProcessor = RpcProcessingTimeProcessor.Create();

		private readonly RpcFailureProcessor rpcFailureProcessor = RpcFailureProcessor.Create();

		private readonly Timer userActivityTimer;

		private readonly IPAddress serverIpAddress;

		private readonly ReaderWriterLockSlim disposeLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		private bool isShuttingDown;

		private struct ExecuteParameters
		{
			public ExecuteParameters(Activity activity, string userLegacyDN, int connectionId, Connection connection)
			{
				this = new RpcDispatch.ExecuteParameters(activity, userLegacyDN, connectionId, connection, connection != null && connection.DispatchOptions.DoNotRethrowExceptionsOnFailure);
			}

			public ExecuteParameters(Activity activity, string userLegacyDN, int connectionId, bool doNotRethrowExceptionsOnFailure)
			{
				this = new RpcDispatch.ExecuteParameters(activity, userLegacyDN, connectionId, null, doNotRethrowExceptionsOnFailure);
			}

			private ExecuteParameters(Activity activity, string userLegacyDN, int connectionId, Connection connection, bool doNotRethrowExceptionsOnFailure)
			{
				this = default(RpcDispatch.ExecuteParameters);
				this.activity = activity;
				this.userLegacyDN = userLegacyDN;
				this.connectionId = connectionId;
				this.connection = connection;
				this.ConsiderAsUserActivity = false;
				this.DropConnectionOnAnyFailure = false;
				this.RawInputTracer = null;
				this.RawOutputTracer = null;
				this.doNotRethrowExceptionsOnFailure = doNotRethrowExceptionsOnFailure;
			}

			public Activity Activity
			{
				get
				{
					return this.activity;
				}
			}

			public string UserLegacyDN
			{
				get
				{
					return this.userLegacyDN;
				}
			}

			public int ConnectionId
			{
				get
				{
					return this.connectionId;
				}
			}

			public Connection Connection
			{
				get
				{
					return this.connection;
				}
			}

			public bool DoNotRethrowExceptionsOnFailure
			{
				get
				{
					return this.doNotRethrowExceptionsOnFailure;
				}
			}

			public bool ConsiderAsUserActivity { get; set; }

			public bool DropConnectionOnAnyFailure { get; set; }

			public int RawTraceType { get; set; }

			public IEnumerable<BufferWriter.SerializeDelegate> MultiRawInputTracer { get; set; }

			public BufferWriter.SerializeDelegate RawInputTracer { get; set; }

			public BufferWriter.SerializeDelegate RawOutputTracer { get; set; }

			private readonly Activity activity;

			private readonly string userLegacyDN;

			private readonly int connectionId;

			private readonly Connection connection;

			private readonly bool doNotRethrowExceptionsOnFailure;
		}
	}
}
