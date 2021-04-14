using System;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[Serializable]
	internal class CacheCookieRow : IEquatable<CacheCookieRow>
	{
		public CacheCookieRow(int copyIndex, DateTime changedTS, Guid changedEntityId)
		{
			this.CopyIndex = copyIndex;
			this.LastChangedDateTime = changedTS;
			this.LastChangedEntityId = changedEntityId;
		}

		public int CopyIndex { get; set; }

		public DateTime LastChangedDateTime { get; set; }

		public Guid LastChangedEntityId { get; set; }

		public bool Equals(CacheCookieRow c2)
		{
			return c2 != null && (this.CopyIndex == c2.CopyIndex && this.LastChangedDateTime == c2.LastChangedDateTime && this.LastChangedEntityId == c2.LastChangedEntityId);
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is CacheCookieRow && this.Equals(obj as CacheCookieRow);
		}

		public override int GetHashCode()
		{
			string text = string.Format("{0}:{1}:{2}", this.CopyIndex, this.LastChangedDateTime, this.LastChangedEntityId);
			return text.GetHashCode();
		}
	}
}
