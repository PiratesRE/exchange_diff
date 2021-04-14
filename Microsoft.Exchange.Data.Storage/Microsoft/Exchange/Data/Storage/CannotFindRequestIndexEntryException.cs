using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotFindRequestIndexEntryException : StoragePermanentException
	{
		public CannotFindRequestIndexEntryException(Guid requestGuid) : base(ServerStrings.CannotFindRequestIndexEntry(requestGuid))
		{
			this.requestGuid = requestGuid;
		}

		public CannotFindRequestIndexEntryException(Guid requestGuid, Exception innerException) : base(ServerStrings.CannotFindRequestIndexEntry(requestGuid), innerException)
		{
			this.requestGuid = requestGuid;
		}

		protected CannotFindRequestIndexEntryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.requestGuid = (Guid)info.GetValue("requestGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("requestGuid", this.requestGuid);
		}

		public Guid RequestGuid
		{
			get
			{
				return this.requestGuid;
			}
		}

		private readonly Guid requestGuid;
	}
}
