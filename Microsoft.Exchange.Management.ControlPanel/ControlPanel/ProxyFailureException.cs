using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ProxyFailureException : ProxyException
	{
		public ProxyFailureException(Uri proxyTarget, ADObjectId user) : base(Strings.ProxyFailure(proxyTarget, user))
		{
			this.proxyTarget = proxyTarget;
			this.user = user;
		}

		public ProxyFailureException(Uri proxyTarget, ADObjectId user, Exception innerException) : base(Strings.ProxyFailure(proxyTarget, user), innerException)
		{
			this.proxyTarget = proxyTarget;
			this.user = user;
		}

		protected ProxyFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.proxyTarget = (Uri)info.GetValue("proxyTarget", typeof(Uri));
			this.user = (ADObjectId)info.GetValue("user", typeof(ADObjectId));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("proxyTarget", this.proxyTarget);
			info.AddValue("user", this.user);
		}

		public Uri ProxyTarget
		{
			get
			{
				return this.proxyTarget;
			}
		}

		public ADObjectId User
		{
			get
			{
				return this.user;
			}
		}

		private readonly Uri proxyTarget;

		private readonly ADObjectId user;
	}
}
