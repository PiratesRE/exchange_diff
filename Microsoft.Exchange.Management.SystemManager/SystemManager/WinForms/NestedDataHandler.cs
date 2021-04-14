using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class NestedDataHandler : DataHandler
	{
		public NestedDataHandler(DataContext parentContext)
		{
			this.parentContext = parentContext;
		}

		internal override void OnReadData(CommandInteractionHandler interactionHandler, string pageName)
		{
			ICloneable cloneable = (ICloneable)this.parentContext.ReadData(interactionHandler, pageName);
			int count = this.parentContext.DataHandler.DataHandlers.Count;
			if (count != 0)
			{
				this.dataSources = new object[count];
				for (int i = 0; i < count; i++)
				{
					DataHandler dataHandler = this.parentContext.DataHandler.DataHandlers[i];
					if (dataHandler.ReadOnly)
					{
						this.dataSources[i] = dataHandler.DataSource;
					}
					else if (dataHandler.DataSource is ICloneable)
					{
						this.dataSources[i] = ((ICloneable)dataHandler.DataSource).Clone();
					}
					else
					{
						this.dataSources[i] = null;
					}
				}
				base.DataSource = this.DataSources[0];
				return;
			}
			this.dataSources = new object[0];
			base.DataSource = (this.parentContext.DataHandler.ReadOnly ? cloneable : cloneable.Clone());
		}

		internal override void SpecifyParameterNames(Dictionary<object, List<string>> bindingMembers)
		{
			Dictionary<object, List<string>> dictionary = new Dictionary<object, List<string>>();
			if (this.DataSources.Length > 0)
			{
				for (int i = 0; i < this.DataSources.Length; i++)
				{
					if (this.DataSources[i] != null && bindingMembers.ContainsKey(this.DataSources[i]))
					{
						dictionary.Add(this.parentContext.DataHandler.DataHandlers[i].DataSource, bindingMembers[this.DataSources[i]]);
					}
				}
			}
			else if (base.DataSource != null && !this.parentContext.DataHandler.ReadOnly && bindingMembers.ContainsKey(base.DataSource))
			{
				dictionary.Add(this.parentContext.DataHandler.DataSource, bindingMembers[base.DataSource]);
			}
			this.parentContext.DataHandler.SpecifyParameterNames(dictionary);
		}

		internal override void OnSaveData(CommandInteractionHandler interactionHandler)
		{
			if (this.parentContext.DataHandler.DataHandlers.Count != 0)
			{
				for (int i = 0; i < this.DataSources.Length; i++)
				{
					DataHandler dataHandler = this.parentContext.DataHandler.DataHandlers[i];
					if (this.DataSources[i] != null && !dataHandler.ReadOnly)
					{
						if (i == 0)
						{
							this.parentContext.DataHandler.DataSource = this.DataSources[i];
						}
						dataHandler.DataSource = this.DataSources[i];
					}
				}
			}
			else
			{
				this.parentContext.DataHandler.DataSource = base.DataSource;
			}
			this.parentContext.IsDirty = true;
		}

		public object[] DataSources
		{
			get
			{
				return this.dataSources;
			}
		}

		public override ValidationError[] Validate()
		{
			List<ValidationError> list = new List<ValidationError>();
			if (this.DataSources.Length > 0)
			{
				foreach (object obj in this.DataSources)
				{
					if (obj is IConfigurable)
					{
						ValidationError[] array2 = (obj as IConfigurable).Validate();
						if (array2 != null)
						{
							list.AddRange(array2);
						}
					}
				}
			}
			else if (base.DataSource is IConfigurable)
			{
				ValidationError[] array2 = (base.DataSource as IConfigurable).Validate();
				if (array2 != null)
				{
					list.AddRange(array2);
				}
			}
			return list.ToArray();
		}

		internal override bool TimeConsuming
		{
			get
			{
				return false;
			}
		}

		private DataContext parentContext;

		private object[] dataSources;
	}
}
