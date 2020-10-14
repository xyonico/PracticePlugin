using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PracticePlugin
{
    public static class SongSeekBeatmapHandler
    {
        private static List<BeatmapObjectCallbackController.BeatmapObjectCallbackData> CallbackList
        {
            get
            {
                if (_beatmapObjectCallbackController == null || _callbackList == null)
                {
                    _beatmapObjectCallbackController = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>()
                        .FirstOrDefault();

                    if (_beatmapObjectCallbackController != null)
                    {
                        _callbackList =
                            _beatmapObjectCallbackController
                                .GetPrivateField<List<BeatmapObjectCallbackController.BeatmapObjectCallbackData>>(
                                    "_beatmapObjectCallbackData");

                        _beatmapData = _beatmapObjectCallbackController
                            .GetPrivateField<BeatmapData>("_beatmapData");
                    }

                    if (_beatmapObjectSpawnController == null)
                    {
                        _beatmapObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>()
                            .FirstOrDefault();

                    }

                    if (_beatmapObjectManager == null)
                    {
                        _beatmapObjectManager = Resources.FindObjectsOfTypeAll<BeatmapObjectExecutionRatingsRecorder>().FirstOrDefault().GetPrivateField<BeatmapObjectManager>("_beatmapObjectManager") as BasicBeatmapObjectManager;

                        if (_beatmapObjectManager != null)
                        {
                            _notePool = _beatmapObjectManager.GetPrivateField<MonoMemoryPoolContainer<GameNoteController>>("_gameNotePoolContainer");
                            _bombNotePool = _beatmapObjectManager.GetPrivateField<MonoMemoryPoolContainer<BombNoteController>>("_bombNotePoolContainer");
                            _obstaclePool =
                                _beatmapObjectManager.GetPrivateField<MonoMemoryPoolContainer<ObstacleController>>("_obstaclePoolContainer");
                        }
                    }
                    if (_noteCutSoundEffectManager == null)
                    {
                        _noteCutSoundEffectManager = Resources.FindObjectsOfTypeAll<NoteCutSoundEffectManager>()
                            .FirstOrDefault();
                    }
                }

                return _callbackList;
            }
        }

        private static List<BeatmapObjectCallbackController.BeatmapObjectCallbackData> _callbackList;
        private static BeatmapObjectCallbackController _beatmapObjectCallbackController;
        private static BeatmapObjectSpawnController _beatmapObjectSpawnController;
        private static BasicBeatmapObjectManager _beatmapObjectManager;
        private static NoteCutSoundEffectManager _noteCutSoundEffectManager;

        private static MonoMemoryPoolContainer<GameNoteController> _notePool;
        private static MonoMemoryPoolContainer<BombNoteController> _bombNotePool;
        private static MonoMemoryPoolContainer<ObstacleController> _obstaclePool;

        private static BeatmapData _beatmapData;

        public static void OnSongTimeChanged(float newSongTime, float aheadTime)
        {
            if (_beatmapObjectCallbackController)
                _beatmapData = _beatmapObjectCallbackController.GetPrivateField<BeatmapData>("_beatmapData");
            foreach (var callbackData in CallbackList)
            {
                for (var i = 0; i < _beatmapData.beatmapLinesData.Count; i++)
                {
                    callbackData.nextObjectIndexInLine[i] = 0;
                    while (callbackData.nextObjectIndexInLine[i] < _beatmapData.beatmapLinesData[i].beatmapObjectsData.Count)
                    {
                        var beatmapObjectData = _beatmapData.beatmapLinesData[i].beatmapObjectsData[callbackData.nextObjectIndexInLine[i]];
                        if (beatmapObjectData.time - aheadTime >= newSongTime)
                        {
                            break;
                        }

                        callbackData.nextObjectIndexInLine[i]++;
                    }
                }
            }

            var newNextEventIndex = 0;

            while (newNextEventIndex < _beatmapData.beatmapEventsData.Count)
            {
                var beatmapEventData = _beatmapData.beatmapEventsData[newNextEventIndex];
                if (beatmapEventData.time >= newSongTime)
                {
                    break;
                }

                newNextEventIndex++;
            }

            _beatmapObjectCallbackController.SetPrivateField("_nextEventIndex", newNextEventIndex);
            _beatmapObjectManager.DissolveAllObjects();
            /*
            var notesA = _notePool.activeItems.ToList();
            foreach (var noteA in notesA)
            {
                //               Console.WriteLine("Despawning, Length: " + notesA.Count);
                _beatmapObjectManager.DissolveAllObjects(noteA);
            }
            
            var notesB = _noteBPool.activeItems.ToList();
            foreach (var noteB in notesB)
            {
                _beatmapObjectManager.Despawn(noteB);
            }

            var bombs = _bombNotePool.activeItems.ToList();
            foreach (var bomb in bombs)
            {
                _beatmapObjectManager.Despawn(bomb);
            }

            var obstacles = _obstaclePool.activeItems.ToList();
            foreach (var obstacle in obstacles)
            {
                _beatmapObjectManager.Despawn(obstacle);
            }
            */

            Plugin.AudioTimeSync.SetPrivateField("_prevAudioSamplePos", -1);
            Plugin.AudioTimeSync.SetPrivateField("_songTime", newSongTime);
            _noteCutSoundEffectManager.SetPrivateField("_prevNoteATime", -1);
            _noteCutSoundEffectManager.SetPrivateField("_prevNoteBTime", -1);
        }
    }
}