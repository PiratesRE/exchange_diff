using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public sealed class PseudoIndexDefinitionTable
	{
		internal PseudoIndexDefinitionTable()
		{
			this.physicalIndexNumber = Factory.CreatePhysicalColumn("PhysicalIndexNumber", "PhysicalIndexNumber", typeof(int), false, true, false, false, false, Visibility.Public, 0, 4, 4);
			this.columnBlob = Factory.CreatePhysicalColumn("ColumnBlob", "ColumnBlob", typeof(byte[]), false, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			string name = "PseudoIndexDefinitionPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[1];
			this.pseudoIndexDefinitionPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.PhysicalIndexNumber
			});
			Index[] indexes = new Index[]
			{
				this.PseudoIndexDefinitionPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.PhysicalIndexNumber,
				this.ColumnBlob,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("PseudoIndexDefinition", TableClass.PseudoIndexDefinition, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public PhysicalColumn PhysicalIndexNumber
		{
			get
			{
				return this.physicalIndexNumber;
			}
		}

		public PhysicalColumn ColumnBlob
		{
			get
			{
				return this.columnBlob;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index PseudoIndexDefinitionPK
		{
			get
			{
				return this.pseudoIndexDefinitionPK;
			}
		}

		internal void PostMountInitialize(ComponentVersion databaseVersion)
		{
			PhysicalColumn physicalColumn = this.physicalIndexNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.physicalIndexNumber = null;
			}
			physicalColumn = this.columnBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.columnBlob = null;
			}
			physicalColumn = this.extensionBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.extensionBlob = null;
			}
			for (int i = this.Table.Columns.Count - 1; i >= 0; i--)
			{
				this.Table.Columns[i].Index = i;
			}
			Index index = this.pseudoIndexDefinitionPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.pseudoIndexDefinitionPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string PhysicalIndexNumberName = "PhysicalIndexNumber";

		public const string ColumnBlobName = "ColumnBlob";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "PseudoIndexDefinition";

		private PhysicalColumn physicalIndexNumber;

		private PhysicalColumn columnBlob;

		private PhysicalColumn extensionBlob;

		private Index pseudoIndexDefinitionPK;

		private Table table;
	}
}
