using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentUnloadableOSFunctionalityException : EsentFatalException
	{
		public EsentUnloadableOSFunctionalityException() : base("The desired OS functionality could not be located and loaded / linked.", JET_err.UnloadableOSFunctionality)
		{
		}

		private EsentUnloadableOSFunctionalityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
