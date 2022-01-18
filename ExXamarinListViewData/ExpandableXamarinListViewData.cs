using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExXamarinListViewData
{
    public class ExpandableXamarinListViewData<TGroup, TItem, TGroupKey>
        : ObservableRangeCollection<TGroup>
        where TGroup : XamarinListViewGroup<TItem, TGroupKey>, IGroup<TGroupKey>
        where TItem : IEquatable<TItem>, IGroupable<TGroupKey>
        where TGroupKey : IEquatable<TGroupKey>
    {
        // Group operations like Add, Remove AddRange, RemoveRange are implemented by parent class.

        public void ExpandGroup(TGroup group, bool collapseOthers)
        {
            var memberGroup = GetGroup(group.GroupKey);

            if (collapseOthers)
            {
                bool anyChanges = false;
                foreach (var item in Items)
                {
                    if (item.GroupKey.Equals(group.GroupKey))
                    {
                        if (!item.Expanded)
                        {
                            item.Expanded = true;
                            anyChanges = true;
                        }
                    }
                    else
                    {
                        if (item.Expanded)
                        {
                            item.Expanded = false;
                            anyChanges = true;
                        }
                    }
                }

                if (anyChanges)
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
            }
            else
            {
                bool wasExpanded = memberGroup.Expanded;
                memberGroup.Expanded = true;
                if(wasExpanded != memberGroup.Expanded)
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
            }
        }

        public void CollapseGroup(TGroup group)
        {
            var memberGroup = GetGroup(group.GroupKey);
            bool wasExpanded = memberGroup.Expanded;
            memberGroup.Expanded = false;
            if (wasExpanded != memberGroup.Expanded)
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
        }

        private void OnLastItemRemovedFromGroup(TGroupKey groupKey)
        {
            var group = Items.FirstOrDefault(x => groupKey.Equals(x.GroupKey));
            if(group != null)
                Remove(group);
        }

        #region Group Operations

        protected override void InsertItem(int index, TGroup group)
        {
            group.OnLastItemRemoved = OnLastItemRemovedFromGroup;
            base.InsertItem(index, group);
        }

        #endregion Group Operations

        #region Item Operations

        public void Add(TItem item)
        {
            var group = GetGroup(item.GroupKey);
            group.Add(item);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
        }

        public void Remove(TItem item)
        {
            var group = GetGroup(item.GroupKey);
            group.Remove(item);
            if (group.Expanded)
            {
                // Slight oversight: if the group is expanded and the item does not actually exist,
                // the PropertyChange is mis-fired.
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
            }
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            bool anyChanges = false;
            foreach (var item in items)
            {
                var group = GetGroup(item.GroupKey);
                group.Add(item);
                anyChanges = true;
            }

            if (anyChanges)
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
        }

        public void RemoveRange(IEnumerable<TItem> items)
        {
            bool anyChanges = false;
            foreach (var item in items)
            {
                var group = Items.FirstOrDefault(x => x.GroupKey.Equals(item.GroupKey));
                if (group != null)
                {
                    group.Remove(item);
                    anyChanges = true;
                }
            }

            if (anyChanges)
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
        }

        #endregion Item Operations

        /// <summary>
        /// Finds a group in this collection.
        /// Throws an exception if the group doesn't exist.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if a group with groupKey doesn't exist.</exception>
        private TGroup GetGroup(TGroupKey groupKey)
        {
            var result = this.FirstOrDefault(g => g.GroupKey.Equals(groupKey));
            if (result == null)
                throw new ArgumentException($"A group with GroupKey '{groupKey}' was not found.");

            return result;
        }

        /// <summary>
        /// Enumerates through all items within each group.
        /// </summary>
        public IEnumerable<TItem> AllItems
        {
            get
            {
                foreach (TGroup group in this)
                {
                    foreach(TItem item in group.Items2)
                        yield return item;
                }
            }
        }
    }
}
