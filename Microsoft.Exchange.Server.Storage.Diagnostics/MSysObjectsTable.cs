using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public sealed class MSysObjectsTable
	{
		internal MSysObjectsTable()
		{
			this.objidTable = Factory.CreatePhysicalColumn("ObjidTable", "ObjidTable", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.type = Factory.CreatePhysicalColumn("Type", "Type", typeof(short), false, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.id = Factory.CreatePhysicalColumn("Id", "Id", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.name = Factory.CreatePhysicalColumn("Name", "Name", typeof(byte[]), false, false, false, false, false, Visibility.Public, 128, 0, 128);
			this.coltypOrPgnoFDP = Factory.CreatePhysicalColumn("ColtypOrPgnoFDP", "ColtypOrPgnoFDP", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.spaceUsage = Factory.CreatePhysicalColumn("SpaceUsage", "SpaceUsage", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.flags = Factory.CreatePhysicalColumn("Flags", "Flags", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.pagesOrLocale = Factory.CreatePhysicalColumn("PagesOrLocale", "PagesOrLocale", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.recordOffset = Factory.CreatePhysicalColumn("RecordOffset", "RecordOffset", typeof(short), true, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.keyMost = Factory.CreatePhysicalColumn("KeyMost", "KeyMost", typeof(short), true, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.stats = Factory.CreatePhysicalColumn("Stats", "Stats", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1, 0, 1);
			this.rootFlag = Factory.CreatePhysicalColumn("RootFlag", "RootFlag", typeof(bool), true, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.templateTable = Factory.CreatePhysicalColumn("TemplateTable", "TemplateTable", typeof(byte[]), true, false, false, false, false, Visibility.Public, 128, 0, 128);
			this.defaultValue = Factory.CreatePhysicalColumn("DefaultValue", "DefaultValue", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1, 0, 1);
			this.keyFldIDs = Factory.CreatePhysicalColumn("KeyFldIDs", "KeyFldIDs", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1, 0, 1);
			this.varSegMac = Factory.CreatePhysicalColumn("VarSegMac", "VarSegMac", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1, 0, 1);
			this.conditionalColumns = Factory.CreatePhysicalColumn("ConditionalColumns", "ConditionalColumns", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1, 0, 1);
			this.lCMapFlags = Factory.CreatePhysicalColumn("LCMapFlags", "LCMapFlags", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.tupleLimits = Factory.CreatePhysicalColumn("TupleLimits", "TupleLimits", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1, 0, 1);
			this.version = Factory.CreatePhysicalColumn("Version", "Version", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1, 0, 1);
			this.sortID = Factory.CreatePhysicalColumn("SortID", "SortID", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1, 0, 1);
			this.callbackData = Factory.CreatePhysicalColumn("CallbackData", "CallbackData", typeof(byte[]), true, false, false, false, false, Visibility.Public, 128, 0, 128);
			this.callbackDependencies = Factory.CreatePhysicalColumn("CallbackDependencies", "CallbackDependencies", typeof(byte[]), true, false, false, false, false, Visibility.Public, 128, 0, 128);
			this.separateLV = Factory.CreatePhysicalColumn("SeparateLV", "SeparateLV", typeof(byte[]), true, false, false, false, false, Visibility.Public, 128, 0, 128);
			this.spaceHints = Factory.CreatePhysicalColumn("SpaceHints", "SpaceHints", typeof(byte[]), true, false, false, false, false, Visibility.Public, 128, 0, 128);
			this.spaceDeferredLVHints = Factory.CreatePhysicalColumn("SpaceDeferredLVHints", "SpaceDeferredLVHints", typeof(byte[]), true, false, false, false, false, Visibility.Public, 128, 0, 128);
			this.localeName = Factory.CreatePhysicalColumn("LocaleName", "LocaleName", typeof(string), true, false, false, false, false, Visibility.Public, 1, 0, 1);
			string text = "Id";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[3];
			this.idIndex = new Index(text, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.ObjidTable,
				this.Type,
				this.Id
			});
			string text2 = "Name";
			bool primaryKey2 = false;
			bool unique2 = true;
			bool schemaExtension2 = false;
			bool[] conditional2 = new bool[3];
			this.nameIndex = new Index(text2, primaryKey2, unique2, schemaExtension2, conditional2, new bool[]
			{
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.ObjidTable,
				this.Type,
				this.Name
			});
			string text3 = "RootObjects";
			bool primaryKey3 = false;
			bool unique3 = true;
			bool schemaExtension3 = false;
			bool[] conditional3 = new bool[2];
			this.rootObjectsIndex = new Index(text3, primaryKey3, unique3, schemaExtension3, conditional3, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.RootFlag,
				this.Name
			});
			Index[] indexes = new Index[]
			{
				this.IdIndex,
				this.NameIndex,
				this.RootObjectsIndex
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.ObjidTable,
				this.Type,
				this.Id,
				this.Name,
				this.ColtypOrPgnoFDP,
				this.SpaceUsage,
				this.Flags,
				this.PagesOrLocale,
				this.RecordOffset,
				this.KeyMost,
				this.Stats,
				this.RootFlag,
				this.TemplateTable,
				this.DefaultValue,
				this.KeyFldIDs,
				this.VarSegMac,
				this.ConditionalColumns,
				this.LCMapFlags,
				this.TupleLimits,
				this.Version,
				this.SortID,
				this.CallbackData,
				this.CallbackDependencies,
				this.SeparateLV,
				this.SpaceHints,
				this.SpaceDeferredLVHints,
				this.LocaleName
			};
			this.table = Factory.CreateTable("MSysObjects", TableClass.Unknown, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, true, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public PhysicalColumn ObjidTable
		{
			get
			{
				return this.objidTable;
			}
		}

		public PhysicalColumn Type
		{
			get
			{
				return this.type;
			}
		}

		public PhysicalColumn Id
		{
			get
			{
				return this.id;
			}
		}

		public PhysicalColumn Name
		{
			get
			{
				return this.name;
			}
		}

		public PhysicalColumn ColtypOrPgnoFDP
		{
			get
			{
				return this.coltypOrPgnoFDP;
			}
		}

		public PhysicalColumn SpaceUsage
		{
			get
			{
				return this.spaceUsage;
			}
		}

		public PhysicalColumn Flags
		{
			get
			{
				return this.flags;
			}
		}

		public PhysicalColumn PagesOrLocale
		{
			get
			{
				return this.pagesOrLocale;
			}
		}

		public PhysicalColumn RecordOffset
		{
			get
			{
				return this.recordOffset;
			}
		}

		public PhysicalColumn KeyMost
		{
			get
			{
				return this.keyMost;
			}
		}

		public PhysicalColumn Stats
		{
			get
			{
				return this.stats;
			}
		}

		public PhysicalColumn RootFlag
		{
			get
			{
				return this.rootFlag;
			}
		}

		public PhysicalColumn TemplateTable
		{
			get
			{
				return this.templateTable;
			}
		}

		public PhysicalColumn DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		public PhysicalColumn KeyFldIDs
		{
			get
			{
				return this.keyFldIDs;
			}
		}

		public PhysicalColumn VarSegMac
		{
			get
			{
				return this.varSegMac;
			}
		}

		public PhysicalColumn ConditionalColumns
		{
			get
			{
				return this.conditionalColumns;
			}
		}

		public PhysicalColumn LCMapFlags
		{
			get
			{
				return this.lCMapFlags;
			}
		}

		public PhysicalColumn TupleLimits
		{
			get
			{
				return this.tupleLimits;
			}
		}

		public PhysicalColumn Version
		{
			get
			{
				return this.version;
			}
		}

		public PhysicalColumn SortID
		{
			get
			{
				return this.sortID;
			}
		}

		public PhysicalColumn CallbackData
		{
			get
			{
				return this.callbackData;
			}
		}

		public PhysicalColumn CallbackDependencies
		{
			get
			{
				return this.callbackDependencies;
			}
		}

		public PhysicalColumn SeparateLV
		{
			get
			{
				return this.separateLV;
			}
		}

		public PhysicalColumn SpaceHints
		{
			get
			{
				return this.spaceHints;
			}
		}

		public PhysicalColumn SpaceDeferredLVHints
		{
			get
			{
				return this.spaceDeferredLVHints;
			}
		}

		public PhysicalColumn LocaleName
		{
			get
			{
				return this.localeName;
			}
		}

		public Index IdIndex
		{
			get
			{
				return this.idIndex;
			}
		}

		public Index NameIndex
		{
			get
			{
				return this.nameIndex;
			}
		}

		public Index RootObjectsIndex
		{
			get
			{
				return this.rootObjectsIndex;
			}
		}

		internal void PostMountInitialize(ComponentVersion databaseVersion)
		{
			PhysicalColumn physicalColumn = this.objidTable;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.objidTable = null;
			}
			physicalColumn = this.type;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.type = null;
			}
			physicalColumn = this.id;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.id = null;
			}
			physicalColumn = this.name;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.name = null;
			}
			physicalColumn = this.coltypOrPgnoFDP;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.coltypOrPgnoFDP = null;
			}
			physicalColumn = this.spaceUsage;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.spaceUsage = null;
			}
			physicalColumn = this.flags;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.flags = null;
			}
			physicalColumn = this.pagesOrLocale;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.pagesOrLocale = null;
			}
			physicalColumn = this.recordOffset;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.recordOffset = null;
			}
			physicalColumn = this.keyMost;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.keyMost = null;
			}
			physicalColumn = this.stats;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.stats = null;
			}
			physicalColumn = this.rootFlag;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.rootFlag = null;
			}
			physicalColumn = this.templateTable;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.templateTable = null;
			}
			physicalColumn = this.defaultValue;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.defaultValue = null;
			}
			physicalColumn = this.keyFldIDs;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.keyFldIDs = null;
			}
			physicalColumn = this.varSegMac;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.varSegMac = null;
			}
			physicalColumn = this.conditionalColumns;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.conditionalColumns = null;
			}
			physicalColumn = this.lCMapFlags;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lCMapFlags = null;
			}
			physicalColumn = this.tupleLimits;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.tupleLimits = null;
			}
			physicalColumn = this.version;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.version = null;
			}
			physicalColumn = this.sortID;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.sortID = null;
			}
			physicalColumn = this.callbackData;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.callbackData = null;
			}
			physicalColumn = this.callbackDependencies;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.callbackDependencies = null;
			}
			physicalColumn = this.separateLV;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.separateLV = null;
			}
			physicalColumn = this.spaceHints;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.spaceHints = null;
			}
			physicalColumn = this.spaceDeferredLVHints;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.spaceDeferredLVHints = null;
			}
			physicalColumn = this.localeName;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.localeName = null;
			}
			for (int i = this.Table.Columns.Count - 1; i >= 0; i--)
			{
				this.Table.Columns[i].Index = i;
			}
			Index index = this.idIndex;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.idIndex = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.nameIndex;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.nameIndex = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.rootObjectsIndex;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.rootObjectsIndex = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string ObjidTableName = "ObjidTable";

		public const string TypeName = "Type";

		public const string IdName = "Id";

		public const string NameName = "Name";

		public const string ColtypOrPgnoFDPName = "ColtypOrPgnoFDP";

		public const string SpaceUsageName = "SpaceUsage";

		public const string FlagsName = "Flags";

		public const string PagesOrLocaleName = "PagesOrLocale";

		public const string RecordOffsetName = "RecordOffset";

		public const string KeyMostName = "KeyMost";

		public const string StatsName = "Stats";

		public const string RootFlagName = "RootFlag";

		public const string TemplateTableName = "TemplateTable";

		public const string DefaultValueName = "DefaultValue";

		public const string KeyFldIDsName = "KeyFldIDs";

		public const string VarSegMacName = "VarSegMac";

		public const string ConditionalColumnsName = "ConditionalColumns";

		public const string LCMapFlagsName = "LCMapFlags";

		public const string TupleLimitsName = "TupleLimits";

		public const string VersionName = "Version";

		public const string SortIDName = "SortID";

		public const string CallbackDataName = "CallbackData";

		public const string CallbackDependenciesName = "CallbackDependencies";

		public const string SeparateLVName = "SeparateLV";

		public const string SpaceHintsName = "SpaceHints";

		public const string SpaceDeferredLVHintsName = "SpaceDeferredLVHints";

		public const string LocaleNameName = "LocaleName";

		public const string PhysicalTableName = "MSysObjects";

		private PhysicalColumn objidTable;

		private PhysicalColumn type;

		private PhysicalColumn id;

		private PhysicalColumn name;

		private PhysicalColumn coltypOrPgnoFDP;

		private PhysicalColumn spaceUsage;

		private PhysicalColumn flags;

		private PhysicalColumn pagesOrLocale;

		private PhysicalColumn recordOffset;

		private PhysicalColumn keyMost;

		private PhysicalColumn stats;

		private PhysicalColumn rootFlag;

		private PhysicalColumn templateTable;

		private PhysicalColumn defaultValue;

		private PhysicalColumn keyFldIDs;

		private PhysicalColumn varSegMac;

		private PhysicalColumn conditionalColumns;

		private PhysicalColumn lCMapFlags;

		private PhysicalColumn tupleLimits;

		private PhysicalColumn version;

		private PhysicalColumn sortID;

		private PhysicalColumn callbackData;

		private PhysicalColumn callbackDependencies;

		private PhysicalColumn separateLV;

		private PhysicalColumn spaceHints;

		private PhysicalColumn spaceDeferredLVHints;

		private PhysicalColumn localeName;

		private Index idIndex;

		private Index nameIndex;

		private Index rootObjectsIndex;

		private Table table;
	}
}
