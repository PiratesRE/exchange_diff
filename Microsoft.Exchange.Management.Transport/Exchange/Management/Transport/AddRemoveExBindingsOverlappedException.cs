using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AddRemoveExBindingsOverlappedException : InvalidCompliancePolicyExBindingException
	{
		public AddRemoveExBindingsOverlappedException(string bindings) : base(Strings.ErrorAddRemoveExBindingsOverlapped(bindings))
		{
			this.bindings = bindings;
		}

		public AddRemoveExBindingsOverlappedException(string bindings, Exception innerException) : base(Strings.ErrorAddRemoveExBindingsOverlapped(bindings), innerException)
		{
			this.bindings = bindings;
		}

		protected AddRemoveExBindingsOverlappedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.bindings = (string)info.GetValue("bindings", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("bindings", this.bindings);
		}

		public string Bindings
		{
			get
			{
				return this.bindings;
			}
		}

		private readonly string bindings;
	}
}
