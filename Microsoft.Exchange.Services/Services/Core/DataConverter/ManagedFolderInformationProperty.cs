using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ManagedFolderInformationProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		public ManagedFolderInformationProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ManagedFolderInformationProperty CreateCommand(CommandContext commandContext)
		{
			return new ManagedFolderInformationProperty(commandContext);
		}

		private bool TryGetPropertyValue<T>(StoreObject storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex managedFolderInfoIndex, out T value)
		{
			value = default(T);
			PropertyDefinition propertyDefinition = this.propertyDefinitions[(int)managedFolderInfoIndex];
			if (!PropertyCommand.StorePropertyExists(storeObject, propertyDefinition))
			{
				return false;
			}
			object propertyValueFromStoreObject = PropertyCommand.GetPropertyValueFromStoreObject(storeObject, propertyDefinition);
			if (propertyValueFromStoreObject is T)
			{
				value = (T)((object)propertyValueFromStoreObject);
				return true;
			}
			return false;
		}

		private bool TryGetPropertyValueForPropertyBag<T>(IDictionary<PropertyDefinition, object> propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex managedFolderInfoIndex, out T value)
		{
			PropertyDefinition key = this.propertyDefinitions[(int)managedFolderInfoIndex];
			return PropertyCommand.TryGetValueFromPropertyBag<T>(propertyBag, key, out value);
		}

		private void GetAdminFolderFlags(int flags, out bool canDelete, out bool canRenameOrMove, out bool mustDisplayComment, out bool hasQuota, out bool isManagedFoldersRoot)
		{
			if (!EnumValidator.IsValidEnum<ELCFolderFlags>((ELCFolderFlags)flags))
			{
				throw new ManagedFolderInvalidPropertyValueException(FolderSchema.AdminFolderFlags.Name);
			}
			canDelete = true;
			canRenameOrMove = true;
			mustDisplayComment = false;
			hasQuota = false;
			isManagedFoldersRoot = false;
			if ((flags & 1) == 1)
			{
				canDelete = false;
			}
			if ((flags & 2) == 2)
			{
				canRenameOrMove = false;
			}
			if ((flags & 4) == 4)
			{
				mustDisplayComment = true;
			}
			if ((flags & 8) == 8)
			{
				hasQuota = true;
			}
			if ((flags & 16) == 16)
			{
				isManagedFoldersRoot = true;
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			int flags;
			if (!this.TryGetPropertyValue<int>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.AdminFolderFlags, out flags))
			{
				return;
			}
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			bool canDelete;
			bool canRenameOrMove;
			bool mustDisplayComment;
			bool hasQuota;
			bool isManagedFoldersRoot;
			this.GetAdminFolderFlags(flags, out canDelete, out canRenameOrMove, out mustDisplayComment, out hasQuota, out isManagedFoldersRoot);
			ManagedFolderInformationType managedFolderInformationType = new ManagedFolderInformationType
			{
				CanDelete = canDelete,
				CanRenameOrMove = canRenameOrMove,
				MustDisplayComment = mustDisplayComment,
				HasQuota = hasQuota,
				IsManagedFoldersRoot = isManagedFoldersRoot
			};
			string managedFolderId;
			if (this.TryGetPropertyValue<string>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.ELCPolicyIds, out managedFolderId))
			{
				managedFolderInformationType.ManagedFolderId = managedFolderId;
			}
			string comment;
			if (this.TryGetPropertyValue<string>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.ELCFolderComment, out comment))
			{
				managedFolderInformationType.Comment = comment;
			}
			int storageQuota;
			if (this.TryGetPropertyValue<int>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderQuota, out storageQuota))
			{
				managedFolderInformationType.StorageQuota = storageQuota;
			}
			int folderSize;
			if (this.TryGetPropertyValue<int>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderSize, out folderSize))
			{
				managedFolderInformationType.FolderSize = folderSize;
			}
			string homePage;
			if (this.TryGetPropertyValue<string>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderHomePageUrl, out homePage))
			{
				managedFolderInformationType.HomePage = homePage;
			}
			serviceObject[propertyInformation] = managedFolderInformationType;
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			int flags;
			if (!this.TryGetPropertyValueForPropertyBag<int>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.AdminFolderFlags, out flags))
			{
				return;
			}
			bool canDelete;
			bool canRenameOrMove;
			bool mustDisplayComment;
			bool hasQuota;
			bool isManagedFoldersRoot;
			this.GetAdminFolderFlags(flags, out canDelete, out canRenameOrMove, out mustDisplayComment, out hasQuota, out isManagedFoldersRoot);
			ManagedFolderInformationType managedFolderInformationType = new ManagedFolderInformationType
			{
				CanDelete = canDelete,
				CanRenameOrMove = canRenameOrMove,
				MustDisplayComment = mustDisplayComment,
				HasQuota = hasQuota,
				IsManagedFoldersRoot = isManagedFoldersRoot
			};
			string managedFolderId;
			if (this.TryGetPropertyValueForPropertyBag<string>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.ELCPolicyIds, out managedFolderId))
			{
				managedFolderInformationType.ManagedFolderId = managedFolderId;
			}
			string comment;
			if (this.TryGetPropertyValueForPropertyBag<string>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.ELCFolderComment, out comment))
			{
				managedFolderInformationType.Comment = comment;
			}
			int storageQuota;
			if (this.TryGetPropertyValueForPropertyBag<int>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderQuota, out storageQuota))
			{
				managedFolderInformationType.StorageQuota = storageQuota;
			}
			int folderSize;
			if (this.TryGetPropertyValueForPropertyBag<int>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderSize, out folderSize))
			{
				managedFolderInformationType.FolderSize = folderSize;
			}
			string homePage;
			if (this.TryGetPropertyValueForPropertyBag<string>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderHomePageUrl, out homePage))
			{
				managedFolderInformationType.HomePage = homePage;
			}
			serviceObject[propertyInformation] = managedFolderInformationType;
		}

		public void ToXml()
		{
			ToXmlCommandSettings commandSettings = base.GetCommandSettings<ToXmlCommandSettings>();
			XmlElement serviceItem = commandSettings.ServiceItem;
			StoreObject storeObject = commandSettings.StoreObject;
			int flags;
			if (!this.TryGetPropertyValue<int>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.AdminFolderFlags, out flags))
			{
				return;
			}
			XmlElement parentElement = base.CreateXmlElement(serviceItem, "ManagedFolderInformation");
			this.RenderAdminFolderFlagsProperty(parentElement, flags);
			string policyIds;
			if (this.TryGetPropertyValue<string>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.ELCPolicyIds, out policyIds))
			{
				this.RenderELCPolicyIdsProperty(parentElement, policyIds);
			}
			string comment;
			if (this.TryGetPropertyValue<string>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.ELCFolderComment, out comment))
			{
				this.RenderELCFolderCommentProperty(parentElement, comment);
			}
			int quota;
			if (this.TryGetPropertyValue<int>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderQuota, out quota))
			{
				this.RenderFolderQuotaProperty(parentElement, quota);
			}
			int size;
			if (this.TryGetPropertyValue<int>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderSize, out size))
			{
				this.RenderFolderSizeProperty(parentElement, size);
			}
			string homePageUrl;
			if (this.TryGetPropertyValue<string>(storeObject, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderHomePageUrl, out homePageUrl))
			{
				this.RenderFolderHomePageUrlProperty(parentElement, homePageUrl);
			}
		}

		public void ToXmlForPropertyBag()
		{
			ToXmlForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToXmlForPropertyBagCommandSettings>();
			XmlElement serviceItem = commandSettings.ServiceItem;
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			int flags;
			if (!this.TryGetPropertyValueForPropertyBag<int>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.AdminFolderFlags, out flags))
			{
				return;
			}
			XmlElement parentElement = base.CreateXmlElement(serviceItem, "ManagedFolderInformation");
			this.RenderAdminFolderFlagsProperty(parentElement, flags);
			string policyIds;
			if (this.TryGetPropertyValueForPropertyBag<string>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.ELCPolicyIds, out policyIds))
			{
				this.RenderELCPolicyIdsProperty(parentElement, policyIds);
			}
			string comment;
			if (this.TryGetPropertyValueForPropertyBag<string>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.ELCFolderComment, out comment))
			{
				this.RenderELCFolderCommentProperty(parentElement, comment);
			}
			int quota;
			if (this.TryGetPropertyValueForPropertyBag<int>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderQuota, out quota))
			{
				this.RenderFolderQuotaProperty(parentElement, quota);
			}
			int size;
			if (this.TryGetPropertyValueForPropertyBag<int>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderSize, out size))
			{
				this.RenderFolderSizeProperty(parentElement, size);
			}
			string homePageUrl;
			if (this.TryGetPropertyValueForPropertyBag<string>(propertyBag, ManagedFolderPropertyInformation.ManagedFolderInfoIndex.FolderHomePageUrl, out homePageUrl))
			{
				this.RenderFolderHomePageUrlProperty(parentElement, homePageUrl);
			}
		}

		private void RenderAdminFolderFlagsProperty(XmlElement parentElement, int flags)
		{
			bool propertyValue;
			bool propertyValue2;
			bool propertyValue3;
			bool propertyValue4;
			bool propertyValue5;
			this.GetAdminFolderFlags(flags, out propertyValue, out propertyValue2, out propertyValue3, out propertyValue4, out propertyValue5);
			base.CreateXmlTextElement(parentElement, "CanDelete", BooleanConverter.ToString(propertyValue));
			base.CreateXmlTextElement(parentElement, "CanRenameOrMove", BooleanConverter.ToString(propertyValue2));
			base.CreateXmlTextElement(parentElement, "MustDisplayComment", BooleanConverter.ToString(propertyValue3));
			base.CreateXmlTextElement(parentElement, "HasQuota", BooleanConverter.ToString(propertyValue4));
			base.CreateXmlTextElement(parentElement, "IsManagedFoldersRoot", BooleanConverter.ToString(propertyValue5));
		}

		private void RenderELCPolicyIdsProperty(XmlElement parentElement, string policyIds)
		{
			if (string.IsNullOrEmpty(policyIds))
			{
				throw new ManagedFolderInvalidPropertyValueException(FolderSchema.ELCPolicyIds.Name);
			}
			base.CreateXmlTextElement(parentElement, "ManagedFolderId", policyIds);
		}

		private void RenderELCFolderCommentProperty(XmlElement parentElement, string comment)
		{
			if (!string.IsNullOrEmpty(comment))
			{
				base.CreateXmlTextElement(parentElement, "Comment", comment);
			}
		}

		private void RenderFolderQuotaProperty(XmlElement parentElement, int quota)
		{
			if (quota < 0)
			{
				throw new ManagedFolderInvalidPropertyValueException(FolderSchema.FolderQuota.Name);
			}
			base.CreateXmlTextElement(parentElement, "StorageQuota", IntConverter.ToString(quota));
		}

		private void RenderFolderSizeProperty(XmlElement parentElement, int size)
		{
			if (size < 0)
			{
				throw new ManagedFolderInvalidPropertyValueException(FolderSchema.FolderSize.Name);
			}
			base.CreateXmlTextElement(parentElement, "FolderSize", IntConverter.ToString(size));
		}

		private void RenderFolderHomePageUrlProperty(XmlElement parentElement, string homePageUrl)
		{
			if (!string.IsNullOrEmpty(homePageUrl))
			{
				base.CreateXmlTextElement(parentElement, "HomePage", homePageUrl);
			}
		}
	}
}
