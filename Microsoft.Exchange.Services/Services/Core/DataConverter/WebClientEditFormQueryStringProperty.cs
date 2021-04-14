using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class WebClientEditFormQueryStringProperty : WebClientQueryStringPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IPropertyCommand
	{
		private WebClientEditFormQueryStringProperty(CommandContext commandContext) : base(commandContext)
		{
			this.isDraftProperty = this.propertyDefinitions[3];
		}

		public static WebClientEditFormQueryStringProperty CreateCommand(CommandContext commandContext)
		{
			return new WebClientEditFormQueryStringProperty(commandContext);
		}

		protected override bool ValidateProperty(StoreObject storeObject, string className, bool isPublic)
		{
			bool isDraft = (bool)PropertyCommand.GetPropertyValueFromStoreObject(storeObject, this.isDraftProperty);
			return this.IsEditable(className, isDraft, isPublic);
		}

		protected override bool ValidatePropertyFromPropertyBag(IDictionary<PropertyDefinition, object> propertyBag, string className, bool isPublic)
		{
			bool isDraft;
			return PropertyCommand.TryGetValueFromPropertyBag<bool>(propertyBag, this.isDraftProperty, out isDraft) && this.IsEditable(className, isDraft, isPublic);
		}

		protected override string GetOwaViewModel(string className)
		{
			if (className != null)
			{
				if (className == "IPM.Appointment")
				{
					return WebClientEditFormQueryStringProperty.EditCalendarViewModel;
				}
				if (className == "IPM.Task")
				{
					return WebClientQueryStringPropertyBase.ReadTaskViewModel;
				}
			}
			return string.Empty;
		}

		private bool IsEditable(string className, bool isDraft, bool isPublic)
		{
			return (!isPublic || !(className == "IPM.Note")) && (isDraft || (className != null && (className == "IPM.Appointment" || className == "IPM.Task")));
		}

		private const int IsDraftPropertyIndex = 3;

		private PropertyDefinition isDraftProperty;

		private static readonly string EditCalendarViewModel = "ComposeCalendarItemViewModelFactory";
	}
}
