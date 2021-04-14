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
	internal class FeedingSkippedForCorruptionException : ComponentFailedTransientException
	{
		public FeedingSkippedForCorruptionException(MdbInfo mdbInfo, ContentIndexStatusType state, IndexStatusErrorCode indexStatusErrorCode, int? failureCode, string failureReason) : base(Strings.FeedingSkippedWithFailureCode(mdbInfo, state, indexStatusErrorCode, failureCode, failureReason))
		{
			this.mdbInfo = mdbInfo;
			this.state = state;
			this.indexStatusErrorCode = indexStatusErrorCode;
			this.failureCode = failureCode;
			this.failureReason = failureReason;
		}

		public FeedingSkippedForCorruptionException(MdbInfo mdbInfo, ContentIndexStatusType state, IndexStatusErrorCode indexStatusErrorCode, int? failureCode, string failureReason, Exception innerException) : base(Strings.FeedingSkippedWithFailureCode(mdbInfo, state, indexStatusErrorCode, failureCode, failureReason), innerException)
		{
			this.mdbInfo = mdbInfo;
			this.state = state;
			this.indexStatusErrorCode = indexStatusErrorCode;
			this.failureCode = failureCode;
			this.failureReason = failureReason;
		}

		protected FeedingSkippedForCorruptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdbInfo = (MdbInfo)info.GetValue("mdbInfo", typeof(MdbInfo));
			this.state = (ContentIndexStatusType)info.GetValue("state", typeof(ContentIndexStatusType));
			this.indexStatusErrorCode = (IndexStatusErrorCode)info.GetValue("indexStatusErrorCode", typeof(IndexStatusErrorCode));
			this.failureCode = (int?)info.GetValue("failureCode", typeof(int?));
			this.failureReason = (string)info.GetValue("failureReason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdbInfo", this.mdbInfo);
			info.AddValue("state", this.state);
			info.AddValue("indexStatusErrorCode", this.indexStatusErrorCode);
			info.AddValue("failureCode", this.failureCode);
			info.AddValue("failureReason", this.failureReason);
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

		public int? FailureCode
		{
			get
			{
				return this.failureCode;
			}
		}

		public string FailureReason
		{
			get
			{
				return this.failureReason;
			}
		}

		private readonly MdbInfo mdbInfo;

		private readonly ContentIndexStatusType state;

		private readonly IndexStatusErrorCode indexStatusErrorCode;

		private readonly int? failureCode;

		private readonly string failureReason;
	}
}
