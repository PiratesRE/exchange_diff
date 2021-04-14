using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ToXmlForPropertyBagPropertyList : ToXmlPropertyListBase
	{
		public ToXmlForPropertyBagPropertyList(Shape shape, ResponseShape responseShape) : base(shape, responseShape)
		{
		}

		protected override ToXmlCommandSettingsBase GetCommandSettings()
		{
			return new ToXmlForPropertyBagCommandSettings();
		}

		protected override ToXmlCommandSettingsBase GetCommandSettings(PropertyPath propertyPath)
		{
			return new ToXmlForPropertyBagCommandSettings(propertyPath);
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
			bool implementsToXmlForPropertyBagCommand = propertyInformation.ImplementsToXmlForPropertyBagCommand;
			if (!implementsToXmlForPropertyBagCommand && returnErrorForInvalidProperty)
			{
				throw new InvalidPropertyForOperationException(propertyInformation.PropertyPath);
			}
			return implementsToXmlForPropertyBagCommand;
		}

		public IList<IToXmlForPropertyBagCommand> CreatePropertyCommands(IDictionary<PropertyDefinition, object> propertyBag, XmlElement serviceItem, IdAndSession idAndSession)
		{
			List<IToXmlForPropertyBagCommand> list = new List<IToXmlForPropertyBagCommand>();
			foreach (CommandContext commandContext in this.commandContextsOrdered)
			{
				ToXmlForPropertyBagCommandSettings toXmlForPropertyBagCommandSettings = (ToXmlForPropertyBagCommandSettings)commandContext.CommandSettings;
				toXmlForPropertyBagCommandSettings.PropertyBag = propertyBag;
				toXmlForPropertyBagCommandSettings.ServiceItem = serviceItem;
				toXmlForPropertyBagCommandSettings.IdAndSession = idAndSession;
				list.Add((IToXmlForPropertyBagCommand)commandContext.PropertyInformation.CreatePropertyCommand(commandContext));
			}
			return list;
		}

		public XmlElement ConvertPropertiesToXml(XmlDocument ownerDocument, IDictionary<PropertyDefinition, object> propertyBag, IdAndSession idAndSession)
		{
			XmlElement xmlElement = base.CreateItemXmlElement(ownerDocument);
			IList<IToXmlForPropertyBagCommand> list = this.CreatePropertyCommands(propertyBag, xmlElement, idAndSession);
			foreach (IToXmlForPropertyBagCommand toXmlForPropertyBagCommand in list)
			{
				try
				{
					toXmlForPropertyBagCommand.ToXmlForPropertyBag();
				}
				catch (InvalidValueForPropertyException)
				{
					ExTraceGlobals.FindCommandBaseCallTracer.TraceError<string>((long)this.GetHashCode(), "[ToXmlForPropertyBagPropertyList::ConvertPropertiesToXml] Failed to render property as xml.  Property: {0}", toXmlForPropertyBagCommand.ToString());
				}
			}
			return xmlElement;
		}
	}
}
