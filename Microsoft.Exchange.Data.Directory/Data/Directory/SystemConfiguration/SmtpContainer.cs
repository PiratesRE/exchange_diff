using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class SmtpContainer : Container
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return SmtpContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return SmtpContainer.mostDerivedClass;
			}
		}

		private static SmtpContainerSchema schema = ObjectSchema.GetInstance<SmtpContainerSchema>();

		private static string mostDerivedClass = "msExchProtocolCfgSMTPContainer";
	}
}
