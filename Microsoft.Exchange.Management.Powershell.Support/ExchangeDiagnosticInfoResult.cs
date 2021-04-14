using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public class ExchangeDiagnosticInfoResult : ConfigurableObject
	{
		internal ExchangeDiagnosticInfoResult(string result) : base(new SimpleProviderPropertyBag())
		{
			this.Result = result;
		}

		public ExchangeDiagnosticInfoResult() : base(new SimpleProviderPropertyBag())
		{
			this.Result = string.Empty;
		}

		public string Result
		{
			get
			{
				return (string)this.propertyBag[ExchangeDiagnosticInfoResult.Schema.Result];
			}
			internal set
			{
				this.propertyBag[ExchangeDiagnosticInfoResult.Schema.Result] = value;
			}
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
				return ExchangeDiagnosticInfoResult.SchemaInstance;
			}
		}

		public override string ToString()
		{
			return this.Result;
		}

		private static readonly ExchangeDiagnosticInfoResult.Schema SchemaInstance = ObjectSchema.GetInstance<ExchangeDiagnosticInfoResult.Schema>();

		internal class Schema : SimpleProviderObjectSchema
		{
			internal static readonly SimpleProviderPropertyDefinition Result = new SimpleProviderPropertyDefinition("Result", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
