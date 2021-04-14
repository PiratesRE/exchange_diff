using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CorruptionTypeParameterIncompatibleException : LocalizedException
	{
		public CorruptionTypeParameterIncompatibleException(string paramName) : base(Strings.CorruptionTypeParameterIncompatible(paramName))
		{
			this.paramName = paramName;
		}

		public CorruptionTypeParameterIncompatibleException(string paramName, Exception innerException) : base(Strings.CorruptionTypeParameterIncompatible(paramName), innerException)
		{
			this.paramName = paramName;
		}

		protected CorruptionTypeParameterIncompatibleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.paramName = (string)info.GetValue("paramName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("paramName", this.paramName);
		}

		public string ParamName
		{
			get
			{
				return this.paramName;
			}
		}

		private readonly string paramName;
	}
}
