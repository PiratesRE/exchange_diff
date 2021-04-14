using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Mdb;

namespace Microsoft.Exchange.Search.Engine
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FeedingSkippedException : ComponentFailedTransientException
	{
		public FeedingSkippedException(MdbInfo mdbInfo, ContentIndexStatusType state, IndexStatusErrorCode indexStatusErrorCode) : base(Strings.FeedingSkipped(mdbInfo, state, indexStatusErrorCode))
		{
			this.mdbInfo = mdbInfo;
			this.state = state;
			this.indexStatusErrorCode = indexStatusErrorCode;
		}

		public FeedingSkippedException(MdbInfo mdbInfo, ContentIndexStatusType state, IndexStatusErrorCode indexStatusErrorCode, Exception innerException) : base(Strings.FeedingSkipped(mdbInfo, state, indexStatusErrorCode), innerException)
		{
			this.mdbInfo = mdbInfo;
			this.state = state;
			this.indexStatusErrorCode = indexStatusErrorCode;
		}

		protected FeedingSkippedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdbInfo = (MdbInfo)info.GetValue("mdbInfo", typeof(MdbInfo));
			this.state = (ContentIndexStatusType)info.GetValue("state", typeof(ContentIndexStatusType));
			this.indexStatusErrorCode = (IndexStatusErrorCode)info.GetValue("indexStatusErrorCode", typeof(IndexStatusErrorCode));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdbInfo", this.mdbInfo);
			info.AddValue("state", this.state);
			info.AddValue("indexStatusErrorCode", this.indexStatusErrorCode);
		}

		public MdbInfo MdbInfo
		{
			get
			{
				return this.mdbInfo;
			}
		}

		public ContentIndexStatusType State
		{
			get
			{
				return this.state;
			}
		}

		public IndexStatusErrorCode IndexStatusErrorCode
		{
			get
			{
				return this.indexStatusErrorCode;
			}
		}

		private readonly MdbInfo mdbInfo;

		private readonly ContentIndexStatusType state;

		private readonly IndexStatusErrorCode indexStatusErrorCode;
	}
}
