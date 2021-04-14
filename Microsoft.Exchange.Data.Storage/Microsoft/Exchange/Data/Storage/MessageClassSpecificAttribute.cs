using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[AttributeUsage(AttributeTargets.Field)]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MessageClassSpecificAttribute : Attribute
	{
		public MessageClassSpecificAttribute()
		{
			this.messageClass = string.Empty;
		}

		public MessageClassSpecificAttribute(string messageClass)
		{
			this.messageClass = messageClass;
		}

		public string MessageClass
		{
			get
			{
				return this.messageClass;
			}
		}

		private string messageClass;
	}
}
