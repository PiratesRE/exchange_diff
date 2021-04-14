using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Compliance.Serialization.Formatters
{
	internal sealed class BlockedTypeException : SerializationException
	{
		public BlockedTypeException(string type)
		{
			this.typeName = type;
		}

		public override string Message
		{
			get
			{
				return "The type to be (de)serialized is not allowed: " + this.typeName;
			}
		}

		private readonly string typeName;
	}
}
