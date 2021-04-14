﻿using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ICacheConsumer
	{
		object Id { get; }
	}
}
