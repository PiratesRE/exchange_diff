using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public sealed class SecurityContextManager
	{
		private SecurityContextManager()
		{
		}

		private static SecurityContextManager._CriticalConsistencyBlock Critical()
		{
			return new SecurityContextManager._CriticalConsistencyBlock
			{
				LockObject = SecurityContextManager.lockObject
			};
		}

		internal static Dictionary<SecurityContextKey, CountedClientSecurityContext> ContextDictionary
		{
			get
			{
				return SecurityContextManager.contextDictionary;
			}
		}

		internal static void Initialize()
		{
			if (SecurityContextManager.lockObject == null)
			{
				SecurityContextManager.contextDictionary = new Dictionary<SecurityContextKey, CountedClientSecurityContext>(500);
				SecurityContextManager.lockObject = new object();
				SecurityContextManager.nextCookie = 0UL;
				SecurityContextManager.queryTarget = SecurityContextsQueryTarget.Create();
			}
		}

		internal static void Terminate()
		{
		}

		internal static CountedClientSecurityContext StartRPCUse(SecurityContextKey securityContextKey, ref ClientSecurityContext clientSecurityContextOnRPCBegin)
		{
			Trace securityContextManagerTracer = ExTraceGlobals.SecurityContextManagerTracer;
			bool flag = securityContextManagerTracer.IsTraceEnabled(TraceType.DebugTrace);
			CountedClientSecurityContext countedClientSecurityContext = null;
			SecurityContextKey securityContextKey2 = (clientSecurityContextOnRPCBegin != null) ? new SecurityContextKey(clientSecurityContextOnRPCBegin) : null;
			CountedClientSecurityContext result;
			using (SecurityContextManager._CriticalConsistencyBlock criticalConsistencyBlock = SecurityContextManager.Critical())
			{
				using (LockManager.Lock(SecurityContextManager.lockObject))
				{
					CountedClientSecurityContext countedClientSecurityContext2;
					if (securityContextKey != null)
					{
						bool assertCondition = SecurityContextManager.contextDictionary.TryGetValue(securityContextKey, out countedClientSecurityContext2);
						Globals.AssertRetail(assertCondition, "incorrect Session usage counter");
						Globals.AssertRetail(countedClientSecurityContext2 != null, "null entry in dictionary");
						Globals.AssertRetail(countedClientSecurityContext2.ActiveSessions > 0, "invalid session count on entry in dictionary");
						countedClientSecurityContext2.ActiveRPCThreads++;
						if (flag)
						{
							securityContextManagerTracer.TraceDebug<string>(0L, "StartRPCUse: 1. key lookup: found context {0}", countedClientSecurityContext2.ToString());
						}
						countedClientSecurityContext = countedClientSecurityContext2;
						Globals.AssertRetail(countedClientSecurityContext != null, "cannot return null");
						criticalConsistencyBlock.Success();
						return countedClientSecurityContext;
					}
					bool flag2 = false;
					if (!SecurityContextManager.contextDictionary.TryGetValue(securityContextKey2, out countedClientSecurityContext2))
					{
						SecurityContextManager.nextCookie += 1UL;
						countedClientSecurityContext = new CountedClientSecurityContext(clientSecurityContextOnRPCBegin, securityContextKey2, SecurityContextManager.nextCookie);
						SecurityContextManager.contextDictionary[countedClientSecurityContext.SecurityContextKey] = countedClientSecurityContext;
						flag2 = true;
						clientSecurityContextOnRPCBegin = null;
					}
					else if (!countedClientSecurityContext2.IsStale())
					{
						countedClientSecurityContext = countedClientSecurityContext2;
						countedClientSecurityContext2.ActiveSessions++;
						countedClientSecurityContext2.ActiveRPCThreads++;
						flag2 = true;
						clientSecurityContextOnRPCBegin.Dispose();
						clientSecurityContextOnRPCBegin = null;
					}
					if (flag2)
					{
						if (flag)
						{
							securityContextManagerTracer.TraceDebug<string>(0L, "StartRPCUse: 1. Add context, non-stale: context {0}", countedClientSecurityContext.ToString());
						}
						Globals.AssertRetail(countedClientSecurityContext != null, "cannot return null");
						criticalConsistencyBlock.Success();
						return countedClientSecurityContext;
					}
					SecurityContextManager.nextCookie += 1UL;
					countedClientSecurityContext = new CountedClientSecurityContext(clientSecurityContextOnRPCBegin, securityContextKey2, SecurityContextManager.nextCookie);
					countedClientSecurityContext.ActiveSessions = countedClientSecurityContext2.ActiveSessions + 1;
					countedClientSecurityContext2.ActiveSessions = countedClientSecurityContext2.ActiveRPCThreads;
					countedClientSecurityContext2.MarkedAsStale = true;
					if (flag)
					{
						securityContextManagerTracer.TraceDebug<string>(0L, "StartRPCUse: 1. Add context, stale context, was replaced {0}", countedClientSecurityContext2.ToString());
					}
					if (countedClientSecurityContext2.ActiveSessions == 0)
					{
						countedClientSecurityContext2.Dispose();
					}
					SecurityContextManager.contextDictionary[countedClientSecurityContext.SecurityContextKey] = countedClientSecurityContext;
					clientSecurityContextOnRPCBegin = null;
					if (flag)
					{
						securityContextManagerTracer.TraceDebug<string>(0L, "StartRPCUse: 1. Add context, new context {0}", countedClientSecurityContext.ToString());
					}
				}
				Globals.AssertRetail(countedClientSecurityContext != null, "cannot return null");
				criticalConsistencyBlock.Success();
				result = countedClientSecurityContext;
			}
			return result;
		}

		internal static void EndRPCUse(ref CountedClientSecurityContext countedSecurityContextOnRPCEnd, bool isDisconnect)
		{
			Trace securityContextManagerTracer = ExTraceGlobals.SecurityContextManagerTracer;
			bool flag = securityContextManagerTracer.IsTraceEnabled(TraceType.DebugTrace);
			using (SecurityContextManager._CriticalConsistencyBlock criticalConsistencyBlock = SecurityContextManager.Critical())
			{
				using (LockManager.Lock(SecurityContextManager.lockObject))
				{
					Globals.AssertRetail(countedSecurityContextOnRPCEnd.ActiveRPCThreads > 0, "incorrect RPC usage counter");
					countedSecurityContextOnRPCEnd.ActiveRPCThreads--;
					Globals.AssertRetail(countedSecurityContextOnRPCEnd.ActiveSessions > 0, "incorrect Session usage counter");
					if (isDisconnect)
					{
						countedSecurityContextOnRPCEnd.ActiveSessions--;
					}
					if (flag)
					{
						securityContextManagerTracer.TraceDebug<bool, string>(0L, "ENDRPCUSE: 1. After updating counters with isDisconnect={0}, Context {1}", isDisconnect, countedSecurityContextOnRPCEnd.ToString());
					}
					bool flag2 = false;
					CountedClientSecurityContext objB;
					if (SecurityContextManager.contextDictionary.TryGetValue(countedSecurityContextOnRPCEnd.SecurityContextKey, out objB))
					{
						flag2 = object.ReferenceEquals(countedSecurityContextOnRPCEnd, objB);
					}
					if (!flag2)
					{
						Globals.AssertRetail(countedSecurityContextOnRPCEnd.MarkedAsStale, "non stale context, not in dictionary");
						if (flag)
						{
							securityContextManagerTracer.TraceDebug<bool, string>(0L, "ENDRPCUSE: 2. InDictionary=true; disposing?={0} Context {1}", 0 == countedSecurityContextOnRPCEnd.ActiveRPCThreads, countedSecurityContextOnRPCEnd.ToString());
						}
						if (countedSecurityContextOnRPCEnd.ActiveRPCThreads == 0)
						{
							countedSecurityContextOnRPCEnd.Dispose();
							countedSecurityContextOnRPCEnd = null;
						}
					}
					else
					{
						if (flag)
						{
							securityContextManagerTracer.TraceDebug<bool, string>(0L, "ENDRPCUSE: 2. InDictionary=false; disposing?={0} Context {1}", 0 == countedSecurityContextOnRPCEnd.ActiveSessions, countedSecurityContextOnRPCEnd.ToString());
						}
						if (countedSecurityContextOnRPCEnd.ActiveSessions == 0)
						{
							Globals.AssertRetail(countedSecurityContextOnRPCEnd.ActiveRPCThreads == 0, "incorrect RPC usage counter");
							SecurityContextManager.contextDictionary.Remove(countedSecurityContextOnRPCEnd.SecurityContextKey);
							countedSecurityContextOnRPCEnd.Dispose();
							countedSecurityContextOnRPCEnd = null;
						}
					}
				}
				criticalConsistencyBlock.Success();
			}
		}

		internal static long GetCounter(ClientSecurityContext securityContext)
		{
			long num = 0L;
			long result;
			using (SecurityContextManager._CriticalConsistencyBlock criticalConsistencyBlock = SecurityContextManager.Critical())
			{
				using (LockManager.Lock(SecurityContextManager.lockObject))
				{
					foreach (CountedClientSecurityContext countedClientSecurityContext in SecurityContextManager.contextDictionary.Values)
					{
						if (countedClientSecurityContext.SecurityContext.UserSid.Equals(securityContext.UserSid))
						{
							num = (long)countedClientSecurityContext.ActiveSessions;
							break;
						}
					}
				}
				criticalConsistencyBlock.Success();
				result = num;
			}
			return result;
		}

		internal static IEnumerable<SecurityContextKey> GetKeysInDictionary()
		{
			IEnumerable<SecurityContextKey> result;
			using (LockManager.Lock(SecurityContextManager.lockObject))
			{
				SecurityContextKey[] array = new SecurityContextKey[SecurityContextManager.contextDictionary.Count];
				SecurityContextManager.contextDictionary.Keys.CopyTo(array, 0);
				result = array;
			}
			return result;
		}

		internal static CountedClientSecurityContext GetValueForKey(SecurityContextKey key)
		{
			CountedClientSecurityContext result;
			using (LockManager.Lock(SecurityContextManager.lockObject))
			{
				CountedClientSecurityContext countedClientSecurityContext;
				if (SecurityContextManager.contextDictionary.TryGetValue(key, out countedClientSecurityContext))
				{
					result = countedClientSecurityContext;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private static Dictionary<SecurityContextKey, CountedClientSecurityContext> contextDictionary;

		private static object lockObject;

		private static ulong nextCookie;

		private static SecurityContextsQueryTarget queryTarget;

		private struct _CriticalConsistencyBlock : IDisposable
		{
			public void Dispose()
			{
				if (this.LockObject != null)
				{
					Globals.AssertRetail(false, "forced crash in SecurityContextManager");
				}
			}

			public void Success()
			{
				this.LockObject = null;
			}

			public object LockObject;
		}
	}
}
