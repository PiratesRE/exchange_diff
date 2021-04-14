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
	public class MonitorableClient : ClientBase<IMonitorable>, IMonitorable
	{
		public MonitorableClient()
		{
		}

		public MonitorableClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public MonitorableClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public MonitorableClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public MonitorableClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public event EventHandler<PingCompletedEventArgs> PingCompleted;

		public string Ping()
		{
			return base.Channel.Ping();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginPing(AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginPing(callback, asyncState);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string EndPing(IAsyncResult result)
		{
			return base.Channel.EndPing(result);
		}

		private IAsyncResult OnBeginPing(object[] inValues, AsyncCallback callback, object asyncState)
		{
			return this.BeginPing(callback, asyncState);
		}

		private object[] OnEndPing(IAsyncResult result)
		{
			string text = this.EndPing(result);
			return new object[]
			{
				text
			};
		}

		private void OnPingCompleted(object state)
		{
			if (this.PingCompleted != null)
			{
				ClientBase<IMonitorable>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IMonitorable>.InvokeAsyncCompletedEventArgs)state;
				this.PingCompleted(this, new PingCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
			}
		}

		public void PingAsync()
		{
			this.PingAsync(null);
		}

		public void PingAsync(object userState)
		{
			if (this.onBeginPingDelegate == null)
			{
				this.onBeginPingDelegate = new ClientBase<IMonitorable>.BeginOperationDelegate(this.OnBeginPing);
			}
			if (this.onEndPingDelegate == null)
			{
				this.onEndPingDelegate = new ClientBase<IMonitorable>.EndOperationDelegate(this.OnEndPing);
			}
			if (this.onPingCompletedDelegate == null)
			{
				this.onPingCompletedDelegate = new SendOrPostCallback(this.OnPingCompleted);
			}
			base.InvokeAsync(this.onBeginPingDelegate, null, this.onEndPingDelegate, this.onPingCompletedDelegate, userState);
		}

		private ClientBase<IMonitorable>.BeginOperationDelegate onBeginPingDelegate;

		private ClientBase<IMonitorable>.EndOperationDelegate onEndPingDelegate;

		private SendOrPostCallback onPingCompletedDelegate;
	}
}
