using System;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	public class LifetimeEntry
	{
		public LifetimeEntry(IntPtr handle, int offset) : this(LifetimeEntry.GetInternalLifetimeEntry(handle, offset), handle)
		{
			this.offset = offset;
		}

		[CLSCompliant(false)]
		public unsafe LifetimeEntry(InternalLifetimeEntry* internalLifetimeEntry, IntPtr handle)
		{
			if (null == internalLifetimeEntry)
			{
				throw new ArgumentNullException("internalLifetimeEntry");
			}
			this.internalLifetimeEntry = internalLifetimeEntry;
		}

		public unsafe int Type
		{
			get
			{
				return this.internalLifetimeEntry->LifetimeType;
			}
		}

		public unsafe int ProcessId
		{
			get
			{
				return this.internalLifetimeEntry->ProcessId;
			}
			set
			{
				this.internalLifetimeEntry->ProcessId = value;
			}
		}

		public unsafe long StartupTime
		{
			get
			{
				return this.internalLifetimeEntry->StartupTime;
			}
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Type: " + this.Type);
			stringBuilder.Append(" ProcessId: " + this.ProcessId);
			stringBuilder.Append(" StartupTime: " + this.StartupTime);
			return stringBuilder.ToString();
		}

		private unsafe static InternalLifetimeEntry* GetInternalLifetimeEntry(IntPtr handle, int offset)
		{
			return (long)handle / (long)sizeof(InternalLifetimeEntry) + offset;
		}

		private unsafe InternalLifetimeEntry* internalLifetimeEntry;

		private int offset;
	}
}
