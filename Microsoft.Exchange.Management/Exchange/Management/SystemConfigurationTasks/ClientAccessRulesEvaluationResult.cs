using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class ClientAccessRulesEvaluationResult : ConfigurableObject
	{
		public new ObjectId Identity { get; set; }

		public string Name { get; set; }

		public ClientAccessRulesAction Action { get; set; }

		public ClientAccessRulesEvaluationResult() : base(new SimpleProviderPropertyBag())
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ClientAccessRulesEvaluationResult.Schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		private static readonly ClientAccessRulesEvaluationResultSchema Schema = ObjectSchema.GetInstance<ClientAccessRulesEvaluationResultSchema>();
	}
}
