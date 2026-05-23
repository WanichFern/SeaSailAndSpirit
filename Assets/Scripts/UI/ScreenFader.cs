using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public CanvasGroup fadeGroup;

    void Awake()
    {
        // เริ่มเกมมาให้ปิดการบล็อกเมาส์ไว้ก่อน
        if (fadeGroup != null) fadeGroup.blocksRaycasts = false;
    }

    public IEnumerator Fade(bool fadeIn)
    {
        float speed = 2f;
        float targetAlpha = fadeIn ? 1 : 0;

        // ถ้าจะ Fade เข้า (ไปสู่หน้าจอสีดำ) ให้เริ่มบล็อกการคลิกทันที
        if (fadeIn) fadeGroup.blocksRaycasts = true;

        while (!Mathf.Approximately(fadeGroup.alpha, targetAlpha))
        {
            fadeGroup.alpha = Mathf.MoveTowards(fadeGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }

        // ถ้า Fade ออกจนหมด (โปร่งใส) ให้ปลดล็อกการคลิก
        if (!fadeIn) fadeGroup.blocksRaycasts = false;
    }
}