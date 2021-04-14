using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class TimeZoneException : LocalizedException
	{
		public TimeZoneException(Exception exception) : base(Strings.GetLocalizedString(Strings.IDs.ErrorTimeZone), exception)
		{
		}

		public TimeZoneException(Enum messageId) : base(Strings.GetLocalizedString((Strings.IDs)messageId))
		{
		}

		public TimeZoneException(Enum messageId, Exception exception) : base(Strings.GetLocalizedString((Strings.IDs)messageId), exception)
		{
		}

		public TimeZoneException(Enum messageId, Exception innerException, ArrayOfTransitionsType transitionsGroup, TransitionType transition, string param, string paramValue) : this(messageId, innerException)
		{
			this.AddParam(param, paramValue);
			this.AddTransitionReferenceInfo(transition, transitionsGroup);
		}

		public TimeZoneException(Enum messageId, Exception innerException, ArrayOfTransitionsType transitionsGroup, TransitionType transition) : this(messageId, innerException)
		{
			this.AddTransitionReferenceInfo(transition, transitionsGroup);
		}

		public TimeZoneException(Enum messageId, ArrayOfTransitionsType transitionsGroup, TransitionType transition) : this(messageId)
		{
			this.AddTransitionReferenceInfo(transition, transitionsGroup);
		}

		public TimeZoneException(Enum messageId, string[] paramNames, string[] paramValues) : this(messageId)
		{
			this.AddParams(paramNames, paramValues);
		}

		public IDictionary<string, string> ConstantValues
		{
			get
			{
				return this.constantValues;
			}
		}

		private static string TransitionTypeToString(TransitionType transition)
		{
			if (transition != null)
			{
				transition.GetType();
				return transition.GetType().Name;
			}
			return "UnknownTansition";
		}

		private void AddTransitionReferenceInfo(ArrayOfTransitionsType transitionsGroup)
		{
			if (!string.IsNullOrEmpty(transitionsGroup.Id))
			{
				this.AddParam(transitionsGroup.Name + ".Id", transitionsGroup.Id);
			}
		}

		private void AddTransitionReferenceInfo(TransitionType transition, ArrayOfTransitionsType transitionsGroup)
		{
			string text = transitionsGroup.Name + "." + TimeZoneException.TransitionTypeToString(transition) + ".To";
			this.AddTransitionReferenceInfo(transitionsGroup);
			this.AddParam(text + ".Kind", transition.To.Kind.ToString());
			this.AddParam(text, transition.To.Value);
		}

		private void AddParam(string paramName, string paramValue)
		{
			if (paramName != null && !this.ConstantValues.ContainsKey(paramName))
			{
				this.ConstantValues.Add(paramName, paramValue);
			}
		}

		private void AddParams(string[] paramNames, string[] paramValues)
		{
			if (paramNames != null)
			{
				for (int i = 0; i < paramNames.Length; i++)
				{
					if (paramNames[i] != null && !this.ConstantValues.ContainsKey(paramNames[i]))
					{
						this.ConstantValues.Add(paramNames[i], (i < paramValues.Length) ? paramValues[i] : null);
					}
				}
			}
		}

		private Dictionary<string, string> constantValues = new Dictionary<string, string>();
	}
}
