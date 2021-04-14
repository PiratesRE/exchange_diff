using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Security.OAuth
{
	[Serializable]
	public sealed class ValidationResultNode : ConfigurableObject
	{
		public ValidationResultNode(LocalizedString task, LocalizedString result, ResultType type) : base(new SimplePropertyBag(SimpleProviderObjectSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = new ValidationResultNodeId();
			this.propertyBag.ResetChangeTracking();
			this.Task = task;
			this.Detail = result;
			this.ResultType = type;
		}

		private new ObjectId Identity { get; set; }

		private new bool IsValid { get; set; }

		public LocalizedString Task
		{
			get
			{
				return (LocalizedString)this[ValidationResultNodeSchema.Task];
			}
			internal set
			{
				this[ValidationResultNodeSchema.Task] = value;
			}
		}

		public LocalizedString Detail
		{
			get
			{
				return (LocalizedString)this[ValidationResultNodeSchema.Detail];
			}
			internal set
			{
				this[ValidationResultNodeSchema.Detail] = value;
			}
		}

		public ResultType ResultType
		{
			get
			{
				return (ResultType)this[ValidationResultNodeSchema.ResultType];
			}
			internal set
			{
				this[ValidationResultNodeSchema.ResultType] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ValidationResultNode.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} {1} - {2}", this.Task, this.ResultType, this.Detail);
		}

		private static readonly ValidationResultNodeSchema schema = ObjectSchema.GetInstance<ValidationResultNodeSchema>();
	}
}
