using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ExCannotContainWideScopeBindingsException : InvalidCompliancePolicyExBindingException
	{
		public ExCannotContainWideScopeBindingsException(string binding) : base(Strings.ExCannotContainWideScopeBindings(binding))
		{
			this.binding = binding;
		}

		public ExCannotContainWideScopeBindingsException(string binding, Exception innerException) : base(Strings.ExCannotContainWideScopeBindings(binding), innerException)
		{
			this.binding = binding;
		}

		protected ExCannotContainWideScopeBindingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.binding = (string)info.GetValue("binding", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("binding", this.binding);
		}

		public string Binding
		{
			get
			{
				return this.binding;
			}
		}

		private readonly string binding;
	}
}
