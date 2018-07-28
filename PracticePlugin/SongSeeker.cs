using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PracticePlugin
{
	public class SongSeeker : MonoBehaviour, IDragHandler, IPointerDownHandler
	{
		private float _playbackPosition;
	
		private Image _seekBackg;
		private Image _seekBar;
		private Image _seekCursor;

		private Camera _mainCamera;
    
		private static readonly Vector2 ParentSize = new Vector2(100, 4);
		private static readonly Vector2 SeekBarSize = new Vector2(100, 2);
		private static readonly Color BackgroundColor = new Color(0, 0, 0, 0.25f);
		private static readonly Color ForegroundColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
	
		private static readonly Vector2 SeekCursorSize = new Vector2(4, 4);
		private static readonly Color SeekCursorColor = new Color(1, 1, 1, 0.5f);

		private static readonly float HalfCursorSize = SeekCursorSize.x / 2;
		private static readonly float HalfSeekBarSize = SeekBarSize.x / 2;

		private void OnEnable()
		{
			var rectTransform = transform as RectTransform;
			rectTransform.anchorMin = Vector2.right * 0.5f;
			rectTransform.anchorMax = Vector2.right * 0.5f;
			rectTransform.sizeDelta = ParentSize;
			rectTransform.anchoredPosition = new Vector2(0, 15);
		
			_seekBackg = new GameObject("Background").AddComponent<Image>();
			rectTransform = _seekBackg.rectTransform;
			rectTransform.SetParent(transform, false);
			rectTransform.sizeDelta = SeekBarSize;
			_seekBackg.color = BackgroundColor;

			_seekBar = new GameObject("Seek Bar").AddComponent<Image>();
			rectTransform = _seekBar.rectTransform;
			rectTransform.SetParent(transform, false);
			rectTransform.sizeDelta = SeekBarSize;
			var tex = Texture2D.whiteTexture;
			var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 100, 1);
			_seekBar.sprite = sprite;
			_seekBar.type = Image.Type.Filled;
			_seekBar.fillMethod = Image.FillMethod.Horizontal;
			_seekBar.color = ForegroundColor;

			_seekCursor = new GameObject("Seek Cursor").AddComponent<Image>();
			rectTransform = _seekCursor.rectTransform;
			rectTransform.SetParent(_seekBar.transform, false);
			rectTransform.anchorMin = Vector2.up * 0.5f;
			rectTransform.anchorMax = Vector2.up * 0.5f;
			rectTransform.sizeDelta = SeekCursorSize;
			_seekCursor.color = SeekCursorColor;

			_mainCamera = Camera.main;
		}

		private void LateUpdate()
		{
			_seekBar.fillAmount = _playbackPosition;
			_seekCursor.rectTransform.anchoredPosition =
				new Vector2(Mathf.Lerp(HalfCursorSize, SeekBarSize.x - HalfCursorSize, _playbackPosition), 0);
		}

		public void OnDrag(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, _mainCamera, out var pos);
			var posX = pos.x + HalfSeekBarSize;
			_playbackPosition = Mathf.InverseLerp(HalfCursorSize, SeekBarSize.x - HalfCursorSize, posX);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.pressPosition, _mainCamera, out var pos);
			var posX = pos.x + HalfSeekBarSize;
			_playbackPosition = Mathf.InverseLerp(HalfCursorSize, SeekBarSize.x - HalfCursorSize, posX);
		}
	}
}
