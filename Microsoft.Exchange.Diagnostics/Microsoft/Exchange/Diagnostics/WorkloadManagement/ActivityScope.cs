using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal sealed class ActivityScope : DisposeTrackableBase, IActivityScope
	{
		internal ActivityScope(ActivityScopeImpl activityScopeImpl)
		{
			if (activityScopeImpl == null)
			{
				throw new ArgumentNullException("activityScopeImpl");
			}
			this.activityScopeImpl = activityScopeImpl;
		}

		public Guid ActivityId
		{
			get
			{
				return this.activityScopeImpl.ActivityId;
			}
		}

		public Guid LocalId
		{
			get
			{
				return this.activityScopeImpl.LocalId;
			}
		}

		public ActivityContextStatus Status
		{
			get
			{
				return this.activityScopeImpl.Status;
			}
		}

		public ActivityType ActivityType
		{
			get
			{
				return this.activityScopeImpl.ActivityType;
			}
			internal set
			{
				this.activityScopeImpl.ActivityType = value;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.activityScopeImpl.StartTime;
			}
		}

		public DateTime? EndTime
		{
			get
			{
				return this.activityScopeImpl.EndTime;
			}
		}

		public double TotalMilliseconds
		{
			get
			{
				return this.activityScopeImpl.TotalMilliseconds;
			}
		}

		public object UserState
		{
			get
			{
				return this.activityScopeImpl.UserState;
			}
			set
			{
				this.activityScopeImpl.UserState = value;
			}
		}

		public string UserId
		{
			get
			{
				return this.activityScopeImpl.UserId;
			}
			set
			{
				this.activityScopeImpl.UserId = value;
			}
		}

		public string Puid
		{
			get
			{
				return this.activityScopeImpl.Puid;
			}
			set
			{
				this.activityScopeImpl.Puid = value;
			}
		}

		public string UserEmail
		{
			get
			{
				return this.activityScopeImpl.UserEmail;
			}
			set
			{
				this.activityScopeImpl.UserEmail = value;
			}
		}

		public string AuthenticationType
		{
			get
			{
				return this.activityScopeImpl.AuthenticationType;
			}
			set
			{
				this.activityScopeImpl.AuthenticationType = value;
			}
		}

		public string AuthenticationToken
		{
			get
			{
				return this.activityScopeImpl.AuthenticationToken;
			}
			set
			{
				this.activityScopeImpl.AuthenticationToken = value;
			}
		}

		public string TenantId
		{
			get
			{
				return this.activityScopeImpl.TenantId;
			}
			set
			{
				this.activityScopeImpl.TenantId = value;
			}
		}

		public string TenantType
		{
			get
			{
				return this.activityScopeImpl.TenantType;
			}
			set
			{
				this.activityScopeImpl.TenantType = value;
			}
		}

		public string Component
		{
			get
			{
				return this.activityScopeImpl.Component;
			}
			set
			{
				this.activityScopeImpl.Component = value;
			}
		}

		public string ComponentInstance
		{
			get
			{
				return this.activityScopeImpl.ComponentInstance;
			}
			set
			{
				this.activityScopeImpl.ComponentInstance = value;
			}
		}

		public string Feature
		{
			get
			{
				return this.activityScopeImpl.Feature;
			}
			set
			{
				this.activityScopeImpl.Feature = value;
			}
		}

		public string Protocol
		{
			get
			{
				return this.activityScopeImpl.Protocol;
			}
			set
			{
				this.activityScopeImpl.Protocol = value;
			}
		}

		public string ClientInfo
		{
			get
			{
				return this.activityScopeImpl.ClientInfo;
			}
			set
			{
				this.activityScopeImpl.ClientInfo = value;
			}
		}

		public string ClientRequestId
		{
			get
			{
				return this.activityScopeImpl.ClientRequestId;
			}
			set
			{
				this.activityScopeImpl.ClientRequestId = value;
			}
		}

		public string ReturnClientRequestId
		{
			get
			{
				return this.activityScopeImpl.ReturnClientRequestId;
			}
			set
			{
				this.activityScopeImpl.ReturnClientRequestId = value;
			}
		}

		public string Action
		{
			get
			{
				return this.activityScopeImpl.Action;
			}
			set
			{
				this.activityScopeImpl.Action = value;
			}
		}

		public IEnumerable<KeyValuePair<Enum, object>> Metadata
		{
			get
			{
				return this.activityScopeImpl.Metadata;
			}
		}

		public IEnumerable<KeyValuePair<OperationKey, OperationStatistics>> Statistics
		{
			get
			{
				return this.activityScopeImpl.Statistics;
			}
		}

		internal ActivityScopeImpl ActivityScopeImpl
		{
			get
			{
				return this.activityScopeImpl;
			}
		}

		public ActivityContextState Suspend()
		{
			return this.activityScopeImpl.Suspend();
		}

		public void End()
		{
			this.Dispose();
		}

		public bool AddOperation(ActivityOperationType operation, string instance, float value = 0f, int count = 1)
		{
			return this.activityScopeImpl.AddOperation(operation, instance, value, count);
		}

		public void SetProperty(Enum property, string value)
		{
			this.activityScopeImpl.SetProperty(property, value);
		}

		public bool AppendToProperty(Enum property, string value)
		{
			return this.activityScopeImpl.AppendToProperty(property, value);
		}

		public string GetProperty(Enum property)
		{
			return this.activityScopeImpl.GetProperty(property);
		}

		public List<KeyValuePair<string, object>> GetFormattableMetadata()
		{
			return this.activityScopeImpl.GetFormattableMetadata();
		}

		public IEnumerable<KeyValuePair<string, object>> GetFormattableMetadata(IEnumerable<Enum> properties)
		{
			return this.activityScopeImpl.GetFormattableMetadata(properties);
		}

		public List<KeyValuePair<string, object>> GetFormattableStatistics()
		{
			return this.activityScopeImpl.GetFormattableStatistics();
		}

		public void UpdateFromMessage(HttpRequestMessageProperty wcfMessage)
		{
			this.activityScopeImpl.UpdateFromMessage(wcfMessage);
		}

		public void UpdateFromMessage(OperationContext wcfOperationContext)
		{
			this.activityScopeImpl.UpdateFromMessage(wcfOperationContext);
		}

		public void UpdateFromMessage(HttpRequest httpRequest)
		{
			this.activityScopeImpl.UpdateFromMessage(httpRequest);
		}

		public void UpdateFromMessage(HttpRequestBase httpRequestBase)
		{
			this.activityScopeImpl.UpdateFromMessage(httpRequestBase);
		}

		public void SerializeTo(HttpRequestMessageProperty wcfMessage)
		{
			this.activityScopeImpl.SerializeTo(wcfMessage);
		}

		public void SerializeTo(OperationContext wcfOperationContext)
		{
			this.activityScopeImpl.SerializeTo(wcfOperationContext);
		}

		public void SerializeTo(HttpWebRequest httpWebRequest)
		{
			this.activityScopeImpl.SerializeTo(httpWebRequest);
		}

		public void SerializeTo(HttpResponse httpResponse)
		{
			this.activityScopeImpl.SerializeTo(httpResponse);
		}

		public void SerializeMinimalTo(HttpRequestBase httpRequest)
		{
			this.activityScopeImpl.SerializeMinimalTo(httpRequest);
		}

		public void SerializeMinimalTo(HttpWebRequest httpWebRequest)
		{
			this.activityScopeImpl.SerializeMinimalTo(httpWebRequest);
		}

		public AggregatedOperationStatistics TakeStatisticsSnapshot(AggregatedOperationType type)
		{
			return this.activityScopeImpl.TakeStatisticsSnapshot(type);
		}

		public override string ToString()
		{
			return this.ActivityScopeImpl.ToString();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ActivityScope>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (!base.IsDisposed && this.activityScopeImpl != null)
			{
				this.activityScopeImpl.End();
			}
		}

		public const string ActivityIdKey = "ActID";

		private readonly ActivityScopeImpl activityScopeImpl;
	}
}
