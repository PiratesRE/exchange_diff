using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RecipientId : IEquatable<RecipientId>
	{
		internal RecipientId(byte[] id)
		{
			this.id = id;
		}

		public override bool Equals(object obj)
		{
			RecipientId recipId = obj as RecipientId;
			return this.Equals(recipId);
		}

		public bool Equals(RecipientId recipId)
		{
			return recipId != null && ArrayComparer<byte>.Comparer.Equals(this.id, recipId.id);
		}

		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		public string GetBase64()
		{
			return Convert.ToBase64String(this.id);
		}

		public byte[] GetBytes()
		{
			return (byte[])this.id.Clone();
		}

		private byte[] id;
	}
}
