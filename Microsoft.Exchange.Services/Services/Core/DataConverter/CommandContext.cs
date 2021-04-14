using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class CommandContext
	{
		public CommandContext(CommandSettings commandSettings, PropertyInformation propertyInformation)
		{
			this.commandSettings = commandSettings;
			this.propertyInformation = propertyInformation;
		}

		public CommandContext(CommandSettings commandSettings, PropertyInformation propertyInformation, IdConverter idConverter) : this(commandSettings, propertyInformation)
		{
			this.idConverter = idConverter;
		}

		public PropertyDefinition[] GetPropertyDefinitions()
		{
			if (this.propertyDefinitions == null)
			{
				this.propertyDefinitions = this.propertyInformation.GetPropertyDefinitions(this.commandSettings);
			}
			return this.propertyDefinitions;
		}

		public CommandSettings CommandSettings
		{
			get
			{
				return this.commandSettings;
			}
		}

		public PropertyInformation PropertyInformation
		{
			get
			{
				return this.propertyInformation;
			}
		}

		public IdConverter IdConverter
		{
			get
			{
				return this.idConverter;
			}
		}

		public override string ToString()
		{
			if (this.propertyInformation == null)
			{
				return base.GetType().Name;
			}
			if (this.propertyInformation.PropertyPath == null)
			{
				return base.GetType().Name;
			}
			PropertyUri propertyUri = this.propertyInformation.PropertyPath as PropertyUri;
			if (propertyUri != null)
			{
				return propertyUri.UriString;
			}
			if (this.GetPropertyDefinitions().Length > 0)
			{
				return this.GetPropertyDefinitions()[0].ToString();
			}
			return base.GetType().Name;
		}

		private CommandSettings commandSettings;

		private PropertyInformation propertyInformation;

		private PropertyDefinition[] propertyDefinitions;

		private IdConverter idConverter;
	}
}
