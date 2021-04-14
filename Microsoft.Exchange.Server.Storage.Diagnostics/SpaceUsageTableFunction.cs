using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PhysicalAccessJet;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public sealed class SpaceUsageTableFunction
	{
		internal SpaceUsageTableFunction()
		{
			this.tableName = Factory.CreatePhysicalColumn("TableName", "TableName", typeof(string), false, false, false, false, false, Visibility.Public, 0, 128, 128);
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.ownedMB = Factory.CreatePhysicalColumn("OwnedMB", "OwnedMB", typeof(double), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.availableMB = Factory.CreatePhysicalColumn("AvailableMB", "AvailableMB", typeof(double), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			string name = "PrimaryKey";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[1];
			Index index = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.TableName
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("SpaceUsage", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Public, new Type[0], indexes, new PhysicalColumn[]
			{
				this.TableName,
				this.MailboxNumber,
				this.OwnedMB,
				this.AvailableMB
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn TableName
		{
			get
			{
				return this.tableName;
			}
		}

		public PhysicalColumn MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public PhysicalColumn OwnedMB
		{
			get
			{
				return this.ownedMB;
			}
		}

		public PhysicalColumn AvailableMB
		{
			get
			{
				return this.availableMB;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			if (connectionProvider.Database.DatabaseType == DatabaseType.Jet)
			{
				return from t in connectionProvider.Database.GetTableNames(connectionProvider).Union(SpaceUsageTableFunction.internalJetTables)
				select new SpaceUsageTableFunction.SpaceUsageRow(t) into s
				orderby s.TableName
				select s;
			}
			return new string[0];
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			SpaceUsageTableFunction.SpaceUsageRow spaceUsageRow = row as SpaceUsageTableFunction.SpaceUsageRow;
			if (this.tableName != null)
			{
				if (columnToFetch == this.TableName)
				{
					return spaceUsageRow.TableName;
				}
				if (columnToFetch == this.MailboxNumber)
				{
					return spaceUsageRow.MailboxNumber;
				}
				if (columnToFetch == this.OwnedMB)
				{
					return spaceUsageRow.GetOwnedMB(connectionProvider);
				}
				if (columnToFetch == this.AvailableMB)
				{
					return spaceUsageRow.GetAvailableMB(connectionProvider);
				}
			}
			return null;
		}

		public const string TableNameName = "TableName";

		public const string MailboxNumberName = "MailboxNumber";

		public const string OwnedMBName = "OwnedMB";

		public const string AvailableMBName = "AvailableMB";

		public const string TableFunctionName = "SpaceUsage";

		private PhysicalColumn tableName;

		private PhysicalColumn mailboxNumber;

		private PhysicalColumn ownedMB;

		private PhysicalColumn availableMB;

		private TableFunction tableFunction;

		private static string[] internalJetTables = new string[]
		{
			null,
			"MSysDatabaseMaintenance",
			"MSysLocales",
			"MSysObjects",
			"MSysObjectsShadow",
			"MSysObjids"
		};

		private class SpaceUsageRow
		{
			public SpaceUsageRow(string tableName)
			{
				this.tableName = tableName;
				this.mailboxNumber = null;
				if (this.tableName != null)
				{
					string[] array = this.tableName.Split(new char[]
					{
						'_'
					});
					int value;
					if (array.Length >= 2 && int.TryParse(array[1], out value))
					{
						this.mailboxNumber = new int?(value);
					}
				}
				this.isLoaded = false;
			}

			public string TableName
			{
				get
				{
					return this.tableName;
				}
			}

			public int? MailboxNumber
			{
				get
				{
					return this.mailboxNumber;
				}
			}

			public double GetOwnedMB(IConnectionProvider connectionProvider)
			{
				this.LoadSpaceUsage(connectionProvider);
				return this.ownedMB;
			}

			public double GetAvailableMB(IConnectionProvider connectionProvider)
			{
				this.LoadSpaceUsage(connectionProvider);
				return this.availableMB;
			}

			private void LoadSpaceUsage(IConnectionProvider connectionProvider)
			{
				if (!this.isLoaded)
				{
					if (this.tableName == null)
					{
						uint num;
						uint num2;
						uint num3;
						connectionProvider.Database.GetDatabaseSize(connectionProvider, out num, out num2, out num3);
						this.ownedMB = num * num3 / 1048576.0;
						this.availableMB = num2 * num3 / 1048576.0;
					}
					else
					{
						JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
						if (jetConnection != null)
						{
							int num4;
							int num5;
							jetConnection.GetTableSize(this.tableName, out num4, out num5);
							this.ownedMB = (double)num4 * (double)connectionProvider.Database.PageSize / 1048576.0;
							this.availableMB = (double)num5 * (double)connectionProvider.Database.PageSize / 1048576.0;
						}
					}
					this.isLoaded = true;
				}
			}

			private readonly string tableName;

			private readonly int? mailboxNumber;

			private bool isLoaded;

			private double ownedMB;

			private double availableMB;
		}
	}
}
