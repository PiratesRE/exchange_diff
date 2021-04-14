using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class UpgradeHandlerClient : ClientBase<IUpgradeHandler>, IUpgradeHandler
	{
		public UpgradeHandlerClient()
		{
		}

		public UpgradeHandlerClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public UpgradeHandlerClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public UpgradeHandlerClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public UpgradeHandlerClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public event EventHandler<QueryWorkItemsCompletedEventArgs> QueryWorkItemsCompleted;

		public event EventHandler<QueryTenantWorkItemsCompletedEventArgs> QueryTenantWorkItemsCompleted;

		public event EventHandler<AsyncCompletedEventArgs> UpdateWorkItemCompleted;

		public WorkItemQueryResult QueryWorkItems(string groupName, string tenantTier, string workItemType, WorkItemStatus status, int pageSize, byte[] bookmark)
		{
			return base.Channel.QueryWorkItems(groupName, tenantTier, workItemType, status, pageSize, bookmark);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginQueryWorkItems(string groupName, string tenantTier, string workItemType, WorkItemStatus status, int pageSize, byte[] bookmark, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginQueryWorkItems(groupName, tenantTier, workItemType, status, pageSize, bookmark, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public WorkItemQueryResult EndQueryWorkItems(IAsyncResult result)
		{
			return base.Channel.EndQueryWorkItems(result);
		}

		private IAsyncResult OnBeginQueryWorkItems(object[] inValues, AsyncCallback callback, object asyncState)
		{
			string groupName = (string)inValues[0];
			string tenantTier = (string)inValues[1];
			string workItemType = (string)inValues[2];
			WorkItemStatus status = (WorkItemStatus)inValues[3];
			int pageSize = (int)inValues[4];
			byte[] bookmark = (byte[])inValues[5];
			return this.BeginQueryWorkItems(groupName, tenantTier, workItemType, status, pageSize, bookmark, callback, asyncState);
		}

		private object[] OnEndQueryWorkItems(IAsyncResult result)
		{
			WorkItemQueryResult workItemQueryResult = this.EndQueryWorkItems(result);
			return new object[]
			{
				workItemQueryResult
			};
		}

		private void OnQueryWorkItemsCompleted(object state)
		{
			if (this.QueryWorkItemsCompleted != null)
			{
				ClientBase<IUpgradeHandler>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeHandler>.InvokeAsyncCompletedEventArgs)state;
				this.QueryWorkItemsCompleted(this, new QueryWorkItemsCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void QueryWorkItemsAsync(string groupName, string tenantTier, string workItemType, WorkItemStatus status, int pageSize, byte[] bookmark)
		{
			this.QueryWorkItemsAsync(groupName, tenantTier, workItemType, status, pageSize, bookmark, null);
		}

		public void QueryWorkItemsAsync(string groupName, string tenantTier, string workItemType, WorkItemStatus status, int pageSize, byte[] bookmark, object userState)
		{
			if (this.onBeginQueryWorkItemsDelegate == null)
			{
				this.onBeginQueryWorkItemsDelegate = new ClientBase<IUpgradeHandler>.BeginOperationDelegate(this.OnBeginQueryWorkItems);
			}
			if (this.onEndQueryWorkItemsDelegate == null)
			{
				this.onEndQueryWorkItemsDelegate = new ClientBase<IUpgradeHandler>.EndOperationDelegate(this.OnEndQueryWorkItems);
			}
			if (this.onQueryWorkItemsCompletedDelegate == null)
			{
				this.onQueryWorkItemsCompletedDelegate = new SendOrPostCallback(this.OnQueryWorkItemsCompleted);
			}
			base.InvokeAsync(this.onBeginQueryWorkItemsDelegate, new object[]
			{
				groupName,
				tenantTier,
				workItemType,
				status,
				pageSize,
				bookmark
			}, this.onEndQueryWorkItemsDelegate, this.onQueryWorkItemsCompletedDelegate, userState);
		}

		public WorkItemInfo[] QueryTenantWorkItems(Guid tenantId)
		{
			return base.Channel.QueryTenantWorkItems(tenantId);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginQueryTenantWorkItems(Guid tenantId, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginQueryTenantWorkItems(tenantId, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public WorkItemInfo[] EndQueryTenantWorkItems(IAsyncResult result)
		{
			return base.Channel.EndQueryTenantWorkItems(result);
		}

		private IAsyncResult OnBeginQueryTenantWorkItems(object[] inValues, AsyncCallback callback, object asyncState)
		{
			Guid tenantId = (Guid)inValues[0];
			return this.BeginQueryTenantWorkItems(tenantId, callback, asyncState);
		}

		private object[] OnEndQueryTenantWorkItems(IAsyncResult result)
		{
			WorkItemInfo[] array = this.EndQueryTenantWorkItems(result);
			return new object[]
			{
				array
			};
		}

		private void OnQueryTenantWorkItemsCompleted(object state)
		{
			if (this.QueryTenantWorkItemsCompleted != null)
			{
				ClientBase<IUpgradeHandler>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeHandler>.InvokeAsyncCompletedEventArgs)state;
				this.QueryTenantWorkItemsCompleted(this, new QueryTenantWorkItemsCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void QueryTenantWorkItemsAsync(Guid tenantId)
		{
			this.QueryTenantWorkItemsAsync(tenantId, null);
		}

		public void QueryTenantWorkItemsAsync(Guid tenantId, object userState)
		{
			if (this.onBeginQueryTenantWorkItemsDelegate == null)
			{
				this.onBeginQueryTenantWorkItemsDelegate = new ClientBase<IUpgradeHandler>.BeginOperationDelegate(this.OnBeginQueryTenantWorkItems);
			}
			if (this.onEndQueryTenantWorkItemsDelegate == null)
			{
				this.onEndQueryTenantWorkItemsDelegate = new ClientBase<IUpgradeHandler>.EndOperationDelegate(this.OnEndQueryTenantWorkItems);
			}
			if (this.onQueryTenantWorkItemsCompletedDelegate == null)
			{
				this.onQueryTenantWorkItemsCompletedDelegate = new SendOrPostCallback(this.OnQueryTenantWorkItemsCompleted);
			}
			base.InvokeAsync(this.onBeginQueryTenantWorkItemsDelegate, new object[]
			{
				tenantId
			}, this.onEndQueryTenantWorkItemsDelegate, this.onQueryTenantWorkItemsCompletedDelegate, userState);
		}

		public void UpdateWorkItem(string workItemId, WorkItemStatusInfo status)
		{
			base.Channel.UpdateWorkItem(workItemId, status);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginUpdateWorkItem(string workItemId, WorkItemStatusInfo status, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginUpdateWorkItem(workItemId, status, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndUpdateWorkItem(IAsyncResult result)
		{
			base.Channel.EndUpdateWorkItem(result);
		}

		private IAsyncResult OnBeginUpdateWorkItem(object[] inValues, AsyncCallback callback, object asyncState)
		{
			string workItemId = (string)inValues[0];
			WorkItemStatusInfo status = (WorkItemStatusInfo)inValues[1];
			return this.BeginUpdateWorkItem(workItemId, status, callback, asyncState);
		}

		private object[] OnEndUpdateWorkItem(IAsyncResult result)
		{
			this.EndUpdateWorkItem(result);
			return null;
		}

		private void OnUpdateWorkItemCompleted(object state)
		{
			if (this.UpdateWorkItemCompleted != null)
			{
				ClientBase<IUpgradeHandler>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IUpgradeHandler>.InvokeAsyncCompletedEventArgs)state;
				this.UpdateWorkItemCompleted(this, new AsyncCompletedEventArgs(invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void UpdateWorkItemAsync(string workItemId, WorkItemStatusInfo status)
		{
			this.UpdateWorkItemAsync(workItemId, status, null);
		}

		public void UpdateWorkItemAsync(string workItemId, WorkItemStatusInfo status, object userState)
		{
			if (this.onBeginUpdateWorkItemDelegate == null)
			{
				this.onBeginUpdateWorkItemDelegate = new ClientBase<IUpgradeHandler>.BeginOperationDelegate(this.OnBeginUpdateWorkItem);
			}
			if (this.onEndUpdateWorkItemDelegate == null)
			{
				this.onEndUpdateWorkItemDelegate = new ClientBase<IUpgradeHandler>.EndOperationDelegate(this.OnEndUpdateWorkItem);
			}
			if (this.onUpdateWorkItemCompletedDelegate == null)
			{
				this.onUpdateWorkItemCompletedDelegate = new SendOrPostCallback(this.OnUpdateWorkItemCompleted);
			}
			base.InvokeAsync(this.onBeginUpdateWorkItemDelegate, new object[]
			{
				workItemId,
				status
			}, this.onEndUpdateWorkItemDelegate, this.onUpdateWorkItemCompletedDelegate, userState);
		}

		private ClientBase<IUpgradeHandler>.BeginOperationDelegate onBeginQueryWorkItemsDelegate;

		private ClientBase<IUpgradeHandler>.EndOperationDelegate onEndQueryWorkItemsDelegate;

		private SendOrPostCallback onQueryWorkItemsCompletedDelegate;

		private ClientBase<IUpgradeHandler>.BeginOperationDelegate onBeginQueryTenantWorkItemsDelegate;

		private ClientBase<IUpgradeHandler>.EndOperationDelegate onEndQueryTenantWorkItemsDelegate;

		private SendOrPostCallback onQueryTenantWorkItemsCompletedDelegate;

		private ClientBase<IUpgradeHandler>.BeginOperationDelegate onBeginUpdateWorkItemDelegate;

		private ClientBase<IUpgradeHandler>.EndOperationDelegate onEndUpdateWorkItemDelegate;

		private SendOrPostCallback onUpdateWorkItemCompletedDelegate;
	}
}
