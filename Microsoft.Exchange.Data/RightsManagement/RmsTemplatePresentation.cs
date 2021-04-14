using System;
using System.Globalization;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.RightsManagement
{
	[Serializable]
	public sealed class RmsTemplatePresentation : ConfigurableObject
	{
		public RmsTemplatePresentation() : this(new RmsTemplateIdentity())
		{
		}

		public RmsTemplatePresentation(RmsTemplateIdentity identity) : base(new SimpleProviderPropertyBag())
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.SetIsReadOnly(true);
			this.propertyBag.SetField(SimpleProviderObjectSchema.Identity, identity);
		}

		internal RmsTemplatePresentation(RmsTemplate template) : base(new SimpleProviderPropertyBag())
		{
			if (template == null)
			{
				throw new ArgumentNullException("template");
			}
			this.propertyBag.SetField(RmsTemplatePresentationSchema.Name, template.GetName(CultureInfo.CurrentUICulture));
			this.propertyBag.SetField(RmsTemplatePresentationSchema.Description, template.GetDescription(CultureInfo.CurrentUICulture));
			this.propertyBag.SetField(RmsTemplatePresentationSchema.Type, template.Type);
			this.propertyBag.SetField(RmsTemplatePresentationSchema.TemplateGuid, template.Id);
			this.propertyBag.SetField(SimpleProviderObjectSchema.Identity, new RmsTemplateIdentity(template.Id, template.Name, template.Type));
		}

		public string Name
		{
			get
			{
				return (string)this[RmsTemplatePresentationSchema.Name];
			}
		}

		public string Description
		{
			get
			{
				return (string)this[RmsTemplatePresentationSchema.Description];
			}
		}

		public RmsTemplateType Type
		{
			get
			{
				return (RmsTemplateType)this[RmsTemplatePresentationSchema.Type];
			}
			set
			{
				this[RmsTemplatePresentationSchema.Type] = value;
			}
		}

		public Guid TemplateGuid
		{
			get
			{
				return (Guid)this[RmsTemplatePresentationSchema.TemplateGuid];
			}
		}

		public override ObjectId Identity
		{
			get
			{
				ObjectId objectId = base.Identity;
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					objectId = (ObjectId)SuppressingPiiProperty.TryRedact(SimpleProviderObjectSchema.Identity, objectId);
				}
				return objectId;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return RmsTemplatePresentation.schema;
			}
		}

		private static readonly RmsTemplatePresentationSchema schema = new RmsTemplatePresentationSchema();
	}
}
