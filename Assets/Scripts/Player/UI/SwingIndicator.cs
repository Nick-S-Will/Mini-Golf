using UnityEngine;

namespace MiniGolf.Player.UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SwingIndicator : MonoBehaviour
    {
        [SerializeField] private LayerMask groundCheckMask;
        [SerializeField] [Min(0.0001f)] private float groundOffset = 0.01f;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            PlayerHandler.OnSetPlayer.AddListener(ChangePlayer);
        }

        private void ChangePlayer(SwingController oldPlayer, SwingController newPlayer)
        {
            if (oldPlayer)
            {
                oldPlayer.OnSwing.RemoveListener(ShowCannotSwing);
                oldPlayer.OnStartMoving.RemoveListener(ShowCannotSwing);
                oldPlayer.OnStopMoving.RemoveListener(ShowCanSwing);
            }
            if (newPlayer)
            {
                newPlayer.OnSwing.AddListener(ShowCannotSwing);
                newPlayer.OnStartMoving.AddListener(ShowCannotSwing);
                newPlayer.OnStopMoving.AddListener(ShowCanSwing);

                if (!newPlayer.IsMoving) ShowCanSwing();
                else ShowCannotSwing();
            }
        }

        private void ShowCanSwing() => SetVisible(true);
        private void ShowCannotSwing() => SetVisible(false);

        private void SetVisible(bool visible)
        {
            if (!visible)
            {
                spriteRenderer.enabled = false;
                return;
            }

            var playerPosition = PlayerHandler.Player.transform.position;
            var ballRadius = PlayerHandler.Player.SphereCollider.radius;
            Physics.SphereCast(playerPosition, 0.5f * ballRadius, Vector3.down, out RaycastHit hitInfo, ballRadius, groundCheckMask);

            spriteRenderer.enabled = hitInfo.collider;
            if (hitInfo.collider == null) return;

            transform.position = hitInfo.point + groundOffset * hitInfo.normal;
            transform.forward = hitInfo.normal;
        }

        private void OnDestroy()
        {
            PlayerHandler.OnSetPlayer.RemoveListener(ChangePlayer);

            if (PlayerHandler.Player == null) return;
            
            PlayerHandler.Player.OnStopMoving.RemoveListener(ShowCanSwing);
            PlayerHandler.Player.OnSwing.RemoveListener(ShowCannotSwing);
        }
    }
}