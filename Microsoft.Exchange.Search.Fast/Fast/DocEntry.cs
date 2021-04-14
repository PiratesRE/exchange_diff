using System;
using Microsoft.Ceres.InteractionEngine.Services.Exchange;
using Microsoft.Ceres.SearchCore.Admin.Model;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal class DocEntry : IDocEntry, IEquatable<IDocEntry>
	{
		public DocEntry()
		{
		}

		public DocEntry(ISearchResultItem item)
		{
			this.Parse(item);
		}

		public string RawItemId { get; set; }

		public MdbItemIdentity ItemId
		{
			get
			{
				if (this.itemId == null)
				{
					this.itemId = MdbItemIdentity.Parse(this.RawItemId);
				}
				return this.itemId;
			}
		}

		public int DocumentId
		{
			get
			{
				return Microsoft.Exchange.Search.OperatorSchema.IndexId.GetDocumentId(this.IndexId);
			}
		}

		public string EntryId
		{
			get
			{
				return this.ItemId.ItemId.ToString();
			}
		}

		public long IndexId { get; private set; }

		public Guid MailboxGuid { get; set; }

		internal static IndexSystemField[] Schema
		{
			get
			{
				return DocEntry.schema;
			}
		}

		public override string ToString()
		{
			if (this.RawItemId != null)
			{
				return string.Format("ItemId: {0}", this.RawItemId);
			}
			return string.Format("ItemId: null, MailboxGuid: {0}, IndexId: {1}", this.MailboxGuid, this.IndexId);
		}

		public override bool Equals(object other)
		{
			DocEntry docEntry = other as DocEntry;
			return docEntry != null && (object.ReferenceEquals(this, docEntry) || this.IndexId == docEntry.IndexId);
		}

		public override int GetHashCode()
		{
			return this.IndexId.GetHashCode();
		}

		public bool Equals(IDocEntry other)
		{
			return other != null && this.IndexId.Equals(other.IndexId);
		}

		protected virtual void SetProp(string name, object value)
		{
		}

		private void Parse(ISearchResultItem item)
		{
			foreach (IFieldHolder fieldHolder in item.Fields)
			{
				string name;
				if (fieldHolder.Value != null && (name = fieldHolder.Name) != null)
				{
					if (!(name == "DocId"))
					{
						if (!(name == "Rank"))
						{
							if (name == "Other")
							{
								ISearchResultItem searchResultItem = (ISearchResultItem)fieldHolder.Value;
								foreach (IFieldHolder fieldHolder2 in searchResultItem.Fields)
								{
									string name2 = fieldHolder2.Name;
									object value = fieldHolder2.Value;
									if (!string.IsNullOrEmpty(name2) && value != null)
									{
										if (name2 == FastIndexSystemSchema.ItemId.Name)
										{
											this.RawItemId = (string)value;
										}
										else if (name2 == FastIndexSystemSchema.MailboxGuid.Name)
										{
											this.MailboxGuid = new Guid((string)value);
										}
										else
										{
											this.SetProp(name2, value);
										}
									}
								}
							}
						}
					}
					else
					{
						this.IndexId = (long)fieldHolder.Value;
					}
				}
			}
		}

		internal const string DocIdField = "DocId";

		internal const string RankField = "Rank";

		internal const string OtherField = "Other";

		private static readonly IndexSystemField[] schema = new IndexSystemField[]
		{
			FastIndexSystemSchema.ItemId.Definition,
			FastIndexSystemSchema.MailboxGuid.Definition
		};

		private MdbItemIdentity itemId;
	}
}
