using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnknownRequestIndexPermanentException : MailboxReplicationPermanentException
	{
		public UnknownRequestIndexPermanentException(string indexId) : base(Strings.ErrorUnknownRequestIndex(indexId))
		{
			this.indexId = indexId;
		}

		public UnknownRequestIndexPermanentException(string indexId, Exception innerException) : base(Strings.ErrorUnknownRequestIndex(indexId), innerException)
		{
			this.indexId = indexId;
		}

		protected UnknownRequestIndexPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.indexId = (string)info.GetValue("indexId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("indexId", this.indexId);
		}

		public string IndexId
		{
			get
			{
				return this.indexId;
			}
		}

		private readonly string indexId;
	}
}
