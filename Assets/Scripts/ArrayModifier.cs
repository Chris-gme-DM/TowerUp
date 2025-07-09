using UnityEngine;
using System.Collections.Generic; // Make sure to include this for List
#if UNITY_EDITOR
using UnityEditor; // Required for EditorApplication.delayCall
#endif

public class ArrayModifier : MonoBehaviour
{
    [Header("Array Settings")]
    [Tooltip("The prefab to be duplicated.")]
    public GameObject objectToDuplicate;

    [Tooltip("The number of times the object should be duplicated.")]
    [Range(0, 100)] // Limit the range for easier control and to prevent accidental massive duplication
    public int arrayCount = 1;

    [Tooltip("The offset applied to each new duplicate.")]
    public Vector3 offset = new Vector3(1.0f, 0, 0);

    [Tooltip("Check this to automatically update the array in the Editor when values change.")]
    public bool liveUpdateInEditor = true;

    // We keep spawnedObjects for clean tracking in play mode and for a more targeted editor destroy,
    // but the editor cleanup will also aggressively look at children.
    [SerializeField]
    private List<GameObject> spawnedObjects = new List<GameObject>();

    // These values will store the last parameters used for generation in the editor
    // to detect if the settings have changed significantly enough to warrant a full rebuild.
    private int _lastArrayCount = -1;
    private GameObject _lastObjectToDuplicate;
    private Vector3 _lastOffset;

    void Start()
    {
        GenerateArray();
    }

    // OnValidate is called in the Editor when the script is loaded or a value is changed in the Inspector.
    void OnValidate()
    {
        if (!Application.isPlaying && liveUpdateInEditor)
        {
            if (arrayCount != _lastArrayCount ||
                objectToDuplicate != _lastObjectToDuplicate ||
                offset != _lastOffset)
            {
                Debug.Log("ArrayModifier: Settings changed. Triggering regeneration.");
                GenerateArray();

                _lastArrayCount = arrayCount;
                _lastObjectToDuplicate = objectToDuplicate;
                _lastOffset = offset;
            }
        }
    }

    /// <summary>
    /// Generates the array of duplicated objects.
    /// Cleans up existing objects before generating new ones.
    /// </summary>
    public void GenerateArray()
    {
        Debug.Log("ArrayModifier: Generating array. Calling cleanup first.");
        CleanUpArray(); // Call cleanup before generation

        if (objectToDuplicate == null)
        {
            Debug.LogWarning("ArrayModifier: 'Object To Duplicate' prefab is not assigned. Cannot generate array.", this);
            return;
        }

        if (arrayCount <= 0)
        {
            Debug.LogWarning("ArrayModifier: 'Array Count' must be greater than 0. No objects will be generated.", this);
            return;
        }

        // Always clear the list before populating with new objects.
        spawnedObjects.Clear();

        for (int i = 0; i < arrayCount; i++)
        {
            Vector3 spawnPosition = transform.position + (offset * i);
            GameObject newObject = null;

            // --- Attempt to Instantiate, handling potential errors from the prefab/object being cloned ---
            try
            {
                newObject = Instantiate(objectToDuplicate, spawnPosition, transform.rotation);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ArrayModifier: Error during Instantiate for {objectToDuplicate.name} at index {i}: {e.Message}. This object may not be fully initialized or might have problematic scripts. Skipping this particular instance.", this);
                continue; // Skip to the next iteration if instantiation fails
            }

            if (newObject != null)
            {
                // --- Attempt to SetParent, handling potential errors from the new child or parent ---
                try
                {
                    newObject.transform.SetParent(this.transform);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"ArrayModifier: Error during SetParent for {newObject.name} at index {i}: {e.Message}. Object may not be correctly parented in hierarchy. This is often caused by other scripts reacting to parenting changes with problematic methods (e.g., SendMessage in OnValidate).", this);
                }

                newObject.name = objectToDuplicate.name + " (Array " + i + ")";
                spawnedObjects.Add(newObject);
            }
        }
        Debug.Log($"ArrayModifier: Generated {spawnedObjects.Count} objects.");
    }

    /// <summary>
    /// Destroys all previously spawned objects managed by this modifier.
    /// This method is designed to be robust for editor cleanup.
    /// </summary>
    public void CleanUpArray()
    {
        Debug.Log($"ArrayModifier: Starting cleanup. Current children count: {transform.childCount}");

        if (!Application.isPlaying) // Editor mode cleanup
        {
            List<Transform> childrenToDestroy = new List<Transform>();
            foreach (Transform child in transform)
            {
                if (child != null && child.name.Contains("(Array "))
                {
                    childrenToDestroy.Add(child);
                }
            }

            if (childrenToDestroy.Count > 0)
            {
                Debug.Log($"ArrayModifier: Found {childrenToDestroy.Count} children matching pattern for destruction.");
            }

            foreach (Transform child in childrenToDestroy)
            {
                if (child != null && child.gameObject != null)
                {
                    // Defer the DestroyImmediate call to the next editor update cycle.
                    // This is crucial to bypass Unity's restriction on DestroyImmediate during OnValidate.
#if UNITY_EDITOR
                    Transform childToDestroy = child; // Capture child for the lambda
                    EditorApplication.delayCall += () =>
                    {
                        if (childToDestroy != null && childToDestroy.gameObject != null)
                        {
                            Debug.Log($"ArrayModifier: Deferred Destroying (child): {childToDestroy.name}");
                            DestroyImmediate(childToDestroy.gameObject);
                        }
                    };
#endif
                }
            }
        }
        else // Play Mode cleanup
        {
            for (int i = spawnedObjects.Count - 1; i >= 0; i--)
            {
                GameObject obj = spawnedObjects[i];
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }

        spawnedObjects.Clear(); // Always clear the list after attempting to destroy all known/identified objects.

        _lastArrayCount = -1;
        _lastObjectToDuplicate = null;
        _lastOffset = Vector3.zero;
        Debug.Log("ArrayModifier: Cleanup finished.");
    }

    void OnEnable()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("ArrayModifier: OnEnable called in editor. Forcing rebuild.");
            _lastArrayCount = -1;
            GenerateArray();
        }
    }

    void OnDestroy()
    {
        Debug.Log("ArrayModifier: OnDestroy called. Cleaning up.");
        CleanUpArray();
    }
}