using System;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal sealed class FolderSecurityDescriptorConversion : PropertyConversion
	{
		internal FolderSecurityDescriptorConversion() : base(PropertyTag.NTSecurityDescriptor, PropertyTag.AclTableAndSecurityDescriptor)
		{
		}

		protected override object ConvertValueFromClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptor = storageObjectProperties.GetAclTableAndSecurityDescriptor();
			RawSecurityDescriptor securityDescriptor = SecurityDescriptorForTransfer.ExtractSecurityDescriptor((byte[])propertyValue);
			return AclModifyTable.BuildAclTableBlob(session, securityDescriptor, (aclTableAndSecurityDescriptor.FreeBusySecurityDescriptor != null) ? aclTableAndSecurityDescriptor.FreeBusySecurityDescriptor.ToRawSecurityDescriptorThrow() : null);
		}

		protected override object ConvertValueToClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = FolderSecurity.AclTableAndSecurityDescriptorProperty.Parse((byte[])propertyValue);
			return SecurityDescriptorForTransfer.FormatSecurityDescriptor(aclTableAndSecurityDescriptorProperty.SecurityDescriptor.ToRawSecurityDescriptorThrow());
		}
	}
}
