using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	[Serializable]
	internal class SchemaException : SystemException
	{
		public SchemaException() : base(Strings.SchemaInvalid)
		{
		}

		public SchemaException(string message) : base(message)
		{
		}

		public SchemaException(string message, Exception inner) : base(message, inner)
		{
		}

		public SchemaException(string message, JET_TABLEID table, DataColumn column) : base(message)
		{
			this.table = table;
			this.column = column;
		}

		protected SchemaException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info != null)
			{
				this.table = (JET_TABLEID)info.GetValue("Table", typeof(JET_TABLEID));
				this.column = (DataColumn)info.GetValue("Column", typeof(DataColumn));
			}
		}

		public DataColumn Column
		{
			get
			{
				return this.column;
			}
		}

		public JET_TABLEID Table
		{
			get
			{
				return this.table;
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("Column", this.Column, typeof(DataColumn));
			info.AddValue("Table", this.Table, typeof(JET_TABLEID));
		}

		private DataColumn column;

		private JET_TABLEID table;
	}
}
