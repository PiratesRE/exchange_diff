using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetPhysicalColumn : PhysicalColumn, IJetColumn, IColumn
	{
		internal JetPhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, bool schemaExtension, Visibility visibility, int maxLength, int size, Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, int index, int maxInlineLength) : base(name, physicalName, type, nullable, identity, streamSupport, notFetchedByDefault, schemaExtension, visibility, maxLength, size, table, index, maxInlineLength)
		{
		}

		internal JetPhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, Visibility visibility, int maxLength, int size, Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, int index, int maxInlineLength) : base(name, physicalName, type, nullable, identity, streamSupport, notFetchedByDefault, visibility, maxLength, size, table, index, maxInlineLength)
		{
		}

		internal JET_COLUMNID GetJetColumnId(JetConnection connection)
		{
			if (!this.jetColumnIdSet)
			{
				try
				{
					JET_TABLEID openTable = connection.GetOpenTable(base.Table, base.Table.Name, null, Connection.OperationType.Query);
					using (connection.TrackTimeInDatabase())
					{
						JET_COLUMNDEF jet_COLUMNDEF;
						Api.JetGetTableColumnInfo(connection.JetSession, openTable, base.PhysicalName, out jet_COLUMNDEF);
						this.jetColumnId = jet_COLUMNDEF.columnid;
						this.jetColumnIdSet = true;
						Api.JetCloseTable(connection.JetSession, openTable);
					}
				}
				catch (EsentErrorException ex)
				{
					connection.OnExceptionCatch(ex);
					throw connection.ProcessJetError((LID)59848U, "JetPhysicalColumn.GetJetColumnId", ex);
				}
			}
			return this.jetColumnId;
		}

		byte[] IJetColumn.GetValueAsBytes(IJetSimpleQueryOperator cursor)
		{
			return cursor.GetPhysicalColumnValueAsBytes(this);
		}

		private JET_COLUMNID jetColumnId;

		private bool jetColumnIdSet;
	}
}
