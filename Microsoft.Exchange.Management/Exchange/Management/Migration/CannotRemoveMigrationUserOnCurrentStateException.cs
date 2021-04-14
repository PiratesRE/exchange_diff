using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRemoveMigrationUserOnCurrentStateException : LocalizedException
	{
		public CannotRemoveMigrationUserOnCurrentStateException(string user, string batchName) : base(Strings.ErrorCannotRemoveMigrationUserOnCurrentState(user, batchName))
		{
			this.user = user;
			this.batchName = batchName;
		}

		public CannotRemoveMigrationUserOnCurrentStateException(string user, string batchName, Exception innerException) : base(Strings.ErrorCannotRemoveMigrationUserOnCurrentState(user, batchName), innerException)
		{
			this.user = user;
			this.batchName = batchName;
		}

		protected CannotRemoveMigrationUserOnCurrentStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
			this.batchName = (string)info.GetValue("batchName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
			info.AddValue("batchName", this.batchName);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		public string BatchName
		{
			get
			{
				return this.batchName;
			}
		}

		private readonly string user;

		private readonly string batchName;
	}
}
