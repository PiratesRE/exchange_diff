using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TombstoneRecord
	{
		public ExDateTime StartTime { get; set; }

		public ExDateTime EndTime { get; set; }

		public byte[] GlobalObjectId { get; set; }

		public byte[] UserName { get; set; }

		public bool TryGetBytes(out byte[] buffer)
		{
			buffer = null;
			bool result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write((int)(this.StartTime.LocalTime.ToFileTimeUtc() / 600000000L));
					binaryWriter.Write((int)(this.EndTime.LocalTime.ToFileTimeUtc() / 600000000L));
					if (this.GlobalObjectId == null)
					{
						ExTraceGlobals.MeetingMessageTracer.TraceError((long)this.GetHashCode(), "Tombstone record GlobalObjectId is null");
						result = false;
					}
					else
					{
						binaryWriter.Write(this.GlobalObjectId.Length);
						binaryWriter.Write(this.GlobalObjectId);
						if (this.UserName == null || this.UserName.Length <= 0)
						{
							ExTraceGlobals.MeetingMessageTracer.TraceError((long)this.GetHashCode(), "Tombstone record UserName is null");
							result = false;
						}
						else
						{
							binaryWriter.Write((short)this.UserName.Length);
							binaryWriter.Write(this.UserName);
							buffer = memoryStream.ToArray();
							result = true;
						}
					}
				}
			}
			return result;
		}
	}
}
