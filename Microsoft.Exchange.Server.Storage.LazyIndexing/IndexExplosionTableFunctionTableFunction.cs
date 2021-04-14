using System;
using System.Collections;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public sealed class IndexExplosionTableFunctionTableFunction
	{
		internal IndexExplosionTableFunctionTableFunction()
		{
			this.instanceNum = Factory.CreatePhysicalColumn("InstanceNum", "InstanceNum", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
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
				this.InstanceNum
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("IndexExplosionTableFunction", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Public, new Type[]
			{
				typeof(Array)
			}, indexes, new PhysicalColumn[]
			{
				this.InstanceNum
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn InstanceNum
		{
			get
			{
				return this.instanceNum;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			return this.GetTableContentsReally(parameters);
		}

		public IEnumerable GetTableContentsReally(object[] parameters)
		{
			if (parameters[0] == null)
			{
				yield return 1;
			}
			else
			{
				Array arrayOfValues = (Array)parameters[0];
				if (arrayOfValues.Length == 0)
				{
					yield return 1;
				}
				else
				{
					for (int i = 0; i < arrayOfValues.Length; i++)
					{
						yield return i + 1;
					}
				}
			}
			yield break;
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			if (columnToFetch == this.InstanceNum)
			{
				return (int)row;
			}
			return null;
		}

		public const string InstanceNumName = "InstanceNum";

		public const string TableFunctionName = "IndexExplosionTableFunction";

		private PhysicalColumn instanceNum;

		private TableFunction tableFunction;
	}
}
