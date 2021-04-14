using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UserPhotoSchema : StoreObjectSchema
	{
		public new static UserPhotoSchema Instance
		{
			get
			{
				if (UserPhotoSchema.instance == null)
				{
					UserPhotoSchema.instance = new UserPhotoSchema();
				}
				return UserPhotoSchema.instance;
			}
		}

		public static readonly PropertyDefinition UserPhotoHR648x648 = InternalSchema.UserPhotoHR648x648;

		public static readonly PropertyDefinition UserPhotoHR504x504 = InternalSchema.UserPhotoHR504x504;

		public static readonly PropertyDefinition UserPhotoHR432x432 = InternalSchema.UserPhotoHR432x432;

		public static readonly PropertyDefinition UserPhotoHR360x360 = InternalSchema.UserPhotoHR360x360;

		public static readonly PropertyDefinition UserPhotoHR240x240 = InternalSchema.UserPhotoHR240x240;

		public static readonly PropertyDefinition UserPhotoHR120x120 = InternalSchema.UserPhotoHR120x120;

		public static readonly PropertyDefinition UserPhotoHR96x96 = InternalSchema.UserPhotoHR96x96;

		public static readonly PropertyDefinition UserPhotoHR64x64 = InternalSchema.UserPhotoHR64x64;

		public static readonly PropertyDefinition UserPhotoHR48x48 = InternalSchema.UserPhotoHR48x48;

		private static UserPhotoSchema instance;
	}
}
