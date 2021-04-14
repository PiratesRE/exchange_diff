using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Serializable]
	public class ReplicationCheckOutputObject : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ReplicationCheckOutputObject.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public string Server
		{
			get
			{
				return (string)this[ReplicationCheckOutputObjectSchema.Server];
			}
			private set
			{
				this[ReplicationCheckOutputObjectSchema.Server] = value;
			}
		}

		public CheckId CheckId
		{
			get
			{
				return (CheckId)this[ReplicationCheckOutputObjectSchema.CheckIdProperty];
			}
			private set
			{
				this[ReplicationCheckOutputObjectSchema.CheckIdProperty] = value;
			}
		}

		internal string Check
		{
			get
			{
				return (string)this[ReplicationCheckOutputObjectSchema.Check];
			}
			private set
			{
				this[ReplicationCheckOutputObjectSchema.Check] = value;
			}
		}

		public new string Identity
		{
			get
			{
				return (string)this[ReplicationCheckOutputObjectSchema.IdentityProperty];
			}
			private set
			{
				this[ReplicationCheckOutputObjectSchema.IdentityProperty] = value;
			}
		}

		public uint? DbFailureEventId
		{
			get
			{
				return (uint?)this[ReplicationCheckOutputObjectSchema.DbFailureEventId];
			}
			private set
			{
				this[ReplicationCheckOutputObjectSchema.DbFailureEventId] = value;
			}
		}

		public ReplicationCheckResult Result
		{
			get
			{
				return (ReplicationCheckResult)this[ReplicationCheckOutputObjectSchema.Result];
			}
			private set
			{
				this[ReplicationCheckOutputObjectSchema.Result] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this[ReplicationCheckOutputObjectSchema.Error];
			}
			private set
			{
				this[ReplicationCheckOutputObjectSchema.Error] = value;
			}
		}

		internal ReplicationCheckOutputObject(ReplicationCheck check) : base(new SimpleProviderPropertyBag())
		{
			this.Server = check.ServerName;
			this.Check = check.Title;
			this.CheckId = check.CheckId;
			this.Result = new ReplicationCheckResult(ReplicationCheckResultEnum.Undefined);
		}

		internal void Update(string instanceIdentity, ReplicationCheckResult newResult, string newErrorMessage)
		{
			this.Update(instanceIdentity, newResult, newErrorMessage, null);
		}

		internal void Update(string instanceIdentity, ReplicationCheckResult newResult, string newErrorMessage, uint? dbFailureEventId)
		{
			this.Identity = instanceIdentity;
			this.Result = newResult;
			this.Error = newErrorMessage;
			this.DbFailureEventId = dbFailureEventId;
		}

		private static ReplicationCheckOutputObjectSchema schema = ObjectSchema.GetInstance<ReplicationCheckOutputObjectSchema>();
	}
}
