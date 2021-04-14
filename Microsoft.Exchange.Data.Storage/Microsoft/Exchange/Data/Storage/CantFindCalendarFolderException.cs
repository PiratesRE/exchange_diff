using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CantFindCalendarFolderException : StoragePermanentException
	{
		public CantFindCalendarFolderException(object identity) : base(ServerStrings.CantFindCalendarFolderException(identity))
		{
			this.identity = identity;
		}

		public CantFindCalendarFolderException(object identity, Exception innerException) : base(ServerStrings.CantFindCalendarFolderException(identity), innerException)
		{
			this.identity = identity;
		}

		protected CantFindCalendarFolderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = info.GetValue("identity", typeof(object));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
		}

		public object Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly object identity;
	}
}
