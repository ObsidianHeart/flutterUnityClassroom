using UnityEngine;

public class ObjectSelection : MonoBehaviour
{
    public LayerMask customizableLayer; // Assign a layer to your customizable objects
    public CustomizationMenu customizationMenu; // Reference to the CustomizationMenu script
    private CustomizableObject currentlySelectedObject; // Track the currently selected object

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, customizableLayer))
            {
                // An object on the customizableLayer was clicked
                CustomizableObject newSelection = hit.collider.GetComponent<CustomizableObject>();
                if (newSelection != null)
                {
                    // If a new object is selected, unhighlight the previous one
                    if (currentlySelectedObject != null && currentlySelectedObject != newSelection)
                    {
                        UnhighlightObject(currentlySelectedObject);
                    }
                    currentlySelectedObject = newSelection;
                    HighlightObject(currentlySelectedObject);
                    customizationMenu.ShowMenu(currentlySelectedObject);
                }
            }
            else
            {
                // Clicked outside of a customizable object, hide the menu and unhighlight
                if (currentlySelectedObject != null)
                {
                    UnhighlightObject(currentlySelectedObject);
                    currentlySelectedObject = null;
                }
                customizationMenu.HideMenu();
            }
        }
    }

    public void OnPrefabSwapped(CustomizableObject oldObject, CustomizableObject newObject)
    {
        if (currentlySelectedObject == oldObject)
        {
            UnhighlightObject(oldObject);
            currentlySelectedObject = newObject;
            HighlightObject(newObject);
        }
    }

    void HighlightObject(CustomizableObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Store original material to restore later
            // Ensure we store a copy of the material if it's shared, to avoid modifying original asset
            obj.originalMaterial = renderer.material; 
            
            // Create a new material for highlighting (e.g., emissive material)
            Material highlightMaterial = new Material(Shader.Find("Standard"));
            highlightMaterial.color = Color.yellow; // Or any highlight color
            highlightMaterial.EnableKeyword("_EMISSION");
            highlightMaterial.SetColor("_EmissionColor", Color.yellow * 0.5f); // Adjust intensity
            renderer.material = highlightMaterial;
        }
    }

    void UnhighlightObject(CustomizableObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && obj.originalMaterial != null)
        {
            renderer.material = obj.originalMaterial;
            obj.originalMaterial = null; // Clear the reference
        }
    }
}


