using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class NonOrgHierarchyConverter
	{
		public NonOrgHierarchyConverter(OrganizationId orgHierarchyToIgnore)
		{
			this.orgHierarchyToIgnore = orgHierarchyToIgnore;
		}

		internal bool TryConvertKeyToNonOrgHierarchy(ConvertOutputPropertyEventArgs args, out object convertedValue)
		{
			convertedValue = null;
			ConfigurableObject configurableObject = args.ConfigurableObject;
			PropertyDefinition property = args.Property;
			string propertyInStr = args.PropertyInStr;
			object value = args.Value;
			if (value == null)
			{
				return false;
			}
			if (!PswsKeyProperties.IsKeyProperty(configurableObject, property, propertyInStr))
			{
				return false;
			}
			if (value is INonOrgHierarchy)
			{
				((INonOrgHierarchy)value).OrgHierarchyToIgnore = this.orgHierarchyToIgnore;
				convertedValue = value;
				return true;
			}
			return false;
		}

		private OrganizationId orgHierarchyToIgnore;
	}
}
