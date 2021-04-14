using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class IsSameUserPredicate : PredicateCondition
	{
		public IsSameUserPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
			if (!base.Property.IsString)
			{
				throw new RulesValidationException(RulesStrings.StringPropertyOrValueRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "isSameUser";
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			BaseTransportRulesEvaluationContext baseTransportRulesEvaluationContext = (BaseTransportRulesEvaluationContext)baseContext;
			if (baseTransportRulesEvaluationContext == null)
			{
				throw new ArgumentException("context is either null or not of type: BaseTransportRulesEvaluationContext");
			}
			baseTransportRulesEvaluationContext.PredicateName = this.Name;
			object value = base.Property.GetValue(baseTransportRulesEvaluationContext);
			object conditionValue = this.GetConditionValue(baseTransportRulesEvaluationContext);
			List<string> list = new List<string>();
			IsSameUserPredicate.LoadExactMatchForIsSameUserPredicateInEOP();
			bool flag = RuleUtils.CompareStringValues(conditionValue, value, (DatacenterRegistry.IsForefrontForOffice() && IsSameUserPredicate.disableExactMatchInEOP != 1) ? ExactUserComparer.CreateInstance() : baseTransportRulesEvaluationContext.UserComparer, base.EvaluationMode, list);
			base.UpdateEvaluationHistory(baseContext, flag, list, 0);
			return flag;
		}

		protected virtual object GetConditionValue(BaseTransportRulesEvaluationContext context)
		{
			return base.Value.GetValue(context);
		}

		private static void LoadExactMatchForIsSameUserPredicateInEOP()
		{
			if (!IsSameUserPredicate.disableExactMatchInEOPRegkeyLoaded)
			{
				try
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport"))
					{
						if (registryKey != null)
						{
							IsSameUserPredicate.disableExactMatchInEOP = (int)registryKey.GetValue("disableExactMatchInEOP", 0);
						}
					}
				}
				catch (SecurityException)
				{
				}
				catch (IOException)
				{
				}
				catch (UnauthorizedAccessException)
				{
				}
				catch (InvalidCastException)
				{
				}
				finally
				{
					IsSameUserPredicate.disableExactMatchInEOPRegkeyLoaded = true;
				}
			}
		}

		private static volatile bool disableExactMatchInEOPRegkeyLoaded;

		private static volatile int disableExactMatchInEOP;
	}
}
