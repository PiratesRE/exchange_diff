using System;
using System.Data.Linq.Mapping;
using System.Reflection;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal class MetaTableWrapper : MetaTable
	{
		public MetaTableWrapper(MetaModelWrapper metaModelWrapper, MetaTable metaTable, string tableName)
		{
			this.metaModelWrapper = metaModelWrapper;
			this.metaTable = metaTable;
			this.tableName = tableName;
			this.metaTypeWrapper = new MetaTypeWrapper(this.metaModelWrapper, this.metaTable.RowType, this);
		}

		public override MetaModel Model
		{
			get
			{
				return this.metaModelWrapper;
			}
		}

		public override string TableName
		{
			get
			{
				return this.tableName;
			}
		}

		public override MetaType RowType
		{
			get
			{
				return this.metaTypeWrapper;
			}
		}

		public override MethodInfo InsertMethod
		{
			get
			{
				return this.metaTable.InsertMethod;
			}
		}

		public override MethodInfo UpdateMethod
		{
			get
			{
				return this.metaTable.UpdateMethod;
			}
		}

		public override MethodInfo DeleteMethod
		{
			get
			{
				return this.metaTable.DeleteMethod;
			}
		}

		private MetaTable metaTable;

		private MetaModelWrapper metaModelWrapper;

		private readonly string tableName;

		private MetaTypeWrapper metaTypeWrapper;
	}
}
