using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.TestTenantManagement
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class TestTenantManagementClient : ClientBase<ITestTenantManagement>, ITestTenantManagement
	{
		public TestTenantManagementClient()
		{
		}

		public TestTenantManagementClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public TestTenantManagementClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public TestTenantManagementClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public TestTenantManagementClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public event EventHandler<QueryTenantsToPopulateCompletedEventArgs> QueryTenantsToPopulateCompleted;

		public event EventHandler<QueryTenantsToValidateCompletedEventArgs> QueryTenantsToValidateCompleted;

		public event EventHandler<QueryTenantsToValidateByScenarioCompletedEventArgs> QueryTenantsToValidateByScenarioCompleted;

		public event EventHandler<AsyncCompletedEventArgs> UpdateTenantPopulationStatusCompleted;

		public event EventHandler<AsyncCompletedEventArgs> UpdateTenantPopulationStatusWithScenarioCompleted;

		public event EventHandler<AsyncCompletedEventArgs> UpdateTenantValidationStatusCompleted;

		public event EventHandler<AsyncCompletedEventArgs> UpdateTenantValidationStatusWithReasonCompleted;

		public Tenant[] QueryTenantsToPopulate(PopulationStatus status)
		{
			return base.Channel.QueryTenantsToPopulate(status);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginQueryTenantsToPopulate(PopulationStatus status, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginQueryTenantsToPopulate(status, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Tenant[] EndQueryTenantsToPopulate(IAsyncResult result)
		{
			return base.Channel.EndQueryTenantsToPopulate(result);
		}

		private IAsyncResult OnBeginQueryTenantsToPopulate(object[] inValues, AsyncCallback callback, object asyncState)
		{
			PopulationStatus status = (PopulationStatus)inValues[0];
			return this.BeginQueryTenantsToPopulate(status, callback, asyncState);
		}

		private object[] OnEndQueryTenantsToPopulate(IAsyncResult result)
		{
			Tenant[] array = this.EndQueryTenantsToPopulate(result);
			return new object[]
			{
				array
			};
		}

		private void OnQueryTenantsToPopulateCompleted(object state)
		{
			if (this.QueryTenantsToPopulateCompleted != null)
			{
				ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs)state;
				this.QueryTenantsToPopulateCompleted(this, new QueryTenantsToPopulateCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void QueryTenantsToPopulateAsync(PopulationStatus status)
		{
			this.QueryTenantsToPopulateAsync(status, null);
		}

		public void QueryTenantsToPopulateAsync(PopulationStatus status, object userState)
		{
			if (this.onBeginQueryTenantsToPopulateDelegate == null)
			{
				this.onBeginQueryTenantsToPopulateDelegate = new ClientBase<ITestTenantManagement>.BeginOperationDelegate(this.OnBeginQueryTenantsToPopulate);
			}
			if (this.onEndQueryTenantsToPopulateDelegate == null)
			{
				this.onEndQueryTenantsToPopulateDelegate = new ClientBase<ITestTenantManagement>.EndOperationDelegate(this.OnEndQueryTenantsToPopulate);
			}
			if (this.onQueryTenantsToPopulateCompletedDelegate == null)
			{
				this.onQueryTenantsToPopulateCompletedDelegate = new SendOrPostCallback(this.OnQueryTenantsToPopulateCompleted);
			}
			base.InvokeAsync(this.onBeginQueryTenantsToPopulateDelegate, new object[]
			{
				status
			}, this.onEndQueryTenantsToPopulateDelegate, this.onQueryTenantsToPopulateCompletedDelegate, userState);
		}

		public Tenant[] QueryTenantsToValidate(ValidationStatus status)
		{
			return base.Channel.QueryTenantsToValidate(status);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginQueryTenantsToValidate(ValidationStatus status, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginQueryTenantsToValidate(status, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Tenant[] EndQueryTenantsToValidate(IAsyncResult result)
		{
			return base.Channel.EndQueryTenantsToValidate(result);
		}

		private IAsyncResult OnBeginQueryTenantsToValidate(object[] inValues, AsyncCallback callback, object asyncState)
		{
			ValidationStatus status = (ValidationStatus)inValues[0];
			return this.BeginQueryTenantsToValidate(status, callback, asyncState);
		}

		private object[] OnEndQueryTenantsToValidate(IAsyncResult result)
		{
			Tenant[] array = this.EndQueryTenantsToValidate(result);
			return new object[]
			{
				array
			};
		}

		private void OnQueryTenantsToValidateCompleted(object state)
		{
			if (this.QueryTenantsToValidateCompleted != null)
			{
				ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs)state;
				this.QueryTenantsToValidateCompleted(this, new QueryTenantsToValidateCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void QueryTenantsToValidateAsync(ValidationStatus status)
		{
			this.QueryTenantsToValidateAsync(status, null);
		}

		public void QueryTenantsToValidateAsync(ValidationStatus status, object userState)
		{
			if (this.onBeginQueryTenantsToValidateDelegate == null)
			{
				this.onBeginQueryTenantsToValidateDelegate = new ClientBase<ITestTenantManagement>.BeginOperationDelegate(this.OnBeginQueryTenantsToValidate);
			}
			if (this.onEndQueryTenantsToValidateDelegate == null)
			{
				this.onEndQueryTenantsToValidateDelegate = new ClientBase<ITestTenantManagement>.EndOperationDelegate(this.OnEndQueryTenantsToValidate);
			}
			if (this.onQueryTenantsToValidateCompletedDelegate == null)
			{
				this.onQueryTenantsToValidateCompletedDelegate = new SendOrPostCallback(this.OnQueryTenantsToValidateCompleted);
			}
			base.InvokeAsync(this.onBeginQueryTenantsToValidateDelegate, new object[]
			{
				status
			}, this.onEndQueryTenantsToValidateDelegate, this.onQueryTenantsToValidateCompletedDelegate, userState);
		}

		public Tenant[] QueryTenantsToValidateByScenario(ValidationStatus status, string scenario)
		{
			return base.Channel.QueryTenantsToValidateByScenario(status, scenario);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginQueryTenantsToValidateByScenario(ValidationStatus status, string scenario, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginQueryTenantsToValidateByScenario(status, scenario, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Tenant[] EndQueryTenantsToValidateByScenario(IAsyncResult result)
		{
			return base.Channel.EndQueryTenantsToValidateByScenario(result);
		}

		private IAsyncResult OnBeginQueryTenantsToValidateByScenario(object[] inValues, AsyncCallback callback, object asyncState)
		{
			ValidationStatus status = (ValidationStatus)inValues[0];
			string scenario = (string)inValues[1];
			return this.BeginQueryTenantsToValidateByScenario(status, scenario, callback, asyncState);
		}

		private object[] OnEndQueryTenantsToValidateByScenario(IAsyncResult result)
		{
			Tenant[] array = this.EndQueryTenantsToValidateByScenario(result);
			return new object[]
			{
				array
			};
		}

		private void OnQueryTenantsToValidateByScenarioCompleted(object state)
		{
			if (this.QueryTenantsToValidateByScenarioCompleted != null)
			{
				ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs)state;
				this.QueryTenantsToValidateByScenarioCompleted(this, new QueryTenantsToValidateByScenarioCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void QueryTenantsToValidateByScenarioAsync(ValidationStatus status, string scenario)
		{
			this.QueryTenantsToValidateByScenarioAsync(status, scenario, null);
		}

		public void QueryTenantsToValidateByScenarioAsync(ValidationStatus status, string scenario, object userState)
		{
			if (this.onBeginQueryTenantsToValidateByScenarioDelegate == null)
			{
				this.onBeginQueryTenantsToValidateByScenarioDelegate = new ClientBase<ITestTenantManagement>.BeginOperationDelegate(this.OnBeginQueryTenantsToValidateByScenario);
			}
			if (this.onEndQueryTenantsToValidateByScenarioDelegate == null)
			{
				this.onEndQueryTenantsToValidateByScenarioDelegate = new ClientBase<ITestTenantManagement>.EndOperationDelegate(this.OnEndQueryTenantsToValidateByScenario);
			}
			if (this.onQueryTenantsToValidateByScenarioCompletedDelegate == null)
			{
				this.onQueryTenantsToValidateByScenarioCompletedDelegate = new SendOrPostCallback(this.OnQueryTenantsToValidateByScenarioCompleted);
			}
			base.InvokeAsync(this.onBeginQueryTenantsToValidateByScenarioDelegate, new object[]
			{
				status,
				scenario
			}, this.onEndQueryTenantsToValidateByScenarioDelegate, this.onQueryTenantsToValidateByScenarioCompletedDelegate, userState);
		}

		public void UpdateTenantPopulationStatus(Guid tenantId, PopulationStatus status)
		{
			base.Channel.UpdateTenantPopulationStatus(tenantId, status);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginUpdateTenantPopulationStatus(Guid tenantId, PopulationStatus status, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginUpdateTenantPopulationStatus(tenantId, status, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndUpdateTenantPopulationStatus(IAsyncResult result)
		{
			base.Channel.EndUpdateTenantPopulationStatus(result);
		}

		private IAsyncResult OnBeginUpdateTenantPopulationStatus(object[] inValues, AsyncCallback callback, object asyncState)
		{
			Guid tenantId = (Guid)inValues[0];
			PopulationStatus status = (PopulationStatus)inValues[1];
			return this.BeginUpdateTenantPopulationStatus(tenantId, status, callback, asyncState);
		}

		private object[] OnEndUpdateTenantPopulationStatus(IAsyncResult result)
		{
			this.EndUpdateTenantPopulationStatus(result);
			return null;
		}

		private void OnUpdateTenantPopulationStatusCompleted(object state)
		{
			if (this.UpdateTenantPopulationStatusCompleted != null)
			{
				ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs)state;
				this.UpdateTenantPopulationStatusCompleted(this, new AsyncCompletedEventArgs(invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void UpdateTenantPopulationStatusAsync(Guid tenantId, PopulationStatus status)
		{
			this.UpdateTenantPopulationStatusAsync(tenantId, status, null);
		}

		public void UpdateTenantPopulationStatusAsync(Guid tenantId, PopulationStatus status, object userState)
		{
			if (this.onBeginUpdateTenantPopulationStatusDelegate == null)
			{
				this.onBeginUpdateTenantPopulationStatusDelegate = new ClientBase<ITestTenantManagement>.BeginOperationDelegate(this.OnBeginUpdateTenantPopulationStatus);
			}
			if (this.onEndUpdateTenantPopulationStatusDelegate == null)
			{
				this.onEndUpdateTenantPopulationStatusDelegate = new ClientBase<ITestTenantManagement>.EndOperationDelegate(this.OnEndUpdateTenantPopulationStatus);
			}
			if (this.onUpdateTenantPopulationStatusCompletedDelegate == null)
			{
				this.onUpdateTenantPopulationStatusCompletedDelegate = new SendOrPostCallback(this.OnUpdateTenantPopulationStatusCompleted);
			}
			base.InvokeAsync(this.onBeginUpdateTenantPopulationStatusDelegate, new object[]
			{
				tenantId,
				status
			}, this.onEndUpdateTenantPopulationStatusDelegate, this.onUpdateTenantPopulationStatusCompletedDelegate, userState);
		}

		public void UpdateTenantPopulationStatusWithScenario(Guid tenantId, PopulationStatus status, string scenario, string comment)
		{
			base.Channel.UpdateTenantPopulationStatusWithScenario(tenantId, status, scenario, comment);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginUpdateTenantPopulationStatusWithScenario(Guid tenantId, PopulationStatus status, string scenario, string comment, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginUpdateTenantPopulationStatusWithScenario(tenantId, status, scenario, comment, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndUpdateTenantPopulationStatusWithScenario(IAsyncResult result)
		{
			base.Channel.EndUpdateTenantPopulationStatusWithScenario(result);
		}

		private IAsyncResult OnBeginUpdateTenantPopulationStatusWithScenario(object[] inValues, AsyncCallback callback, object asyncState)
		{
			Guid tenantId = (Guid)inValues[0];
			PopulationStatus status = (PopulationStatus)inValues[1];
			string scenario = (string)inValues[2];
			string comment = (string)inValues[3];
			return this.BeginUpdateTenantPopulationStatusWithScenario(tenantId, status, scenario, comment, callback, asyncState);
		}

		private object[] OnEndUpdateTenantPopulationStatusWithScenario(IAsyncResult result)
		{
			this.EndUpdateTenantPopulationStatusWithScenario(result);
			return null;
		}

		private void OnUpdateTenantPopulationStatusWithScenarioCompleted(object state)
		{
			if (this.UpdateTenantPopulationStatusWithScenarioCompleted != null)
			{
				ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs)state;
				this.UpdateTenantPopulationStatusWithScenarioCompleted(this, new AsyncCompletedEventArgs(invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void UpdateTenantPopulationStatusWithScenarioAsync(Guid tenantId, PopulationStatus status, string scenario, string comment)
		{
			this.UpdateTenantPopulationStatusWithScenarioAsync(tenantId, status, scenario, comment, null);
		}

		public void UpdateTenantPopulationStatusWithScenarioAsync(Guid tenantId, PopulationStatus status, string scenario, string comment, object userState)
		{
			if (this.onBeginUpdateTenantPopulationStatusWithScenarioDelegate == null)
			{
				this.onBeginUpdateTenantPopulationStatusWithScenarioDelegate = new ClientBase<ITestTenantManagement>.BeginOperationDelegate(this.OnBeginUpdateTenantPopulationStatusWithScenario);
			}
			if (this.onEndUpdateTenantPopulationStatusWithScenarioDelegate == null)
			{
				this.onEndUpdateTenantPopulationStatusWithScenarioDelegate = new ClientBase<ITestTenantManagement>.EndOperationDelegate(this.OnEndUpdateTenantPopulationStatusWithScenario);
			}
			if (this.onUpdateTenantPopulationStatusWithScenarioCompletedDelegate == null)
			{
				this.onUpdateTenantPopulationStatusWithScenarioCompletedDelegate = new SendOrPostCallback(this.OnUpdateTenantPopulationStatusWithScenarioCompleted);
			}
			base.InvokeAsync(this.onBeginUpdateTenantPopulationStatusWithScenarioDelegate, new object[]
			{
				tenantId,
				status,
				scenario,
				comment
			}, this.onEndUpdateTenantPopulationStatusWithScenarioDelegate, this.onUpdateTenantPopulationStatusWithScenarioCompletedDelegate, userState);
		}

		public void UpdateTenantValidationStatus(Guid tenantId, ValidationStatus status, int? office15BugId)
		{
			base.Channel.UpdateTenantValidationStatus(tenantId, status, office15BugId);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginUpdateTenantValidationStatus(Guid tenantId, ValidationStatus status, int? office15BugId, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginUpdateTenantValidationStatus(tenantId, status, office15BugId, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndUpdateTenantValidationStatus(IAsyncResult result)
		{
			base.Channel.EndUpdateTenantValidationStatus(result);
		}

		private IAsyncResult OnBeginUpdateTenantValidationStatus(object[] inValues, AsyncCallback callback, object asyncState)
		{
			Guid tenantId = (Guid)inValues[0];
			ValidationStatus status = (ValidationStatus)inValues[1];
			int? office15BugId = (int?)inValues[2];
			return this.BeginUpdateTenantValidationStatus(tenantId, status, office15BugId, callback, asyncState);
		}

		private object[] OnEndUpdateTenantValidationStatus(IAsyncResult result)
		{
			this.EndUpdateTenantValidationStatus(result);
			return null;
		}

		private void OnUpdateTenantValidationStatusCompleted(object state)
		{
			if (this.UpdateTenantValidationStatusCompleted != null)
			{
				ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs)state;
				this.UpdateTenantValidationStatusCompleted(this, new AsyncCompletedEventArgs(invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void UpdateTenantValidationStatusAsync(Guid tenantId, ValidationStatus status, int? office15BugId)
		{
			this.UpdateTenantValidationStatusAsync(tenantId, status, office15BugId, null);
		}

		public void UpdateTenantValidationStatusAsync(Guid tenantId, ValidationStatus status, int? office15BugId, object userState)
		{
			if (this.onBeginUpdateTenantValidationStatusDelegate == null)
			{
				this.onBeginUpdateTenantValidationStatusDelegate = new ClientBase<ITestTenantManagement>.BeginOperationDelegate(this.OnBeginUpdateTenantValidationStatus);
			}
			if (this.onEndUpdateTenantValidationStatusDelegate == null)
			{
				this.onEndUpdateTenantValidationStatusDelegate = new ClientBase<ITestTenantManagement>.EndOperationDelegate(this.OnEndUpdateTenantValidationStatus);
			}
			if (this.onUpdateTenantValidationStatusCompletedDelegate == null)
			{
				this.onUpdateTenantValidationStatusCompletedDelegate = new SendOrPostCallback(this.OnUpdateTenantValidationStatusCompleted);
			}
			base.InvokeAsync(this.onBeginUpdateTenantValidationStatusDelegate, new object[]
			{
				tenantId,
				status,
				office15BugId
			}, this.onEndUpdateTenantValidationStatusDelegate, this.onUpdateTenantValidationStatusCompletedDelegate, userState);
		}

		public void UpdateTenantValidationStatusWithReason(Guid tenantId, ValidationStatus status, string failureReason)
		{
			base.Channel.UpdateTenantValidationStatusWithReason(tenantId, status, failureReason);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginUpdateTenantValidationStatusWithReason(Guid tenantId, ValidationStatus status, string failureReason, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginUpdateTenantValidationStatusWithReason(tenantId, status, failureReason, callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndUpdateTenantValidationStatusWithReason(IAsyncResult result)
		{
			base.Channel.EndUpdateTenantValidationStatusWithReason(result);
		}

		private IAsyncResult OnBeginUpdateTenantValidationStatusWithReason(object[] inValues, AsyncCallback callback, object asyncState)
		{
			Guid tenantId = (Guid)inValues[0];
			ValidationStatus status = (ValidationStatus)inValues[1];
			string failureReason = (string)inValues[2];
			return this.BeginUpdateTenantValidationStatusWithReason(tenantId, status, failureReason, callback, asyncState);
		}

		private object[] OnEndUpdateTenantValidationStatusWithReason(IAsyncResult result)
		{
			this.EndUpdateTenantValidationStatusWithReason(result);
			return null;
		}

		private void OnUpdateTenantValidationStatusWithReasonCompleted(object state)
		{
			if (this.UpdateTenantValidationStatusWithReasonCompleted != null)
			{
				ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ITestTenantManagement>.InvokeAsyncCompletedEventArgs)state;
				this.UpdateTenantValidationStatusWithReasonCompleted(this, new AsyncCompletedEventArgs(invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void UpdateTenantValidationStatusWithReasonAsync(Guid tenantId, ValidationStatus status, string failureReason)
		{
			this.UpdateTenantValidationStatusWithReasonAsync(tenantId, status, failureReason, null);
		}

		public void UpdateTenantValidationStatusWithReasonAsync(Guid tenantId, ValidationStatus status, string failureReason, object userState)
		{
			if (this.onBeginUpdateTenantValidationStatusWithReasonDelegate == null)
			{
				this.onBeginUpdateTenantValidationStatusWithReasonDelegate = new ClientBase<ITestTenantManagement>.BeginOperationDelegate(this.OnBeginUpdateTenantValidationStatusWithReason);
			}
			if (this.onEndUpdateTenantValidationStatusWithReasonDelegate == null)
			{
				this.onEndUpdateTenantValidationStatusWithReasonDelegate = new ClientBase<ITestTenantManagement>.EndOperationDelegate(this.OnEndUpdateTenantValidationStatusWithReason);
			}
			if (this.onUpdateTenantValidationStatusWithReasonCompletedDelegate == null)
			{
				this.onUpdateTenantValidationStatusWithReasonCompletedDelegate = new SendOrPostCallback(this.OnUpdateTenantValidationStatusWithReasonCompleted);
			}
			base.InvokeAsync(this.onBeginUpdateTenantValidationStatusWithReasonDelegate, new object[]
			{
				tenantId,
				status,
				failureReason
			}, this.onEndUpdateTenantValidationStatusWithReasonDelegate, this.onUpdateTenantValidationStatusWithReasonCompletedDelegate, userState);
		}

		private ClientBase<ITestTenantManagement>.BeginOperationDelegate onBeginQueryTenantsToPopulateDelegate;

		private ClientBase<ITestTenantManagement>.EndOperationDelegate onEndQueryTenantsToPopulateDelegate;

		private SendOrPostCallback onQueryTenantsToPopulateCompletedDelegate;

		private ClientBase<ITestTenantManagement>.BeginOperationDelegate onBeginQueryTenantsToValidateDelegate;

		private ClientBase<ITestTenantManagement>.EndOperationDelegate onEndQueryTenantsToValidateDelegate;

		private SendOrPostCallback onQueryTenantsToValidateCompletedDelegate;

		private ClientBase<ITestTenantManagement>.BeginOperationDelegate onBeginQueryTenantsToValidateByScenarioDelegate;

		private ClientBase<ITestTenantManagement>.EndOperationDelegate onEndQueryTenantsToValidateByScenarioDelegate;

		private SendOrPostCallback onQueryTenantsToValidateByScenarioCompletedDelegate;

		private ClientBase<ITestTenantManagement>.BeginOperationDelegate onBeginUpdateTenantPopulationStatusDelegate;

		private ClientBase<ITestTenantManagement>.EndOperationDelegate onEndUpdateTenantPopulationStatusDelegate;

		private SendOrPostCallback onUpdateTenantPopulationStatusCompletedDelegate;

		private ClientBase<ITestTenantManagement>.BeginOperationDelegate onBeginUpdateTenantPopulationStatusWithScenarioDelegate;

		private ClientBase<ITestTenantManagement>.EndOperationDelegate onEndUpdateTenantPopulationStatusWithScenarioDelegate;

		private SendOrPostCallback onUpdateTenantPopulationStatusWithScenarioCompletedDelegate;

		private ClientBase<ITestTenantManagement>.BeginOperationDelegate onBeginUpdateTenantValidationStatusDelegate;

		private ClientBase<ITestTenantManagement>.EndOperationDelegate onEndUpdateTenantValidationStatusDelegate;

		private SendOrPostCallback onUpdateTenantValidationStatusCompletedDelegate;

		private ClientBase<ITestTenantManagement>.BeginOperationDelegate onBeginUpdateTenantValidationStatusWithReasonDelegate;

		private ClientBase<ITestTenantManagement>.EndOperationDelegate onEndUpdateTenantValidationStatusWithReasonDelegate;

		private SendOrPostCallback onUpdateTenantValidationStatusWithReasonCompletedDelegate;
	}
}
