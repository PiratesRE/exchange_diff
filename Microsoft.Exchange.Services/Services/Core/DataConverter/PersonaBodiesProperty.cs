using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class PersonaBodiesProperty : BodyProperty, IToServiceObjectForPropertyBagCommand, IToXmlForPropertyBagCommand, IPropertyCommand
	{
		protected PersonaBodiesProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static PersonaBodiesProperty CreateCommand(CommandContext commandContext)
		{
			return new PersonaBodiesProperty(commandContext);
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("Do not call this. It's obsolete");
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			PropertyDefinition propertyDefinition = this.commandContext.GetPropertyDefinitions()[0];
			object obj = null;
			if (propertyDefinition != null && PropertyCommand.TryGetValueFromPropertyBag<object>(propertyBag, propertyDefinition, out obj))
			{
				ArrayPropertyInformation arrayPropertyInformation = propertyInformation as ArrayPropertyInformation;
				if (arrayPropertyInformation != null && obj != null)
				{
					object[] array = obj as object[];
					if (array != null && array.Length > 0)
					{
						object servicePropertyValue = this.GetServicePropertyValue(array[0]);
						if (servicePropertyValue != null)
						{
							Array array2 = Array.CreateInstance(servicePropertyValue.GetType(), array.Length);
							array2.SetValue(servicePropertyValue, 0);
							for (int i = 1; i < array.Length; i++)
							{
								array2.SetValue(this.GetServicePropertyValue(array[i]), i);
							}
							serviceObject[propertyInformation] = array2;
						}
					}
				}
			}
		}

		protected override BodyFormat ComputeBodyFormat(BodyResponseType bodyType, Item item)
		{
			return BodyFormat.TextPlain;
		}

		private object GetServicePropertyValue(object propertyValue)
		{
			object result = null;
			if (propertyValue != null)
			{
				AttributedValue<PersonNotes> attributedValue = (AttributedValue<PersonNotes>)propertyValue;
				if (attributedValue.Value != null)
				{
					BodyContentAttributedValue bodyContentAttributedValue = new BodyContentAttributedValue
					{
						Attributions = attributedValue.Attributions,
						Value = new BodyContentType
						{
							BodyType = BodyType.Text,
							Value = attributedValue.Value.NotesBody,
							IsTruncated = attributedValue.Value.IsTruncated
						}
					};
					bool encodeStringProperties;
					if (CallContext.Current == null)
					{
						encodeStringProperties = Global.EncodeStringProperties;
					}
					else
					{
						encodeStringProperties = CallContext.Current.EncodeStringProperties;
					}
					if (encodeStringProperties && ExchangeVersion.Current.Supports(ExchangeVersionType.Exchange2012))
					{
						string value = bodyContentAttributedValue.Value.Value;
						if (value != null)
						{
							bodyContentAttributedValue.Value.Value = Util.EncodeForAntiXSS(value);
						}
					}
					result = bodyContentAttributedValue;
				}
			}
			return result;
		}
	}
}
