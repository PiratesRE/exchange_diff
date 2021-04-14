using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class CasTransactionResult : ConfigurableObject
	{
		internal CasTransactionResult(CasTransactionResultEnum result) : base(new CasTransactionPropertyBag())
		{
			this.Value = result;
		}

		public CasTransactionResultEnum Value
		{
			get
			{
				return (CasTransactionResultEnum)this.propertyBag[CasTransactionResultSchema.Value];
			}
			internal set
			{
				this.propertyBag[CasTransactionResultSchema.Value] = value;
			}
		}

		public override string ToString()
		{
			string result = string.Empty;
			switch (this.Value)
			{
			case CasTransactionResultEnum.Undefined:
				result = Strings.CasTransactionResultUndefined;
				break;
			case CasTransactionResultEnum.Success:
				result = Strings.CasTransactionResultSuccess;
				break;
			case CasTransactionResultEnum.Failure:
				result = Strings.CasTransactionResultFailure;
				break;
			case CasTransactionResultEnum.Skipped:
				result = Strings.CasTransactionResultSkipped;
				break;
			default:
				throw new CasTransactionResultToStringCaseNotHandledException(this.Value);
			}
			return result;
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return CasTransactionResult.schema;
			}
		}

		private static CasTransactionResultSchema schema = ObjectSchema.GetInstance<CasTransactionResultSchema>();
	}
}
