using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class CustomPropertyHandlers
	{
		internal Dictionary<string, CustomPropertyHandlerBase> Handlers { get; private set; }

		private CustomPropertyHandlers()
		{
			this.Handlers = new Dictionary<string, CustomPropertyHandlerBase>(StringComparer.InvariantCultureIgnoreCase);
			this.Handlers.Add(ADRecipientSchema.PoliciesIncluded.LdapDisplayName, StringObjectGuidHandler.Instance);
			this.Handlers.Add(ADRecipientSchema.PoliciesExcluded.LdapDisplayName, StringObjectGuidHandler.Instance);
			this.Handlers.Add("msExchTargetServerAdmins", BinaryObjectGuidHandler.Instance);
			this.Handlers.Add("msExchTargetServerViewOnlyAdmins", BinaryObjectGuidHandler.Instance);
			this.Handlers.Add("msExchTargetServerPartnerAdmins", BinaryObjectGuidHandler.Instance);
			this.Handlers.Add("msExchTargetServerPartnerViewOnlyAdmins", BinaryObjectGuidHandler.Instance);
		}

		internal static CustomPropertyHandlers Instance
		{
			get
			{
				if (CustomPropertyHandlers.instance == null)
				{
					CustomPropertyHandlers.instance = new CustomPropertyHandlers();
				}
				return CustomPropertyHandlers.instance;
			}
		}

		private static CustomPropertyHandlers instance;
	}
}
