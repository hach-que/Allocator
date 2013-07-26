using System;

namespace AllocatorLib
{
    public interface ILease
    {
        int ID { get; }
        DateTime? Expiry { get; }
        void Release();
    }
}
