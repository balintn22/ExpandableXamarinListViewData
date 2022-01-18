using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExXamarinListViewData
{
    /// <summary>
    /// When implemented in a derived type, represents a group of items
    /// (parent level) in the groupable ListView data.
    /// Supports a child collection end Expand out of the box.
    /// </summary>
    /// <typeparam name="TItem">Specifies the type of child items in this group.</typeparam>
    /// <typeparam name="TGroupKey">Specifies the type of the group key.</typeparam>
    public abstract class XamarinListViewGroup<TItem, TGroupKey>
        : ObservableRangeCollection<TItem>, INotifyPropertyChanged, IGroup<TGroupKey>
        where TItem : IEquatable<TItem>, IGroupable<TGroupKey>
        where TGroupKey : IEquatable<TGroupKey>
    {
        /// <summary>Holds child items regardless of Expanded state.</summary>
        public List<TItem> Items2 { get => Items2; }
        public int ChildCount { get => Items2.Count; }

        private bool _expanded;
        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    if (_expanded)
                        base.AddRange(Items2);
                    else
                        base.Clear();
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Expanded)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
                }
            }
        }

        /// <summary>
        /// Callback method executed when the last item from the group is removed.
        /// </summary>
        public Action<TGroupKey> OnLastItemRemoved { get; set; }

        private TGroupKey _groupKey = default(TGroupKey);
        public TGroupKey GroupKey
        {
            get
            {
                if (EqualityComparer<TGroupKey>.Default.Equals(_groupKey, default(TGroupKey)))
                {
                    if (Items2.Count > 0)
                        _groupKey = Items2[0].GroupKey;
                }
                return _groupKey;
            }
        }

        public XamarinListViewGroup(
            IEnumerable<TItem> items,
            bool expanded,
            Action<TGroupKey> onLastItemRemoved = null)
        {
            _expanded = expanded;
            Items2 = items.ToList();
            OnLastItemRemoved = onLastItemRemoved;

            if (_expanded)
                base.AddRange(items);
        }

        public new void Add(TItem item)
        {
            Items2.Add(item);
            if (_expanded)
            {
                base.Add(item);
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ChildCount)));
        }

        public new void Remove(TItem item)
        {
            Items2.Remove(item);
            if (_expanded)
            {
                base.Remove(item);
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ChildCount)));

            if (Items2.Count == 0 && OnLastItemRemoved != null)
                OnLastItemRemoved(this.GroupKey);
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            Items2.AddRange(items);
            if (_expanded)
            {
                base.AddRange(items);
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ChildCount)));
        }

        public void RemoveRange(IEnumerable<TItem> items)
        {
            foreach(var item in items)
                Items2.Remove(item);

            if (_expanded)
            {
                base.RemoveRange(items);
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ChildCount)));

            if (Items2.Count == 0 && OnLastItemRemoved != null)
                OnLastItemRemoved(this.GroupKey);
        }
    }
}
