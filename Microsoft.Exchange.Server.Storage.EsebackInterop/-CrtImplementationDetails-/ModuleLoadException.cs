using System;
using System.Runtime.Serialization;

namespace <CrtImplementationDetails>
{
	[Serializable]
	internal class ModuleLoadException : Exception
	{
		protected ModuleLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public ModuleLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public ModuleLoadException(string message) : base(message)
		{
		}

		public const string Nested = "A nested exception occurred after the primary exception that caused the C++ module to fail to load.\n";
	}
}
