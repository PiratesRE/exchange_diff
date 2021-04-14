using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxConstraintsMismatchException : LocalizedException
	{
		public MailboxConstraintsMismatchException(string user, string databaseName, string constraint) : base(Strings.ErrorCannotMoveToTargetDatabaseAsConstraintsAreNotMet(user, databaseName, constraint))
		{
			this.user = user;
			this.databaseName = databaseName;
			this.constraint = constraint;
		}

		public MailboxConstraintsMismatchException(string user, string databaseName, string constraint, Exception innerException) : base(Strings.ErrorCannotMoveToTargetDatabaseAsConstraintsAreNotMet(user, databaseName, constraint), innerException)
		{
			this.user = user;
			this.databaseName = databaseName;
			this.constraint = constraint;
		}

		protected MailboxConstraintsMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
			this.constraint = (string)info.GetValue("constraint", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
			info.AddValue("databaseName", this.databaseName);
			info.AddValue("constraint", this.constraint);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		public string Constraint
		{
			get
			{
				return this.constraint;
			}
		}

		private readonly string user;

		private readonly string databaseName;

		private readonly string constraint;
	}
}
