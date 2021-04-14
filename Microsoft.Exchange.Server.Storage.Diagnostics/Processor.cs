using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal abstract class Processor
	{
		protected Processor(IList<Column> arguments)
		{
			this.arguments = Processor.GetDictionary(arguments);
		}

		public IDictionary<string, Column> Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		public abstract IEnumerable<Processor.ColumnDefinition> GetGeneratedColumns();

		public abstract object GetValue(SimpleQueryOperator qop, Reader reader, Column column);

		public virtual void OnBeginRow()
		{
		}

		public virtual void OnAfterRow()
		{
		}

		private static IDictionary<string, Column> GetDictionary(IList<Column> columns)
		{
			IDictionary<string, Column> dictionary = new Dictionary<string, Column>(columns.Count);
			foreach (Column column in columns)
			{
				dictionary[column.Name] = column;
			}
			return dictionary;
		}

		private readonly IDictionary<string, Column> arguments;

		public struct ColumnDefinition
		{
			public ColumnDefinition(string name, Type type, Visibility visibility)
			{
				this.Name = name;
				this.Type = type;
				this.Visibility = visibility;
			}

			public string Name;

			public Type Type;

			public Visibility Visibility;
		}
	}
}
