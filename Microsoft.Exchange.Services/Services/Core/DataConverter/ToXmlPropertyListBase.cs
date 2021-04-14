using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class ToXmlPropertyListBase : PropertyList
	{
		protected ToXmlPropertyListBase()
		{
			this.commandContextsUnordered = new Dictionary<PropertyInformation, List<CommandContext>>();
			this.commandContextsOrdered = new List<CommandContext>();
		}

		public ToXmlPropertyListBase(Shape shape, ResponseShape responseShape) : base(shape)
		{
			this.responseShape = responseShape;
			this.commandContextsUnordered = new Dictionary<PropertyInformation, List<CommandContext>>();
			this.commandContextsOrdered = new List<CommandContext>();
			this.SelectProperties();
			this.SequencePropertyList(shape);
		}

		public ResponseShape ResponseShape
		{
			get
			{
				return this.responseShape;
			}
		}

		private void SelectProperties()
		{
			this.AddBaseShapeProperties(this.responseShape.BaseShape);
			this.AddAdditionalProperties(this.responseShape.AdditionalProperties);
			ItemResponseShape itemResponseShape = this.responseShape as ItemResponseShape;
			if (itemResponseShape != null && itemResponseShape.IncludeMimeContent)
			{
				this.Add(ItemSchema.MimeContent, this.GetCommandSettings(), true);
			}
		}

		private void AddBaseShapeProperties(ShapeEnum shapeEnum)
		{
			if (shapeEnum == ShapeEnum.Default)
			{
				this.Add(this.shape.DefaultProperties);
				return;
			}
			this.AddFromSchemaPropertyList(this.shape, shapeEnum);
		}

		private void AddFromSchemaPropertyList(Shape shapeToAddFrom, ShapeEnum shapeEnum)
		{
			if (shapeToAddFrom.InnerShape != null)
			{
				this.AddFromSchemaPropertyList(shapeToAddFrom.InnerShape, shapeEnum);
			}
			this.Add(shapeToAddFrom.Schema.GetPropertyInformationListByShapeEnum(shapeEnum));
		}

		private void AddAdditionalProperties(PropertyPath[] additionalProperties)
		{
			if (additionalProperties != null)
			{
				foreach (PropertyPath propertyPath in additionalProperties)
				{
					PropertyInformation propertyInformation = null;
					if (this.shape.TryGetPropertyInformation(propertyPath, out propertyInformation))
					{
						if (!ExchangeVersion.Current.Supports(propertyInformation.EffectiveVersion))
						{
							throw new InvalidPropertyRequestException(propertyPath);
						}
						this.Add(propertyInformation, this.GetCommandSettings(propertyPath), true);
					}
					else if (this.IsPropertyRequiredInShape && !ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010SP1))
					{
						throw new InvalidPropertyRequestException(propertyPath);
					}
				}
			}
		}

		private void Add(PropertyInformation propertyInformation, CommandSettings commandSettings, bool returnErrorForInvalidProperty)
		{
			List<CommandContext> list = null;
			if (this.ValidateProperty(propertyInformation, returnErrorForInvalidProperty))
			{
				if (!this.commandContextsUnordered.TryGetValue(propertyInformation, out list))
				{
					list = new List<CommandContext>();
					list.Add(new CommandContext(commandSettings, propertyInformation));
					this.commandContextsUnordered.Add(propertyInformation, list);
					return;
				}
				if (propertyInformation.SupportsMultipleInstancesForToXml)
				{
					list.Add(new CommandContext(commandSettings, propertyInformation));
				}
			}
		}

		private void Add(IList<PropertyInformation> properties)
		{
			foreach (PropertyInformation propertyInformation in properties)
			{
				if (ExchangeVersion.Current.Supports(propertyInformation.EffectiveVersion))
				{
					this.Add(propertyInformation, this.GetCommandSettings(), this.IsErrorReturnedForInvalidBaseShapeProperty);
				}
			}
		}

		private void SequencePropertyList(Shape shape)
		{
			if (shape.InnerShape != null && !this.IsInSequence)
			{
				this.SequencePropertyList(shape.InnerShape);
			}
			this.SequenceCommands(shape.Schema);
		}

		private void SequenceCommands(Schema schema)
		{
			IList<PropertyInformation> propertyInformationInXmlSchemaSequence = schema.PropertyInformationInXmlSchemaSequence;
			foreach (PropertyInformation key in propertyInformationInXmlSchemaSequence)
			{
				List<CommandContext> list = null;
				if (this.commandContextsUnordered.TryGetValue(key, out list))
				{
					foreach (CommandContext item in list)
					{
						this.commandContextsOrdered.Add(item);
					}
					this.commandContextsUnordered.Remove(key);
				}
			}
		}

		private bool IsInSequence
		{
			get
			{
				return this.commandContextsUnordered.Count == 0 && this.commandContextsOrdered.Count >= 0;
			}
		}

		public PropertyDefinition[] GetPropertyDefinitions()
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			foreach (CommandContext commandContext in this.commandContextsOrdered)
			{
				PropertyDefinition[] propertyDefinitions = commandContext.GetPropertyDefinitions();
				if (propertyDefinitions != null && propertyDefinitions.Length > 0)
				{
					list.AddRange(propertyDefinitions);
				}
			}
			return list.ToArray();
		}

		public void FilterProperties(List<PropertyPath> allowedProperties)
		{
			foreach (CommandContext commandContext in this.commandContextsOrdered.ToArray())
			{
				PropertyPath propertyPath = commandContext.PropertyInformation.PropertyPath;
				if (!allowedProperties.Contains(propertyPath))
				{
					this.commandContextsOrdered.Remove(commandContext);
				}
			}
		}

		protected virtual bool IsPropertyRequiredInShape
		{
			get
			{
				return true;
			}
		}

		protected virtual bool IsErrorReturnedForInvalidBaseShapeProperty
		{
			get
			{
				return true;
			}
		}

		protected abstract ToXmlCommandSettingsBase GetCommandSettings();

		protected abstract ToXmlCommandSettingsBase GetCommandSettings(PropertyPath propertyPath);

		protected abstract bool ValidateProperty(PropertyInformation propertyInformation, bool returnErrorForInvalidProperty);

		public XmlElement CreateItemXmlElement(XmlDocument ownerDocument)
		{
			return this.shape.CreateItemXmlElement(ownerDocument);
		}

		public XmlElement CreateItemXmlElement(XmlElement parentElement)
		{
			return this.shape.CreateItemXmlElement(parentElement);
		}

		private Dictionary<PropertyInformation, List<CommandContext>> commandContextsUnordered;

		protected List<CommandContext> commandContextsOrdered;

		protected ResponseShape responseShape;
	}
}
