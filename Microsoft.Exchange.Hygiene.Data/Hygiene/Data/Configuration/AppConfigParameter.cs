using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Configuration
{
	internal class AppConfigParameter : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				if (this.id == null)
				{
					object obj = this[AppConfigSchema.ParamIdProp];
					if (obj != null)
					{
						this.id = new GenericObjectId((Guid)obj);
					}
				}
				return this.id;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[AppConfigSchema.ParamNameProp];
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length > 255)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this[AppConfigSchema.ParamNameProp] = value;
			}
		}

		public string Value
		{
			get
			{
				return (string)this[AppConfigSchema.ParamValueProp];
			}
			set
			{
				this[AppConfigSchema.ParamValueProp] = value;
			}
		}

		public AppConfigVersion Version
		{
			get
			{
				return new AppConfigVersion((long)this[AppConfigSchema.ParamVersionProp]);
			}
			set
			{
				this[AppConfigSchema.ParamVersionProp] = value.ToInt64();
			}
		}

		public string Description
		{
			get
			{
				return (string)this[AppConfigSchema.DescriptionProp];
			}
			set
			{
				this[AppConfigSchema.DescriptionProp] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(AppConfigSchema);
		}

		private ObjectId id;
	}
}
