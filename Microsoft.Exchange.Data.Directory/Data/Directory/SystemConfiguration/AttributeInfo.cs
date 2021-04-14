using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	internal class AttributeInfo
	{
		internal int MapiID
		{
			get
			{
				return this.mapiID;
			}
		}

		internal DetailsTemplateControl.AttributeControlTypes ControlType
		{
			get
			{
				return this.controlType;
			}
		}

		internal bool this[string templateType]
		{
			get
			{
				bool flag;
				return this.templateTypes.TryGetValue(templateType, out flag) && flag;
			}
			set
			{
				if (value)
				{
					this.templateTypes[templateType] = true;
					return;
				}
				this.templateTypes[templateType] = false;
			}
		}

		internal AttributeInfo(int mapiID, DetailsTemplateControl.AttributeControlTypes controlType)
		{
			this.mapiID = mapiID;
			this.templateTypes = new Dictionary<string, bool>();
			this.controlType = controlType;
		}

		private Dictionary<string, bool> templateTypes;

		private int mapiID;

		private DetailsTemplateControl.AttributeControlTypes controlType;
	}
}
