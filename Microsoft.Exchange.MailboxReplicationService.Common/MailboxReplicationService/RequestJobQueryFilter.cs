using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class RequestJobQueryFilter : QueryFilter
	{
		public RequestJobQueryFilter(RequestJobObjectId requestJobId)
		{
			this.RequestGuid = requestJobId.RequestGuid;
			this.MdbGuid = requestJobId.MdbGuid;
			this.RequestType = null;
		}

		public RequestJobQueryFilter(Guid req, Guid mdb, MRSRequestType type)
		{
			this.RequestGuid = req;
			this.MdbGuid = mdb;
			this.RequestType = new MRSRequestType?(type);
		}

		public Guid RequestGuid { get; private set; }

		public Guid MdbGuid { get; private set; }

		public MRSRequestType? RequestType { get; private set; }

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(database=");
			sb.Append(this.MdbGuid.ToString());
			if (this.RequestGuid != Guid.Empty)
			{
				sb.Append(",request=");
				sb.Append(this.RequestGuid.ToString());
			}
			if (this.RequestType != null)
			{
				sb.Append(",type=");
				sb.Append(this.RequestType.ToString());
			}
			sb.Append(")");
		}
	}
}
