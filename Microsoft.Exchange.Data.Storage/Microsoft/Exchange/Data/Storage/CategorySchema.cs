using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CategorySchema : Schema
	{
		public new static CategorySchema Instance
		{
			get
			{
				if (CategorySchema.instance == null)
				{
					CategorySchema.instance = new CategorySchema();
				}
				return CategorySchema.instance;
			}
		}

		public static StorePropertyDefinition AllowRenameOnFirstUse = InternalSchema.CategoryAllowRenameOnFirstUse;

		[Required]
		[Autoload]
		public static StorePropertyDefinition Name = InternalSchema.CategoryName;

		public static StorePropertyDefinition Color = InternalSchema.CategoryColor;

		public static StorePropertyDefinition KeyboardShortcut = InternalSchema.CategoryKeyboardShortcut;

		public static StorePropertyDefinition LastTimeUsedNotes = InternalSchema.CategoryLastTimeUsedNotes;

		public static StorePropertyDefinition LastTimeUsedJournal = InternalSchema.CategoryLastTimeUsedJournal;

		public static StorePropertyDefinition LastTimeUsedContacts = InternalSchema.CategoryLastTimeUsedContacts;

		public static StorePropertyDefinition LastTimeUsedTasks = InternalSchema.CategoryLastTimeUsedTasks;

		public static StorePropertyDefinition LastTimeUsedCalendar = InternalSchema.CategoryLastTimeUsedCalendar;

		public static StorePropertyDefinition LastTimeUsedMail = InternalSchema.CategoryLastTimeUsedMail;

		public static StorePropertyDefinition LastTimeUsed = InternalSchema.CategoryLastTimeUsed;

		public static StorePropertyDefinition LastSessionUsed = InternalSchema.CategoryLastSessionUsed;

		[Autoload]
		[Required]
		public static StorePropertyDefinition Guid = InternalSchema.CategoryGuid;

		private static CategorySchema instance = null;
	}
}
