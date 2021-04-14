using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class FolderPolicyTagProperty : RetentionTagPropertyBase
	{
		private FolderPolicyTagProperty(CommandContext commandContext) : base(commandContext, Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionActionType.DeleteAndAllowRecovery)
		{
		}

		public static FolderPolicyTagProperty CreateCommand(CommandContext commandContext)
		{
			return new FolderPolicyTagProperty(commandContext);
		}

		internal override Guid? GetRetentionTag(StoreObject storeObject, out bool isExplicit)
		{
			Folder folder = storeObject as Folder;
			if (folder == null)
			{
				throw new InvalidPropertyRequestException(this.commandContext.PropertyInformation.PropertyPath);
			}
			isExplicit = false;
			if (!PropertyCommand.StorePropertyExists(storeObject, StoreObjectSchema.PolicyTag))
			{
				ExTraceGlobals.ELCTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "[FolderPolicyTagProperty::GetRetentionTag] PolicyTag property did not exist for {0}", storeObject.StoreObjectId);
				return null;
			}
			Guid? policyTagForDeleteFromFolder = PolicyTagHelper.GetPolicyTagForDeleteFromFolder(folder, out isExplicit);
			if (policyTagForDeleteFromFolder != null)
			{
				ExTraceGlobals.ELCTracer.TraceDebug<Guid?, StoreObjectId>((long)this.GetHashCode(), "[FolderPolicyTagProperty::GetRetentionTag] Policy tag {0} was found for {1}", policyTagForDeleteFromFolder, storeObject.StoreObjectId);
			}
			else
			{
				ExTraceGlobals.ELCTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "[FolderPolicyTagProperty::GetRetentionTag] Policy tag was not found for {1}", storeObject.StoreObjectId);
			}
			return policyTagForDeleteFromFolder;
		}

		internal override Guid? GetRetentionTagFromPropertyBag(IDictionary<PropertyDefinition, object> propertyBag, out bool isExplicit)
		{
			isExplicit = false;
			byte[] b;
			if (!PropertyCommand.TryGetValueFromPropertyBag<byte[]>(propertyBag, StoreObjectSchema.PolicyTag, out b))
			{
				ExTraceGlobals.ELCTracer.TraceDebug((long)this.GetHashCode(), "[FolderPolicyTagProperty::GetRetentionTagFromPropertyBag] Policy tag was not found in property bag.");
				return null;
			}
			Guid value = new Guid(b);
			object obj;
			if (PropertyCommand.TryGetValueFromPropertyBag<object>(propertyBag, StoreObjectSchema.RetentionFlags, out obj) && obj != null && obj is int)
			{
				isExplicit = (((int)obj & 1) != 0);
			}
			return new Guid?(value);
		}

		internal override void SetRetentionTag(StoreObject storeObject, PolicyTag policyTag)
		{
			using (Folder folder = base.OpenFolderForRetentionTag(storeObject.Session, storeObject.StoreObjectId, PolicyTagHelper.RetentionProperties))
			{
				PolicyTagHelper.SetPolicyTagForDeleteOnFolder(policyTag, folder);
				folder.Save();
			}
			ExTraceGlobals.ELCTracer.TraceDebug<Guid, StoreObjectId>((long)this.GetHashCode(), "[FolderPolicyTagProperty::SetRetentionTag] Policy tag {0} was set on {1}", policyTag.PolicyGuid, storeObject.StoreObjectId);
		}

		internal override void NewRetentionTag(StoreObject storeObject, PolicyTag policyTag)
		{
			Folder folder = storeObject as Folder;
			if (folder == null)
			{
				throw new InvalidPropertyRequestException(this.commandContext.PropertyInformation.PropertyPath);
			}
			PolicyTagHelper.SetPolicyTagForDeleteOnNewFolder(policyTag, folder);
			ExTraceGlobals.ELCTracer.TraceDebug<Guid>((long)this.GetHashCode(), "[FolderPolicyTagProperty::NewRetentionTag] Policy tag {0} was set on new folder", policyTag.PolicyGuid);
		}

		internal override void DeleteRetentionTag(StoreObject storeObject)
		{
			using (Folder folder = base.OpenFolderForRetentionTag(storeObject.Session, storeObject.StoreObjectId, PolicyTagHelper.RetentionProperties))
			{
				PolicyTagHelper.ClearPolicyTagForDeleteOnFolder(folder);
				folder.Save();
			}
			ExTraceGlobals.ELCTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "[FolderPolicyTagProperty::DeleteRetentionTag] PolicyTag was cleared on {0}", storeObject.StoreObjectId);
		}
	}
}
