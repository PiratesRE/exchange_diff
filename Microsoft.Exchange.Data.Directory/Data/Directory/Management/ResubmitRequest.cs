using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ResubmitRequest : ConfigurableObject
	{
		public ResubmitRequest() : base(new SimpleProviderPropertyBag())
		{
		}

		private ResubmitRequest(ResubmitRequestId identity) : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag.SetObjectVersion(ExchangeObjectVersion.Exchange2010);
			this[ResubmitRequestSchema.ResubmitRequestIdentity] = identity.ToString();
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return ResubmitRequestId.Parse((string)this[ResubmitRequestSchema.ResubmitRequestIdentity]);
			}
		}

		public string Server
		{
			get
			{
				return (string)this[ResubmitRequestSchema.Server];
			}
			internal set
			{
				this[ResubmitRequestSchema.Server] = value;
			}
		}

		public string Destination
		{
			get
			{
				return (string)this[ResubmitRequestSchema.Destination];
			}
			internal set
			{
				this[ResubmitRequestSchema.Destination] = value;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return (DateTime)this[ResubmitRequestSchema.StartTime];
			}
			internal set
			{
				this[ResubmitRequestSchema.StartTime] = value;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return (DateTime)this[ResubmitRequestSchema.EndTime];
			}
			internal set
			{
				this[ResubmitRequestSchema.EndTime] = value;
			}
		}

		public string DiagnosticInformation
		{
			get
			{
				return (string)this[ResubmitRequestSchema.DiagnosticInformation];
			}
			internal set
			{
				this[ResubmitRequestSchema.DiagnosticInformation] = value;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return (DateTime)this[ResubmitRequestSchema.CreationTime];
			}
			internal set
			{
				this[ResubmitRequestSchema.CreationTime] = value;
			}
		}

		public ResubmitRequestState State
		{
			get
			{
				return (ResubmitRequestState)this[ResubmitRequestSchema.State];
			}
			internal set
			{
				this[ResubmitRequestSchema.State] = value;
			}
		}

		internal static ResubmitRequest Create(long rowId, string server, DateTime starttime, string destination, string diagnosticInformation, DateTime endTime, DateTime dateCreated, int state)
		{
			return new ResubmitRequest(new ResubmitRequestId(rowId))
			{
				Server = server,
				StartTime = starttime,
				Destination = destination,
				DiagnosticInformation = diagnosticInformation,
				EndTime = endTime,
				CreationTime = dateCreated,
				State = (ResubmitRequestState)state
			};
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ResubmitRequest.schema;
			}
		}

		internal ResubmitRequestId ResubmitRequestId
		{
			get
			{
				return ResubmitRequestId.Parse((string)this[ResubmitRequestSchema.ResubmitRequestIdentity]);
			}
		}

		public const string IdentityParameterName = "ResubmitRequestIdentity";

		public const string DumpsterRequestEnabledName = "DumpsterRequestEnabled";

		private static ResubmitRequestSchema schema = ObjectSchema.GetInstance<ResubmitRequestSchema>();
	}
}
