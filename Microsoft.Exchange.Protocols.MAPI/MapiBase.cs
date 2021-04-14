using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public abstract class MapiBase : DisposableBase, ICriticalBlockFailureHandler, IMapiObject, IDisposable, IServerObject, ICountableObject
	{
		public MapiBase(MapiObjectType mapiObjectType)
		{
			this.mapiObjectType = mapiObjectType;
		}

		internal MapiObjectType MapiObjectType
		{
			get
			{
				return this.mapiObjectType;
			}
		}

		internal ObjectType PropTagObjectType
		{
			get
			{
				return Helper.GetPropTagObjectType(this.MapiObjectType);
			}
		}

		internal ObjectType PropTagBaseObjectType
		{
			get
			{
				return WellKnownProperties.BaseObjectType[(int)this.PropTagObjectType];
			}
		}

		public bool IsValid
		{
			get
			{
				return this.valid;
			}
			protected set
			{
				this.valid = value;
			}
		}

		public MapiLogon Logon
		{
			get
			{
				return this.logon;
			}
			protected set
			{
				this.logon = value;
			}
		}

		public virtual MapiSession Session
		{
			get
			{
				return this.logon.Session;
			}
		}

		public MapiContext CurrentOperationContext
		{
			get
			{
				return (MapiContext)this.Logon.StoreMailbox.CurrentOperationContext;
			}
		}

		public MapiPropBagBase ParentObject
		{
			get
			{
				return this.parent;
			}
			protected set
			{
				this.parent = value;
				this.parent.AddSubObject(this);
			}
		}

		public Encoding String8Encoding
		{
			get
			{
				return Encoding.Default;
			}
		}

		public static IDisposable SetOnDisposeTestHook(Action<MapiBase> action)
		{
			return MapiBase.onDisposeHook.SetTestHook(action);
		}

		public void ThrowIfNotValid(string errorMessage)
		{
			if (!this.valid)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "This " + this.MapiObjectType.ToString() + " object is not valid!  Throwing ExExceptionInvalidObject!");
				throw new ExExceptionInvalidObject((LID)42296U, (errorMessage == null) ? ("This " + this.MapiObjectType.ToString() + " object is not valid.") : errorMessage);
			}
		}

		public virtual void IncrementObjectCounter(MapiObjectTrackingScope scope, MapiObjectTrackedType trackedType)
		{
			if ((scope & MapiObjectTrackingScope.Session) != (MapiObjectTrackingScope)0U)
			{
				this.objectCounter = this.Logon.Session.GetPerSessionObjectCounter(trackedType);
				this.objectCounter.IncrementCount();
				this.objectCounter.CheckObjectQuota(false);
			}
		}

		public virtual void DecrementObjectCounter(MapiObjectTrackingScope scope)
		{
			if ((scope & MapiObjectTrackingScope.Session) != (MapiObjectTrackingScope)0U && this.objectCounter != null)
			{
				this.objectCounter.DecrementCount();
				this.objectCounter = null;
			}
		}

		public virtual IMapiObjectCounter GetObjectCounter(MapiObjectTrackingScope scope)
		{
			if ((scope & MapiObjectTrackingScope.Session) != (MapiObjectTrackingScope)0U && this.objectCounter != null)
			{
				return this.objectCounter;
			}
			return UnlimitedObjectCounter.Instance;
		}

		void ICriticalBlockFailureHandler.OnCriticalBlockFailed(LID lid, Context context, CriticalBlockScope criticalBlockScope)
		{
			this.valid = false;
			context.OnCriticalBlockFailed(lid, criticalBlockScope);
		}

		public virtual void OnRelease(MapiContext context)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiBase>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (MapiBase.onDisposeHook.Value != null)
				{
					MapiBase.onDisposeHook.Value(this);
				}
				if (this.parent != null)
				{
					this.parent.RemoveSubObject(this);
				}
				this.DecrementObjectCounter(MapiObjectTrackingScope.All);
			}
			this.logon = null;
			this.parent = null;
			this.valid = false;
		}

		public virtual void FormatDiagnosticInformation(TraceContentBuilder cb, int indentLevel)
		{
		}

		public virtual void ClearDiagnosticInformation()
		{
		}

		public virtual void GetSummaryInformation(ref ExecutionDiagnostics.LongOperationSummary summary)
		{
		}

		protected void TraceNotificationIgnored(NotificationEvent nev, string reasonIgnored)
		{
			if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append(base.GetType().Name);
				stringBuilder.Append(" has ignored a notification: ");
				stringBuilder.Append(reasonIgnored);
				stringBuilder.Append(" [");
				nev.AppendToString(stringBuilder);
				stringBuilder.Append("]");
				ExTraceGlobals.NotificationTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		private static Hookable<Action<MapiBase>> onDisposeHook = Hookable<Action<MapiBase>>.Create(false, null);

		private MapiLogon logon;

		private MapiObjectType mapiObjectType;

		private bool valid;

		private MapiPropBagBase parent;

		private IMapiObjectCounter objectCounter;
	}
}
