using UnityEngine;
using UnityEngine.EventSystems;

namespace GameDevTV.RTS.UI
{
    [RequireComponent(typeof(EventTrigger))]
    public class MinimapClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
                                        IPointerExitHandler, IPointerMoveHandler
    {
        [SerializeField] private Camera minimapCamera;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private LayerMask floorMask;

        private bool isMouseDownOnMinimap;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (minimapCamera == null || cameraTarget == null)
            {
                Debug.LogError("MinimapClickHandler is missing some references! Ensure minimapCamera and cameraTarget are assigned!");
                enabled = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                isMouseDownOnMinimap = true;
                MoveVirtualCameraTarget(eventData.position);
            }
        }

        public void OnPointerMove(PointerEventData eventData) => MoveVirtualCameraTarget(eventData.position);

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                isMouseDownOnMinimap = false;
            }
        }

        public void OnPointerExit(PointerEventData eventData) => isMouseDownOnMinimap = false;

        private void MoveVirtualCameraTarget(Vector2 mousePosition)
        {
            if (!isMouseDownOnMinimap) return;

            float widthMultiplier = minimapCamera.scaledPixelWidth / rectTransform.rect.width;
            float heightMultiplier = minimapCamera.scaledPixelHeight / rectTransform.rect.height;

            Vector2 convertedMousePosition = new(
                mousePosition.x * widthMultiplier,
                mousePosition.y * heightMultiplier
            );

            Ray cameraRay = minimapCamera.ScreenPointToRay(convertedMousePosition);
            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, floorMask))
            {
                cameraTarget.position = hit.point;
            }
        }
    }
}