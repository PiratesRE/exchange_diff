using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal class MappingSourceWrapper : MappingSource
	{
		public MappingSourceWrapper(MappingSource mappingSource)
		{
			this.MappingSource = mappingSource;
			this.viewTypeMapping = new Dictionary<Type, string>();
		}

		public MappingSource MappingSource { get; private set; }

		public void AddMapping(Type type, string viewName)
		{
			this.viewTypeMapping[type] = viewName;
		}

		protected override MetaModel CreateModel(Type dataContextType)
		{
			MetaModel model = this.MappingSource.GetModel(dataContextType);
			return new MetaModelWrapper(this, model);
		}

		public string FindView(Type type)
		{
			if (this.viewTypeMapping.ContainsKey(type))
			{
				return this.viewTypeMapping[type];
			}
			return null;
		}

		private readonly Dictionary<Type, string> viewTypeMapping;
	}
}
