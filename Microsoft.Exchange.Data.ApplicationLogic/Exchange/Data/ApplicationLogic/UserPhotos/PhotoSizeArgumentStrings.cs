using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class PhotoSizeArgumentStrings
	{
		public static string Get(UserPhotoSize size)
		{
			switch (size)
			{
			case UserPhotoSize.HR48x48:
				return "size=HR48X48";
			case UserPhotoSize.HR64x64:
				return "size=HR64X64";
			case UserPhotoSize.HR96x96:
				return "size=HR96X96";
			case UserPhotoSize.HR120x120:
				return "size=HR120X120";
			case UserPhotoSize.HR240x240:
				return "size=HR240X240";
			case UserPhotoSize.HR360x360:
				return "size=HR360X360";
			case UserPhotoSize.HR432x432:
				return "size=HR432X432";
			case UserPhotoSize.HR504x504:
				return "size=HR504X504";
			case UserPhotoSize.HR648x648:
				return "size=HR648X648";
			default:
				throw new ArgumentOutOfRangeException("size");
			}
		}

		private const string HR48x48 = "size=HR48X48";

		private const string HR64x64 = "size=HR64X64";

		private const string HR96x96 = "size=HR96X96";

		private const string HR120x120 = "size=HR120X120";

		private const string HR240x240 = "size=HR240X240";

		private const string HR360x360 = "size=HR360X360";

		private const string HR432x432 = "size=HR432X432";

		private const string HR504x504 = "size=HR504X504";

		private const string HR648x648 = "size=HR648X648";
	}
}
