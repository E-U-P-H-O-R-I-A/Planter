using Data;
using DG.Tweening;
using Services.CurrencyService;
using TMPro;
using UnityEngine;
using VContainer;

namespace Game.UI.Currency
{
    public sealed class CurrencyPanel : MonoBehaviour
    {
        private const int MaxAnimatedAmount = 1000;

        [Header("References")]
        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private CurrencyType currencyType;

        [Header("Animation")]
        [SerializeField] private float countDuration = 0.5f;
        [SerializeField] private float pulseScale = 1.2f;
        [SerializeField] private float pulseDuration = 0.2f;
        [SerializeField] private Ease countEase = Ease.OutQuad;

        private ICurrencyService currencyService;
        private Vector3 startScale;
        private Tween countTween;
        private Tween pulseTween;
        private int currentShowingMoney;
        private int realMoney;

        [Inject]
        public void Construct(ICurrencyService currencyService)
        {
            Release();

            this.currencyService = currencyService;
            startScale = currencyText.transform.localScale;

            currencyService.Changed += OnCurrencyChanged;
            SetCurrencyText(currencyService.GetAmountCurrency(currencyType), true);
        }

        public void Release()
        {
            if (currencyService != null)
                currencyService.Changed -= OnCurrencyChanged;

            currencyService = null;
            CleanupTweens();
        }

        private void OnCurrencyChanged(CurrencyType type, int amount)
        {
            if (type != currencyType)
                return;

            SetCurrencyText(amount, amount < realMoney);
        }

        private void SetCurrencyText(int amount, bool noAnimation)
        {
            if (noAnimation)
            {
                SetCurrencyTextInstantly(amount);
                return;
            }

            if (amount == realMoney && countTween != null && countTween.IsActive())
                return;

            realMoney = amount;

            if (currentShowingMoney == realMoney)
            {
                SetCurrentShowingMoney(realMoney);
                return;
            }

            int startAmount = Mathf.Max(currentShowingMoney, realMoney - MaxAnimatedAmount);
            SetCurrentShowingMoney(startAmount);
            PlayCountTween(realMoney);
            PlayPulseTween();
        }

        private void SetCurrentShowingMoney(int amount)
        {
            currentShowingMoney = amount;
            currencyText.text = FormatAmount(amount);
        }

        private void SetCurrencyTextInstantly(int amount)
        {
            realMoney = amount;
            CleanupTweens();
            SetCurrentShowingMoney(realMoney);
        }

        private void PlayCountTween(int targetAmount)
        {
            countTween?.Kill();
            countTween = DOTween
                .To(() => currentShowingMoney, SetCurrentShowingMoney, targetAmount, countDuration)
                .SetEase(countEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    SetCurrentShowingMoney(targetAmount);
                    countTween = null;
                });
        }

        private void PlayPulseTween()
        {
            pulseTween?.Kill();
            pulseTween = DOTween.Sequence()
                .SetUpdate(true)
                .AppendCallback(() => currencyText.transform.localScale = startScale)
                .Append(currencyText.transform
                    .DOScale(startScale * pulseScale, pulseDuration)
                    .SetEase(Ease.OutQuad)
                    .SetLoops(2, LoopType.Yoyo))
                .OnComplete(() => pulseTween = null);
        }

        private void CleanupTweens()
        {
            countTween?.Kill();
            pulseTween?.Kill();
            countTween = null;
            pulseTween = null;
        }

        private static string FormatAmount(int amount)
        {
            if (amount >= 1_000_000)
                return $"{amount / 1_000_000f:0.#}M";

            if (amount >= 1_000)
                return $"{amount / 1_000f:0.#}K";

            return amount.ToString();
        }

        private void OnDestroy() =>
            Release();
    }
}
