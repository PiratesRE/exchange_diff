using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class OnDemandQueryRequest : ConfigurablePropertyBag
	{
		public OnDemandQueryRequest(Guid tenantId, Guid requestId) : this()
		{
			this.TenantId = tenantId;
			this.RequestId = requestId;
		}

		public OnDemandQueryRequest()
		{
			this[ADObjectSchema.Id] = CombGuidGenerator.NewGuid();
			this[OnDemandQueryRequestSchema.Container] = OnDemandQueryRequestStatus.NotStarted.ToString();
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.RequestId.ToString());
			}
		}

		public Guid TenantId
		{
			get
			{
				return (Guid)this[OnDemandQueryRequestSchema.TenantId];
			}
			set
			{
				this[OnDemandQueryRequestSchema.TenantId] = value;
			}
		}

		public Guid RequestId
		{
			get
			{
				return Guid.Parse((string)this[OnDemandQueryRequestSchema.RequestId]);
			}
			set
			{
				this[OnDemandQueryRequestSchema.RequestId] = value.ToString();
			}
		}

		public DateTime SubmissionTime
		{
			get
			{
				return (DateTime)this[OnDemandQueryRequestSchema.SubmissionTime];
			}
			set
			{
				this[OnDemandQueryRequestSchema.SubmissionTime] = value;
			}
		}

		public OnDemandQueryRequestStatus RequestStatus
		{
			get
			{
				return (OnDemandQueryRequestStatus)this[OnDemandQueryRequestSchema.RequestStatus];
			}
			set
			{
				this[OnDemandQueryRequestSchema.RequestStatus] = value;
				this[OnDemandQueryRequestSchema.Container] = value.ToString();
			}
		}

		public string QueryDefinition
		{
			get
			{
				return (string)this[OnDemandQueryRequestSchema.QueryDefinition];
			}
			set
			{
				this[OnDemandQueryRequestSchema.QueryDefinition] = value;
			}
		}

		public Guid? BatchId
		{
			get
			{
				return (Guid?)this[OnDemandQueryRequestSchema.BatchId];
			}
			set
			{
				this[OnDemandQueryRequestSchema.BatchId] = value;
			}
		}

		public string Region
		{
			get
			{
				return (string)this[OnDemandQueryRequestSchema.Region];
			}
			set
			{
				this[OnDemandQueryRequestSchema.Region] = value;
			}
		}

		public string QuerySubject
		{
			get
			{
				return (string)this[OnDemandQueryRequestSchema.QuerySubject];
			}
			set
			{
				this[OnDemandQueryRequestSchema.QuerySubject] = value;
			}
		}

		public OnDemandQueryType QueryType
		{
			get
			{
				return (OnDemandQueryType)this[OnDemandQueryRequestSchema.QueryType];
			}
			set
			{
				this[OnDemandQueryRequestSchema.QueryType] = value;
			}
		}

		public OnDemandQueryPriority QueryPriority
		{
			get
			{
				return (OnDemandQueryPriority)this[OnDemandQueryRequestSchema.QueryPriority];
			}
			set
			{
				this[OnDemandQueryRequestSchema.QueryPriority] = value;
			}
		}

		public OnDemandQueryCallerType CallerType
		{
			get
			{
				return (OnDemandQueryCallerType)this[OnDemandQueryRequestSchema.CallerType];
			}
			set
			{
				this[OnDemandQueryRequestSchema.CallerType] = value;
			}
		}

		public long ResultSize
		{
			get
			{
				return (long)this[OnDemandQueryRequestSchema.ResultSize];
			}
			set
			{
				this[OnDemandQueryRequestSchema.ResultSize] = value;
			}
		}

		public int MatchRowCounts
		{
			get
			{
				return (int)this[OnDemandQueryRequestSchema.MatchRowCounts];
			}
			set
			{
				this[OnDemandQueryRequestSchema.MatchRowCounts] = value;
			}
		}

		public int ResultRowCounts
		{
			get
			{
				return (int)this[OnDemandQueryRequestSchema.ResultRowCounts];
			}
			set
			{
				this[OnDemandQueryRequestSchema.ResultRowCounts] = value;
			}
		}

		public int ViewCounts
		{
			get
			{
				return (int)this[OnDemandQueryRequestSchema.ViewCounts];
			}
			set
			{
				this[OnDemandQueryRequestSchema.ViewCounts] = value;
			}
		}

		public string CosmosResultUri
		{
			get
			{
				return (string)this[OnDemandQueryRequestSchema.CosmosResultUri];
			}
			set
			{
				this[OnDemandQueryRequestSchema.CosmosResultUri] = value;
			}
		}

		public Guid? CosmosJobId
		{
			get
			{
				return (Guid?)this[OnDemandQueryRequestSchema.CosmosJobId];
			}
			set
			{
				this[OnDemandQueryRequestSchema.CosmosJobId] = value;
			}
		}

		public int InBatchQueryId
		{
			get
			{
				return (int)this[OnDemandQueryRequestSchema.InBatchQueryId];
			}
			set
			{
				this[OnDemandQueryRequestSchema.InBatchQueryId] = value;
			}
		}

		public string NotificationEmail
		{
			get
			{
				return (string)this[OnDemandQueryRequestSchema.NotificationEmail];
			}
			set
			{
				this[OnDemandQueryRequestSchema.NotificationEmail] = value;
			}
		}

		public int RetryCount
		{
			get
			{
				return (int)this[OnDemandQueryRequestSchema.RetryCount];
			}
			set
			{
				this[OnDemandQueryRequestSchema.RetryCount] = value;
			}
		}

		public string ResultLocale
		{
			get
			{
				return (string)this[OnDemandQueryRequestSchema.ResultLocale];
			}
			set
			{
				this[OnDemandQueryRequestSchema.ResultLocale] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(OnDemandQueryRequestSchema);
		}

		public static string DefaultContainer = OnDemandQueryRequestStatus.NotStarted.ToString();
	}
}
