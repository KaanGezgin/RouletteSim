using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core; // GameManager'a eriþim için
using static Util.GeneralItilElements;

public class RouletteWheelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform wheelTransform; // Dönecek olan 3D çark modeli
    [SerializeField] private AnimationCurve spinCurve;  // Hýzlanma/Yavaþlama eðrisi

    [Header("Settings")]
    [SerializeField] private float spinDuration = 5.0f; // Toplam dönüþ süresi 
    [SerializeField] private int fullRotations = 6;    // Durmadan önce atacaðý tam tur sayýsý

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSpinResultDetermined += StartSpinSequence;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSpinResultDetermined -= StartSpinSequence;
        }
    }

    private void StartSpinSequence(int winningNumber)
    {
        StartCoroutine(SpinRoutine(winningNumber));
    }

    private IEnumerator SpinRoutine(int winningNumber)
    {
        Vector3 startRot = wheelTransform.eulerAngles;

        float targetSlotAngle = CalculateAngleForNumber(winningNumber);

        float totalRotation = (360f * fullRotations) + targetSlotAngle;
        Vector3 endRot = new Vector3(startRot.x, startRot.y + totalRotation, startRot.z);

        yield return RotateTo(wheelTransform, startRot, endRot, spinDuration, spinCurve);

        GameManager.Instance.ChangeState(GameState.Result);
    }

    private float CalculateAngleForNumber(int number)
    {
        var slots = GameManager.Instance.GetRouletteData().wheelSlots;

        int index = slots.FindIndex(s => s.number == number);

        if (index == -1) return 0f;

        float anglePerSlot = 360f / slots.Count;
        return index * anglePerSlot;
    }

    private IEnumerator RotateTo(Transform target, Vector3 start, Vector3 end, float duration, AnimationCurve curve)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = curve.Evaluate(elapsed / duration); 
            target.eulerAngles = Vector3.LerpUnclamped(start, end, t);
            yield return null;
        }
        target.eulerAngles = end;
    }
}