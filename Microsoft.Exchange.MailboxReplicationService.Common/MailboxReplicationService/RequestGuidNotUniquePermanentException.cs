using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RequestGuidNotUniquePermanentException : MailboxReplicationPermanentException
	{
		public RequestGuidNotUniquePermanentException(string guid, string type) : base(MrsStrings.RequestGuidNotUnique(guid, type))
		{
			this.guid = guid;
			this.type = type;
		}

		public RequestGuidNotUniquePermanentException(string guid, string type, Exception innerException) : base(MrsStrings.RequestGuidNotUnique(guid, type), innerException)
		{
			this.guid = guid;
			this.type = type;
		}

		protected RequestGuidNotUniquePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.guid = (string)info.GetValue("guid", typeof(string));
			this.type = (string)info.GetValue("type", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("guid", this.guid);
			info.AddValue("type", this.type);
		}

		public string Guid
		{
			get
			{
				return this.guid;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly string guid;

		private readonly string type;
	}
}
