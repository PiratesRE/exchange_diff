using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PersonalContactsSpeechGrammarTimeoutException : SpeechGrammarException
	{
		public PersonalContactsSpeechGrammarTimeoutException(string user) : base(Strings.PersonalContactsSpeechGrammarTimeoutException(user))
		{
			this.user = user;
		}

		public PersonalContactsSpeechGrammarTimeoutException(string user, Exception innerException) : base(Strings.PersonalContactsSpeechGrammarTimeoutException(user), innerException)
		{
			this.user = user;
		}

		protected PersonalContactsSpeechGrammarTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		private readonly string user;
	}
}
