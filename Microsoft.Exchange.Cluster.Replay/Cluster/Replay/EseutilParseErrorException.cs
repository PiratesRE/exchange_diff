using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EseutilParseErrorException : TransientException
	{
		public EseutilParseErrorException(string line, string regex) : base(ReplayStrings.EseutilParseError(line, regex))
		{
			this.line = line;
			this.regex = regex;
		}

		public EseutilParseErrorException(string line, string regex, Exception innerException) : base(ReplayStrings.EseutilParseError(line, regex), innerException)
		{
			this.line = line;
			this.regex = regex;
		}

		protected EseutilParseErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.line = (string)info.GetValue("line", typeof(string));
			this.regex = (string)info.GetValue("regex", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("line", this.line);
			info.AddValue("regex", this.regex);
		}

		public string Line
		{
			get
			{
				return this.line;
			}
		}

		public string Regex
		{
			get
			{
				return this.regex;
			}
		}

		private readonly string line;

		private readonly string regex;
	}
}
