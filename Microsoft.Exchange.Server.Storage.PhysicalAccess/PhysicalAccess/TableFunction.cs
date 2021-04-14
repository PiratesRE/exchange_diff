using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class TableFunction : Table
	{
		protected TableFunction(string name, TableFunction.GetTableContentsDelegate getTableContents, TableFunction.GetColumnFromRowDelegate getColumnFromRow, Visibility visibility, Type[] parameterTypes, Index[] indexes, params PhysicalColumn[] columns) : base(name, TableClass.TableFunction, CultureHelper.DefaultCultureInfo, false, TableAccessHints.None, false, visibility, false, TableFunction.noSpecialColumns, indexes, Table.NoColumns, columns)
		{
			this.getTableContents = getTableContents;
			this.getColumnFromRow = getColumnFromRow;
			this.parameterTypes = parameterTypes;
		}

		public Type[] ParameterTypes
		{
			get
			{
				return this.parameterTypes;
			}
		}

		internal TableFunction.GetTableContentsDelegate GetTableContents
		{
			get
			{
				return this.getTableContents;
			}
		}

		internal TableFunction.GetColumnFromRowDelegate GetColumnFromRow
		{
			get
			{
				return this.getColumnFromRow;
			}
		}

		public override void CreateTable(IConnectionProvider connectionProvider, int version)
		{
			throw new NotSupportedException("CreateTable not supported against table function");
		}

		public override void AddColumn(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			throw new NotSupportedException("AddColumn not supported against table function");
		}

		public override void RemoveColumn(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			throw new NotSupportedException("RemoveColumn not supported against table function");
		}

		public override void CreateIndex(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues)
		{
			throw new NotSupportedException("CreateIndex not supported against table function");
		}

		public override void DeleteIndex(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues)
		{
			throw new NotSupportedException("DeleteIndex not supported against table function");
		}

		public override bool IsIndexCreated(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues)
		{
			throw new NotSupportedException("IsIndexCreated not supported against table function");
		}

		public override bool ValidateLocaleVersion(IConnectionProvider connectionProvider, IList<object> partitionValues)
		{
			throw new NotSupportedException("ValidateLocaleVersion not supported against table function");
		}

		public override void GetTableSize(IConnectionProvider connectionProvider, IList<object> partitionValues, out int totalPages, out int availablePages)
		{
			throw new NotSupportedException("GetTableSize not supported against table function");
		}

		private static readonly SpecialColumns noSpecialColumns = default(SpecialColumns);

		private readonly TableFunction.GetTableContentsDelegate getTableContents;

		private readonly TableFunction.GetColumnFromRowDelegate getColumnFromRow;

		private readonly Type[] parameterTypes;

		public delegate object GetTableContentsDelegate(IConnectionProvider connectionProvider, object[] parameters);

		public delegate object GetColumnFromRowDelegate(IConnectionProvider connectionProvider, object obj, PhysicalColumn columnToFetch);
	}
}
