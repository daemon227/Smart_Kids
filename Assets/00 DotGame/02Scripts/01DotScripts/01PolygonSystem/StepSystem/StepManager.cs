
using System.Linq;
namespace Inwave.DongA.Manager
{
    public class StepManager
    {
        public void ShowVerticesOfStep(StepData stepData, bool isShow)
        {
            foreach (var polygon in stepData.polygons)
            {
                foreach (var vertice in polygon.vertices)
                {
                    vertice.gameObject.SetActive(isShow);
                }
            }
        }
        public bool IsStepComplete(StepData stepData)
        {
            var polygon = stepData.polygons.FirstOrDefault(p => p.IsComplete == false);
            if (polygon == null) return true;
            return false;
        }
    }
}

