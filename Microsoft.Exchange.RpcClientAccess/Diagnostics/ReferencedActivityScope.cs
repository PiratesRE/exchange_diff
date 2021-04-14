using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ReferencedActivityScope : ReferenceCount<ReferencedActivityScope.ActivityScopeGuard>
	{
		private ReferencedActivityScope(ReferencedActivityScope.ActivityScopeGuard activityScopeGuard) : base(activityScopeGuard)
		{
		}

		public static ReferencedActivityScope Create(IEnumerable<KeyValuePair<Enum, object>> initialMetadata)
		{
			ReferencedActivityScope referencedActivityScope = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				try
				{
					ActivityContext.ClearThreadScope();
					IActivityScope activityScope = ActivityContext.Start(null);
					ReferencedActivityScope.ActivityScopeGuard activityScopeGuard = new ReferencedActivityScope.ActivityScopeGuard(activityScope);
					disposeGuard.Add<ReferencedActivityScope.ActivityScopeGuard>(activityScopeGuard);
					referencedActivityScope = new ReferencedActivityScope(activityScopeGuard);
					if (initialMetadata != null)
					{
						referencedActivityScope.SetMetadata(initialMetadata);
					}
					activityScope.UserState = referencedActivityScope;
				}
				finally
				{
					ActivityContext.SetThreadScope(currentActivityScope);
				}
				disposeGuard.Success();
			}
			return referencedActivityScope;
		}

		public static ReferencedActivityScope Current
		{
			get
			{
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				if (currentActivityScope != null)
				{
					return currentActivityScope.UserState as ReferencedActivityScope;
				}
				return null;
			}
		}

		public IActivityScope ActivityScope
		{
			get
			{
				return base.ReferencedObject.ActivityScope;
			}
		}

		public string TenantId
		{
			get
			{
				return this.ActivityScope.TenantId;
			}
			set
			{
				this.ActivityScope.TenantId = value;
			}
		}

		public string Protocol
		{
			get
			{
				return this.ActivityScope.Protocol;
			}
			set
			{
				this.ActivityScope.Protocol = value;
			}
		}

		public string UserEmail
		{
			get
			{
				return this.ActivityScope.UserEmail;
			}
			set
			{
				this.ActivityScope.UserEmail = value;
			}
		}

		public string UserId
		{
			get
			{
				return this.ActivityScope.UserId;
			}
			set
			{
				this.ActivityScope.UserId = value;
			}
		}

		public string Puid
		{
			get
			{
				return this.ActivityScope.Puid;
			}
			set
			{
				this.ActivityScope.Puid = value;
			}
		}

		public string Component
		{
			get
			{
				return this.ActivityScope.Component;
			}
			set
			{
				this.ActivityScope.Component = value;
			}
		}

		public string ClientInfo
		{
			get
			{
				return this.ActivityScope.ClientInfo;
			}
			set
			{
				this.ActivityScope.ClientInfo = value;
			}
		}

		private void SetMetadata(IEnumerable<KeyValuePair<Enum, object>> metadata)
		{
			foreach (KeyValuePair<Enum, object> keyValuePair in metadata)
			{
				string text = keyValuePair.Value as string;
				if (keyValuePair.Value == null || text != null)
				{
					this.ActivityScope.SetProperty(keyValuePair.Key, text);
				}
			}
		}

		internal sealed class ActivityScopeGuard : BaseObject
		{
			public ActivityScopeGuard(IActivityScope activityScope)
			{
				if (activityScope == null)
				{
					throw new ArgumentNullException("activityScope");
				}
				this.activityScope = activityScope;
			}

			public IActivityScope ActivityScope
			{
				get
				{
					return this.activityScope;
				}
			}

			protected override void InternalDispose()
			{
				this.activityScope.End();
				base.InternalDispose();
			}

			protected override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<ReferencedActivityScope.ActivityScopeGuard>(this);
			}

			private readonly IActivityScope activityScope;
		}
	}
}
