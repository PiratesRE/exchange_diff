using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class MAPITypeConverter : TypeConverter
	{
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			if (context != null)
			{
				DetailsTemplateTypeService detailsTemplateTypeService = (DetailsTemplateTypeService)context.GetService(typeof(DetailsTemplateTypeService));
				IDetailsTemplateControlBound detailsTemplateControlBound = context.Instance as IDetailsTemplateControlBound;
				if (detailsTemplateTypeService != null && detailsTemplateTypeService.TemplateType != null && detailsTemplateTypeService.MAPIPropertiesDictionary != null && detailsTemplateControlBound != null && detailsTemplateControlBound.DetailsTemplateControl != null && detailsTemplateTypeService.MAPIPropertiesDictionary.GetControlMAPIAttributes(detailsTemplateTypeService.TemplateType, detailsTemplateControlBound.DetailsTemplateControl.GetAttributeControlType()) != null)
				{
					return true;
				}
			}
			return false;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			DetailsTemplateTypeService detailsTemplateTypeService = (DetailsTemplateTypeService)context.GetService(typeof(DetailsTemplateTypeService));
			DetailsTemplateControl detailsTemplateControl = (context.Instance as IDetailsTemplateControlBound).DetailsTemplateControl;
			ICollection<string> controlMAPIAttributes = detailsTemplateTypeService.MAPIPropertiesDictionary.GetControlMAPIAttributes(detailsTemplateTypeService.TemplateType, detailsTemplateControl.GetAttributeControlType());
			return new TypeConverter.StandardValuesCollection(controlMAPIAttributes as ICollection);
		}
	}
}
