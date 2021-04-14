using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MaximumUriRedirectionsReachedException : LocalizedException
	{
		public MaximumUriRedirectionsReachedException(int maximumUriRedirections) : base(AuthenticationStrings.MaximumUriRedirectionsReachedException(maximumUriRedirections))
		{
			this.maximumUriRedirections = maximumUriRedirections;
		}

		public MaximumUriRedirectionsReachedException(int maximumUriRedirections, Exception innerException) : base(AuthenticationStrings.MaximumUriRedirectionsReachedException(maximumUriRedirections), innerException)
		{
			this.maximumUriRedirections = maximumUriRedirections;
		}

		protected MaximumUriRedirectionsReachedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.maximumUriRedirections = (int)info.GetValue("maximumUriRedirections", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("maximumUriRedirections", this.maximumUriRedirections);
		}

		public int MaximumUriRedirections
		{
			get
			{
				return this.maximumUriRedirections;
			}
		}

		private readonly int maximumUriRedirections;
	}
}
