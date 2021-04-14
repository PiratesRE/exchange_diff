using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreInstanceComponentNotInitializedException : DxStoreInstanceServerException
	{
		public DxStoreInstanceComponentNotInitializedException(string component) : base(Strings.DxStoreInstanceComponentNotInitialized(component))
		{
			this.component = component;
		}

		public DxStoreInstanceComponentNotInitializedException(string component, Exception innerException) : base(Strings.DxStoreInstanceComponentNotInitialized(component), innerException)
		{
			this.component = component;
		}

		protected DxStoreInstanceComponentNotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.component = (string)info.GetValue("component", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("component", this.component);
		}

		public string Component
		{
			get
			{
				return this.component;
			}
		}

		private readonly string component;
	}
}
