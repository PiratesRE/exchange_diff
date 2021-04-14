using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotRemoveUserWithoutBatchException : MigrationPermanentException
	{
		public CannotRemoveUserWithoutBatchException(string userName) : base(Strings.ErrorCannotRemoveUserWithoutBatch(userName))
		{
			this.userName = userName;
		}

		public CannotRemoveUserWithoutBatchException(string userName, Exception innerException) : base(Strings.ErrorCannotRemoveUserWithoutBatch(userName), innerException)
		{
			this.userName = userName;
		}

		protected CannotRemoveUserWithoutBatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userName = (string)info.GetValue("userName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userName", this.userName);
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		private readonly string userName;
	}
}
