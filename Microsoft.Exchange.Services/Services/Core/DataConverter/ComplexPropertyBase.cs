using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class ComplexPropertyBase : PropertyCommand
	{
		internal ComplexPropertyBase(CommandContext commandContext) : base(commandContext)
		{
			this.propertyDefinitions = commandContext.GetPropertyDefinitions();
		}

		protected PropertyDefinition[] propertyDefinitions;
	}
}
