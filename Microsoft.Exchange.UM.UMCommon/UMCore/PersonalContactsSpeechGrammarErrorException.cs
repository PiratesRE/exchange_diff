using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PersonalContactsSpeechGrammarErrorException : SpeechGrammarException
	{
		public PersonalContactsSpeechGrammarErrorException(string user) : base(Strings.PersonalContactsSpeechGrammarErrorException(user))
		{
			this.user = user;
		}

		public PersonalContactsSpeechGrammarErrorException(string user, Exception innerException) : base(Strings.PersonalContactsSpeechGrammarErrorException(user), innerException)
		{
			this.user = user;
		}

		protected PersonalContactsSpeechGrammarErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
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
