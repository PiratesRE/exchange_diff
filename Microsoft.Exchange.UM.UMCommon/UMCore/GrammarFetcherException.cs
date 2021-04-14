using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class GrammarFetcherException : LocalizedException
	{
		public GrammarFetcherException(string msg) : base(Strings.GrammarFetcherException(msg))
		{
			this.msg = msg;
		}

		public GrammarFetcherException(string msg, Exception innerException) : base(Strings.GrammarFetcherException(msg), innerException)
		{
			this.msg = msg;
		}

		protected GrammarFetcherException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("msg", this.msg);
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly string msg;
	}
}
