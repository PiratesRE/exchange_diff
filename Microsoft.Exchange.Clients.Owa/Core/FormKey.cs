using System;
using System.Globalization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class FormKey
	{
		internal string Experience
		{
			get
			{
				return this.experience;
			}
			set
			{
				this.experience = value;
			}
		}

		internal ApplicationElement Application
		{
			get
			{
				return this.application;
			}
		}

		internal string Class
		{
			get
			{
				return this.itemClass;
			}
			set
			{
				this.itemClass = value;
			}
		}

		internal string Action
		{
			get
			{
				return this.action;
			}
			set
			{
				this.action = value;
			}
		}

		internal string State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		internal FormKey(ApplicationElement application, string itemClass, string action, string state)
		{
			this.experience = string.Empty;
			this.application = application;
			this.itemClass = itemClass;
			this.action = action;
			this.state = state;
		}

		internal FormKey(string experience, ApplicationElement application, string itemClass, string action, string state)
		{
			this.experience = experience;
			this.application = application;
			this.itemClass = itemClass;
			this.action = action;
			this.state = state;
		}

		public new static bool Equals(object a, object b)
		{
			FormKey formKey = a as FormKey;
			FormKey formKey2 = b as FormKey;
			return formKey != null && formKey2 != null && (formKey.Action == formKey2.Action && formKey.Application == formKey2.Application && formKey.Class == formKey2.Class && formKey.State == formKey2.State) && formKey.Experience == formKey2.Experience;
		}

		public override int GetHashCode()
		{
			return this.experience.GetHashCode() ^ this.application.GetHashCode() ^ this.itemClass.GetHashCode() ^ this.action.GetHashCode() ^ this.state.GetHashCode();
		}

		public override bool Equals(object value)
		{
			return FormKey.Equals(value, this);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Experience = {0}, Application = {1}, Class = {2}, Action = {3}, State = {4}", new object[]
			{
				this.experience,
				this.application,
				this.itemClass,
				this.action,
				this.state
			});
		}

		private string experience;

		private ApplicationElement application;

		private string itemClass;

		private string action;

		private string state;
	}
}
