using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedCustomGreetingSizeFormatException : PublishingException
	{
		public UnsupportedCustomGreetingSizeFormatException(string minutes) : base(Strings.UnsupportedCustomGreetingSizeFormat(minutes))
		{
			this.minutes = minutes;
		}

		public UnsupportedCustomGreetingSizeFormatException(string minutes, Exception innerException) : base(Strings.UnsupportedCustomGreetingSizeFormat(minutes), innerException)
		{
			this.minutes = minutes;
		}

		protected UnsupportedCustomGreetingSizeFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.minutes = (string)info.GetValue("minutes", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("minutes", this.minutes);
		}

		public string Minutes
		{
			get
			{
				return this.minutes;
			}
		}

		private readonly string minutes;
	}
}
