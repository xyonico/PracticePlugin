using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PracticePlugin
{
	public class LooperCursor : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public Type CursorType { get; private set; }
		public PointerEventData EventData { get; private set; }
		public float Position { get; set; }

		public event Action<LooperCursor, PointerEventData> BeginDragEvent;
		public event Action<LooperCursor, PointerEventData> EndDragEvent;

		private RectTransform _rectTransform;

		public void Init(Type cursorType)
		{
			CursorType = cursorType;
			_rectTransform = transform as RectTransform;
		}

		private void LateUpdate()
		{
			_rectTransform.anchoredPosition = new Vector2(Position, 0);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
			EventData = eventData;
			if (BeginDragEvent != null)
			{
				BeginDragEvent(this, eventData);
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
			EventData = eventData;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
			EventData = eventData;
			if (EndDragEvent != null)
			{
				EndDragEvent(this, eventData);
			}
		}

		public enum Type
		{
			Start,
			End
		}
	}
}