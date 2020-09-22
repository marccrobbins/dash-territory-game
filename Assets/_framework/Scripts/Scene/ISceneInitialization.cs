using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public interface ISceneInitialization
    {
//        void LoadingContent();
        void ContentLoaded();
        void UnloadingContent();
//        void ContentUnloaded();
    }
}