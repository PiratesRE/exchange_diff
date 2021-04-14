using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ManifestCallbackHelperBase<TCallback> where TCallback : class
	{
		protected ManifestCallbackHelperBase()
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(32))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(15538, 32, (long)this.GetHashCode(), "ManifestCallbackHelper.ManifestCallbackHelper: this={0}", TraceUtils.MakeHash(this));
			}
		}

		public ManifestCallbackQueue<TCallback> Changes
		{
			get
			{
				return this.changeList;
			}
		}

		public ManifestCallbackQueue<TCallback> Deletes
		{
			get
			{
				return this.deleteList;
			}
		}

		public ManifestStatus DoCallbacks(TCallback callback, params ManifestCallbackQueue<TCallback>[] callbackQueues)
		{
			ManifestCallbackStatus manifestCallbackStatus = ManifestCallbackStatus.Continue;
			foreach (ManifestCallbackQueue<TCallback> manifestCallbackQueue in callbackQueues)
			{
				manifestCallbackStatus = manifestCallbackQueue.Execute(callback);
				if (manifestCallbackStatus != ManifestCallbackStatus.Continue)
				{
					break;
				}
			}
			switch (manifestCallbackStatus)
			{
			case ManifestCallbackStatus.Continue:
				return ManifestStatus.Done;
			case ManifestCallbackStatus.Stop:
				return ManifestStatus.Stopped;
			case ManifestCallbackStatus.Yield:
				return ManifestStatus.Yielded;
			default:
				throw new InvalidOperationException();
			}
		}

		private readonly ManifestCallbackQueue<TCallback> changeList = new ManifestCallbackQueue<TCallback>();

		private readonly ManifestCallbackQueue<TCallback> deleteList = new ManifestCallbackQueue<TCallback>();
	}
}
