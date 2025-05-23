using Unity.XR.CoreUtils;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using System.Collections;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// A version of continuous movement that automatically controls the frame of reference that
    /// determines the forward direction of movement based on user preference for each hand.
    /// For example, can configure to use head relative movement for the left hand and controller relative movement for the right hand.
    /// </summary>
    public class DynamicMoveProvider : ContinuousMoveProvider
    {
        /// <summary>
        /// Defines which transform the XR Origin's movement direction is relative to.
        /// </summary>
        /// <seealso cref="leftHandMovementDirection"/>
        /// <seealso cref="rightHandMovementDirection"/>
        public enum MovementDirection
        {
            /// <summary>
            /// Use the forward direction of the head (camera) as the forward direction of the XR Origin's movement.
            /// </summary>
            HeadRelative,

            /// <summary>
            /// Use the forward direction of the hand (controller) as the forward direction of the XR Origin's movement.
            /// </summary>
            HandRelative,
        }

        [Space, Header("Movement Direction")]
        [SerializeField]
        [Tooltip("Directs the XR Origin's movement when using the head-relative mode. If not set, will automatically find and use the XR Origin Camera.")]
        Transform m_HeadTransform;

        /// <summary>
        /// Directs the XR Origin's movement when using the head-relative mode. If not set, will automatically find and use the XR Origin Camera.
        /// </summary>
        public Transform headTransform
        {
            get => m_HeadTransform;
            set => m_HeadTransform = value;
        }

        [SerializeField]
        [Tooltip("Directs the XR Origin's movement when using the hand-relative mode with the left hand.")]
        Transform m_LeftControllerTransform;

        /// <summary>
        /// Directs the XR Origin's movement when using the hand-relative mode with the left hand.
        /// </summary>
        public Transform leftControllerTransform
        {
            get => m_LeftControllerTransform;
            set => m_LeftControllerTransform = value;
        }

        [SerializeField]
        [Tooltip("Directs the XR Origin's movement when using the hand-relative mode with the right hand.")]
        Transform m_RightControllerTransform;

        public Transform rightControllerTransform
        {
            get => m_RightControllerTransform;
            set => m_RightControllerTransform = value;
        }

        [SerializeField]
        [Tooltip("Whether to use the specified head transform or left controller transform to direct the XR Origin's movement for the left hand.")]
        MovementDirection m_LeftHandMovementDirection;

        /// <summary>
        /// Whether to use the specified head transform or controller transform to direct the XR Origin's movement for the left hand.
        /// </summary>
        /// <seealso cref="MovementDirection"/>
        public MovementDirection leftHandMovementDirection
        {
            get => m_LeftHandMovementDirection;
            set => m_LeftHandMovementDirection = value;
        }

        [SerializeField]
        [Tooltip("Whether to use the specified head transform or right controller transform to direct the XR Origin's movement for the right hand.")]
        MovementDirection m_RightHandMovementDirection;

        /// <summary>
        /// Whether to use the specified head transform or controller transform to direct the XR Origin's movement for the right hand.
        /// </summary>
        /// <seealso cref="MovementDirection"/>
        public MovementDirection rightHandMovementDirection
        {
            get => m_RightHandMovementDirection;
            set => m_RightHandMovementDirection = value;
        }

        Transform m_CombinedTransform;
        Pose m_LeftMovementPose = Pose.identity;
        Pose m_RightMovementPose = Pose.identity;


        [Header("Footstep Audio")]
        private AudioSource footstepAudioSource;
        [SerializeField] private AudioClip footstepSound;
        [SerializeField] private float fadeDuration = 0.5f; // 淡入淡出时间（秒）

        private bool m_IsMoving;
        private Coroutine m_FadeCoroutine;


        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();

            m_CombinedTransform = new GameObject("[Dynamic Move Provider] Combined Forward Source").transform;
            m_CombinedTransform.SetParent(transform, false);
            m_CombinedTransform.localPosition = Vector3.zero;
            m_CombinedTransform.localRotation = Quaternion.identity;

            forwardSource = m_CombinedTransform;
            if (footstepAudioSource == null)
            {
                footstepAudioSource = GetComponent<AudioSource>();
            }
        }

        /// <inheritdoc />
        protected override Vector3 ComputeDesiredMove(Vector2 input)
        {
            // Don't need to do anything if the total input is zero.
            // This is the same check as the base method.
            // 检测是否正在移动
            bool wasMoving = m_IsMoving;
            m_IsMoving = input != Vector2.zero;
            // 状态变化时触发淡入/淡出
            if (m_IsMoving != wasMoving)
            {
                HandleFootstepFade(m_IsMoving);
            }
            if (input == Vector2.zero)
                return Vector3.zero;

            // Initialize the Head Transform if necessary, getting the Camera from XR Origin
            if (m_HeadTransform == null)
            {
                var xrOrigin = mediator.xrOrigin;
                if (xrOrigin != null)
                {
                    var xrCamera = xrOrigin.Camera;
                    if (xrCamera != null)
                        m_HeadTransform = xrCamera.transform;
                }
            }

            // Get the forward source for the left hand input
            switch (m_LeftHandMovementDirection)
            {
                case MovementDirection.HeadRelative:
                    if (m_HeadTransform != null)
                        m_LeftMovementPose = m_HeadTransform.GetWorldPose();

                    break;

                case MovementDirection.HandRelative:
                    if (m_LeftControllerTransform != null)
                        m_LeftMovementPose = m_LeftControllerTransform.GetWorldPose();

                    break;

                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(MovementDirection)}={m_LeftHandMovementDirection}");
                    break;
            }

            // Get the forward source for the right hand input
            switch (m_RightHandMovementDirection)
            {
                case MovementDirection.HeadRelative:
                    if (m_HeadTransform != null)
                        m_RightMovementPose = m_HeadTransform.GetWorldPose();

                    break;

                case MovementDirection.HandRelative:
                    if (m_RightControllerTransform != null)
                        m_RightMovementPose = m_RightControllerTransform.GetWorldPose();

                    break;

                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(MovementDirection)}={m_RightHandMovementDirection}");
                    break;
            }

            // Combine the two poses into the forward source based on the magnitude of input
            var leftHandValue = leftHandMoveInput.ReadValue();
            var rightHandValue = rightHandMoveInput.ReadValue();

            var totalSqrMagnitude = leftHandValue.sqrMagnitude + rightHandValue.sqrMagnitude;
            var leftHandBlend = 0.5f;
            if (totalSqrMagnitude > Mathf.Epsilon)
                leftHandBlend = leftHandValue.sqrMagnitude / totalSqrMagnitude;

            var combinedPosition = Vector3.Lerp(m_RightMovementPose.position, m_LeftMovementPose.position, leftHandBlend);
            var combinedRotation = Quaternion.Slerp(m_RightMovementPose.rotation, m_LeftMovementPose.rotation, leftHandBlend);
            m_CombinedTransform.SetPositionAndRotation(combinedPosition, combinedRotation);
            return base.ComputeDesiredMove(input);
        }
        private void HandleFootstepFade(bool shouldPlay)
        {
            // 如果已有淡入淡出协程在运行，先停止
            if (m_FadeCoroutine != null)
            {
                StopCoroutine(m_FadeCoroutine);
            }

            m_FadeCoroutine = StartCoroutine(FadeFootsteps(shouldPlay));
        }

        private IEnumerator FadeFootsteps(bool fadeIn)
        {
            float startVolume = footstepAudioSource.volume;
            float targetVolume = fadeIn ? 1f : 0f;
            float elapsedTime = 0f;

            // 如果是要播放且当前未播放，先启动音频
            if (fadeIn && !footstepAudioSource.isPlaying)
            {
                footstepAudioSource.clip = footstepSound;
                footstepAudioSource.loop = true; // 启用循环
                footstepAudioSource.volume = 0f; // 初始音量为0
                footstepAudioSource.Play();
            }

            // 渐变音量
            while (elapsedTime < fadeDuration)
            {
                footstepAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 确保最终音量准确
            footstepAudioSource.volume = targetVolume;

            // 如果是淡出且音量降为0，停止音频
            if (!fadeIn && footstepAudioSource.volume <= 0f)
            {
                footstepAudioSource.Stop();
            }
        }
    }
}
