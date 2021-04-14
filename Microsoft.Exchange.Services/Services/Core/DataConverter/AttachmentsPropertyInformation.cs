using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class AttachmentsPropertyInformation : PropertyInformation
	{
		public AttachmentsPropertyInformation() : base("Attachments", ExchangeVersion.Exchange2007, null, new PropertyUri(PropertyUriEnum.Attachments), new PropertyCommand.CreatePropertyCommand(AttachmentsProperty.CreateCommand))
		{
			this.exchange2010SP2PropertyInformationAttributes = base.PreparePropertyInformationAttributes(typeof(SetEnabledAttachmentsProperty), PropertyInformationAttributes.None);
		}

		public override PropertyCommand.CreatePropertyCommand CreatePropertyCommand
		{
			get
			{
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010SP2))
				{
					return new PropertyCommand.CreatePropertyCommand(SetEnabledAttachmentsProperty.CreateCommand);
				}
				return base.CreatePropertyCommand;
			}
		}

		public override PropertyInformationAttributes PropertyInformationAttributes
		{
			get
			{
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010SP2))
				{
					return this.exchange2010SP2PropertyInformationAttributes;
				}
				return base.PropertyInformationAttributes;
			}
		}

		private readonly PropertyInformationAttributes exchange2010SP2PropertyInformationAttributes;
	}
}
