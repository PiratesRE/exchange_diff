using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class AttachmentFilteringConfig : ADConfigurationObject
	{
		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return AttachmentFilteringConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AttachmentFilteringConfig.mostDerivedClass;
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(AttachmentFilteringConfigSchema.RejectResponse))
			{
				this[AttachmentFilteringConfigSchema.RejectResponse] = "Message rejected due to unacceptable attachments";
			}
			if (!base.IsModified(AttachmentFilteringConfigSchema.AdminMessage))
			{
				this[AttachmentFilteringConfigSchema.AdminMessage] = DirectoryStrings.AttachmentsWereRemovedMessage.ToString();
			}
			base.StampPersistableDefaultValues();
		}

		[Parameter]
		public string RejectResponse
		{
			get
			{
				return (string)this[AttachmentFilteringConfigSchema.RejectResponse];
			}
			set
			{
				this[AttachmentFilteringConfigSchema.RejectResponse] = value;
			}
		}

		[Parameter]
		public string AdminMessage
		{
			get
			{
				return (string)this[AttachmentFilteringConfigSchema.AdminMessage];
			}
			set
			{
				this[AttachmentFilteringConfigSchema.AdminMessage] = value;
			}
		}

		[Parameter]
		public FilterActions Action
		{
			get
			{
				return (FilterActions)this[AttachmentFilteringConfigSchema.FilterAction];
			}
			set
			{
				this[AttachmentFilteringConfigSchema.FilterAction] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ExceptionConnectors
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[AttachmentFilteringConfigSchema.ExceptionConnectors];
			}
			set
			{
				this[AttachmentFilteringConfigSchema.ExceptionConnectors] = value;
			}
		}

		public MultiValuedProperty<string> AttachmentNames
		{
			get
			{
				return (MultiValuedProperty<string>)this[AttachmentFilteringConfigSchema.AttachmentNames];
			}
			internal set
			{
				this[AttachmentFilteringConfigSchema.AttachmentNames] = value;
			}
		}

		private static AttachmentFilteringConfigSchema schema = ObjectSchema.GetInstance<AttachmentFilteringConfigSchema>();

		private static string mostDerivedClass = "msExchTransportSettings";
	}
}
