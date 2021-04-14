using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeServerNotValidException : ExchangeServerNotFoundException
	{
		public ExchangeServerNotValidException(string name) : base(Strings.ExceptionExchangeServerNotValid(name))
		{
			this.name = name;
		}

		public ExchangeServerNotValidException(string name, Exception innerException) : base(Strings.ExceptionExchangeServerNotValid(name), innerException)
		{
			this.name = name;
		}

		protected ExchangeServerNotValidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
