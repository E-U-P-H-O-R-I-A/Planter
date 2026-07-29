using System;
using System.Collections;
using Data;
using Services.PrivateModelProvider;
using Services.PublicModelProvider;
using Services.RewardService;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Game.Level
{
    public enum PotStates
    {
        Empty,
        Sprout,
        Grew,
    }

    public class Pot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject timer;
        [SerializeField] private TextMeshProUGUI timerText;
        [Space] 
        [SerializeField] private SpriteRenderer spriteRenderer;

        private IPrivateModelProvider privateModelProvider;
        private IPublicModelProvider publicModelProvider;
        private PlantPublicModel plantPublicModel;
        private PotPrivateModel potPrivateModel;
        private IRewardService rewardService;

        private Coroutine timerCoroutine;
        private PotStates currentState;
        private string id;

        public bool IsEmpty => currentState == PotStates.Empty;

        [Inject]
        public void Construct(IPublicModelProvider publicModelProvider, IPrivateModelProvider privateModelProvider, 
            IRewardService rewardService)
        {
            this.rewardService = rewardService;
            this.privateModelProvider = privateModelProvider;
            this.publicModelProvider = publicModelProvider;
        }

        public void Initialize(string id)
        {
            this.id = id;

            currentState = PotStates.Empty;

            plantPublicModel = publicModelProvider.GetModel<PlantPublicModel>();
            potPrivateModel = privateModelProvider.GetModel<PotPrivateModel>();

            UpdateState();
        }

        public void Release()
        {
            StopTimer();
            id = string.Empty;

            currentState = PotStates.Empty;
            spriteRenderer.sprite = null;
            timer.SetActive(false);
        }
        
        public bool Plant(string plantID)
        {
              PotPrivateScheme potScheme = GetPotScheme(id);
  
              if (potScheme == null || potScheme.IsPlanted)
                  return false;

            if (GetPlantScheme(plantID) == null)
                return false;

            potScheme.Plant(plantID);
            
            SaveModel();
            UpdateState();
            return true;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (currentState != PotStates.Grew) 
                return;
            
            PotPrivateScheme potScheme = GetPotScheme(id);
            PlantPublicScheme plantScheme = GetPlantScheme(potScheme.PlantID);
            
            rewardService.GetReward(plantScheme.SellReward);

            potScheme.Clear();
            
            SaveModel();
            SetEmptyState();
        }
        
        private PotPrivateScheme GetPotScheme(string id) =>
            potPrivateModel.GetScheme(id);

        private PlantPublicScheme GetPlantScheme(string id) =>
            plantPublicModel.GetScheme(id);

        private void SaveModel() => 
            privateModelProvider.SaveModel<PotPrivateModel>();

        private void StartTimer() =>
            timerCoroutine = StartCoroutine(Timer());

        private void UpdateTimer(TimeSpan time)
        {
            timerText.text = time.TotalMinutes < 1
                ? $"{time.Seconds:00}"
                : $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
        }

        private void StopTimer()
        {
            if (timerCoroutine == null)
                return;

            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        private void UpdateState()
        {
            PotPrivateScheme potScheme = GetPotScheme(id);

            if (potScheme == null || !potScheme.IsPlanted)
            {
                SetEmptyState();
                return;
            }

            if (GetPlantScheme(potScheme.PlantID) == null)
            {
                potScheme.Clear();
                SaveModel();
                SetEmptyState();
                return;
            }

            if (potScheme.PlantedTime == DateTime.MinValue)
            {
                // Migrate old records that did not contain a serializable planting time.
                potScheme.Plant(potScheme.PlantID);
                SaveModel();
            }

            SetSproutState();
        }

        private void SetEmptyState()
        {
            currentState = PotStates.Empty;

            spriteRenderer.sprite = null;
            timer.gameObject.SetActive(false);

            StopTimer();
        }

        private void SetSproutState()
        {
            if (currentState == PotStates.Sprout)
                return;

            currentState = PotStates.Sprout;

            PotPrivateScheme potScheme = GetPotScheme(id);
            PlantPublicScheme plantScheme = GetPlantScheme(potScheme.PlantID);

            if (plantScheme == null)
            {
                SetEmptyState();
                return;
            }

            timer.gameObject.SetActive(true);
            spriteRenderer.sprite = plantScheme.ImageSprout;

            StartTimer();
        }

        private void SetGrewState()
        {
            if (currentState == PotStates.Grew)
                return;

            currentState = PotStates.Grew;
            
            PotPrivateScheme potScheme = GetPotScheme(id);
            PlantPublicScheme plantScheme = GetPlantScheme(potScheme.PlantID);

            timer.gameObject.SetActive(false);
            spriteRenderer.sprite = plantScheme.ImagePlant;

            StopTimer();
        }

        private IEnumerator Timer()
        {
            PotPrivateScheme potScheme = GetPotScheme(id);
            PlantPublicScheme plantScheme = GetPlantScheme(potScheme.PlantID);
            
            DateTime grewTime = potScheme.PlantedTime.AddSeconds(plantScheme.DurationGrow);
            
            while (true)
            {
                TimeSpan time = grewTime - DateTime.UtcNow;

                if (time.TotalSeconds < 0)
                {
                    SetGrewState();
                    yield break;
                }
                
                UpdateTimer(time);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
