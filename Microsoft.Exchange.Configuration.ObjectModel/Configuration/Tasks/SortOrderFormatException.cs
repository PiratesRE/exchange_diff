using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SortOrderFormatException : LocalizedException
	{
		public SortOrderFormatException(string input) : base(Strings.SortOrderFormatException(input))
		{
			this.input = input;
		}

		public SortOrderFormatException(string input, Exception innerException) : base(Strings.SortOrderFormatException(input), innerException)
		{
			this.input = input;
		}

		protected SortOrderFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.input = (string)info.GetValue("input", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("input", this.input);
		}

		public string Input
		{
			get
			{
				return this.input;
			}
		}

		private readonly string input;
	}
}
