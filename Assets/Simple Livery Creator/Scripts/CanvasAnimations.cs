using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAnimations : MonoBehaviour
{
    int toggle, toggle2,toggle3;
    public GameObject bp, dp,dpp;

    public void BodyPaintShowUp()
    {
        toggle++;
        StartCoroutine(GoUp(toggle));
    }
    public void DecalPaintShowUp()
    {
        toggle2++;
        StartCoroutine(GoDown(toggle2));
    }
    public void DecalPaintPickerShowUp()
    {
        toggle3++;
        StartCoroutine(GoUpPicker(toggle3));
    }
    public float animationSpeed = 0.03f;
    public AnimationCurve motion;

    float lerpAmount1;
    float lerpAmount2;
    IEnumerator GoUp(int toggle)
    {
        var start = bp.GetComponent<RectTransform>().localPosition.y;

        //lerpAmount1+=Time.fixedDeltaTime*10;
        for (float i = 0; i < 1; i+=animationSpeed)
        {
            if (toggle % 2 == 1)
                bp.GetComponent<RectTransform>().localPosition = new Vector2(bp.GetComponent<RectTransform>().localPosition.x, Mathf.Lerp(start, start + 250, motion.Evaluate(i)));
            else
                bp.GetComponent<RectTransform>().localPosition = new Vector2(bp.GetComponent<RectTransform>().localPosition.x, Mathf.Lerp(start, start - 250, motion.Evaluate(i)));

        

        yield return null;
        }

    }
    IEnumerator GoUpPicker(int toggle3)
    {
        var start = dpp.GetComponent<RectTransform>().localPosition.y;

        for (float i = 0; i < 1; i+=animationSpeed)
        {
            if (toggle3 % 2 == 1)
                dpp.GetComponent<RectTransform>().localPosition = new Vector2(dpp.GetComponent<RectTransform>().localPosition.x, Mathf.Lerp(start, start + 200, motion.Evaluate(i)));
            else
                dpp.GetComponent<RectTransform>().localPosition = new Vector2(dpp.GetComponent<RectTransform>().localPosition.x, Mathf.Lerp(start, start - 200, motion.Evaluate(i)));

        

        yield return null;
        }

    }
    IEnumerator GoDown(int toggle2)
    {
        var start = dp.GetComponent<RectTransform>().localPosition.y;


        for (float i = 0; i < 1; i+=animationSpeed)
        {
            lerpAmount2 += Time.fixedDeltaTime * 3;
            if (toggle2 % 2 == 1)
                dp.GetComponent<RectTransform>().localPosition = new Vector2(dp.GetComponent<RectTransform>().localPosition.x,  Mathf.Lerp(start, start - 150, motion.Evaluate(i)));
            else
                dp.GetComponent<RectTransform>().localPosition = new Vector2(dp.GetComponent<RectTransform>().localPosition.x,  Mathf.Lerp(start, start + 150, motion.Evaluate(i)));

            yield return null;
        }

    }
}
