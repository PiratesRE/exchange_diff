using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class GrammarFileNotFoundException : LocalizedException
	{
		public GrammarFileNotFoundException(string grammarFile) : base(Strings.GrammarFileNotFoundException(grammarFile))
		{
			this.grammarFile = grammarFile;
		}

		public GrammarFileNotFoundException(string grammarFile, Exception innerException) : base(Strings.GrammarFileNotFoundException(grammarFile), innerException)
		{
			this.grammarFile = grammarFile;
		}

		protected GrammarFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.grammarFile = (string)info.GetValue("grammarFile", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("grammarFile", this.grammarFile);
		}

		public string GrammarFile
		{
			get
			{
				return this.grammarFile;
			}
		}

		private readonly string grammarFile;
	}
}
