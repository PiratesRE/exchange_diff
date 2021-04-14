using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiErrorNotification : MapiNotification
	{
		public byte[] EntryId
		{
			get
			{
				return this.entryId;
			}
		}

		public int SCode
		{
			get
			{
				return this.scode;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		public string Component
		{
			get
			{
				return this.component;
			}
		}

		public int LowLevelError
		{
			get
			{
				return this.lowLevelError;
			}
		}

		public int Context
		{
			get
			{
				return this.context;
			}
		}

		internal unsafe MapiErrorNotification(NOTIFICATION* notification) : base(notification)
		{
			if (notification->info.err.cbEntryID > 0)
			{
				this.entryId = new byte[notification->info.err.cbEntryID];
				Marshal.Copy(notification->info.err.lpEntryID, this.entryId, 0, this.entryId.Length);
			}
			this.scode = notification->info.err.scode;
			bool unicodeEncoded = (notification->info.err.ulFlags & int.MinValue) != 0;
			this.error = notification->info.err.lpMAPIError->ErrorText(unicodeEncoded);
			this.component = notification->info.err.lpMAPIError->Component(unicodeEncoded);
			this.lowLevelError = notification->info.err.lpMAPIError->ulLowLevelError;
			this.context = notification->info.err.lpMAPIError->ulContext;
		}

		private readonly byte[] entryId;

		private readonly int scode;

		private readonly string error;

		private readonly string component;

		private readonly int lowLevelError;

		private readonly int context;
	}
}
