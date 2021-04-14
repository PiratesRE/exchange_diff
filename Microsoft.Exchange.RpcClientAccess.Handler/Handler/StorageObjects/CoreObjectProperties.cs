using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects
{
	internal class CoreObjectProperties : IStorageObjectProperties
	{
		public CoreObjectProperties(ICorePropertyBag corePropertyBag)
		{
			Util.ThrowOnNullArgument(corePropertyBag, "corePropertyBag");
			this.corePropertyBag = corePropertyBag;
		}

		protected ICorePropertyBag CorePropertyBag
		{
			get
			{
				return this.corePropertyBag;
			}
		}

		public ICollection<PropertyDefinition> AllFoundProperties
		{
			get
			{
				return this.corePropertyBag.AllFoundProperties;
			}
		}

		public virtual void SetProperty(PropertyDefinition propertyDefinition, object value)
		{
			this.corePropertyBag.SetProperty(propertyDefinition, value);
		}

		public virtual void DeleteProperty(PropertyDefinition propertyDefinition)
		{
			this.corePropertyBag.Delete(propertyDefinition);
		}

		public virtual Stream OpenStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			return this.corePropertyBag.OpenPropertyStream(propertyDefinition, openMode);
		}

		public void Load(ICollection<PropertyDefinition> propertiesToLoad)
		{
			this.corePropertyBag.Load(propertiesToLoad);
		}

		public virtual object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			return this.corePropertyBag.TryGetProperty(propertyDefinition);
		}

		public FolderSecurity.AclTableAndSecurityDescriptorProperty GetAclTableAndSecurityDescriptor()
		{
			return AclModifyTable.ReadAclTableAndSecurityDescriptor(this.corePropertyBag);
		}

		private readonly ICorePropertyBag corePropertyBag;
	}
}
