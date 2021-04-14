using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal abstract class WatermarkManager<T> where T : IComparable
	{
		protected WatermarkManager(int batchSize)
		{
			this.batchSize = batchSize;
			this.diagnosticsSession = Microsoft.Exchange.Search.Core.Diagnostics.DiagnosticsSession.CreateComponentDiagnosticsSession("WatermarkManager", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.CoreComponentTracer, (long)this.GetHashCode());
		}

		protected IDiagnosticsSession DiagnosticsSession
		{
			get
			{
				return this.diagnosticsSession;
			}
		}

		protected bool TryFindNewWatermark(SortedDictionary<T, bool> stateUpdateList, out T newWatermark)
		{
			this.diagnosticsSession.TraceDebug<int>("TryFindNewWatermark for {0} items", stateUpdateList.Count);
			List<T> list = null;
			T t = default(T);
			foreach (KeyValuePair<T, bool> keyValuePair in stateUpdateList)
			{
				T key = keyValuePair.Key;
				if (!keyValuePair.Value)
				{
					this.diagnosticsSession.TraceDebug<T>("Document (Id={0}) has not completed", key);
					break;
				}
				if (list == null)
				{
					list = new List<T>(Math.Min(this.batchSize, stateUpdateList.Count));
				}
				list.Add(key);
				t = key;
			}
			if (list == null)
			{
				newWatermark = default(T);
				return false;
			}
			this.diagnosticsSession.TraceDebug<T, int>("Last Id = {0}, Count={1}", t, list.Count);
			if (list.Count == stateUpdateList.Count)
			{
				stateUpdateList.Clear();
				newWatermark = t;
				return true;
			}
			if (list.Count >= this.batchSize)
			{
				foreach (T key2 in list)
				{
					stateUpdateList.Remove(key2);
				}
				newWatermark = t;
				return true;
			}
			newWatermark = default(T);
			return false;
		}

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly int batchSize;
	}
}
