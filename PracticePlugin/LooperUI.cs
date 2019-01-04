using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PracticePlugin
{
    public class LooperUI : MonoBehaviour
    {
        public float StartTime
        {
            get { return Mathf.InverseLerp(0, SongSeeker.SeekBarSize.x, startCursor.Position); }
        }

        public float EndTime
        {
            get { return Mathf.InverseLerp(0, SongSeeker.SeekBarSize.x, endCursor.Position); }
        }

        public event Action OnDragEndEvent;

        private static readonly Vector2 CursorSize = new Vector2(3, 3);

        private static readonly Color StartColor = new Color(0.15f, 0.35f, 0.8f, 0.75f);
        private static readonly Color EndColor = new Color(0.85f, 0.12f, 0.25f, 0.75f);
        private static readonly Color LineDurationColor = new Color(1, 1, 1, 0.4f);

        private const float LineDurationWidth = 1f;
        private const float MinCursorDistance = 4f;
        private const float StickToSeekerCursorDistance = 2f;

        private static float _prevStartTime;
        private static float _prevEndTime = 1f;

        private SongSeeker _songSeeker;

        private Image _lineDuration;

        public LooperCursor startCursor { get; private set; }
        public LooperCursor endCursor { get; private set; }

        private LooperCursor _draggingCursor;

        private Camera _mainCamera;

        public void Init(SongSeeker songSeeker)
        {
            _songSeeker = songSeeker;

            if (Plugin.PlayingNewSong)
            {
                _prevStartTime = 0;
                _prevEndTime = 1;
            }

            _lineDuration = new GameObject("Line Duration").AddComponent<Image>();
            var rectTransform = _lineDuration.rectTransform;
            rectTransform.SetParent(transform, false);
            rectTransform.anchorMin = Vector2.up * 0.5f;
            rectTransform.anchorMax = Vector2.up * 0.5f;
            rectTransform.sizeDelta = Vector2.zero;
            _lineDuration.color = LineDurationColor;

            var startCursorImage = new GameObject("Start Cursor").AddComponent<Image>();
            rectTransform = startCursorImage.rectTransform;
            rectTransform.SetParent(transform, false);
            rectTransform.anchorMin = Vector2.up * 0.5f;
            rectTransform.anchorMax = Vector2.up * 0.5f;
            rectTransform.sizeDelta = CursorSize;
            rectTransform.localEulerAngles = new Vector3(0, 0, 45);
            startCursorImage.color = StartColor;

            startCursor = startCursorImage.gameObject.AddComponent<LooperCursor>();
            startCursor.BeginDragEvent += CursorOnBeginDragEvent;
            startCursor.EndDragEvent += CursorOnEndDragEvent;
            startCursor.Position = Mathf.Lerp(0, SongSeeker.SeekBarSize.x, _prevStartTime);

            var endCursorImage = new GameObject("End Cursor").AddComponent<Image>();
            rectTransform = endCursorImage.rectTransform;
            rectTransform.SetParent(transform, false);
            rectTransform.anchorMin = Vector2.up * 0.5f;
            rectTransform.anchorMax = Vector2.up * 0.5f;
            rectTransform.sizeDelta = CursorSize;
            rectTransform.localEulerAngles = new Vector3(0, 0, 45);
            endCursorImage.color = EndColor;

            endCursor = endCursorImage.gameObject.AddComponent<LooperCursor>();
            endCursor.BeginDragEvent += CursorOnBeginDragEvent;
            endCursor.EndDragEvent += CursorOnEndDragEvent;
            endCursor.Position = Mathf.Lerp(0, SongSeeker.SeekBarSize.x, _prevEndTime);

            startCursor.Init(LooperCursor.Type.Start);
            endCursor.Init(LooperCursor.Type.End);

            _mainCamera = Camera.main;
        }

        private void CursorOnBeginDragEvent(LooperCursor cursor, PointerEventData eventData)
        {
            _draggingCursor = cursor;
        }

        private void CursorOnEndDragEvent(LooperCursor cursor, PointerEventData eventData)
        {
            _draggingCursor = null;

            if (OnDragEndEvent != null)
            {
                OnDragEndEvent();
            }
        }

        private void Update()
        {
            if (_draggingCursor != null)
            {
                var eventData = _draggingCursor.EventData;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position,
                    _mainCamera, out var pos);
                var newPos = pos.x + SongSeeker.HalfSeekBarSize;

                var seekerPos = Mathf.Lerp(0, SongSeeker.SeekBarSize.x, _songSeeker.PlaybackPosition);
                if (Mathf.Abs(newPos - seekerPos) <= StickToSeekerCursorDistance)
                {
                    newPos = seekerPos;
                }

                if (_draggingCursor.CursorType == LooperCursor.Type.Start)
                {
                    _draggingCursor.Position = Mathf.Clamp(newPos, 0, endCursor.Position - MinCursorDistance);
                }
                else
                {
                    _draggingCursor.Position = Mathf.Clamp(newPos, startCursor.Position + MinCursorDistance,
                        SongSeeker.SeekBarSize.x);
                }
            }

            _lineDuration.rectTransform.sizeDelta = new Vector2(endCursor.Position - startCursor.Position, LineDurationWidth);
            _lineDuration.rectTransform.anchoredPosition = new Vector2((startCursor.Position + endCursor.Position) / 2, 0);
        }

        private void OnDestroy()
        {
            _prevStartTime = StartTime;
            _prevEndTime = EndTime;
        }
    }
}