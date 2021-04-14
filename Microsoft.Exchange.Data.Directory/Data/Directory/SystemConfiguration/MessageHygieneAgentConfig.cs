using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public abstract class MessageHygieneAgentConfig : ADConfigurationObject
	{
		public MessageHygieneAgentConfig()
		{
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MessageHygieneAgentConfig.schema;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return MessageHygieneAgentConfig.parentPath;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this[MessageHygieneAgentConfigSchema.Enabled];
			}
			set
			{
				this[MessageHygieneAgentConfigSchema.Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExternalMailEnabled
		{
			get
			{
				return (bool)this[MessageHygieneAgentConfigSchema.ExternalMailEnabled];
			}
			set
			{
				this[MessageHygieneAgentConfigSchema.ExternalMailEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InternalMailEnabled
		{
			get
			{
				return (bool)this[MessageHygieneAgentConfigSchema.InternalMailEnabled];
			}
			set
			{
				this[MessageHygieneAgentConfigSchema.InternalMailEnabled] = value;
			}
		}

		protected void ValidateMaximumCollectionCount(List<ValidationError> errors, ICollection collection, int maximum, PropertyDefinition property)
		{
			if (collection != null && property != null)
			{
				int count = collection.Count;
				if (count > maximum)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ExceededMaximumCollectionCount(property.Name, maximum, count), property, count));
				}
			}
		}

		private static ADObjectSchema schema = ObjectSchema.GetInstance<MessageHygieneAgentConfigSchema>();

		private static ADObjectId parentPath = new ADObjectId("CN=Message Hygiene,CN=Transport Settings");
	}
}
