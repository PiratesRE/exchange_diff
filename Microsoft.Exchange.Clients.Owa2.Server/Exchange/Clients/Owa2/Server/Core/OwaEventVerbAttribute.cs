using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class OwaEventVerbAttribute : Attribute
	{
		public OwaEventVerbAttribute(OwaEventVerb verb)
		{
			this.verb = verb;
		}

		public OwaEventVerb Verb
		{
			get
			{
				return this.verb;
			}
		}

		public static OwaEventVerb Parse(string verb)
		{
			if (verb == null)
			{
				throw new ArgumentNullException("verb");
			}
			if (string.Equals(verb, "POST", StringComparison.OrdinalIgnoreCase))
			{
				return OwaEventVerb.Post;
			}
			if (string.Equals(verb, "GET", StringComparison.OrdinalIgnoreCase))
			{
				return OwaEventVerb.Get;
			}
			return OwaEventVerb.Unsupported;
		}

		private OwaEventVerb verb = OwaEventVerb.Post;
	}
}
