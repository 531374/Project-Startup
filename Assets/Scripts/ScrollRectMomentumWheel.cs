using UnityEngine;
using UnityEngine.UI;

public class ScrollRectMomentumWheel : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float momentumFactor = 0.5f; // Adjust for stronger/slower momentum
    public float smoothTime = 0.3f; // How quickly it slows down

    private Vector2 currentVelocity = Vector2.zero;
    private Vector2 previousPosition;
    private float scrollWheelInput;

    void Update()
    {
        if (scrollRect != null && scrollRect.vertical)
        {
            // Get scroll wheel input (positive is up, negative is down)
            scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

            // Apply the scroll wheel input
            if (Mathf.Abs(scrollWheelInput) > 0.01f)
            {
                // Move the content based on the scroll wheel input
                Vector2 deltaPosition = new Vector2(0, scrollWheelInput * 100); // Multiply by sensitivity

                // Apply momentum effect
                currentVelocity = Vector2.Lerp(currentVelocity, deltaPosition, momentumFactor);
            }
            else
            {
                // Apply smooth deceleration after scrolling stops
                currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, smoothTime * Time.deltaTime);
            }

            // Apply the momentum to the ScrollRect content position
            scrollRect.content.anchoredPosition += currentVelocity * Time.deltaTime;
        }
    }
}
