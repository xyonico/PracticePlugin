﻿using HMUI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BeatSaberMarkupLanguage;

namespace PracticePlugin
{
    public class LooperUI : MonoBehaviour
    {
        public float StartTime
        {
            get { return Mathf.InverseLerp(0, SongSeeker.SeekBarSize.x, _startCursor.Position); }
        }

        public float EndTime
        {
            get { return Mathf.InverseLerp(0, SongSeeker.SeekBarSize.x, _endCursor.Position); }
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

        private ImageView _lineDuration;

        private LooperCursor _startCursor;
        private LooperCursor _endCursor;

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

            var tex = Texture2D.whiteTexture;
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 100, 1);

            var bg = new GameObject("Background").AddComponent<ImageView>();
            var rectTransform = bg.rectTransform;
            rectTransform.SetParent(transform, false);
            rectTransform.sizeDelta = SongSeeker.SeekBarSize + new Vector2(0, 4);
            rectTransform.anchoredPosition = new Vector2(0, -1);
            bg.sprite = sprite;
            bg.type = Image.Type.Simple;
            bg.color = new Color(0, 0, 0, 0);
            bg.material = Utilities.ImageResources.NoGlowMat;

            _lineDuration = new GameObject("Line Duration").AddComponent<ImageView>();
            rectTransform = _lineDuration.rectTransform;
            rectTransform.SetParent(transform, false);
            rectTransform.anchorMin = Vector2.up * 0.5f;
            rectTransform.anchorMax = Vector2.up * 0.5f;
            rectTransform.sizeDelta = Vector2.zero;
            _lineDuration.sprite = sprite;
            _lineDuration.type = Image.Type.Simple;
            _lineDuration.color = LineDurationColor;
            _lineDuration.material = Utilities.ImageResources.NoGlowMat;

            var startCursorImage = new GameObject("Start Cursor").AddComponent<ImageView>();
            rectTransform = startCursorImage.rectTransform;
            rectTransform.SetParent(transform, false);
            rectTransform.anchorMin = Vector2.up * 0.5f;
            rectTransform.anchorMax = Vector2.up * 0.5f;
            rectTransform.sizeDelta = CursorSize;
            rectTransform.localEulerAngles = new Vector3(0, 0, 45);
            startCursorImage.sprite = sprite;
            startCursorImage.type = Image.Type.Simple;
            startCursorImage.color = StartColor;
            startCursorImage.material = Utilities.ImageResources.NoGlowMat;

            _startCursor = startCursorImage.gameObject.AddComponent<LooperCursor>();
            _startCursor.BeginDragEvent += CursorOnBeginDragEvent;
            _startCursor.EndDragEvent += CursorOnEndDragEvent;
            _startCursor.Position = Mathf.Lerp(0, SongSeeker.SeekBarSize.x, _prevStartTime);

            var endCursorImage = new GameObject("End Cursor").AddComponent<ImageView>();
            rectTransform = endCursorImage.rectTransform;
            rectTransform.SetParent(transform, false);
            rectTransform.anchorMin = Vector2.up * 0.5f;
            rectTransform.anchorMax = Vector2.up * 0.5f;
            rectTransform.sizeDelta = CursorSize;
            rectTransform.localEulerAngles = new Vector3(0, 0, 45);
            endCursorImage.sprite = sprite;
            endCursorImage.type = Image.Type.Simple;
            endCursorImage.color = EndColor;
            endCursorImage.material = Utilities.ImageResources.NoGlowMat;

            _endCursor = endCursorImage.gameObject.AddComponent<LooperCursor>();
            _endCursor.BeginDragEvent += CursorOnBeginDragEvent;
            _endCursor.EndDragEvent += CursorOnEndDragEvent;
            _endCursor.Position = Mathf.Lerp(0, SongSeeker.SeekBarSize.x, _prevEndTime);

            _startCursor.Init(LooperCursor.Type.Start);
            _endCursor.Init(LooperCursor.Type.End);

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
                bool hovering = (eventData.hovered.Count > 0);
                if (!hovering) { return; }
                var newPos = Mathf.Lerp(0, SongSeeker.SeekBarSize.x, Mathf.InverseLerp(-1, 1, Mathf.Clamp(eventData.position.x, -1f, 1f)));

                var seekerPos = _songSeeker.PlaybackPosition;
                if (Mathf.Abs(newPos - seekerPos) <= StickToSeekerCursorDistance)
                {
                    newPos = seekerPos;
                }

                if (_draggingCursor.CursorType == LooperCursor.Type.Start)
                {
                    _draggingCursor.Position = Mathf.Clamp(newPos, 0, _endCursor.Position - MinCursorDistance);
                }
                else
                {
                    _draggingCursor.Position = Mathf.Clamp(newPos, _startCursor.Position + MinCursorDistance,
                        SongSeeker.SeekBarSize.x);
                }
            }

            _lineDuration.rectTransform.sizeDelta = new Vector2(_endCursor.Position - _startCursor.Position, LineDurationWidth);
            _lineDuration.rectTransform.anchoredPosition = new Vector2((_startCursor.Position + _endCursor.Position) / 2, 0);
        }

        private void OnDestroy()
        {
            _prevStartTime = StartTime;
            _prevEndTime = EndTime;
        }
    }
}