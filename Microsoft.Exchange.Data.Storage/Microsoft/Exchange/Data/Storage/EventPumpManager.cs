using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class EventPumpManager
	{
		private EventPumpManager()
		{
		}

		internal void RegisterEventSink(StoreSession session, EventSink eventSink)
		{
			EventPump eventPump = this.GetEventPump(session);
			bool flag = false;
			try
			{
				eventPump.AddEventSink(eventSink);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					eventPump.Release();
					eventPump = null;
				}
			}
		}

		internal static EventPumpManager Instance
		{
			get
			{
				return EventPumpManager.instance;
			}
		}

		internal void RemoveEventPump(EventPump eventPump)
		{
			bool flag = false;
			lock (this.thisLock)
			{
				if (eventPump.ReferenceCount == 0)
				{
					this.RemoveEventPumpFromList(eventPump);
					flag = true;
				}
			}
			if (flag)
			{
				eventPump.Dispose();
			}
		}

		internal void RemoveBrokenEventPump(EventPump eventPump)
		{
			this.RemoveEventPumpFromList(eventPump);
		}

		private void RemoveEventPumpFromList(EventPump eventPump)
		{
			lock (this.thisLock)
			{
				EventPump eventPump2 = null;
				if (this.TryGetEventPump(eventPump.MdbGuid, out eventPump2) && eventPump == eventPump2)
				{
					this.eventPumps.Remove(eventPump.MdbGuid);
				}
			}
		}

		private EventPump GetEventPump(StoreSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			string text = session.IsRemote ? null : session.ServerFullyQualifiedDomainName;
			if (text == null)
			{
				throw new NotSupportedException("Reliable notifications are not supported for remote connections.");
			}
			int num = text.IndexOf('.');
			if (num == -1)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidFullyQualifiedServerName);
			}
			string server = text.Substring(0, num);
			Guid mdbGuid = session.MdbGuid;
			if (mdbGuid == Guid.Empty)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidMdbGuid);
			}
			EventPump eventPump = null;
			lock (this.thisLock)
			{
				if (this.TryGetEventPump(mdbGuid, out eventPump))
				{
					eventPump.AddRef();
				}
			}
			EventPump eventPump2 = null;
			try
			{
				if (eventPump == null)
				{
					eventPump2 = new EventPump(this, server, mdbGuid);
					eventPump2.AddRef();
					lock (this.thisLock)
					{
						if (this.TryGetEventPump(mdbGuid, out eventPump))
						{
							eventPump.AddRef();
						}
						else
						{
							if (eventPump2.Exception != null)
							{
								throw eventPump2.Exception;
							}
							this.AddEventPump(mdbGuid, eventPump2);
							eventPump = eventPump2;
							eventPump2 = null;
						}
					}
				}
			}
			finally
			{
				if (eventPump2 != null)
				{
					eventPump2.Release();
					eventPump2 = null;
				}
			}
			return eventPump;
		}

		private bool TryGetEventPump(Guid mdbGuid, out EventPump eventPump)
		{
			eventPump = null;
			lock (this.thisLock)
			{
				WeakReference weakReference = null;
				if (this.eventPumps.TryGetValue(mdbGuid, out weakReference))
				{
					eventPump = (EventPump)weakReference.Target;
					if (weakReference.IsAlive)
					{
						return true;
					}
					this.eventPumps.Remove(mdbGuid);
				}
			}
			return false;
		}

		private void AddEventPump(Guid mdbGuid, EventPump eventPump)
		{
			lock (this.thisLock)
			{
				this.eventPumps.Add(mdbGuid, new WeakReference(eventPump));
			}
		}

		private Dictionary<Guid, WeakReference> eventPumps = new Dictionary<Guid, WeakReference>();

		private static EventPumpManager instance = new EventPumpManager();

		private readonly object thisLock = new object();
	}
}
