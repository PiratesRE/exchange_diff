using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Directory.TopologyService;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	internal sealed class SuitabilityVerifier
	{
		private static bool IsSetupOrTestContext()
		{
			ADDriverContext processADContext = ADSessionSettings.GetProcessADContext();
			return (processADContext != null && (processADContext.Mode == ContextMode.Setup || processADContext.Mode == ContextMode.Test)) || ExEnvironment.IsTestDomain;
		}

		internal static bool IsOSVersionSuitable(string serverName, string osVersion, string osServicePack)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				throw new ArgumentNullException("serverName");
			}
			if (string.IsNullOrEmpty(osVersion))
			{
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string>(0L, "{0} - IsOSVersionSuitable was invoked with a null or empty osVersion parameter.", serverName);
				SuitabilityVerifier.LogNoSuitableOsEvent(serverName, osVersion, osServicePack);
				return false;
			}
			Match match = new Regex("^(\\d+)\\.(\\d+) \\(\\d+\\)$").Match(osVersion);
			if (!match.Success)
			{
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string, string>(0L, "{0} - osVersion '{1}' does not match the OSVersionPattern.", serverName, osVersion);
				SuitabilityVerifier.LogNoSuitableOsEvent(serverName, osVersion, osServicePack);
				return false;
			}
			int num;
			try
			{
				num = (int)short.Parse(match.Groups[1].Value);
			}
			catch (OverflowException)
			{
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string, string>(0L, "{0} - Parsing major version number '{1}' has thrown an OverflowException.", serverName, match.Groups[1].Value);
				SuitabilityVerifier.LogNoSuitableOsEvent(serverName, osVersion, osServicePack);
				return false;
			}
			if (num > 5)
			{
				return true;
			}
			if (num < 5)
			{
				SuitabilityVerifier.LogNoSuitableOsEvent(serverName, osVersion, osServicePack);
				return false;
			}
			int num2;
			try
			{
				num2 = (int)short.Parse(match.Groups[2].Value);
			}
			catch (OverflowException)
			{
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string, string>(0L, "{0} - Parsing minor version number '{1}' has thrown an OverflowException.", serverName, match.Groups[2].Value);
				SuitabilityVerifier.LogNoSuitableOsEvent(serverName, osVersion, osServicePack);
				return false;
			}
			if (num2 > 2)
			{
				return true;
			}
			if (num2 < 2)
			{
				SuitabilityVerifier.LogNoSuitableOsEvent(serverName, osVersion, osServicePack);
				return false;
			}
			if (string.IsNullOrEmpty(osServicePack))
			{
				SuitabilityVerifier.LogNoSuitableOsEvent(serverName, osVersion, osServicePack);
				return false;
			}
			return true;
		}

		private static void LogNoSuitableOsEvent(string serverName, string osVersion, string spVersion)
		{
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_BAD_OS_VERSION, "OsSuitability" + serverName, new object[]
			{
				serverName,
				string.IsNullOrEmpty(osVersion) ? "<null>" : osVersion,
				string.IsNullOrEmpty(spVersion) ? "<null>" : spVersion
			});
		}

		private static void LogEventSyncFailed(string serverName, int errorCode, string errorMessage, SearchRequest request)
		{
			string text;
			object obj;
			object obj2;
			if (request != null)
			{
				text = request.DistinguishedName;
				obj = request.Filter;
				obj2 = request.Scope;
			}
			else
			{
				text = string.Empty;
				obj = string.Empty;
				obj2 = string.Empty;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_SYNC_FAILED, "LdapSearchFailedOn" + serverName, new object[]
			{
				serverName,
				errorCode,
				text,
				obj,
				obj2,
				errorMessage
			});
		}

		private static PooledLdapConnection CreateConnectionAndBind(string serverFqdn, NetworkCredential networkCredential, int portNumber)
		{
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, int>(0L, "{0} - CreateConnectionAndBind. Port Number {1}", serverFqdn, portNumber);
			ADServerRole role = (3268 == portNumber) ? ADServerRole.GlobalCatalog : ADServerRole.DomainController;
			ADServerInfo serverInfo = new ADServerInfo(serverFqdn, portNumber, null, 100, AuthType.Negotiate);
			PooledLdapConnection pooledLdapConnection = new PooledLdapConnection(serverInfo, role, false, networkCredential);
			int num = 0;
			ADErrorRecord aderrorRecord;
			for (;;)
			{
				aderrorRecord = null;
				if (pooledLdapConnection.TryBindWithRetry(1, out aderrorRecord))
				{
					break;
				}
				num++;
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_BIND_FAILED, "BindFailedOn" + serverFqdn, new object[]
				{
					serverFqdn,
					(int)aderrorRecord.LdapError,
					portNumber,
					aderrorRecord.Message
				});
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError(0L, "CreateConnectionAndBind. Failed to create and bind AD connection against server '{0}' to perform the suitability check. Connection: {1}, Retry attempt: '{2}'. Error: {3}", new object[]
				{
					serverFqdn,
					pooledLdapConnection,
					num,
					aderrorRecord
				});
				if (aderrorRecord.LdapError != LdapError.InvalidCredentials)
				{
					goto IL_10E;
				}
				if (aderrorRecord.HandlingType == HandlingType.Throw || num == 3)
				{
					goto IL_F7;
				}
				Thread.Sleep(3000);
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, int>(0L, "CreateConnectionAndBind. Successfully created and bound AD connection against server '{0}' to perform the suitability check. Port: '{1}'", serverFqdn, portNumber);
			return pooledLdapConnection;
			IL_F7:
			throw aderrorRecord.InnerException;
			IL_10E:
			throw new SuitabilityDirectoryException(serverFqdn, (int)aderrorRecord.LdapError, aderrorRecord.Message, aderrorRecord.InnerException)
			{
				ServerFqdn = serverFqdn
			};
		}

		private static IEnumerable<Task> SuitabilityChecks(SuitabilityVerifier.SuitabilityCheckContext context, NetworkCredential credential, bool doFullCheck, bool isInitialDiscovery)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug((long)context.GetHashCode(), "{0} - Starting suitabilities checks. DoFullCheck {1}. IsInitialDiscovery {2}. NetworkCredentials {3}", new object[]
			{
				context.ServerFqdn,
				doFullCheck,
				isInitialDiscovery,
				(credential == null) ? "<NULL>" : credential.Domain
			});
			Task activeTask = null;
			if (doFullCheck)
			{
				activeTask = SuitabilityVerifier.CheckDNS(context);
				yield return activeTask;
			}
			else
			{
				context.SuitabilityResult.IsDNSEntryAvailable = true;
			}
			if (!context.SuitabilityResult.IsDNSEntryAvailable)
			{
				yield return DnsTroubleshooter.DiagnoseDnsProblemForDomainController(context.ServerFqdn);
				if (activeTask.IsFaulted)
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED, null, new object[]
					{
						context.ServerFqdn,
						activeTask.Exception.ToString() + " (in CheckDNS)"
					});
					throw activeTask.Exception;
				}
			}
			else
			{
				if (doFullCheck)
				{
					foreach (Task task in SuitabilityVerifier.PerformReachabilityChecks(context))
					{
						activeTask = task;
						yield return task;
					}
					if (activeTask.IsFaulted)
					{
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED, null, new object[]
						{
							context.ServerFqdn,
							activeTask.Exception.ToString() + " (in PerformReachabilityChecks)"
						});
						throw activeTask.Exception;
					}
				}
				else
				{
					ADServerRole adserverRole = ADServerRole.None;
					if (context.GCPort >= 0)
					{
						adserverRole |= ADServerRole.GlobalCatalog;
					}
					if (context.DCPort >= 0)
					{
						adserverRole |= (ADServerRole.DomainController | ADServerRole.ConfigurationDomainController);
					}
					context.SuitabilityResult.IsReachableByTCPConnection = adserverRole;
				}
				SuitabilityVerifier.CheckCreateConnection(context, credential);
				activeTask = SuitabilityVerifier.GetDefaultNCResponse(context);
				yield return activeTask;
				if (activeTask.IsFaulted)
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED, null, new object[]
					{
						context.ServerFqdn,
						activeTask.Exception.ToString() + " (in GetDefaultNCResponse)"
					});
					throw activeTask.Exception;
				}
				SuitabilityVerifier.CheckIsSyncronized(context);
				bool isForestLocal = SuitabilityVerifier.IsForestLocal(context.ServerFqdn);
				ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, string>((long)context.GetHashCode(), "{0} - SACL and critical data checks {1} be performed", context.ServerFqdn, (doFullCheck && isForestLocal) ? "WILL" : "WON'T");
				if (doFullCheck && isForestLocal)
				{
					activeTask = SuitabilityVerifier.CheckSACLRight(context);
					yield return activeTask;
					if (activeTask.IsFaulted)
					{
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED, null, new object[]
						{
							context.ServerFqdn,
							activeTask.Exception.ToString() + " (in CheckSACLRight)"
						});
						throw activeTask.Exception;
					}
					activeTask = SuitabilityVerifier.CheckCriticalData(context);
					yield return activeTask;
					if (activeTask.IsFaulted)
					{
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED, null, new object[]
						{
							context.ServerFqdn,
							activeTask.Exception.ToString() + " (in CheckCriticalData)"
						});
						throw activeTask.Exception;
					}
				}
				else
				{
					context.SuitabilityResult.IsSACLRightAvailable = true;
					context.SuitabilityResult.IsCriticalDataAvailable = true;
				}
				ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, string>((long)context.GetHashCode(), "{0} - OS version checks {1} be performed", context.ServerFqdn, (!context.AllowPreW2KSP3DC) ? "WILL" : "WON'T");
				if (!context.AllowPreW2KSP3DC)
				{
					activeTask = SuitabilityVerifier.CheckOperatingSystemSuitable(context);
					yield return activeTask;
					if (activeTask.IsFaulted)
					{
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED, null, new object[]
						{
							context.ServerFqdn,
							activeTask.Exception.ToString() + " (in CheckOperatingSystemSuitable)"
						});
						throw activeTask.Exception;
					}
				}
				else
				{
					context.SuitabilityResult.IsOSVersionSuitable = true;
				}
				ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, string>((long)context.GetHashCode(), "{0} - PDC checks {1} be performed", context.ServerFqdn, context.IsPDCCheckEnabled ? "WILL" : "WONT");
				if (context.IsPDCCheckEnabled)
				{
					activeTask = SuitabilityVerifier.CheckPDC(context);
					yield return activeTask;
					if (activeTask.IsFaulted)
					{
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED, null, new object[]
						{
							context.ServerFqdn,
							activeTask.Exception.ToString() + " (in CheckPDC)"
						});
						throw activeTask.Exception;
					}
				}
				ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, string>((long)context.GetHashCode(), "{0} - MM checks {1} be performed", context.ServerFqdn, (Globals.IsDatacenter && !context.AllowPreW2KSP3DC) ? "WILL" : "WONT");
				if (Globals.IsDatacenter && !context.AllowPreW2KSP3DC)
				{
					activeTask = SuitabilityVerifier.CheckIsDCInMaintenanceMode(context);
					yield return activeTask;
					if (activeTask.IsFaulted)
					{
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED, null, new object[]
						{
							context.ServerFqdn,
							activeTask.Exception.ToString() + " (in CheckIsDCInMaintenanceMode)"
						});
						throw activeTask.Exception;
					}
				}
				ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, string>((long)context.GetHashCode(), "{0} - Net logon checks {1} be performed", context.ServerFqdn, (doFullCheck && !isInitialDiscovery && !context.DisableNetLogonCheck) ? "WILL" : "WON'T");
				if (doFullCheck && !isInitialDiscovery && !context.DisableNetLogonCheck)
				{
					SuitabilityVerifier.CheckNetLogon(context);
				}
				else
				{
					context.SuitabilityResult.IsNetlogonAllowed = (ADServerRole.GlobalCatalog | ADServerRole.DomainController | ADServerRole.ConfigurationDomainController);
				}
			}
			yield break;
		}

		private static bool InternalTryCheckIsServerSuitable(string fqdn, bool isGlobalCatalog, NetworkCredential credential, out string writableNC, out string siteName, out Exception exception)
		{
			siteName = null;
			writableNC = null;
			exception = null;
			SuitabilityVerifier.SuitabilityCheckContext suitabilityCheckContext = new SuitabilityVerifier.SuitabilityCheckContext(fqdn, isGlobalCatalog, false, false, false);
			try
			{
				Task task = Task.Factory.Iterate(SuitabilityVerifier.SuitabilityChecks(suitabilityCheckContext, credential, false, false));
				task.Wait();
			}
			catch (AggregateException ex)
			{
				Exception ex2 = ex.Flatten();
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string, string>(0L, "{0} Suitability Error: {1}", suitabilityCheckContext.ServerFqdn, ex2.ToString());
				ex2 = ex2.InnerException;
				if (!(ex2 is SuitabilityException) && !(ex2 is ADOperationException) && !(ex2 is ADExternalException) && !(ex2 is ADTransientException) && !(ex2 is LdapException) && !(ex2 is DirectoryOperationException) && !(ex2 is ADServerNotSuitableException))
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_SUITABILITY_CHECK_FAILED, null, new object[]
					{
						suitabilityCheckContext.ServerFqdn,
						ex2.ToString()
					});
					throw ex2;
				}
				exception = ex2;
			}
			finally
			{
				suitabilityCheckContext.CloseDCConnection();
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, SuitabilityCheckResult>(0L, "{0} Suitabilities for server {1}", suitabilityCheckContext.ServerFqdn, suitabilityCheckContext.SuitabilityResult);
			writableNC = suitabilityCheckContext.SuitabilityResult.WritableNC;
			siteName = suitabilityCheckContext.SiteName;
			return suitabilityCheckContext.SuitabilityResult.IsSuitable(isGlobalCatalog ? ADServerRole.GlobalCatalog : ADServerRole.DomainController);
		}

		private static Task<IPHostEntry> StartCheckDNSData(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - StartCheckDNSData", context.ServerFqdn);
			Tuple<SuitabilityVerifier.SuitabilityCheckContext, int> state = new Tuple<SuitabilityVerifier.SuitabilityCheckContext, int>(context, Environment.TickCount);
			return Task.Factory.FromAsync<string, IPHostEntry>(new Func<string, AsyncCallback, object, IAsyncResult>(Dns.BeginGetHostEntry), new Func<IAsyncResult, IPHostEntry>(Dns.EndGetHostEntry), context.ServerFqdn, state, TaskCreationOptions.AttachedToParent);
		}

		private static void EndCheckDNSData(Task<IPHostEntry> task)
		{
			ArgumentValidator.ThrowIfTypeInvalid<Tuple<SuitabilityVerifier.SuitabilityCheckContext, int>>("Async Context", task.AsyncState);
			Tuple<SuitabilityVerifier.SuitabilityCheckContext, int> tuple = (Tuple<SuitabilityVerifier.SuitabilityCheckContext, int>)task.AsyncState;
			SuitabilityVerifier.SuitabilityCheckContext item = tuple.Item1;
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string>((long)item.GetHashCode(), "{0} - Check DNS Entry", item.ServerFqdn);
			int item2 = tuple.Item2;
			ADProviderPerf.UpdateDCCounter(item.ServerFqdn, Counter.DCTimeGethostbyname, UpdateType.Update, Convert.ToUInt32(Globals.GetTickDifference(item2, Environment.TickCount)));
			if (task.Status == TaskStatus.RanToCompletion)
			{
				IPHostEntry result = task.Result;
				if (result != null && result.AddressList != null && result.AddressList.Length > 0)
				{
					item.SuitabilityResult.IsDNSEntryAvailable = true;
					item.HostName = result.HostName;
					item.IPAddresses = result.AddressList;
				}
				else
				{
					item.SuitabilityResult.IsDNSEntryAvailable = false;
				}
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, bool>((long)item.GetHashCode(), "{0} - Check DNS Entry. Value {1}", item.ServerFqdn, item.SuitabilityResult.IsDNSEntryAvailable);
			Exception ex = null;
			if (task.Status != TaskStatus.RanToCompletion)
			{
				item.SuitabilityResult.IsDNSEntryAvailable = false;
				ex = ((task.Exception != null) ? task.Exception.GetBaseException() : null);
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string, string>((long)item.GetHashCode(), "{0} - Check DNS Failed. Error {1}", item.ServerFqdn, (ex == null) ? "UnKnown" : ex.Message);
				if (ex != null)
				{
					SocketException ex2 = ex as SocketException;
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_GETHOSTBYNAME_FAILED, null, new object[]
					{
						item.ServerFqdn,
						(ex2 != null) ? ex2.NativeErrorCode : -1,
						ex.Message
					});
				}
			}
			if (!item.SuitabilityResult.IsDNSEntryAvailable)
			{
				LocalizedString errorMessage = DirectoryStrings.SuitabilityErrorDNS(item.ServerFqdn, (ex != null) ? ex.Message : "Unknown");
				SuitabilityVerifier.ThrowException(ex, errorMessage, item.ServerFqdn);
			}
		}

		private static Task CheckDNS(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - CheckDNS", context.ServerFqdn);
			Task<IPHostEntry> task = SuitabilityVerifier.StartCheckDNSData(context);
			return task.ContinueWith(new Action<Task<IPHostEntry>>(SuitabilityVerifier.EndCheckDNSData), TaskContinuationOptions.AttachedToParent);
		}

		private static Task StartCheckReachabilityForPort(SuitabilityVerifier.SuitabilityCheckContext context, IPAddress[] addresses, int port)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNegative("port", port);
			ArgumentValidator.ThrowIfNull("addresses", addresses);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string, int>((long)context.GetHashCode(), "{0} - StartCheckReachabilityForPort {1}", context.ServerFqdn, port);
			if (string.IsNullOrEmpty(context.HostName))
			{
				throw new InvalidOperationException("HostName shouldn't be null at this point.");
			}
			if (addresses.Length == 0)
			{
				throw new InvalidOperationException("IpAddresses should have at least one ipAddress.");
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, AddressFamily>((long)context.GetHashCode(), "{0} - Reachability checks will be done using {1}", context.ServerFqdn, addresses[0].AddressFamily);
			TcpClient tcpClient = new TcpClient(addresses[0].AddressFamily);
			Tuple<SuitabilityVerifier.SuitabilityCheckContext, TcpClient, int> state = new Tuple<SuitabilityVerifier.SuitabilityCheckContext, TcpClient, int>(context, tcpClient, port);
			return Task.Factory.FromAsync<IPAddress[], int>(new Func<IPAddress[], int, AsyncCallback, object, IAsyncResult>(tcpClient.BeginConnect), new Action<IAsyncResult>(tcpClient.EndConnect), addresses, port, state, TaskCreationOptions.AttachedToParent);
		}

		private static void EndCheckReachabilityForPort(Task task)
		{
			ArgumentValidator.ThrowIfTypeInvalid<Tuple<SuitabilityVerifier.SuitabilityCheckContext, TcpClient, int>>("SuitabilityContext", task.AsyncState);
			Tuple<SuitabilityVerifier.SuitabilityCheckContext, TcpClient, int> tuple = (Tuple<SuitabilityVerifier.SuitabilityCheckContext, TcpClient, int>)task.AsyncState;
			SuitabilityVerifier.SuitabilityCheckContext item = tuple.Item1;
			TcpClient item2 = tuple.Item2;
			int item3 = tuple.Item3;
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)item.GetHashCode(), "{0} - EndCheckReachabilityForPort", item.ServerFqdn);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, int, bool>((long)item.GetHashCode(), "{0} - EndCheckReachabilityForPort {1}. TCP Client Is Connected {2}", item.ServerFqdn, item3, item2.Connected);
			if (item2.Connected)
			{
				using (NetworkStream stream = item2.GetStream())
				{
					ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug((long)item.GetHashCode(), "{0} - EndCheckReachabilityForPort {1}. CanRead {2} CanWrite {3}", new object[]
					{
						item.ServerFqdn,
						item3,
						stream.CanRead,
						stream.CanWrite
					});
					if (stream.CanRead && stream.CanWrite)
					{
						if (item3 == item.DCPort)
						{
							lock (item.SuitabilityResult)
							{
								item.SuitabilityResult.IsReachableByTCPConnection |= (ADServerRole.DomainController | ADServerRole.ConfigurationDomainController);
								goto IL_15A;
							}
						}
						lock (item.SuitabilityResult)
						{
							item.SuitabilityResult.IsReachableByTCPConnection |= ADServerRole.GlobalCatalog;
						}
					}
					IL_15A:;
				}
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, int, ADServerRole>((long)item.GetHashCode(), "{0} - CheckReachabilityForPort {1}. Value {2}", item.ServerFqdn, item3, item.SuitabilityResult.IsReachableByTCPConnection);
			item2.Close();
			((IDisposable)item2).Dispose();
			if (task.Status != TaskStatus.RanToCompletion && task.Exception != null)
			{
				Exception baseException = task.Exception.GetBaseException();
				SuitabilityVerifier.ThrowException(baseException, DirectoryStrings.SuitabilityReachabilityError(item.ServerFqdn, item3.ToString(), baseException.Message), item.ServerFqdn);
			}
		}

		private static Task CheckReachabilityForPort(SuitabilityVerifier.SuitabilityCheckContext context, IPAddress[] addresses, int port)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("addresses", addresses);
			ArgumentValidator.ThrowIfNegative("port", port);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string, int>((long)context.GetHashCode(), "{0} - CheckReachabilityForPort {1}", context.ServerFqdn, port);
			Task task = SuitabilityVerifier.StartCheckReachabilityForPort(context, addresses, port);
			return task.ContinueWith(new Action<Task>(SuitabilityVerifier.EndCheckReachabilityForPort), TaskContinuationOptions.AttachedToParent);
		}

		private static IEnumerable<Task> PerformReachabilityChecks(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - PerformReachabilityChecks", context.ServerFqdn);
			if (context.IPAddresses == null)
			{
				throw new InvalidOperationException("IpAddresses shouldn't be null at this point.");
			}
			if (context.IPAddresses.Length == 0)
			{
				throw new InvalidOperationException("IpAddresses should have at least one ipAddress.");
			}
			List<IPAddress> ipv6 = new List<IPAddress>(Math.Max(context.IPAddresses.Length / 2, 1));
			List<IPAddress> ipv7 = new List<IPAddress>(Math.Max(context.IPAddresses.Length / 2, 1));
			foreach (IPAddress ipaddress in context.IPAddresses)
			{
				if (AddressFamily.InterNetworkV6 == ipaddress.AddressFamily)
				{
					ipv6.Add(ipaddress);
				}
				else if (AddressFamily.InterNetwork == ipaddress.AddressFamily)
				{
					ipv7.Add(ipaddress);
				}
			}
			List<List<IPAddress>> ipAddressesByFamily = new List<List<IPAddress>>((ipv6.Count > 0 && ipv7.Count > 0) ? 2 : 1);
			if (ipv7.Count > 0)
			{
				ipAddressesByFamily.Add(ipv7);
			}
			if (ipv6.Count > 0)
			{
				ipAddressesByFamily.Add(ipv6);
			}
			string aggregatedErrorMessage = string.Empty;
			foreach (List<IPAddress> ipAddresses in ipAddressesByFamily)
			{
				List<Task> reachabilityChecks = new List<Task>(2);
				if (context.DCPort >= 0)
				{
					ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string>((long)context.GetHashCode(), "{0} - DC Reachability will be checked", context.ServerFqdn);
					Task item = SuitabilityVerifier.CheckReachabilityForPort(context, ipAddresses.ToArray(), context.DCPort);
					reachabilityChecks.Add(item);
				}
				if (context.GCPort >= 0)
				{
					ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string>((long)context.GetHashCode(), "{0} - GC Reachability will be checked", context.ServerFqdn);
					Task item2 = SuitabilityVerifier.CheckReachabilityForPort(context, ipAddresses.ToArray(), context.GCPort);
					reachabilityChecks.Add(item2);
				}
				yield return Task.WhenAll(reachabilityChecks.ToArray());
				ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, AddressFamily>((long)context.GetHashCode(), "{0} - Validating Reachability checks for Address Family {1}", context.ServerFqdn, ipAddresses[0].AddressFamily);
				bool checkNextAddressFamily = false;
				foreach (Task task in reachabilityChecks)
				{
					checkNextAddressFamily |= (task.Status != TaskStatus.RanToCompletion);
					if (task.Exception != null)
					{
						ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, Exception>((long)context.GetHashCode(), "{0} - Reachability checks failed. Error {1}", context.ServerFqdn, task.Exception.GetBaseException());
						Exception baseException = task.Exception.GetBaseException();
						aggregatedErrorMessage = string.Format("{0}{1}{2}", aggregatedErrorMessage, Environment.NewLine, baseException.Message);
					}
				}
				if (!checkNextAddressFamily)
				{
					break;
				}
			}
			if (context.SuitabilityResult.IsReachableByTCPConnection == ADServerRole.None)
			{
				ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string>((long)context.GetHashCode(), "{0} - All Reachability checks failed", context.ServerFqdn);
				throw new SuitabilityException(DirectoryStrings.ComposedSuitabilityReachabilityError(context.ServerFqdn, aggregatedErrorMessage), context.ServerFqdn);
			}
			yield break;
		}

		private static Task<DirectoryResponse> StartGetDefaultNamingContexts(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - StartGetDefaultNamingContexts", context.ServerFqdn);
			SearchRequest request = new SearchRequest(string.Empty, "(objectclass=*)", SearchScope.Base, SuitabilityVerifier.DefaultNamingContextProperties);
			return SuitabilityVerifier.SendAsyncLdapRequest(context.DCConnection, request, context);
		}

		private static void EndGetDefaultNCResponse(Task<DirectoryResponse> task)
		{
			ArgumentValidator.ThrowIfTypeInvalid<SuitabilityVerifier.SuitabilityCheckContext>("Async Context", task.AsyncState);
			SuitabilityVerifier.SuitabilityCheckContext suitabilityCheckContext = (SuitabilityVerifier.SuitabilityCheckContext)task.AsyncState;
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)suitabilityCheckContext.GetHashCode(), "{0} - EndGetDefaultNCResponse", suitabilityCheckContext.ServerFqdn);
			SearchResponse searchResponse = (SearchResponse)task.Result;
			if (searchResponse == null || searchResponse.Entries.Count <= 0)
			{
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string>((long)suitabilityCheckContext.GetHashCode(), "{0} - CheckDefaultNCResponse. Error trying to retrieve all the necessary information to continue with suitability checks", suitabilityCheckContext.ServerFqdn);
				throw new ADExternalException(DirectoryStrings.ErrorIsServerSuitableMissingDefaultNamingContext(suitabilityCheckContext.DCConnection.SessionOptions.HostName));
			}
			if (!searchResponse.Entries[0].Attributes.Contains("configurationNamingContext") || !searchResponse.Entries[0].Attributes.Contains("defaultNamingContext") || !searchResponse.Entries[0].Attributes.Contains("supportedCapabilities") || !searchResponse.Entries[0].Attributes.Contains("schemaNamingContext"))
			{
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string>((long)suitabilityCheckContext.GetHashCode(), "{0} - CheckDefaultNCResponse. Could not retrieve all the necessary information to continue with suitability checks", suitabilityCheckContext.ServerFqdn);
				throw new ADExternalException(DirectoryStrings.ErrorIsServerSuitableMissingDefaultNamingContext(suitabilityCheckContext.DCConnection.SessionOptions.HostName));
			}
			string[] array = (string[])searchResponse.Entries[0].Attributes["supportedCapabilities"].GetValues(typeof(string));
			foreach (string a in array)
			{
				if (string.Equals(a, "1.2.840.113556.1.4.1920", StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string>((long)suitabilityCheckContext.GetHashCode(), "{0} - CheckOperatingSystemSuitable. DC is RODC", suitabilityCheckContext.ServerFqdn);
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_RODC_FOUND, "OsSuitability" + suitabilityCheckContext.DCConnection.SessionOptions.HostName, new object[]
					{
						suitabilityCheckContext.DCConnection.SessionOptions.HostName
					});
					suitabilityCheckContext.SuitabilityResult.IsReadOnlyDC = true;
					throw new ADServerNotSuitableException(DirectoryStrings.ErrorIsServerSuitableRODC(suitabilityCheckContext.DCConnection.SessionOptions.HostName), suitabilityCheckContext.DCConnection.SessionOptions.HostName);
				}
			}
			suitabilityCheckContext.DefaultNCResponse = searchResponse;
			suitabilityCheckContext.SuitabilityResult.RootNC = (string)suitabilityCheckContext.DefaultNCResponse.Entries[0].Attributes["defaultNamingContext"].GetValues(typeof(string))[0];
			suitabilityCheckContext.SuitabilityResult.ConfigNC = (string)suitabilityCheckContext.DefaultNCResponse.Entries[0].Attributes["configurationNamingContext"].GetValues(typeof(string))[0];
			suitabilityCheckContext.SuitabilityResult.SchemaNC = (string)suitabilityCheckContext.DefaultNCResponse.Entries[0].Attributes["schemaNamingContext"].GetValues(typeof(string))[0];
			if (!suitabilityCheckContext.SuitabilityResult.IsReadOnlyDC)
			{
				suitabilityCheckContext.SuitabilityResult.WritableNC = suitabilityCheckContext.SuitabilityResult.RootNC;
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug((long)suitabilityCheckContext.GetHashCode(), "{0} - PopulateDefaultNamingContexts. RootNC {1} ConfigNC {2} SchemaNC {3}", new object[]
			{
				suitabilityCheckContext.ServerFqdn,
				suitabilityCheckContext.SuitabilityResult.RootNC,
				suitabilityCheckContext.SuitabilityResult.ConfigNC,
				suitabilityCheckContext.SuitabilityResult.SchemaNC
			});
		}

		private static Task GetDefaultNCResponse(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - GetDefaultNCResponse", context.ServerFqdn);
			Task<DirectoryResponse> task = SuitabilityVerifier.StartGetDefaultNamingContexts(context);
			return task.ContinueWith(new Action<Task<DirectoryResponse>>(SuitabilityVerifier.EndGetDefaultNCResponse), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		private static Task<DirectoryResponse> StartCheckSACLRight(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - StartCheckSACLRight", context.ServerFqdn);
			SearchRequest request = new SearchRequest(context.SuitabilityResult.ConfigNC, "(objectclass=configuration)", SearchScope.Base, SuitabilityVerifier.SaclRightProperties);
			return SuitabilityVerifier.SendAsyncLdapRequest(context.DCConnection, request, context);
		}

		private static void EndCheckSACLRight(Task<DirectoryResponse> task)
		{
			ArgumentValidator.ThrowIfTypeInvalid<SuitabilityVerifier.SuitabilityCheckContext>("Async Context", task.AsyncState);
			SuitabilityVerifier.SuitabilityCheckContext suitabilityCheckContext = (SuitabilityVerifier.SuitabilityCheckContext)task.AsyncState;
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)suitabilityCheckContext.GetHashCode(), "{0} - EndCheckSACLRight", suitabilityCheckContext.ServerFqdn);
			SearchResponse searchResponse = (SearchResponse)task.Result;
			if (searchResponse != null && searchResponse.Entries.Count > 0 && searchResponse.Entries[0].Attributes.Contains("ntSecurityDescriptor"))
			{
				suitabilityCheckContext.SuitabilityResult.IsSACLRightAvailable = true;
			}
			else
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_NO_SACL, null, new object[]
				{
					suitabilityCheckContext.ServerFqdn,
					suitabilityCheckContext.ServerFqdn
				});
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, bool>((long)suitabilityCheckContext.GetHashCode(), "{0} - CheckSACLRight. Value {1}", suitabilityCheckContext.ServerFqdn, suitabilityCheckContext.SuitabilityResult.IsSACLRightAvailable);
		}

		private static Task CheckSACLRight(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - CheckSACLRight", context.ServerFqdn);
			Task<DirectoryResponse> task = SuitabilityVerifier.StartCheckSACLRight(context);
			return task.ContinueWith(new Action<Task<DirectoryResponse>>(SuitabilityVerifier.EndCheckSACLRight), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		private static Task<DirectoryResponse> StartCheckCriticalData(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - StartCheckCriticalData", context.ServerFqdn);
			SearchRequest request = new SearchRequest(context.SuitabilityResult.ConfigNC, string.Format("(&(objectCategory=msExchExchangeServer)(cn={0}))", NativeHelpers.GetLocalComputerFqdn(true).Split(new char[]
			{
				'.'
			})[0]), SearchScope.Subtree, SuitabilityVerifier.CriticalDataProperties);
			return SuitabilityVerifier.SendAsyncLdapRequest(context.DCConnection, request, context);
		}

		private static void EndCheckCriticalData(Task<DirectoryResponse> task)
		{
			ArgumentValidator.ThrowIfTypeInvalid<SuitabilityVerifier.SuitabilityCheckContext>("Async Context", task.AsyncState);
			SuitabilityVerifier.SuitabilityCheckContext suitabilityCheckContext = (SuitabilityVerifier.SuitabilityCheckContext)task.AsyncState;
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)suitabilityCheckContext.GetHashCode(), "{0} - EndCheckCriticalData", suitabilityCheckContext.ServerFqdn);
			SearchResponse searchResponse = (SearchResponse)task.Result;
			if ((searchResponse != null & searchResponse.Entries.Count > 0) && searchResponse.Entries[0].Attributes.Contains("objectClass"))
			{
				suitabilityCheckContext.SuitabilityResult.IsCriticalDataAvailable = true;
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, bool>((long)suitabilityCheckContext.GetHashCode(), "{0} - CheckCriticalData. Value {1}", suitabilityCheckContext.ServerFqdn, suitabilityCheckContext.SuitabilityResult.IsCriticalDataAvailable);
		}

		private static Task CheckCriticalData(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - CheckCriticalData", context.ServerFqdn);
			Task<DirectoryResponse> task = SuitabilityVerifier.StartCheckCriticalData(context);
			return task.ContinueWith(new Action<Task<DirectoryResponse>>(SuitabilityVerifier.EndCheckCriticalData), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		private static Task<DirectoryResponse> StartCheckOperatingSystemSuitable(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - StartCheckOperatingSystemSuitable", context.ServerFqdn);
			SearchRequest request = new SearchRequest(context.SuitabilityResult.RootNC, string.Format("(&(objectClass=computer)(servicePrincipalName=HOST/{0}))", context.ServerFqdn.Trim()), SearchScope.Subtree, SuitabilityVerifier.OSSuitableProperties);
			return SuitabilityVerifier.SendAsyncLdapRequest(context.DCConnection, request, context);
		}

		private static void EndCheckOperatingSystemSuitable(Task<DirectoryResponse> task)
		{
			ArgumentValidator.ThrowIfTypeInvalid<SuitabilityVerifier.SuitabilityCheckContext>("Async Context", task.AsyncState);
			SuitabilityVerifier.SuitabilityCheckContext suitabilityCheckContext = (SuitabilityVerifier.SuitabilityCheckContext)task.AsyncState;
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)suitabilityCheckContext.GetHashCode(), "{0} - EndCheckOperatingSystemSuitable", suitabilityCheckContext.ServerFqdn);
			SearchResponse searchResponse = (SearchResponse)task.Result;
			if (searchResponse == null || searchResponse.Entries.Count == 0 || !searchResponse.Entries[0].Attributes.Contains("operatingSystemVersion"))
			{
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string, string>((long)suitabilityCheckContext.GetHashCode(), "{0} - CheckOperatingSystemSuitable. Could not retrieve all the necessary information from '{1}' to perform the suitability check", suitabilityCheckContext.ServerFqdn, suitabilityCheckContext.DCConnection.SessionOptions.HostName);
				throw new ADExternalException(DirectoryStrings.ErrorIsServerSuitableMissingComputerData(suitabilityCheckContext.ServerFqdn, suitabilityCheckContext.DCConnection.SessionOptions.HostName));
			}
			suitabilityCheckContext.OperatingSystemResponse = searchResponse;
			string osVersion = (string)suitabilityCheckContext.OperatingSystemResponse.Entries[0].Attributes["operatingSystemVersion"].GetValues(typeof(string))[0];
			string osServicePack = string.Empty;
			if (suitabilityCheckContext.OperatingSystemResponse.Entries[0].Attributes.Contains("operatingSystemServicePack"))
			{
				osServicePack = (string)suitabilityCheckContext.OperatingSystemResponse.Entries[0].Attributes["operatingSystemServicePack"].GetValues(typeof(string))[0];
			}
			suitabilityCheckContext.SuitabilityResult.IsOSVersionSuitable = SuitabilityVerifier.IsOSVersionSuitable(suitabilityCheckContext.ServerFqdn, osVersion, osServicePack);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, bool>((long)suitabilityCheckContext.GetHashCode(), "{0} - CheckOperatingSystemSuitable. Value {1}", suitabilityCheckContext.ServerFqdn, suitabilityCheckContext.SuitabilityResult.IsOSVersionSuitable);
			if (!suitabilityCheckContext.SuitabilityResult.IsOSVersionSuitable)
			{
				throw new ADServerNotSuitableException(DirectoryStrings.ErrorIsServerSuitableInvalidOSVersion(suitabilityCheckContext.DCConnection.SessionOptions.HostName, osVersion, osServicePack, "5.2 (3790)", "Service Pack 1"), suitabilityCheckContext.DCConnection.SessionOptions.HostName);
			}
		}

		private static Task CheckOperatingSystemSuitable(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - CheckOperatingSystemSuitable", context.ServerFqdn);
			Task<DirectoryResponse> task = SuitabilityVerifier.StartCheckOperatingSystemSuitable(context);
			return task.ContinueWith(new Action<Task<DirectoryResponse>>(SuitabilityVerifier.EndCheckOperatingSystemSuitable), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		private static Task<DirectoryResponse> StartCheckPDC(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - StartCheckPDC", context.ServerFqdn);
			SearchRequest request = new SearchRequest(context.SuitabilityResult.WritableNC, "(objectclass=domain)", SearchScope.Base, SuitabilityVerifier.PDCSuitabilityProperties);
			return SuitabilityVerifier.SendAsyncLdapRequest(context.DCConnection, request, context);
		}

		private static void EndCheckPDC(Task<DirectoryResponse> task)
		{
			ArgumentValidator.ThrowIfTypeInvalid<SuitabilityVerifier.SuitabilityCheckContext>("Async Context", task.AsyncState);
			SuitabilityVerifier.SuitabilityCheckContext suitabilityCheckContext = (SuitabilityVerifier.SuitabilityCheckContext)task.AsyncState;
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)suitabilityCheckContext.GetHashCode(), "{0} - EndCheckSACLRight", suitabilityCheckContext.ServerFqdn);
			SearchResponse searchResponse = (SearchResponse)task.Result;
			if (searchResponse == null || searchResponse.Entries.Count == 0 || !searchResponse.Entries[0].Attributes.Contains("fSMORoleOwner"))
			{
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string, string>((long)suitabilityCheckContext.GetHashCode(), "{0} - CheckPDC. Could not retrieve all the necessary information from '{1}' to perform the suitability check", suitabilityCheckContext.ServerFqdn, suitabilityCheckContext.DCConnection.SessionOptions.HostName);
				throw new ADExternalException(DirectoryStrings.ErrorIsServerSuitableMissingComputerData(suitabilityCheckContext.ServerFqdn, suitabilityCheckContext.DCConnection.SessionOptions.HostName));
			}
			string distinguishedName = (string)searchResponse.Entries[0].Attributes["fSMORoleOwner"].GetValues(typeof(string))[0];
			ADObjectId adobjectId = new ADObjectId(distinguishedName);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<int, string, ADObjectId>((long)suitabilityCheckContext.GetHashCode(), "{0} - Successfully got Fsmo role information {1}", suitabilityCheckContext.GetHashCode(), suitabilityCheckContext.ServerFqdn, adobjectId);
			if (adobjectId.Parent.Name.Equals(suitabilityCheckContext.ServerFqdn.Split(new char[]
			{
				'.'
			})[0], StringComparison.OrdinalIgnoreCase))
			{
				suitabilityCheckContext.SuitabilityResult.IsPDC = true;
			}
			else
			{
				suitabilityCheckContext.SuitabilityResult.IsPDC = false;
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, bool>((long)suitabilityCheckContext.GetHashCode(), "{0} - CheckPDC. Value {1}", suitabilityCheckContext.ServerFqdn, suitabilityCheckContext.SuitabilityResult.IsPDC);
		}

		private static Task CheckPDC(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - CheckPDC", context.ServerFqdn);
			Task<DirectoryResponse> task = SuitabilityVerifier.StartCheckPDC(context);
			return task.ContinueWith(new Action<Task<DirectoryResponse>>(SuitabilityVerifier.EndCheckPDC), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		private static Task<DirectoryResponse> StartReadDCMaintenanceModeAttributeSchema(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - StartReadDCMaintenanceModeAttributeSchema", context.ServerFqdn);
			SearchRequest request = new SearchRequest(string.Format("CN=ms-Exch-Provisioning-Flags,{0}", context.SuitabilityResult.SchemaNC), "(objectclass=*)", SearchScope.Base, new string[0]);
			return SuitabilityVerifier.SendAsyncLdapRequest(context.DCConnection, request, context);
		}

		private static void EndReadDCMaintenanceModeAttributeSchema(Task<DirectoryResponse> task)
		{
			ArgumentValidator.ThrowIfTypeInvalid<SuitabilityVerifier.SuitabilityCheckContext>("Async Context", task.AsyncState);
			SuitabilityVerifier.SuitabilityCheckContext suitabilityCheckContext = (SuitabilityVerifier.SuitabilityCheckContext)task.AsyncState;
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)suitabilityCheckContext.GetHashCode(), "{0} - EndReadDCMaintenanceModeAttributeSchema", suitabilityCheckContext.ServerFqdn);
			SearchResponse searchResponse = (SearchResponse)task.Result;
			bool flag = false;
			if (searchResponse != null && searchResponse.Entries.Count > 0)
			{
				flag = true;
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, string>((long)suitabilityCheckContext.GetHashCode(), "{0} - EndReadDCMaintenanceModeAttributeSchema. Schema attribute ms-Exch-Provisioning-Flags {1}found.", suitabilityCheckContext.ServerFqdn, flag ? string.Empty : " NOT ");
			Exception ex;
			if (!SuitabilityVerifier.TryCheckIsDCInMaintenanceMode(suitabilityCheckContext, flag, out ex))
			{
				throw ex;
			}
		}

		private static bool TryCheckIsDCInMaintenanceMode(SuitabilityVerifier.SuitabilityCheckContext context, bool isSchemaAttributePresent, out Exception exception)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			exception = null;
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string, bool>((long)context.GetHashCode(), "{0} - TryCheckIsDCInMaintenanceMode. isSchemaAttributePresent {1}", context.ServerFqdn, isSchemaAttributePresent);
			if (context.OperatingSystemResponse == null)
			{
				throw new ADExternalException(DirectoryStrings.ErrorIsServerSuitableMissingOperatingSystemResponse(context.ServerFqdn));
			}
			context.SuitabilityResult.IsInMM = true;
			if (context.OperatingSystemResponse.Entries[0].Attributes.Contains(SuitabilityVerifier.DcMaintenanceModeFlagAttr))
			{
				string s = (string)context.OperatingSystemResponse.Entries[0].Attributes[SuitabilityVerifier.DcMaintenanceModeFlagAttr].GetValues(typeof(string))[0];
				context.SuitabilityResult.IsInMM = (0 != int.Parse(s));
			}
			else if (!isSchemaAttributePresent)
			{
				context.SuitabilityResult.IsInMM = false;
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, bool>((long)context.GetHashCode(), "{0} - CheckIsDCInMaintenanceMode. Value {1}", context.ServerFqdn, context.SuitabilityResult.IsInMM);
			if (context.SuitabilityResult.IsInMM)
			{
				exception = new ServerInMMException(context.ServerFqdn)
				{
					ServerFqdn = context.ServerFqdn
				};
			}
			return !context.SuitabilityResult.IsInMM;
		}

		private static Task CheckIsDCInMaintenanceMode(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - CheckIsDCInMaintenanceMode", context.ServerFqdn);
			bool flag = SuitabilityVerifier.IsSetupOrTestContext();
			if (flag || (context.OperatingSystemResponse != null && context.OperatingSystemResponse.Entries[0].Attributes.Contains(SuitabilityVerifier.DcMaintenanceModeFlagAttr)))
			{
				TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>(context, TaskCreationOptions.AttachedToParent);
				Exception exception;
				if (SuitabilityVerifier.TryCheckIsDCInMaintenanceMode(context, !flag, out exception))
				{
					taskCompletionSource.TrySetResult(string.Empty);
				}
				else
				{
					taskCompletionSource.TrySetException(exception);
				}
				return taskCompletionSource.Task;
			}
			Task<DirectoryResponse> task = SuitabilityVerifier.StartReadDCMaintenanceModeAttributeSchema(context);
			return task.ContinueWith(new Action<Task<DirectoryResponse>>(SuitabilityVerifier.EndReadDCMaintenanceModeAttributeSchema), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		private static void CheckCreateConnection(SuitabilityVerifier.SuitabilityCheckContext context, NetworkCredential networkCredential)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - CheckCreateConnection", context.ServerFqdn);
			context.DCConnection = SuitabilityVerifier.CreateConnectionAndBind(context.ServerFqdn, networkCredential, (context.DCPort > 0) ? context.DCPort : 389);
			if (context.GCPort > 0 && (context.SuitabilityResult.IsReachableByTCPConnection & ADServerRole.GlobalCatalog) != ADServerRole.None)
			{
				PooledLdapConnection pooledLdapConnection = null;
				try
				{
					pooledLdapConnection = SuitabilityVerifier.CreateConnectionAndBind(context.ServerFqdn, networkCredential, context.GCPort);
					if (context.DCConnection != null && pooledLdapConnection != null)
					{
						context.SuitabilityResult.IsEnabled = true;
					}
					goto IL_B3;
				}
				finally
				{
					if (pooledLdapConnection != null)
					{
						pooledLdapConnection.Dispose();
					}
				}
			}
			if (context.DCConnection != null)
			{
				context.SuitabilityResult.IsEnabled = true;
			}
			IL_B3:
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug((long)context.GetHashCode(), "{0} - CheckCreateConnection. Value {1} . DC port {2} GC port {3}.", new object[]
			{
				context.ServerFqdn,
				context.SuitabilityResult.IsEnabled,
				context.DCPort,
				context.GCPort
			});
		}

		private static Task<DirectoryResponse> SendAsyncLdapRequest(PooledLdapConnection connection, SearchRequest request, SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("connection", connection);
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("context", context);
			Task<DirectoryResponse> task2 = Task.Factory.FromAsync<DirectoryRequest, TimeSpan, PartialResultProcessing, DirectoryResponse>(new Func<DirectoryRequest, TimeSpan, PartialResultProcessing, AsyncCallback, object, IAsyncResult>(connection.BeginSendRequest), new Func<IAsyncResult, DirectoryResponse>(connection.EndSendRequest), request, ConnectionPoolManager.ClientSideSearchTimeout, PartialResultProcessing.NoPartialResultSupport, context, TaskCreationOptions.AttachedToParent);
			TaskCompletionSource<DirectoryResponse> tcs = new TaskCompletionSource<DirectoryResponse>(context, TaskCreationOptions.AttachedToParent);
			task2.ContinueWith(delegate(Task<DirectoryResponse> task)
			{
				if (task.Status != TaskStatus.RanToCompletion)
				{
					Exception ex = (task.Exception != null) ? task.Exception.GetBaseException() : null;
					ExTraceGlobals.SuitabilityVerifierTracer.TraceError((long)context.GetHashCode(), "{0} Request failed on server '{1}'. DN {2} Filter {3} Scope {4}", new object[]
					{
						context.ServerFqdn,
						connection.SessionOptions.HostName,
						request.DistinguishedName,
						request.Filter,
						request.Scope
					});
					DirectoryException ex2 = (ex != null) ? (ex as DirectoryException) : null;
					if (ex2 == null)
					{
						tcs.SetException(new SuitabilityException(DirectoryStrings.SuitabilityExceptionLdapSearch(context.ServerFqdn, (ex != null) ? ex.Message : "Unknown"), context.ServerFqdn));
						return;
					}
					ADErrorRecord aderrorRecord = context.DCConnection.AnalyzeDirectoryError(ex2);
					SuitabilityVerifier.LogEventSyncFailed(context.ServerFqdn, (int)aderrorRecord.LdapError, aderrorRecord.Message, request);
					if (aderrorRecord.LdapError != LdapError.InvalidCredentials)
					{
						tcs.SetException(new SuitabilityDirectoryException(context.ServerFqdn, (int)aderrorRecord.LdapError, aderrorRecord.Message, aderrorRecord.InnerException)
						{
							ServerFqdn = context.ServerFqdn
						});
						return;
					}
					if (aderrorRecord.HandlingType == HandlingType.Throw)
					{
						tcs.SetException(aderrorRecord.InnerException);
						return;
					}
				}
				else
				{
					tcs.SetResult(task.Result);
				}
			}, TaskContinuationOptions.AttachedToParent);
			return tcs.Task;
		}

		private static void CheckIsSyncronized(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - CheckIsSyncronized.", context.ServerFqdn);
			if (context.DefaultNCResponse == null)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED, null, new object[]
				{
					context.ServerFqdn,
					DirectoryStrings.ErrorIsServerSuitableMissingDefaultNamingContext(context.ServerFqdn) + " (in CheckIsSyncronized)"
				});
				throw new ADExternalException(DirectoryStrings.ErrorIsServerSuitableMissingDefaultNamingContext(context.ServerFqdn));
			}
			if (context.DefaultNCResponse.Entries[0].Attributes.Contains("isSynchronized"))
			{
				context.SuitabilityResult.IsSynchronized = (ADServerRole.DomainController | ADServerRole.ConfigurationDomainController);
			}
			if (context.DefaultNCResponse.Entries[0].Attributes.Contains("isGlobalCatalogReady"))
			{
				context.SuitabilityResult.IsSynchronized |= ADServerRole.GlobalCatalog;
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, ADServerRole>((long)context.GetHashCode(), "{0} - CheckIsSyncronized Value {1}", context.ServerFqdn, context.SuitabilityResult.IsSynchronized);
		}

		private static void CheckNetLogon(SuitabilityVerifier.SuitabilityCheckContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ExTraceGlobals.SuitabilityVerifierTracer.TraceFunction<string>((long)context.GetHashCode(), "{0} - CheckNetLogon.", context.ServerFqdn);
			int tickCount = Environment.TickCount;
			try
			{
				SafeDomainControllerInfoHandle safeDomainControllerInfoHandle;
				NativeMethods.DsGetDcName(context.ServerFqdn, null, null, NativeMethods.DsGetDCNameFlags.ReturnDNSName, out safeDomainControllerInfoHandle);
				string a = string.Empty;
				if (safeDomainControllerInfoHandle != null && !string.IsNullOrEmpty(safeDomainControllerInfoHandle.GetDomainControllerInfo().DomainControllerName))
				{
					a = safeDomainControllerInfoHandle.GetDomainControllerInfo().DomainControllerName.Replace("\\", string.Empty);
				}
				if (string.Equals(a, context.ServerFqdn, StringComparison.OrdinalIgnoreCase))
				{
					context.SuitabilityResult.IsNetlogonAllowed |= (ADServerRole.DomainController | ADServerRole.ConfigurationDomainController);
					if ((safeDomainControllerInfoHandle.GetDomainControllerInfo().Flags & 4U) > 0U)
					{
						context.SuitabilityResult.IsNetlogonAllowed |= ADServerRole.GlobalCatalog;
					}
					ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, string>((long)context.GetHashCode(), "{0} - Successfully called DsGetDcName() against server '{0}' to perform the suitability check for NetLogon. DomainController info: {1}", context.ServerFqdn, safeDomainControllerInfoHandle.GetDomainControllerInfo().DomainControllerName);
				}
			}
			catch (Exception ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED, null, new object[]
				{
					context.ServerFqdn,
					ex.ToString() + " (in CheckNetLogon)"
				});
				ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string, Exception>((long)context.GetHashCode(), "Failed to call DsGetDcName() against server '{0}' to perform the suitability check for NetLogon. Error: {1}", context.ServerFqdn, ex);
			}
			finally
			{
				ADProviderPerf.UpdateDCCounter(context.HostName, Counter.DCTimeNetlogon, UpdateType.Update, Convert.ToUInt32(Globals.GetTickDifference(tickCount, Environment.TickCount)));
			}
			ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, ADServerRole>((long)context.GetHashCode(), "{0} - CheckNetLogon. Value {1}", context.ServerFqdn, context.SuitabilityResult.IsNetlogonAllowed);
		}

		private static void ThrowException(Exception exception, LocalizedString errorMessage, string serverFqdn)
		{
			if (exception != null)
			{
				if (exception is ArgumentNullException)
				{
					throw exception;
				}
				if (exception is ArgumentException)
				{
					throw exception;
				}
				if (exception is ObjectDisposedException)
				{
					throw exception;
				}
				if (exception is NotSupportedException)
				{
					throw exception;
				}
			}
			if (exception == null)
			{
				throw new ADServerNotSuitableException(errorMessage, serverFqdn);
			}
			throw new SuitabilityException(errorMessage, serverFqdn);
		}

		private static string GetDomainNameFromServerFqdn(string serverFqdn)
		{
			string result = null;
			int num = serverFqdn.IndexOf('.');
			if (num > 0)
			{
				result = serverFqdn.Substring(num);
			}
			return result;
		}

		private static bool IsForestLocal(string serverFqdn)
		{
			string localForestFqdn = TopologyProvider.LocalForestFqdn;
			string text = null;
			if (Globals.IsMicrosoftHostedOnly)
			{
				text = SuitabilityVerifier.GetDomainNameFromServerFqdn(serverFqdn);
			}
			else
			{
				try
				{
					text = NativeHelpers.GetDomainForestNameFromServer(serverFqdn);
				}
				catch (Exception arg)
				{
					ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, Exception>(0L, "Failed to call GetDomainForestNameFromServer() against server '{0}'. Error: {1}", serverFqdn, arg);
					if (string.IsNullOrEmpty(text))
					{
						text = SuitabilityVerifier.GetDomainNameFromServerFqdn(serverFqdn);
					}
				}
			}
			return !string.IsNullOrEmpty(text) && text.Equals(localForestFqdn, StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsServerSuitableIgnoreExceptions(string fqdn, bool isGlobalCatalog, NetworkCredential credential, out string writableNC, out string siteName, out LocalizedString errorMessage)
		{
			Exception ex;
			if (SuitabilityVerifier.InternalTryCheckIsServerSuitable(fqdn, isGlobalCatalog, credential, out writableNC, out siteName, out ex))
			{
				errorMessage = LocalizedString.Empty;
				return true;
			}
			errorMessage = ((ex != null) ? new LocalizedString(ex.ToString()) : LocalizedString.Empty);
			return false;
		}

		internal static bool IsServerSuitableIgnoreExceptions(string fqdn, bool isGlobalCatalog, NetworkCredential credential, out string writableNC, out LocalizedString errorMessage)
		{
			string text;
			Exception ex;
			if (SuitabilityVerifier.InternalTryCheckIsServerSuitable(fqdn, isGlobalCatalog, credential, out writableNC, out text, out ex))
			{
				errorMessage = LocalizedString.Empty;
				return true;
			}
			errorMessage = ((ex != null) ? new LocalizedString(ex.ToString()) : LocalizedString.Empty);
			return false;
		}

		internal static bool IsAdamServerSuitable(string fqdn, int portNumber, NetworkCredential credential, ref LocalizedString errorMessage)
		{
			using (SuitabilityVerifier.CreateConnectionAndBind(fqdn, credential, portNumber))
			{
			}
			return true;
		}

		internal static void CheckIsServerSuitable(string fqdn, bool isGlobalCatalog, NetworkCredential credentials, out string writableNC)
		{
			string text;
			Exception ex;
			if (!SuitabilityVerifier.InternalTryCheckIsServerSuitable(fqdn, isGlobalCatalog, credentials, out writableNC, out text, out ex))
			{
				throw ex;
			}
		}

		internal static Task CheckAndUpdateServerSuitabilities(DirectoryServer server, CancellationToken cancellationToken, bool isInitialDiscovery, bool allowPreW2KSP3DC, bool isPDCCheckEnabled, bool disableNetLogonCheck)
		{
			SuitabilityVerifier.SuitabilityCheckContext context = new SuitabilityVerifier.SuitabilityCheckContext(server, allowPreW2KSP3DC, isPDCCheckEnabled, disableNetLogonCheck);
			Task task = SuitabilityVerifier.PerformServerSuitabilities(context, cancellationToken, isInitialDiscovery);
			return task.ContinueWith(delegate(Task t)
			{
				context.CloseDCConnection();
				if (t.Exception != null)
				{
					ExTraceGlobals.SuitabilityVerifierTracer.TraceError<string, string>((long)context.GetHashCode(), "{0} Final Error: {1}", context.ServerFqdn, t.Exception.Flatten().ToString());
				}
				ExTraceGlobals.SuitabilityVerifierTracer.TraceDebug<string, SuitabilityCheckResult>((long)context.GetHashCode(), "{0} Suitabilities for server {1}", context.ServerFqdn, context.SuitabilityResult);
			}, cancellationToken);
		}

		internal static Task PerformServerSuitabilities(SuitabilityVerifier.SuitabilityCheckContext context, CancellationToken cancellationToken, bool isInitialDiscovery)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("cancellationToken", cancellationToken);
			return Task.Factory.Iterate(SuitabilityVerifier.SuitabilityChecks(context, null, true, isInitialDiscovery), cancellationToken);
		}

		private const string DefaultNamingContextAttr = "defaultNamingContext";

		private const string ConfigurationNamingContextAttr = "configurationNamingContext";

		private const string SchemaNamingContextAttr = "schemaNamingContext";

		private const string SupportedCapabilitiesAttr = "supportedCapabilities";

		private const string OperatingSystemLdapAttr = "operatingSystemVersion";

		private const string OperatingSystemServicePackLdapAttr = "operatingSystemServicePack";

		private const string SaclRightAttr = "ntSecurityDescriptor";

		private const string CriticalDataAttr = "objectClass";

		private const string IsSynchronizedAttr = "isSynchronized";

		private const string IsGlobalCatalogReadyAttr = "isGlobalCatalogReady";

		private const string FSMORoleAttr = "fSMORoleOwner";

		private const string ServerNameAttr = "serverName";

		private const string DefaultNamingContextFilter = "(objectclass=*)";

		private const string ConfigurationNamingContextFilter = "(objectclass=configuration)";

		private const string DomainControllerFilter = "(&(objectClass=computer)(servicePrincipalName=HOST/{0}))";

		private const string CriticalDataFilter = "(&(objectCategory=msExchExchangeServer)(cn={0}))";

		private const string DomainFilter = "(objectclass=domain)";

		private const string ActiveDirectoryPartialSecrets = "1.2.840.113556.1.4.1920";

		private const string DCMaintenanceModeAttributeDNFormat = "CN=ms-Exch-Provisioning-Flags,{0}";

		private const int MaxRetryAttempts = 3;

		private const int RetrySleep = 3000;

		private const string MinOperatingSystemVersion = "5.2 (3790)";

		private const string MinOperatingSystemServicePack = "Service Pack 1";

		private const string OSVersionPattern = "^(\\d+)\\.(\\d+) \\(\\d+\\)$";

		private const int MinMajorVersion = 5;

		private const int MinMinorVersion = 2;

		private const uint GcFlag = 4U;

		private static readonly string DcMaintenanceModeFlagAttr = SharedPropertyDefinitions.ProvisioningFlags.LdapDisplayName;

		private static readonly string[] DefaultNamingContextProperties = new string[]
		{
			"isSynchronized",
			"isGlobalCatalogReady",
			"configurationNamingContext",
			"defaultNamingContext",
			"schemaNamingContext",
			"supportedCapabilities",
			"serverName"
		};

		private static readonly string[] SaclRightProperties = new string[]
		{
			"ntSecurityDescriptor"
		};

		private static readonly string[] CriticalDataProperties = new string[]
		{
			"objectClass"
		};

		private static readonly string[] OSSuitableProperties = new string[]
		{
			"operatingSystemVersion",
			"operatingSystemServicePack",
			SuitabilityVerifier.DcMaintenanceModeFlagAttr
		};

		private static readonly string[] PDCSuitabilityProperties = new string[]
		{
			"fSMORoleOwner"
		};

		internal class SuitabilityCheckContext
		{
			internal SuitabilityCheckContext(string serverFqdn, bool isGlobalCatalog, bool allowPreW2KSP3DC = false, bool isPDCCheckEnabled = false, bool disableNetLogonCheck = false)
			{
				if (string.IsNullOrEmpty(serverFqdn))
				{
					throw new ArgumentNullException("serverFqdn");
				}
				this.ServerFqdn = serverFqdn.Trim();
				this.DCPort = 389;
				this.GCPort = (isGlobalCatalog ? 3268 : -1);
				this.AllowPreW2KSP3DC = allowPreW2KSP3DC;
				this.IsPDCCheckEnabled = isPDCCheckEnabled;
				this.DisableNetLogonCheck = disableNetLogonCheck;
				this.SuitabilityResult = new SuitabilityCheckResult();
			}

			internal SuitabilityCheckContext(DirectoryServer dsServer, bool allowPreW2KSP3DC = false, bool isPDCCheckEnabled = false, bool disableNetLogonCheck = false) : this(dsServer.DnsName, dsServer.IsGC, allowPreW2KSP3DC, isPDCCheckEnabled, disableNetLogonCheck)
			{
				if (dsServer == null)
				{
					throw new ArgumentNullException("dsServer");
				}
				this.SuitabilityResult = dsServer.SuitabilityResult;
			}

			public string ServerFqdn { get; private set; }

			public SuitabilityCheckResult SuitabilityResult { get; private set; }

			public int GCPort { get; private set; }

			public int DCPort { get; private set; }

			public string HostName { get; set; }

			public PooledLdapConnection DCConnection { get; set; }

			public SearchResponse DefaultNCResponse { get; set; }

			public SearchResponse OperatingSystemResponse { get; set; }

			public IPAddress[] IPAddresses { get; set; }

			public bool AllowPreW2KSP3DC { get; private set; }

			public bool IsPDCCheckEnabled { get; private set; }

			public bool DisableNetLogonCheck { get; private set; }

			public string SiteName
			{
				get
				{
					this.PopulateSiteName();
					return this.siteName;
				}
			}

			public void CloseDCConnection()
			{
				if (this.DCConnection != null)
				{
					this.DCConnection.Dispose();
					this.DCConnection = null;
				}
			}

			private void PopulateSiteName()
			{
				if (this.DefaultNCResponse == null || this.DefaultNCResponse.Entries.Count == 0)
				{
					return;
				}
				if (!string.IsNullOrEmpty(this.siteName))
				{
					return;
				}
				if (this.DefaultNCResponse.Entries[0].Attributes.Contains("serverName"))
				{
					string text = (string)this.DefaultNCResponse.Entries[0].Attributes["serverName"].GetValues(typeof(string))[0];
					if (!string.IsNullOrEmpty(text))
					{
						ADObjectId adobjectId = new ADObjectId(text);
						if (adobjectId.Parent != null && adobjectId.Parent.Parent != null)
						{
							this.siteName = adobjectId.Parent.Parent.Name;
						}
					}
				}
			}

			private string siteName;
		}
	}
}
