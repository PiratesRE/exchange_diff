using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SpeechGrammarFetchErrorException : SpeechGrammarException
	{
		public SpeechGrammarFetchErrorException(string grammar) : base(Strings.SpeechGrammarFetchErrorException(grammar))
		{
			this.grammar = grammar;
		}

		public SpeechGrammarFetchErrorException(string grammar, Exception innerException) : base(Strings.SpeechGrammarFetchErrorException(grammar), innerException)
		{
			this.grammar = grammar;
		}

		protected SpeechGrammarFetchErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.grammar = (string)info.GetValue("grammar", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("grammar", this.grammar);
		}

		public string Grammar
		{
			get
			{
				return this.grammar;
			}
		}

		private readonly string grammar;
	}
}
