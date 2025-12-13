using UnityEngine;

public static class MyTool
{
    public static int GetRandomNumberInt(int startNumber, int endNumber)
    {
        return Random.Range(startNumber, endNumber);
    }
    public static float GetRandomNumberFloat(float startNumber, float endNumber)
    {
        return Random.Range(startNumber, endNumber);
    }
}
