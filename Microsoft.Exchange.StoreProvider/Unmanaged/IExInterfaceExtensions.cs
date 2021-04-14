using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class IExInterfaceExtensions
	{
		internal static void DisposeIfValid(this IExInterface iExInterface)
		{
			if (iExInterface != null)
			{
				iExInterface.Dispose();
				iExInterface = null;
			}
		}

		internal static T ToInterface<T>(this IExInterface iExInterface) where T : class, IExInterface
		{
			if (iExInterface is SafeExInterfaceHandle)
			{
				if (typeof(T) == typeof(IExMapiFolder))
				{
					return new SafeExMapiFolderHandle((SafeExInterfaceHandle)iExInterface) as T;
				}
				if (typeof(T) == typeof(IExMapiMessage))
				{
					return new SafeExMapiMessageHandle((SafeExInterfaceHandle)iExInterface) as T;
				}
				if (typeof(T) == typeof(IExMapiStream))
				{
					return new SafeExMapiStreamHandle((SafeExInterfaceHandle)iExInterface) as T;
				}
				if (typeof(T) == typeof(IExModifyTable))
				{
					return new SafeExModifyTableHandle((SafeExInterfaceHandle)iExInterface) as T;
				}
				if (typeof(T) == typeof(IExExportChanges))
				{
					return new SafeExExportChangesHandle((SafeExInterfaceHandle)iExInterface) as T;
				}
				if (typeof(T) == typeof(IExImportContentsChanges))
				{
					return new SafeExImportContentsChangesHandle((SafeExInterfaceHandle)iExInterface) as T;
				}
				if (typeof(T) == typeof(IExImportHierarchyChanges))
				{
					return new SafeExImportHierarchyChangesHandle((SafeExInterfaceHandle)iExInterface) as T;
				}
				if (typeof(T) == typeof(IExExportManifest))
				{
					return new SafeExExportManifestHandle((SafeExInterfaceHandle)iExInterface) as T;
				}
				if (typeof(T) == typeof(IExFastTransferEx))
				{
					return new SafeExFastTransferExHandle((SafeExInterfaceHandle)iExInterface) as T;
				}
				if (typeof(T) == typeof(IExLastErrorInfo))
				{
					return new SafeExLastErrorInfoHandle((SafeExInterfaceHandle)iExInterface) as T;
				}
			}
			if (iExInterface is T && (typeof(T) == typeof(IExMapiFolder) || typeof(T) == typeof(IExMapiMessage) || typeof(T) == typeof(IExMapiStream)))
			{
				return (T)((object)iExInterface);
			}
			throw new InvalidCastException(string.Format("Cannot cast an instance of type '{0}' to '{1}'.", iExInterface.GetType(), typeof(T)));
		}
	}
}
