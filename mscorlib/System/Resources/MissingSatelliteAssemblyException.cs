using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Resources
{
	[ComVisible(true)]
	[Serializable]
	public class MissingSatelliteAssemblyException : SystemException
	{
		public MissingSatelliteAssemblyException() : base(Environment.GetResourceString("MissingSatelliteAssembly_Default"))
		{
			base.SetErrorCode(-2146233034);
		}

		public MissingSatelliteAssemblyException(string message) : base(message)
		{
			base.SetErrorCode(-2146233034);
		}

		public MissingSatelliteAssemblyException(string message, string cultureName) : base(message)
		{
			base.SetErrorCode(-2146233034);
			this._cultureName = cultureName;
		}

		public MissingSatelliteAssemblyException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233034);
		}

		protected MissingSatelliteAssemblyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public string CultureName
		{
			get
			{
				return this._cultureName;
			}
		}

		private string _cultureName;
	}
}
