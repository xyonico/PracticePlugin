using System.Collections;
using System.Linq;
using UnityEngine;

namespace PracticePlugin
{
	public class NoFailGameEnergy : MonoBehaviour
	{
		private GameEnergyUIPanel _gameEnergyUIPanel;
		private GameEnergyCounter _gameEnergyCounter;
		private Animator _levelFailedAnimator;
		private GameObject _levelFailedGameObject;

		private bool _isFailedVisible;

		private void Awake()
		{
			_gameEnergyUIPanel = Resources.FindObjectsOfTypeAll<GameEnergyUIPanel>().FirstOrDefault();
			if (_gameEnergyUIPanel == null) return;
			_gameEnergyUIPanel.EnableEnergyPanel(true);

			_gameEnergyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
			var levelFailedController = Resources.FindObjectsOfTypeAll<LevelFailedController>().FirstOrDefault();
			if (levelFailedController == null) return;
			var textEffect = levelFailedController.GetPrivateField<LevelFailedTextEffect>("_levelFailedTextEffect");
			_levelFailedAnimator = textEffect.GetPrivateField<Animator>("_animator");
			_levelFailedGameObject = GameObject.Find("LevelFailedTextEffect");
		}

		private void LateUpdate()
		{
			if (_isFailedVisible) return;
			if (_gameEnergyCounter.energy > 1E-05f) return;

			StartCoroutine(LevelFailedRoutine());
		}

		private IEnumerator LevelFailedRoutine()
		{
			_isFailedVisible = true;
			
			_levelFailedGameObject.SetActive(false);
			_levelFailedAnimator.enabled = true;
			yield return new WaitForSeconds(0.1f);
			_levelFailedGameObject.SetActive(true);
			var waitTime = Time.realtimeSinceStartup + 3;
			while (Time.realtimeSinceStartup < waitTime)
			{
				_gameEnergyCounter.AddEnergy(-1);
				yield return null;
			}
			_levelFailedGameObject.SetActive(false);
			
			_gameEnergyCounter.AddEnergy(0.5f);
			_isFailedVisible = false;
		}
	}
}