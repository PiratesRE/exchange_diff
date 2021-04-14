using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidSpeechGrammarFilterListException : LocalizedException
	{
		public InvalidSpeechGrammarFilterListException(LocalizedString message) : base(message)
		{
		}

		public InvalidSpeechGrammarFilterListException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidSpeechGrammarFilterListException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
