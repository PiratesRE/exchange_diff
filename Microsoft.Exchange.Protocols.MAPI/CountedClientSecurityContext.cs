using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class CountedClientSecurityContext : DisposableBase
	{
		public DateTime WhenCreated { get; private set; }

		public bool MarkedAsStale { get; set; }

		public ulong Cookie { get; private set; }

		public int ActiveSessions { get; internal set; }

		public int ActiveRPCThreads { get; internal set; }

		public ClientSecurityContext SecurityContext { get; private set; }

		public SecurityContextKey SecurityContextKey { get; private set; }

		public SecurityIdentifier UserSid
		{
			get
			{
				return this.SecurityContext.UserSid;
			}
		}

		public CountedClientSecurityContext(ClientSecurityContext clientSecurityContext, SecurityContextKey securityContextKey, ulong cookie)
		{
			Globals.AssertRetail(clientSecurityContext != null, "clientSecurityContext can't be null.");
			Globals.AssertRetail(securityContextKey != null, "securityContextKey can't be null.");
			this.WhenCreated = DateTime.UtcNow;
			this.MarkedAsStale = false;
			this.Cookie = cookie;
			this.ActiveSessions = 1;
			this.ActiveRPCThreads = 1;
			this.SecurityContext = clientSecurityContext;
			this.SecurityContextKey = securityContextKey;
		}

		public bool IsStale()
		{
			if (this.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid) || this.UserSid.IsWellKnown(WellKnownSidType.NetworkServiceSid))
			{
				return false;
			}
			TimeSpan t = DateTime.UtcNow - this.WhenCreated;
			return t > TimeSpan.FromMinutes(5.0);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CountedClientSecurityContext>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			Trace securityContextManagerTracer = ExTraceGlobals.SecurityContextManagerTracer;
			bool flag = securityContextManagerTracer.IsTraceEnabled(TraceType.DebugTrace);
			if (calledFromDispose && this.SecurityContext != null)
			{
				if (flag)
				{
					securityContextManagerTracer.TraceDebug<string>(0L, "INTERNALDISPOSE: disposing context{0}", this.ToString());
				}
				this.SecurityContext.Dispose();
			}
		}

		public override string ToString()
		{
			return string.Format("\nSID:{0} CK:{1} CTR:{2} RPC:{3} SI:{4} CREATED:{5}", new object[]
			{
				this.SecurityContext.UserSid,
				this.Cookie,
				this.ActiveSessions,
				this.ActiveRPCThreads,
				!this.MarkedAsStale,
				this.WhenCreated
			});
		}

		internal void MarkStaleForTest()
		{
			this.WhenCreated = DateTime.UtcNow - TimeSpan.FromMinutes(10.0);
		}

		private const ushort StaleTTLInMinutes = 5;
	}
}
