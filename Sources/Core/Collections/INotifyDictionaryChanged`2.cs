// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;

namespace TerraFX.Collections
{
    /// <summary>Defines a means of listening for notifications that occur when a <see cref="IDictionary{TKey, TValue}" /> is changed.</summary>
    public interface INotifyDictionaryChanged<TKey, TValue>
    {
        #region Events
        /// <summary>Occurs when the underlying <see cref="IDictionary{TKey, TValue}" /> changes.</summary>
        event NotifyDictionaryChangedEventHandler<TKey, TValue> DictionaryChanged;
        #endregion
    }
}
