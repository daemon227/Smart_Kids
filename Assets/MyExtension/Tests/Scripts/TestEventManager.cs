using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class TestEventManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            EventManager.Game.OnStart += Game_OnStart;
            var type = Type.GetType("Test.EventManager");
            if (type != null)
            {
                var nestedTypes = type.GetNestedTypes();
                foreach (var nestedType in nestedTypes)
                {

                    foreach (var eventInfo in nestedType.GetEvents())
                    {
                        //var attributes = eventInfo.GetCustomAttributes(typeof(EventAutoGenAttribute), true);

                        var eventHandlerType = eventInfo.EventHandlerType;

                        //eventInfo.RemoveMethod.Invoke(this, new object[1]);
                        //var methodInfo = eventHandlerType.GetMethod("GetInvocationList");

                        //methodInfo.Invoke(this, new object[1]);
                        Debug.Log(eventHandlerType.Name);
                        var g = eventHandlerType.GenericTypeArguments;
                        foreach (var t in g)
                        {
                            Debug.Log(t.Name);
                        }
                    }
                }
                
            }
            
            
        }

        private void Game_OnStart(string obj)
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
