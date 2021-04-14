using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class EffectiveRightsProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		public EffectiveRightsProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static EffectiveRightsProperty CreateCommand(CommandContext commandContext)
		{
			return new EffectiveRightsProperty(commandContext);
		}

		public static EffectiveRightsType GetFromEffectiveRights(EffectiveRights effectiveRights, IStoreSession session)
		{
			bool? viewPrivateItems = EffectiveRightsProperty.GetViewPrivateItems(session);
			EffectiveRightsType effectiveRightsType = new EffectiveRightsType();
			effectiveRightsType.CreateAssociated = EffectiveRightsProperty.IsSet(effectiveRights, EffectiveRights.CreateAssociated);
			effectiveRightsType.CreateContents = EffectiveRightsProperty.IsSet(effectiveRights, EffectiveRights.CreateContents);
			effectiveRightsType.CreateHierarchy = EffectiveRightsProperty.IsSet(effectiveRights, EffectiveRights.CreateHierarchy);
			effectiveRightsType.Delete = EffectiveRightsProperty.IsSet(effectiveRights, EffectiveRights.Delete);
			effectiveRightsType.Modify = EffectiveRightsProperty.IsSet(effectiveRights, EffectiveRights.Modify);
			effectiveRightsType.Read = EffectiveRightsProperty.IsSet(effectiveRights, EffectiveRights.Read);
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010SP1) && viewPrivateItems != null)
			{
				effectiveRightsType.ViewPrivateItems = new bool?(viewPrivateItems.Value);
			}
			return effectiveRightsType;
		}

		public void ToXml()
		{
			throw new InvalidOperationException("EffectiveRightsProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("EffectiveRightsProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			EffectiveRights effectiveRights;
			try
			{
				effectiveRights = (EffectiveRights)storeObject[this.propertyDefinitions[0]];
			}
			catch (PropertyErrorException)
			{
				effectiveRights = EffectiveRights.None;
			}
			this.RenderEffectiveRights(serviceObject, effectiveRights, storeObject.Session);
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			int effectiveRights;
			if (PropertyCommand.TryGetValueFromPropertyBag<int>(propertyBag, this.propertyDefinitions[0], out effectiveRights))
			{
				this.RenderEffectiveRights(serviceObject, (EffectiveRights)effectiveRights, commandSettings.IdAndSession.Session);
			}
		}

		private static bool IsSet(EffectiveRights effectiveRights, EffectiveRights flag)
		{
			return flag == (effectiveRights & flag);
		}

		private static bool? GetViewPrivateItems(IStoreSession storeSession)
		{
			if (storeSession is MailboxSession)
			{
				MailboxSession mailboxSession = (MailboxSession)storeSession;
				bool privateItemsFilterDisabled = mailboxSession.PrivateItemsFilterDisabled;
				bool value;
				try
				{
					if (privateItemsFilterDisabled)
					{
						mailboxSession.EnablePrivateItemsFilter();
					}
					value = !mailboxSession.FilterPrivateItems;
				}
				finally
				{
					if (privateItemsFilterDisabled)
					{
						mailboxSession.DisablePrivateItemsFilter();
					}
				}
				return new bool?(value);
			}
			return null;
		}

		private void RenderEffectiveRights(ServiceObject serviceObject, EffectiveRights effectiveRights, StoreSession session)
		{
			serviceObject[this.commandContext.PropertyInformation] = EffectiveRightsProperty.GetFromEffectiveRights(effectiveRights, session);
		}
	}
}
