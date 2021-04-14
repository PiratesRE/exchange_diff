using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class UserInfoTable
	{
		internal UserInfoTable()
		{
			this.userGuid = Factory.CreatePhysicalColumn("UserGuid", "UserGuid", typeof(Guid), false, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.status = Factory.CreatePhysicalColumn("Status", "Status", typeof(short), false, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.deletedOn = Factory.CreatePhysicalColumn("DeletedOn", "DeletedOn", typeof(DateTime), true, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.creationTime = Factory.CreatePhysicalColumn("CreationTime", "CreationTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.lastModificationTime = Factory.CreatePhysicalColumn("LastModificationTime", "LastModificationTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.changeNumber = Factory.CreatePhysicalColumn("ChangeNumber", "ChangeNumber", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.lastInteractiveLogonTime = Factory.CreatePhysicalColumn("LastInteractiveLogonTime", "LastInteractiveLogonTime", typeof(DateTime), true, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.propertyBlob = Factory.CreatePhysicalColumn("PropertyBlob", "PropertyBlob", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 30000);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			string name = "UserInfoPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[1];
			this.userInfoPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.UserGuid
			});
			Index[] indexes = new Index[]
			{
				this.UserInfoPK
			};
			SpecialColumns specialCols = new SpecialColumns(this.PropertyBlob, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.UserGuid,
				this.Status,
				this.DeletedOn,
				this.CreationTime,
				this.LastModificationTime,
				this.ChangeNumber,
				this.LastInteractiveLogonTime,
				this.PropertyBlob,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("UserInfo", TableClass.UserInfo, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Redacted, true, specialCols, indexes, computedColumns, columns);
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public PhysicalColumn UserGuid
		{
			get
			{
				return this.userGuid;
			}
		}

		public PhysicalColumn Status
		{
			get
			{
				return this.status;
			}
		}

		public PhysicalColumn DeletedOn
		{
			get
			{
				return this.deletedOn;
			}
		}

		public PhysicalColumn CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		public PhysicalColumn LastModificationTime
		{
			get
			{
				return this.lastModificationTime;
			}
		}

		public PhysicalColumn ChangeNumber
		{
			get
			{
				return this.changeNumber;
			}
		}

		public PhysicalColumn LastInteractiveLogonTime
		{
			get
			{
				return this.lastInteractiveLogonTime;
			}
		}

		public PhysicalColumn PropertyBlob
		{
			get
			{
				return this.propertyBlob;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index UserInfoPK
		{
			get
			{
				return this.userInfoPK;
			}
		}

		internal void PostMountInitialize(ComponentVersion databaseVersion)
		{
			PhysicalColumn physicalColumn = this.userGuid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.userGuid = null;
			}
			physicalColumn = this.status;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.status = null;
			}
			physicalColumn = this.deletedOn;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.deletedOn = null;
			}
			physicalColumn = this.creationTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.creationTime = null;
			}
			physicalColumn = this.lastModificationTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastModificationTime = null;
			}
			physicalColumn = this.changeNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.changeNumber = null;
			}
			physicalColumn = this.lastInteractiveLogonTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastInteractiveLogonTime = null;
			}
			physicalColumn = this.propertyBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propertyBlob = null;
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
			Index index = this.userInfoPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.userInfoPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string UserGuidName = "UserGuid";

		public const string StatusName = "Status";

		public const string DeletedOnName = "DeletedOn";

		public const string CreationTimeName = "CreationTime";

		public const string LastModificationTimeName = "LastModificationTime";

		public const string ChangeNumberName = "ChangeNumber";

		public const string LastInteractiveLogonTimeName = "LastInteractiveLogonTime";

		public const string PropertyBlobName = "PropertyBlob";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "UserInfo";

		private PhysicalColumn userGuid;

		private PhysicalColumn status;

		private PhysicalColumn deletedOn;

		private PhysicalColumn creationTime;

		private PhysicalColumn lastModificationTime;

		private PhysicalColumn changeNumber;

		private PhysicalColumn lastInteractiveLogonTime;

		private PhysicalColumn propertyBlob;

		private PhysicalColumn extensionBlob;

		private Index userInfoPK;

		private Table table;
	}
}
