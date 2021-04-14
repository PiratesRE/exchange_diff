using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidLanguageIdException : LocalizedException
	{
		public InvalidLanguageIdException(string l) : base(Strings.InvalidLanguageIdException(l))
		{
			this.l = l;
		}

		public InvalidLanguageIdException(string l, Exception innerException) : base(Strings.InvalidLanguageIdException(l), innerException)
		{
			this.l = l;
		}

		protected InvalidLanguageIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.l = (string)info.GetValue("l", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("l", this.l);
		}

		public string L
		{
			get
			{
				return this.l;
			}
		}

		private readonly string l;
	}
}
