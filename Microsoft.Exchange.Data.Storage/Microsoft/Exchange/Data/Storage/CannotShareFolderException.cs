using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotShareFolderException : StoragePermanentException
	{
		public CannotShareFolderException(string reason) : base(ServerStrings.CannotShareFolderException(reason))
		{
			this.reason = reason;
		}

		public CannotShareFolderException(string reason, Exception innerException) : base(ServerStrings.CannotShareFolderException(reason), innerException)
		{
			this.reason = reason;
		}

		protected CannotShareFolderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", this.reason);
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string reason;
	}
}
