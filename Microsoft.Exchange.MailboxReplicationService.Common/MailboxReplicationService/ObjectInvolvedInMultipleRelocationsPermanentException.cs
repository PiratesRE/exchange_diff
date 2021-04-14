using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ObjectInvolvedInMultipleRelocationsPermanentException : MailboxReplicationPermanentException
	{
		public ObjectInvolvedInMultipleRelocationsPermanentException(LocalizedString objectInvolved, string requestGuids) : base(MrsStrings.ValidationObjectInvolvedInMultipleRelocations(objectInvolved, requestGuids))
		{
			this.objectInvolved = objectInvolved;
			this.requestGuids = requestGuids;
		}

		public ObjectInvolvedInMultipleRelocationsPermanentException(LocalizedString objectInvolved, string requestGuids, Exception innerException) : base(MrsStrings.ValidationObjectInvolvedInMultipleRelocations(objectInvolved, requestGuids), innerException)
		{
			this.objectInvolved = objectInvolved;
			this.requestGuids = requestGuids;
		}

		protected ObjectInvolvedInMultipleRelocationsPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.objectInvolved = (LocalizedString)info.GetValue("objectInvolved", typeof(LocalizedString));
			this.requestGuids = (string)info.GetValue("requestGuids", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("objectInvolved", this.objectInvolved);
			info.AddValue("requestGuids", this.requestGuids);
		}

		public LocalizedString ObjectInvolved
		{
			get
			{
				return this.objectInvolved;
			}
		}

		public string RequestGuids
		{
			get
			{
				return this.requestGuids;
			}
		}

		private readonly LocalizedString objectInvolved;

		private readonly string requestGuids;
	}
}
