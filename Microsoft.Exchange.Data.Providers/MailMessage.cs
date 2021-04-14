using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Providers
{
	[Serializable]
	public class MailMessage : ConfigurableObject
	{
		[Parameter]
		public string Subject
		{
			get
			{
				return (string)this.propertyBag[MailMessageSchema.Subject];
			}
			set
			{
				this.propertyBag[MailMessageSchema.Subject] = value;
			}
		}

		[Parameter]
		public string Body
		{
			get
			{
				return (string)this.propertyBag[MailMessageSchema.Body];
			}
			set
			{
				this.propertyBag[MailMessageSchema.Body] = value;
			}
		}

		[Parameter]
		public MailBodyFormat BodyFormat
		{
			get
			{
				return (MailBodyFormat)this.propertyBag[MailMessageSchema.BodyFormat];
			}
			set
			{
				this.propertyBag[MailMessageSchema.BodyFormat] = value;
			}
		}

		internal void SetIdentity(ObjectId id)
		{
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = id;
		}

		public MailMessage() : base(new MailMessagePropertyBag())
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailMessage.s_schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static ObjectSchema s_schema = ObjectSchema.GetInstance<MailMessageSchema>();
	}
}
