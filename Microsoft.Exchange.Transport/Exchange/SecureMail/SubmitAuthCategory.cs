using System;

namespace Microsoft.Exchange.SecureMail
{
	internal class SubmitAuthCategory
	{
		private SubmitAuthCategory(string name, bool recordDomain)
		{
			this.name = name;
			this.logFormat = "{0:x2}" + name.Substring(0, 1) + (recordDomain ? ":{1}" : ":");
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public bool IsAnonymous
		{
			get
			{
				return this == SubmitAuthCategory.Anonymous;
			}
		}

		public bool IsPartner
		{
			get
			{
				return this == SubmitAuthCategory.Partner;
			}
		}

		public static string FormatExisting(MultilevelAuthMechanism mechanism, string category)
		{
			if (string.IsNullOrEmpty(category))
			{
				category = SubmitAuthCategory.Other.Name;
			}
			return string.Format("{0:x2}{1}:", (int)mechanism, category.Substring(0, 1));
		}

		public bool Matches(string optCategoryName)
		{
			return string.Equals(optCategoryName, this.Name, StringComparison.InvariantCultureIgnoreCase);
		}

		public override string ToString()
		{
			return this.name;
		}

		public string FormatLog(MultilevelAuthMechanism mechanism, string authDomain)
		{
			if (!MultilevelAuth.MayWriteAuthDomain(mechanism))
			{
				authDomain = null;
			}
			return string.Format(this.logFormat, (int)mechanism, authDomain);
		}

		public static readonly SubmitAuthCategory Anonymous = new SubmitAuthCategory("Anonymous", false);

		public static readonly SubmitAuthCategory Other = new SubmitAuthCategory("Other", true);

		public static readonly SubmitAuthCategory Internal = new SubmitAuthCategory("Internal", true);

		public static readonly SubmitAuthCategory External = new SubmitAuthCategory("External", true);

		public static readonly SubmitAuthCategory Partner = new SubmitAuthCategory("Partner", true);

		private string name;

		private string logFormat;
	}
}
