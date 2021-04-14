using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DefaultFolderValidator
	{
		internal DefaultFolderValidator(params IValidator[] validators)
		{
			if (validators != null)
			{
				this.validators = validators;
			}
		}

		internal void SetProperties(DefaultFolderContext context, Folder folder)
		{
			this.SetPropertiesInternal(context, folder);
		}

		internal virtual bool EnsureIsValid(DefaultFolderContext context, Folder folder)
		{
			return this.EnsureIsValid(context, folder.PropertyBag);
		}

		internal virtual bool EnsureIsValid(DefaultFolderContext context, StoreObjectId folderId, Dictionary<string, DefaultFolderManager.FolderData> folderDataDictionary)
		{
			string text = Convert.ToBase64String(folderId.ProviderLevelItemId);
			DefaultFolderManager.FolderData folderData;
			if (folderDataDictionary != null && folderDataDictionary.TryGetValue(text, out folderData))
			{
				return this.EnsureIsValid(context, folderData.PropertyBag);
			}
			return this.BindAndValidateFolder(context, folderId, text);
		}

		protected bool BindAndValidateFolder(DefaultFolderContext context, StoreObjectId folderId, string idString)
		{
			bool result2;
			try
			{
				bool result = false;
				context.Session.BypassAuditing(delegate
				{
					using (Folder folder = Folder.Bind(context.Session, folderId, DefaultFolderInfo.InboxOrConfigurationFolderProperties))
					{
						result = this.EnsureIsValid(context, folder);
					}
				});
				result2 = result;
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceDebug<string>((long)this.GetHashCode(), "FolderValidationStrategy::EnsureIsValid. The folder is missing. FolderId = {0}.", idString);
				result2 = false;
			}
			return result2;
		}

		internal bool EnsureIsValid(DefaultFolderContext context, PropertyBag propertyBag)
		{
			return !string.IsNullOrEmpty(propertyBag.TryGetProperty(InternalSchema.DisplayName) as string) && this.ValidateInternal(context, propertyBag);
		}

		protected virtual bool ValidateInternal(DefaultFolderContext context, PropertyBag propertyBag)
		{
			if (this.validators == null)
			{
				return true;
			}
			foreach (IValidator validator in this.validators)
			{
				if (!validator.Validate(context, propertyBag))
				{
					return false;
				}
			}
			return true;
		}

		protected virtual void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			foreach (IValidator validator in this.validators)
			{
				validator.SetProperties(context, folder);
			}
		}

		internal static NullDefaultFolderValidator NullValidator = new NullDefaultFolderValidator();

		internal static DefaultFolderValidator MessageFolderGenericTypeValidator = new DefaultFolderValidator(new IValidator[]
		{
			new CompositeValidator(new IValidator[]
			{
				new MatchMapiFolderType(FolderType.Generic),
				new MatchContainerClass("IPF.Note")
			})
		});

		internal static DefaultFolderValidator FolderGenericTypeValidator = new DefaultFolderValidator(new IValidator[]
		{
			new MatchMapiFolderType(FolderType.Generic)
		});

		private IValidator[] validators;
	}
}
