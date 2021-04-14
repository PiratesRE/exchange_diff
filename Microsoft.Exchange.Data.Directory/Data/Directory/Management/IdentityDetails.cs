using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class IdentityDetails
	{
		internal IdentityDetails(ADObjectId identity, string name, string displayName, string externalDirectoryObjectId)
		{
			this.Identity = identity;
			this.Name = name;
			this.DisplayName = displayName;
			this.ExternalDirectoryObjectId = externalDirectoryObjectId;
		}

		internal IdentityDetails(ADRawEntry dataObject)
		{
			this.Identity = dataObject.Id;
			this.Name = (string)dataObject[ADObjectSchema.Name];
			this.DisplayName = (string)dataObject[ADRecipientSchema.DisplayName];
			this.ExternalDirectoryObjectId = (string)dataObject[ADRecipientSchema.ExternalDirectoryObjectId];
		}

		public ADObjectId Identity { get; private set; }

		public string Name { get; private set; }

		public string DisplayName { get; private set; }

		public string ExternalDirectoryObjectId { get; private set; }

		public override string ToString()
		{
			return this.Identity.ToString();
		}

		internal static readonly ADPropertyDefinition[] Properties = new ADPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADObjectSchema.Name,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.ExternalDirectoryObjectId
		};
	}
}
