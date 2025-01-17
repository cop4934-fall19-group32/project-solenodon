﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

	private Dictionary<string, ControllableUIElement> ControllableUIElements;
	private GameObject blurPanel;
	private static int duplicateCounter = 1;

	// Scans scene hierarchy for controllable child components
	private void Start()
	{
		ScanForControls();
	}

	public void ScanForControls() 
	{
		ControllableUIElements = new Dictionary<string, ControllableUIElement>();
		var controlsList = FindObjectsOfType<ControllableUIElement>();
		foreach (var control in controlsList) {
			if (ControllableUIElements.ContainsKey(control.ElementName)) {
				Debug.LogWarning("Error - UIController encountered duplicate control name: " + control.ElementName);
				control.ElementName += duplicateCounter++;
			}

			ControllableUIElements.Add(control.ElementName, control);
		}
	}

	private void Awake()
	{
		blurPanel = GameObject.FindWithTag("Blur");
		if (blurPanel == null)
		{
			Debug.LogError("Error - UIController could not find game object with tag \"Blur\"");
		}
		else
		{
			blurPanel.gameObject.SetActive(false);
			blurPanel.gameObject.GetComponent<Canvas>().enabled = true;
		}
	}

	/**
	 * Enables a ControllableUIElement with the matching name
	 * @return Whether the element was found and enabled
	 */
	public bool EnableUIElement(string elementName)
	{
		if (!ControllableUIElements.ContainsKey(elementName))
		{
			Debug.LogWarning("Element \"" + elementName + "\" not found. Enable failed");
			return false;
		}

		ControllableUIElements[elementName].Enable();
		return true;
	}

	public void EnableAll()
	{
		foreach (var entry in ControllableUIElements)
		{
			// entry.Value.Enable();
			EnableUIElement(entry.Key);
		}
	}

	/**
	 * Disables a ControllableUIElement with the matching name
	 * @return Whether the element was found and disabled
	 */
	public bool DisableUIElement(string elementName)
	{
		if (!ControllableUIElements.ContainsKey(elementName))
		{
			Debug.LogWarning("Element \"" + elementName + "\" not found. Disable failed");
			return false;
		}

		ControllableUIElements[elementName].Disable();
		return true;
	}

	public void DisableAll()
	{
		foreach (var entry in ControllableUIElements)
		{
			// entry.Value.Disable();
			DisableUIElement(entry.Key);
		}
	}

	public bool ShowUIElement(string elementName)
	{
		if (!ControllableUIElements.ContainsKey(elementName))
		{
			Debug.LogWarning("Element \"" + elementName + "\" not found. Show failed");
			return false;
		}

		ControllableUIElements[elementName].gameObject.SetActive(true);
		return true;
	}
	public bool HideUIElement(string elementName)
	{
		if (!ControllableUIElements.ContainsKey(elementName))
		{
			Debug.LogWarning("Element \"" + elementName + "\" not found. Hide failed");
			return false;
		}

		ControllableUIElements[elementName].gameObject.SetActive(false);
		return true;
	}

	// Focus on a specific element, leaving the rest blurred out and dimmed.
	// Multiple elements can be focused at the same time, but FocusUIElement must be called for each one.
	public bool FocusUIElement(string elementName)
	{
		Debug.Log("Focusing Element: [" + elementName + "]");
		if (!ControllableUIElements.ContainsKey(elementName))
		{
			Debug.LogError("Element \"" + elementName + "\" not found. FocusUIElement failed");
			return false;
		}
		blurPanel.SetActive(true);

		ControllableUIElements[elementName].Focus();

		return true;
	}

	public bool FocusUIElement(ControllableUIElement element) {
		blurPanel.SetActive(true);
		element.Focus();
		return true;
	}

	public bool UnfocusUIElement(string elementName)
	{
		if (!ControllableUIElements.ContainsKey(elementName))
		{
			Debug.LogWarning("Element \"" + elementName + "\" not found. UnfocusUIElement failed");
			return false;
		}

		ControllableUIElements[elementName].Unfocus();

		return true;
	}

	public void Blur()
	{
		blurPanel.SetActive(true);
	}

	// Reset so no elements are focused or blurred.
	public void ClearFocus()
	{
		blurPanel.SetActive(false);
		foreach (var entry in ControllableUIElements)
		{
			entry.Value.Unfocus();
		}
	}

	public bool HighlightUIElement(string elementName)
	{
		if (!ControllableUIElements.ContainsKey(elementName))
		{
			Debug.LogWarning("Element \"" + elementName + "\" not found. Highlight failed");
			return false;
		}

		ControllableUIElements[elementName].StartHighlight();

		return true;
	}


	public bool StopHighlightUIElement(string elementName)
	{
		if (!ControllableUIElements.ContainsKey(elementName))
		{
			Debug.LogWarning("Element \"" + elementName + "\" not found. Highlight termination failed");
			return false;
		}

		ControllableUIElements[elementName].StopHighlight();

		return true;
	}

	public void StopAllHighlights() {
		foreach (var element in ControllableUIElements) {
			element.Value.StopHighlight();
		}
	}

	public GameObject GetElement(string elementName)
	{
		if (!ControllableUIElements.ContainsKey(elementName))
		{
			return null;
		}
		return ControllableUIElements[elementName].gameObject;
	}

	public void RemoveEntry(ControllableUIElement element) {
		//0(n) becuase UIControls do not accurately know their own key
		foreach (var entry in ControllableUIElements) {
			if (entry.Value == element) {
				ControllableUIElements.Remove(entry.Key);
				break;
			}
		}
	}

	public ControllableUIElement GetControllableUIElement(string name) {
		return ControllableUIElements[name];
	}

}
