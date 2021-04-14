using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class WrapperBase<T> : DisposableWrapper<T> where T : class, IDisposable
	{
		public WrapperBase(T wrappedObject, CommonUtils.CreateContextDelegate createContext) : base(wrappedObject, true)
		{
			this.ProviderInfo = new ProviderInfo();
			if (createContext == null)
			{
				createContext = new CommonUtils.CreateContextDelegate(this.DefaultCreateContext);
			}
			this.CreateContext = createContext;
		}

		public CommonUtils.CreateContextDelegate CreateContext { get; protected set; }

		public ProviderInfo ProviderInfo { get; protected set; }

		public void UpdateDuration(string callName, TimeSpan duration)
		{
			DurationInfo durationInfo = this.ProviderInfo.Durations.Find((DurationInfo d) => d.Name.Equals(callName));
			if (durationInfo != null)
			{
				durationInfo.Duration = durationInfo.Duration.Add(duration);
				return;
			}
			this.ProviderInfo.Durations.Add(new DurationInfo
			{
				Name = callName,
				Duration = duration
			});
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			this.CreateContext("WrapperBase.Dispose", new DataContext[0]).Execute(delegate
			{
				this.<>n__FabricatedMethod7(calledFromDispose);
			}, true);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WrapperBase<T>>(this);
		}

		private ExecutionContextWrapper DefaultCreateContext(string callName, params DataContext[] additionalContexts)
		{
			return new ExecutionContextWrapper(new CommonUtils.UpdateDuration(this.UpdateDuration), callName, additionalContexts);
		}
	}
}
