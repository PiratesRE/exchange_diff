using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Transport.Storage
{
	[Serializable]
	internal class DataSeekException : InvalidOperationException
	{
		public DataSeekException() : base(Strings.SeekGeneralFailure)
		{
		}

		public DataSeekException(string message) : base(message)
		{
		}

		public DataSeekException(string message, Exception inner) : base(message, inner)
		{
		}

		public DataSeekException(DataRow row, DataTableCursor cursor, string message) : base(message)
		{
			this.row = row;
			this.cursor = cursor;
		}

		protected DataSeekException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info != null)
			{
				this.row = (DataRow)info.GetValue("Row", typeof(DataRow));
				this.cursor = (DataTableCursor)info.GetValue("Cursor", typeof(DataTableCursor));
			}
		}

		public DataRow Row
		{
			get
			{
				return this.row;
			}
		}

		public DataTableCursor Cursor
		{
			get
			{
				return this.cursor;
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
			info.AddValue("Row", this.Row, typeof(DataRow));
			info.AddValue("cursor", this.Cursor, typeof(DataTableCursor));
		}

		private readonly DataRow row;

		private readonly DataTableCursor cursor;
	}
}
