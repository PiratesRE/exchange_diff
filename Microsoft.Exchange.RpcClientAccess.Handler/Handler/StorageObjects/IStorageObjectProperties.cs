using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects
{
	internal interface IStorageObjectProperties
	{
		ICollection<PropertyDefinition> AllFoundProperties { get; }

		object TryGetProperty(PropertyDefinition propertyDefinition);

		void SetProperty(PropertyDefinition propertyDefinition, object value);

		void DeleteProperty(PropertyDefinition propertyDefinition);

		Stream OpenStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode);

		void Load(ICollection<PropertyDefinition> propertiesToLoad);

		FolderSecurity.AclTableAndSecurityDescriptorProperty GetAclTableAndSecurityDescriptor();
	}
}
