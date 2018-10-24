// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// TabView methods related to tracking Items and ItemsSource changes.
    /// </summary>
    public partial class TabView
    {
        // Temporary tracking of previous collections for removing events.
        private MethodInfo _removeItemsSourceMethod;

        /// <inheritdoc/>
        protected override void OnItemsChanged(object e)
        {
            IVectorChangedEventArgs args = (IVectorChangedEventArgs)e;

            base.OnItemsChanged(e);

            if (args.CollectionChange == CollectionChange.ItemRemoved && SelectedIndex == -1)
            {
                // If we remove the selected item we should select the previous item
                int startIndex = (int)args.Index + 1;
                if (startIndex > Items.Count)
                {
                    startIndex = 0;
                }

                SelectedIndex = FindNextTabIndex(startIndex, -1);
            }

            // Update Sizing (in case there are less items now)
            TabView_SizeChanged(this, null);
        }

        private void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            var action = (CollectionChange)e.Action;
            if (action == CollectionChange.Reset)
            {
                // Reset collection to reload later.
                hasLoaded = false;
            }
        }

        private void SetInitialSelection()
        {
            if (SelectedItem == null)
            {
                // If we have an index, but didn't get the selection, make the selection
                if (SelectedIndex >= 0 && SelectedIndex < Items.Count())
                {
                    SelectedItem = Items[SelectedIndex];
                }

                // Otherwise, select the first item by default
                else if (Items.Count() >= 1)
                {
                    SelectedItem = Items[0];
                }
            }
        }

        // Finds the next visible & enabled tab index.
        private int FindNextTabIndex(int startIndex, int direction)
        {
            int index = startIndex;
            if (direction != 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    index += direction;

                    if (index >= Items.Count)
                    {
                        index = 0;
                    }
                    else if (index < 0)
                    {
                        index = Items.Count - 1;
                    }

                    var tabItem = ContainerFromIndex(index) as TabViewItem;
                    if (tabItem != null && tabItem.IsEnabled && tabItem.Visibility == Visibility.Visible)
                    {
                        break;
                    }
                }
            }

            return index;
        }

        private void ItemsSource_PropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            // Use reflection to store a 'Remove' method of any possible collection in ItemsSource
            // Cache for efficiency later.
            if (ItemsSource != null)
            {
                _removeItemsSourceMethod = ItemsSource.GetType().GetMethod("Remove");
            }
            else
            {
                _removeItemsSourceMethod = null;
            }
        }

        private object GetTabSource()
        {
            if (ItemsSource != null)
            {
                return ItemsSource;
            }
            else if (Items != null)
            {
                return Items;
            }

            return null;
        }
    }
}
