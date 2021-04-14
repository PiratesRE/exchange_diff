using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ToServiceObjectForPropertyBagPropertyList : ToServiceObjectPropertyListBase
	{
		public ToServiceObjectForPropertyBagPropertyList(Shape shape, ResponseShape responseShape) : base(shape, responseShape)
		{
		}

		protected override ToServiceObjectCommandSettingsBase GetCommandSettings()
		{
			return new ToServiceObjectForPropertyBagCommandSettings();
		}

		protected override ToServiceObjectCommandSettingsBase GetCommandSettings(PropertyPath propertyPath)
		{
			return new ToServiceObjectForPropertyBagCommandSettings(propertyPath);
		}

		protected override bool IsPropertyRequiredInShape
		{
			get
			{
				return false;
			}
		}

		protected override bool IsErrorReturnedForInvalidBaseShapeProperty
		{
			get
			{
				return false;
			}
		}

		protected override bool ValidateProperty(PropertyInformation propertyInformation, bool returnErrorForInvalidProperty)
		{
			bool implementsToServiceObjectForPropertyBagCommand = propertyInformation.ImplementsToServiceObjectForPropertyBagCommand;
			if (!implementsToServiceObjectForPropertyBagCommand && returnErrorForInvalidProperty)
			{
				throw new InvalidPropertyForOperationException(propertyInformation.PropertyPath);
			}
			return implementsToServiceObjectForPropertyBagCommand;
		}

		public IList<IToServiceObjectForPropertyBagCommand> CreatePropertyCommands(IDictionary<PropertyDefinition, object> propertyBag, ServiceObject serviceObject, IdAndSession idAndSession, CommandOptions commandOptions)
		{
			List<IToServiceObjectForPropertyBagCommand> list = new List<IToServiceObjectForPropertyBagCommand>();
			foreach (CommandContext commandContext in this.commandContextsOrdered)
			{
				ToServiceObjectForPropertyBagCommandSettings toServiceObjectForPropertyBagCommandSettings = (ToServiceObjectForPropertyBagCommandSettings)commandContext.CommandSettings;
				toServiceObjectForPropertyBagCommandSettings.PropertyBag = propertyBag;
				toServiceObjectForPropertyBagCommandSettings.ServiceObject = serviceObject;
				toServiceObjectForPropertyBagCommandSettings.IdAndSession = idAndSession;
				toServiceObjectForPropertyBagCommandSettings.CommandOptions = commandOptions;
				list.Add((IToServiceObjectForPropertyBagCommand)commandContext.PropertyInformation.CreatePropertyCommand(commandContext));
			}
			return list;
		}

		public ServiceObject ConvertPropertiesToServiceObject(ServiceObject serviceObject, IDictionary<PropertyDefinition, object> propertyBag, IdAndSession idAndSession)
		{
			return this.ConvertPropertiesToServiceObject(serviceObject, propertyBag, idAndSession, CommandOptions.None);
		}

		public ServiceObject ConvertPropertiesToServiceObject(ServiceObject serviceObject, IDictionary<PropertyDefinition, object> propertyBag, IdAndSession idAndSession, CommandOptions commandOptions)
		{
			IList<IToServiceObjectForPropertyBagCommand> list = this.CreatePropertyCommands(propertyBag, serviceObject, idAndSession, commandOptions);
			foreach (IToServiceObjectForPropertyBagCommand toServiceObjectForPropertyBagCommand in list)
			{
				try
				{
					toServiceObjectForPropertyBagCommand.ToServiceObjectForPropertyBag();
				}
				catch (InvalidValueForPropertyException)
				{
					ExTraceGlobals.FindCommandBaseCallTracer.TraceError<string>((long)this.GetHashCode(), "[ToServiceObjectForPropertyBagPropertyList::ConvertPropertiesToServiceObject] Failed to render property to service object.  Property: {0}", toServiceObjectForPropertyBagCommand.ToString());
				}
			}
			return serviceObject;
		}
	}
}
