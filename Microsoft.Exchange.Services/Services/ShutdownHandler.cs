using System;
using System.Collections.Generic;
using System.Web.Hosting;

namespace Microsoft.Exchange.Services
{
	internal class ShutdownHandler : IRegisteredObject
	{
		private ShutdownHandler()
		{
			HostingEnvironment.RegisterObject(this);
		}

		internal static ShutdownHandler Singleton
		{
			get
			{
				return ShutdownHandler.singleton;
			}
		}

		internal void Add(IDisposable disposable)
		{
			lock (this.disposableList)
			{
				this.disposableList.Add(disposable);
			}
		}

		void IRegisteredObject.Stop(bool immediate)
		{
			try
			{
				lock (this.disposableList)
				{
					foreach (IDisposable disposable in this.disposableList)
					{
						disposable.Dispose();
					}
					this.disposableList.Clear();
				}
			}
			finally
			{
				HostingEnvironment.UnregisterObject(this);
			}
		}

		private static ShutdownHandler singleton = new ShutdownHandler();

		private List<IDisposable> disposableList = new List<IDisposable>();
	}
}
