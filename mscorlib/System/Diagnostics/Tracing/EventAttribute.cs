using System;

namespace System.Diagnostics.Tracing
{
	[AttributeUsage(AttributeTargets.Method)]
	[__DynamicallyInvokable]
	public sealed class EventAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public EventAttribute(int eventId)
		{
			this.EventId = eventId;
			this.Level = EventLevel.Informational;
			this.m_opcodeSet = false;
		}

		[__DynamicallyInvokable]
		public int EventId { [__DynamicallyInvokable] get; private set; }

		[__DynamicallyInvokable]
		public EventLevel Level { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		[__DynamicallyInvokable]
		public EventKeywords Keywords { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		[__DynamicallyInvokable]
		public EventOpcode Opcode
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_opcode;
			}
			[__DynamicallyInvokable]
			set
			{
				this.m_opcode = value;
				this.m_opcodeSet = true;
			}
		}

		internal bool IsOpcodeSet
		{
			get
			{
				return this.m_opcodeSet;
			}
		}

		[__DynamicallyInvokable]
		public EventTask Task { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		[__DynamicallyInvokable]
		public EventChannel Channel { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		[__DynamicallyInvokable]
		public byte Version { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		[__DynamicallyInvokable]
		public string Message { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		[__DynamicallyInvokable]
		public EventTags Tags { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		[__DynamicallyInvokable]
		public EventActivityOptions ActivityOptions { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		private EventOpcode m_opcode;

		private bool m_opcodeSet;
	}
}
