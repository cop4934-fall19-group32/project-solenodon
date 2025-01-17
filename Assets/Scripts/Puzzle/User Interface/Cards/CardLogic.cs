﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CardType { 
    REGISTER,
    STACK,
    QUEUE,
    HEAP
}

public class CardLogic : MonoBehaviour {

    public CardType CardType;

    public float Cost;

    public string Description;

    public TMPro.TextMeshProUGUI CostText;

    public List<GameObject> BoundInstructions { get; private set; }

    public CardDatastructure datastructure;

    private TMPro.TextMeshProUGUI PanelText;
    private TMPro.TextMeshProUGUI LabelText;

    private AudioCue InputAudio;
    private AudioCue OutputAudio;

    public string Address { 
        get {
            return LabelText.text; 
        } 
        set {
            LabelText.text = value;
        } 
    }

    public bool CardInPlay {
        get {
            var container = transform.GetComponentInParent<PlayedCardContainer>();

            return container != null;
        }
    }

    private void Awake() {
        PanelText = transform.Find("Panel").GetComponentInChildren<TMPro.TextMeshProUGUI>();
        LabelText = transform.Find("Label").GetComponentInChildren<TMPro.TextMeshProUGUI>();
        BoundInstructions = new List<GameObject>();
        InitializeAudioCues();
    }

    private void InitializeAudioCues() {
        AudioCue[] audioCues = GetComponents<AudioCue>();
        foreach (AudioCue audioCue in audioCues) {
            if (audioCue.Name == "Input Audio") {
                InputAudio = audioCue;
            }
            else if (audioCue.Name == "Output Audio") {
                OutputAudio = audioCue;
            }
        }
    }

    // Start is called before the first frame update
    private void Start() {
        switch (CardType) {
            case CardType.REGISTER:
                datastructure = new RegisterCard();
                break;
            case CardType.STACK:
                datastructure = new StackCard();
                break;
            case CardType.QUEUE:
                datastructure = new QueueCard();
                break;
            case CardType.HEAP:
                datastructure = new HeapCard();
                break;
            default:
                throw new System.Exception();
        }

        CostText.text = Cost.ToString();
    }

    private void Update() {
        PanelText.text = datastructure.Peek().ToString();
    }

    private IEnumerator WaitForUnlink() {

        while (BoundInstructions.Count == 0) {
            yield return null;
        }

        GetComponent<UIControl>().Enable();
        yield break;
    }

    public void LinkInstruction(GameObject instruction) {
        BoundInstructions.Add(instruction);
        GetComponent<UIControl>().Disable();
        StartCoroutine(WaitForUnlink());
    }

    public void UnlinkInstruction(GameObject instruction) {
        //Iterates backwards through array to avoid counter invalidation.
        BoundInstructions.RemoveAll(element => element == instruction);

        if (BoundInstructions.Count == 0) {
            GetComponent<CardUIControl>().Enable();
        }
    }

    public void MoveTo(int num) {
        OutputAudio.Play();
        datastructure.Add(num);
    }

    public int? MoveFrom() {
        InputAudio.Play();
        return datastructure.Remove();
    }

    public void CopyTo(int num) {
        OutputAudio.Play();
        datastructure.Add(num);
    }

    public int? CopyFrom()
    {
        InputAudio.Play();
        return datastructure.Peek();
    }

    public void ClearData() {
        datastructure.ClearData();
    }

    public Vector3 GetWaypoint()
    {
        Vector3[] target = new Vector3[4];
        PanelText.rectTransform.GetWorldCorners(target);
        float x = target[2].x - ((target[2].x - target[1].x) / 2);
        float y = target[1].y + 1;
        float z = target[1].z;
        return new Vector3(x, y, x);
    }
}
