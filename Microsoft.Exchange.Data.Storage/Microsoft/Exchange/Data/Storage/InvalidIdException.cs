﻿using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidIdException : StoragePermanentException
	{
		internal InvalidIdException() : base(LocalizedString.Empty)
		{
		}

		private InvalidIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
