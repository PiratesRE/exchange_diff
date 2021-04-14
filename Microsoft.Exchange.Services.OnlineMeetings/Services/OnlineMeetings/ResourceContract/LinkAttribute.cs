using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	internal class LinkAttribute : Attribute
	{
		public LinkAttribute()
		{
		}

		public LinkAttribute(string token) : this(token, false)
		{
		}

		public LinkAttribute(string token, bool isRequired)
		{
			this.token = token;
			this.isRequired = isRequired;
		}

		public bool IsRequired
		{
			get
			{
				return this.isRequired;
			}
		}

		public string Token
		{
			get
			{
				return this.token;
			}
		}

		public string ContextToken
		{
			get
			{
				return this.contextToken;
			}
			set
			{
				this.contextToken = value;
			}
		}

		public string SampleHrefContext
		{
			get
			{
				return this.sampleHrefContext;
			}
			set
			{
				this.sampleHrefContext = value;
			}
		}

		public string Rel
		{
			get
			{
				return this.rel;
			}
			set
			{
				this.rel = value;
			}
		}

		public string OrGroupName
		{
			get
			{
				return this.orGroupName;
			}
			set
			{
				this.orGroupName = value;
			}
		}

		public bool AppliesToToken(string token)
		{
			return this.contextToken == null || token == null || string.Compare(this.contextToken, token, StringComparison.CurrentCultureIgnoreCase) == 0;
		}

		private readonly bool isRequired;

		private readonly string token;

		private string rel = "related";

		private string contextToken;

		private string orGroupName;

		private string sampleHrefContext;
	}
}
