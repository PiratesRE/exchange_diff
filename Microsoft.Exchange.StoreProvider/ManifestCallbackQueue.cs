using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ManifestCallbackQueue<TCallback> where TCallback : class
	{
		public int Count
		{
			get
			{
				return this.callbackInvocations.Count;
			}
		}

		public void Enqueue(ManifestCallbackQueue<TCallback>.CallbackInvocation callbackInvocation)
		{
			this.callbackInvocations.Enqueue(callbackInvocation);
		}

		public ManifestCallbackStatus Execute(TCallback callback)
		{
			return ManifestCallbackQueue<TCallback>.Execute(callback, this.DequeueCallbackInvocations());
		}

		public ManifestCallbackStatus ExecuteNoDequeue(TCallback callback)
		{
			return ManifestCallbackQueue<TCallback>.Execute(callback, this.callbackInvocations);
		}

		private IEnumerable<ManifestCallbackQueue<TCallback>.CallbackInvocation> DequeueCallbackInvocations()
		{
			while (this.callbackInvocations.Count > 0)
			{
				yield return this.callbackInvocations.Dequeue();
			}
			yield break;
		}

		private static ManifestCallbackStatus Execute(TCallback callback, IEnumerable<ManifestCallbackQueue<TCallback>.CallbackInvocation> callbackInvocations)
		{
			ManifestCallbackStatus manifestCallbackStatus = ManifestCallbackStatus.Continue;
			foreach (ManifestCallbackQueue<TCallback>.CallbackInvocation callbackInvocation in callbackInvocations)
			{
				manifestCallbackStatus = callbackInvocation(callback);
				if (manifestCallbackStatus != ManifestCallbackStatus.Continue)
				{
					break;
				}
			}
			return manifestCallbackStatus;
		}

		private readonly Queue<ManifestCallbackQueue<TCallback>.CallbackInvocation> callbackInvocations = new Queue<ManifestCallbackQueue<TCallback>.CallbackInvocation>();

		public delegate ManifestCallbackStatus CallbackInvocation(TCallback callback);
	}
}
