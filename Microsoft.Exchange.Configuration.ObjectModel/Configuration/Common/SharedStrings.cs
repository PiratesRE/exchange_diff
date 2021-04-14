using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SharedStrings
	{
		public static LocalizedString GenericConditionFailure
		{
			get
			{
				return Strings.GenericConditionFailure;
			}
		}

		public static LocalizedString ExceptionSetupFileNotFound(string fileName)
		{
			return Strings.ExceptionSetupFileNotFound(fileName);
		}

		public static LocalizedString LogCheckpoint(object checkPoint)
		{
			return Strings.LogCheckpoint(checkPoint);
		}
	}
}
