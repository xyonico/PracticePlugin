using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;
//using CustomUI.GameplaySettings;
using IPA;
using BS_Utils.Gameplay;
using BeatSaberMarkupLanguage;
namespace PracticePlugin
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {

        public const float SpeedMaxSize = 5.05f;
        public const float SpeedStepSize = 0.05f;

        public const int NjsMaxSize = 100;
        public const int NjstepSize = 1;

        public const string MenuSceneName = "MenuViewControllers";
        public const string GameSceneName = "GameCore";
        public const string ContextSceneName = "GameplayCore";

        public string failTime { get; private set; }
        internal bool showFailTextNext { get; set; }
 //       public static GameObject SpeedSettingsObject { get; private set; }
 //       public static GameObject NjsSettingsObject { get; private set; }
 //       public static GameObject SpawnOffsetSettingsObject { get; private set; }
        internal static bool startWithFullEnergy = false;
        internal static bool disablePitchCorrection = false;
        internal static bool adjustNJSWithSpeed = false;
        public static float TimeScale
        {
            get { return _timeScale; }
            private set
            {
                _timeScale = value;
                if (!AudioTimeSync) return;
                AudioTimeSyncController.InitData initData = AudioTimeSync.GetPrivateField<AudioTimeSyncController.InitData>("_initData");
                AudioTimeSyncController.InitData newInitData = new AudioTimeSyncController.InitData(initData.audioClip,
                    AudioTimeSync.songTime, initData.songTimeOffset, _timeScale);
                AudioTimeSync.SetPrivateField("_initData", newInitData);
                //Chipmunk Removal as per base game
                if(!disablePitchCorrection)
                {
                    if (_timeScale == 1f)
                        _mixer.musicPitch = 1;
                    else
                        _mixer.musicPitch = 1f / _timeScale;
                }
                else
                {
                    _mixer.musicPitch = 1f;
                }

                AudioTimeSync.StartSong();


                return;
                //       AudioTimeSync.SetPrivateField("_timeScale", value);
                //        AudioTimeSync.Init(_songAudio.clip, _songAudio.time, AudioTimeSync.GetPrivateField<float>("_songTimeOffset"), value);
                if (_timeScale == 1f)
                    _mixer.musicPitch = 1;
                else
                    _mixer.musicPitch = 1f / _timeScale;
                if (!IsEqualToOne(_timeScale))
                {

                    if (AudioTimeSync != null)
                    {
                        //           AudioTimeSync.forcedNoAudioSync = true;
                    }
                }
                else
                {
                    if (AudioTimeSync != null)
                    {
                        //           AudioTimeSync.forcedNoAudioSync = false;
                    }
                }
                if (AudioTimeSync != null)
                {
               //     AudioTimeSync.SetPrivateField("_timeScale", _timeScale); // = _timeScale;
                                                                             //     AudioTimeSync.Init(_songAudio.clip, _songAudio.time, 
                                                                             //           AudioTimeSync.GetPrivateField<float>("_songTimeOffset") - AudioTimeSync.GetPrivateField<FloatSO>("_audioLatency").value, _timeScale);
                    Console.WriteLine("Called TimeScale");

                    if (_songAudio != null)
                    {
                        _songAudio.pitch = _timeScale;
                    }
           //         AudioTimeSync.forcedNoAudioSync = true;
                    //         float num = AudioTimeSync.GetPrivateField<float>("_startSongTime") + AudioTimeSync.GetPrivateField<float>("_songTimeOffset");
                    //     AudioTimeSync.SetPrivateField("_audioStartTimeOffsetSinceStart", (Time.timeSinceLevelLoad * _timeScale) - num);
                    //   AudioTimeSync.SetPrivateField("_fixingAudioSyncError", false);
                    //   AudioTimeSync.SetPrivateField("_prevAudioSamplePos", _songAudio.timeSamples);
                    //   AudioTimeSync.SetPrivateField("_playbackLoopIndex", 0);
                    //          AudioTimeSync.SetPrivateField("_dspTimeOffset", AudioSettings.dspTime - (double)num);
                    //    AudioTimeSync.SetPrivateField("_timeScale", _timeScale); // = _timeScale;
                }


            }
        }

        private static float _timeScale = 1;

        public static bool PracticeMode { get; private set; }

        public static bool PlayingNewSong { get; private set; }

        private static bool _init;
        public static BS_Utils.Gameplay.LevelData _levelData { get; private set; }
        public static BS_Utils.Utilities.Config Config = new BS_Utils.Utilities.Config("PracticePlugin");
        public static BeatmapObjectSpawnController _spawnController { get; private set; }
        public static AudioTimeSyncController AudioTimeSync { get; private set; }
        private static AudioManagerSO _mixer;
        private static AudioSource _songAudio;
        private static string _lastLevelId;
        public static UIElementsCreator _uiElementsCreator;
        private static ResultsViewController resultsViewController;
        private static bool _resetNoFail;
        private static bool showTimeFailed = true;
        private static TextMeshProUGUI failText;
        [OnStart]
        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;
            //  NoFailGameEnergy.limitLevelFail = Config.GetBool("PracticePlugin", "limitLevelFailDisplay", false, true);
            startWithFullEnergy = Config.GetBool("PracticePlugin", "startWithFullEnergy", false, true);
            showTimeFailed = Config.GetBool("PracticePlugin", "Show Time Failed", true, true);
            disablePitchCorrection = Config.GetBool("PracticePlugin", "Disable Pitch Correction", false, true);
            adjustNJSWithSpeed = Config.GetBool("PracticePlugin", "Adjust NJS With Speed", false, true);
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "MenuCore")
            {
                /*
                var practicePluginSubmenu = GameplaySettingsUI.CreateSubmenuOption(GameplaySettingsPanels.PlayerSettingsRight, "PracticePlugin", "MainMenu", "PracticePlugin", "Practice Plugin Settings");
                var fullEnergy = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.PlayerSettingsRight, "Start With Full Energy", "PracticePlugin", "Start With full energy in Practice Mode.");
                fullEnergy.GetValue = Config.GetBool("PracticePlugin", "startWithFullEnergy", false, true);
                fullEnergy.OnToggle += (value) =>
                {
                    startWithFullEnergy = value;
                    Config.SetBool("PracticePlugin", "startWithFullEnergy", value);

                };

                var timeFailedOption = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.PlayerSettingsRight, "Show Time Failed", "PracticePlugin", "Show the time the level was failed on the results screen.");
                timeFailedOption.GetValue = Config.GetBool("PracticePlugin", "Show Time Failed", true, true);
                timeFailedOption.OnToggle += (value) =>
            {
                showTimeFailed = value;
                Config.SetBool("PracticePlugin", "Show Time Failed", value);
            };
            */
            }



        }

        public void OnApplicationQuit()
        {

        }

        public void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            Object.Destroy(Resources.FindObjectsOfTypeAll<UIElementsCreator>().FirstOrDefault()?.gameObject);
            if (newScene.name == MenuSceneName)
            {
                resultsViewController =
                Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();
                if (resultsViewController != null)
                {

                    resultsViewController.didActivateEvent -= ResultsViewController_didActivateEvent;
                    resultsViewController.didActivateEvent += ResultsViewController_didActivateEvent;
                }

                /*
                if (SpeedSettingsObject != null) return;

                var volumeSettings = Resources.FindObjectsOfTypeAll<NamedIntListSettingsController>().FirstOrDefault();

                if (volumeSettings == null) return;

                volumeSettings.gameObject.SetActive(false);
                SpeedSettingsObject = Object.Instantiate(volumeSettings.gameObject);
                SpeedSettingsObject.SetActive(false);
                volumeSettings.gameObject.SetActive(true);

                if (SpeedSettingsObject == null) return;

                var volume = SpeedSettingsObject.GetComponent<NamedIntListSettingsController>();
                ReflectionUtil.CopyComponent(volume, typeof(IncDecSettingsController),
                    typeof(SpeedSettingsController), SpeedSettingsObject);
                Object.DestroyImmediate(volume);

                Polyglot.LocalizedTextMeshProUGUI localizer = SpeedSettingsObject.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
                if (localizer != null)
                    GameObject.Destroy(localizer);

                SpeedSettingsObject.GetComponentInChildren<TMP_Text>().text = "";
                Object.DontDestroyOnLoad(SpeedSettingsObject);


                //NJS Object
                if (NjsSettingsObject != null) return;

                var volumeSettings2 = Resources.FindObjectsOfTypeAll<NamedIntListSettingsController>().FirstOrDefault();

                if (volumeSettings2 == null) return;

                volumeSettings2.gameObject.SetActive(false);
                NjsSettingsObject = Object.Instantiate(volumeSettings2.gameObject);
                NjsSettingsObject.SetActive(false);
                volumeSettings2.gameObject.SetActive(true);

                if (NjsSettingsObject == null) return;

                var volume2 = NjsSettingsObject.GetComponent<NamedIntListSettingsController>();
                ReflectionUtil.CopyComponent(volume2, typeof(IncDecSettingsController),
                    typeof(NjsSettingsController), NjsSettingsObject);
                Object.DestroyImmediate(volume2);

                localizer = NjsSettingsObject.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
                if (localizer != null)
                    GameObject.Destroy(localizer);

                NjsSettingsObject.GetComponentInChildren<TMP_Text>().text = "";
                Object.DontDestroyOnLoad(NjsSettingsObject);


                //Spawn Offset Object
                if (SpawnOffsetSettingsObject != null) return;

                var volumeSettings3 = Resources.FindObjectsOfTypeAll<NamedIntListSettingsController>().FirstOrDefault();

                if (volumeSettings3 == null) return;

                volumeSettings3.gameObject.SetActive(false);
                SpawnOffsetSettingsObject = Object.Instantiate(volumeSettings3.gameObject);
                SpawnOffsetSettingsObject.SetActive(false);
                volumeSettings3.gameObject.SetActive(true);

                if (SpawnOffsetSettingsObject == null) return;

                var volume3 = SpawnOffsetSettingsObject.GetComponent<NamedIntListSettingsController>();
                ReflectionUtil.CopyComponent(volume3, typeof(IncDecSettingsController),
                    typeof(SpawnOffsetController), SpawnOffsetSettingsObject);
                Object.DestroyImmediate(volume3);

                localizer = SpawnOffsetSettingsObject.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
                if (localizer != null)
                    GameObject.Destroy(localizer);

                SpawnOffsetSettingsObject.GetComponentInChildren<TMP_Text>().text = "";
                Object.DontDestroyOnLoad(SpawnOffsetSettingsObject);
                */
            }
            else if (newScene.name == GameSceneName)
            {
                /*
                CustomEffectPoolsInstaller customEffectPoolsInstaller = null;
                var effectPoolsInstaller = Resources.FindObjectsOfTypeAll<EffectPoolsInstaller>().FirstOrDefault();
                if (effectPoolsInstaller != null)
                {
                    customEffectPoolsInstaller = (CustomEffectPoolsInstaller)ReflectionUtil.CopyComponent(effectPoolsInstaller,
                        typeof(EffectPoolsInstaller), typeof(CustomEffectPoolsInstaller), effectPoolsInstaller.gameObject);
                }

                SceneContext sceneContext = null;
                SceneDecoratorContext sceneDecoratorContext = null;
                try
                {
                    Console.WriteLine("Custom effect Pool Installer Made");
                    foreach (var gameObject in newScene.GetRootGameObjects())
                    {
                        if (sceneContext == null)
                        {
                            sceneContext = gameObject.GetComponentInChildren<SceneContext>(true);
                        }
                    }

                    foreach (var gameObject in SceneManager.GetSceneByName(ContextSceneName).GetRootGameObjects())
                    {
                        if (sceneDecoratorContext == null)
                        {
                            sceneDecoratorContext = gameObject.GetComponentInChildren<SceneDecoratorContext>(true);
                        }
                    }


                    if (sceneContext != null && sceneDecoratorContext != null)
                    {
                        
                        var prop = typeof(Context).GetField("_monoInstallers", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        var installersList = (List<MonoInstaller>)prop.GetValue(sceneDecoratorContext);
                        installersList.Remove(effectPoolsInstaller);
                        Object.DestroyImmediate(effectPoolsInstaller);
                        installersList.Add(customEffectPoolsInstaller);
                        Console.WriteLine("Custom effect Pool Installer Added");
                        
                    }
            
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                */

                if (_levelData == null)
                {
                    _levelData = BS_Utils.Plugin.LevelData;
                    if (_levelData == null) return;
                    BS_Utils.Plugin.LevelDidFinishEvent += MainGameSceneSetupDataOnDidFinishEvent;
                }

                if (_spawnController == null)
                {
                    _spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault();

                }


                if (_lastLevelId != _levelData.GameplayCoreSceneSetupData.difficultyBeatmap.level.levelID &&
                    !string.IsNullOrEmpty(_lastLevelId))
                {
                    PlayingNewSong = true;
                   // TimeScale = 1;
                    _lastLevelId = _levelData.GameplayCoreSceneSetupData.difficultyBeatmap.level.levelID;
                }
                else
                {
                    PlayingNewSong = false;
                }


                _lastLevelId = _levelData.GameplayCoreSceneSetupData.difficultyBeatmap.level.levelID;
                _mixer = Resources.FindObjectsOfTypeAll<AudioManagerSO>().FirstOrDefault();
                AudioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
                _songAudio = AudioTimeSync.GetPrivateField<AudioSource>("_audioSource");
                PracticeMode = (_levelData.Mode == BS_Utils.Gameplay.Mode.Standard && _levelData.GameplayCoreSceneSetupData.practiceSettings != null && !BS_Utils.Gameplay.Gamemode.IsIsolatedLevel 
                    && Resources.FindObjectsOfTypeAll<MissionLevelGameplayManager>().FirstOrDefault() == null);


                if (!PracticeMode)
                {
                    _timeScale = Mathf.Clamp(TimeScale, 1, SpeedMaxSize);
                }
                if (PracticeMode)
                {
                    if (_levelData.GameplayCoreSceneSetupData.practiceSettings.songSpeedMul != 1f)
                        _timeScale = _levelData.GameplayCoreSceneSetupData.practiceSettings.songSpeedMul;
                    else
                        _timeScale = _levelData.GameplayCoreSceneSetupData.gameplayModifiers.songSpeedMul;
                    SharedCoroutineStarter.instance.StartCoroutine(DelayedSetup());
                    
                }

            }
        }


        private void ResultsViewController_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (showFailTextNext && showTimeFailed)
            {
                if (failText == null)
                    failText = BeatSaberMarkupLanguage.BeatSaberUI.CreateText(resultsViewController.rectTransform, failTime, new Vector2(15f, -35f));
                else
                   failText.text = failTime;
                failText.richText = true;
            }
            else
            {
                if (failText != null)
                    failText.text = "";
            }
            showFailTextNext = false;
        }

        public System.Collections.IEnumerator DelayedSetup()
        {
            yield return new WaitForSeconds(0.2f);
            try
            {
                Console.WriteLine("Atemmpting Practice Plugin UI");
                var canvas = GameObject.Find("PauseMenu").transform.Find("Wrapper").transform.Find("MenuWrapper").transform.Find("Canvas");

                if (canvas == null)
                {
                    Console.WriteLine("Canvas Null");

                }
                BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "PracticePlugin.PracticeUI.bsml"), canvas.gameObject, PracticeUI.instance);

                var uiObj = new GameObject("PracticePlugin Seeker UI");

                
                _uiElementsCreator = uiObj.AddComponent<UIElementsCreator>();
                _uiElementsCreator.ValueChangedEvent += UIElementsCreatorOnValueChangedEvent;
                _uiElementsCreator.Init();
                uiObj.transform.SetParent(canvas);
             //   CurvedCanvasSettings curvedCanvasSettings = uiObj.AddComponent<CurvedCanvasSettings>();
             //   curvedCanvasSettings.SetRadius(150f);
                uiObj.transform.localScale = new Vector3(1, 1, 1);
                uiObj.transform.localPosition = new Vector3(0, -65f, 0f);

                //  TimeScale = TimeScale;

                //     var bg = GameObject.Find("PauseMenu").transform.Find("Wrapper").transform.Find("UI").transform.Find("BG");
                //      bg.transform.localScale = new Vector3(bg.transform.localScale.x * 1f, bg.transform.localScale.y * 1.2f, bg.transform.localScale.z * 1f);
                //    bg.transform.localPosition = new Vector3(bg.transform.localPosition.x, bg.transform.localPosition.y - 0.35f, bg.transform.localPosition.z);
                //      var pauseMenu = GameObject.Find("PauseMenu");
                //      pauseMenu.transform.localPosition = new Vector3(pauseMenu.transform.localPosition.x, pauseMenu.transform.localPosition.y + 0.175f, pauseMenu.transform.localPosition.z);
                new GameObject("Practice Plugin Behavior").AddComponent<Behavior>();
                if (startWithFullEnergy)
                {
                    GameEnergyCounter energyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
                    if (energyCounter != null)
                        energyCounter.AddEnergy(1 - energyCounter.energy);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void ResultsViewControllerOnContinueButtonPressedEvent(ResultsViewController obj)
        {
            /*
			PersistentSingleton<GameDataModel>.instance.gameDynamicData.GetCurrentPlayerDynamicData()
				.gameplayOptions.noEnergy = false;
                */
        }

        private void MainGameSceneSetupDataOnDidFinishEvent(StandardLevelScenesTransitionSetupDataSO levelData, LevelCompletionResults results)
        {
            if (results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Failed)
            {
                float endTime = results.endSongTime;
                float length = _songAudio.clip.length;
                failTime = $"<#ff0000>Failed At</color> - {Math.Floor(endTime / 60):N0}:{Math.Floor(endTime % 60):00}  /  {Math.Floor(length / 60):N0}:{Math.Floor(length % 60):00}";
                showFailTextNext = true;

            }
            /*
             * 
			if (!NoFail && HasTimeScaleChanged && results != null &&
			    results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared)
			{
				levelData.gameplayCoreSetupData.gameplayModifiers.noFail = true;
				_resetNoFail = true;
			}
            */
        }

        private void UIElementsCreatorOnValueChangedEvent(float timeScale)
        {

            TimeScale = timeScale;
        }

        private static bool IsEqualToOne(float value)
        {
            return Math.Abs(value - 1) < 0.000000001f;
        }

        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
        {

        }


        public void OnFixedUpdate()
        {

        }

        public static void AdjustNJS(float njs)
        {

            float halfJumpDur = 4f;
            float maxHalfJump = _spawnController.GetPrivateField<float>("_maxHalfJumpDistance");
            float noteJumpStartBeatOffset = _levelData.GameplayCoreSceneSetupData.difficultyBeatmap.noteJumpStartBeatOffset;
            float moveSpeed = _spawnController.GetPrivateField<float>("_moveSpeed");
            float moveDir = _spawnController.GetPrivateField<float>("_moveDurationInBeats");
            float jumpDis;
            float spawnAheadTime;
            float moveDis;
            float bpm = _spawnController.GetPrivateField<float>("_beatsPerMinute");
            float num = 60f / bpm;
            moveDis = moveSpeed * num * moveDir;
            while (njs * num * halfJumpDur > maxHalfJump)
            {
                halfJumpDur /= 2f;
            }
            halfJumpDur += noteJumpStartBeatOffset;
            if (halfJumpDur < 1f) halfJumpDur = 1f;
            //        halfJumpDur = spawnController.GetPrivateField<float>("_halfJumpDurationInBeats");
            jumpDis = njs * num * halfJumpDur * 2f;
            spawnAheadTime = moveDis / moveSpeed + jumpDis * 0.5f / njs;
            _spawnController.SetPrivateField("_halfJumpDurationInBeats", halfJumpDur);
            _spawnController.SetPrivateField("_spawnAheadTime", spawnAheadTime);
            _spawnController.SetPrivateField("_jumpDistance", jumpDis);
            _spawnController.SetPrivateField("_noteJumpMovementSpeed", njs);
            _spawnController.SetPrivateField("_moveDistance", moveDis);


        }
        public static void AdjustSpawnOffset(float offset)
        {
            float njs = _spawnController.GetPrivateField<BeatmapObjectSpawnController.InitData>("_initData").noteJumpMovementSpeed;
            float halfJumpDur = 4f;
            float maxHalfJump = _spawnController.GetPrivateField<float>("_maxHalfJumpDistance");
            float noteJumpStartBeatOffset = offset;
            float moveSpeed = _spawnController.GetPrivateField<float>("_moveSpeed");
            float moveDir = _spawnController.GetPrivateField<float>("_moveDurationInBeats");
            float jumpDis;
            float spawnAheadTime;
            float moveDis;
            float bpm = _spawnController.GetPrivateField<float>("_beatsPerMinute");
            float num = 60f / bpm;
            moveDis = moveSpeed * num * moveDir;
            while (njs * num * halfJumpDur > maxHalfJump)
            {
                halfJumpDur /= 2f;
            }
            halfJumpDur += noteJumpStartBeatOffset;
            if (halfJumpDur < 1f) halfJumpDur = 1f;
            //        halfJumpDur = spawnController.GetPrivateField<float>("_halfJumpDurationInBeats");
            jumpDis = njs * num * halfJumpDur * 2f;
            spawnAheadTime = moveDis / moveSpeed + jumpDis * 0.5f / njs;
            _spawnController.SetPrivateField("_halfJumpDurationInBeats", halfJumpDur);
            _spawnController.SetPrivateField("_spawnAheadTime", spawnAheadTime);
            _spawnController.SetPrivateField("_jumpDistance", jumpDis);
            _spawnController.SetPrivateField("_noteJumpMovementSpeed", njs);
            _spawnController.SetPrivateField("_moveDistance", moveDis);


        }
        public static void AdjustNjsAndOffset()
        {
            float njs = UIElementsCreator.currentNJS;
            float noteJumpStartBeatOffset = UIElementsCreator.currentSpawnOffset;
            float halfJumpDur = 4f;
            float maxHalfJump = _spawnController.GetPrivateField<float>("_maxHalfJumpDistance");
            float moveSpeed = _spawnController.GetPrivateField<float>("_moveSpeed");
            float moveDir = _spawnController.GetPrivateField<float>("_moveDurationInBeats");
            float jumpDis;
            float spawnAheadTime;
            float moveDis;
            float bpm = _spawnController.GetPrivateField<float>("_beatsPerMinute");
            float num = 60f / bpm;
            moveDis = moveSpeed * num * moveDir;
            while (njs * num * halfJumpDur > maxHalfJump)
            {
                halfJumpDur /= 2f;
            }
            halfJumpDur += noteJumpStartBeatOffset;
            if (halfJumpDur < 1f) halfJumpDur = 1f;
            //        halfJumpDur = spawnController.GetPrivateField<float>("_halfJumpDurationInBeats");
            jumpDis = njs * num * halfJumpDur * 2f;
            spawnAheadTime = moveDis / moveSpeed + jumpDis * 0.5f / njs;
            _spawnController.SetPrivateField("_halfJumpDurationInBeats", halfJumpDur);
            _spawnController.SetPrivateField("_spawnAheadTime", spawnAheadTime);
            _spawnController.SetPrivateField("_jumpDistance", jumpDis);
         //   _spawnController.SetPrivateField("_noteJumpMovementSpeed", njs);
            _spawnController.SetPrivateField("_moveDistance", moveDis);


        }

        public static void UpdateSpawnMovementData(float njs, float noteJumpStartBeatOffset)
        {
            BeatmapObjectSpawnMovementData spawnMovementData =
    _spawnController.GetPrivateField<BeatmapObjectSpawnMovementData>("_beatmapObjectSpawnMovementData");

            float bpm = _spawnController.GetPrivateField<VariableBpmProcessor>("_variableBpmProcessor").currentBpm;

            
            if (adjustNJSWithSpeed)
            {
                float newNJS = njs * (1 / TimeScale);
                njs = newNJS; 
            }



            spawnMovementData.SetPrivateField("_startNoteJumpMovementSpeed", njs);
            spawnMovementData.SetPrivateField("_noteJumpStartBeatOffset", noteJumpStartBeatOffset);

            spawnMovementData.Update(bpm,
                _spawnController.GetPrivateField<float>("_jumpOffsetY"));


        }
        public void OnSceneUnloaded(Scene scene)
        {
        }

        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition)
        {
            return CreateText(parent, text, anchoredPosition, new Vector2(60f, 10f));
        }

        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            GameObject gameObj = new GameObject("CustomUIText");
            gameObj.SetActive(false);

            TextMeshProUGUI textMesh = gameObj.AddComponent<TextMeshProUGUI>();
            textMesh.font = UnityEngine.Object.Instantiate(Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(t => t.name == "Teko-Medium SDF No Glow"));
            textMesh.rectTransform.SetParent(parent, false);
            textMesh.text = text;
            textMesh.fontSize = 4;
            textMesh.color = Color.white;

            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.sizeDelta = sizeDelta;
            textMesh.rectTransform.anchoredPosition = anchoredPosition;

            gameObj.SetActive(true);
            return textMesh;
        }
    }
}