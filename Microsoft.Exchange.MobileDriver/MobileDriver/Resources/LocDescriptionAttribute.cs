﻿using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.TextMessaging.MobileDriver.Resources
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal sealed class LocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public LocDescriptionAttribute(Strings.IDs ids) : base(Strings.GetLocalizedString(ids))
		{
		}
	}
}
