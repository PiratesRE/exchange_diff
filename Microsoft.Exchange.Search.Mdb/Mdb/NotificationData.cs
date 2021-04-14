using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Search.Mdb
{
	internal sealed class NotificationData : IEquatable<NotificationData>
	{
		internal NotificationData(MapiEvent mapiEvent)
		{
			this.MapiEvent = mapiEvent;
		}

		internal NotificationData(MapiEvent mapiEvent, bool interesting, DocumentOperation operation, MdbItemIdentity identity)
		{
			this.MapiEvent = mapiEvent;
			this.Type = (interesting ? NotificationType.Insert : NotificationType.Uninteresting);
			this.Operation = operation;
			this.Identity = identity;
		}

		internal MapiEvent MapiEvent { get; private set; }

		internal DocumentOperation Operation { get; set; }

		internal MdbItemIdentity Identity { get; set; }

		internal NotificationType Type { get; set; }

		internal List<NotificationData> MergedEvents { get; set; }

		internal bool IsMoveDestination
		{
			get
			{
				return (this.MapiEvent.ExtendedEventFlags & MapiExtendedEventFlags.MoveDestination) != MapiExtendedEventFlags.None;
			}
		}

		public override string ToString()
		{
			return string.Format("Notification (EventId={0}, Type={1}, Operation={2}, DocumentId={3}, Identity={4})", new object[]
			{
				this.MapiEvent.EventCounter,
				this.Type,
				this.Operation,
				this.MapiEvent.DocumentId,
				this.Identity
			});
		}

		public string ToMergeDebuggingString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Notification (MapiEvent={0}, Type={1}, Operation={2}, Identity={3}, Merged Events: [", new object[]
			{
				this.MapiEvent,
				this.Type,
				this.Operation,
				this.Identity
			});
			lock (this.lockObject)
			{
				if (this.MergedEvents != null && this.MergedEvents.Count > 0)
				{
					using (List<NotificationData>.Enumerator enumerator = this.MergedEvents.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							NotificationData notificationData = enumerator.Current;
							stringBuilder.AppendFormat("{0} ", notificationData.ToString());
						}
						goto IL_BE;
					}
				}
				stringBuilder.Append("None");
				IL_BE:;
			}
			stringBuilder.Append("])");
			return stringBuilder.ToString();
		}

		public bool Equals(NotificationData other)
		{
			return other != null && this.Identity.Equals(other.Identity);
		}

		public override int GetHashCode()
		{
			return this.Identity.GetHashCode();
		}

		public void TrackMergeWith(NotificationData other)
		{
			if (this.MergedEvents == null)
			{
				if (other.MergedEvents != null)
				{
					this.MergedEvents = new List<NotificationData>(other.MergedEvents.Capacity + 1);
					this.MergedEvents.AddRange(other.MergedEvents);
				}
				else
				{
					this.MergedEvents = new List<NotificationData>(1);
				}
			}
			this.MergedEvents.Add(other);
		}

		private readonly object lockObject = new object();
	}
}
