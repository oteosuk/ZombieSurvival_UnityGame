﻿using UnityEngine;
using System.Collections;

public class PrPickupObject : MonoBehaviour {
   
      
    [HideInInspector]
    public GameObject Player;

    public Renderer MeshSelector;

    [HideInInspector]
    public bool ShowSelectorAlways = false;
    //[HideInInspector]
    public PrPlayerSettings ColorSetup;
    private bool activeColor = false;

    [Header("HUD")]
    public bool showText = false;
    private UnityEngine.UI.Text UseText;

    [Header("VFX")]
    public Vector3 vfxPositionOffset = Vector3.zero;
    public GameObject spawnVFX;
    private GameObject currentSpawnVFX;
    public GameObject pickupVFX;
    private GameObject currentPickupVFX;

    [HideInInspector]
    public string itemName = "item";
    [HideInInspector]
    public string[] weaponNames;

    private bool initialized = false;
    // Use this for initialization
    void Start()
    {
        gameObject.tag ="Pickup";
        if (MeshSelector)
        {
            if (ColorSetup)
            {
                ShowSelectorAlways = ColorSetup.AlwaysShowPickups;
                ChangeColor();
            }

            MeshSelector.enabled = ShowSelectorAlways;
        }

        UseText = GetComponentInChildren<UnityEngine.UI.Text>() as UnityEngine.UI.Text;
        
        if (currentSpawnVFX)
        {
            
            // currentSpawnVFX.name = "spawnFX";
            // currentSpawnVFX.transform.SetParent(transform);
            // currentSpawnVFX.transform.position = (transform.position + vfxPositionOffset);
        }

    }

    private void InitializeVFX(GameObject VFX, string name)
    {
        VFX.transform.SetParent(transform);
        VFX.name = name;
        VFX.transform.position = transform.position + vfxPositionOffset;
        VFX.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
	
	}

    protected virtual void SetName()
    {
        // set Name
    }

    public virtual void Initialize()
    {
        if (!initialized)
        {
            initialized = true;
            
            SetName();

            if (MeshSelector)
            {
                if (ColorSetup)
                {
                    ShowSelectorAlways = ColorSetup.AlwaysShowPickups;
                    ChangeColor();
                }

                MeshSelector.enabled = ShowSelectorAlways;
            }

            if (ColorSetup)
            {
                UseText = GetComponentInChildren<UnityEngine.UI.Text>() as UnityEngine.UI.Text;

                if (UseText)
                {
                    SetName();
                    showText = ColorSetup.showPickupText;
                    UseText.text = itemName;
                    UseText.color = ColorSetup.PickupTextColor;
                    UseText.lineSpacing = 1f;
                    UseText.enabled = false;
                }
                else
                {
                    // Debug.Log("No Text Found");
                }
            }

            // Initialize VFX
            if (spawnVFX)
            {
                currentSpawnVFX = Instantiate(spawnVFX);
                InitializeVFX(currentSpawnVFX, "SpawnFX");
                currentSpawnVFX.SetActive(true);
            }
            if (pickupVFX)
            {
                currentPickupVFX = Instantiate(pickupVFX);
                InitializeVFX(currentPickupVFX, "PickupFX");
            }
        }
    }

    protected virtual void ChangeColor()
    {
        if (ColorSetup && activeColor)
        {
            MeshSelector.material.SetColor("_TintColor", ColorSetup.ActivePickupColor);
            if (showText && UseText)
            {
                UseText.enabled = true;
            }
        }
            
        else if (ColorSetup && !activeColor)
        {
            MeshSelector.material.SetColor("_TintColor", ColorSetup.InactivePickupColor);
            if (showText && UseText)
            {
                UseText.enabled = false;
            }
        }
    
    }

    protected virtual void PickupObjectNow(int ActiveWeapon)
    {
        SendMessageUpwards("TargetPickedUp", SendMessageOptions.DontRequireReceiver);
        SendMessageUpwards("CollectablePickup", SendMessageOptions.DontRequireReceiver);
        
        if (currentPickupVFX)
        {
            currentPickupVFX.transform.SetParent(null);
            currentPickupVFX.SetActive(true);
        }

        Destroy(gameObject);
        
        
    }
    void OnTriggerStay(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            if (MeshSelector && !activeColor)
            {
                MeshSelector.enabled = true;
                activeColor = true;
                ChangeColor();

            }
            if (other.gameObject.GetComponent<PrActorUtils>())
                Player = other.gameObject.GetComponent<PrActorUtils>().character.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (MeshSelector)
            {
                MeshSelector.enabled = false;
                activeColor = false;
                ChangeColor();
            }
            Player = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnVFX || pickupVFX)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(transform.position + vfxPositionOffset, Vector3.one * 0.4f);
        }
    }

}
