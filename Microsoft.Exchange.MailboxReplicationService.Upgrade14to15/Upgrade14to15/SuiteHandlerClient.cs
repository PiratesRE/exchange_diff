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
	public class SuiteHandlerClient : ClientBase<ISuiteHandler>, ISuiteHandler
	{
		public SuiteHandlerClient()
		{
		}

		public SuiteHandlerClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public SuiteHandlerClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public SuiteHandlerClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public SuiteHandlerClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public event EventHandler<AddPilotUsersCompletedEventArgs> AddPilotUsersCompleted;

		public event EventHandler<GetPilotUsersCompletedEventArgs> GetPilotUsersCompleted;

		public event EventHandler<AsyncCompletedEventArgs> PostponeTenantUpgradeCompleted;

		public int AddPilotUsers(Guid tenantId, UserId[] users)
		{
			return base.Channel.AddPilotUsers(tenantId, users);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginAddPilotUsers(Guid tenantId, UserId[] users, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginAddPilotUsers(tenantId, users, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int EndAddPilotUsers(IAsyncResult result)
		{
			return base.Channel.EndAddPilotUsers(result);
		}

		private IAsyncResult OnBeginAddPilotUsers(object[] inValues, AsyncCallback callback, object asyncState)
		{
			Guid tenantId = (Guid)inValues[0];
			UserId[] users = (UserId[])inValues[1];
			return this.BeginAddPilotUsers(tenantId, users, callback, asyncState);
		}

		private object[] OnEndAddPilotUsers(IAsyncResult result)
		{
			int num = this.EndAddPilotUsers(result);
			return new object[]
			{
				num
			};
		}

		private void OnAddPilotUsersCompleted(object state)
		{
			if (this.AddPilotUsersCompleted != null)
			{
				ClientBase<ISuiteHandler>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ISuiteHandler>.InvokeAsyncCompletedEventArgs)state;
				this.AddPilotUsersCompleted(this, new AddPilotUsersCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void AddPilotUsersAsync(Guid tenantId, UserId[] users)
		{
			this.AddPilotUsersAsync(tenantId, users, null);
		}

		public void AddPilotUsersAsync(Guid tenantId, UserId[] users, object userState)
		{
			if (this.onBeginAddPilotUsersDelegate == null)
			{
				this.onBeginAddPilotUsersDelegate = new ClientBase<ISuiteHandler>.BeginOperationDelegate(this.OnBeginAddPilotUsers);
			}
			if (this.onEndAddPilotUsersDelegate == null)
			{
				this.onEndAddPilotUsersDelegate = new ClientBase<ISuiteHandler>.EndOperationDelegate(this.OnEndAddPilotUsers);
			}
			if (this.onAddPilotUsersCompletedDelegate == null)
			{
				this.onAddPilotUsersCompletedDelegate = new SendOrPostCallback(this.OnAddPilotUsersCompleted);
			}
			base.InvokeAsync(this.onBeginAddPilotUsersDelegate, new object[]
			{
				tenantId,
				users
			}, this.onEndAddPilotUsersDelegate, this.onAddPilotUsersCompletedDelegate, userState);
		}

		public UserWorkloadStatusInfo[] GetPilotUsers(Guid tenantId)
		{
			return base.Channel.GetPilotUsers(tenantId);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginGetPilotUsers(Guid tenantId, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginGetPilotUsers(tenantId, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public UserWorkloadStatusInfo[] EndGetPilotUsers(IAsyncResult result)
		{
			return base.Channel.EndGetPilotUsers(result);
		}

		private IAsyncResult OnBeginGetPilotUsers(object[] inValues, AsyncCallback callback, object asyncState)
		{
			Guid tenantId = (Guid)inValues[0];
			return this.BeginGetPilotUsers(tenantId, callback, asyncState);
		}

		private object[] OnEndGetPilotUsers(IAsyncResult result)
		{
			UserWorkloadStatusInfo[] array = this.EndGetPilotUsers(result);
			return new object[]
			{
				array
			};
		}

		private void OnGetPilotUsersCompleted(object state)
		{
			if (this.GetPilotUsersCompleted != null)
			{
				ClientBase<ISuiteHandler>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ISuiteHandler>.InvokeAsyncCompletedEventArgs)state;
				this.GetPilotUsersCompleted(this, new GetPilotUsersCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void GetPilotUsersAsync(Guid tenantId)
		{
			this.GetPilotUsersAsync(tenantId, null);
		}

		public void GetPilotUsersAsync(Guid tenantId, object userState)
		{
			if (this.onBeginGetPilotUsersDelegate == null)
			{
				this.onBeginGetPilotUsersDelegate = new ClientBase<ISuiteHandler>.BeginOperationDelegate(this.OnBeginGetPilotUsers);
			}
			if (this.onEndGetPilotUsersDelegate == null)
			{
				this.onEndGetPilotUsersDelegate = new ClientBase<ISuiteHandler>.EndOperationDelegate(this.OnEndGetPilotUsers);
			}
			if (this.onGetPilotUsersCompletedDelegate == null)
			{
				this.onGetPilotUsersCompletedDelegate = new SendOrPostCallback(this.OnGetPilotUsersCompleted);
			}
			base.InvokeAsync(this.onBeginGetPilotUsersDelegate, new object[]
			{
				tenantId
			}, this.onEndGetPilotUsersDelegate, this.onGetPilotUsersCompletedDelegate, userState);
		}

		public void PostponeTenantUpgrade(Guid tenantId, string userUpn)
		{
			base.Channel.PostponeTenantUpgrade(tenantId, userUpn);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginPostponeTenantUpgrade(Guid tenantId, string userUpn, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginPostponeTenantUpgrade(tenantId, userUpn, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndPostponeTenantUpgrade(IAsyncResult result)
		{
			base.Channel.EndPostponeTenantUpgrade(result);
		}

		private IAsyncResult OnBeginPostponeTenantUpgrade(object[] inValues, AsyncCallback callback, object asyncState)
		{
			Guid tenantId = (Guid)inValues[0];
			string userUpn = (string)inValues[1];
			return this.BeginPostponeTenantUpgrade(tenantId, userUpn, callback, asyncState);
		}

		private object[] OnEndPostponeTenantUpgrade(IAsyncResult result)
		{
			this.EndPostponeTenantUpgrade(result);
			return null;
		}

		private void OnPostponeTenantUpgradeCompleted(object state)
		{
			if (this.PostponeTenantUpgradeCompleted != null)
			{
				ClientBase<ISuiteHandler>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ISuiteHandler>.InvokeAsyncCompletedEventArgs)state;
				this.PostponeTenantUpgradeCompleted(this, new AsyncCompletedEventArgs(invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void PostponeTenantUpgradeAsync(Guid tenantId, string userUpn)
		{
			this.PostponeTenantUpgradeAsync(tenantId, userUpn, null);
		}

		public void PostponeTenantUpgradeAsync(Guid tenantId, string userUpn, object userState)
		{
			if (this.onBeginPostponeTenantUpgradeDelegate == null)
			{
				this.onBeginPostponeTenantUpgradeDelegate = new ClientBase<ISuiteHandler>.BeginOperationDelegate(this.OnBeginPostponeTenantUpgrade);
			}
			if (this.onEndPostponeTenantUpgradeDelegate == null)
			{
				this.onEndPostponeTenantUpgradeDelegate = new ClientBase<ISuiteHandler>.EndOperationDelegate(this.OnEndPostponeTenantUpgrade);
			}
			if (this.onPostponeTenantUpgradeCompletedDelegate == null)
			{
				this.onPostponeTenantUpgradeCompletedDelegate = new SendOrPostCallback(this.OnPostponeTenantUpgradeCompleted);
			}
			base.InvokeAsync(this.onBeginPostponeTenantUpgradeDelegate, new object[]
			{
				tenantId,
				userUpn
			}, this.onEndPostponeTenantUpgradeDelegate, this.onPostponeTenantUpgradeCompletedDelegate, userState);
		}

		private ClientBase<ISuiteHandler>.BeginOperationDelegate onBeginAddPilotUsersDelegate;

		private ClientBase<ISuiteHandler>.EndOperationDelegate onEndAddPilotUsersDelegate;

		private SendOrPostCallback onAddPilotUsersCompletedDelegate;

		private ClientBase<ISuiteHandler>.BeginOperationDelegate onBeginGetPilotUsersDelegate;

		private ClientBase<ISuiteHandler>.EndOperationDelegate onEndGetPilotUsersDelegate;

		private SendOrPostCallback onGetPilotUsersCompletedDelegate;

		private ClientBase<ISuiteHandler>.BeginOperationDelegate onBeginPostponeTenantUpgradeDelegate;

		private ClientBase<ISuiteHandler>.EndOperationDelegate onEndPostponeTenantUpgradeDelegate;

		private SendOrPostCallback onPostponeTenantUpgradeCompletedDelegate;
	}
}
