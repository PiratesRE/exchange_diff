using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class BindingCannotCombineAllWithIndividualBindingsException : LocalizedException
	{
		public BindingCannotCombineAllWithIndividualBindingsException(string workLoad) : base(Strings.BindingCannotCombineAllWithIndividualBindings(workLoad))
		{
			this.workLoad = workLoad;
		}

		public BindingCannotCombineAllWithIndividualBindingsException(string workLoad, Exception innerException) : base(Strings.BindingCannotCombineAllWithIndividualBindings(workLoad), innerException)
		{
			this.workLoad = workLoad;
		}

		protected BindingCannotCombineAllWithIndividualBindingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.workLoad = (string)info.GetValue("workLoad", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("workLoad", this.workLoad);
		}

		public string WorkLoad
		{
			get
			{
				return this.workLoad;
			}
		}

		private readonly string workLoad;
	}
}
