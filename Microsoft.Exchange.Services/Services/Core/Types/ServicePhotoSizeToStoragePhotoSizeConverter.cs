using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ServicePhotoSizeToStoragePhotoSizeConverter
	{
		public static UserPhotoSize Convert(UserPhotoSize size)
		{
			switch (size)
			{
			case UserPhotoSize.HR48x48:
				return UserPhotoSize.HR48x48;
			case UserPhotoSize.HR64x64:
				return UserPhotoSize.HR64x64;
			case UserPhotoSize.HR96x96:
				return UserPhotoSize.HR96x96;
			case UserPhotoSize.HR120x120:
				return UserPhotoSize.HR120x120;
			case UserPhotoSize.HR240x240:
				return UserPhotoSize.HR240x240;
			case UserPhotoSize.HR360x360:
				return UserPhotoSize.HR360x360;
			case UserPhotoSize.HR432x432:
				return UserPhotoSize.HR432x432;
			case UserPhotoSize.HR504x504:
				return UserPhotoSize.HR504x504;
			case UserPhotoSize.HR648x648:
				return UserPhotoSize.HR648x648;
			default:
				throw new ArgumentOutOfRangeException("size");
			}
		}
	}
}
