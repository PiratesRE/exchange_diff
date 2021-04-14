using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Reflection;
using Microsoft.Win32;

namespace System.Diagnostics.Tracing
{
	[__DynamicallyInvokable]
	public class EventSource : IDisposable
	{
		[__DynamicallyInvokable]
		public string Name
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_name;
			}
		}

		[__DynamicallyInvokable]
		public Guid Guid
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_guid;
			}
		}

		[__DynamicallyInvokable]
		public bool IsEnabled()
		{
			return this.m_eventSourceEnabled;
		}

		[__DynamicallyInvokable]
		public bool IsEnabled(EventLevel level, EventKeywords keywords)
		{
			return this.IsEnabled(level, keywords, EventChannel.None);
		}

		[__DynamicallyInvokable]
		public bool IsEnabled(EventLevel level, EventKeywords keywords, EventChannel channel)
		{
			return this.m_eventSourceEnabled && this.IsEnabledCommon(this.m_eventSourceEnabled, this.m_level, this.m_matchAnyKeyword, level, keywords, channel);
		}

		[__DynamicallyInvokable]
		public EventSourceSettings Settings
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_config;
			}
		}

		[__DynamicallyInvokable]
		public static Guid GetGuid(Type eventSourceType)
		{
			if (eventSourceType == null)
			{
				throw new ArgumentNullException("eventSourceType");
			}
			EventSourceAttribute eventSourceAttribute = (EventSourceAttribute)EventSource.GetCustomAttributeHelper(eventSourceType, typeof(EventSourceAttribute), EventManifestOptions.None);
			string name = eventSourceType.Name;
			if (eventSourceAttribute != null)
			{
				if (eventSourceAttribute.Guid != null)
				{
					Guid empty = Guid.Empty;
					if (Guid.TryParse(eventSourceAttribute.Guid, out empty))
					{
						return empty;
					}
				}
				if (eventSourceAttribute.Name != null)
				{
					name = eventSourceAttribute.Name;
				}
			}
			if (name == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidTypeName"), "eventSourceType");
			}
			return EventSource.GenerateGuidFromName(name.ToUpperInvariant());
		}

		[__DynamicallyInvokable]
		public static string GetName(Type eventSourceType)
		{
			return EventSource.GetName(eventSourceType, EventManifestOptions.None);
		}

		[__DynamicallyInvokable]
		public static string GenerateManifest(Type eventSourceType, string assemblyPathToIncludeInManifest)
		{
			return EventSource.GenerateManifest(eventSourceType, assemblyPathToIncludeInManifest, EventManifestOptions.None);
		}

		[__DynamicallyInvokable]
		public static string GenerateManifest(Type eventSourceType, string assemblyPathToIncludeInManifest, EventManifestOptions flags)
		{
			if (eventSourceType == null)
			{
				throw new ArgumentNullException("eventSourceType");
			}
			byte[] array = EventSource.CreateManifestAndDescriptors(eventSourceType, assemblyPathToIncludeInManifest, null, flags);
			if (array != null)
			{
				return Encoding.UTF8.GetString(array, 0, array.Length);
			}
			return null;
		}

		[__DynamicallyInvokable]
		public static IEnumerable<EventSource> GetSources()
		{
			List<EventSource> list = new List<EventSource>();
			object eventListenersLock = EventListener.EventListenersLock;
			lock (eventListenersLock)
			{
				foreach (WeakReference weakReference in EventListener.s_EventSources)
				{
					EventSource eventSource = weakReference.Target as EventSource;
					if (eventSource != null && !eventSource.IsDisposed)
					{
						list.Add(eventSource);
					}
				}
			}
			return list;
		}

		[__DynamicallyInvokable]
		public static void SendCommand(EventSource eventSource, EventCommand command, IDictionary<string, string> commandArguments)
		{
			if (eventSource == null)
			{
				throw new ArgumentNullException("eventSource");
			}
			if (command <= EventCommand.Update && command != EventCommand.SendManifest)
			{
				throw new ArgumentException(Environment.GetResourceString("EventSource_InvalidCommand"), "command");
			}
			eventSource.SendCommand(null, 0, 0, command, true, EventLevel.LogAlways, EventKeywords.None, commandArguments);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static void SetCurrentThreadActivityId(Guid activityId)
		{
			Guid a = activityId;
			if (UnsafeNativeMethods.ManifestEtw.EventActivityIdControl(UnsafeNativeMethods.ManifestEtw.ActivityControl.EVENT_ACTIVITY_CTRL_GET_SET_ID, ref activityId) == 0)
			{
				Action<Guid> action = EventSource.s_activityDying;
				if (action != null && a != activityId)
				{
					if (activityId == Guid.Empty)
					{
						activityId = EventSource.FallbackActivityId;
					}
					action(activityId);
				}
			}
			if (TplEtwProvider.Log != null)
			{
				TplEtwProvider.Log.SetActivityId(activityId);
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static void SetCurrentThreadActivityId(Guid activityId, out Guid oldActivityThatWillContinue)
		{
			oldActivityThatWillContinue = activityId;
			UnsafeNativeMethods.ManifestEtw.EventActivityIdControl(UnsafeNativeMethods.ManifestEtw.ActivityControl.EVENT_ACTIVITY_CTRL_GET_SET_ID, ref oldActivityThatWillContinue);
			if (TplEtwProvider.Log != null)
			{
				TplEtwProvider.Log.SetActivityId(activityId);
			}
		}

		[__DynamicallyInvokable]
		public static Guid CurrentThreadActivityId
		{
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			get
			{
				Guid result = default(Guid);
				UnsafeNativeMethods.ManifestEtw.EventActivityIdControl(UnsafeNativeMethods.ManifestEtw.ActivityControl.EVENT_ACTIVITY_CTRL_GET_ID, ref result);
				return result;
			}
		}

		internal static Guid InternalCurrentThreadActivityId
		{
			[SecurityCritical]
			get
			{
				Guid guid = EventSource.CurrentThreadActivityId;
				if (guid == Guid.Empty)
				{
					guid = EventSource.FallbackActivityId;
				}
				return guid;
			}
		}

		internal static Guid FallbackActivityId
		{
			[SecurityCritical]
			get
			{
				return new Guid((uint)AppDomain.GetCurrentThreadId(), (ushort)EventSource.s_currentPid, (ushort)(EventSource.s_currentPid >> 16), 148, 27, 135, 213, 166, 92, 54, 100);
			}
		}

		[__DynamicallyInvokable]
		public Exception ConstructionException
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_constructionException;
			}
		}

		[__DynamicallyInvokable]
		public string GetTrait(string key)
		{
			if (this.m_traits != null)
			{
				for (int i = 0; i < this.m_traits.Length - 1; i += 2)
				{
					if (this.m_traits[i] == key)
					{
						return this.m_traits[i + 1];
					}
				}
			}
			return null;
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			return Environment.GetResourceString("EventSource_ToString", new object[]
			{
				this.Name,
				this.Guid
			});
		}

		public event EventHandler<EventCommandEventArgs> EventCommandExecuted
		{
			add
			{
				this.m_eventCommandExecuted = (EventHandler<EventCommandEventArgs>)Delegate.Combine(this.m_eventCommandExecuted, value);
				for (EventCommandEventArgs eventCommandEventArgs = this.m_deferredCommands; eventCommandEventArgs != null; eventCommandEventArgs = eventCommandEventArgs.nextCommand)
				{
					value(this, eventCommandEventArgs);
				}
			}
			remove
			{
				this.m_eventCommandExecuted = (EventHandler<EventCommandEventArgs>)Delegate.Remove(this.m_eventCommandExecuted, value);
			}
		}

		[__DynamicallyInvokable]
		protected EventSource() : this(EventSourceSettings.EtwManifestEventFormat)
		{
		}

		[__DynamicallyInvokable]
		protected EventSource(bool throwOnEventWriteErrors) : this(EventSourceSettings.EtwManifestEventFormat | (throwOnEventWriteErrors ? EventSourceSettings.ThrowOnEventWriteErrors : EventSourceSettings.Default))
		{
		}

		[__DynamicallyInvokable]
		protected EventSource(EventSourceSettings settings) : this(settings, null)
		{
		}

		[__DynamicallyInvokable]
		protected EventSource(EventSourceSettings settings, params string[] traits)
		{
			this.m_config = this.ValidateSettings(settings);
			Type type = base.GetType();
			this.Initialize(EventSource.GetGuid(type), EventSource.GetName(type), traits);
		}

		[__DynamicallyInvokable]
		protected virtual void OnEventCommand(EventCommandEventArgs command)
		{
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected void WriteEvent(int eventId)
		{
			this.WriteEventCore(eventId, 0, null);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, int arg1)
		{
			if (this.m_eventSourceEnabled)
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)1) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->DataPointer = (IntPtr)((void*)(&arg1));
				ptr->Size = 4;
				this.WriteEventCore(eventId, 1, ptr);
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, int arg1, int arg2)
		{
			if (this.m_eventSourceEnabled)
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)2) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->DataPointer = (IntPtr)((void*)(&arg1));
				ptr->Size = 4;
				ptr[1].DataPointer = (IntPtr)((void*)(&arg2));
				ptr[1].Size = 4;
				this.WriteEventCore(eventId, 2, ptr);
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, int arg1, int arg2, int arg3)
		{
			if (this.m_eventSourceEnabled)
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)3) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->DataPointer = (IntPtr)((void*)(&arg1));
				ptr->Size = 4;
				ptr[1].DataPointer = (IntPtr)((void*)(&arg2));
				ptr[1].Size = 4;
				ptr[2].DataPointer = (IntPtr)((void*)(&arg3));
				ptr[2].Size = 4;
				this.WriteEventCore(eventId, 3, ptr);
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, long arg1)
		{
			if (this.m_eventSourceEnabled)
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)1) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->DataPointer = (IntPtr)((void*)(&arg1));
				ptr->Size = 8;
				this.WriteEventCore(eventId, 1, ptr);
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, long arg1, long arg2)
		{
			if (this.m_eventSourceEnabled)
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)2) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->DataPointer = (IntPtr)((void*)(&arg1));
				ptr->Size = 8;
				ptr[1].DataPointer = (IntPtr)((void*)(&arg2));
				ptr[1].Size = 8;
				this.WriteEventCore(eventId, 2, ptr);
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, long arg1, long arg2, long arg3)
		{
			if (this.m_eventSourceEnabled)
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)3) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->DataPointer = (IntPtr)((void*)(&arg1));
				ptr->Size = 8;
				ptr[1].DataPointer = (IntPtr)((void*)(&arg2));
				ptr[1].Size = 8;
				ptr[2].DataPointer = (IntPtr)((void*)(&arg3));
				ptr[2].Size = 8;
				this.WriteEventCore(eventId, 3, ptr);
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, string arg1)
		{
			if (this.m_eventSourceEnabled)
			{
				if (arg1 == null)
				{
					arg1 = "";
				}
				fixed (string text = arg1)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					EventSource.EventData* ptr2 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)1) * (UIntPtr)sizeof(EventSource.EventData))];
					ptr2->DataPointer = (IntPtr)((void*)ptr);
					ptr2->Size = (arg1.Length + 1) * 2;
					this.WriteEventCore(eventId, 1, ptr2);
				}
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, string arg1, string arg2)
		{
			if (this.m_eventSourceEnabled)
			{
				if (arg1 == null)
				{
					arg1 = "";
				}
				if (arg2 == null)
				{
					arg2 = "";
				}
				fixed (string text = arg1)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text2 = arg2)
					{
						char* ptr2 = text2;
						if (ptr2 != null)
						{
							ptr2 += RuntimeHelpers.OffsetToStringData / 2;
						}
						EventSource.EventData* ptr3 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)2) * (UIntPtr)sizeof(EventSource.EventData))];
						ptr3->DataPointer = (IntPtr)((void*)ptr);
						ptr3->Size = (arg1.Length + 1) * 2;
						ptr3[1].DataPointer = (IntPtr)((void*)ptr2);
						ptr3[1].Size = (arg2.Length + 1) * 2;
						this.WriteEventCore(eventId, 2, ptr3);
					}
				}
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, string arg1, string arg2, string arg3)
		{
			if (this.m_eventSourceEnabled)
			{
				if (arg1 == null)
				{
					arg1 = "";
				}
				if (arg2 == null)
				{
					arg2 = "";
				}
				if (arg3 == null)
				{
					arg3 = "";
				}
				fixed (string text = arg1)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text2 = arg2)
					{
						char* ptr2 = text2;
						if (ptr2 != null)
						{
							ptr2 += RuntimeHelpers.OffsetToStringData / 2;
						}
						fixed (string text3 = arg3)
						{
							char* ptr3 = text3;
							if (ptr3 != null)
							{
								ptr3 += RuntimeHelpers.OffsetToStringData / 2;
							}
							EventSource.EventData* ptr4 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)3) * (UIntPtr)sizeof(EventSource.EventData))];
							ptr4->DataPointer = (IntPtr)((void*)ptr);
							ptr4->Size = (arg1.Length + 1) * 2;
							ptr4[1].DataPointer = (IntPtr)((void*)ptr2);
							ptr4[1].Size = (arg2.Length + 1) * 2;
							ptr4[2].DataPointer = (IntPtr)((void*)ptr3);
							ptr4[2].Size = (arg3.Length + 1) * 2;
							this.WriteEventCore(eventId, 3, ptr4);
						}
					}
				}
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, string arg1, int arg2)
		{
			if (this.m_eventSourceEnabled)
			{
				if (arg1 == null)
				{
					arg1 = "";
				}
				fixed (string text = arg1)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					EventSource.EventData* ptr2 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)2) * (UIntPtr)sizeof(EventSource.EventData))];
					ptr2->DataPointer = (IntPtr)((void*)ptr);
					ptr2->Size = (arg1.Length + 1) * 2;
					ptr2[1].DataPointer = (IntPtr)((void*)(&arg2));
					ptr2[1].Size = 4;
					this.WriteEventCore(eventId, 2, ptr2);
				}
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, string arg1, int arg2, int arg3)
		{
			if (this.m_eventSourceEnabled)
			{
				if (arg1 == null)
				{
					arg1 = "";
				}
				fixed (string text = arg1)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					EventSource.EventData* ptr2 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)3) * (UIntPtr)sizeof(EventSource.EventData))];
					ptr2->DataPointer = (IntPtr)((void*)ptr);
					ptr2->Size = (arg1.Length + 1) * 2;
					ptr2[1].DataPointer = (IntPtr)((void*)(&arg2));
					ptr2[1].Size = 4;
					ptr2[2].DataPointer = (IntPtr)((void*)(&arg3));
					ptr2[2].Size = 4;
					this.WriteEventCore(eventId, 3, ptr2);
				}
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, string arg1, long arg2)
		{
			if (this.m_eventSourceEnabled)
			{
				if (arg1 == null)
				{
					arg1 = "";
				}
				fixed (string text = arg1)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					EventSource.EventData* ptr2 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)2) * (UIntPtr)sizeof(EventSource.EventData))];
					ptr2->DataPointer = (IntPtr)((void*)ptr);
					ptr2->Size = (arg1.Length + 1) * 2;
					ptr2[1].DataPointer = (IntPtr)((void*)(&arg2));
					ptr2[1].Size = 8;
					this.WriteEventCore(eventId, 2, ptr2);
				}
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, long arg1, string arg2)
		{
			if (this.m_eventSourceEnabled)
			{
				if (arg2 == null)
				{
					arg2 = "";
				}
				fixed (string text = arg2)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					EventSource.EventData* ptr2 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)2) * (UIntPtr)sizeof(EventSource.EventData))];
					ptr2->DataPointer = (IntPtr)((void*)(&arg1));
					ptr2->Size = 8;
					ptr2[1].DataPointer = (IntPtr)((void*)ptr);
					ptr2[1].Size = (arg2.Length + 1) * 2;
					this.WriteEventCore(eventId, 2, ptr2);
				}
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, int arg1, string arg2)
		{
			if (this.m_eventSourceEnabled)
			{
				if (arg2 == null)
				{
					arg2 = "";
				}
				fixed (string text = arg2)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					EventSource.EventData* ptr2 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)2) * (UIntPtr)sizeof(EventSource.EventData))];
					ptr2->DataPointer = (IntPtr)((void*)(&arg1));
					ptr2->Size = 4;
					ptr2[1].DataPointer = (IntPtr)((void*)ptr);
					ptr2[1].Size = (arg2.Length + 1) * 2;
					this.WriteEventCore(eventId, 2, ptr2);
				}
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, byte[] arg1)
		{
			if (this.m_eventSourceEnabled)
			{
				if (arg1 == null)
				{
					arg1 = new byte[0];
				}
				int size = arg1.Length;
				fixed (byte* ptr = &arg1[0])
				{
					EventSource.EventData* ptr2 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)2) * (UIntPtr)sizeof(EventSource.EventData))];
					ptr2->DataPointer = (IntPtr)((void*)(&size));
					ptr2->Size = 4;
					ptr2[1].DataPointer = (IntPtr)((void*)ptr);
					ptr2[1].Size = size;
					this.WriteEventCore(eventId, 2, ptr2);
				}
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEvent(int eventId, long arg1, byte[] arg2)
		{
			if (this.m_eventSourceEnabled)
			{
				if (arg2 == null)
				{
					arg2 = new byte[0];
				}
				int size = arg2.Length;
				fixed (byte* ptr = &arg2[0])
				{
					EventSource.EventData* ptr2 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)3) * (UIntPtr)sizeof(EventSource.EventData))];
					ptr2->DataPointer = (IntPtr)((void*)(&arg1));
					ptr2->Size = 8;
					ptr2[1].DataPointer = (IntPtr)((void*)(&size));
					ptr2[1].Size = 4;
					ptr2[2].DataPointer = (IntPtr)((void*)ptr);
					ptr2[2].Size = size;
					this.WriteEventCore(eventId, 3, ptr2);
				}
			}
		}

		[SecurityCritical]
		[CLSCompliant(false)]
		protected unsafe void WriteEventCore(int eventId, int eventDataCount, EventSource.EventData* data)
		{
			this.WriteEventWithRelatedActivityIdCore(eventId, null, eventDataCount, data);
		}

		[SecurityCritical]
		[CLSCompliant(false)]
		protected unsafe void WriteEventWithRelatedActivityIdCore(int eventId, Guid* relatedActivityId, int eventDataCount, EventSource.EventData* data)
		{
			if (this.m_eventSourceEnabled)
			{
				try
				{
					if (relatedActivityId != null)
					{
						this.ValidateEventOpcodeForTransfer(ref this.m_eventData[eventId], this.m_eventData[eventId].Name);
					}
					EventOpcode opcode = (EventOpcode)this.m_eventData[eventId].Descriptor.Opcode;
					EventActivityOptions activityOptions = this.m_eventData[eventId].ActivityOptions;
					Guid* activityID = null;
					Guid empty = Guid.Empty;
					Guid empty2 = Guid.Empty;
					if (opcode != EventOpcode.Info && relatedActivityId == null && (activityOptions & EventActivityOptions.Disable) == EventActivityOptions.None)
					{
						if (opcode == EventOpcode.Start)
						{
							this.m_activityTracker.OnStart(this.m_name, this.m_eventData[eventId].Name, this.m_eventData[eventId].Descriptor.Task, ref empty, ref empty2, this.m_eventData[eventId].ActivityOptions);
						}
						else if (opcode == EventOpcode.Stop)
						{
							this.m_activityTracker.OnStop(this.m_name, this.m_eventData[eventId].Name, this.m_eventData[eventId].Descriptor.Task, ref empty);
						}
						if (empty != Guid.Empty)
						{
							activityID = &empty;
						}
						if (empty2 != Guid.Empty)
						{
							relatedActivityId = &empty2;
						}
					}
					if (this.m_eventData[eventId].EnabledForETW)
					{
						SessionMask m = SessionMask.All;
						if ((ulong)this.m_curLiveSessions != 0UL)
						{
							m = this.GetEtwSessionMask(eventId, relatedActivityId);
						}
						if ((ulong)m != 0UL || (this.m_legacySessions != null && this.m_legacySessions.Count > 0))
						{
							if (!this.SelfDescribingEvents)
							{
								if (m.IsEqualOrSupersetOf(this.m_curLiveSessions))
								{
									if (!this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, activityID, relatedActivityId, eventDataCount, (IntPtr)((void*)data)))
									{
										this.ThrowEventSourceException(this.m_eventData[eventId].Name, null);
									}
								}
								else
								{
									long num = this.m_eventData[eventId].Descriptor.Keywords & (long)(~(long)SessionMask.All.ToEventKeywords());
									EventDescriptor eventDescriptor = new EventDescriptor(this.m_eventData[eventId].Descriptor.EventId, this.m_eventData[eventId].Descriptor.Version, this.m_eventData[eventId].Descriptor.Channel, this.m_eventData[eventId].Descriptor.Level, this.m_eventData[eventId].Descriptor.Opcode, this.m_eventData[eventId].Descriptor.Task, (long)(m.ToEventKeywords() | (ulong)num));
									if (!this.m_provider.WriteEvent(ref eventDescriptor, activityID, relatedActivityId, eventDataCount, (IntPtr)((void*)data)))
									{
										this.ThrowEventSourceException(this.m_eventData[eventId].Name, null);
									}
								}
							}
							else
							{
								TraceLoggingEventTypes traceLoggingEventTypes = this.m_eventData[eventId].TraceLoggingEventTypes;
								if (traceLoggingEventTypes == null)
								{
									traceLoggingEventTypes = new TraceLoggingEventTypes(this.m_eventData[eventId].Name, EventTags.None, this.m_eventData[eventId].Parameters);
									Interlocked.CompareExchange<TraceLoggingEventTypes>(ref this.m_eventData[eventId].TraceLoggingEventTypes, traceLoggingEventTypes, null);
								}
								long num2 = this.m_eventData[eventId].Descriptor.Keywords & (long)(~(long)SessionMask.All.ToEventKeywords());
								EventSourceOptions eventSourceOptions = new EventSourceOptions
								{
									Keywords = (EventKeywords)(m.ToEventKeywords() | (ulong)num2),
									Level = (EventLevel)this.m_eventData[eventId].Descriptor.Level,
									Opcode = (EventOpcode)this.m_eventData[eventId].Descriptor.Opcode
								};
								this.WriteMultiMerge(this.m_eventData[eventId].Name, ref eventSourceOptions, traceLoggingEventTypes, activityID, relatedActivityId, data);
							}
						}
					}
					if (this.m_Dispatchers != null && this.m_eventData[eventId].EnabledForAnyListener)
					{
						this.WriteToAllListeners(eventId, activityID, relatedActivityId, eventDataCount, data);
					}
				}
				catch (Exception ex)
				{
					if (ex is EventSourceException)
					{
						throw;
					}
					this.ThrowEventSourceException(this.m_eventData[eventId].Name, ex);
				}
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected void WriteEvent(int eventId, params object[] args)
		{
			this.WriteEventVarargs(eventId, null, args);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		protected unsafe void WriteEventWithRelatedActivityId(int eventId, Guid relatedActivityId, params object[] args)
		{
			this.WriteEventVarargs(eventId, &relatedActivityId, args);
		}

		[__DynamicallyInvokable]
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[__DynamicallyInvokable]
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.m_eventSourceEnabled)
				{
					try
					{
						this.SendManifest(this.m_rawManifest);
					}
					catch (Exception)
					{
					}
					this.m_eventSourceEnabled = false;
				}
				if (this.m_provider != null)
				{
					this.m_provider.Dispose();
					this.m_provider = null;
				}
			}
			this.m_eventSourceEnabled = false;
		}

		[__DynamicallyInvokable]
		~EventSource()
		{
			this.Dispose(false);
		}

		internal void WriteStringToListener(EventListener listener, string msg, SessionMask m)
		{
			if (this.m_eventSourceEnabled)
			{
				if (listener == null)
				{
					this.WriteEventString(EventLevel.LogAlways, (long)m.ToEventKeywords(), msg);
					return;
				}
				List<object> list = new List<object>();
				list.Add(msg);
				listener.OnEventWritten(new EventWrittenEventArgs(this)
				{
					EventId = 0,
					Payload = new ReadOnlyCollection<object>(list)
				});
			}
		}

		[SecurityCritical]
		private unsafe void WriteEventRaw(string eventName, ref EventDescriptor eventDescriptor, Guid* activityID, Guid* relatedActivityID, int dataCount, IntPtr data)
		{
			if (this.m_provider == null)
			{
				this.ThrowEventSourceException(eventName, null);
				return;
			}
			if (!this.m_provider.WriteEventRaw(ref eventDescriptor, activityID, relatedActivityID, dataCount, data))
			{
				this.ThrowEventSourceException(eventName, null);
			}
		}

		internal EventSource(Guid eventSourceGuid, string eventSourceName) : this(eventSourceGuid, eventSourceName, EventSourceSettings.EtwManifestEventFormat, null)
		{
		}

		internal EventSource(Guid eventSourceGuid, string eventSourceName, EventSourceSettings settings, string[] traits = null)
		{
			this.m_config = this.ValidateSettings(settings);
			this.Initialize(eventSourceGuid, eventSourceName, traits);
		}

		[SecuritySafeCritical]
		private unsafe void Initialize(Guid eventSourceGuid, string eventSourceName, string[] traits)
		{
			try
			{
				this.m_traits = traits;
				if (this.m_traits != null && this.m_traits.Length % 2 != 0)
				{
					throw new ArgumentException(Environment.GetResourceString("TraitEven"), "traits");
				}
				if (eventSourceGuid == Guid.Empty)
				{
					throw new ArgumentException(Environment.GetResourceString("EventSource_NeedGuid"));
				}
				if (eventSourceName == null)
				{
					throw new ArgumentException(Environment.GetResourceString("EventSource_NeedName"));
				}
				this.m_name = eventSourceName;
				this.m_guid = eventSourceGuid;
				this.m_curLiveSessions = new SessionMask(0U);
				this.m_etwSessionIdMap = new EtwSession[4];
				this.m_activityTracker = ActivityTracker.Instance;
				this.InitializeProviderMetadata();
				EventSource.OverideEventProvider overideEventProvider = new EventSource.OverideEventProvider(this);
				overideEventProvider.Register(eventSourceGuid);
				EventListener.AddEventSource(this);
				this.m_provider = overideEventProvider;
				int num = Environment.OSVersion.Version.Major * 10 + Environment.OSVersion.Version.Minor;
				if (this.Name != "System.Diagnostics.Eventing.FrameworkEventSource" || num >= 62)
				{
					try
					{
						fixed (IntPtr* ptr = this.providerMetadata)
						{
							int num2 = this.m_provider.SetInformation(UnsafeNativeMethods.ManifestEtw.EVENT_INFO_CLASS.SetTraits, (void*)ptr, this.providerMetadata.Length);
						}
					}
					finally
					{
						IntPtr* ptr = null;
					}
				}
				this.m_completelyInited = true;
			}
			catch (Exception ex)
			{
				if (this.m_constructionException == null)
				{
					this.m_constructionException = ex;
				}
				this.ReportOutOfBandMessage("ERROR: Exception during construction of EventSource " + this.Name + ": " + ex.Message, true);
			}
			object eventListenersLock = EventListener.EventListenersLock;
			lock (eventListenersLock)
			{
				for (EventCommandEventArgs eventCommandEventArgs = this.m_deferredCommands; eventCommandEventArgs != null; eventCommandEventArgs = eventCommandEventArgs.nextCommand)
				{
					this.DoCommand(eventCommandEventArgs);
				}
			}
		}

		private static string GetName(Type eventSourceType, EventManifestOptions flags)
		{
			if (eventSourceType == null)
			{
				throw new ArgumentNullException("eventSourceType");
			}
			EventSourceAttribute eventSourceAttribute = (EventSourceAttribute)EventSource.GetCustomAttributeHelper(eventSourceType, typeof(EventSourceAttribute), flags);
			if (eventSourceAttribute != null && eventSourceAttribute.Name != null)
			{
				return eventSourceAttribute.Name;
			}
			return eventSourceType.Name;
		}

		private static Guid GenerateGuidFromName(string name)
		{
			byte[] bytes = Encoding.BigEndianUnicode.GetBytes(name);
			EventSource.Sha1ForNonSecretPurposes sha1ForNonSecretPurposes = default(EventSource.Sha1ForNonSecretPurposes);
			sha1ForNonSecretPurposes.Start();
			sha1ForNonSecretPurposes.Append(EventSource.namespaceBytes);
			sha1ForNonSecretPurposes.Append(bytes);
			Array.Resize<byte>(ref bytes, 16);
			sha1ForNonSecretPurposes.Finish(bytes);
			bytes[7] = ((bytes[7] & 15) | 80);
			return new Guid(bytes);
		}

		[SecurityCritical]
		private unsafe object DecodeObject(int eventId, int parameterId, ref EventSource.EventData* data)
		{
			IntPtr dataPointer = data.DataPointer;
			data += (IntPtr)sizeof(EventSource.EventData);
			Type type = this.m_eventData[eventId].Parameters[parameterId].ParameterType;
			while (!(type == typeof(IntPtr)))
			{
				if (type == typeof(int))
				{
					return *(int*)((void*)dataPointer);
				}
				if (type == typeof(uint))
				{
					return *(uint*)((void*)dataPointer);
				}
				if (type == typeof(long))
				{
					return *(long*)((void*)dataPointer);
				}
				if (type == typeof(ulong))
				{
					return (ulong)(*(long*)((void*)dataPointer));
				}
				if (type == typeof(byte))
				{
					return *(byte*)((void*)dataPointer);
				}
				if (type == typeof(sbyte))
				{
					return *(sbyte*)((void*)dataPointer);
				}
				if (type == typeof(short))
				{
					return *(short*)((void*)dataPointer);
				}
				if (type == typeof(ushort))
				{
					return *(ushort*)((void*)dataPointer);
				}
				if (type == typeof(float))
				{
					return *(float*)((void*)dataPointer);
				}
				if (type == typeof(double))
				{
					return *(double*)((void*)dataPointer);
				}
				if (type == typeof(decimal))
				{
					return *(decimal*)((void*)dataPointer);
				}
				if (type == typeof(bool))
				{
					if (*(int*)((void*)dataPointer) == 1)
					{
						return true;
					}
					return false;
				}
				else
				{
					if (type == typeof(Guid))
					{
						return *(Guid*)((void*)dataPointer);
					}
					if (type == typeof(char))
					{
						return (char)(*(ushort*)((void*)dataPointer));
					}
					if (type == typeof(DateTime))
					{
						long fileTime = *(long*)((void*)dataPointer);
						return DateTime.FromFileTimeUtc(fileTime);
					}
					if (type == typeof(byte[]))
					{
						int num = *(int*)((void*)dataPointer);
						byte[] array = new byte[num];
						dataPointer = data.DataPointer;
						data += (IntPtr)sizeof(EventSource.EventData);
						for (int i = 0; i < num; i++)
						{
							array[i] = ((byte*)((void*)dataPointer))[i];
						}
						return array;
					}
					if (type == typeof(byte*))
					{
						return null;
					}
					if (!type.IsEnum())
					{
						return Marshal.PtrToStringUni(dataPointer);
					}
					type = Enum.GetUnderlyingType(type);
				}
			}
			return *(IntPtr*)((void*)dataPointer);
		}

		private EventDispatcher GetDispatcher(EventListener listener)
		{
			EventDispatcher eventDispatcher;
			for (eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
			{
				if (eventDispatcher.m_Listener == listener)
				{
					return eventDispatcher;
				}
			}
			return eventDispatcher;
		}

		[SecurityCritical]
		private unsafe void WriteEventVarargs(int eventId, Guid* childActivityID, object[] args)
		{
			if (this.m_eventSourceEnabled)
			{
				try
				{
					if (childActivityID != null)
					{
						this.ValidateEventOpcodeForTransfer(ref this.m_eventData[eventId], this.m_eventData[eventId].Name);
						if (!this.m_eventData[eventId].HasRelatedActivityID)
						{
							throw new ArgumentException(Environment.GetResourceString("EventSource_NoRelatedActivityId"));
						}
					}
					this.LogEventArgsMismatches(this.m_eventData[eventId].Parameters, args);
					Guid* activityID = null;
					Guid empty = Guid.Empty;
					Guid empty2 = Guid.Empty;
					EventOpcode opcode = (EventOpcode)this.m_eventData[eventId].Descriptor.Opcode;
					EventActivityOptions activityOptions = this.m_eventData[eventId].ActivityOptions;
					if (childActivityID == null && (activityOptions & EventActivityOptions.Disable) == EventActivityOptions.None)
					{
						if (opcode == EventOpcode.Start)
						{
							this.m_activityTracker.OnStart(this.m_name, this.m_eventData[eventId].Name, this.m_eventData[eventId].Descriptor.Task, ref empty, ref empty2, this.m_eventData[eventId].ActivityOptions);
						}
						else if (opcode == EventOpcode.Stop)
						{
							this.m_activityTracker.OnStop(this.m_name, this.m_eventData[eventId].Name, this.m_eventData[eventId].Descriptor.Task, ref empty);
						}
						if (empty != Guid.Empty)
						{
							activityID = &empty;
						}
						if (empty2 != Guid.Empty)
						{
							childActivityID = &empty2;
						}
					}
					if (this.m_eventData[eventId].EnabledForETW)
					{
						SessionMask m = SessionMask.All;
						if ((ulong)this.m_curLiveSessions != 0UL)
						{
							m = this.GetEtwSessionMask(eventId, childActivityID);
						}
						if ((ulong)m != 0UL || (this.m_legacySessions != null && this.m_legacySessions.Count > 0))
						{
							if (!this.SelfDescribingEvents)
							{
								if (m.IsEqualOrSupersetOf(this.m_curLiveSessions))
								{
									if (!this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, activityID, childActivityID, args))
									{
										this.ThrowEventSourceException(this.m_eventData[eventId].Name, null);
									}
								}
								else
								{
									long num = this.m_eventData[eventId].Descriptor.Keywords & (long)(~(long)SessionMask.All.ToEventKeywords());
									EventDescriptor eventDescriptor = new EventDescriptor(this.m_eventData[eventId].Descriptor.EventId, this.m_eventData[eventId].Descriptor.Version, this.m_eventData[eventId].Descriptor.Channel, this.m_eventData[eventId].Descriptor.Level, this.m_eventData[eventId].Descriptor.Opcode, this.m_eventData[eventId].Descriptor.Task, (long)(m.ToEventKeywords() | (ulong)num));
									if (!this.m_provider.WriteEvent(ref eventDescriptor, activityID, childActivityID, args))
									{
										this.ThrowEventSourceException(this.m_eventData[eventId].Name, null);
									}
								}
							}
							else
							{
								TraceLoggingEventTypes traceLoggingEventTypes = this.m_eventData[eventId].TraceLoggingEventTypes;
								if (traceLoggingEventTypes == null)
								{
									traceLoggingEventTypes = new TraceLoggingEventTypes(this.m_eventData[eventId].Name, EventTags.None, this.m_eventData[eventId].Parameters);
									Interlocked.CompareExchange<TraceLoggingEventTypes>(ref this.m_eventData[eventId].TraceLoggingEventTypes, traceLoggingEventTypes, null);
								}
								long num2 = this.m_eventData[eventId].Descriptor.Keywords & (long)(~(long)SessionMask.All.ToEventKeywords());
								EventSourceOptions eventSourceOptions = new EventSourceOptions
								{
									Keywords = (EventKeywords)(m.ToEventKeywords() | (ulong)num2),
									Level = (EventLevel)this.m_eventData[eventId].Descriptor.Level,
									Opcode = (EventOpcode)this.m_eventData[eventId].Descriptor.Opcode
								};
								this.WriteMultiMerge(this.m_eventData[eventId].Name, ref eventSourceOptions, traceLoggingEventTypes, activityID, childActivityID, args);
							}
						}
					}
					if (this.m_Dispatchers != null && this.m_eventData[eventId].EnabledForAnyListener)
					{
						if (AppContextSwitches.PreserveEventListnerObjectIdentity)
						{
							this.WriteToAllListeners(eventId, activityID, childActivityID, args);
						}
						else
						{
							object[] args2 = this.SerializeEventArgs(eventId, args);
							this.WriteToAllListeners(eventId, activityID, childActivityID, args2);
						}
					}
				}
				catch (Exception ex)
				{
					if (ex is EventSourceException)
					{
						throw;
					}
					this.ThrowEventSourceException(this.m_eventData[eventId].Name, ex);
				}
			}
		}

		[SecurityCritical]
		private object[] SerializeEventArgs(int eventId, object[] args)
		{
			TraceLoggingEventTypes traceLoggingEventTypes = this.m_eventData[eventId].TraceLoggingEventTypes;
			if (traceLoggingEventTypes == null)
			{
				traceLoggingEventTypes = new TraceLoggingEventTypes(this.m_eventData[eventId].Name, EventTags.None, this.m_eventData[eventId].Parameters);
				Interlocked.CompareExchange<TraceLoggingEventTypes>(ref this.m_eventData[eventId].TraceLoggingEventTypes, traceLoggingEventTypes, null);
			}
			object[] array = new object[traceLoggingEventTypes.typeInfos.Length];
			for (int i = 0; i < traceLoggingEventTypes.typeInfos.Length; i++)
			{
				array[i] = traceLoggingEventTypes.typeInfos[i].GetData(args[i]);
			}
			return array;
		}

		private void LogEventArgsMismatches(ParameterInfo[] infos, object[] args)
		{
			bool flag = args.Length == infos.Length;
			int num = 0;
			while (flag && num < args.Length)
			{
				Type parameterType = infos[num].ParameterType;
				if ((args[num] != null && args[num].GetType() != parameterType) || (args[num] == null && (!parameterType.IsGenericType || !(parameterType.GetGenericTypeDefinition() == typeof(Nullable<>)))))
				{
					flag = false;
					break;
				}
				num++;
			}
			if (!flag)
			{
				Debugger.Log(0, null, Environment.GetResourceString("EventSource_VarArgsParameterMismatch") + "\r\n");
			}
		}

		private int GetParamLengthIncludingByteArray(ParameterInfo[] parameters)
		{
			int num = 0;
			foreach (ParameterInfo parameterInfo in parameters)
			{
				if (parameterInfo.ParameterType == typeof(byte[]))
				{
					num += 2;
				}
				else
				{
					num++;
				}
			}
			return num;
		}

		[SecurityCritical]
		private unsafe void WriteToAllListeners(int eventId, Guid* activityID, Guid* childActivityID, int eventDataCount, EventSource.EventData* data)
		{
			int num = this.m_eventData[eventId].Parameters.Length;
			int paramLengthIncludingByteArray = this.GetParamLengthIncludingByteArray(this.m_eventData[eventId].Parameters);
			if (eventDataCount != paramLengthIncludingByteArray)
			{
				this.ReportOutOfBandMessage(Environment.GetResourceString("EventSource_EventParametersMismatch", new object[]
				{
					eventId,
					eventDataCount,
					num
				}), true);
				num = Math.Min(num, eventDataCount);
			}
			object[] array = new object[num];
			EventSource.EventData* ptr = data;
			for (int i = 0; i < num; i++)
			{
				array[i] = this.DecodeObject(eventId, i, ref ptr);
			}
			this.WriteToAllListeners(eventId, activityID, childActivityID, array);
		}

		[SecurityCritical]
		private unsafe void WriteToAllListeners(int eventId, Guid* activityID, Guid* childActivityID, params object[] args)
		{
			EventWrittenEventArgs eventWrittenEventArgs = new EventWrittenEventArgs(this);
			eventWrittenEventArgs.EventId = eventId;
			if (activityID != null)
			{
				eventWrittenEventArgs.ActivityId = *activityID;
			}
			if (childActivityID != null)
			{
				eventWrittenEventArgs.RelatedActivityId = *childActivityID;
			}
			eventWrittenEventArgs.EventName = this.m_eventData[eventId].Name;
			eventWrittenEventArgs.Message = this.m_eventData[eventId].Message;
			eventWrittenEventArgs.Payload = new ReadOnlyCollection<object>(args);
			this.DispatchToAllListeners(eventId, childActivityID, eventWrittenEventArgs);
		}

		[SecurityCritical]
		private unsafe void DispatchToAllListeners(int eventId, Guid* childActivityID, EventWrittenEventArgs eventCallbackArgs)
		{
			Exception ex = null;
			for (EventDispatcher eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
			{
				if (eventId == -1 || eventDispatcher.m_EventEnabled[eventId])
				{
					ActivityFilter activityFilter = eventDispatcher.m_Listener.m_activityFilter;
					if (activityFilter == null || ActivityFilter.PassesActivityFilter(activityFilter, childActivityID, this.m_eventData[eventId].TriggersActivityTracking > 0, this, eventId) || !eventDispatcher.m_activityFilteringEnabled)
					{
						try
						{
							eventDispatcher.m_Listener.OnEventWritten(eventCallbackArgs);
						}
						catch (Exception ex2)
						{
							this.ReportOutOfBandMessage("ERROR: Exception during EventSource.OnEventWritten: " + ex2.Message, false);
							ex = ex2;
						}
					}
				}
			}
			if (ex != null)
			{
				throw new EventSourceException(ex);
			}
		}

		[SecuritySafeCritical]
		private unsafe void WriteEventString(EventLevel level, long keywords, string msgString)
		{
			if (this.m_provider != null)
			{
				string text = "EventSourceMessage";
				if (this.SelfDescribingEvents)
				{
					EventSourceOptions eventSourceOptions = new EventSourceOptions
					{
						Keywords = (EventKeywords)keywords,
						Level = level
					};
					var <>f__AnonymousType = new
					{
						message = msgString
					};
					TraceLoggingEventTypes eventTypes = new TraceLoggingEventTypes(text, EventTags.None, new Type[]
					{
						<>f__AnonymousType.GetType()
					});
					this.WriteMultiMergeInner(text, ref eventSourceOptions, eventTypes, null, null, new object[]
					{
						<>f__AnonymousType
					});
					return;
				}
				if (this.m_rawManifest == null && this.m_outOfBandMessageCount == 1)
				{
					ManifestBuilder manifestBuilder = new ManifestBuilder(this.Name, this.Guid, this.Name, null, EventManifestOptions.None);
					manifestBuilder.StartEvent(text, new EventAttribute(0)
					{
						Level = EventLevel.LogAlways,
						Task = (EventTask)65534
					});
					manifestBuilder.AddEventParameter(typeof(string), "message");
					manifestBuilder.EndEvent();
					this.SendManifest(manifestBuilder.CreateManifest());
				}
				fixed (string text2 = msgString)
				{
					char* ptr = text2;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					EventDescriptor eventDescriptor = new EventDescriptor(0, 0, 0, (byte)level, 0, 0, keywords);
					EventProvider.EventData eventData = default(EventProvider.EventData);
					eventData.Ptr = ptr;
					eventData.Size = (uint)(2 * (msgString.Length + 1));
					eventData.Reserved = 0U;
					this.m_provider.WriteEvent(ref eventDescriptor, null, null, 1, (IntPtr)((void*)(&eventData)));
				}
			}
		}

		private void WriteStringToAllListeners(string eventName, string msg)
		{
			EventWrittenEventArgs eventWrittenEventArgs = new EventWrittenEventArgs(this);
			eventWrittenEventArgs.EventId = 0;
			eventWrittenEventArgs.Message = msg;
			eventWrittenEventArgs.Payload = new ReadOnlyCollection<object>(new List<object>
			{
				msg
			});
			eventWrittenEventArgs.PayloadNames = new ReadOnlyCollection<string>(new List<string>
			{
				"message"
			});
			eventWrittenEventArgs.EventName = eventName;
			for (EventDispatcher eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
			{
				bool flag = false;
				if (eventDispatcher.m_EventEnabled == null)
				{
					flag = true;
				}
				else
				{
					for (int i = 0; i < eventDispatcher.m_EventEnabled.Length; i++)
					{
						if (eventDispatcher.m_EventEnabled[i])
						{
							flag = true;
							break;
						}
					}
				}
				try
				{
					if (flag)
					{
						eventDispatcher.m_Listener.OnEventWritten(eventWrittenEventArgs);
					}
				}
				catch
				{
				}
			}
		}

		[SecurityCritical]
		private unsafe SessionMask GetEtwSessionMask(int eventId, Guid* childActivityID)
		{
			SessionMask result = default(SessionMask);
			int num = 0;
			while ((long)num < 4L)
			{
				EtwSession etwSession = this.m_etwSessionIdMap[num];
				if (etwSession != null)
				{
					ActivityFilter activityFilter = etwSession.m_activityFilter;
					if ((activityFilter == null && !this.m_activityFilteringForETWEnabled[num]) || (activityFilter != null && ActivityFilter.PassesActivityFilter(activityFilter, childActivityID, this.m_eventData[eventId].TriggersActivityTracking > 0, this, eventId)) || !this.m_activityFilteringForETWEnabled[num])
					{
						result[num] = true;
					}
				}
				num++;
			}
			if (this.m_legacySessions != null && this.m_legacySessions.Count > 0 && this.m_eventData[eventId].Descriptor.Opcode == 9)
			{
				Guid* ptr = null;
				foreach (EtwSession etwSession2 in this.m_legacySessions)
				{
					if (etwSession2 != null)
					{
						ActivityFilter activityFilter2 = etwSession2.m_activityFilter;
						if (activityFilter2 != null)
						{
							if (ptr == null)
							{
								Guid internalCurrentThreadActivityId = EventSource.InternalCurrentThreadActivityId;
								ptr = &internalCurrentThreadActivityId;
							}
							ActivityFilter.FlowActivityIfNeeded(activityFilter2, ptr, childActivityID);
						}
					}
				}
			}
			return result;
		}

		private bool IsEnabledByDefault(int eventNum, bool enable, EventLevel currentLevel, EventKeywords currentMatchAnyKeyword)
		{
			if (!enable)
			{
				return false;
			}
			EventLevel level = (EventLevel)this.m_eventData[eventNum].Descriptor.Level;
			EventKeywords eventKeywords = (EventKeywords)(this.m_eventData[eventNum].Descriptor.Keywords & (long)(~(long)SessionMask.All.ToEventKeywords()));
			EventChannel channel = (EventChannel)this.m_eventData[eventNum].Descriptor.Channel;
			return this.IsEnabledCommon(enable, currentLevel, currentMatchAnyKeyword, level, eventKeywords, channel);
		}

		private bool IsEnabledCommon(bool enabled, EventLevel currentLevel, EventKeywords currentMatchAnyKeyword, EventLevel eventLevel, EventKeywords eventKeywords, EventChannel eventChannel)
		{
			if (!enabled)
			{
				return false;
			}
			if (currentLevel != EventLevel.LogAlways && currentLevel < eventLevel)
			{
				return false;
			}
			if (currentMatchAnyKeyword != EventKeywords.None && eventKeywords != EventKeywords.None)
			{
				if (eventChannel != EventChannel.None && this.m_channelData != null && this.m_channelData.Length > (int)eventChannel)
				{
					EventKeywords eventKeywords2 = (EventKeywords)(this.m_channelData[(int)eventChannel] | (ulong)eventKeywords);
					if (eventKeywords2 != EventKeywords.None && (eventKeywords2 & currentMatchAnyKeyword) == EventKeywords.None)
					{
						return false;
					}
				}
				else if ((eventKeywords & currentMatchAnyKeyword) == EventKeywords.None)
				{
					return false;
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ThrowEventSourceException(string eventName, Exception innerEx = null)
		{
			if (EventSource.m_EventSourceExceptionRecurenceCount > 0)
			{
				return;
			}
			try
			{
				EventSource.m_EventSourceExceptionRecurenceCount += 1;
				string text = "EventSourceException";
				if (eventName != null)
				{
					text = text + " while processing event \"" + eventName + "\"";
				}
				switch (EventProvider.GetLastWriteEventError())
				{
				case EventProvider.WriteEventErrorCode.NoFreeBuffers:
					this.ReportOutOfBandMessage(text + ": " + Environment.GetResourceString("EventSource_NoFreeBuffers"), true);
					if (this.ThrowOnEventWriteErrors)
					{
						throw new EventSourceException(Environment.GetResourceString("EventSource_NoFreeBuffers"), innerEx);
					}
					break;
				case EventProvider.WriteEventErrorCode.EventTooBig:
					this.ReportOutOfBandMessage(text + ": " + Environment.GetResourceString("EventSource_EventTooBig"), true);
					if (this.ThrowOnEventWriteErrors)
					{
						throw new EventSourceException(Environment.GetResourceString("EventSource_EventTooBig"), innerEx);
					}
					break;
				case EventProvider.WriteEventErrorCode.NullInput:
					this.ReportOutOfBandMessage(text + ": " + Environment.GetResourceString("EventSource_NullInput"), true);
					if (this.ThrowOnEventWriteErrors)
					{
						throw new EventSourceException(Environment.GetResourceString("EventSource_NullInput"), innerEx);
					}
					break;
				case EventProvider.WriteEventErrorCode.TooManyArgs:
					this.ReportOutOfBandMessage(text + ": " + Environment.GetResourceString("EventSource_TooManyArgs"), true);
					if (this.ThrowOnEventWriteErrors)
					{
						throw new EventSourceException(Environment.GetResourceString("EventSource_TooManyArgs"), innerEx);
					}
					break;
				default:
					if (innerEx != null)
					{
						this.ReportOutOfBandMessage(string.Concat(new object[]
						{
							text,
							": ",
							innerEx.GetType(),
							":",
							innerEx.Message
						}), true);
					}
					else
					{
						this.ReportOutOfBandMessage(text, true);
					}
					if (this.ThrowOnEventWriteErrors)
					{
						throw new EventSourceException(innerEx);
					}
					break;
				}
			}
			finally
			{
				EventSource.m_EventSourceExceptionRecurenceCount -= 1;
			}
		}

		private void ValidateEventOpcodeForTransfer(ref EventSource.EventMetadata eventData, string eventName)
		{
			if (eventData.Descriptor.Opcode != 9 && eventData.Descriptor.Opcode != 240 && eventData.Descriptor.Opcode != 1)
			{
				this.ThrowEventSourceException(eventName, null);
			}
		}

		internal static EventOpcode GetOpcodeWithDefault(EventOpcode opcode, string eventName)
		{
			if (opcode == EventOpcode.Info && eventName != null)
			{
				if (eventName.EndsWith("Start"))
				{
					return EventOpcode.Start;
				}
				if (eventName.EndsWith("Stop"))
				{
					return EventOpcode.Stop;
				}
			}
			return opcode;
		}

		internal void SendCommand(EventListener listener, int perEventSourceSessionId, int etwSessionId, EventCommand command, bool enable, EventLevel level, EventKeywords matchAnyKeyword, IDictionary<string, string> commandArguments)
		{
			EventCommandEventArgs eventCommandEventArgs = new EventCommandEventArgs(command, commandArguments, this, listener, perEventSourceSessionId, etwSessionId, enable, level, matchAnyKeyword);
			object eventListenersLock = EventListener.EventListenersLock;
			lock (eventListenersLock)
			{
				if (this.m_completelyInited)
				{
					this.m_deferredCommands = null;
					this.DoCommand(eventCommandEventArgs);
				}
				else
				{
					eventCommandEventArgs.nextCommand = this.m_deferredCommands;
					this.m_deferredCommands = eventCommandEventArgs;
				}
			}
		}

		internal void DoCommand(EventCommandEventArgs commandArgs)
		{
			if (this.m_provider == null)
			{
				return;
			}
			this.m_outOfBandMessageCount = 0;
			bool flag = commandArgs.perEventSourceSessionId > 0 && (long)commandArgs.perEventSourceSessionId <= 4L;
			try
			{
				this.EnsureDescriptorsInitialized();
				commandArgs.dispatcher = this.GetDispatcher(commandArgs.listener);
				if (commandArgs.dispatcher == null && commandArgs.listener != null)
				{
					throw new ArgumentException(Environment.GetResourceString("EventSource_ListenerNotFound"));
				}
				if (commandArgs.Arguments == null)
				{
					commandArgs.Arguments = new Dictionary<string, string>();
				}
				if (commandArgs.Command == EventCommand.Update)
				{
					for (int i = 0; i < this.m_eventData.Length; i++)
					{
						this.EnableEventForDispatcher(commandArgs.dispatcher, i, this.IsEnabledByDefault(i, commandArgs.enable, commandArgs.level, commandArgs.matchAnyKeyword));
					}
					if (commandArgs.enable)
					{
						if (!this.m_eventSourceEnabled)
						{
							this.m_level = commandArgs.level;
							this.m_matchAnyKeyword = commandArgs.matchAnyKeyword;
						}
						else
						{
							if (commandArgs.level > this.m_level)
							{
								this.m_level = commandArgs.level;
							}
							if (commandArgs.matchAnyKeyword == EventKeywords.None)
							{
								this.m_matchAnyKeyword = EventKeywords.None;
							}
							else if (this.m_matchAnyKeyword != EventKeywords.None)
							{
								this.m_matchAnyKeyword |= commandArgs.matchAnyKeyword;
							}
						}
					}
					bool flag2 = commandArgs.perEventSourceSessionId >= 0;
					if (commandArgs.perEventSourceSessionId == 0 && !commandArgs.enable)
					{
						flag2 = false;
					}
					if (commandArgs.listener == null)
					{
						if (!flag2)
						{
							commandArgs.perEventSourceSessionId = -commandArgs.perEventSourceSessionId;
						}
						commandArgs.perEventSourceSessionId--;
					}
					commandArgs.Command = (flag2 ? EventCommand.Enable : EventCommand.Disable);
					if (flag2 && commandArgs.dispatcher == null && !this.SelfDescribingEvents)
					{
						this.SendManifest(this.m_rawManifest);
					}
					if (flag2 && commandArgs.perEventSourceSessionId != -1)
					{
						bool flag3 = false;
						string text;
						int num;
						EventSource.ParseCommandArgs(commandArgs.Arguments, out flag3, out text, out num);
						if (commandArgs.listener == null && commandArgs.Arguments.Count > 0 && commandArgs.perEventSourceSessionId != num)
						{
							throw new ArgumentException(Environment.GetResourceString("EventSource_SessionIdError", new object[]
							{
								commandArgs.perEventSourceSessionId + 44,
								num + 44
							}));
						}
						if (commandArgs.listener == null)
						{
							this.UpdateEtwSession(commandArgs.perEventSourceSessionId, commandArgs.etwSessionId, true, text, flag3);
						}
						else
						{
							ActivityFilter.UpdateFilter(ref commandArgs.listener.m_activityFilter, this, 0, text);
							commandArgs.dispatcher.m_activityFilteringEnabled = flag3;
						}
					}
					else if (!flag2 && commandArgs.listener == null && commandArgs.perEventSourceSessionId >= 0 && (long)commandArgs.perEventSourceSessionId < 4L)
					{
						commandArgs.Arguments["EtwSessionKeyword"] = (commandArgs.perEventSourceSessionId + 44).ToString(CultureInfo.InvariantCulture);
					}
					if (commandArgs.enable)
					{
						this.m_eventSourceEnabled = true;
					}
					this.OnEventCommand(commandArgs);
					EventHandler<EventCommandEventArgs> eventCommandExecuted = this.m_eventCommandExecuted;
					if (eventCommandExecuted != null)
					{
						eventCommandExecuted(this, commandArgs);
					}
					if (commandArgs.listener == null && !flag2 && commandArgs.perEventSourceSessionId != -1)
					{
						this.UpdateEtwSession(commandArgs.perEventSourceSessionId, commandArgs.etwSessionId, false, null, false);
					}
					if (!commandArgs.enable)
					{
						if (commandArgs.listener == null)
						{
							int num2 = 0;
							while ((long)num2 < 4L)
							{
								EtwSession etwSession = this.m_etwSessionIdMap[num2];
								if (etwSession != null)
								{
									ActivityFilter.DisableFilter(ref etwSession.m_activityFilter, this);
								}
								num2++;
							}
							this.m_activityFilteringForETWEnabled = new SessionMask(0U);
							this.m_curLiveSessions = new SessionMask(0U);
							if (this.m_etwSessionIdMap != null)
							{
								int num3 = 0;
								while ((long)num3 < 4L)
								{
									this.m_etwSessionIdMap[num3] = null;
									num3++;
								}
							}
							if (this.m_legacySessions != null)
							{
								this.m_legacySessions.Clear();
							}
						}
						else
						{
							ActivityFilter.DisableFilter(ref commandArgs.listener.m_activityFilter, this);
							commandArgs.dispatcher.m_activityFilteringEnabled = false;
						}
						for (int j = 0; j < this.m_eventData.Length; j++)
						{
							bool enabledForAnyListener = false;
							for (EventDispatcher eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
							{
								if (eventDispatcher.m_EventEnabled[j])
								{
									enabledForAnyListener = true;
									break;
								}
							}
							this.m_eventData[j].EnabledForAnyListener = enabledForAnyListener;
						}
						if (!this.AnyEventEnabled())
						{
							this.m_level = EventLevel.LogAlways;
							this.m_matchAnyKeyword = EventKeywords.None;
							this.m_eventSourceEnabled = false;
						}
					}
					this.UpdateKwdTriggers(commandArgs.enable);
				}
				else
				{
					if (commandArgs.Command == EventCommand.SendManifest && this.m_rawManifest != null)
					{
						this.SendManifest(this.m_rawManifest);
					}
					this.OnEventCommand(commandArgs);
					EventHandler<EventCommandEventArgs> eventCommandExecuted2 = this.m_eventCommandExecuted;
					if (eventCommandExecuted2 != null)
					{
						eventCommandExecuted2(this, commandArgs);
					}
				}
				if (this.m_completelyInited && (commandArgs.listener != null || flag))
				{
					SessionMask sessions = SessionMask.FromId(commandArgs.perEventSourceSessionId);
					this.ReportActivitySamplingInfo(commandArgs.listener, sessions);
				}
			}
			catch (Exception ex)
			{
				this.ReportOutOfBandMessage("ERROR: Exception in Command Processing for EventSource " + this.Name + ": " + ex.Message, true);
			}
		}

		internal void UpdateEtwSession(int sessionIdBit, int etwSessionId, bool bEnable, string activityFilters, bool participateInSampling)
		{
			if ((long)sessionIdBit < 4L)
			{
				if (bEnable)
				{
					EtwSession etwSession = EtwSession.GetEtwSession(etwSessionId, true);
					ActivityFilter.UpdateFilter(ref etwSession.m_activityFilter, this, sessionIdBit, activityFilters);
					this.m_etwSessionIdMap[sessionIdBit] = etwSession;
					this.m_activityFilteringForETWEnabled[sessionIdBit] = participateInSampling;
				}
				else
				{
					EtwSession etwSession2 = EtwSession.GetEtwSession(etwSessionId, false);
					this.m_etwSessionIdMap[sessionIdBit] = null;
					this.m_activityFilteringForETWEnabled[sessionIdBit] = false;
					if (etwSession2 != null)
					{
						ActivityFilter.DisableFilter(ref etwSession2.m_activityFilter, this);
						EtwSession.RemoveEtwSession(etwSession2);
					}
				}
				this.m_curLiveSessions[sessionIdBit] = bEnable;
				return;
			}
			if (bEnable)
			{
				if (this.m_legacySessions == null)
				{
					this.m_legacySessions = new List<EtwSession>(8);
				}
				EtwSession etwSession3 = EtwSession.GetEtwSession(etwSessionId, true);
				if (!this.m_legacySessions.Contains(etwSession3))
				{
					this.m_legacySessions.Add(etwSession3);
					return;
				}
			}
			else
			{
				EtwSession etwSession4 = EtwSession.GetEtwSession(etwSessionId, false);
				if (etwSession4 != null)
				{
					if (this.m_legacySessions != null)
					{
						this.m_legacySessions.Remove(etwSession4);
					}
					EtwSession.RemoveEtwSession(etwSession4);
				}
			}
		}

		internal static bool ParseCommandArgs(IDictionary<string, string> commandArguments, out bool participateInSampling, out string activityFilters, out int sessionIdBit)
		{
			bool result = true;
			participateInSampling = false;
			if (commandArguments.TryGetValue("ActivitySamplingStartEvent", out activityFilters))
			{
				participateInSampling = true;
			}
			string text;
			if (commandArguments.TryGetValue("ActivitySampling", out text))
			{
				if (string.Compare(text, "false", StringComparison.OrdinalIgnoreCase) == 0 || text == "0")
				{
					participateInSampling = false;
				}
				else
				{
					participateInSampling = true;
				}
			}
			int num = -1;
			string s;
			if (!commandArguments.TryGetValue("EtwSessionKeyword", out s) || !int.TryParse(s, out num) || num < 44 || (long)num >= 48L)
			{
				sessionIdBit = -1;
				result = false;
			}
			else
			{
				sessionIdBit = num - 44;
			}
			return result;
		}

		internal void UpdateKwdTriggers(bool enable)
		{
			if (enable)
			{
				ulong num = (ulong)this.m_matchAnyKeyword;
				if (num == 0UL)
				{
					num = ulong.MaxValue;
				}
				this.m_keywordTriggers = 0L;
				int num2 = 0;
				while ((long)num2 < 4L)
				{
					EtwSession etwSession = this.m_etwSessionIdMap[num2];
					if (etwSession != null)
					{
						ActivityFilter activityFilter = etwSession.m_activityFilter;
						ActivityFilter.UpdateKwdTriggers(activityFilter, this.m_guid, this, (EventKeywords)num);
					}
					num2++;
				}
				return;
			}
			this.m_keywordTriggers = 0L;
		}

		internal bool EnableEventForDispatcher(EventDispatcher dispatcher, int eventId, bool value)
		{
			if (dispatcher == null)
			{
				if (eventId >= this.m_eventData.Length)
				{
					return false;
				}
				if (this.m_provider != null)
				{
					this.m_eventData[eventId].EnabledForETW = value;
				}
			}
			else
			{
				if (eventId >= dispatcher.m_EventEnabled.Length)
				{
					return false;
				}
				dispatcher.m_EventEnabled[eventId] = value;
				if (value)
				{
					this.m_eventData[eventId].EnabledForAnyListener = true;
				}
			}
			return true;
		}

		private bool AnyEventEnabled()
		{
			for (int i = 0; i < this.m_eventData.Length; i++)
			{
				if (this.m_eventData[i].EnabledForETW || this.m_eventData[i].EnabledForAnyListener)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsDisposed
		{
			get
			{
				return this.m_provider == null || this.m_provider.m_disposed;
			}
		}

		[SecuritySafeCritical]
		private void EnsureDescriptorsInitialized()
		{
			if (this.m_eventData == null)
			{
				this.m_rawManifest = EventSource.CreateManifestAndDescriptors(base.GetType(), this.Name, this, EventManifestOptions.None);
				foreach (WeakReference weakReference in EventListener.s_EventSources)
				{
					EventSource eventSource = weakReference.Target as EventSource;
					if (eventSource != null && eventSource.Guid == this.m_guid && !eventSource.IsDisposed && eventSource != this)
					{
						throw new ArgumentException(Environment.GetResourceString("EventSource_EventSourceGuidInUse", new object[]
						{
							this.m_guid
						}));
					}
				}
				for (EventDispatcher eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
				{
					if (eventDispatcher.m_EventEnabled == null)
					{
						eventDispatcher.m_EventEnabled = new bool[this.m_eventData.Length];
					}
				}
			}
			if (EventSource.s_currentPid == 0U)
			{
				EventSource.s_currentPid = Win32Native.GetCurrentProcessId();
			}
		}

		[SecuritySafeCritical]
		private unsafe bool SendManifest(byte[] rawManifest)
		{
			bool result = true;
			if (rawManifest == null)
			{
				return false;
			}
			fixed (byte* ptr = rawManifest)
			{
				EventDescriptor eventDescriptor = new EventDescriptor(65534, 1, 0, 0, 254, 65534, 72057594037927935L);
				ManifestEnvelope manifestEnvelope = default(ManifestEnvelope);
				manifestEnvelope.Format = ManifestEnvelope.ManifestFormats.SimpleXmlFormat;
				manifestEnvelope.MajorVersion = 1;
				manifestEnvelope.MinorVersion = 0;
				manifestEnvelope.Magic = 91;
				int i = rawManifest.Length;
				manifestEnvelope.ChunkNumber = 0;
				EventProvider.EventData* ptr2 = stackalloc EventProvider.EventData[checked(unchecked((UIntPtr)2) * (UIntPtr)sizeof(EventProvider.EventData))];
				ptr2->Ptr = &manifestEnvelope;
				ptr2->Size = (uint)sizeof(ManifestEnvelope);
				ptr2->Reserved = 0U;
				ptr2[1].Ptr = ptr;
				ptr2[1].Reserved = 0U;
				int num = 65280;
				for (;;)
				{
					IL_CA:
					manifestEnvelope.TotalChunks = (ushort)((i + (num - 1)) / num);
					while (i > 0)
					{
						ptr2[1].Size = (uint)Math.Min(i, num);
						if (this.m_provider != null && !this.m_provider.WriteEvent(ref eventDescriptor, null, null, 2, (IntPtr)((void*)ptr2)))
						{
							if (EventProvider.GetLastWriteEventError() == EventProvider.WriteEventErrorCode.EventTooBig && manifestEnvelope.ChunkNumber == 0 && num > 256)
							{
								num /= 2;
								goto IL_CA;
							}
							goto IL_141;
						}
						else
						{
							i -= num;
							ptr2[1].Ptr += (ulong)num;
							manifestEnvelope.ChunkNumber += 1;
							if (manifestEnvelope.ChunkNumber % 5 == 0)
							{
								Thread.Sleep(15);
							}
						}
					}
					goto IL_19C;
				}
				IL_141:
				result = false;
				if (this.ThrowOnEventWriteErrors)
				{
					this.ThrowEventSourceException("SendManifest", null);
				}
				IL_19C:;
			}
			return result;
		}

		internal static Attribute GetCustomAttributeHelper(MemberInfo member, Type attributeType, EventManifestOptions flags = EventManifestOptions.None)
		{
			if (!member.Module.Assembly.ReflectionOnly() && (flags & EventManifestOptions.AllowEventSourceOverride) == EventManifestOptions.None)
			{
				Attribute result = null;
				object[] customAttributes = member.GetCustomAttributes(attributeType, false);
				int num = 0;
				if (num < customAttributes.Length)
				{
					object obj = customAttributes[num];
					result = (Attribute)obj;
				}
				return result;
			}
			string fullName = attributeType.FullName;
			foreach (CustomAttributeData customAttributeData in CustomAttributeData.GetCustomAttributes(member))
			{
				if (EventSource.AttributeTypeNamesMatch(attributeType, customAttributeData.Constructor.ReflectedType))
				{
					Attribute attribute = null;
					if (customAttributeData.ConstructorArguments.Count == 1)
					{
						attribute = (Attribute)Activator.CreateInstance(attributeType, new object[]
						{
							customAttributeData.ConstructorArguments[0].Value
						});
					}
					else if (customAttributeData.ConstructorArguments.Count == 0)
					{
						attribute = (Attribute)Activator.CreateInstance(attributeType);
					}
					if (attribute != null)
					{
						Type type = attribute.GetType();
						foreach (CustomAttributeNamedArgument customAttributeNamedArgument in customAttributeData.NamedArguments)
						{
							PropertyInfo property = type.GetProperty(customAttributeNamedArgument.MemberInfo.Name, BindingFlags.Instance | BindingFlags.Public);
							object obj2 = customAttributeNamedArgument.TypedValue.Value;
							if (property.PropertyType.IsEnum)
							{
								obj2 = Enum.Parse(property.PropertyType, obj2.ToString());
							}
							property.SetValue(attribute, obj2, null);
						}
						return attribute;
					}
				}
			}
			return null;
		}

		private static bool AttributeTypeNamesMatch(Type attributeType, Type reflectedAttributeType)
		{
			return attributeType == reflectedAttributeType || string.Equals(attributeType.FullName, reflectedAttributeType.FullName, StringComparison.Ordinal) || (string.Equals(attributeType.Name, reflectedAttributeType.Name, StringComparison.Ordinal) && attributeType.Namespace.EndsWith("Diagnostics.Tracing") && reflectedAttributeType.Namespace.EndsWith("Diagnostics.Tracing"));
		}

		private static Type GetEventSourceBaseType(Type eventSourceType, bool allowEventSourceOverride, bool reflectionOnly)
		{
			if (eventSourceType.BaseType() == null)
			{
				return null;
			}
			do
			{
				eventSourceType = eventSourceType.BaseType();
			}
			while (eventSourceType != null && eventSourceType.IsAbstract());
			if (eventSourceType != null)
			{
				if (!allowEventSourceOverride)
				{
					if ((reflectionOnly && eventSourceType.FullName != typeof(EventSource).FullName) || (!reflectionOnly && eventSourceType != typeof(EventSource)))
					{
						return null;
					}
				}
				else if (eventSourceType.Name != "EventSource")
				{
					return null;
				}
			}
			return eventSourceType;
		}

		private static byte[] CreateManifestAndDescriptors(Type eventSourceType, string eventSourceDllName, EventSource source, EventManifestOptions flags = EventManifestOptions.None)
		{
			ManifestBuilder manifestBuilder = null;
			bool flag = source == null || !source.SelfDescribingEvents;
			Exception ex = null;
			byte[] result = null;
			if (eventSourceType.IsAbstract() && (flags & EventManifestOptions.Strict) == EventManifestOptions.None)
			{
				return null;
			}
			try
			{
				MethodInfo[] methods = eventSourceType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				int num = 1;
				EventSource.EventMetadata[] array = null;
				Dictionary<string, string> dictionary = null;
				if (source != null || (flags & EventManifestOptions.Strict) != EventManifestOptions.None)
				{
					array = new EventSource.EventMetadata[methods.Length + 1];
					array[0].Name = "";
				}
				ResourceManager resources = null;
				EventSourceAttribute eventSourceAttribute = (EventSourceAttribute)EventSource.GetCustomAttributeHelper(eventSourceType, typeof(EventSourceAttribute), flags);
				if (eventSourceAttribute != null && eventSourceAttribute.LocalizationResources != null)
				{
					resources = new ResourceManager(eventSourceAttribute.LocalizationResources, eventSourceType.Assembly());
				}
				manifestBuilder = new ManifestBuilder(EventSource.GetName(eventSourceType, flags), EventSource.GetGuid(eventSourceType), eventSourceDllName, resources, flags);
				manifestBuilder.StartEvent("EventSourceMessage", new EventAttribute(0)
				{
					Level = EventLevel.LogAlways,
					Task = (EventTask)65534
				});
				manifestBuilder.AddEventParameter(typeof(string), "message");
				manifestBuilder.EndEvent();
				if ((flags & EventManifestOptions.Strict) != EventManifestOptions.None)
				{
					if (!(EventSource.GetEventSourceBaseType(eventSourceType, (flags & EventManifestOptions.AllowEventSourceOverride) > EventManifestOptions.None, eventSourceType.Assembly().ReflectionOnly()) != null))
					{
						manifestBuilder.ManifestError(Environment.GetResourceString("EventSource_TypeMustDeriveFromEventSource"), false);
					}
					if (!eventSourceType.IsAbstract() && !eventSourceType.IsSealed())
					{
						manifestBuilder.ManifestError(Environment.GetResourceString("EventSource_TypeMustBeSealedOrAbstract"), false);
					}
				}
				foreach (string text in new string[]
				{
					"Keywords",
					"Tasks",
					"Opcodes"
				})
				{
					Type nestedType = eventSourceType.GetNestedType(text);
					if (nestedType != null)
					{
						if (eventSourceType.IsAbstract())
						{
							manifestBuilder.ManifestError(Environment.GetResourceString("EventSource_AbstractMustNotDeclareKTOC", new object[]
							{
								nestedType.Name
							}), false);
						}
						else
						{
							foreach (FieldInfo staticField in nestedType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
							{
								EventSource.AddProviderEnumKind(manifestBuilder, staticField, text);
							}
						}
					}
				}
				manifestBuilder.AddKeyword("Session3", 17592186044416UL);
				manifestBuilder.AddKeyword("Session2", 35184372088832UL);
				manifestBuilder.AddKeyword("Session1", 70368744177664UL);
				manifestBuilder.AddKeyword("Session0", 140737488355328UL);
				if (eventSourceType != typeof(EventSource))
				{
					foreach (MethodInfo methodInfo in methods)
					{
						ParameterInfo[] parameters = methodInfo.GetParameters();
						EventAttribute eventAttribute = (EventAttribute)EventSource.GetCustomAttributeHelper(methodInfo, typeof(EventAttribute), flags);
						if (eventAttribute != null && source != null && eventAttribute.EventId <= 3 && source.Guid.Equals(EventSource.AspNetEventSourceGuid))
						{
							eventAttribute.ActivityOptions |= EventActivityOptions.Disable;
						}
						if (!methodInfo.IsStatic)
						{
							if (eventSourceType.IsAbstract())
							{
								if (eventAttribute != null)
								{
									manifestBuilder.ManifestError(Environment.GetResourceString("EventSource_AbstractMustNotDeclareEventMethods", new object[]
									{
										methodInfo.Name,
										eventAttribute.EventId
									}), false);
								}
							}
							else
							{
								if (eventAttribute == null)
								{
									if (methodInfo.ReturnType != typeof(void) || methodInfo.IsVirtual || EventSource.GetCustomAttributeHelper(methodInfo, typeof(NonEventAttribute), flags) != null)
									{
										goto IL_666;
									}
									EventAttribute eventAttribute2 = new EventAttribute(num);
									eventAttribute = eventAttribute2;
								}
								else if (eventAttribute.EventId <= 0)
								{
									manifestBuilder.ManifestError(Environment.GetResourceString("EventSource_NeedPositiveId", new object[]
									{
										methodInfo.Name
									}), true);
									goto IL_666;
								}
								if (methodInfo.Name.LastIndexOf('.') >= 0)
								{
									manifestBuilder.ManifestError(Environment.GetResourceString("EventSource_EventMustNotBeExplicitImplementation", new object[]
									{
										methodInfo.Name,
										eventAttribute.EventId
									}), false);
								}
								num++;
								string name = methodInfo.Name;
								if (eventAttribute.Opcode == EventOpcode.Info)
								{
									bool flag2 = eventAttribute.Task == EventTask.None;
									if (flag2)
									{
										eventAttribute.Task = (EventTask)65534 - eventAttribute.EventId;
									}
									if (!eventAttribute.IsOpcodeSet)
									{
										eventAttribute.Opcode = EventSource.GetOpcodeWithDefault(EventOpcode.Info, name);
									}
									if (flag2)
									{
										if (eventAttribute.Opcode == EventOpcode.Start)
										{
											string text2 = name.Substring(0, name.Length - "Start".Length);
											if (string.Compare(name, 0, text2, 0, text2.Length) == 0 && string.Compare(name, text2.Length, "Start", 0, Math.Max(name.Length - text2.Length, "Start".Length)) == 0)
											{
												manifestBuilder.AddTask(text2, (int)eventAttribute.Task);
											}
										}
										else if (eventAttribute.Opcode == EventOpcode.Stop)
										{
											int num2 = eventAttribute.EventId - 1;
											if (array != null && num2 < array.Length)
											{
												EventSource.EventMetadata eventMetadata = array[num2];
												string text3 = name.Substring(0, name.Length - "Stop".Length);
												if (eventMetadata.Descriptor.Opcode == 1 && string.Compare(eventMetadata.Name, 0, text3, 0, text3.Length) == 0 && string.Compare(eventMetadata.Name, text3.Length, "Start", 0, Math.Max(eventMetadata.Name.Length - text3.Length, "Start".Length)) == 0)
												{
													eventAttribute.Task = (EventTask)eventMetadata.Descriptor.Task;
													flag2 = false;
												}
											}
											if (flag2 && (flags & EventManifestOptions.Strict) != EventManifestOptions.None)
											{
												throw new ArgumentException(Environment.GetResourceString("EventSource_StopsFollowStarts"));
											}
										}
									}
								}
								bool hasRelatedActivityID = EventSource.RemoveFirstArgIfRelatedActivityId(ref parameters);
								if (source == null || !source.SelfDescribingEvents)
								{
									manifestBuilder.StartEvent(name, eventAttribute);
									for (int l = 0; l < parameters.Length; l++)
									{
										manifestBuilder.AddEventParameter(parameters[l].ParameterType, parameters[l].Name);
									}
									manifestBuilder.EndEvent();
								}
								if (source != null || (flags & EventManifestOptions.Strict) != EventManifestOptions.None)
								{
									EventSource.DebugCheckEvent(ref dictionary, array, methodInfo, eventAttribute, manifestBuilder, flags);
									if (eventAttribute.Channel != EventChannel.None)
									{
										eventAttribute.Keywords |= (EventKeywords)manifestBuilder.GetChannelKeyword(eventAttribute.Channel);
									}
									string key = "event_" + name;
									string localizedMessage = manifestBuilder.GetLocalizedMessage(key, CultureInfo.CurrentUICulture, false);
									if (localizedMessage != null)
									{
										eventAttribute.Message = localizedMessage;
									}
									EventSource.AddEventDescriptor(ref array, name, eventAttribute, parameters, hasRelatedActivityID);
								}
							}
						}
						IL_666:;
					}
				}
				NameInfo.ReserveEventIDsBelow(num);
				if (source != null)
				{
					EventSource.TrimEventDescriptors(ref array);
					source.m_eventData = array;
					source.m_channelData = manifestBuilder.GetChannelData();
				}
				if (!eventSourceType.IsAbstract() && (source == null || !source.SelfDescribingEvents))
				{
					flag = ((flags & EventManifestOptions.OnlyIfNeededForRegistration) == EventManifestOptions.None || manifestBuilder.GetChannelData().Length != 0);
					if (!flag && (flags & EventManifestOptions.Strict) == EventManifestOptions.None)
					{
						return null;
					}
					result = manifestBuilder.CreateManifest();
				}
			}
			catch (Exception ex2)
			{
				if ((flags & EventManifestOptions.Strict) == EventManifestOptions.None)
				{
					throw;
				}
				ex = ex2;
			}
			if ((flags & EventManifestOptions.Strict) != EventManifestOptions.None && (manifestBuilder.Errors.Count > 0 || ex != null))
			{
				string text4 = string.Empty;
				if (manifestBuilder.Errors.Count > 0)
				{
					bool flag3 = true;
					using (IEnumerator<string> enumerator = manifestBuilder.Errors.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string str = enumerator.Current;
							if (!flag3)
							{
								text4 += Environment.NewLine;
							}
							flag3 = false;
							text4 += str;
						}
						goto IL_782;
					}
				}
				text4 = "Unexpected error: " + ex.Message;
				IL_782:
				throw new ArgumentException(text4, ex);
			}
			if (!flag)
			{
				return null;
			}
			return result;
		}

		private static bool RemoveFirstArgIfRelatedActivityId(ref ParameterInfo[] args)
		{
			if (args.Length != 0 && args[0].ParameterType == typeof(Guid) && string.Compare(args[0].Name, "relatedActivityId", StringComparison.OrdinalIgnoreCase) == 0)
			{
				ParameterInfo[] array = new ParameterInfo[args.Length - 1];
				Array.Copy(args, 1, array, 0, args.Length - 1);
				args = array;
				return true;
			}
			return false;
		}

		private static void AddProviderEnumKind(ManifestBuilder manifest, FieldInfo staticField, string providerEnumKind)
		{
			bool flag = staticField.Module.Assembly.ReflectionOnly();
			Type fieldType = staticField.FieldType;
			if ((!flag && fieldType == typeof(EventOpcode)) || EventSource.AttributeTypeNamesMatch(fieldType, typeof(EventOpcode)))
			{
				if (!(providerEnumKind != "Opcodes"))
				{
					int value = (int)staticField.GetRawConstantValue();
					manifest.AddOpcode(staticField.Name, value);
					return;
				}
			}
			else
			{
				if ((flag || !(fieldType == typeof(EventTask))) && !EventSource.AttributeTypeNamesMatch(fieldType, typeof(EventTask)))
				{
					if ((!flag && fieldType == typeof(EventKeywords)) || EventSource.AttributeTypeNamesMatch(fieldType, typeof(EventKeywords)))
					{
						if (providerEnumKind != "Keywords")
						{
							goto IL_107;
						}
						ulong value2 = (ulong)((long)staticField.GetRawConstantValue());
						manifest.AddKeyword(staticField.Name, value2);
					}
					return;
				}
				if (!(providerEnumKind != "Tasks"))
				{
					int value3 = (int)staticField.GetRawConstantValue();
					manifest.AddTask(staticField.Name, value3);
					return;
				}
			}
			IL_107:
			manifest.ManifestError(Environment.GetResourceString("EventSource_EnumKindMismatch", new object[]
			{
				staticField.Name,
				staticField.FieldType.Name,
				providerEnumKind
			}), false);
		}

		private static void AddEventDescriptor(ref EventSource.EventMetadata[] eventData, string eventName, EventAttribute eventAttribute, ParameterInfo[] eventParameters, bool hasRelatedActivityID)
		{
			if (eventData == null || eventData.Length <= eventAttribute.EventId)
			{
				EventSource.EventMetadata[] array = new EventSource.EventMetadata[Math.Max(eventData.Length + 16, eventAttribute.EventId + 1)];
				Array.Copy(eventData, array, eventData.Length);
				eventData = array;
			}
			eventData[eventAttribute.EventId].Descriptor = new EventDescriptor(eventAttribute.EventId, eventAttribute.Version, (byte)eventAttribute.Channel, (byte)eventAttribute.Level, (byte)eventAttribute.Opcode, (int)eventAttribute.Task, (long)(eventAttribute.Keywords | (EventKeywords)SessionMask.All.ToEventKeywords()));
			eventData[eventAttribute.EventId].Tags = eventAttribute.Tags;
			eventData[eventAttribute.EventId].Name = eventName;
			eventData[eventAttribute.EventId].Parameters = eventParameters;
			eventData[eventAttribute.EventId].Message = eventAttribute.Message;
			eventData[eventAttribute.EventId].ActivityOptions = eventAttribute.ActivityOptions;
			eventData[eventAttribute.EventId].HasRelatedActivityID = hasRelatedActivityID;
		}

		private static void TrimEventDescriptors(ref EventSource.EventMetadata[] eventData)
		{
			int num = eventData.Length;
			while (0 < num)
			{
				num--;
				if (eventData[num].Descriptor.EventId != 0)
				{
					break;
				}
			}
			if (eventData.Length - num > 2)
			{
				EventSource.EventMetadata[] array = new EventSource.EventMetadata[num + 1];
				Array.Copy(eventData, array, array.Length);
				eventData = array;
			}
		}

		internal void AddListener(EventListener listener)
		{
			object eventListenersLock = EventListener.EventListenersLock;
			lock (eventListenersLock)
			{
				bool[] eventEnabled = null;
				if (this.m_eventData != null)
				{
					eventEnabled = new bool[this.m_eventData.Length];
				}
				this.m_Dispatchers = new EventDispatcher(this.m_Dispatchers, eventEnabled, listener);
				listener.OnEventSourceCreated(this);
			}
		}

		private static void DebugCheckEvent(ref Dictionary<string, string> eventsByName, EventSource.EventMetadata[] eventData, MethodInfo method, EventAttribute eventAttribute, ManifestBuilder manifest, EventManifestOptions options)
		{
			int eventId = eventAttribute.EventId;
			string name = method.Name;
			int helperCallFirstArg = EventSource.GetHelperCallFirstArg(method);
			if (helperCallFirstArg >= 0 && eventId != helperCallFirstArg)
			{
				manifest.ManifestError(Environment.GetResourceString("EventSource_MismatchIdToWriteEvent", new object[]
				{
					name,
					eventId,
					helperCallFirstArg
				}), true);
			}
			if (eventId < eventData.Length && eventData[eventId].Descriptor.EventId != 0)
			{
				manifest.ManifestError(Environment.GetResourceString("EventSource_EventIdReused", new object[]
				{
					name,
					eventId,
					eventData[eventId].Name
				}), true);
			}
			for (int i = 0; i < eventData.Length; i++)
			{
				if (eventData[i].Name != null && eventData[i].Descriptor.Task == (int)eventAttribute.Task && (EventOpcode)eventData[i].Descriptor.Opcode == eventAttribute.Opcode)
				{
					manifest.ManifestError(Environment.GetResourceString("EventSource_TaskOpcodePairReused", new object[]
					{
						name,
						eventId,
						eventData[i].Name,
						i
					}), false);
					if ((options & EventManifestOptions.Strict) == EventManifestOptions.None)
					{
						break;
					}
				}
			}
			if (eventAttribute.Opcode != EventOpcode.Info)
			{
				bool flag = false;
				if (eventAttribute.Task == EventTask.None)
				{
					flag = true;
				}
				else
				{
					EventTask eventTask = (EventTask)65534 - eventId;
					if (eventAttribute.Opcode != EventOpcode.Start && eventAttribute.Opcode != EventOpcode.Stop && eventAttribute.Task == eventTask)
					{
						flag = true;
					}
				}
				if (flag)
				{
					manifest.ManifestError(Environment.GetResourceString("EventSource_EventMustHaveTaskIfNonDefaultOpcode", new object[]
					{
						name,
						eventId
					}), false);
				}
			}
			if (eventsByName == null)
			{
				eventsByName = new Dictionary<string, string>();
			}
			if (eventsByName.ContainsKey(name))
			{
				manifest.ManifestError(Environment.GetResourceString("EventSource_EventNameReused", new object[]
				{
					name
				}), true);
			}
			eventsByName[name] = name;
		}

		[SecuritySafeCritical]
		private static int GetHelperCallFirstArg(MethodInfo method)
		{
			new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Assert();
			byte[] ilasByteArray = method.GetMethodBody().GetILAsByteArray();
			int num = -1;
			for (int i = 0; i < ilasByteArray.Length; i++)
			{
				byte b = ilasByteArray[i];
				if (b <= 110)
				{
					switch (b)
					{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 20:
					case 37:
						break;
					case 14:
					case 16:
						i++;
						break;
					case 15:
					case 17:
					case 18:
					case 19:
					case 33:
					case 34:
					case 35:
					case 36:
					case 38:
					case 39:
					case 41:
					case 42:
					case 43:
					case 46:
					case 47:
					case 48:
					case 49:
					case 50:
					case 51:
					case 52:
					case 53:
					case 54:
					case 55:
					case 56:
						return -1;
					case 21:
					case 22:
					case 23:
					case 24:
					case 25:
					case 26:
					case 27:
					case 28:
					case 29:
					case 30:
						if (i > 0 && ilasByteArray[i - 1] == 2)
						{
							num = (int)(ilasByteArray[i] - 22);
						}
						break;
					case 31:
						if (i > 0 && ilasByteArray[i - 1] == 2)
						{
							num = (int)ilasByteArray[i + 1];
						}
						i++;
						break;
					case 32:
						i += 4;
						break;
					case 40:
						i += 4;
						if (num >= 0)
						{
							for (int j = i + 1; j < ilasByteArray.Length; j++)
							{
								if (ilasByteArray[j] == 42)
								{
									return num;
								}
								if (ilasByteArray[j] != 0)
								{
									break;
								}
							}
						}
						num = -1;
						break;
					case 44:
					case 45:
						num = -1;
						i++;
						break;
					case 57:
					case 58:
						num = -1;
						i += 4;
						break;
					default:
						if (b - 103 > 3 && b - 109 > 1)
						{
							return -1;
						}
						break;
					}
				}
				else if (b - 140 > 1)
				{
					if (b != 162)
					{
						if (b != 254)
						{
							return -1;
						}
						i++;
						if (i >= ilasByteArray.Length || ilasByteArray[i] >= 6)
						{
							return -1;
						}
					}
				}
				else
				{
					i += 4;
				}
			}
			return -1;
		}

		internal void ReportOutOfBandMessage(string msg, bool flush)
		{
			try
			{
				Debugger.Log(0, null, msg + "\r\n");
				if (this.m_outOfBandMessageCount < 15)
				{
					this.m_outOfBandMessageCount += 1;
				}
				else
				{
					if (this.m_outOfBandMessageCount == 16)
					{
						return;
					}
					this.m_outOfBandMessageCount = 16;
					msg = "Reached message limit.   End of EventSource error messages.";
				}
				this.WriteEventString(EventLevel.LogAlways, -1L, msg);
				this.WriteStringToAllListeners("EventSourceMessage", msg);
			}
			catch (Exception)
			{
			}
		}

		private EventSourceSettings ValidateSettings(EventSourceSettings settings)
		{
			EventSourceSettings eventSourceSettings = EventSourceSettings.EtwManifestEventFormat | EventSourceSettings.EtwSelfDescribingEventFormat;
			if ((settings & eventSourceSettings) == eventSourceSettings)
			{
				throw new ArgumentException(Environment.GetResourceString("EventSource_InvalidEventFormat"), "settings");
			}
			if ((settings & eventSourceSettings) == EventSourceSettings.Default)
			{
				settings |= EventSourceSettings.EtwSelfDescribingEventFormat;
			}
			return settings;
		}

		private bool ThrowOnEventWriteErrors
		{
			get
			{
				return (this.m_config & EventSourceSettings.ThrowOnEventWriteErrors) > EventSourceSettings.Default;
			}
			set
			{
				if (value)
				{
					this.m_config |= EventSourceSettings.ThrowOnEventWriteErrors;
					return;
				}
				this.m_config &= ~EventSourceSettings.ThrowOnEventWriteErrors;
			}
		}

		private bool SelfDescribingEvents
		{
			get
			{
				return (this.m_config & EventSourceSettings.EtwSelfDescribingEventFormat) > EventSourceSettings.Default;
			}
			set
			{
				if (!value)
				{
					this.m_config |= EventSourceSettings.EtwManifestEventFormat;
					this.m_config &= ~EventSourceSettings.EtwSelfDescribingEventFormat;
					return;
				}
				this.m_config |= EventSourceSettings.EtwSelfDescribingEventFormat;
				this.m_config &= ~EventSourceSettings.EtwManifestEventFormat;
			}
		}

		private void ReportActivitySamplingInfo(EventListener listener, SessionMask sessions)
		{
			int num = 0;
			while ((long)num < 4L)
			{
				if (sessions[num])
				{
					ActivityFilter activityFilter;
					if (listener == null)
					{
						EtwSession etwSession = this.m_etwSessionIdMap[num];
						activityFilter = etwSession.m_activityFilter;
					}
					else
					{
						activityFilter = listener.m_activityFilter;
					}
					if (activityFilter != null)
					{
						SessionMask m = default(SessionMask);
						m[num] = true;
						foreach (Tuple<int, int> tuple in activityFilter.GetFilterAsTuple(this.m_guid))
						{
							this.WriteStringToListener(listener, string.Format(CultureInfo.InvariantCulture, "Session {0}: {1} = {2}", num, tuple.Item1, tuple.Item2), m);
						}
						bool flag = (listener == null) ? this.m_activityFilteringForETWEnabled[num] : this.GetDispatcher(listener).m_activityFilteringEnabled;
						this.WriteStringToListener(listener, string.Format(CultureInfo.InvariantCulture, "Session {0}: Activity Sampling support: {1}", num, flag ? "enabled" : "disabled"), m);
					}
				}
				num++;
			}
		}

		[__DynamicallyInvokable]
		public EventSource(string eventSourceName) : this(eventSourceName, EventSourceSettings.EtwSelfDescribingEventFormat)
		{
		}

		[__DynamicallyInvokable]
		public EventSource(string eventSourceName, EventSourceSettings config) : this(eventSourceName, config, null)
		{
		}

		[__DynamicallyInvokable]
		public EventSource(string eventSourceName, EventSourceSettings config, params string[] traits) : this((eventSourceName == null) ? default(Guid) : EventSource.GenerateGuidFromName(eventSourceName.ToUpperInvariant()), eventSourceName, config, traits)
		{
			if (eventSourceName == null)
			{
				throw new ArgumentNullException("eventSourceName");
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public void Write(string eventName)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (!this.IsEnabled())
			{
				return;
			}
			EventSourceOptions eventSourceOptions = default(EventSourceOptions);
			EmptyStruct emptyStruct = default(EmptyStruct);
			this.WriteImpl<EmptyStruct>(eventName, ref eventSourceOptions, ref emptyStruct, null, null);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public void Write(string eventName, EventSourceOptions options)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (!this.IsEnabled())
			{
				return;
			}
			EmptyStruct emptyStruct = default(EmptyStruct);
			this.WriteImpl<EmptyStruct>(eventName, ref options, ref emptyStruct, null, null);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public void Write<T>(string eventName, T data)
		{
			if (!this.IsEnabled())
			{
				return;
			}
			EventSourceOptions eventSourceOptions = default(EventSourceOptions);
			this.WriteImpl<T>(eventName, ref eventSourceOptions, ref data, null, null);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public void Write<T>(string eventName, EventSourceOptions options, T data)
		{
			if (!this.IsEnabled())
			{
				return;
			}
			this.WriteImpl<T>(eventName, ref options, ref data, null, null);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public void Write<T>(string eventName, ref EventSourceOptions options, ref T data)
		{
			if (!this.IsEnabled())
			{
				return;
			}
			this.WriteImpl<T>(eventName, ref options, ref data, null, null);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe void Write<T>(string eventName, ref EventSourceOptions options, ref Guid activityId, ref Guid relatedActivityId, ref T data)
		{
			if (!this.IsEnabled())
			{
				return;
			}
			fixed (Guid* ptr = &activityId, ptr2 = &relatedActivityId)
			{
				this.WriteImpl<T>(eventName, ref options, ref data, ptr, (relatedActivityId == Guid.Empty) ? null : ptr2);
			}
		}

		[SecuritySafeCritical]
		private unsafe void WriteMultiMerge(string eventName, ref EventSourceOptions options, TraceLoggingEventTypes eventTypes, Guid* activityID, Guid* childActivityID, params object[] values)
		{
			if (!this.IsEnabled())
			{
				return;
			}
			byte level = ((options.valuesSet & 4) != 0) ? options.level : eventTypes.level;
			EventKeywords keywords = ((options.valuesSet & 1) != 0) ? options.keywords : eventTypes.keywords;
			if (this.IsEnabled((EventLevel)level, keywords))
			{
				this.WriteMultiMergeInner(eventName, ref options, eventTypes, activityID, childActivityID, values);
			}
		}

		[SecuritySafeCritical]
		private unsafe void WriteMultiMergeInner(string eventName, ref EventSourceOptions options, TraceLoggingEventTypes eventTypes, Guid* activityID, Guid* childActivityID, params object[] values)
		{
			byte level = ((options.valuesSet & 4) != 0) ? options.level : eventTypes.level;
			byte opcode = ((options.valuesSet & 8) != 0) ? options.opcode : eventTypes.opcode;
			EventTags tags = ((options.valuesSet & 2) != 0) ? options.tags : eventTypes.Tags;
			EventKeywords keywords = ((options.valuesSet & 1) != 0) ? options.keywords : eventTypes.keywords;
			NameInfo nameInfo = eventTypes.GetNameInfo(eventName ?? eventTypes.Name, tags);
			if (nameInfo == null)
			{
				return;
			}
			int identity = nameInfo.identity;
			EventDescriptor eventDescriptor = new EventDescriptor(identity, level, opcode, (long)keywords);
			int pinCount = eventTypes.pinCount;
			byte* scratch;
			EventSource.EventData* ptr;
			GCHandle* ptr2;
			checked
			{
				scratch = stackalloc byte[unchecked((UIntPtr)eventTypes.scratchSize) * 1];
				ptr = stackalloc EventSource.EventData[unchecked((UIntPtr)(eventTypes.dataCount + 3)) * (UIntPtr)sizeof(EventSource.EventData)];
				ptr2 = stackalloc GCHandle[unchecked((UIntPtr)pinCount) * (UIntPtr)sizeof(GCHandle)];
			}
			fixed (byte* ptr3 = this.providerMetadata, nameMetadata = nameInfo.nameMetadata, typeMetadata = eventTypes.typeMetadata)
			{
				ptr->SetMetadata(ptr3, this.providerMetadata.Length, 2);
				ptr[1].SetMetadata(nameMetadata, nameInfo.nameMetadata.Length, 1);
				ptr[2].SetMetadata(typeMetadata, eventTypes.typeMetadata.Length, 1);
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					DataCollector.ThreadInstance.Enable(scratch, eventTypes.scratchSize, ptr + 3, eventTypes.dataCount, ptr2, pinCount);
					for (int i = 0; i < eventTypes.typeInfos.Length; i++)
					{
						eventTypes.typeInfos[i].WriteObjectData(TraceLoggingDataCollector.Instance, values[i]);
					}
					this.WriteEventRaw(eventName, ref eventDescriptor, activityID, childActivityID, (int)((long)(DataCollector.ThreadInstance.Finish() - ptr)), (IntPtr)((void*)ptr));
				}
				finally
				{
					this.WriteCleanup(ptr2, pinCount);
				}
			}
		}

		[SecuritySafeCritical]
		internal unsafe void WriteMultiMerge(string eventName, ref EventSourceOptions options, TraceLoggingEventTypes eventTypes, Guid* activityID, Guid* childActivityID, EventSource.EventData* data)
		{
			if (!this.IsEnabled())
			{
				return;
			}
			fixed (EventSourceOptions* ptr = &options)
			{
				EventDescriptor eventDescriptor;
				NameInfo nameInfo = this.UpdateDescriptor(eventName, eventTypes, ref options, out eventDescriptor);
				if (nameInfo == null)
				{
					return;
				}
				EventSource.EventData* ptr2 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)(eventTypes.dataCount + eventTypes.typeInfos.Length * 2 + 3)) * (UIntPtr)sizeof(EventSource.EventData))];
				fixed (byte* ptr3 = this.providerMetadata, nameMetadata = nameInfo.nameMetadata, typeMetadata = eventTypes.typeMetadata)
				{
					ptr2->SetMetadata(ptr3, this.providerMetadata.Length, 2);
					ptr2[1].SetMetadata(nameMetadata, nameInfo.nameMetadata.Length, 1);
					ptr2[2].SetMetadata(typeMetadata, eventTypes.typeMetadata.Length, 1);
					int num = 3;
					for (int i = 0; i < eventTypes.typeInfos.Length; i++)
					{
						if (eventTypes.typeInfos[i].DataType == typeof(string))
						{
							ptr2[num].DataPointer = (IntPtr)((void*)(&ptr2[num + 1].m_Size));
							ptr2[num].m_Size = 2;
							num++;
							ptr2[num].m_Ptr = data[i].m_Ptr;
							ptr2[num].m_Size = data[i].m_Size - 2;
							num++;
						}
						else
						{
							ptr2[num].m_Ptr = data[i].m_Ptr;
							ptr2[num].m_Size = data[i].m_Size;
							if (data[i].m_Size == 4 && eventTypes.typeInfos[i].DataType == typeof(bool))
							{
								ptr2[num].m_Size = 1;
							}
							num++;
						}
					}
					this.WriteEventRaw(eventName, ref eventDescriptor, activityID, childActivityID, num, (IntPtr)((void*)ptr2));
				}
			}
		}

		[SecuritySafeCritical]
		private unsafe void WriteImpl<T>(string eventName, ref EventSourceOptions options, ref T data, Guid* pActivityId, Guid* pRelatedActivityId)
		{
			try
			{
				SimpleEventTypes<T> instance = SimpleEventTypes<T>.Instance;
				try
				{
					fixed (EventSourceOptions* ptr = &options)
					{
						options.Opcode = (options.IsOpcodeSet ? options.Opcode : EventSource.GetOpcodeWithDefault(options.Opcode, eventName));
						EventDescriptor eventDescriptor;
						NameInfo nameInfo = this.UpdateDescriptor(eventName, instance, ref options, out eventDescriptor);
						if (nameInfo != null)
						{
							int pinCount = instance.pinCount;
							byte* scratch;
							EventSource.EventData* ptr2;
							GCHandle* ptr3;
							checked
							{
								scratch = stackalloc byte[unchecked((UIntPtr)instance.scratchSize) * 1];
								ptr2 = stackalloc EventSource.EventData[unchecked((UIntPtr)(instance.dataCount + 3)) * (UIntPtr)sizeof(EventSource.EventData)];
								ptr3 = stackalloc GCHandle[unchecked((UIntPtr)pinCount) * (UIntPtr)sizeof(GCHandle)];
							}
							try
							{
								fixed (byte* ptr4 = this.providerMetadata)
								{
									fixed (byte* ptr5 = nameInfo.nameMetadata)
									{
										fixed (byte* ptr6 = instance.typeMetadata)
										{
											ptr2->SetMetadata(ptr4, this.providerMetadata.Length, 2);
											ptr2[1].SetMetadata(ptr5, nameInfo.nameMetadata.Length, 1);
											ptr2[2].SetMetadata(ptr6, instance.typeMetadata.Length, 1);
											RuntimeHelpers.PrepareConstrainedRegions();
											EventOpcode opcode = (EventOpcode)eventDescriptor.Opcode;
											Guid empty = Guid.Empty;
											Guid empty2 = Guid.Empty;
											if (pActivityId == null && pRelatedActivityId == null && (options.ActivityOptions & EventActivityOptions.Disable) == EventActivityOptions.None)
											{
												if (opcode == EventOpcode.Start)
												{
													this.m_activityTracker.OnStart(this.m_name, eventName, 0, ref empty, ref empty2, options.ActivityOptions);
												}
												else if (opcode == EventOpcode.Stop)
												{
													this.m_activityTracker.OnStop(this.m_name, eventName, 0, ref empty);
												}
												if (empty != Guid.Empty)
												{
													pActivityId = &empty;
												}
												if (empty2 != Guid.Empty)
												{
													pRelatedActivityId = &empty2;
												}
											}
											try
											{
												DataCollector.ThreadInstance.Enable(scratch, instance.scratchSize, ptr2 + 3, instance.dataCount, ptr3, pinCount);
												instance.typeInfo.WriteData(TraceLoggingDataCollector.Instance, ref data);
												this.WriteEventRaw(eventName, ref eventDescriptor, pActivityId, pRelatedActivityId, (int)((long)(DataCollector.ThreadInstance.Finish() - ptr2)), (IntPtr)((void*)ptr2));
												if (this.m_Dispatchers != null)
												{
													EventPayload payload = (EventPayload)instance.typeInfo.GetData(data);
													this.WriteToAllListeners(eventName, ref eventDescriptor, nameInfo.tags, pActivityId, pRelatedActivityId, payload);
												}
											}
											catch (Exception ex)
											{
												if (ex is EventSourceException)
												{
													throw;
												}
												this.ThrowEventSourceException(eventName, ex);
											}
											finally
											{
												this.WriteCleanup(ptr3, pinCount);
											}
										}
									}
								}
							}
							finally
							{
								byte* ptr4 = null;
								byte* ptr5 = null;
								byte* ptr6 = null;
							}
						}
					}
				}
				finally
				{
					EventSourceOptions* ptr = null;
				}
			}
			catch (Exception ex2)
			{
				if (ex2 is EventSourceException)
				{
					throw;
				}
				this.ThrowEventSourceException(eventName, ex2);
			}
		}

		[SecurityCritical]
		private unsafe void WriteToAllListeners(string eventName, ref EventDescriptor eventDescriptor, EventTags tags, Guid* pActivityId, Guid* pChildActivityId, EventPayload payload)
		{
			EventWrittenEventArgs eventWrittenEventArgs = new EventWrittenEventArgs(this);
			eventWrittenEventArgs.EventName = eventName;
			eventWrittenEventArgs.m_level = (EventLevel)eventDescriptor.Level;
			eventWrittenEventArgs.m_keywords = (EventKeywords)eventDescriptor.Keywords;
			eventWrittenEventArgs.m_opcode = (EventOpcode)eventDescriptor.Opcode;
			eventWrittenEventArgs.m_tags = tags;
			eventWrittenEventArgs.EventId = -1;
			if (pActivityId != null)
			{
				eventWrittenEventArgs.ActivityId = *pActivityId;
			}
			if (pChildActivityId != null)
			{
				eventWrittenEventArgs.RelatedActivityId = *pChildActivityId;
			}
			if (payload != null)
			{
				eventWrittenEventArgs.Payload = new ReadOnlyCollection<object>((IList<object>)payload.Values);
				eventWrittenEventArgs.PayloadNames = new ReadOnlyCollection<string>((IList<string>)payload.Keys);
			}
			this.DispatchToAllListeners(-1, pActivityId, eventWrittenEventArgs);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SecurityCritical]
		[NonEvent]
		private unsafe void WriteCleanup(GCHandle* pPins, int cPins)
		{
			DataCollector.ThreadInstance.Disable();
			for (int num = 0; num != cPins; num++)
			{
				if (IntPtr.Zero != (IntPtr)pPins[num])
				{
					pPins[num].Free();
				}
			}
		}

		private void InitializeProviderMetadata()
		{
			if (this.m_traits != null)
			{
				List<byte> list = new List<byte>(100);
				for (int i = 0; i < this.m_traits.Length - 1; i += 2)
				{
					if (this.m_traits[i].StartsWith("ETW_"))
					{
						string text = this.m_traits[i].Substring(4);
						byte item;
						if (!byte.TryParse(text, out item))
						{
							if (!(text == "GROUP"))
							{
								throw new ArgumentException(Environment.GetResourceString("UnknownEtwTrait", new object[]
								{
									text
								}), "traits");
							}
							item = 1;
						}
						string value = this.m_traits[i + 1];
						int count = list.Count;
						list.Add(0);
						list.Add(0);
						list.Add(item);
						int num = EventSource.AddValueToMetaData(list, value) + 3;
						list[count] = (byte)num;
						list[count + 1] = (byte)(num >> 8);
					}
				}
				this.providerMetadata = Statics.MetadataForString(this.Name, 0, list.Count, 0);
				int num2 = this.providerMetadata.Length - list.Count;
				using (List<byte>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						byte b = enumerator.Current;
						this.providerMetadata[num2++] = b;
					}
					return;
				}
			}
			this.providerMetadata = Statics.MetadataForString(this.Name, 0, 0, 0);
		}

		private static int AddValueToMetaData(List<byte> metaData, string value)
		{
			if (value.Length == 0)
			{
				return 0;
			}
			int count = metaData.Count;
			char c = value[0];
			if (c == '@')
			{
				metaData.AddRange(Encoding.UTF8.GetBytes(value.Substring(1)));
			}
			else if (c == '{')
			{
				metaData.AddRange(new Guid(value).ToByteArray());
			}
			else if (c == '#')
			{
				for (int i = 1; i < value.Length; i++)
				{
					if (value[i] != ' ')
					{
						if (i + 1 >= value.Length)
						{
							throw new ArgumentException(Environment.GetResourceString("EvenHexDigits"), "traits");
						}
						metaData.Add((byte)(EventSource.HexDigit(value[i]) * 16 + EventSource.HexDigit(value[i + 1])));
						i++;
					}
				}
			}
			else
			{
				if (' ' > c)
				{
					throw new ArgumentException(Environment.GetResourceString("IllegalValue", new object[]
					{
						value
					}), "traits");
				}
				metaData.AddRange(Encoding.UTF8.GetBytes(value));
			}
			return metaData.Count - count;
		}

		private static int HexDigit(char c)
		{
			if ('0' <= c && c <= '9')
			{
				return (int)(c - '0');
			}
			if ('a' <= c)
			{
				c -= ' ';
			}
			if ('A' <= c && c <= 'F')
			{
				return (int)(c - 'A' + '\n');
			}
			throw new ArgumentException(Environment.GetResourceString("BadHexDigit", new object[]
			{
				c
			}), "traits");
		}

		private NameInfo UpdateDescriptor(string name, TraceLoggingEventTypes eventInfo, ref EventSourceOptions options, out EventDescriptor descriptor)
		{
			NameInfo nameInfo = null;
			int traceloggingId = 0;
			byte level = ((options.valuesSet & 4) != 0) ? options.level : eventInfo.level;
			byte opcode = ((options.valuesSet & 8) != 0) ? options.opcode : eventInfo.opcode;
			EventTags tags = ((options.valuesSet & 2) != 0) ? options.tags : eventInfo.Tags;
			EventKeywords keywords = ((options.valuesSet & 1) != 0) ? options.keywords : eventInfo.keywords;
			if (this.IsEnabled((EventLevel)level, keywords))
			{
				nameInfo = eventInfo.GetNameInfo(name ?? eventInfo.Name, tags);
				traceloggingId = nameInfo.identity;
			}
			descriptor = new EventDescriptor(traceloggingId, level, opcode, (long)keywords);
			return nameInfo;
		}

		private string m_name;

		internal int m_id;

		private Guid m_guid;

		internal volatile EventSource.EventMetadata[] m_eventData;

		private volatile byte[] m_rawManifest;

		private EventHandler<EventCommandEventArgs> m_eventCommandExecuted;

		private EventSourceSettings m_config;

		private bool m_eventSourceEnabled;

		internal EventLevel m_level;

		internal EventKeywords m_matchAnyKeyword;

		internal volatile EventDispatcher m_Dispatchers;

		private volatile EventSource.OverideEventProvider m_provider;

		private bool m_completelyInited;

		private Exception m_constructionException;

		private byte m_outOfBandMessageCount;

		private EventCommandEventArgs m_deferredCommands;

		private string[] m_traits;

		internal static uint s_currentPid;

		[ThreadStatic]
		private static byte m_EventSourceExceptionRecurenceCount = 0;

		internal volatile ulong[] m_channelData;

		private SessionMask m_curLiveSessions;

		private EtwSession[] m_etwSessionIdMap;

		private List<EtwSession> m_legacySessions;

		internal long m_keywordTriggers;

		internal SessionMask m_activityFilteringForETWEnabled;

		internal static Action<Guid> s_activityDying;

		private ActivityTracker m_activityTracker;

		internal const string s_ActivityStartSuffix = "Start";

		internal const string s_ActivityStopSuffix = "Stop";

		private static readonly byte[] namespaceBytes = new byte[]
		{
			72,
			44,
			45,
			178,
			195,
			144,
			71,
			200,
			135,
			248,
			26,
			21,
			191,
			193,
			48,
			251
		};

		private static readonly Guid AspNetEventSourceGuid = new Guid("ee799f41-cfa5-550b-bf2c-344747c1c668");

		private byte[] providerMetadata;

		[__DynamicallyInvokable]
		protected internal struct EventData
		{
			[__DynamicallyInvokable]
			public unsafe IntPtr DataPointer
			{
				[SecuritySafeCritical]
				get
				{
					return (IntPtr)this.m_Ptr;
				}
				set
				{
					this.m_Ptr = (void*)value;
				}
			}

			[__DynamicallyInvokable]
			public int Size
			{
				[__DynamicallyInvokable]
				get
				{
					return this.m_Size;
				}
				[__DynamicallyInvokable]
				set
				{
					this.m_Size = value;
				}
			}

			[SecurityCritical]
			internal unsafe void SetMetadata(byte* pointer, int size, int reserved)
			{
				this.m_Ptr = pointer;
				this.m_Size = size;
				this.m_Reserved = reserved;
			}

			internal ulong m_Ptr;

			internal int m_Size;

			internal int m_Reserved;
		}

		private struct Sha1ForNonSecretPurposes
		{
			public void Start()
			{
				if (this.w == null)
				{
					this.w = new uint[85];
				}
				this.length = 0L;
				this.pos = 0;
				this.w[80] = 1732584193U;
				this.w[81] = 4023233417U;
				this.w[82] = 2562383102U;
				this.w[83] = 271733878U;
				this.w[84] = 3285377520U;
			}

			public void Append(byte input)
			{
				this.w[this.pos / 4] = (this.w[this.pos / 4] << 8 | (uint)input);
				int num = 64;
				int num2 = this.pos + 1;
				this.pos = num2;
				if (num == num2)
				{
					this.Drain();
				}
			}

			public void Append(byte[] input)
			{
				foreach (byte input2 in input)
				{
					this.Append(input2);
				}
			}

			public void Finish(byte[] output)
			{
				long num = this.length + (long)(8 * this.pos);
				this.Append(128);
				while (this.pos != 56)
				{
					this.Append(0);
				}
				this.Append((byte)(num >> 56));
				this.Append((byte)(num >> 48));
				this.Append((byte)(num >> 40));
				this.Append((byte)(num >> 32));
				this.Append((byte)(num >> 24));
				this.Append((byte)(num >> 16));
				this.Append((byte)(num >> 8));
				this.Append((byte)num);
				int num2 = (output.Length < 20) ? output.Length : 20;
				for (int num3 = 0; num3 != num2; num3++)
				{
					uint num4 = this.w[80 + num3 / 4];
					output[num3] = (byte)(num4 >> 24);
					this.w[80 + num3 / 4] = num4 << 8;
				}
			}

			private void Drain()
			{
				for (int num = 16; num != 80; num++)
				{
					this.w[num] = EventSource.Sha1ForNonSecretPurposes.Rol1(this.w[num - 3] ^ this.w[num - 8] ^ this.w[num - 14] ^ this.w[num - 16]);
				}
				uint num2 = this.w[80];
				uint num3 = this.w[81];
				uint num4 = this.w[82];
				uint num5 = this.w[83];
				uint num6 = this.w[84];
				for (int num7 = 0; num7 != 20; num7++)
				{
					uint num8 = (num3 & num4) | (~num3 & num5);
					uint num9 = EventSource.Sha1ForNonSecretPurposes.Rol5(num2) + num8 + num6 + 1518500249U + this.w[num7];
					num6 = num5;
					num5 = num4;
					num4 = EventSource.Sha1ForNonSecretPurposes.Rol30(num3);
					num3 = num2;
					num2 = num9;
				}
				for (int num10 = 20; num10 != 40; num10++)
				{
					uint num11 = num3 ^ num4 ^ num5;
					uint num12 = EventSource.Sha1ForNonSecretPurposes.Rol5(num2) + num11 + num6 + 1859775393U + this.w[num10];
					num6 = num5;
					num5 = num4;
					num4 = EventSource.Sha1ForNonSecretPurposes.Rol30(num3);
					num3 = num2;
					num2 = num12;
				}
				for (int num13 = 40; num13 != 60; num13++)
				{
					uint num14 = (num3 & num4) | (num3 & num5) | (num4 & num5);
					uint num15 = EventSource.Sha1ForNonSecretPurposes.Rol5(num2) + num14 + num6 + 2400959708U + this.w[num13];
					num6 = num5;
					num5 = num4;
					num4 = EventSource.Sha1ForNonSecretPurposes.Rol30(num3);
					num3 = num2;
					num2 = num15;
				}
				for (int num16 = 60; num16 != 80; num16++)
				{
					uint num17 = num3 ^ num4 ^ num5;
					uint num18 = EventSource.Sha1ForNonSecretPurposes.Rol5(num2) + num17 + num6 + 3395469782U + this.w[num16];
					num6 = num5;
					num5 = num4;
					num4 = EventSource.Sha1ForNonSecretPurposes.Rol30(num3);
					num3 = num2;
					num2 = num18;
				}
				this.w[80] += num2;
				this.w[81] += num3;
				this.w[82] += num4;
				this.w[83] += num5;
				this.w[84] += num6;
				this.length += 512L;
				this.pos = 0;
			}

			private static uint Rol1(uint input)
			{
				return input << 1 | input >> 31;
			}

			private static uint Rol5(uint input)
			{
				return input << 5 | input >> 27;
			}

			private static uint Rol30(uint input)
			{
				return input << 30 | input >> 2;
			}

			private long length;

			private uint[] w;

			private int pos;
		}

		private class OverideEventProvider : EventProvider
		{
			public OverideEventProvider(EventSource eventSource)
			{
				this.m_eventSource = eventSource;
			}

			protected override void OnControllerCommand(ControllerCommand command, IDictionary<string, string> arguments, int perEventSourceSessionId, int etwSessionId)
			{
				EventListener listener = null;
				this.m_eventSource.SendCommand(listener, perEventSourceSessionId, etwSessionId, (EventCommand)command, base.IsEnabled(), base.Level, base.MatchAnyKeyword, arguments);
			}

			private EventSource m_eventSource;
		}

		internal struct EventMetadata
		{
			public EventDescriptor Descriptor;

			public EventTags Tags;

			public bool EnabledForAnyListener;

			public bool EnabledForETW;

			public bool HasRelatedActivityID;

			public byte TriggersActivityTracking;

			public string Name;

			public string Message;

			public ParameterInfo[] Parameters;

			public TraceLoggingEventTypes TraceLoggingEventTypes;

			public EventActivityOptions ActivityOptions;
		}
	}
}
