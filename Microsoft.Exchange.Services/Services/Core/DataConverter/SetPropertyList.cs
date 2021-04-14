using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class SetPropertyList : PropertyList
	{
		public SetPropertyList(Shape shape, ServiceObject serviceObject, StoreObject storeObject, IdConverter idConverter) : base(shape)
		{
			this.serviceObject = serviceObject;
			this.storeObject = storeObject;
			this.CreatePropertyList(idConverter);
		}

		private void CreatePropertyList(IdConverter idConverter)
		{
			this.commandContexts = new List<CommandContext>();
			this.BuildPropertyListFromShape(this.shape, idConverter);
		}

		private void BuildPropertyList(ServiceObject serviceObject, string path, Shape shape, IdConverter idConverter)
		{
			foreach (PropertyInformation propertyInformation in serviceObject.LoadedProperties)
			{
				PropertyInformation propertyInformation2;
				if (shape.Schema.TryGetPropertyInformationByPath(propertyInformation.PropertyPath, out propertyInformation2))
				{
					if (!propertyInformation.ImplementsSetCommand)
					{
						throw new InvalidPropertySetException(propertyInformation.PropertyPath);
					}
					this.commandContexts.Add(new CommandContext(new SetCommandSettings(serviceObject, this.storeObject), propertyInformation, idConverter));
				}
			}
		}

		public IList<ISetCommand> CreatePropertyCommands()
		{
			List<ISetCommand> list = new List<ISetCommand>();
			foreach (CommandContext commandContext in this.commandContexts)
			{
				list.Add((ISetCommand)commandContext.PropertyInformation.CreatePropertyCommand(commandContext));
			}
			return list;
		}

		public SetPropertyList(Shape shape, XmlElement serviceItem, StoreObject storeObject, IdConverter idConverter) : base(shape)
		{
			this.serviceItem = serviceItem;
			this.storeObject = storeObject;
			this.CreatePropertyList(idConverter);
		}

		private void BuildPropertyListFromShape(Shape shape, IdConverter idConverter)
		{
			if (shape.InnerShape != null)
			{
				this.BuildPropertyListFromShape(shape.InnerShape, idConverter);
			}
			if (this.serviceObject != null)
			{
				this.BuildPropertyList(this.serviceObject, Schema.RootXmlElementPath, shape, idConverter);
				return;
			}
			this.BuildPropertyList(this.serviceItem, Schema.RootXmlElementPath, shape, idConverter);
		}

		private void BuildPropertyList(XmlElement xmlContainer, string path, Shape shape, IdConverter idConverter)
		{
			foreach (object obj in xmlContainer.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement != null)
				{
					string path2 = Schema.BuildXmlElementPath(xmlElement, path);
					XmlElementInformation xmlElementInformation = null;
					if (shape.Schema.TryGetXmlElementInformationByPath(path2, out xmlElementInformation))
					{
						PropertyInformation propertyInformation = xmlElementInformation as PropertyInformation;
						if (propertyInformation != null)
						{
							if (!propertyInformation.ImplementsSetCommand)
							{
								throw new InvalidPropertySetException(propertyInformation.PropertyPath);
							}
							this.commandContexts.Add(new CommandContext(new SetCommandSettings(xmlElement, this.storeObject), propertyInformation, idConverter));
						}
						else if (xmlElementInformation is ContainerInformation)
						{
							this.BuildPropertyList(xmlElement, path2, shape, idConverter);
						}
					}
				}
			}
		}

		private List<CommandContext> commandContexts;

		private ServiceObject serviceObject;

		private StoreObject storeObject;

		private XmlElement serviceItem;
	}
}
