using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal class MetaModelWrapper : MetaModel
	{
		public MetaModelWrapper(MappingSourceWrapper mappingSourceWrapper, MetaModel metaModel)
		{
			this.MetaModel = metaModel;
			this.mappingSourceWrapper = mappingSourceWrapper;
		}

		public MetaModel MetaModel { get; private set; }

		public MetaTable Wrap(MetaTable metaTable)
		{
			string text = this.mappingSourceWrapper.FindView(metaTable.RowType.Type);
			if (text != null)
			{
				return new MetaTableWrapper(this, metaTable, text);
			}
			return metaTable;
		}

		public override MetaTable GetTable(Type rowType)
		{
			MetaTable table = this.MetaModel.GetTable(rowType);
			return this.Wrap(table);
		}

		public override IEnumerable<MetaTable> GetTables()
		{
			return this.MetaModel.GetTables().Select(new Func<MetaTable, MetaTable>(this.Wrap));
		}

		public override MetaFunction GetFunction(MethodInfo method)
		{
			return this.MetaModel.GetFunction(method);
		}

		public override IEnumerable<MetaFunction> GetFunctions()
		{
			return this.MetaModel.GetFunctions();
		}

		public override MetaType GetMetaType(Type type)
		{
			return this.MetaModel.GetMetaType(type);
		}

		public override MappingSource MappingSource
		{
			get
			{
				return this.mappingSourceWrapper;
			}
		}

		public override Type ContextType
		{
			get
			{
				return this.MetaModel.ContextType;
			}
		}

		public override string DatabaseName
		{
			get
			{
				return this.MetaModel.DatabaseName;
			}
		}

		public override Type ProviderType
		{
			get
			{
				return this.MetaModel.ProviderType;
			}
		}

		private readonly MappingSourceWrapper mappingSourceWrapper;
	}
}
