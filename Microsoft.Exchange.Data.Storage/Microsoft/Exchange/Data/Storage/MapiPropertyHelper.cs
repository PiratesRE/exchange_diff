using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MapiPropertyHelper
	{
		public static PropertyErrorCode MapiErrorToXsoError(int mapiErrorCode, out string errorText)
		{
			errorText = string.Empty;
			long num = (long)mapiErrorCode;
			if (num <= -2147221221L)
			{
				if (num <= -2147221246L)
				{
					if (num == -2147467259L)
					{
						return PropertyErrorCode.MapiCallFailed;
					}
					if (num == -2147221246L)
					{
						return PropertyErrorCode.NotSupported;
					}
				}
				else
				{
					if (num == -2147221233L)
					{
						return PropertyErrorCode.NotFound;
					}
					if (num <= -2147221221L && num >= -2147221222L)
					{
						switch ((int)(num - -2147221222L))
						{
						case 0:
							return PropertyErrorCode.SetStoreComputedPropertyError;
						case 1:
							return PropertyErrorCode.CorruptedData;
						}
					}
				}
			}
			else if (num <= -2147219964L)
			{
				if (num == -2147220732L)
				{
					return PropertyErrorCode.IncorrectValueType;
				}
				if (num == -2147219964L)
				{
					return PropertyErrorCode.FolderNameConflict;
				}
			}
			else
			{
				if (num == -2147024891L)
				{
					return PropertyErrorCode.AccessDenied;
				}
				if (num == -2147024882L)
				{
					return PropertyErrorCode.NotEnoughMemory;
				}
				if (num == 2608L)
				{
					return PropertyErrorCode.PropertyNotPromoted;
				}
			}
			errorText = "0x" + mapiErrorCode.ToString("X8", NumberFormatInfo.InvariantInfo);
			return PropertyErrorCode.UnknownError;
		}
	}
}
