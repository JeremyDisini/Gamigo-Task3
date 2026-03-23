using UnityEngine;
using UnityEngine.UI;

namespace TestTask.Editable
{
    public class ClientColors : MonoBehaviour
    {
        [SerializeField] private Transform colorContainer;

        //public method to request colors, triggered by buttons
        public void RequestColors(int count)
        {
            ClientPacketsHandler.SendColorRequest(count);
        }

        //create new UI object and set it's image color to the desired color
        private void CreateColorImage(Color32 color)
        {
            GameObject newColor = new GameObject("color");
            newColor.AddComponent<RectTransform>();
            newColor.transform.SetParent(colorContainer);
            newColor.transform.localScale = Vector2.one;
            newColor.AddComponent<RawImage>().color = color;
        }

        //Helper function to delete all color images from the panel
        private void ClearColorImages()
        {
            for(int i = colorContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(colorContainer.GetChild(i).gameObject);
            }
        }

        //called every time a color response is received from the server
        public void OnReceivedColors(byte[] colors)
        {
            ClearColorImages();
            
            //unwrap byte array into color32
            for(int i = 0; i < colors.Length; i += 3)
            {
                Color32 newColor = new Color32(colors[i], colors[i + 1], colors[i + 2], byte.MaxValue);
                CreateColorImage(newColor);
            }      
        }
    }
}
