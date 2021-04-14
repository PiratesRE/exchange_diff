﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoLocatorInformationInMServException : ADOperationException
	{
		public NoLocatorInformationInMServException() : base(DirectoryStrings.NoLocatorInformationInMServException)
		{
		}

		public NoLocatorInformationInMServException(Exception innerException) : base(DirectoryStrings.NoLocatorInformationInMServException, innerException)
		{
		}

		protected NoLocatorInformationInMServException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
