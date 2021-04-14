using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EncryptedCondition : StringCondition
	{
		private EncryptedCondition(Rule rule, string[] text) : base(ConditionType.EncryptedCondition, rule, text)
		{
		}

		public static EncryptedCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			string[] text = new string[]
			{
				"IPM.Note.Secure",
				"IPM.Note" + "." + "SMIME.SignedEncrypted",
				"IPM.Note" + "." + "SMIME.Encrypted"
			};
			return new EncryptedCondition(rule, text);
		}

		internal override Restriction BuildRestriction()
		{
			Restriction restriction = Condition.CreateORStringContentRestriction(base.Text, PropTag.MessageClass, ContentFlags.IgnoreCase | ContentFlags.Loose);
			if (Restriction.ResType.Content == restriction.Type)
			{
				return Restriction.Or(new Restriction[]
				{
					restriction
				});
			}
			return restriction;
		}
	}
}
