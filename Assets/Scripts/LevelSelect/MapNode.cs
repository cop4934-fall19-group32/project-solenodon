﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * @class MapNode
 * @brief Script to represent operations of a MapNode in the level selection scene
 */
public class MapNode : MonoBehaviour, IPointerClickHandler 
{
	[Header("Display")]
	public Sprite LockedNode;
	public Sprite UnlockedNode;
	public Sprite CompletedNode;
	public SpriteRenderer Renderer;
	public ParticleSystem ParticleEmmiter;

	/** Per node options to control node representation */
	[Header("Options")]
	public int ScoreRequired;
    public bool IsJunction;

	/** Member variables to allow neigbor specificaiton in editor */
	[Header("Neighbors")] //
	public MapNode Next;
    public MapNode North;
    public MapNode South;
    public MapNode East;
    public MapNode West;

	/** Captures neighbors as adjacency list to make pathfinding algorithms easier */
	public List<MapNode> AdjacencyList { get; private set; }

	public bool Locked { get; set; }

    // Configure all Per-node data and display options before other scripts need access
    void Awake() {
		var playerState = FindObjectOfType<PlayerState>();
		FindObjectOfType<MapController>().RegisterMapNode(gameObject);

		//Disable development sprite visible in the editor
		if (IsJunction) {
			Renderer.enabled = false;
		}

		if (IsJunction && ScoreRequired != 0) {
			Debug.LogWarning("Junctions do not have score requirements. Editor value discarded");
			ScoreRequired = 0;
		}

		//Construct an adjacency list from editor neighbor settings
		//This list is used in path finding algorithms in place of the individual variables
		AdjacencyList = new List<MapNode>();
		if (North) {
			AdjacencyList.Add(North);
		}
		if (South) {
			AdjacencyList.Add(South);
		}
		if (East) {
			AdjacencyList.Add(East);
		}
		if (West) {
			AdjacencyList.Add(West);
		} 

		//Lines between nodes are drawn at initialization
		DrawLevelPaths();
		
		bool solved = playerState.GetPuzzleCompleted(gameObject.name);
		if (playerState.GetScore() < ScoreRequired) {
			Locked = true;

			ParticleSystem.MainModule ma = ParticleEmmiter.main;

			var gradient = new ParticleSystem.MinMaxGradient(
				new Color(0.25f, 0.25f, 0.25f),
				new Color(1, 0, 0)
			);

			gradient.mode = ParticleSystemGradientMode.TwoColors;

			ma.startColor = gradient;
			Renderer.sprite = LockedNode;
		}
		else if (solved) {
			// This level is solved. Color it green on the map.
			ParticleSystem.MainModule ma = ParticleEmmiter.main;

			var gradient = new ParticleSystem.MinMaxGradient(
				new Color(0.25f, 0.25f, 0.25f),
				new Color(1, 0.77f, 0.04f)
			);

			gradient.mode = ParticleSystemGradientMode.TwoColors;

			ma.startColor = gradient;
			Renderer.sprite = CompletedNode;
		}
		else {
			Renderer.sprite = UnlockedNode;
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	/**
	 * OnClick event handler to trigger Computron pathfinding in LevelSelect scene
	 * @param data Information about the click event
	 */
	public void OnPointerClick(PointerEventData data) {
		if (transform.parent == null) {
			Debug.LogError("Pin not contained in map, clicking disabled.");
			return;
		}

		var map = transform.parent.gameObject;
		if (!map) {
			Debug.LogError("MapNode does not have a parent map. Click movement will not work.");
			return;	
		}
		
		var mapcontroller = map.GetComponent<MapController>();
		if (!mapcontroller) {
			Debug.LogError("Map does not have a MapController. How did you manage that?");
			return;
		}

        GetComponent<AudioCue>().Play();
		mapcontroller.ReportNodeSelection(this);
	}

	/**
	 * Draws debug adjacency lines between map nodes at edit times
	 * @note Function cannot utilize AdjacencyList, as it is not initialized until runtime
	 */
	private void OnDrawGizmos() {
        if (North != null) {
            DrawDebugLine(North);
        }
        if (South != null) {
            DrawDebugLine(South);
        }
        if (East != null) {
            DrawDebugLine(East);
        }
        if (West != null) { 
            DrawDebugLine(West);
        }
    }

	/**
	 * Draws lines between connected nodes at runtime
	 */
	private void DrawLevelPaths() {
		var lineRenderer = GetComponent<LineRenderer>();
		int positionIndex = 0;

		if (!lineRenderer) {
			return;
		}

		lineRenderer.enabled = true;

		//Drawing only north and east edges still ensures every node appears connected
		if (North) {
			lineRenderer.positionCount += 3;
			lineRenderer.SetPosition(positionIndex++, transform.position);
			lineRenderer.SetPosition(positionIndex++, North.transform.position);
			lineRenderer.SetPosition(positionIndex++, transform.position);
		}
		if (East) {
			lineRenderer.positionCount += 3;
			lineRenderer.SetPosition(positionIndex++, transform.position);
			lineRenderer.SetPosition(positionIndex++, East.transform.position);
			lineRenderer.SetPosition(positionIndex++, transform.position);
		}
	}

	/**
	 * Helper function to draw a debug line between this and target pin
	 * @param pin The target pin
	 */
    private void DrawDebugLine(MapNode pin) {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, pin.transform.position);
    }

}
