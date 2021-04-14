using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	internal class TypeMapping
	{
		internal TypeMapping(string name, Type type, LocalizedString linkedDisplayText)
		{
			this.name = name;
			this.type = type;
			this.linkedDisplayText = linkedDisplayText;
		}

		internal TypeMapping(string name, Type type, LocalizedString linkedDisplayText, LocalizedString linkedDisplayTextException)
		{
			this.name = name;
			this.type = type;
			this.linkedDisplayText = linkedDisplayText;
			this.linkedDisplayTextException = linkedDisplayTextException;
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal Type Type
		{
			get
			{
				return this.type;
			}
		}

		internal LocalizedString LinkedDisplayText
		{
			get
			{
				return this.linkedDisplayText;
			}
		}

		internal LocalizedString LinkedDisplayTextException
		{
			get
			{
				return this.linkedDisplayTextException;
			}
		}

		private readonly string name;

		private Type type;

		private LocalizedString linkedDisplayText;

		private LocalizedString linkedDisplayTextException;
	}
}
