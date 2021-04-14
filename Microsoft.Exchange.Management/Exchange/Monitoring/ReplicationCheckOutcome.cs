using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public sealed class ReplicationCheckOutcome : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ReplicationCheckOutcome.schema;
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
				return (string)this[ReplicationCheckOutcomeSchema.Server];
			}
			private set
			{
				this[ReplicationCheckOutcomeSchema.Server] = value;
			}
		}

		public string Check
		{
			get
			{
				return (string)this[ReplicationCheckOutcomeSchema.Check];
			}
			private set
			{
				this[ReplicationCheckOutcomeSchema.Check] = value;
			}
		}

		public string CheckDescription
		{
			get
			{
				return (string)this[ReplicationCheckOutcomeSchema.CheckDescription];
			}
			private set
			{
				this[ReplicationCheckOutcomeSchema.CheckDescription] = value;
			}
		}

		public ReplicationCheckResult Result
		{
			get
			{
				return (ReplicationCheckResult)this[ReplicationCheckOutcomeSchema.Result];
			}
			private set
			{
				this[ReplicationCheckOutcomeSchema.Result] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this[ReplicationCheckOutcomeSchema.Error];
			}
			private set
			{
				this[ReplicationCheckOutcomeSchema.Error] = value;
			}
		}

		internal ReplicationCheckOutcome(string serverName, string checktitle, string checkdescription, ReplicationCheckResult result1, string errorMsg) : base(new SimpleProviderPropertyBag())
		{
			this.Server = serverName;
			this.Check = checktitle;
			this.CheckDescription = checkdescription;
			this.Result = result1;
			this.Error = errorMsg;
		}

		internal ReplicationCheckOutcome(ReplicationCheck check) : base(new SimpleProviderPropertyBag())
		{
			this.Server = check.ServerName;
			this.Check = check.Title;
			this.CheckDescription = check.Description;
			this.Result = new ReplicationCheckResult(ReplicationCheckResultEnum.Undefined);
		}

		internal void Update(ReplicationCheckResult newResult, string newErrorMessage)
		{
			this.Result = newResult;
			this.Error = newErrorMessage;
		}

		private static ReplicationCheckOutcomeSchema schema = ObjectSchema.GetInstance<ReplicationCheckOutcomeSchema>();
	}
}
