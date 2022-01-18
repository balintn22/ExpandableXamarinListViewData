using System;

namespace ExXamarinListViewData
{
    /// <summary>
    /// Declares capability to be grouped, meaning it is possible
    /// to get a group key from the implementing type.
    /// </summary>
    /// <typeparam name="TGroupKey">
    /// Specifies the type of the GroupKey.
    /// Must implement IEquatable, so that keys can be tested for equality.
    /// </typeparam>
    public interface IGroupable<TGroupKey> where TGroupKey : IEquatable<TGroupKey>
    {
        TGroupKey GroupKey { get; }
    }
}
