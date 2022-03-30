using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombButton : MonoBehaviour
{
    private Toggle toggle;
    private bool selected = false;
    private bool interactable = false;
    private Vector3 startScale;

    public Image outline;
    void Start()
    {
        toggle = GetComponent<Toggle>();
        //setEnabled(false);
    }

    private void Awake()
    {
        startScale = transform.localScale;
    }

    private void Update()
    {
        selected = toggle.isOn;
        toggle.interactable = interactable;

        if (interactable)
            outline.enabled = !toggle.isOn;
    }

    public void setEnabled(bool state) {
        interactable = state;
        outline.enabled = state;
        if (state == false)
            toggle.isOn = state;
    }

    public void changeScale()
    {
        if (toggle.isOn)
            transform.localScale -= new Vector3(0.5f, 0.5f);
        else
            transform.localScale = startScale;
    }

    public bool getSelected() { return selected; }
    public bool getEnabled() { return interactable; }
}

