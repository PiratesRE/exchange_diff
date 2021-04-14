using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiStatusObjectNotification : MapiNotification
	{
		public byte[] EntryId
		{
			get
			{
				return this.entryId;
			}
		}

		public PropValue[] PropValues
		{
			get
			{
				return this.propValues;
			}
		}

		internal unsafe MapiStatusObjectNotification(NOTIFICATION* notification) : base(notification)
		{
			if (notification->info.statobj.cbEntryID > 0)
			{
				this.entryId = new byte[notification->info.statobj.cbEntryID];
				Marshal.Copy(notification->info.statobj.lpEntryID, this.entryId, 0, this.entryId.Length);
			}
			this.propValues = new PropValue[notification->info.statobj.cValues];
			for (int i = 0; i < this.propValues.Length; i++)
			{
				this.propValues[i] = new PropValue(notification->info.statobj.lpPropVals + i);
			}
		}

		private readonly byte[] entryId;

		private readonly PropValue[] propValues;
	}
}
