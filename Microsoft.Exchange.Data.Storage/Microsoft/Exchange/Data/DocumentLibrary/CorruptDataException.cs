using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class CorruptDataException : DocumentLibraryException
	{
		internal CorruptDataException(object obj, string message) : this(obj, message, null)
		{
		}

		internal CorruptDataException(object obj, string message, Exception innerException) : base(message, innerException)
		{
			this.obj = obj;
		}

		public object CorruptedObject
		{
			get
			{
				return this.obj;
			}
		}

		private readonly object obj;
	}
}
