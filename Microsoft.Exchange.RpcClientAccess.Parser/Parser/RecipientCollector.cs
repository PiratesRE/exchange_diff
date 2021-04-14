using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RecipientCollector : BaseObject
	{
		internal RecipientCollector(int maxSize, PropertyTag[] extraPropertyTags, RecipientSerializationFlags recipientSerializationFlags)
		{
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				disposeGuard.Add<RecipientCollector>(this);
				if (maxSize < 0)
				{
					throw new BufferTooSmallException();
				}
				this.maxSize = maxSize;
				this.recipientRows = new List<RecipientRow>();
				this.extraPropertyTags = extraPropertyTags;
				this.recipientSerializationFlags = recipientSerializationFlags;
				disposeGuard.Success();
			}
		}

		internal RecipientCollector(int maxSize, PropertyTag[] extraPropertyTags, RecipientSerializationFlags recipientSerializationFlags, RecipientRow[] recipientRows) : this(maxSize, extraPropertyTags, recipientSerializationFlags)
		{
			this.recipientRows = new List<RecipientRow>(recipientRows);
		}

		internal PropertyTag[] ExtraPropertyTags
		{
			get
			{
				return this.extraPropertyTags;
			}
		}

		internal RecipientRow[] RecipientRows
		{
			get
			{
				return this.recipientRows.ToArray();
			}
		}

		internal RecipientSerializationFlags RecipientSerializationFlags
		{
			get
			{
				return this.recipientSerializationFlags;
			}
		}

		internal bool IsEmpty
		{
			get
			{
				return this.recipientRows.Count == 0;
			}
		}

		public bool TryAddRecipientRow(RecipientRow row)
		{
			if (row.ExtraPropertyValues.Length > this.extraPropertyTags.Length)
			{
				string message = string.Format("Expecting at most {0} extra properties. Instead found {1}.", this.extraPropertyTags.Length, row.ExtraPropertyValues.Length);
				throw new ArgumentException(message, "row");
			}
			for (int i = 0; i < row.ExtraPropertyValues.Length; i++)
			{
				PropertyTag propertyTag = this.extraPropertyTags[i];
				PropertyTag propertyTag2 = row.ExtraPropertyValues[i].PropertyTag;
				if (propertyTag2 != propertyTag && propertyTag2.PropertyType != PropertyType.Error)
				{
					string message2 = string.Format("Expecting to find {0} for extra property {1}. Instead found {2}.", propertyTag, i, propertyTag2);
					throw new ArgumentException(message2, "row");
				}
			}
			if (this.recipientRows.Count == 255)
			{
				return false;
			}
			row.Serialize(this.writer, this.extraPropertyTags, this.recipientSerializationFlags);
			if (this.writer.Position > (long)this.maxSize)
			{
				return false;
			}
			this.recipientRows.Add(row);
			return true;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RecipientCollector>(this);
		}

		protected override void InternalDispose()
		{
			if (this.writer != null)
			{
				this.writer.Dispose();
				this.writer = null;
			}
			base.InternalDispose();
		}

		private readonly int maxSize;

		private PropertyTag[] extraPropertyTags;

		private RecipientSerializationFlags recipientSerializationFlags;

		private List<RecipientRow> recipientRows;

		private CountWriter writer = new CountWriter();
	}
}
