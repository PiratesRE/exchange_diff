using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeDataHandler : DataHandler, IVersionable
	{
		public ExchangeDataHandler()
		{
		}

		public ExchangeDataHandler(ICloneable dataSource) : this()
		{
			base.DataSource = dataSource.Clone();
		}

		public ExchangeDataHandler(bool breakOnError) : this()
		{
			base.BreakOnError = breakOnError;
		}

		internal virtual ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				ExchangeObjectVersion exchangeObjectVersion = ExchangeObjectVersion.Exchange2003;
				if (base.HasDataHandlers)
				{
					using (IEnumerator<DataHandler> enumerator = base.DataHandlers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							DataHandler dataHandler = enumerator.Current;
							ExchangeDataHandler exchangeDataHandler = (ExchangeDataHandler)dataHandler;
							if (exchangeObjectVersion.IsOlderThan(((IVersionable)exchangeDataHandler).MaximumSupportedExchangeObjectVersion))
							{
								exchangeObjectVersion = ((IVersionable)exchangeDataHandler).MaximumSupportedExchangeObjectVersion;
							}
						}
						return exchangeObjectVersion;
					}
				}
				if (base.DataSource != null && this != base.DataSource && base.DataSource is IVersionable)
				{
					exchangeObjectVersion = (base.DataSource as IVersionable).MaximumSupportedExchangeObjectVersion;
				}
				return exchangeObjectVersion;
			}
		}

		ExchangeObjectVersion IVersionable.MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return this.MaximumSupportedExchangeObjectVersion;
			}
		}

		bool IVersionable.ExchangeVersionUpgradeSupported
		{
			get
			{
				return false;
			}
		}

		bool IVersionable.IsPropertyAccessible(PropertyDefinition propertyDefinition)
		{
			return true;
		}

		internal virtual ExchangeObjectVersion ExchangeVersion
		{
			get
			{
				ExchangeObjectVersion exchangeObjectVersion = ExchangeObjectVersion.Exchange2003;
				if (base.HasDataHandlers)
				{
					using (IEnumerator<DataHandler> enumerator = base.DataHandlers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							DataHandler dataHandler = enumerator.Current;
							ExchangeDataHandler exchangeDataHandler = (ExchangeDataHandler)dataHandler;
							if (exchangeObjectVersion.IsOlderThan(((IVersionable)exchangeDataHandler).ExchangeVersion))
							{
								exchangeObjectVersion = ((IVersionable)exchangeDataHandler).ExchangeVersion;
							}
						}
						return exchangeObjectVersion;
					}
				}
				if (base.DataSource != null && this != base.DataSource && base.DataSource is IVersionable)
				{
					exchangeObjectVersion = (base.DataSource as IVersionable).ExchangeVersion;
				}
				return exchangeObjectVersion;
			}
		}

		ExchangeObjectVersion IVersionable.ExchangeVersion
		{
			get
			{
				return this.ExchangeVersion;
			}
		}

		bool IVersionable.IsReadOnly
		{
			get
			{
				return base.IsObjectReadOnly;
			}
		}

		protected override void CheckObjectReadOnly()
		{
			bool isObjectReadOnly = false;
			if (base.HasDataHandlers)
			{
				using (IEnumerator<DataHandler> enumerator = base.DataHandlers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DataHandler dataHandler = enumerator.Current;
						if (dataHandler.IsObjectReadOnly)
						{
							isObjectReadOnly = true;
						}
					}
					goto IL_6C;
				}
			}
			if (base.DataSource != null && this != base.DataSource && base.DataSource is IVersionable)
			{
				isObjectReadOnly = (base.DataSource as IVersionable).IsReadOnly;
			}
			IL_6C:
			base.IsObjectReadOnly = isObjectReadOnly;
			this.SetObjectReadOnlyReason();
		}

		protected void SetObjectReadOnlyReason()
		{
			if (base.IsObjectReadOnly)
			{
				base.ObjectReadOnlyReason = Strings.VersionMismatchWarning(this.ExchangeVersion.ExchangeBuild);
				return;
			}
			base.ObjectReadOnlyReason = string.Empty;
		}

		internal virtual ObjectSchema ObjectSchema
		{
			get
			{
				return null;
			}
		}

		ObjectSchema IVersionable.ObjectSchema
		{
			get
			{
				return this.ObjectSchema;
			}
		}
	}
}
