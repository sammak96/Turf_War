// Scripts/ScriptableObjects/Base/BaseGameDataSO.cs
using UnityEngine;

// This is a base class for all our game data ScriptableObjects.
// It provides common properties like a unique ID and a display name.
public abstract class BaseGameDataSO : ScriptableObject
{
    // The attribute exposes the private backing field
    // of the auto-property in the Unity Inspector, allowing it to be set there.
    // 'public string Id { get; private set; }' defines an auto-property
    // that can be read publicly but only set internally within the class.
    public string Id { get; private set; } // Unique identifier for this data asset
    public string DisplayName { get; private set; } // Name to show in UI

    // OnValidate is a Unity callback method invoked when the script is loaded
    // or a value is changed in the Inspector. It's useful for validation.
    // Here, it ensures that if the 'Id' field is left empty, it automatically
    // defaults to the asset's file name, promoting data consistency.
    protected virtual void OnValidate()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = name; // Auto-assign name as ID if not set
        }
    }
}