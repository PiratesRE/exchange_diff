using System;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Serializable]
	public sealed class SupervisionListEntryId : ObjectId
	{
		public SupervisionListEntryId(SupervisionListEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			this.id = string.Concat(new string[]
			{
				entry.EntryName,
				",",
				entry.Tag,
				",",
				entry.RecipientType.ToString()
			});
		}

		public override string ToString()
		{
			return this.id;
		}

		public override byte[] GetBytes()
		{
			return Encoding.Unicode.GetBytes(this.id);
		}

		private readonly string id;
	}
}
