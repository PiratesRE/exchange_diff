using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[DebuggerStepThrough]
	public class UpgradeSchedulingConstraintsClient : ClientBase<IUpgradeSchedulingConstraints>, IUpgradeSchedulingConstraints
	{
		public UpgradeSchedulingConstraintsClient()
		{
		}

		public UpgradeSchedulingConstraintsClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public UpgradeSchedulingConstraintsClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public UpgradeSchedulingConstraintsClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public UpgradeSchedulingConstraintsClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public event EventHandler<AsyncCompletedEventArgs> UpdateTenantReadinessCompleted;

		public event EventHandler<AsyncCompletedEventArgs> UpdateGroupCompleted;

		public event EventHandler<AsyncCompletedEventArgs> UpdateCapacityCompleted;

		public event EventHandler<AsyncCompletedEventArgs> UpdateBlackoutCompleted;

		public event EventHandler<AsyncCompletedEventArgs> UpdateConstraintCompleted;

		public event EventHandler<QueryTenantReadinessCompletedEventArgs> QueryTenantReadinessCompleted;

		public event EventHandler<QueryGroupCompletedEventArgs> QueryGroupCompleted;

		public event EventHandler<QueryCapacityCompletedEventArgs> QueryCapacityCompleted;

		public event EventHandler<QueryBlackoutCompletedEventArgs> QueryBlackoutCompleted;

		public event EventHandler<QueryConstraintCompletedEventArgs> QueryConstraintCompleted;

		public void UpdateTenantReadiness(TenantReadiness[] tenantReadiness)
		{
			base.Channel.UpdateTenantReadiness(tenantReadiness);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginUpdateTenantReadiness(TenantReadiness[] tenantReadiness, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginUpdateTenantReadiness(tenantReadiness, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndUpdateTenantReadiness(IAsyncResult result)
		{
			base.Channel.EndUpdateTenantReadiness(result);
		}

		private IAsyncResult OnBeginUpdateTenantReadiness(object[] inValues, AsyncCallback callback, object asyncState)
		{
			TenantReadiness[] tenantReadiness = (TenantReadiness[])inValues[0];
			return this.BeginUpdateTenantReadiness(tenantReadiness, callback, asyncState);
		}

		private object[] OnEndUpdateTenantReadiness(IAsyncResult result)
		{
			this.EndUpdateTenantReadiness(result);
			return null;
		}

		private void OnUpdateTenantReadinessCompleted(object state)
		{
			if (this.UpdateTenantReadinessCompleted != null)
			{
				ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs)state;
				this.UpdateTenantReadinessCompleted(this, new AsyncCompletedEventArgs(invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void UpdateTenantReadinessAsync(TenantReadiness[] tenantReadiness)
		{
			this.UpdateTenantReadinessAsync(tenantReadiness, null);
		}

		public void UpdateTenantReadinessAsync(TenantReadiness[] tenantReadiness, object userState)
		{
			if (this.onBeginUpdateTenantReadinessDelegate == null)
			{
				this.onBeginUpdateTenantReadinessDelegate = new ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate(this.OnBeginUpdateTenantReadiness);
			}
			if (this.onEndUpdateTenantReadinessDelegate == null)
			{
				this.onEndUpdateTenantReadinessDelegate = new ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate(this.OnEndUpdateTenantReadiness);
			}
			if (this.onUpdateTenantReadinessCompletedDelegate == null)
			{
				this.onUpdateTenantReadinessCompletedDelegate = new SendOrPostCallback(this.OnUpdateTenantReadinessCompleted);
			}
			base.InvokeAsync(this.onBeginUpdateTenantReadinessDelegate, new object[]
			{
				tenantReadiness
			}, this.onEndUpdateTenantReadinessDelegate, this.onUpdateTenantReadinessCompletedDelegate, userState);
		}

		public void UpdateGroup(Group[] group)
		{
			base.Channel.UpdateGroup(group);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginUpdateGroup(Group[] group, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginUpdateGroup(group, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndUpdateGroup(IAsyncResult result)
		{
			base.Channel.EndUpdateGroup(result);
		}

		private IAsyncResult OnBeginUpdateGroup(object[] inValues, AsyncCallback callback, object asyncState)
		{
			Group[] group = (Group[])inValues[0];
			return this.BeginUpdateGroup(group, callback, asyncState);
		}

		private object[] OnEndUpdateGroup(IAsyncResult result)
		{
			this.EndUpdateGroup(result);
			return null;
		}

		private void OnUpdateGroupCompleted(object state)
		{
			if (this.UpdateGroupCompleted != null)
			{
				ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs)state;
				this.UpdateGroupCompleted(this, new AsyncCompletedEventArgs(invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void UpdateGroupAsync(Group[] group)
		{
			this.UpdateGroupAsync(group, null);
		}

		public void UpdateGroupAsync(Group[] group, object userState)
		{
			if (this.onBeginUpdateGroupDelegate == null)
			{
				this.onBeginUpdateGroupDelegate = new ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate(this.OnBeginUpdateGroup);
			}
			if (this.onEndUpdateGroupDelegate == null)
			{
				this.onEndUpdateGroupDelegate = new ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate(this.OnEndUpdateGroup);
			}
			if (this.onUpdateGroupCompletedDelegate == null)
			{
				this.onUpdateGroupCompletedDelegate = new SendOrPostCallback(this.OnUpdateGroupCompleted);
			}
			base.InvokeAsync(this.onBeginUpdateGroupDelegate, new object[]
			{
				group
			}, this.onEndUpdateGroupDelegate, this.onUpdateGroupCompletedDelegate, userState);
		}

		public void UpdateCapacity(GroupCapacity[] capacities)
		{
			base.Channel.UpdateCapacity(capacities);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginUpdateCapacity(GroupCapacity[] capacities, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginUpdateCapacity(capacities, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndUpdateCapacity(IAsyncResult result)
		{
			base.Channel.EndUpdateCapacity(result);
		}

		private IAsyncResult OnBeginUpdateCapacity(object[] inValues, AsyncCallback callback, object asyncState)
		{
			GroupCapacity[] capacities = (GroupCapacity[])inValues[0];
			return this.BeginUpdateCapacity(capacities, callback, asyncState);
		}

		private object[] OnEndUpdateCapacity(IAsyncResult result)
		{
			this.EndUpdateCapacity(result);
			return null;
		}

		private void OnUpdateCapacityCompleted(object state)
		{
			if (this.UpdateCapacityCompleted != null)
			{
				ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs)state;
				this.UpdateCapacityCompleted(this, new AsyncCompletedEventArgs(invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void UpdateCapacityAsync(GroupCapacity[] capacities)
		{
			this.UpdateCapacityAsync(capacities, null);
		}

		public void UpdateCapacityAsync(GroupCapacity[] capacities, object userState)
		{
			if (this.onBeginUpdateCapacityDelegate == null)
			{
				this.onBeginUpdateCapacityDelegate = new ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate(this.OnBeginUpdateCapacity);
			}
			if (this.onEndUpdateCapacityDelegate == null)
			{
				this.onEndUpdateCapacityDelegate = new ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate(this.OnEndUpdateCapacity);
			}
			if (this.onUpdateCapacityCompletedDelegate == null)
			{
				this.onUpdateCapacityCompletedDelegate = new SendOrPostCallback(this.OnUpdateCapacityCompleted);
			}
			base.InvokeAsync(this.onBeginUpdateCapacityDelegate, new object[]
			{
				capacities
			}, this.onEndUpdateCapacityDelegate, this.onUpdateCapacityCompletedDelegate, userState);
		}

		public void UpdateBlackout(GroupBlackout[] blackouts)
		{
			base.Channel.UpdateBlackout(blackouts);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginUpdateBlackout(GroupBlackout[] blackouts, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginUpdateBlackout(blackouts, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndUpdateBlackout(IAsyncResult result)
		{
			base.Channel.EndUpdateBlackout(result);
		}

		private IAsyncResult OnBeginUpdateBlackout(object[] inValues, AsyncCallback callback, object asyncState)
		{
			GroupBlackout[] blackouts = (GroupBlackout[])inValues[0];
			return this.BeginUpdateBlackout(blackouts, callback, asyncState);
		}

		private object[] OnEndUpdateBlackout(IAsyncResult result)
		{
			this.EndUpdateBlackout(result);
			return null;
		}

		private void OnUpdateBlackoutCompleted(object state)
		{
			if (this.UpdateBlackoutCompleted != null)
			{
				ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs)state;
				this.UpdateBlackoutCompleted(this, new AsyncCompletedEventArgs(invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void UpdateBlackoutAsync(GroupBlackout[] blackouts)
		{
			this.UpdateBlackoutAsync(blackouts, null);
		}

		public void UpdateBlackoutAsync(GroupBlackout[] blackouts, object userState)
		{
			if (this.onBeginUpdateBlackoutDelegate == null)
			{
				this.onBeginUpdateBlackoutDelegate = new ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate(this.OnBeginUpdateBlackout);
			}
			if (this.onEndUpdateBlackoutDelegate == null)
			{
				this.onEndUpdateBlackoutDelegate = new ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate(this.OnEndUpdateBlackout);
			}
			if (this.onUpdateBlackoutCompletedDelegate == null)
			{
				this.onUpdateBlackoutCompletedDelegate = new SendOrPostCallback(this.OnUpdateBlackoutCompleted);
			}
			base.InvokeAsync(this.onBeginUpdateBlackoutDelegate, new object[]
			{
				blackouts
			}, this.onEndUpdateBlackoutDelegate, this.onUpdateBlackoutCompletedDelegate, userState);
		}

		public void UpdateConstraint(Constraint[] constraint)
		{
			base.Channel.UpdateConstraint(constraint);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginUpdateConstraint(Constraint[] constraint, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginUpdateConstraint(constraint, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndUpdateConstraint(IAsyncResult result)
		{
			base.Channel.EndUpdateConstraint(result);
		}

		private IAsyncResult OnBeginUpdateConstraint(object[] inValues, AsyncCallback callback, object asyncState)
		{
			Constraint[] constraint = (Constraint[])inValues[0];
			return this.BeginUpdateConstraint(constraint, callback, asyncState);
		}

		private object[] OnEndUpdateConstraint(IAsyncResult result)
		{
			this.EndUpdateConstraint(result);
			return null;
		}

		private void OnUpdateConstraintCompleted(object state)
		{
			if (this.UpdateConstraintCompleted != null)
			{
				ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs)state;
				this.UpdateConstraintCompleted(this, new AsyncCompletedEventArgs(invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void UpdateConstraintAsync(Constraint[] constraint)
		{
			this.UpdateConstraintAsync(constraint, null);
		}

		public void UpdateConstraintAsync(Constraint[] constraint, object userState)
		{
			if (this.onBeginUpdateConstraintDelegate == null)
			{
				this.onBeginUpdateConstraintDelegate = new ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate(this.OnBeginUpdateConstraint);
			}
			if (this.onEndUpdateConstraintDelegate == null)
			{
				this.onEndUpdateConstraintDelegate = new ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate(this.OnEndUpdateConstraint);
			}
			if (this.onUpdateConstraintCompletedDelegate == null)
			{
				this.onUpdateConstraintCompletedDelegate = new SendOrPostCallback(this.OnUpdateConstraintCompleted);
			}
			base.InvokeAsync(this.onBeginUpdateConstraintDelegate, new object[]
			{
				constraint
			}, this.onEndUpdateConstraintDelegate, this.onUpdateConstraintCompletedDelegate, userState);
		}

		public TenantReadiness[] QueryTenantReadiness(Guid[] tenantIds)
		{
			return base.Channel.QueryTenantReadiness(tenantIds);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginQueryTenantReadiness(Guid[] tenantIds, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginQueryTenantReadiness(tenantIds, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public TenantReadiness[] EndQueryTenantReadiness(IAsyncResult result)
		{
			return base.Channel.EndQueryTenantReadiness(result);
		}

		private IAsyncResult OnBeginQueryTenantReadiness(object[] inValues, AsyncCallback callback, object asyncState)
		{
			Guid[] tenantIds = (Guid[])inValues[0];
			return this.BeginQueryTenantReadiness(tenantIds, callback, asyncState);
		}

		private object[] OnEndQueryTenantReadiness(IAsyncResult result)
		{
			TenantReadiness[] array = this.EndQueryTenantReadiness(result);
			return new object[]
			{
				array
			};
		}

		private void OnQueryTenantReadinessCompleted(object state)
		{
			if (this.QueryTenantReadinessCompleted != null)
			{
				ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs)state;
				this.QueryTenantReadinessCompleted(this, new QueryTenantReadinessCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void QueryTenantReadinessAsync(Guid[] tenantIds)
		{
			this.QueryTenantReadinessAsync(tenantIds, null);
		}

		public void QueryTenantReadinessAsync(Guid[] tenantIds, object userState)
		{
			if (this.onBeginQueryTenantReadinessDelegate == null)
			{
				this.onBeginQueryTenantReadinessDelegate = new ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate(this.OnBeginQueryTenantReadiness);
			}
			if (this.onEndQueryTenantReadinessDelegate == null)
			{
				this.onEndQueryTenantReadinessDelegate = new ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate(this.OnEndQueryTenantReadiness);
			}
			if (this.onQueryTenantReadinessCompletedDelegate == null)
			{
				this.onQueryTenantReadinessCompletedDelegate = new SendOrPostCallback(this.OnQueryTenantReadinessCompleted);
			}
			base.InvokeAsync(this.onBeginQueryTenantReadinessDelegate, new object[]
			{
				tenantIds
			}, this.onEndQueryTenantReadinessDelegate, this.onQueryTenantReadinessCompletedDelegate, userState);
		}

		public Group[] QueryGroup(string[] groupNames)
		{
			return base.Channel.QueryGroup(groupNames);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginQueryGroup(string[] groupNames, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginQueryGroup(groupNames, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Group[] EndQueryGroup(IAsyncResult result)
		{
			return base.Channel.EndQueryGroup(result);
		}

		private IAsyncResult OnBeginQueryGroup(object[] inValues, AsyncCallback callback, object asyncState)
		{
			string[] groupNames = (string[])inValues[0];
			return this.BeginQueryGroup(groupNames, callback, asyncState);
		}

		private object[] OnEndQueryGroup(IAsyncResult result)
		{
			Group[] array = this.EndQueryGroup(result);
			return new object[]
			{
				array
			};
		}

		private void OnQueryGroupCompleted(object state)
		{
			if (this.QueryGroupCompleted != null)
			{
				ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs)state;
				this.QueryGroupCompleted(this, new QueryGroupCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void QueryGroupAsync(string[] groupNames)
		{
			this.QueryGroupAsync(groupNames, null);
		}

		public void QueryGroupAsync(string[] groupNames, object userState)
		{
			if (this.onBeginQueryGroupDelegate == null)
			{
				this.onBeginQueryGroupDelegate = new ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate(this.OnBeginQueryGroup);
			}
			if (this.onEndQueryGroupDelegate == null)
			{
				this.onEndQueryGroupDelegate = new ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate(this.OnEndQueryGroup);
			}
			if (this.onQueryGroupCompletedDelegate == null)
			{
				this.onQueryGroupCompletedDelegate = new SendOrPostCallback(this.OnQueryGroupCompleted);
			}
			base.InvokeAsync(this.onBeginQueryGroupDelegate, new object[]
			{
				groupNames
			}, this.onEndQueryGroupDelegate, this.onQueryGroupCompletedDelegate, userState);
		}

		public GroupCapacity[] QueryCapacity(string[] groupNames)
		{
			return base.Channel.QueryCapacity(groupNames);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginQueryCapacity(string[] groupNames, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginQueryCapacity(groupNames, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public GroupCapacity[] EndQueryCapacity(IAsyncResult result)
		{
			return base.Channel.EndQueryCapacity(result);
		}

		private IAsyncResult OnBeginQueryCapacity(object[] inValues, AsyncCallback callback, object asyncState)
		{
			string[] groupNames = (string[])inValues[0];
			return this.BeginQueryCapacity(groupNames, callback, asyncState);
		}

		private object[] OnEndQueryCapacity(IAsyncResult result)
		{
			GroupCapacity[] array = this.EndQueryCapacity(result);
			return new object[]
			{
				array
			};
		}

		private void OnQueryCapacityCompleted(object state)
		{
			if (this.QueryCapacityCompleted != null)
			{
				ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs)state;
				this.QueryCapacityCompleted(this, new QueryCapacityCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void QueryCapacityAsync(string[] groupNames)
		{
			this.QueryCapacityAsync(groupNames, null);
		}

		public void QueryCapacityAsync(string[] groupNames, object userState)
		{
			if (this.onBeginQueryCapacityDelegate == null)
			{
				this.onBeginQueryCapacityDelegate = new ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate(this.OnBeginQueryCapacity);
			}
			if (this.onEndQueryCapacityDelegate == null)
			{
				this.onEndQueryCapacityDelegate = new ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate(this.OnEndQueryCapacity);
			}
			if (this.onQueryCapacityCompletedDelegate == null)
			{
				this.onQueryCapacityCompletedDelegate = new SendOrPostCallback(this.OnQueryCapacityCompleted);
			}
			base.InvokeAsync(this.onBeginQueryCapacityDelegate, new object[]
			{
				groupNames
			}, this.onEndQueryCapacityDelegate, this.onQueryCapacityCompletedDelegate, userState);
		}

		public GroupBlackout[] QueryBlackout(string[] groupNames)
		{
			return base.Channel.QueryBlackout(groupNames);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginQueryBlackout(string[] groupNames, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginQueryBlackout(groupNames, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public GroupBlackout[] EndQueryBlackout(IAsyncResult result)
		{
			return base.Channel.EndQueryBlackout(result);
		}

		private IAsyncResult OnBeginQueryBlackout(object[] inValues, AsyncCallback callback, object asyncState)
		{
			string[] groupNames = (string[])inValues[0];
			return this.BeginQueryBlackout(groupNames, callback, asyncState);
		}

		private object[] OnEndQueryBlackout(IAsyncResult result)
		{
			GroupBlackout[] array = this.EndQueryBlackout(result);
			return new object[]
			{
				array
			};
		}

		private void OnQueryBlackoutCompleted(object state)
		{
			if (this.QueryBlackoutCompleted != null)
			{
				ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs)state;
				this.QueryBlackoutCompleted(this, new QueryBlackoutCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void QueryBlackoutAsync(string[] groupNames)
		{
			this.QueryBlackoutAsync(groupNames, null);
		}

		public void QueryBlackoutAsync(string[] groupNames, object userState)
		{
			if (this.onBeginQueryBlackoutDelegate == null)
			{
				this.onBeginQueryBlackoutDelegate = new ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate(this.OnBeginQueryBlackout);
			}
			if (this.onEndQueryBlackoutDelegate == null)
			{
				this.onEndQueryBlackoutDelegate = new ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate(this.OnEndQueryBlackout);
			}
			if (this.onQueryBlackoutCompletedDelegate == null)
			{
				this.onQueryBlackoutCompletedDelegate = new SendOrPostCallback(this.OnQueryBlackoutCompleted);
			}
			base.InvokeAsync(this.onBeginQueryBlackoutDelegate, new object[]
			{
				groupNames
			}, this.onEndQueryBlackoutDelegate, this.onQueryBlackoutCompletedDelegate, userState);
		}

		public Constraint[] QueryConstraint(string[] constraintName)
		{
			return base.Channel.QueryConstraint(constraintName);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginQueryConstraint(string[] constraintName, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginQueryConstraint(constraintName, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Constraint[] EndQueryConstraint(IAsyncResult result)
		{
			return base.Channel.EndQueryConstraint(result);
		}

		private IAsyncResult OnBeginQueryConstraint(object[] inValues, AsyncCallback callback, object asyncState)
		{
			string[] constraintName = (string[])inValues[0];
			return this.BeginQueryConstraint(constraintName, callback, asyncState);
		}

		private object[] OnEndQueryConstraint(IAsyncResult result)
		{
			Constraint[] array = this.EndQueryConstraint(result);
			return new object[]
			{
				array
			};
		}

		private void OnQueryConstraintCompleted(object state)
		{
			if (this.QueryConstraintCompleted != null)
			{
				ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeSchedulingConstraints>.InvokeAsyncCompletedEventArgs)state;
				this.QueryConstraintCompleted(this, new QueryConstraintCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void QueryConstraintAsync(string[] constraintName)
		{
			this.QueryConstraintAsync(constraintName, null);
		}

		public void QueryConstraintAsync(string[] constraintName, object userState)
		{
			if (this.onBeginQueryConstraintDelegate == null)
			{
				this.onBeginQueryConstraintDelegate = new ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate(this.OnBeginQueryConstraint);
			}
			if (this.onEndQueryConstraintDelegate == null)
			{
				this.onEndQueryConstraintDelegate = new ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate(this.OnEndQueryConstraint);
			}
			if (this.onQueryConstraintCompletedDelegate == null)
			{
				this.onQueryConstraintCompletedDelegate = new SendOrPostCallback(this.OnQueryConstraintCompleted);
			}
			base.InvokeAsync(this.onBeginQueryConstraintDelegate, new object[]
			{
				constraintName
			}, this.onEndQueryConstraintDelegate, this.onQueryConstraintCompletedDelegate, userState);
		}

		private ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate onBeginUpdateTenantReadinessDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate onEndUpdateTenantReadinessDelegate;

		private SendOrPostCallback onUpdateTenantReadinessCompletedDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate onBeginUpdateGroupDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate onEndUpdateGroupDelegate;

		private SendOrPostCallback onUpdateGroupCompletedDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate onBeginUpdateCapacityDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate onEndUpdateCapacityDelegate;

		private SendOrPostCallback onUpdateCapacityCompletedDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate onBeginUpdateBlackoutDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate onEndUpdateBlackoutDelegate;

		private SendOrPostCallback onUpdateBlackoutCompletedDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate onBeginUpdateConstraintDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate onEndUpdateConstraintDelegate;

		private SendOrPostCallback onUpdateConstraintCompletedDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate onBeginQueryTenantReadinessDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate onEndQueryTenantReadinessDelegate;

		private SendOrPostCallback onQueryTenantReadinessCompletedDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate onBeginQueryGroupDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate onEndQueryGroupDelegate;

		private SendOrPostCallback onQueryGroupCompletedDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate onBeginQueryCapacityDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate onEndQueryCapacityDelegate;

		private SendOrPostCallback onQueryCapacityCompletedDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate onBeginQueryBlackoutDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate onEndQueryBlackoutDelegate;

		private SendOrPostCallback onQueryBlackoutCompletedDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.BeginOperationDelegate onBeginQueryConstraintDelegate;

		private ClientBase<IUpgradeSchedulingConstraints>.EndOperationDelegate onEndQueryConstraintDelegate;

		private SendOrPostCallback onQueryConstraintCompletedDelegate;
	}
}
