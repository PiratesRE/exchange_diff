using System;

namespace System.Diagnostics.Tracing
{
	internal class EventDispatcher
	{
		internal EventDispatcher(EventDispatcher next, bool[] eventEnabled, EventListener listener)
		{
			this.m_Next = next;
			this.m_EventEnabled = eventEnabled;
			this.m_Listener = listener;
		}

		internal readonly EventListener m_Listener;

		internal bool[] m_EventEnabled;

		internal bool m_activityFilteringEnabled;

		internal EventDispatcher m_Next;
	}
}
