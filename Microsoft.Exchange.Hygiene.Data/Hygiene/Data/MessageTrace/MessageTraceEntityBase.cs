using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal abstract class MessageTraceEntityBase : ConfigurablePropertyBag
	{
		public static byte[] EmptyEmailHashKey
		{
			get
			{
				return MessageTraceEntityBase.emptyEmailHashKey;
			}
		}

		public static byte[] EmptyEmailDomainHashKey
		{
			get
			{
				return MessageTraceEntityBase.emptyEmailDomainHashKey;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual void Accept(IMessageTraceVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.Visit(this);
		}

		public abstract HygienePropertyDefinition[] GetAllProperties();

		internal static byte[] GetEmailHashKey(string emailPrefix, string emailDomain)
		{
			if (emailPrefix == null || emailDomain == null)
			{
				return null;
			}
			return DalHelper.GetSHA1Hash(string.Format("{0}@{1}", emailPrefix.ToLower(), emailDomain.ToLower()));
		}

		internal static byte[] GetEmailDomainHashKey(string emailDomain)
		{
			if (emailDomain == null)
			{
				return null;
			}
			return DalHelper.GetSHA1Hash(emailDomain.ToLower());
		}

		internal static string StandardizeEmailPrefix(string emailPrefix)
		{
			if (!string.IsNullOrWhiteSpace(emailPrefix))
			{
				return emailPrefix;
			}
			return MessageTraceEntityBase.emptyEmailPrefix;
		}

		internal static string StandardizeEmailDomain(string emailDomain)
		{
			if (!string.IsNullOrWhiteSpace(emailDomain))
			{
				return emailDomain;
			}
			return MessageTraceEntityBase.emptyEmailDomain;
		}

		private static string emptyEmailPrefix = "<>";

		private static string emptyEmailDomain = string.Empty;

		private static byte[] emptyEmailHashKey = DalHelper.GetSHA1Hash(string.Format("{0}@{1}", MessageTraceEntityBase.emptyEmailPrefix, MessageTraceEntityBase.emptyEmailDomain));

		private static byte[] emptyEmailDomainHashKey = DalHelper.GetSHA1Hash(MessageTraceEntityBase.emptyEmailDomain);
	}
}
