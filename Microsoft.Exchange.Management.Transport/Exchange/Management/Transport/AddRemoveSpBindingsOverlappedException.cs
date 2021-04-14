using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AddRemoveSpBindingsOverlappedException : InvalidCompliancePolicySpBindingException
	{
		public AddRemoveSpBindingsOverlappedException(string bindings) : base(Strings.ErrorAddRemoveSpBindingsOverlapped(bindings))
		{
			this.bindings = bindings;
		}

		public AddRemoveSpBindingsOverlappedException(string bindings, Exception innerException) : base(Strings.ErrorAddRemoveSpBindingsOverlapped(bindings), innerException)
		{
			this.bindings = bindings;
		}

		protected AddRemoveSpBindingsOverlappedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
