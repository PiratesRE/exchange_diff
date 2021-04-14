using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.TenantMonitoring
{
	[Serializable]
	public sealed class NotificationIdentity : ObjectId, IEquatable<NotificationIdentity>
	{
		public NotificationIdentity(byte[] id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			this.id = new Guid(id);
		}

		public NotificationIdentity() : this(Guid.NewGuid())
		{
		}

		private NotificationIdentity(Guid id)
		{
			this.id = id;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as NotificationIdentity);
		}

		public bool Equals(NotificationIdentity other)
		{
			return other != null && this.id.Equals(other.id);
		}

		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		public override byte[] GetBytes()
		{
			return this.id.ToByteArray();
		}

		public override string ToString()
		{
			return this.id.ToString("D");
		}

		internal string EventSource
		{
			get
			{
				return this.eventSource;
			}
			set
			{
				this.eventSource = value;
			}
		}

		private readonly Guid id;

		[NonSerialized]
		private string eventSource;
	}
}
