using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class EsentInvalidColumnException : EsentException
	{
		public EsentInvalidColumnException()
		{
		}

		protected EsentInvalidColumnException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override string Message
		{
			get
			{
				return "Column is not valid for this operation";
			}
		}
	}
}
