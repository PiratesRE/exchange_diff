using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public sealed class ReplicationCheckResult : ConfigurableObject, IEquatable<ReplicationCheckResult>
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ReplicationCheckResult.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal ReplicationCheckResult(ReplicationCheckResultEnum result) : base(new SimpleProviderPropertyBag())
		{
			this.Value = result;
		}

		public ReplicationCheckResultEnum Value
		{
			get
			{
				return (ReplicationCheckResultEnum)this[ReplicationCheckResultSchema.Value];
			}
			private set
			{
				this[ReplicationCheckResultSchema.Value] = value;
			}
		}

		public override string ToString()
		{
			string result = string.Empty;
			switch (this.Value)
			{
			case ReplicationCheckResultEnum.Undefined:
				result = Strings.ReplicationCheckResultUndefined;
				break;
			case ReplicationCheckResultEnum.Passed:
				result = Strings.ReplicationCheckResultPassed;
				break;
			case ReplicationCheckResultEnum.Warning:
				result = Strings.ReplicationCheckResultWarning;
				break;
			case ReplicationCheckResultEnum.Failed:
				result = Strings.ReplicationCheckResultFailed;
				break;
			default:
				throw new ReplicationCheckResultToStringCaseNotHandled(this.Value);
			}
			return result;
		}

		bool IEquatable<ReplicationCheckResult>.Equals(ReplicationCheckResult other)
		{
			return this.Value == other.Value;
		}

		private static ReplicationCheckResultSchema schema = ObjectSchema.GetInstance<ReplicationCheckResultSchema>();
	}
}
