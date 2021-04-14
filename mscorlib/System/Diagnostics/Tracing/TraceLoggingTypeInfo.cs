using System;

namespace System.Diagnostics.Tracing
{
	internal abstract class TraceLoggingTypeInfo
	{
		internal TraceLoggingTypeInfo(Type dataType)
		{
			if (dataType == null)
			{
				throw new ArgumentNullException("dataType");
			}
			this.name = dataType.Name;
			this.dataType = dataType;
		}

		internal TraceLoggingTypeInfo(Type dataType, string name, EventLevel level, EventOpcode opcode, EventKeywords keywords, EventTags tags)
		{
			if (dataType == null)
			{
				throw new ArgumentNullException("dataType");
			}
			if (name == null)
			{
				throw new ArgumentNullException("eventName");
			}
			Statics.CheckName(name);
			this.name = name;
			this.keywords = keywords;
			this.level = level;
			this.opcode = opcode;
			this.tags = tags;
			this.dataType = dataType;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public EventLevel Level
		{
			get
			{
				return this.level;
			}
		}

		public EventOpcode Opcode
		{
			get
			{
				return this.opcode;
			}
		}

		public EventKeywords Keywords
		{
			get
			{
				return this.keywords;
			}
		}

		public EventTags Tags
		{
			get
			{
				return this.tags;
			}
		}

		internal Type DataType
		{
			get
			{
				return this.dataType;
			}
		}

		public abstract void WriteMetadata(TraceLoggingMetadataCollector collector, string name, EventFieldFormat format);

		public abstract void WriteObjectData(TraceLoggingDataCollector collector, object value);

		public virtual object GetData(object value)
		{
			return value;
		}

		private readonly string name;

		private readonly EventKeywords keywords;

		private readonly EventLevel level = (EventLevel)(-1);

		private readonly EventOpcode opcode = (EventOpcode)(-1);

		private readonly EventTags tags;

		private readonly Type dataType;
	}
}
