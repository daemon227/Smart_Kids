using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DotPuzzle.Editor
{
    public partial class EditorEventManager
    {
        public static void ClearAllEventHandlers()
        {
            EditorEvents.ClearEventHandlers();
            LevelEvents.ClearEventHandlers();
            EntityEvents.ClearEventHandlers();
        }
    }
}

