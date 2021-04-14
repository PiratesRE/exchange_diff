using System;
using System.ComponentModel;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class MonadDataHandler : SingleTaskDataHandler
	{
		public MonadDataHandler(Type configObjectType) : this(null, configObjectType)
		{
		}

		public MonadDataHandler(object objectID, Type configObjectType) : this(objectID, "get-" + configObjectType.Name.ToLowerInvariant(), "set-" + configObjectType.Name.ToLowerInvariant())
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public MonadDataHandler(object objectID, string selectCommandText, string updateCommandText) : base(updateCommandText)
		{
			base.DataSource = null;
			this.Identity = objectID;
			this.selectCommand = new LoggableMonadCommand(selectCommandText, base.Connection);
		}

		[DefaultValue(null)]
		public object Identity
		{
			get
			{
				return this.identity;
			}
			set
			{
				this.identity = value;
			}
		}

		internal MonadCommand SelectCommand
		{
			get
			{
				return this.selectCommand;
			}
		}

		private bool HasIdentity()
		{
			bool flag = null == this.Identity;
			bool flag2 = this.Identity is string && string.IsNullOrEmpty(this.Identity as string);
			return !flag && !flag2;
		}

		protected override void OnReadData()
		{
			if (!string.IsNullOrEmpty(this.selectCommand.CommandText))
			{
				this.selectCommand.Parameters.Remove("Identity");
				if (this.HasIdentity())
				{
					this.selectCommand.Parameters.AddWithValue("Identity", this.Identity);
				}
				object[] array = this.selectCommand.Execute();
				switch (array.Length)
				{
				case 1:
					base.DataSource = (IConfigurable)array[0];
					break;
				}
			}
			base.OnReadData();
		}

		protected override void AddParameters()
		{
			base.Parameters.Remove("Instance");
			base.Parameters.AddWithValue("Instance", base.DataSource);
		}

		public override void Cancel()
		{
			base.Cancel();
			this.selectCommand.Cancel();
		}

		public override ValidationError[] Validate()
		{
			if (!this.IsModified())
			{
				return new ValidationError[0];
			}
			return base.Validate();
		}

		protected override void HandleIdentityParameter()
		{
			if (this.HasIdentity())
			{
				base.HandleIdentityParameter();
			}
		}

		protected override bool IsModified()
		{
			return base.ParameterNames.Count != 0;
		}

		private MonadCommand selectCommand;

		private object identity;
	}
}
