using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FolderImportPropertyBag : FolderPropertyBag
	{
		internal FolderImportPropertyBag(HierarchySynchronizationUploadContext context, StoreObjectId parentFolderId, StoreObjectId folderId, ICollection<PropertyDefinition> properties) : base(context.Session, null, properties)
		{
			this.context = context;
			this.parentFolderId = parentFolderId;
			this.folderId = folderId;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FolderImportPropertyBag>(this);
		}

		internal override FolderSaveResult SaveFolderPropertyBag(bool needVersionCheck)
		{
			if (this.Context.CoreObject.Origin == Origin.New)
			{
				try
				{
					try
					{
						using (Folder folder = Folder.Bind(this.context.Session, this.parentFolderId, null))
						{
							if (folder is SearchFolder)
							{
								throw new InvalidParentFolderException(ServerStrings.ExCannotCreateSubfolderUnderSearchFolder);
							}
						}
					}
					catch (ObjectNotFoundException)
					{
					}
					List<PropertyDefinition> list = new List<PropertyDefinition>(base.MemoryPropertyBag.ChangeList.Count);
					List<object> list2 = new List<object>(list.Count);
					foreach (PropertyDefinition propertyDefinition in base.MemoryPropertyBag.ChangeList)
					{
						object obj = base.MemoryPropertyBag.TryGetProperty(propertyDefinition);
						if (!PropertyError.IsPropertyError(obj))
						{
							list.Add(propertyDefinition);
							list2.Add(obj);
						}
					}
					this.context.ImportChange(this.ExTimeZone, list, list2);
					this.Context.CoreState.Origin = Origin.Existing;
					return FolderPropertyBag.SuccessfulSave;
				}
				finally
				{
					this.Clear();
				}
			}
			return base.SaveFolderPropertyBag(needVersionCheck);
		}

		protected override void LazyCreateMapiPropertyBag()
		{
			if (this.Context.CoreObject != null)
			{
				if (this.Context.CoreState.Origin == Origin.New)
				{
					return;
				}
				base.MapiPropertyBag = MapiPropertyBag.CreateMapiPropertyBag(this.context.Session, this.folderId);
			}
		}

		private readonly HierarchySynchronizationUploadContext context;

		private readonly StoreObjectId parentFolderId;

		private readonly StoreObjectId folderId;
	}
}
