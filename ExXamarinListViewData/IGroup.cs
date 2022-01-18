using System;

namespace ExXamarinListViewData
{
    /// <summary>
    /// Represents the behavior required from a group item in the list view.
    /// </summary>
    /// <typeparam name="TGroupKey">
    /// Specifies the type of the GroupKey.
    /// Must implement IEquatable, so that keys can be tested for equality.
    /// </typeparam>
    public interface IGroup<TGroupKey> where TGroupKey : IEquatable<TGroupKey>
    {
        TGroupKey GroupKey { get; }
    }
}
