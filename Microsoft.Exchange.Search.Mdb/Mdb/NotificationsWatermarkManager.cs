using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Mdb
{
	internal sealed class NotificationsWatermarkManager : WatermarkManager<long>
	{
		internal NotificationsWatermarkManager(int batchSize) : base(batchSize)
		{
			base.DiagnosticsSession.ComponentName = "NotificationsWatermarkManager";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.NotificationsWatermarkManagerTracer;
			this.stateUpdateList = new SortedDictionary<long, bool>();
		}

		internal void Add(long eventId, bool interested)
		{
			lock (this.stateUpdateList)
			{
				base.DiagnosticsSession.TraceDebug<long>("Add a new document (EventId={0})", eventId);
				this.stateUpdateList.Add(eventId, !interested);
			}
		}

		internal bool TryComplete(long eventId, out long newWatermark)
		{
			bool result;
			lock (this.stateUpdateList)
			{
				base.DiagnosticsSession.TraceDebug<long>("TryComplete: EventId = {0}", eventId);
				bool flag2;
				if (!this.stateUpdateList.TryGetValue(eventId, out flag2))
				{
					newWatermark = -1L;
					base.DiagnosticsSession.TraceDebug("State does not need to be updated.", new object[0]);
					result = false;
				}
				else
				{
					this.stateUpdateList[eventId] = true;
					if (base.TryFindNewWatermark(this.stateUpdateList, out newWatermark))
					{
						base.DiagnosticsSession.TraceDebug<long>("State needs to update to {0}.", newWatermark);
						result = true;
					}
					else
					{
						newWatermark = -1L;
						base.DiagnosticsSession.TraceDebug("State does not need to be updated.", new object[0]);
						result = false;
					}
				}
			}
			return result;
		}

		private readonly SortedDictionary<long, bool> stateUpdateList;
	}
}
