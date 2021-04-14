using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidSubnetSpecException : ClusCommonValidationFailedException
	{
		public InvalidSubnetSpecException(string userInput) : base(Strings.InvalidSubnetSpec(userInput))
		{
			this.userInput = userInput;
		}

		public InvalidSubnetSpecException(string userInput, Exception innerException) : base(Strings.InvalidSubnetSpec(userInput), innerException)
		{
			this.userInput = userInput;
		}

		protected InvalidSubnetSpecException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userInput = (string)info.GetValue("userInput", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userInput", this.userInput);
		}

		public string UserInput
		{
			get
			{
				return this.userInput;
			}
		}

		private readonly string userInput;
	}
}
