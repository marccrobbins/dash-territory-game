using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public interface IInitializable
    {
        bool IsInitialized { get; }
        bool IsInitializing { get; }
        void Initialize();
    }
}
