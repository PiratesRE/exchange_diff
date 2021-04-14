using System;

namespace Microsoft.Exchange.Inference.Mdb
{
	[Serializable]
	internal sealed class DeleteItemsException : Exception
	{
		public DeleteItemsException(string message) : base(message)
		{
		}

		public DeleteItemsException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public override string ToString()
		{
			return "Deletion error:" + base.ToString();
		}
	}
}
