#if false
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CGDP2.Utilities;

namespace CGDP2.UtilityComponents
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class AnimatedSprite : MonoBehaviour
	{
		private SpriteRenderer m_spriteRenderer;
		public SpriteRenderer spriteRenderer => this.CacheGetComponent(ref m_spriteRenderer);

		[SerializeField]
		private float m_period = 0.5f;
		public float period => m_period;

		[SerializeField]
		private Sprite[] m_frames = new Sprite[0];
		public IReadOnlyList<Sprite> frames => m_frames;

		private float m_time;
		private float time
		{
			get => m_time;
			set
			{
				m_time = Mathf.Clamp01(value);
				if (m_time >= 1.0f)
					m_time = 0.0f;

				if (frames.Count > 0)
				{
					spriteRenderer.sprite = frames[Mathf.FloorToInt(m_time * frames.Count)];
				}
			}
		}

		private Tween tween;

		private void Awake()
		{
			time = 0.0f;
			tween = DOTween.To(
				() => time,
				t => time = t,
				endValue: 1.0f,
				duration: period
				)
				.SetEase(Ease.Linear)
				.SetLoops(-1);
		}

		private void OnDestroy()
		{
			tween.Kill();
		}
	}
}
#endif