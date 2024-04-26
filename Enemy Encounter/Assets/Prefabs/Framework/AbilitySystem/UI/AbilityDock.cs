using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// RESPONSIBLE FOR KNOWING WHAT ABILITY IS ADDED, AND THEN INSTANCIATING THAT ABILITY UI AND POPULATING THE ICON, AND WHEN THE ABILITY IS CASTED IT WILL SHOW THE COOLDOWN
// THIS CLASS SHOULD BE ATTACHED TO THE AbilityDock Prefab
public class AbilityDock : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] AbilityComponent abilityComponent;
    [SerializeField] RectTransform Root;
    [SerializeField] VerticalLayoutGroup LayoutGrp;
    [SerializeField] AbilityUI AbilityUIPrefab;

    [SerializeField] float ScaleRange = 200f;

    [SerializeField] float highlightSize = 1.5f;
    [SerializeField] float ScaleSpeed = 20f;

    Vector3 GoalScale = Vector3.one;

    List<AbilityUI> abilityUIs = new List<AbilityUI>();

    PointerEventData touchData;
    AbilityUI hightlightedAbility;
    private void Awake()
    {
        abilityComponent.onNewAbilityAdded += AddAbility;
    }

    private void AddAbility(Ability newAbility) // WHEN A NEW ABILITY IS ADDED, THIS FUNCTION IS CALLED TO ADD THE UI OF THAT NEW ABILITY
    {
        AbilityUI newAbilityUI = Instantiate(AbilityUIPrefab, Root);
        newAbilityUI.Init(newAbility); // LET THE AbilityUI KNOW WHOS THE NEW ADDED ABILITY
        abilityUIs.Add(newAbilityUI);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(touchData!=null)
        {
            GetUIUnderPointer(touchData, out hightlightedAbility);
            ArrangeScale(touchData);
        }

        transform.localScale = Vector3.Lerp(transform.localScale, GoalScale, Time.deltaTime * ScaleSpeed);
    }

    private void ArrangeScale(PointerEventData touchData)
    {
        if (ScaleRange == 0) return;

        float touchVerticalPos = touchData.position.y;
        foreach(AbilityUI abilityUI in abilityUIs)
        {
            float abilityUIVerticalPos = abilityUI.transform.position.y;
            float distance = Mathf.Abs(touchVerticalPos - abilityUIVerticalPos);

            if(distance > ScaleRange)
            {
                abilityUI.SetScaleAmt(0);
                continue;
            }

            float scaleAmt = (ScaleRange - distance) / ScaleRange;
            abilityUI.SetScaleAmt(scaleAmt);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchData = eventData;
        GoalScale = Vector3.one * highlightSize;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(hightlightedAbility)
        {
            hightlightedAbility.ActivateAbility();
        }
        touchData = null;
        ResetScale();
        GoalScale = Vector3.one;
    }

    private void ResetScale()
    {
        foreach(AbilityUI abilityUI in abilityUIs)
        {
            abilityUI.SetScaleAmt(0);
        }
    }

    bool GetUIUnderPointer(PointerEventData eventData, out AbilityUI abilityUI)
    {
        List<RaycastResult> findAbility = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, findAbility);

        abilityUI = null;
        foreach(RaycastResult result in findAbility)
        {
            abilityUI = result.gameObject.GetComponentInParent<AbilityUI>();
            if (abilityUI != null)
                return true;
        }

        return false;
    }
}
