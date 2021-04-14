using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ManagedFolderInvalidPropertyValueException : ServicePermanentException
	{
		static ManagedFolderInvalidPropertyValueException()
		{
			ManagedFolderInvalidPropertyValueException.errorMappings.Add(FolderSchema.AdminFolderFlags.Name, CoreResources.IDs.ErrorInvalidManagedFolderProperty);
			ManagedFolderInvalidPropertyValueException.errorMappings.Add(FolderSchema.ELCPolicyIds.Name, (CoreResources.IDs)2518142400U);
			ManagedFolderInvalidPropertyValueException.errorMappings.Add(FolderSchema.FolderQuota.Name, (CoreResources.IDs)2756368512U);
			ManagedFolderInvalidPropertyValueException.errorMappings.Add(FolderSchema.FolderSize.Name, (CoreResources.IDs)4227165423U);
		}

		public ManagedFolderInvalidPropertyValueException(string propertyName) : base(ManagedFolderInvalidPropertyValueException.errorMappings[propertyName])
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		private static Dictionary<string, Enum> errorMappings = new Dictionary<string, Enum>();
	}
}
