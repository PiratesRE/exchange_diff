using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class ExtendedPropertyColumn : Column
	{
		protected ExtendedPropertyColumn(string name, Type type, bool nullable, Visibility visibility, int size, int maxLength, Table table, StorePropTag propTag) : base(name, type, nullable, visibility, maxLength, size, table)
		{
			this.propTag = propTag;
		}

		protected ExtendedPropertyColumn(string name, Type type, Visibility visibility, int size, int maxLength, Table table, StorePropTag propTag) : this(name, type, true, visibility, size, maxLength, table, propTag)
		{
		}

		public StorePropTag StorePropTag
		{
			get
			{
				return this.propTag;
			}
		}

		public override void GetNameOrIdForSerialization(out string columnName, out uint columnId)
		{
			columnName = string.Empty;
			columnId = this.StorePropTag.PropTag;
		}

		private readonly StorePropTag propTag;
	}
}
