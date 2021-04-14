using System;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal abstract class RbacSession : RbacPrincipal, IRbacSession, IPrincipal, IIdentity
	{
		protected RbacSession(RbacContext context, SessionPerformanceCounters sessionPerfCounters, EsoSessionPerformanceCounters esoSessionPerfCounters) : base(context.Roles, context.Settings.CacheKey)
		{
			this.Context = context;
			this.Settings = context.Settings;
			this.sessionPerfCounters = (context.Settings.IsExplicitSignOn ? esoSessionPerfCounters : sessionPerfCounters);
		}

		internal RbacSettings Settings { get; private set; }

		internal RbacContext Context { get; private set; }

		public override void SetCurrentThreadPrincipal()
		{
			base.SetCurrentThreadPrincipal();
			if (base.UserCulture != null)
			{
				Thread.CurrentThread.CurrentCulture = base.UserCulture;
				Thread.CurrentThread.CurrentUICulture = base.UserCulture;
			}
		}

		public virtual void Initialize()
		{
			this.SetCurrentThreadPrincipal();
		}

		public virtual void SessionStart()
		{
			ExTraceGlobals.RBACTracer.TraceInformation<string>(0, 0L, "Starting RBAC session for {0}", base.NameForEventLog);
			this.WriteInitializationLog();
			RbacSession.totalSessionsCounter.Increment();
			this.sessionPerfCounters.IncreaseSessionCounter();
		}

		public virtual void SessionEnd()
		{
			this.sessionPerfCounters.DecreaseSessionCounter();
			RbacSession.totalSessionsCounter.Decrement();
			ExTraceGlobals.RBACTracer.TraceInformation<string>(0, 0L, "Ending RBAC session for {0}", base.NameForEventLog);
		}

		public virtual void RequestReceived()
		{
			this.sessionPerfCounters.IncreaseRequestCounter();
			ExTraceGlobals.RBACTracer.TraceInformation<string>(0, 0L, "Request received from {0}", base.NameForEventLog);
			this.SetCurrentThreadPrincipal();
		}

		public virtual void RequestCompleted()
		{
			this.sessionPerfCounters.DecreaseRequestCounter();
			ExTraceGlobals.RBACTracer.TraceInformation<string>(0, 0L, "Request completed for {0}", base.NameForEventLog);
			base.RbacConfiguration.TroubleshootingContext.TraceOperationCompletedAndUpdateContext();
		}

		protected abstract void WriteInitializationLog();

		private static PerfCounterGroup totalSessionsCounter = new PerfCounterGroup(EcpPerfCounters.RbacSessions, EcpPerfCounters.RbacSessionsPeak, EcpPerfCounters.RbacSessionsTotal);

		private SessionPerformanceCounters sessionPerfCounters;

		public abstract class Factory
		{
			protected Factory(RbacContext context)
			{
				this.Context = context;
			}

			private protected RbacContext Context { protected get; private set; }

			protected RbacSettings Settings
			{
				get
				{
					return this.Context.Settings;
				}
			}

			protected abstract bool CanCreateSession();

			protected abstract RbacSession CreateNewSession();

			public RbacSession CreateSession()
			{
				ExTraceGlobals.RBACTracer.TraceInformation<RbacSession.Factory, string>(0, 0L, "Testing if {0} can create a session for {1}.", this, this.Settings.UserName);
				if (this.CanCreateSession())
				{
					ExTraceGlobals.RBACTracer.TraceInformation<RbacSession.Factory, string>(0, 0L, "{0} accepted the request to create a session for {1}.", this, this.Settings.UserName);
					RbacSession rbacSession = this.CreateNewSession();
					ExTraceGlobals.RBACTracer.TraceInformation<RbacSession, string>(0, 0L, "Initializing {0} session for {1}.", rbacSession, this.Settings.UserName);
					rbacSession.Initialize();
					return rbacSession;
				}
				ExTraceGlobals.RBACTracer.TraceInformation<RbacSession.Factory, string>(0, 0L, "{0} declined the request to create a session for {1}.", this, this.Settings.UserName);
				return null;
			}
		}
	}
}
