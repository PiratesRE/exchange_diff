using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRemoveCompletedDuringCancelPermanentException : CannotSetCompletedPermanentException
	{
		public CannotRemoveCompletedDuringCancelPermanentException(string indexEntry) : base(Strings.ErrorRequestCompletedDuringCancellation(indexEntry))
		{
			this.indexEntry = indexEntry;
		}

		public CannotRemoveCompletedDuringCancelPermanentException(string indexEntry, Exception innerException) : base(Strings.ErrorRequestCompletedDuringCancellation(indexEntry), innerException)
		{
			this.indexEntry = indexEntry;
		}

		protected CannotRemoveCompletedDuringCancelPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.indexEntry = (string)info.GetValue("indexEntry", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("indexEntry", this.indexEntry);
		}

		public string IndexEntry
		{
			get
			{
				return this.indexEntry;
			}
		}

		private readonly string indexEntry;
	}
}
