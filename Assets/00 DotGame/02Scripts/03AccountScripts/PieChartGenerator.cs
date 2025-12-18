using UnityEngine;
using UnityEngine.UI;

namespace DACN.Account
{
    public class PieChartGenerator : MonoBehaviour
    {
        [SerializeField] private Image pieChartImage;
        [SerializeField] private Material pieChartMaterial;

        private float dotPercentage;
        private float memoryPercentage;
        private float quizPercentage;

        public void UpdateChart(float dotPercent, float memoryPercent, float quizPercent)
        {
            dotPercentage = dotPercent / 100f;
            memoryPercentage = memoryPercent / 100f;
            quizPercentage = quizPercent / 100f;

            if (pieChartImage != null && pieChartMaterial != null)
            {
                // Set material properties for pie chart rendering
                Material newMaterial = new Material(pieChartMaterial);
                newMaterial.SetFloat("_DotSize", dotPercentage);
                newMaterial.SetFloat("_MemorySize", memoryPercentage);
                newMaterial.SetFloat("_QuizSize", quizPercentage);

                // Use appropriate colors
                newMaterial.SetColor("_DotColor", new Color(1f, 0.5f, 0f)); // Orange for Dot
                newMaterial.SetColor("_MemoryColor", new Color(0f, 0.7f, 1f)); // Light blue for Memory
                newMaterial.SetColor("_QuizColor", new Color(1f, 0f, 0.7f)); // Pink for Quiz

                pieChartImage.material = newMaterial;
            }
        }

        // Alternative: Create pie chart using multiple images/panels with rotated segments
        public void UpdateChartWithImages(Transform chartContainer, float dotPercent, float memoryPercent, float quizPercent)
        {
            // Clear existing segments
            foreach (Transform child in chartContainer)
            {
                Destroy(child.gameObject);
            }

            // Create three pie segments
            CreatePieSegment(chartContainer, "Dot Game", dotPercent, new Color(1f, 0.5f, 0f), 0f);
            CreatePieSegment(chartContainer, "Memory Game", memoryPercent, new Color(0f, 0.7f, 1f), dotPercent);
            CreatePieSegment(chartContainer, "Quiz Game", quizPercent, new Color(1f, 0f, 0.7f), dotPercent + memoryPercent);
        }

        private void CreatePieSegment(Transform parent, string label, float percentage, Color color, float startAngle)
        {
            var segmentGO = new GameObject(label);
            segmentGO.transform.SetParent(parent, false);

            var image = segmentGO.AddComponent<Image>();
            image.color = color;

            var rectTransform = segmentGO.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            // Rotate segment based on start angle
            rectTransform.rotation = Quaternion.Euler(0, 0, -startAngle * 360f);
        }
    }
}
