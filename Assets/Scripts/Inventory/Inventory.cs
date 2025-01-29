using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    public int collectablesInventorySize;
    public int weaponInventorySize;
    public static Inventory instance { get; private set; }

    private void Awake()
    private void Awake()
    {
        if (instance != null && instance != this)
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
        DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
    }

    public List<CollectableItem> collectableItems = new List<CollectableItem>();
    public List<Weapon> weapons = new List<Weapon>();
    public delegate void OnItemChanged();
    public delegate void OnWeaponChanged();
    public List<CollectableItem> collectableItems = new List<CollectableItem>();
    public List<Weapon> weapons = new List<Weapon>();
    public delegate void OnItemChanged();
    public delegate void OnWeaponChanged();
    public OnItemChanged onCollectableItemChangedCallback;
    public OnWeaponChanged onWeaponChangedCallback;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Invoke the callbacks to update the UI
        if (onCollectableItemChangedCallback != null)
            onCollectableItemChangedCallback.Invoke();

        if (onWeaponChangedCallback != null)
        {
            onWeaponChangedCallback.Invoke();
        }

    }

    public bool AddWeapon(Weapon weapon)
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Invoke the callbacks to update the UI
        if (onCollectableItemChangedCallback != null)
            onCollectableItemChangedCallback.Invoke();

        if (onWeaponChangedCallback != null)
        {
            onWeaponChangedCallback.Invoke();
        }

    }

    public bool AddWeapon(Weapon weapon)
    {
        Debug.Log ("2");
        if (weapons.Count >= weaponInventorySize) return false;

        weapons.Add(weapon);

        if (onWeaponChangedCallback != null) onWeaponChangedCallback.Invoke();
        return true;
    }

    public void RemoveWeapon(Weapon weapon)

    public void RemoveWeapon(Weapon weapon)
    {
        weapons.Remove(weapon);
        if (onWeaponChangedCallback != null) onWeaponChangedCallback.Invoke();
        if (onWeaponChangedCallback != null) onWeaponChangedCallback.Invoke();
    }

    public bool AddCollectable(CollectableItem item)
    public bool AddCollectable(CollectableItem item)
    {
        if (collectableItems.Count >= collectablesInventorySize) return false;

        collectableItems.Add(item);

        if (onCollectableItemChangedCallback != null) onCollectableItemChangedCallback.Invoke();
        return true;
    }

    public void RemoveCollectable(CollectableItem item)
    public void RemoveCollectable(CollectableItem item)
    {
        collectableItems.Remove(item);
        if (onCollectableItemChangedCallback != null) onCollectableItemChangedCallback.Invoke();
        if (onCollectableItemChangedCallback != null) onCollectableItemChangedCallback.Invoke();
    }
}