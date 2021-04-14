using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorMoveActiveCopyNotFoundException : LocalizedException
	{
		public ErrorMoveActiveCopyNotFoundException(Guid db, string errorMsg) : base(Strings.ErrorMoveActiveCopyNotFoundException(db, errorMsg))
		{
			this.db = db;
			this.errorMsg = errorMsg;
		}

		public ErrorMoveActiveCopyNotFoundException(Guid db, string errorMsg, Exception innerException) : base(Strings.ErrorMoveActiveCopyNotFoundException(db, errorMsg), innerException)
		{
			this.db = db;
			this.errorMsg = errorMsg;
		}

		protected ErrorMoveActiveCopyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.db = (Guid)info.GetValue("db", typeof(Guid));
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("db", this.db);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public Guid Db
		{
			get
			{
				return this.db;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly Guid db;

		private readonly string errorMsg;
	}
}
