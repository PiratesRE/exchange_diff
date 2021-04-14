using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public class UMDiagnosticObject : ConfigurableObject
	{
		internal UMDiagnosticObject() : base(new SimpleProviderPropertyBag())
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return UMDiagnosticObject.schema;
			}
		}

		private static UMDiagnosticObject.UMDiagnosticObjectSchema schema = new UMDiagnosticObject.UMDiagnosticObjectSchema();

		private class UMDiagnosticObjectSchema : SimpleProviderObjectSchema
		{
		}
	}
}
