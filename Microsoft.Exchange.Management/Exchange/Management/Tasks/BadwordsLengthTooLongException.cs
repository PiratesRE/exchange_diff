using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BadwordsLengthTooLongException : LocalizedException
	{
		public BadwordsLengthTooLongException(string prefix, int maxLength) : base(Strings.BadwordsLengthTooLongId(prefix, maxLength))
		{
			this.prefix = prefix;
			this.maxLength = maxLength;
		}

		public BadwordsLengthTooLongException(string prefix, int maxLength, Exception innerException) : base(Strings.BadwordsLengthTooLongId(prefix, maxLength), innerException)
		{
			this.prefix = prefix;
			this.maxLength = maxLength;
		}

		protected BadwordsLengthTooLongException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.prefix = (string)info.GetValue("prefix", typeof(string));
			this.maxLength = (int)info.GetValue("maxLength", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("prefix", this.prefix);
			info.AddValue("maxLength", this.maxLength);
		}

		public string Prefix
		{
			get
			{
				return this.prefix;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
		}

		private readonly string prefix;

		private readonly int maxLength;
	}
}
