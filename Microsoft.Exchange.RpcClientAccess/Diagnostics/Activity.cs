using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Activity
	{
		private Activity(long traceId)
		{
			this.traceId = traceId;
		}

		public static Activity Current
		{
			get
			{
				return Activity.currentActivity;
			}
		}

		public static bool IsForeground
		{
			get
			{
				return Activity.foregroundActivity != null && Activity.foregroundActivity.Value;
			}
		}

		public static bool AllowImplicit
		{
			get
			{
				return Activity.allowImplicit;
			}
			set
			{
				Activity.allowImplicit = value;
			}
		}

		public static long TraceId
		{
			get
			{
				Activity activity = Activity.Current;
				if (activity == null)
				{
					return 0L;
				}
				return activity.traceId;
			}
		}

		public IStandardBudget Budget
		{
			get
			{
				return this.budget;
			}
		}

		public ProtocolLogSession ProtocolLogSession
		{
			get
			{
				return this.logSession;
			}
		}

		private bool IsCurrent
		{
			get
			{
				return Activity.currentActivity == this;
			}
		}

		public static Activity Create(long activityId)
		{
			return new Activity(activityId);
		}

		public void RegisterWatsonReportAction(WatsonReportAction reportAction)
		{
			lock (this.watsonReportActions)
			{
				this.watsonReportActions.Add(reportAction);
			}
			this.InternalRegisterWatsonReportAction(reportAction);
		}

		public void UnregisterWatsonReportAction(WatsonReportAction reportAction)
		{
			lock (this.watsonReportActions)
			{
				this.watsonReportActions.Remove(reportAction);
			}
			this.InternalUnregisterWatsonReportAction(reportAction);
		}

		public void RegisterBudget(IStandardBudget budget)
		{
			this.budget = budget;
		}

		private void OnResume()
		{
			if (this.logSession != null)
			{
				this.logSession.OnClientActivityResume();
			}
			this.RegisterWatsonReportActionsForCurrentThread();
		}

		private void OnPause()
		{
			if (this.logSession != null)
			{
				this.logSession.OnClientActivityPause();
			}
			this.UnregisterWatsonReportActionsForCurrentThread();
		}

		private void InternalRegisterWatsonReportAction(WatsonReportAction reportAction)
		{
			if (this.IsCurrent)
			{
				ExWatson.RegisterReportAction(reportAction, WatsonActionScope.Thread);
			}
		}

		private void InternalUnregisterWatsonReportAction(WatsonReportAction reportAction)
		{
			if (this.IsCurrent)
			{
				ExWatson.UnregisterReportAction(reportAction, WatsonActionScope.Thread);
			}
		}

		private void RegisterWatsonReportActionsForCurrentThread()
		{
			WatsonReportAction[] array;
			lock (this.watsonReportActions)
			{
				array = this.watsonReportActions.ToArray<WatsonReportAction>();
			}
			foreach (WatsonReportAction reportAction in array)
			{
				this.InternalRegisterWatsonReportAction(reportAction);
			}
		}

		private void UnregisterWatsonReportActionsForCurrentThread()
		{
			WatsonReportAction[] array;
			lock (this.watsonReportActions)
			{
				array = this.watsonReportActions.ToArray<WatsonReportAction>();
			}
			foreach (WatsonReportAction reportAction in array)
			{
				this.InternalUnregisterWatsonReportAction(reportAction);
			}
		}

		private static bool allowImplicit = false;

		[ThreadStatic]
		private static Activity currentActivity;

		[ThreadStatic]
		private static bool? foregroundActivity;

		private readonly ICollection<WatsonReportAction> watsonReportActions = new List<WatsonReportAction>();

		private readonly ProtocolLogSession logSession = ProtocolLog.CreateNewSession();

		private readonly long traceId;

		private IStandardBudget budget;

		internal sealed class Guard : BaseObject
		{
			public Guard()
			{
				GC.SuppressFinalize(this);
			}

			public void AssociateWithCurrentThread(Activity activity, bool foreground)
			{
				if (this.activity != null)
				{
					throw new InvalidOperationException("Activity.Guard should not be reused.");
				}
				if (activity == null)
				{
					return;
				}
				this.activity = activity;
				Activity.currentActivity = activity;
				Activity.foregroundActivity = new bool?(foreground);
				activity.OnResume();
			}

			protected override void InternalDispose()
			{
				if (this.activity != null)
				{
					this.activity.OnPause();
					Activity.currentActivity = null;
					Activity.foregroundActivity = null;
				}
				base.InternalDispose();
			}

			protected override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<Activity.Guard>(this);
			}

			private Activity activity;
		}
	}
}
