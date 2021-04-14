using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class ExtendedRuleConditionConstraint : StoreObjectConstraint
	{
		private static int GetExtendedRuleSizeLimit(StoreSession storeSession)
		{
			storeSession.Mailbox.Load(new PropertyDefinition[]
			{
				MailboxSchema.ExtendedRuleSizeLimit
			});
			return storeSession.Mailbox.GetValueOrDefault<int>(MailboxSchema.ExtendedRuleSizeLimit, 522240);
		}

		public static void InitExtendedRuleSizeLimitIfNeeded(MailboxSession originalSession)
		{
			if (originalSession.LogonType != LogonType.BestAccess && originalSession.LogonType != LogonType.Delegated && originalSession.LogonType != LogonType.Owner)
			{
				return;
			}
			int? num = originalSession.Mailbox.TryGetProperty(MailboxSchema.ExtendedRuleSizeLimit) as int?;
			if (num == null || num > 522240)
			{
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(originalSession.MailboxOwner, originalSession.InternalCulture, "Client=Management;Action=InitExtendedRuleSizeLimit"))
				{
					mailboxSession.Mailbox.SetProperties(new PropertyDefinition[]
					{
						MailboxSchema.ExtendedRuleSizeLimit
					}, new object[]
					{
						522240
					});
					CoreObject.GetPersistablePropertyBag((CoreMailboxObject)mailboxSession.Mailbox.CoreObject).FlushChanges();
				}
				originalSession.Mailbox.ForceReload(new PropertyDefinition[]
				{
					MailboxSchema.ExtendedRuleSizeLimit
				});
			}
		}

		public static void ValidateStreamIfApplicable(long streamLength, PropertyDefinition propertyDefinition, StoreObjectPropertyBag propertyBag)
		{
			if (ExtendedRuleConditionConstraint.propertyDefinition.Equals(propertyDefinition) && propertyBag != null && propertyBag.MapiPropertyBag != null && propertyBag.MapiPropertyBag.StoreSession != null)
			{
				int extendedRuleSizeLimit = ExtendedRuleConditionConstraint.GetExtendedRuleSizeLimit(propertyBag.MapiPropertyBag.StoreSession);
				if (streamLength > (long)extendedRuleSizeLimit)
				{
					throw new StoragePermanentException(ServerStrings.ExConstraintViolationByteArrayLengthTooLong(propertyDefinition.Name, (long)extendedRuleSizeLimit, streamLength));
				}
			}
		}

		public ExtendedRuleConditionConstraint() : base(new PropertyDefinition[]
		{
			ExtendedRuleConditionConstraint.propertyDefinition
		})
		{
		}

		internal override StoreObjectValidationError Validate(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			if (validatablePropertyBag is PropertyBag && validatablePropertyBag.IsPropertyDirty(ExtendedRuleConditionConstraint.propertyDefinition))
			{
				byte[] array = validatablePropertyBag.TryGetProperty(ExtendedRuleConditionConstraint.propertyDefinition) as byte[];
				StoreSession session = ((PropertyBag)validatablePropertyBag).Context.Session;
				if (array != null && session != null && array.Length > ExtendedRuleConditionConstraint.GetExtendedRuleSizeLimit(session))
				{
					return new StoreObjectValidationError(context, ExtendedRuleConditionConstraint.propertyDefinition, array, this);
				}
			}
			return null;
		}

		public override string ToString()
		{
			return string.Format("Property {0} has length constraint.", ExtendedRuleConditionConstraint.propertyDefinition.Name);
		}

		private const string InitExtendedRuleSizeLimitSessionClientString = "Client=Management;Action=InitExtendedRuleSizeLimit";

		private const int DefaultExtendedRuleSizeLimit = 522240;

		private static readonly StorePropertyDefinition propertyDefinition = InternalSchema.ExtendedRuleCondition;
	}
}
